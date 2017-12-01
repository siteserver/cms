using BaiRong.Core;
using BaiRong.Core.Model;
using SiteServer.CMS.Model;
using SiteServer.CMS.Model.Enumerations;

namespace SiteServer.CMS.Core.Create
{
    public class CreateManager
    {
        public static string GetTaskName(ECreateType createType, int publishmentSystemId, int channelId, int contentId,
            int templateId, out int pageCount)
        {
            pageCount = 0;
            var name = string.Empty;
            if (createType == ECreateType.Channel)
            {
                name = channelId == publishmentSystemId ? "首页" : NodeManager.GetNodeName(publishmentSystemId, channelId);
                if (!string.IsNullOrEmpty(name))
                {
                    pageCount = 1;
                }
            }
            else if (createType == ECreateType.AllContent)
            {
                var nodeInfo = NodeManager.GetNodeInfo(publishmentSystemId, channelId);
                if (nodeInfo != null && nodeInfo.ContentNum > 0)
                {
                    pageCount = nodeInfo.ContentNum;
                    name = $"{nodeInfo.NodeName}下所有内容页，共{pageCount}项";
                }
            }
            else if (createType == ECreateType.Content)
            {
                name =
                    BaiRongDataProvider.ContentDao.GetValue(
                        NodeManager.GetTableName(
                            PublishmentSystemManager.GetPublishmentSystemInfo(publishmentSystemId), channelId),
                        contentId, ContentAttribute.Title);
                if (!string.IsNullOrEmpty(name))
                {
                    pageCount = 1;
                }
            }
            else if (createType == ECreateType.File)
            {
                name = TemplateManager.GetTemplateName(publishmentSystemId, templateId);
                if (!string.IsNullOrEmpty(name))
                {
                    pageCount = 1;
                }
            }
            return name;
        }

        public static void CreateChannel(int publishmentSystemId, int channelId)
        {
            if (publishmentSystemId <= 0 || channelId <= 0) return;

            int pageCount;
            var taskName = GetTaskName(ECreateType.Channel, publishmentSystemId, channelId, 0, 0, out pageCount);
            if (pageCount == 0) return;

            var taskInfo = new CreateTaskInfo(0, taskName, ECreateType.Channel, publishmentSystemId, channelId, 0, 0, pageCount);
            CreateTaskManager.Instance.AddPendingTask(taskInfo);
        }

        public static void CreateContent(int publishmentSystemId, int channelId, int contentId)
        {
            if (publishmentSystemId <= 0 || channelId <= 0 || contentId <= 0) return;

            int pageCount;
            var taskName = GetTaskName(ECreateType.Content, publishmentSystemId, channelId, contentId, 0, out pageCount);
            if (pageCount == 0) return;

            var taskInfo = new CreateTaskInfo(0, taskName, ECreateType.Content, publishmentSystemId, channelId, contentId, 0, pageCount);
            CreateTaskManager.Instance.AddPendingTask(taskInfo);
        }

        public static void CreateAllContent(int publishmentSystemId, int channelId)
        {
            if (publishmentSystemId <= 0 || channelId <= 0) return;

            int pageCount;
            var taskName = GetTaskName(ECreateType.AllContent, publishmentSystemId, channelId, 0, 0, out pageCount);
            if (pageCount == 0) return;

            var taskInfo = new CreateTaskInfo(0, taskName, ECreateType.AllContent, publishmentSystemId, channelId, 0, 0, pageCount);
            CreateTaskManager.Instance.AddPendingTask(taskInfo);
        }

        public static void CreateFile(int publishmentSystemId, int templateId)
        {
            if (publishmentSystemId <= 0 || templateId <= 0) return;

            int pageCount;
            var taskName = GetTaskName(ECreateType.File, publishmentSystemId, 0, 0, templateId, out pageCount);
            if (pageCount == 0) return;

            var taskInfo = new CreateTaskInfo(0, taskName, ECreateType.File, publishmentSystemId, 0, 0, templateId, pageCount);
            CreateTaskManager.Instance.AddPendingTask(taskInfo);
        }

        public static void CreateAll(int publishmentSystemId)
        {
            CreateTaskManager.Instance.ClearAllTask(publishmentSystemId);

            var dic = NodeManager.GetNodeInfoDictionaryByPublishmentSystemId(publishmentSystemId);
            foreach (var nodeInfo in dic.Values)
            {
                CreateChannel(publishmentSystemId, nodeInfo.NodeId);
                CreateAllContent(publishmentSystemId, nodeInfo.NodeId);
            }

            foreach (var templateId in TemplateManager.GetAllFileTemplateIdList(publishmentSystemId))
            {
                CreateFile(publishmentSystemId, templateId);
            }
        }

        public static void CreateContentTrigger(int publishmentSystemId, int channelId)
        {
            if (channelId > 0)
            {
                ContentTrigger(publishmentSystemId, channelId);
            }
        }

        public static void CreateContentAndTrigger(int publishmentSystemId, int channelId, int contentId)
        {
            if (publishmentSystemId <= 0 || channelId <= 0 || contentId <= 0) return;

            CreateContent(publishmentSystemId, channelId, contentId);

            ContentTrigger(publishmentSystemId, channelId);
        }

        private static void ContentTrigger(int publishmentSystemId, int channelId)
        {
            if (publishmentSystemId <= 0 || channelId <= 0) return;

            var nodeInfo = NodeManager.GetNodeInfo(publishmentSystemId, channelId);
            var nodeIdList = TranslateUtils.StringCollectionToIntList(nodeInfo.Additional.CreateChannelIDsIfContentChanged);
            if (nodeInfo.Additional.IsCreateChannelIfContentChanged && !nodeIdList.Contains(channelId))
            {
                nodeIdList.Add(channelId);
            }
            foreach (var theNodeId in nodeIdList)
            {
                CreateChannel(publishmentSystemId, theNodeId);
            }
        }
    }
}
