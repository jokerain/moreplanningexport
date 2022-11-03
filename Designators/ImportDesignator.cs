using MorePlanning.Designators;
using MorePlanning.Plan;
using System.Linq;
using UnityEngine;
using Verse;

namespace MorePlanningExport.Designators
{
    public class ImportDesignator : BaseDesignator
    {
        public ImportDesignator()
            : base("MorePlanningExport.PlanImport".Translate(), "MorePlanningExport.PlanImportDesc".Translate())
        {
            icon = ContentFinder<Texture2D>.Get("MorePlanningExport/PlanImport");
        }

        public override AcceptanceReport CanDesignateCell(IntVec3 c)
        {
            return MorePlanningExport.MorePlannings.Count > 0;
        }
        
        public override void ProcessInput(Event ev)
        {
            var floatMenuOptions = MorePlanningExport.MorePlannings.Select(morePlanning =>
                    new FloatMenuOption("MorePlanningExport.PlanImportFrom".Translate(morePlanning.Name), () =>
                    {
                        morePlanning = MorePlanningExport.LoadFromXML(morePlanning);
                        var planDesignationInfo = MorePlanningExport.TransPlanInfoExported(morePlanning.Contents);
                        PasteDesignator.CurrentPlanCopy = new PlanInfoSet(planDesignationInfo);
                        Find.DesignatorManager.Select(PlanUtility.GetPlanningDesignator<PasteDesignator>());
                    }))
                .ToList();

            if (floatMenuOptions.NullOrEmpty())
            {
                floatMenuOptions.Add(
                    new FloatMenuOption("MorePlanningExport.NoExportedPlan".Translate(),
                        () => { },
                        MenuOptionPriority.DisabledOption));
            }
            else
            {
                VisibilityDesignator.PlanningVisibility = true;
            }

            Find.WindowStack.Add((Window) new FloatMenu(floatMenuOptions));
        }

    }
}