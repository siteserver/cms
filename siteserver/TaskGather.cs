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
            if (taskInfo.PublishmentSystemId != 0)
            {
                var webGatherNames = TranslateUtils.StringCollectionToStringList(taskGatherInfo.WebGatherNames);
                var databaseGatherNames = TranslateUtils.StringCollectionToStringList(taskGatherInfo.DatabaseGatherNames);
                var fileGatherNames = TranslateUtils.StringCollectionToStringList(taskGatherInfo.FileGatherNames);
                Gather(taskInfo.PublishmentSystemId, webGatherNames, databaseGatherNames, fileGatherNames);
            }
            else
            {
                var publishmentSystemIdList = TranslateUtils.StringCollectionToIntList(taskGatherInfo.PublishmentSystemIdCollection);
                foreach (var publishmentSystemId in publishmentSystemIdList)
                {
                    var webGatherNames = DataProvider.GatherRuleDao.GetGatherRuleNameList(publishmentSystemId);
                    var databaseGatherNames = DataProvider.GatherDatabaseRuleDao.GetGatherRuleNameList(publishmentSystemId);
                    var fileGatherNames = DataProvider.GatherFileRuleDao.GetGatherRuleNameList(publishmentSystemId);
                    Gather(publishmentSystemId, webGatherNames, databaseGatherNames, fileGatherNames);
                }
            }

            return true;
        }

        private static void Gather(int publishmentSystemId, List<string> webGatherNames, List<string> databaseGatherNames, List<string> fileGatherNames)
        {
            foreach (string webGatherName in webGatherNames)
            {
                var resultBuilder = new StringBuilder();
                var errorBuilder = new StringBuilder();
                GatherUtility.GatherWeb(publishmentSystemId, webGatherName, resultBuilder, errorBuilder, false, string.Empty, string.Empty);
            }
            foreach (string databaseGatherName in databaseGatherNames)
            {
                var resultBuilder = new StringBuilder();
                var errorBuilder = new StringBuilder();
                GatherUtility.GatherDatabase(publishmentSystemId, databaseGatherName, resultBuilder, errorBuilder, false, string.Empty, string.Empty);
            }
            foreach (string fileGatherName in fileGatherNames)
            {
                var resultBuilder = new StringBuilder();
                var errorBuilder = new StringBuilder();
                GatherUtility.GatherFile(publishmentSystemId, fileGatherName, resultBuilder, errorBuilder, false, string.Empty, string.Empty);
            }
        }
    }
}
