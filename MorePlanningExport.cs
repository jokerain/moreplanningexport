using System.IO;
using System.Reflection;
using Verse;
using MorePlanning.Plan;
using System;
using System.Collections.Generic;
using System.Linq;
using HugsLib;
using MorePlanningExport.Data;

namespace MorePlanningExport
{
    public class MorePlanningExport : ModBase
    {
        public override string ModIdentifier => "com.github.jokerain.moreplanningexport";

        private static MorePlanningExport _instance;

        // public static MorePlanningExport Instance => _instance;

        private bool _initialized;

        private static string _saveLocation;

        private static string SaveLocation => _saveLocation ?? (_saveLocation = GetSaveLocation());

        private static string FullFilePath(string name) => Path.Combine(SaveLocation, name + ".xml");

        private static List<FileInfo> GetSavedFilesList() =>
            new DirectoryInfo(SaveLocation).GetFiles()
                .Where((f => f.Extension == ".xml"))
                .OrderByDescending((f => f.LastWriteTime))
                .ToList();

        public static List<MorePlanningData> MorePlannings { get; private set; }

        public MorePlanningExport()
        {
            _instance = this;
        }

        public static void LogError(string text) => _instance.Logger.Error(text);

        public static void LogMessage(string text) => _instance.Logger.Message(text);

        public override void Initialize()
        {
            if (_instance._initialized)
                return;
            MorePlannings = InitMorePlanningDatas();
            _initialized = true;
        }

        private static string GetSaveLocation()
        {
            var method =
                typeof(GenFilePaths).GetMethod("FolderUnderSaveData", BindingFlags.Static | BindingFlags.NonPublic);
            return !(method == null)
                ? (string)method.Invoke(null, new object[] { "MorePlanningExport" })
                : throw new Exception("MorePlanningExport :: FolderUnderSaveData is null [reflection]");
        }

        public static MorePlanningData FindMorePlanning(string name)
        {
            if (!_instance._initialized)
                _instance.Initialize();
            return MorePlannings.FirstOrDefault(morePlannings => morePlannings.Name == name);
        }

        public static bool MorePlanningExists(string name)
        {
            return MorePlannings.Exists(morePlannings => morePlannings.Name == name);
        }

        private static List<MorePlanningData> InitMorePlanningDatas()
        {
            return (from savedFiles in GetSavedFilesList()
                let withoutExtension = Path.GetFileNameWithoutExtension(savedFiles.Name)
                select new MorePlanningData(withoutExtension, savedFiles.Name)).ToList();
        }

        public static List<PlanInfoData> TransPlanInfo(IEnumerable<PlanInfo> planInfos)
        {
            return planInfos.Select(planInfo => new PlanInfoData(planInfo.Pos, planInfo.Color)).ToList();
        }

        public static List<PlanInfo> TransPlanInfoExported(IEnumerable<PlanInfoData> planInfoData)
        {
            return planInfoData.Select(item =>
            {
                var planInfo = new PlanInfo
                {
                    Color = item.Color,
                    Pos = item.Pos
                };
                return planInfo;
            }).ToList();
        }

        public static MorePlanningData LoadFromXML(MorePlanningData morePlanningData)
        {
            var found = FindMorePlanning(morePlanningData.Name);
            if (found.Contents != null) return found;
            try
            {
                Scribe.loader.InitLoading(SaveLocation + "/" + morePlanningData.FileName);
                ScribeMetaHeaderUtility.LoadGameDataHeader((ScribeMetaHeaderUtility.ScribeHeaderMode)1, true);
                Scribe.EnterNode("MorePlanningExport");
                found.ExposeData();
                Scribe.ExitNode();
            }
            catch (Exception ex)
            {
                LogError("Exception while loading blueprint: " + ex);
            }
            finally
            {
                Scribe.loader.FinalizeLoading();
                Scribe.mode = 0;
            }

            return found;
        }

        public static void SaveToXML(MorePlanningData data)
        {
            try
            {
                try
                {
                    Scribe.saver.InitSaving(FullFilePath(data.Name), "MorePlanningExport");
                }
                catch (Exception ex)
                {
                    GenUI.ErrorDialog("ProblemSavingFile".Translate(ex.ToString()));
                    throw;
                }

                ScribeMetaHeaderUtility.WriteMetaHeader();
                Scribe_Deep.Look(ref data, "MorePlanningExport", Array.Empty<object>());
            }
            catch (Exception ex)
            {
                Log.Error("Exception while saving blueprint: " + ex);
            }
            finally
            {
                Scribe.saver.FinalizeSaving();
            }
        }
        
        public static void DeleteXML(MorePlanningData data) => File.Delete(FullFilePath(data.Name));
    }
}