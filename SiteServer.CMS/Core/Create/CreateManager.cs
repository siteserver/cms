using System.Threading.Tasks;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.DataCache.Content;
using SiteServer.CMS.Enumerations;
using SiteServer.CMS.Model;
using SiteServer.Utils;
using SiteServer.Plugin;

namespace SiteServer.CMS.Core.Create
{
    public static class CreateManager
    {
        private static async Task<(string Name, int PageCount)> GetTaskNameAsync(ECreateType createType, int siteId, int channelId, int contentId,
            int fileTemplateId, int specialId)
        {
            var name = string.Empty;
            var pageCount = 0;

            if (createType == ECreateType.Channel)
            {
                name = channelId == siteId ? "首页" : ChannelManager.GetChannelNameAsync(siteId, channelId).GetAwaiter().GetResult();
                if (!string.IsNullOrEmpty(name))
                {
                    pageCount = 1;
                }
            }
            else if (createType == ECreateType.AllContent)
            {
                var site = await SiteManager.GetSiteAsync(siteId);
                var channelInfo = await ChannelManager.GetChannelAsync(siteId, channelId);
                
                if (channelInfo != null)
                {
                    var count = await ContentManager.GetCountAsync(site, channelInfo, true);
                    if (count > 0)
                    {
                        pageCount = count;
                        name = $"{channelInfo.ChannelName}下所有内容页，共 {pageCount} 项";
                    }
                }
            }
            else if (createType == ECreateType.Content)
            {
                var tuple = DataProvider.ContentDao.GetValue(await ChannelManager.GetTableNameAsync(await 
                    SiteManager.GetSiteAsync(siteId), channelId), contentId, ContentAttribute.Title);
                if (tuple != null)
                {
                    name = tuple.Item2;
                    pageCount = 1;
                }
            }
            else if (createType == ECreateType.File)
            {
                name = await TemplateManager.GetTemplateNameAsync(siteId, fileTemplateId);
                if (!string.IsNullOrEmpty(name))
                {
                    pageCount = 1;
                }
            }
            else if (createType == ECreateType.Special)
            {
                name = await SpecialManager.GetTitleAsync(siteId, specialId);
                if (!string.IsNullOrEmpty(name))
                {
                    pageCount = 1;
                }
            }
            return (name, pageCount);
        }

        public static async Task CreateByAllAsync(int siteId)
        {
            CreateTaskManager.ClearAllTask(siteId);

            var channelIdList = await ChannelManager.GetChannelIdListAsync(siteId);
            foreach (var channelId in channelIdList)
            {
                await CreateChannelAsync(siteId, channelId);
            }

            foreach (var channelId in channelIdList)
            {
                await CreateAllContentAsync(siteId, channelId);
            }

            foreach (var specialId in await SpecialManager.GetAllSpecialIdListAsync(siteId))
            {
                await CreateSpecialAsync(siteId, specialId);
            }

            foreach (var fileTemplateId in await TemplateManager.GetAllFileTemplateIdListAsync(siteId))
            {
                await CreateFileAsync(siteId, fileTemplateId);
            }
        }

        public static async Task CreateByTemplateAsync(int siteId, int templateId)
        {
            var templateInfo = await TemplateManager.GetTemplateAsync(siteId, templateId);

            if (templateInfo.Type == TemplateType.IndexPageTemplate)
            {
                await CreateChannelAsync(siteId, siteId);
            }
            else if (templateInfo.Type == TemplateType.ChannelTemplate)
            {
                var channelIdList = await DataProvider.ChannelDao.GetChannelIdListAsync(templateInfo);
                foreach (var channelId in channelIdList)
                {
                    await CreateChannelAsync(siteId, channelId);
                }
            }
            else if (templateInfo.Type == TemplateType.ContentTemplate)
            {
                var channelIdList = await DataProvider.ChannelDao.GetChannelIdListAsync(templateInfo);
                foreach (var channelId in channelIdList)
                {
                    await CreateAllContentAsync(siteId, channelId);
                }
            }
            else if (templateInfo.Type == TemplateType.FileTemplate)
            {
                await CreateFileAsync(siteId, templateId);
            }
        }

        public static async Task CreateChannelAsync(int siteId, int channelId)
        {
            if (siteId <= 0 || channelId <= 0) return;

            var (taskName, pageCount) = await GetTaskNameAsync(ECreateType.Channel, siteId, channelId, 0, 0, 0);
            if (pageCount == 0) return;

            var taskInfo = new CreateTaskInfo(0, taskName, ECreateType.Channel, siteId, channelId, 0, 0, 0, pageCount);
            CreateTaskManager.AddPendingTask(taskInfo);
        }

        public static async Task CreateContentAsync(int siteId, int channelId, int contentId)
        {
            if (siteId <= 0 || channelId <= 0 || contentId <= 0) return;

            var (taskName, pageCount) = await GetTaskNameAsync(ECreateType.Content, siteId, channelId, contentId, 0, 0);
            if (pageCount == 0) return;

            var taskInfo = new CreateTaskInfo(0, taskName, ECreateType.Content, siteId, channelId, contentId, 0, 0, pageCount);
            CreateTaskManager.AddPendingTask(taskInfo);
        }

        public static async Task CreateAllContentAsync(int siteId, int channelId)
        {
            if (siteId <= 0 || channelId <= 0) return;

            var (taskName, pageCount) = await GetTaskNameAsync(ECreateType.AllContent, siteId, channelId, 0, 0, 0);
            if (pageCount == 0) return;

            var taskInfo = new CreateTaskInfo(0, taskName, ECreateType.AllContent, siteId, channelId, 0, 0, 0, pageCount);
            CreateTaskManager.AddPendingTask(taskInfo);
        }

        public static async Task CreateFileAsync(int siteId, int fileTemplateId)
        {
            if (siteId <= 0 || fileTemplateId <= 0) return;

            var (taskName, pageCount) = await GetTaskNameAsync(ECreateType.File, siteId, 0, 0, fileTemplateId, 0);
            if (pageCount == 0) return;

            var taskInfo = new CreateTaskInfo(0, taskName, ECreateType.File, siteId, 0, 0, fileTemplateId, 0, pageCount);
            CreateTaskManager.AddPendingTask(taskInfo);
        }

        public static async Task CreateSpecialAsync(int siteId, int specialId)
        {
            if (siteId <= 0 || specialId <= 0) return;

            var (taskName, pageCount) = await GetTaskNameAsync(ECreateType.Special, siteId, 0, 0, 0, specialId);
            if (pageCount == 0) return;

            var taskInfo = new CreateTaskInfo(0, taskName, ECreateType.Special, siteId, 0, 0, 0, specialId, pageCount);
            CreateTaskManager.AddPendingTask(taskInfo);
        }

        public static async Task TriggerContentChangedEventAsync(int siteId, int channelId)
        {
            if (siteId <= 0 || channelId <= 0) return;

            var channelInfo = await ChannelManager.GetChannelAsync(siteId, channelId);
            var channelIdList = TranslateUtils.StringCollectionToIntList(channelInfo.CreateChannelIdsIfContentChanged);
            if (channelInfo.IsCreateChannelIfContentChanged && !channelIdList.Contains(channelId))
            {
                channelIdList.Add(channelId);
            }
            foreach (var theChannelId in channelIdList)
            {
                await CreateChannelAsync(siteId, theChannelId);
            }
        }
    }
}
