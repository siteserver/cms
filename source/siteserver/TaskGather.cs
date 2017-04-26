using System.Collections.Generic;
using System.Text;
using BaiRong.Core;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;

namespace siteserver
{
    public class TaskGather
    {
        public static bool Execute(TaskInfo taskInfo)
        {
            var taskGatherInfo = new TaskGatherInfo(taskInfo.ServiceParameters);
            if (taskInfo.PublishmentSystemID != 0)
            {
                var webGatherNames = TranslateUtils.StringCollectionToStringList(taskGatherInfo.WebGatherNames);
                var databaseGatherNames = TranslateUtils.StringCollectionToStringList(taskGatherInfo.DatabaseGatherNames);
                var fileGatherNames = TranslateUtils.StringCollectionToStringList(taskGatherInfo.FileGatherNames);
                Gather(taskInfo.PublishmentSystemID, webGatherNames, databaseGatherNames, fileGatherNames);
            }
            else
            {
                var publishmentSystemIDArrayList = TranslateUtils.StringCollectionToIntList(taskGatherInfo.PublishmentSystemIDCollection);
                foreach (int publishmentSystemID in publishmentSystemIDArrayList)
                {
                    var webGatherNames = DataProvider.GatherRuleDao.GetGatherRuleNameArrayList(publishmentSystemID);
                    var databaseGatherNames = DataProvider.GatherDatabaseRuleDao.GetGatherRuleNameArrayList(publishmentSystemID);
                    var fileGatherNames = DataProvider.GatherFileRuleDao.GetGatherRuleNameArrayList(publishmentSystemID);
                    Gather(publishmentSystemID, webGatherNames, databaseGatherNames, fileGatherNames);
                }
            }

            return true;
        }

        private static void Gather(int publishmentSystemID, List<string> webGatherNames, List<string> databaseGatherNames, List<string> fileGatherNames)
        {
            foreach (string webGatherName in webGatherNames)
            {
                var resultBuilder = new StringBuilder();
                var errorBuilder = new StringBuilder();
                GatherUtility.GatherWeb(publishmentSystemID, webGatherName, resultBuilder, errorBuilder, false, string.Empty, string.Empty);
            }
            foreach (string databaseGatherName in databaseGatherNames)
            {
                var resultBuilder = new StringBuilder();
                var errorBuilder = new StringBuilder();
                GatherUtility.GatherDatabase(publishmentSystemID, databaseGatherName, resultBuilder, errorBuilder, false, string.Empty, string.Empty);
            }
            foreach (string fileGatherName in fileGatherNames)
            {
                var resultBuilder = new StringBuilder();
                var errorBuilder = new StringBuilder();
                GatherUtility.GatherFile(publishmentSystemID, fileGatherName, resultBuilder, errorBuilder, false, string.Empty, string.Empty);
            }
        }
    }
}
