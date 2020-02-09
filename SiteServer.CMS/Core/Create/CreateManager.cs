using System.Threading.Tasks;
using Datory.Utils;
using SiteServer.CMS.DataCache;
using SiteServer.Abstractions;
using SiteServer.CMS.Repositories;


namespace SiteServer.CMS.Core.Create
{
    public static class CreateManager
    {
        private static async Task<(string Name, int PageCount)> GetTaskNameAsync(CreateType createType, int siteId, int channelId, int contentId,
            int fileTemplateId, int specialId)
        {
            var name = string.Empty;
            var pageCount = 0;

            if (createType == CreateType.Channel)
            {
                name = channelId == siteId ? "首页" : DataProvider.ChannelRepository.GetChannelNameAsync(siteId, channelId).GetAwaiter().GetResult();
                if (!string.IsNullOrEmpty(name))
                {
                    pageCount = 1;
                }
            }
            else if (createType == CreateType.AllContent)
            {
                var site = await DataProvider.SiteRepository.GetAsync(siteId);
                var channelInfo = await DataProvider.ChannelRepository.GetAsync(channelId);
                
                if (channelInfo != null)
                {
                    var count = await DataProvider.ContentRepository.GetCountAsync(site, channelInfo);
                    if (count > 0)
                    {
                        pageCount = count;
                        name = $"{channelInfo.ChannelName}下所有内容页，共 {pageCount} 项";
                    }
                }
            }
            else if (createType == CreateType.Content)
            {
                var title = await DataProvider.ContentRepository.GetValueAsync(await DataProvider.ChannelRepository.GetTableNameAsync(await 
                    DataProvider.SiteRepository.GetAsync(siteId), channelId), contentId, ContentAttribute.Title);
                if (!string.IsNullOrEmpty(title))
                {
                    name = title;
                    pageCount = 1;
                }
            }
            else if (createType == CreateType.File)
            {
                name = await DataProvider.TemplateRepository.GetTemplateNameAsync(fileTemplateId);
                if (!string.IsNullOrEmpty(name))
                {
                    pageCount = 1;
                }
            }
            else if (createType == CreateType.Special)
            {
                name = await DataProvider.SpecialRepository.GetTitleAsync(siteId, specialId);
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

            var channelIdList = await DataProvider.ChannelRepository.GetChannelIdListAsync(siteId);
            foreach (var channelId in channelIdList)
            {
                await CreateChannelAsync(siteId, channelId);
            }

            foreach (var channelId in channelIdList)
            {
                await CreateAllContentAsync(siteId, channelId);
            }

            foreach (var specialId in await DataProvider.SpecialRepository.GetAllSpecialIdListAsync(siteId))
            {
                await CreateSpecialAsync(siteId, specialId);
            }

            foreach (var fileTemplateId in await DataProvider.TemplateRepository.GetAllFileTemplateIdListAsync(siteId))
            {
                await CreateFileAsync(siteId, fileTemplateId);
            }
        }

        public static async Task CreateByTemplateAsync(int siteId, int templateId)
        {
            var templateInfo = await DataProvider.TemplateRepository.GetAsync(templateId);

            if (templateInfo.TemplateType == TemplateType.IndexPageTemplate)
            {
                await CreateChannelAsync(siteId, siteId);
            }
            else if (templateInfo.TemplateType == TemplateType.ChannelTemplate)
            {
                var channelIdList = await DataProvider.ChannelRepository.GetChannelIdListAsync(templateInfo);
                foreach (var channelId in channelIdList)
                {
                    await CreateChannelAsync(siteId, channelId);
                }
            }
            else if (templateInfo.TemplateType == TemplateType.ContentTemplate)
            {
                var channelIdList = await DataProvider.ChannelRepository.GetChannelIdListAsync(templateInfo);
                foreach (var channelId in channelIdList)
                {
                    await CreateAllContentAsync(siteId, channelId);
                }
            }
            else if (templateInfo.TemplateType == TemplateType.FileTemplate)
            {
                await CreateFileAsync(siteId, templateId);
            }
        }

        public static async Task CreateChannelAsync(int siteId, int channelId)
        {
            if (siteId <= 0 || channelId <= 0) return;

            var (taskName, pageCount) = await GetTaskNameAsync(CreateType.Channel, siteId, channelId, 0, 0, 0);
            if (pageCount == 0) return;

            var taskInfo = new CreateTaskInfo(0, taskName, CreateType.Channel, siteId, channelId, 0, 0, 0, pageCount);
            CreateTaskManager.AddPendingTask(taskInfo);
        }

        public static async Task CreateContentAsync(int siteId, int channelId, int contentId)
        {
            if (siteId <= 0 || channelId <= 0 || contentId <= 0) return;

            var (taskName, pageCount) = await GetTaskNameAsync(CreateType.Content, siteId, channelId, contentId, 0, 0);
            if (pageCount == 0) return;

            var taskInfo = new CreateTaskInfo(0, taskName, CreateType.Content, siteId, channelId, contentId, 0, 0, pageCount);
            CreateTaskManager.AddPendingTask(taskInfo);
        }

        public static async Task CreateAllContentAsync(int siteId, int channelId)
        {
            if (siteId <= 0 || channelId <= 0) return;

            var (taskName, pageCount) = await GetTaskNameAsync(CreateType.AllContent, siteId, channelId, 0, 0, 0);
            if (pageCount == 0) return;

            var taskInfo = new CreateTaskInfo(0, taskName, CreateType.AllContent, siteId, channelId, 0, 0, 0, pageCount);
            CreateTaskManager.AddPendingTask(taskInfo);
        }

        public static async Task CreateFileAsync(int siteId, int fileTemplateId)
        {
            if (siteId <= 0 || fileTemplateId <= 0) return;

            var (taskName, pageCount) = await GetTaskNameAsync(CreateType.File, siteId, 0, 0, fileTemplateId, 0);
            if (pageCount == 0) return;

            var taskInfo = new CreateTaskInfo(0, taskName, CreateType.File, siteId, 0, 0, fileTemplateId, 0, pageCount);
            CreateTaskManager.AddPendingTask(taskInfo);
        }

        public static async Task CreateSpecialAsync(int siteId, int specialId)
        {
            if (siteId <= 0 || specialId <= 0) return;

            var (taskName, pageCount) = await GetTaskNameAsync(CreateType.Special, siteId, 0, 0, 0, specialId);
            if (pageCount == 0) return;

            var taskInfo = new CreateTaskInfo(0, taskName, CreateType.Special, siteId, 0, 0, 0, specialId, pageCount);
            CreateTaskManager.AddPendingTask(taskInfo);
        }

        public static async Task TriggerContentChangedEventAsync(int siteId, int channelId)
        {
            if (siteId <= 0 || channelId <= 0) return;

            var channelInfo = await DataProvider.ChannelRepository.GetAsync(channelId);
            var channelIdList = Utilities.GetIntList(channelInfo.CreateChannelIdsIfContentChanged);
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
