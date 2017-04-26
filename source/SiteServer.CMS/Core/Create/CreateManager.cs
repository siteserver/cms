using BaiRong.Core;
using SiteServer.CMS.Model;
using SiteServer.CMS.Model.Enumerations;

namespace SiteServer.CMS.Core.Create
{
    public class CreateManager
    {
        public static void CreateIndex(int publishmentSystemId)
        {
            var taskInfo = new CreateTaskInfo(0, ECreateType.Index, publishmentSystemId, 0, 0, 0);
            CreateTaskManager.Instance.AddPendingTask(taskInfo);
        }

        public static void CreateChannel(int publishmentSystemId, int channelId)
        {
            if (channelId > 0)
            {
                var taskInfo = new CreateTaskInfo(0, ECreateType.Channel, publishmentSystemId, channelId, 0, 0);
                CreateTaskManager.Instance.AddPendingTask(taskInfo);
            }
        }

        public static void CreateContent(int publishmentSystemId, int channelId, int contentId)
        {
            if (channelId > 0 && contentId > 0)
            {
                var taskInfo = new CreateTaskInfo(0, ECreateType.Content, publishmentSystemId, channelId, contentId, 0);
                CreateTaskManager.Instance.AddPendingTask(taskInfo);
            }
        }

        public static void CreateAllContent(int publishmentSystemId, int channelId)
        {
            if (channelId > 0)
            {
                var taskInfo = new CreateTaskInfo(0, ECreateType.AllContent, publishmentSystemId, channelId, 0, 0);
                CreateTaskManager.Instance.AddPendingTask(taskInfo);
            }
        }

        public static void CreateFile(int publishmentSystemId, int templateId)
        {
            if (templateId > 0)
            {
                var taskInfo = new CreateTaskInfo(0, ECreateType.File, publishmentSystemId, 0, 0, templateId);
                CreateTaskManager.Instance.AddPendingTask(taskInfo);
            }
        }

        public static void CreateAll(int publishmentSystemId)
        {
            CreateTaskManager.Instance.ClearAllTask(publishmentSystemId);

            var taskInfo = new CreateTaskInfo(0, ECreateType.Index, publishmentSystemId, 0, 0, 0);
            CreateTaskManager.Instance.AddPendingTask(taskInfo);

            var dic = NodeManager.GetNodeInfoHashtableByPublishmentSystemId(publishmentSystemId);
            foreach (NodeInfo nodeInfo in dic.Values)
            {
                if (nodeInfo.NodeId != publishmentSystemId)
                {
                    taskInfo = new CreateTaskInfo(0, ECreateType.Channel, publishmentSystemId, nodeInfo.NodeId, 0, 0);
                    CreateTaskManager.Instance.AddPendingTask(taskInfo);
                }

                taskInfo = new CreateTaskInfo(0, ECreateType.AllContent, publishmentSystemId, nodeInfo.NodeId, 0, 0);
                CreateTaskManager.Instance.AddPendingTask(taskInfo);
            }

            foreach (var templateId in TemplateManager.GetAllTemplateIDList(publishmentSystemId))
            {
                taskInfo = new CreateTaskInfo(0, ECreateType.File, publishmentSystemId, 0, 0, templateId);
                CreateTaskManager.Instance.AddPendingTask(taskInfo);
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
            if (channelId > 0 && contentId > 0)
            {
                var taskInfo = new CreateTaskInfo(0, ECreateType.Content, publishmentSystemId, channelId, contentId, 0);
                CreateTaskManager.Instance.AddPendingTask(taskInfo);
                ContentTrigger(publishmentSystemId, channelId);
            }
        }

        private static void ContentTrigger(int publishmentSystemId, int channelId)
        {
            if (channelId > 0)
            {
                var nodeInfo = NodeManager.GetNodeInfo(publishmentSystemId, channelId);
                var nodeIdList = TranslateUtils.StringCollectionToIntList(nodeInfo.Additional.CreateChannelIDsIfContentChanged);
                if (nodeInfo.Additional.IsCreateChannelIfContentChanged && !nodeIdList.Contains(channelId))
                {
                    nodeIdList.Add(channelId);
                }
                foreach (var theNodeId in nodeIdList)
                {
                    var taskInfo = new CreateTaskInfo(0, ECreateType.Channel, publishmentSystemId, theNodeId, 0, 0);
                    CreateTaskManager.Instance.AddPendingTask(taskInfo);
                }
            }
        }
    }
}
