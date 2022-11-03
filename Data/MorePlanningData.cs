using MorePlanning.Plan;
using Verse;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace MorePlanningExport.Data
{
    public class MorePlanningData : IExposable
    {
        private static readonly Regex ValidNameRegex = new Regex("^[\\w]+$");

        public string Name;

        public List<PlanInfoData> Contents;

        public readonly string FileName;

        public MorePlanningData()
        {
        }

        public MorePlanningData(string name, string fileName)
        {
            Name = name;
            FileName = fileName;
        }

        public MorePlanningData(IEnumerable<PlanInfo> contents)
        {
            Contents = MorePlanningExport.TransPlanInfo(contents);
            Name = "MorePlanningExport.DefaultPlanName".Translate();
            var num = 1;
            while (MorePlanningExport.FindMorePlanning(Name + "_" + num) != null)
                ++num;
            Name = Name + "_" + num;
        }
        
        public void ExposeData()
        {
            Scribe_Collections.Look(ref Contents, "PlanInfo", (LookMode) 2);
            Scribe_Values.Look(ref Name, "Name");
        }

        public static AcceptanceReport IsValidMorePlanningName(string name)
        {
            if (!ValidNameRegex.IsMatch(name))
                return "MorePlanningExport.InvalidPlanName".Translate(name);
            return MorePlanningExport.FindMorePlanning(name) != null
                ? "MorePlanningExport.ExportedPlanWithThatNameAlreadyExists".Translate(name)
                : AcceptanceReport.WasAccepted;
        }
    }
}