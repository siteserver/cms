using SiteServer.CMS.DataCache;
using SiteServer.CMS.DataCache.Content;
using SiteServer.Utils;
using SiteServer.CMS.Model.Attributes;
using SiteServer.CMS.Model.Enumerations;
using SiteServer.Plugin;

namespace SiteServer.CMS.Core.Create
{
    public static class CreateManager
    {
        private static string GetTaskName(ECreateType createType, int siteId, int channelId, int contentId,
            int fileTemplateId, int specialId, out int pageCount)
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
                var siteInfo = SiteManager.GetSiteInfo(siteId);
                var channelInfo = ChannelManager.GetChannelInfo(siteId, channelId);
                
                if (channelInfo != null)
                {
                    var count = ContentManager.GetCount(siteInfo, channelInfo, true);
                    if (count > 0)
                    {
                        pageCount = count;
                        name = $"{channelInfo.ChannelName}下所有内容页，共 {pageCount} 项";
                    }
                }
            }
            else if (createType == ECreateType.Content)
            {
                var tuple = DataProvider.ContentDao.GetValue(ChannelManager.GetTableName(
                    SiteManager.GetSiteInfo(siteId), channelId), contentId, ContentAttribute.Title);
                if (tuple != null)
                {
                    name = tuple.Item2;
                    pageCount = 1;
                }
            }
            else if (createType == ECreateType.File)
            {
                name = TemplateManager.GetTemplateName(siteId, fileTemplateId);
                if (!string.IsNullOrEmpty(name))
                {
                    pageCount = 1;
                }
            }
            else if (createType == ECreateType.Special)
            {
                name = SpecialManager.GetTitle(siteId, specialId);
                if (!string.IsNullOrEmpty(name))
                {
                    pageCount = 1;
                }
            }
            return name;
        }

        public static void CreateByAll(int siteId)
        {
            CreateTaskManager.ClearAllTask(siteId);

            var channelIdList = ChannelManager.GetChannelIdList(siteId);
            foreach (var channelId in channelIdList)
            {
                CreateChannel(siteId, channelId);
            }

            foreach (var channelId in channelIdList)
            {
                CreateAllContent(siteId, channelId);
            }

            foreach (var specialId in SpecialManager.GetAllSpecialIdList(siteId))
            {
                CreateSpecial(siteId, specialId);
            }

            foreach (var fileTemplateId in TemplateManager.GetAllFileTemplateIdList(siteId))
            {
                CreateFile(siteId, fileTemplateId);
            }
        }

        public static void CreateByTemplate(int siteId, int templateId)
        {
            var templateInfo = TemplateManager.GetTemplateInfo(siteId, templateId);

            if (templateInfo.TemplateType == TemplateType.IndexPageTemplate)
            {
                CreateChannel(siteId, siteId);
            }
            else if (templateInfo.TemplateType == TemplateType.ChannelTemplate)
            {
                var channelIdList = DataProvider.ChannelDao.GetChannelIdList(templateInfo);
                foreach (var channelId in channelIdList)
                {
                    CreateChannel(siteId, channelId);
                }
            }
            else if (templateInfo.TemplateType == TemplateType.ContentTemplate)
            {
                var channelIdList = DataProvider.ChannelDao.GetChannelIdList(templateInfo);
                foreach (var channelId in channelIdList)
                {
                    CreateAllContent(siteId, channelId);
                }
            }
            else if (templateInfo.TemplateType == TemplateType.FileTemplate)
            {
                CreateFile(siteId, templateId);
            }
        }

        public static void CreateChannel(int siteId, int channelId)
        {
            if (siteId <= 0 || channelId <= 0) return;

            int pageCount;
            var taskName = GetTaskName(ECreateType.Channel, siteId, channelId, 0, 0, 0, out pageCount);
            if (pageCount == 0) return;

            var taskInfo = new CreateTaskInfo(0, taskName, ECreateType.Channel, siteId, channelId, 0, 0, 0, pageCount);
            CreateTaskManager.AddPendingTask(taskInfo);
        }

        public static void CreateContent(int siteId, int channelId, int contentId)
        {
            if (siteId <= 0 || channelId <= 0 || contentId <= 0) return;

            int pageCount;
            var taskName = GetTaskName(ECreateType.Content, siteId, channelId, contentId, 0, 0, out pageCount);
            if (pageCount == 0) return;

            var taskInfo = new CreateTaskInfo(0, taskName, ECreateType.Content, siteId, channelId, contentId, 0, 0, pageCount);
            CreateTaskManager.AddPendingTask(taskInfo);
        }

        public static void CreateAllContent(int siteId, int channelId)
        {
            if (siteId <= 0 || channelId <= 0) return;

            int pageCount;
            var taskName = GetTaskName(ECreateType.AllContent, siteId, channelId, 0, 0, 0, out pageCount);
            if (pageCount == 0) return;

            var taskInfo = new CreateTaskInfo(0, taskName, ECreateType.AllContent, siteId, channelId, 0, 0, 0, pageCount);
            CreateTaskManager.AddPendingTask(taskInfo);
        }

        public static void CreateFile(int siteId, int fileTemplateId)
        {
            if (siteId <= 0 || fileTemplateId <= 0) return;

            int pageCount;
            var taskName = GetTaskName(ECreateType.File, siteId, 0, 0, fileTemplateId, 0, out pageCount);
            if (pageCount == 0) return;

            var taskInfo = new CreateTaskInfo(0, taskName, ECreateType.File, siteId, 0, 0, fileTemplateId, 0, pageCount);
            CreateTaskManager.AddPendingTask(taskInfo);
        }

        public static void CreateSpecial(int siteId, int specialId)
        {
            if (siteId <= 0 || specialId <= 0) return;

            int pageCount;
            var taskName = GetTaskName(ECreateType.Special, siteId, 0, 0, 0, specialId, out pageCount);
            if (pageCount == 0) return;

            var taskInfo = new CreateTaskInfo(0, taskName, ECreateType.Special, siteId, 0, 0, 0, specialId, pageCount);
            CreateTaskManager.AddPendingTask(taskInfo);
        }

        public static void TriggerContentChangedEvent(int siteId, int channelId)
        {
            if (siteId <= 0 || channelId <= 0) return;

            var channelInfo = ChannelManager.GetChannelInfo(siteId, channelId);
            var channelIdList = TranslateUtils.StringCollectionToIntList(channelInfo.Additional.CreateChannelIdsIfContentChanged);
            if (channelInfo.Additional.IsCreateChannelIfContentChanged && !channelIdList.Contains(channelId))
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
