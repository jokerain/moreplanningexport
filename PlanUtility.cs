using System.Collections.Generic;
using System.Reflection;
using Verse;

namespace MorePlanningExport
{
    public class PlanUtility
    {
        private static FieldInfo _resolvedDesignatorsInfo;

        private static void InitReflection()
        {
            _resolvedDesignatorsInfo = typeof(DesignationCategoryDef).GetField("resolvedDesignators",
                BindingFlags.Instance | BindingFlags.NonPublic);
            if (!(_resolvedDesignatorsInfo == null))
                return;
            MorePlanningExport.LogError(
                "Reflection failed (MenuUtility::InitReflection, DesignationCategoryDef.resolvedDesignators)");
        }

        private static List<Designator> GetPlanningDesignators()
        {
            if (_resolvedDesignatorsInfo == null)
                InitReflection();
            var named = DefDatabase<DesignationCategoryDef>.GetNamed("Planning");
            if (named != null)
                return (List<Designator>)_resolvedDesignatorsInfo?.GetValue(named);
            MorePlanningExport.LogError("Menu planning not found");
            return null;
        }

        public static T GetPlanningDesignator<T>() where T : class
        {
            foreach (var planningDesignator1 in GetPlanningDesignators())
            {
                if (planningDesignator1 is T planningDesignator2)
                    return planningDesignator2;
            }

            return default;
        }
    }
}