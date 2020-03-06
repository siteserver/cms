using System.Threading.Tasks;
using Datory.Utils;
using SS.CMS.Abstractions;
using SS.CMS.Abstractions.Dto.Create;

namespace SS.CMS.Services
{
    public partial class CreateManager : ICreateManager
    {
        private readonly IPathManager _pathManager;
        private readonly IParseManager _parseManager;
        private readonly IConfigRepository _configRepository;
        private readonly IChannelRepository _channelRepository;
        private readonly IContentRepository _contentRepository;
        private readonly ISiteRepository _siteRepository;
        private readonly ISpecialRepository _specialRepository;
        private readonly ITemplateRepository _templateRepository;

        public CreateManager(IPathManager pathManager, IParseManager parseManager, IAccessTokenRepository accessTokenRepository, IAdministratorRepository administratorRepository, IAdministratorsInRolesRepository administratorsInRolesRepository, IChannelGroupRepository channelGroupRepository, IChannelRepository channelRepository, IConfigRepository configRepository, IContentCheckRepository contentCheckRepository, IContentGroupRepository contentGroupRepository, IContentRepository contentRepository, IContentTagRepository contentTagRepository, IDbCacheRepository dbCacheRepository, IErrorLogRepository errorLogRepository, ILibraryFileRepository libraryFileRepository, ILibraryGroupRepository libraryGroupRepository, ILibraryImageRepository libraryImageRepository, ILibraryTextRepository libraryTextRepository, ILibraryVideoRepository libraryVideoRepository, ILogRepository logRepository, IPermissionsInRolesRepository permissionsInRolesRepository, IPluginConfigRepository pluginConfigRepository, IPluginRepository pluginRepository, IRelatedFieldItemRepository relatedFieldItemRepository, IRelatedFieldRepository relatedFieldRepository, IRoleRepository roleRepository, ISiteLogRepository siteLogRepository, ISitePermissionsRepository sitePermissionsRepository, ISiteRepository siteRepository, ISpecialRepository specialRepository, ITableStyleRepository tableStyleRepository, ITemplateLogRepository templateLogRepository, ITemplateRepository templateRepository, IUserGroupRepository userGroupRepository, IUserLogRepository userLogRepository, IUserMenuRepository userMenuRepository, IUserRepository userRepository)
        {
            _pathManager = pathManager;
            _parseManager = parseManager;
            _configRepository = configRepository;
            _channelRepository = channelRepository;
            _contentRepository = contentRepository;
            _siteRepository = siteRepository;
            _specialRepository = specialRepository;
            _templateRepository = templateRepository;
        }

        public async Task CreateByAllAsync(int siteId)
        {
            ClearAllTask(siteId);

            var channelIdList = await _channelRepository.GetChannelIdListAsync(siteId);
            foreach (var channelId in channelIdList)
            {
                await CreateChannelAsync(siteId, channelId);
            }

            foreach (var channelId in channelIdList)
            {
                await CreateAllContentAsync(siteId, channelId);
            }

            foreach (var specialId in await _specialRepository.GetAllSpecialIdListAsync(siteId))
            {
                await CreateSpecialAsync(siteId, specialId);
            }

            foreach (var fileTemplateId in await _templateRepository.GetAllFileTemplateIdListAsync(siteId))
            {
                await CreateFileAsync(siteId, fileTemplateId);
            }
        }

        public async Task CreateByTemplateAsync(int siteId, int templateId)
        {
            var templateInfo = await _templateRepository.GetAsync(templateId);

            if (templateInfo.TemplateType == TemplateType.IndexPageTemplate)
            {
                await CreateChannelAsync(siteId, siteId);
            }
            else if (templateInfo.TemplateType == TemplateType.ChannelTemplate)
            {
                var channelIdList = await _channelRepository.GetChannelIdListAsync(templateInfo);
                foreach (var channelId in channelIdList)
                {
                    await CreateChannelAsync(siteId, channelId);
                }
            }
            else if (templateInfo.TemplateType == TemplateType.ContentTemplate)
            {
                var channelIdList = await _channelRepository.GetChannelIdListAsync(templateInfo);
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

        public async Task CreateChannelAsync(int siteId, int channelId)
        {
            if (siteId <= 0 || channelId <= 0) return;

            var (taskName, pageCount) = await GetTaskNameAsync(CreateType.Channel, siteId, channelId, 0, 0, 0);
            if (pageCount == 0) return;

            var taskInfo = new CreateTask(0, taskName, CreateType.Channel, siteId, channelId, 0, 0, 0, pageCount);
            AddPendingTask(taskInfo);
        }

        public async Task CreateContentAsync(int siteId, int channelId, int contentId)
        {
            if (siteId <= 0 || channelId <= 0 || contentId <= 0) return;

            var (taskName, pageCount) = await GetTaskNameAsync(CreateType.Content, siteId, channelId, contentId, 0, 0);
            if (pageCount == 0) return;

            var taskInfo = new CreateTask(0, taskName, CreateType.Content, siteId, channelId, contentId, 0, 0, pageCount);
            AddPendingTask(taskInfo);
        }

        public async Task CreateAllContentAsync(int siteId, int channelId)
        {
            if (siteId <= 0 || channelId <= 0) return;

            var (taskName, pageCount) = await GetTaskNameAsync(CreateType.AllContent, siteId, channelId, 0, 0, 0);
            if (pageCount == 0) return;

            var taskInfo = new CreateTask(0, taskName, CreateType.AllContent, siteId, channelId, 0, 0, 0, pageCount);
            AddPendingTask(taskInfo);
        }

        public async Task CreateFileAsync(int siteId, int fileTemplateId)
        {
            if (siteId <= 0 || fileTemplateId <= 0) return;

            var (taskName, pageCount) = await GetTaskNameAsync(CreateType.File, siteId, 0, 0, fileTemplateId, 0);
            if (pageCount == 0) return;

            var taskInfo = new CreateTask(0, taskName, CreateType.File, siteId, 0, 0, fileTemplateId, 0, pageCount);
            AddPendingTask(taskInfo);
        }

        public async Task CreateSpecialAsync(int siteId, int specialId)
        {
            if (siteId <= 0 || specialId <= 0) return;

            var (taskName, pageCount) = await GetTaskNameAsync(CreateType.Special, siteId, 0, 0, 0, specialId);
            if (pageCount == 0) return;

            var taskInfo = new CreateTask(0, taskName, CreateType.Special, siteId, 0, 0, 0, specialId, pageCount);
            AddPendingTask(taskInfo);
        }

        public async Task TriggerContentChangedEventAsync(int siteId, int channelId)
        {
            if (siteId <= 0 || channelId <= 0) return;

            var channelInfo = await _channelRepository.GetAsync(channelId);
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

        private async Task<(string Name, int PageCount)> GetTaskNameAsync(CreateType createType, int siteId, int channelId, int contentId,
            int fileTemplateId, int specialId)
        {
            var name = string.Empty;
            var pageCount = 0;

            if (createType == CreateType.Channel)
            {
                name = channelId == siteId ? "首页" : _channelRepository.GetChannelNameAsync(siteId, channelId).GetAwaiter().GetResult();
                if (!string.IsNullOrEmpty(name))
                {
                    pageCount = 1;
                }
            }
            else if (createType == CreateType.AllContent)
            {
                var site = await _siteRepository.GetAsync(siteId);
                var channelInfo = await _channelRepository.GetAsync(channelId);

                if (channelInfo != null)
                {
                    var count = await _contentRepository.GetCountAsync(site, channelInfo);
                    if (count > 0)
                    {
                        pageCount = count;
                        name = $"{channelInfo.ChannelName}下所有内容页，共 {pageCount} 项";
                    }
                }
            }
            else if (createType == CreateType.Content)
            {
                var site = await _siteRepository.GetAsync(siteId);
                var content = await _contentRepository.GetAsync(site, channelId, contentId);

                if (content != null)
                {
                    if (!string.IsNullOrEmpty(content.Title))
                    {
                        name = content.Title;
                        pageCount = 1;
                    }
                }
            }
            else if (createType == CreateType.File)
            {
                name = await _templateRepository.GetTemplateNameAsync(fileTemplateId);
                if (!string.IsNullOrEmpty(name))
                {
                    pageCount = 1;
                }
            }
            else if (createType == CreateType.Special)
            {
                name = await _specialRepository.GetTitleAsync(siteId, specialId);
                if (!string.IsNullOrEmpty(name))
                {
                    pageCount = 1;
                }
            }
            return (name, pageCount);
        }
    }
}
