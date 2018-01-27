using SiteServer.Utils;
using SiteServer.CMS.Model;
using SiteServer.CMS.Model.Enumerations;

namespace SiteServer.CMS.Core.Create
{
    public class CreateManager
    {
        public static string GetTaskName(ECreateType createType, int siteId, int channelId, int contentId,
            int templateId, out int pageCount)
        {
            pageCount = 0;
            var name = string.Empty;
            if (createType == ECreateType.Channel)
            {
                name = channelId == siteId ? "首页" : ChannelManager.GetChannelName(siteId, channelId);
                if (!string.IsNullOrEmpty(name))
                {
                    pageCount = 1;
                }
            }
            else if (createType == ECreateType.AllContent)
            {
                var nodeInfo = ChannelManager.GetChannelInfo(siteId, channelId);
                if (nodeInfo != null && nodeInfo.ContentNum > 0)
                {
                    pageCount = nodeInfo.ContentNum;
                    name = $"{nodeInfo.ChannelName}下所有内容页，共{pageCount}项";
                }
            }
            else if (createType == ECreateType.Content)
            {
                name =
                    DataProvider.ContentDao.GetValue(
                        ChannelManager.GetTableName(
                            SiteManager.GetSiteInfo(siteId), channelId),
                        contentId, ContentAttribute.Title);
                if (!string.IsNullOrEmpty(name))
                {
                    pageCount = 1;
                }
            }
            else if (createType == ECreateType.File)
            {
                name = TemplateManager.GetTemplateName(siteId, templateId);
                if (!string.IsNullOrEmpty(name))
                {
                    pageCount = 1;
                }
            }
            return name;
        }

        public static void CreateChannel(int siteId, int channelId)
        {
            if (siteId <= 0 || channelId <= 0) return;

            int pageCount;
            var taskName = GetTaskName(ECreateType.Channel, siteId, channelId, 0, 0, out pageCount);
            if (pageCount == 0) return;

            var taskInfo = new CreateTaskInfo(0, taskName, ECreateType.Channel, siteId, channelId, 0, 0, pageCount);
            CreateTaskManager.Instance.AddPendingTask(taskInfo);
        }

        public static void CreateContent(int siteId, int channelId, int contentId)
        {
            if (siteId <= 0 || channelId <= 0 || contentId <= 0) return;

            int pageCount;
            var taskName = GetTaskName(ECreateType.Content, siteId, channelId, contentId, 0, out pageCount);
            if (pageCount == 0) return;

            var taskInfo = new CreateTaskInfo(0, taskName, ECreateType.Content, siteId, channelId, contentId, 0, pageCount);
            CreateTaskManager.Instance.AddPendingTask(taskInfo);
        }

        public static void CreateAllContent(int siteId, int channelId)
        {
            if (siteId <= 0 || channelId <= 0) return;

            int pageCount;
            var taskName = GetTaskName(ECreateType.AllContent, siteId, channelId, 0, 0, out pageCount);
            if (pageCount == 0) return;

            var taskInfo = new CreateTaskInfo(0, taskName, ECreateType.AllContent, siteId, channelId, 0, 0, pageCount);
            CreateTaskManager.Instance.AddPendingTask(taskInfo);
        }

        public static void CreateFile(int siteId, int templateId)
        {
            if (siteId <= 0 || templateId <= 0) return;

            int pageCount;
            var taskName = GetTaskName(ECreateType.File, siteId, 0, 0, templateId, out pageCount);
            if (pageCount == 0) return;

            var taskInfo = new CreateTaskInfo(0, taskName, ECreateType.File, siteId, 0, 0, templateId, pageCount);
            CreateTaskManager.Instance.AddPendingTask(taskInfo);
        }

        public static void CreateAll(int siteId)
        {
            CreateTaskManager.Instance.ClearAllTask(siteId);

            var nodeInfoList = ChannelManager.GetChannelInfoList(siteId);
            foreach (var nodeInfo in nodeInfoList)
            {
                CreateChannel(siteId, nodeInfo.Id);
                CreateAllContent(siteId, nodeInfo.Id);
            }

            foreach (var templateId in TemplateManager.GetAllFileTemplateIdList(siteId))
            {
                CreateFile(siteId, templateId);
            }
        }

        public static void CreateContentTrigger(int siteId, int channelId)
        {
            if (channelId > 0)
            {
                ContentTrigger(siteId, channelId);
            }
        }

        public static void CreateContentAndTrigger(int siteId, int channelId, int contentId)
        {
            if (siteId <= 0 || channelId <= 0 || contentId <= 0) return;

            CreateContent(siteId, channelId, contentId);

            ContentTrigger(siteId, channelId);
        }

        private static void ContentTrigger(int siteId, int channelId)
        {
            if (siteId <= 0 || channelId <= 0) return;

            var nodeInfo = ChannelManager.GetChannelInfo(siteId, channelId);
            var channelIdList = TranslateUtils.StringCollectionToIntList(nodeInfo.Additional.CreateChannelIDsIfContentChanged);
            if (nodeInfo.Additional.IsCreateChannelIfContentChanged && !channelIdList.Contains(channelId))
            {
                channelIdList.Add(channelId);
            }
            foreach (var theChannelId in channelIdList)
            {
                CreateChannel(siteId, theChannelId);
            }
        }
    }
}
