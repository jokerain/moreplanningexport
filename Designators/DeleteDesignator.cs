using MorePlanning.Designators;
using System.Linq;
using UnityEngine;
using Verse;

namespace MorePlanningExport.Designators
{
    public class DeleteDesignator : BaseDesignator
    {
        public DeleteDesignator()
            : base("MorePlanningExport.PlanDelete".Translate(), "MorePlanningExport.PlanDeleteDesc".Translate())
        {
            icon = ContentFinder<Texture2D>.Get("MorePlanningExport/PlanDelete");
        }

        public override AcceptanceReport CanDesignateCell(IntVec3 c) => false;

        
        public override void ProcessInput(Event ev)
        {
            var floatMenuOptions = MorePlanningExport.MorePlannings.Select(morePlanning =>
                    new FloatMenuOption("MorePlanningExport.PlanImportFrom".Translate(morePlanning.Name), () =>
                    {
                        MorePlanningExport.DeleteXML(morePlanning);
                        MorePlanningExport.MorePlannings.Remove(morePlanning);
                    }))
                .ToList();

            if (floatMenuOptions.NullOrEmpty())
                floatMenuOptions.Add(new FloatMenuOption("MorePlanningExport.NoExportedPlan".Translate(), () => { },
                    MenuOptionPriority.DisabledOption));

            Find.WindowStack.Add((Window) new FloatMenu(floatMenuOptions));
        }

    }
}