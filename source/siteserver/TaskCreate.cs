using System.Collections.Generic;
using BaiRong.Core;
using SiteServer.CMS.Core;
using SiteServer.CMS.Core.Create;
using SiteServer.CMS.Model;
using SiteServer.CMS.Model.Enumerations;

namespace siteserver
{
    public class TaskCreate
    {
        public static bool Execute(TaskInfo taskInfo)
        {
            var taskCreateInfo = new TaskCreateInfo(taskInfo.ServiceParameters);
            if (string.IsNullOrEmpty(taskCreateInfo.CreateTypes)) return true;

            var createTypeArrayList = TranslateUtils.StringCollectionToStringList(taskCreateInfo.CreateTypes);
            var createChannel = createTypeArrayList.Contains(ECreateTypeUtils.GetValue(ECreateType.Channel));
            var createContent = createTypeArrayList.Contains(ECreateTypeUtils.GetValue(ECreateType.Content));
            var createFile = createTypeArrayList.Contains(ECreateTypeUtils.GetValue(ECreateType.File));
            if (taskInfo.PublishmentSystemID != 0)
            {
                var nodeIdList = taskCreateInfo.IsCreateAll ? DataProvider.NodeDao.GetNodeIdListByPublishmentSystemId(taskInfo.PublishmentSystemID) : TranslateUtils.StringCollectionToIntList(taskCreateInfo.ChannelIDCollection);

                Create(createChannel, createContent, createFile, taskInfo, taskInfo.PublishmentSystemID, nodeIdList);
            }
            else
            {
                var publishmentSystemIdList = taskCreateInfo.IsCreateAll ? PublishmentSystemManager.GetPublishmentSystemIdList() : TranslateUtils.StringCollectionToIntList(taskCreateInfo.ChannelIDCollection);
                foreach (var publishmentSystemId in publishmentSystemIdList)
                {
                    var nodeIdList = DataProvider.NodeDao.GetNodeIdListByPublishmentSystemId(publishmentSystemId);
                    Create(createChannel, createContent, createFile, taskInfo, publishmentSystemId, nodeIdList);
                }
            }

            return true;
        }

        private static void Create(bool createChannel, bool createContent, bool createFile, TaskInfo taskInfo, int publishmentSystemId, List<int> nodeIdList)
        {
            var publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(publishmentSystemId);
            if (publishmentSystemInfo == null) return;

            if (nodeIdList != null && nodeIdList.Count > 0)
            {
                if (createChannel)
                {
                    foreach (var nodeId in nodeIdList)
                    {
                        CreateManager.CreateChannel(publishmentSystemId, nodeId);
                    }
                }
                if (createContent)
                {
                    foreach (var nodeId in nodeIdList)
                    {
                        CreateManager.CreateAllContent(publishmentSystemId, nodeId);
                    }
                }
            }

            if (createFile)
            {
                var templateIdList = DataProvider.TemplateDao.GetTemplateIdListByType(publishmentSystemId, ETemplateType.FileTemplate);
                foreach (var templateId in templateIdList)
                {
                    CreateManager.CreateFile(publishmentSystemId, templateId);
                }
            }

            if (taskInfo.ServiceType == EServiceType.Create && taskInfo.FrequencyType == EFrequencyType.OnlyOnce)
            {
                DataProvider.TaskDao.Delete(taskInfo.TaskID);
            }
        }
    }
}
