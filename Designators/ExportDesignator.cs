using MorePlanning.Designators;
using MorePlanning.Plan;
using MorePlanning.Utility;
using MorePlanning;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using MorePlanningExport.Data;
using UnityEngine;
using Verse;

namespace MorePlanningExport.Designators
{
    public class ExportDesignator : BaseDesignator
    {
        public override int DraggableDimensions => 2;

        public override bool DragDrawMeasurements => true;

        public ExportDesignator()
            : base("MorePlanningExport.PlanExport".Translate(), "MorePlanningExport.PlanExportDesc".Translate())
        {
            icon = ContentFinder<Texture2D>.Get("MorePlanningExport/PlanExport");
        }
        
        public override void SelectedUpdate()
        {
            VisibilityDesignator.PlanningVisibility = true;
        }

        public override AcceptanceReport CanDesignateCell(IntVec3 c)
        {
            if (!c.InBounds(Map))
                return false;
            return c.InNoBuildEdgeArea(Map)
                ? "TooCloseToMapEdge".Translate()
                : MapUtility.HasAnyPlanDesignationAt(c, Map)
                    ? AcceptanceReport.WasAccepted
                    : AcceptanceReport.WasRejected;
        }

        public override void RenderHighlight(List<IntVec3> dragCells) =>
            DesignatorUtility.RenderHighlightOverSelectableCells(this, dragCells);

        public override void DesignateMultiCell(IEnumerable<IntVec3> cells)
        {
            var list = cells
                .Select(cell => MapUtility.GetPlanDesignationAt(cell, this.Map))
                .Where((designation => designation != null))
                .ToList();
            cells = list.Select(plan => plan.target.Cell);
            if (list.Count == 0)
            {
                Messages.Message("MorePlanning.MissingPlanningDesignations".Translate(), MessageTypeDefOf.RejectInput);
            }
            else
            {
                var intVec3S = cells.ToList();
                var num1 = intVec3S.Min((cell => cell.x));
                var num2 = intVec3S.Max((cell => cell.z));
                var num3 = intVec3S.Max((cell => cell.x));
                var num4 = intVec3S.Min((cell => cell.z));
                var intVec2 = new IntVec2((int)Math.Floor(UI.MouseMapPosition().x),
                    (int)Math.Floor(UI.MouseMapPosition().z));
                if (intVec2.x < num1)
                    intVec2.x = num1;
                else if (intVec2.x > num1)
                    intVec2.x = num3;
                if (intVec2.z > num2)
                    intVec2.z = num2;
                else if (intVec2.z < num4)
                    intVec2.z = num4;
                var x = intVec2.x;
                var z = intVec2.z;
                var planDesignationInfo = (
                    from designation in list
                    let planDesignation = designation as PlanDesignation
                    select new PlanInfo()
                    {
                        Color = planDesignation?.Color ?? 0,
                        Pos = new IntVec3((designation.target).Cell.x - x, (designation.target).Cell.y,
                            (designation.target).Cell.z - z)
                    }
                ).ToList();

                var morePlanningData = new MorePlanningData(planDesignationInfo);
                var dialogName = new DialogName(morePlanningData, () =>
                {
                    MorePlanningExport.SaveToXML(morePlanningData);
                    MorePlanningExport.MorePlannings.Add(morePlanningData);
                    Messages.Message("MorePlanningExport.ExportSuccess".Translate(), MessageTypeDefOf.RejectInput);
                }, () =>
                {
                    Messages.Message("MorePlanningExport.ExportCancel".Translate(), MessageTypeDefOf.RejectInput);
                });
                Find.WindowStack.Add(dialogName);
            }
        }
    }
}