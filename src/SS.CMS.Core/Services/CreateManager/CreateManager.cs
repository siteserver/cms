using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using SS.CMS.Core.Common;
using SS.CMS.Core.Common.Create;
using SS.CMS.Core.Models.Attributes;
using SS.CMS.Core.StlParser;
using SS.CMS.Core.StlParser.Models;
using SS.CMS.Core.StlParser.StlElement;
using SS.CMS.Core.StlParser.Utility;
using SS.CMS.Enums;
using SS.CMS.Models;
using SS.CMS.Repositories;
using SS.CMS.Services;
using SS.CMS.Utils;

namespace SS.CMS.Core.Services
{
    public class CreateManager : ICreateManager
    {
        private readonly IConfiguration _configuration;
        private readonly IDistributedCache _cache;
        private readonly ISettingsManager _settingsManager;
        private readonly IPluginManager _pluginManager;
        private readonly IUrlManager _urlManager;
        private readonly IPathManager _pathManager;
        private readonly IFileManager _fileManager;
        private readonly ITableManager _tableManager;
        private readonly ISiteRepository _siteRepository;
        private readonly IChannelRepository _channelRepository;
        private readonly ISpecialRepository _specialRepository;
        private readonly IUserRepository _userRepository;
        private readonly ITableStyleRepository _tableStyleRepository;
        private readonly ITemplateRepository _templateRepository;
        private readonly ITagRepository _tagRepository;
        private readonly IErrorLogRepository _errorLogRepository;

        public CreateManager(IConfiguration configuration, IDistributedCache cache, ISettingsManager settingsManager, IPluginManager pluginManager, IUrlManager urlManager, IPathManager pathManager, IFileManager fileManager, ITableManager tableManager, ISiteRepository siteRepository, IChannelRepository channelRepository, ISpecialRepository specialRepository, IUserRepository userRepository, ITableStyleRepository tableStyleRepository, ITemplateRepository templateRepository, ITagRepository tagRepository, IErrorLogRepository errorLogRepository)
        {
            _configuration = configuration;
            _cache = cache;
            _settingsManager = settingsManager;
            _pluginManager = pluginManager;
            _urlManager = urlManager;
            _pathManager = pathManager;
            _fileManager = fileManager;
            _tableManager = tableManager;
            _siteRepository = siteRepository;
            _channelRepository = channelRepository;
            _specialRepository = specialRepository;
            _userRepository = userRepository;
            _tableStyleRepository = tableStyleRepository;
            _templateRepository = templateRepository;
            _tagRepository = tagRepository;
            _errorLogRepository = errorLogRepository;
        }

        private async Task<(string TaskName, int PageCount)> GetTaskNameAsync(CreateType createType, int siteId, int channelId, int contentId, int fileTemplateId, int specialId)
        {
            var pageCount = 0;
            var name = string.Empty;
            if (createType == CreateType.Channel)
            {
                name = channelId == siteId ? "首页" : await _channelRepository.GetChannelNameAsync(channelId);
                if (!string.IsNullOrEmpty(name))
                {
                    pageCount = 1;
                }
            }
            else if (createType == CreateType.AllContent)
            {
                var siteInfo = await _siteRepository.GetSiteInfoAsync(siteId);
                var channelInfo = await _channelRepository.GetChannelInfoAsync(channelId);

                if (channelInfo != null)
                {
                    var count = await channelInfo.ContentRepository.GetCountAsync(siteInfo, channelInfo, true);
                    if (count > 0)
                    {
                        pageCount = count;
                        name = $"{channelInfo.ChannelName}下所有内容页，共 {pageCount} 项";
                    }
                }
            }
            else if (createType == CreateType.Content)
            {
                var channelInfo = await _channelRepository.GetChannelInfoAsync(channelId);
                var tuple = channelInfo.ContentRepository.GetValueWithChannelId<string>(contentId, ContentAttribute.Title);
                if (tuple != null)
                {
                    name = tuple.Item2;
                    pageCount = 1;
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

        public async Task AddCreateByAllTaskAsync(int siteId)
        {
            CreateTaskManager.ClearAllTask(siteId);

            var channelIdList = await _channelRepository.GetChannelIdListAsync(siteId);
            foreach (var channelId in channelIdList)
            {
                await AddCreateChannelTaskAsync(siteId, channelId);
            }

            foreach (var channelId in channelIdList)
            {
                await AddCreateAllContentTaskAsync(siteId, channelId);
            }

            foreach (var specialId in await _specialRepository.GetAllSpecialIdListAsync(siteId))
            {
                await AddCreateSpecialTaskAsync(siteId, specialId);
            }

            foreach (var fileTemplateId in await _templateRepository.GetAllFileTemplateIdListAsync(siteId))
            {
                await AddCreateFileTaskAsync(siteId, fileTemplateId);
            }
        }

        public async Task AddCreateByTemplateTaskAsync(int siteId, int templateId)
        {
            var templateInfo = await _templateRepository.GetTemplateInfoAsync(templateId);

            if (templateInfo.Type == TemplateType.IndexPageTemplate)
            {
                await AddCreateChannelTaskAsync(siteId, siteId);
            }
            else if (templateInfo.Type == TemplateType.ChannelTemplate)
            {
                var channelIdList = _channelRepository.GetChannelIdList(templateInfo);
                foreach (var channelId in channelIdList)
                {
                    await AddCreateChannelTaskAsync(siteId, channelId);
                }
            }
            else if (templateInfo.Type == TemplateType.ContentTemplate)
            {
                var channelIdList = _channelRepository.GetChannelIdList(templateInfo);
                foreach (var channelId in channelIdList)
                {
                    await AddCreateAllContentTaskAsync(siteId, channelId);
                }
            }
            else if (templateInfo.Type == TemplateType.FileTemplate)
            {
                await AddCreateFileTaskAsync(siteId, templateId);
            }
        }

        public async Task AddCreateChannelTaskAsync(int siteId, int channelId)
        {
            if (siteId <= 0 || channelId <= 0) return;

            var (taskName, pageCount) = await GetTaskNameAsync(CreateType.Channel, siteId, channelId, 0, 0, 0);
            if (pageCount == 0) return;

            var taskInfo = new CreateTaskInfo(0, taskName, CreateType.Channel, siteId, channelId, 0, 0, 0, pageCount);
            CreateTaskManager.AddPendingTask(taskInfo);
        }

        public async Task AddCreateContentTaskAsync(int siteId, int channelId, int contentId)
        {
            if (siteId <= 0 || channelId <= 0 || contentId <= 0) return;

            var (taskName, pageCount) = await GetTaskNameAsync(CreateType.Content, siteId, channelId, contentId, 0, 0);
            if (pageCount == 0) return;

            var taskInfo = new CreateTaskInfo(0, taskName, CreateType.Content, siteId, channelId, contentId, 0, 0, pageCount);
            CreateTaskManager.AddPendingTask(taskInfo);
        }

        public async Task AddCreateAllContentTaskAsync(int siteId, int channelId)
        {
            if (siteId <= 0 || channelId <= 0) return;

            var (taskName, pageCount) = await GetTaskNameAsync(CreateType.AllContent, siteId, channelId, 0, 0, 0);
            if (pageCount == 0) return;

            var taskInfo = new CreateTaskInfo(0, taskName, CreateType.AllContent, siteId, channelId, 0, 0, 0, pageCount);
            CreateTaskManager.AddPendingTask(taskInfo);
        }

        public async Task AddCreateFileTaskAsync(int siteId, int fileTemplateId)
        {
            if (siteId <= 0 || fileTemplateId <= 0) return;

            var (taskName, pageCount) = await GetTaskNameAsync(CreateType.File, siteId, 0, 0, fileTemplateId, 0);
            if (pageCount == 0) return;

            var taskInfo = new CreateTaskInfo(0, taskName, CreateType.File, siteId, 0, 0, fileTemplateId, 0, pageCount);
            CreateTaskManager.AddPendingTask(taskInfo);
        }

        public async Task AddCreateSpecialTaskAsync(int siteId, int specialId)
        {
            if (siteId <= 0 || specialId <= 0) return;

            var (taskName, pageCount) = await GetTaskNameAsync(CreateType.Special, siteId, 0, 0, 0, specialId);
            if (pageCount == 0) return;

            var taskInfo = new CreateTaskInfo(0, taskName, CreateType.Special, siteId, 0, 0, 0, specialId, pageCount);
            CreateTaskManager.AddPendingTask(taskInfo);
        }

        public async Task TriggerContentChangedEventAsync(int siteId, int channelId)
        {
            if (siteId <= 0 || channelId <= 0) return;

            var channelInfo = await _channelRepository.GetChannelInfoAsync(channelId);
            var channelIdList = TranslateUtils.StringCollectionToIntList(channelInfo.CreateChannelIdsIfContentChanged);
            if (channelInfo.IsCreateChannelIfContentChanged && !channelIdList.Contains(channelId))
            {
                channelIdList.Add(channelId);
            }
            foreach (var theChannelId in channelIdList)
            {
                await AddCreateChannelTaskAsync(siteId, theChannelId);
            }
        }

        public async Task ExecuteAsync(int siteId, CreateType createType, int channelId, int contentId,
            int fileTemplateId, int specialId)
        {
            if (createType == CreateType.Channel)
            {
                await CreateChannelAsync(siteId, channelId);
            }
            else if (createType == CreateType.Content)
            {
                var siteInfo = await _siteRepository.GetSiteInfoAsync(siteId);
                var channelInfo = await _channelRepository.GetChannelInfoAsync(channelId);
                await CreateContentAsync(siteInfo, channelInfo, contentId);
            }
            else if (createType == CreateType.AllContent)
            {
                await CreateContentsAsync(siteId, channelId);
            }
            else if (createType == CreateType.File)
            {
                await CreateFileAsync(siteId, fileTemplateId);
            }
            else if (createType == CreateType.Special)
            {
                await CreateSpecialAsync(siteId, specialId);
            }
        }

        private async Task CreateContentsAsync(int siteId, int channelId)
        {
            var siteInfo = await _siteRepository.GetSiteInfoAsync(siteId);
            var channelInfo = await _channelRepository.GetChannelInfoAsync(channelId);

            var contentIdList = channelInfo.ContentRepository.GetContentIdListChecked(channelInfo.Id, TaxisType.OrderByTaxisDesc);

            foreach (var contentId in contentIdList)
            {
                await CreateContentAsync(siteInfo, channelInfo, contentId);
            }
        }

        private async Task CreateChannelAsync(int siteId, int channelId)
        {
            var siteInfo = await _siteRepository.GetSiteInfoAsync(siteId);
            var channelInfo = await _channelRepository.GetChannelInfoAsync(channelId);

            if (!await _channelRepository.IsCreatableAsync(siteInfo, channelInfo)) return;

            var templateInfo = channelId == siteId
                ? _templateRepository.GetIndexPageTemplateInfo(siteId)
                : await _templateRepository.GetChannelTemplateInfoAsync(siteId, channelId);
            var filePath = await _pathManager.GetChannelPageFilePathAsync(siteInfo, channelId, 0);
            var parseContext = new ParseContext(new PageInfo(channelId, 0, siteInfo, templateInfo, new Dictionary<string, object>()), _configuration, _cache, _settingsManager, _pluginManager, _pathManager, _urlManager, _fileManager, _tableManager, _siteRepository, _channelRepository, _userRepository, _tableStyleRepository, _templateRepository, _tagRepository, _errorLogRepository)
            {
                ContextType = EContextType.Channel
            };
            var contentBuilder = new StringBuilder(await _templateRepository.GetTemplateContentAsync(siteInfo, templateInfo));

            var stlLabelList = StlParserUtility.GetStlLabelList(contentBuilder.ToString());

            //如果标签中存在<stl:channel type="PageContent"></stl:channel>
            if (StlParserUtility.IsStlChannelElementWithTypePageContent(stlLabelList)) //内容存在
            {
                var stlElement = StlParserUtility.GetStlChannelElementWithTypePageContent(stlLabelList);
                var stlElementTranslated = parseContext.StlEncrypt(stlElement);
                contentBuilder.Replace(stlElement, stlElementTranslated);

                var innerBuilder = new StringBuilder(stlElement);
                await parseContext.ParseInnerContentAsync(innerBuilder);
                var pageContentHtml = innerBuilder.ToString();
                var pageCount = StringUtils.GetCount(ContentUtility.PagePlaceHolder, pageContentHtml) + 1; //一共需要的页数

                await parseContext.ParseAsync(contentBuilder, filePath, false);

                for (var currentPageIndex = 0; currentPageIndex < pageCount; currentPageIndex++)
                {
                    parseContext = parseContext.Clone();

                    var index = pageContentHtml.IndexOf(ContentUtility.PagePlaceHolder, StringComparison.Ordinal);
                    var length = index == -1 ? pageContentHtml.Length : index;

                    var pageHtml = pageContentHtml.Substring(0, length);
                    var pagedBuilder =
                        new StringBuilder(contentBuilder.ToString().Replace(stlElementTranslated, pageHtml));
                    await parseContext.ReplacePageElementsInChannelPageAsync(pagedBuilder, stlLabelList, currentPageIndex, pageCount, 0);

                    filePath = await _pathManager.GetChannelPageFilePathAsync(siteInfo, parseContext.PageChannelId,
                        currentPageIndex);
                    await GenerateFileAsync(filePath, pagedBuilder);

                    if (index != -1)
                    {
                        pageContentHtml = pageContentHtml.Substring(length + ContentUtility.PagePlaceHolder.Length);
                    }
                }
            }
            //如果标签中存在<stl:pageContents>
            else if (StlParserUtility.IsStlElementExists(StlPageContents.ElementName, stlLabelList))
            {
                var stlElement = StlParserUtility.GetStlElement(StlPageContents.ElementName, stlLabelList);
                var stlElementTranslated = parseContext.StlEncrypt(stlElement);

                var pageContentsElementParser = new StlPageContents();
                await pageContentsElementParser.LoadAsync(stlElement, parseContext);
                var (pageCount, totalNum) = await pageContentsElementParser.GetPageCountAsync();

                parseContext.PageInfo.AddPageBodyCodeIfNotExists(_urlManager, PageInfo.Const.Jquery);
                await parseContext.ParseAsync(contentBuilder, filePath, false);

                for (var currentPageIndex = 0; currentPageIndex < pageCount; currentPageIndex++)
                {
                    var context = parseContext.Clone();
                    var pageHtml = await pageContentsElementParser.ParseAsync(totalNum, currentPageIndex, pageCount, true);
                    var pagedBuilder =
                        new StringBuilder(contentBuilder.ToString().Replace(stlElementTranslated, pageHtml));

                    await context.ReplacePageElementsInChannelPageAsync(pagedBuilder, stlLabelList,
                        currentPageIndex, pageCount, totalNum);

                    filePath = await _pathManager.GetChannelPageFilePathAsync(siteInfo, context.PageChannelId,
                        currentPageIndex);

                    await GenerateFileAsync(filePath, pagedBuilder);
                }
            }
            //如果标签中存在<stl:pageChannels>
            else if (StlParserUtility.IsStlElementExists(StlPageChannels.ElementName, stlLabelList))
            {
                var stlElement = StlParserUtility.GetStlElement(StlPageChannels.ElementName, stlLabelList);
                var stlElementTranslated = parseContext.StlEncrypt(stlElement);

                var pageChannelsElementParser = new StlPageChannels();
                await pageChannelsElementParser.LoadAsync(stlElement, parseContext);
                var pageCount = pageChannelsElementParser.GetPageCount(out var totalNum);

                parseContext.PageInfo.AddPageBodyCodeIfNotExists(_urlManager, PageInfo.Const.Jquery);
                await parseContext.ParseAsync(contentBuilder, filePath, false);

                for (var currentPageIndex = 0; currentPageIndex < pageCount; currentPageIndex++)
                {
                    var context = parseContext.Clone();
                    var pageHtml = await pageChannelsElementParser.ParseAsync(currentPageIndex, pageCount);
                    var pagedBuilder =
                        new StringBuilder(contentBuilder.ToString().Replace(stlElementTranslated, pageHtml));

                    await context.ReplacePageElementsInChannelPageAsync(pagedBuilder, stlLabelList,
                        currentPageIndex, pageCount, totalNum);

                    filePath = await _pathManager.GetChannelPageFilePathAsync(siteInfo, context.PageChannelId,
                        currentPageIndex);
                    await GenerateFileAsync(filePath, pagedBuilder);
                }
            }
            //如果标签中存在<stl:pageSqlContents>
            else if (StlParserUtility.IsStlElementExists(StlPageSqlContents.ElementName, stlLabelList))
            {
                var stlElement = StlParserUtility.GetStlElement(StlPageSqlContents.ElementName, stlLabelList);
                var stlElementTranslated = parseContext.StlEncrypt(stlElement);

                var pageSqlContentsElementParser = new StlPageSqlContents();
                await pageSqlContentsElementParser.LoadAsync(stlElement, parseContext);
                var pageCount = pageSqlContentsElementParser.GetPageCount(out var totalNum);

                parseContext.PageInfo.AddPageBodyCodeIfNotExists(_urlManager, PageInfo.Const.Jquery);
                await parseContext.ParseAsync(contentBuilder, filePath, false);

                for (var currentPageIndex = 0; currentPageIndex < pageCount; currentPageIndex++)
                {
                    var context = parseContext.Clone();
                    var pageHtml = await pageSqlContentsElementParser.ParseAsync(totalNum, currentPageIndex, pageCount, true);
                    var pagedBuilder =
                        new StringBuilder(contentBuilder.ToString().Replace(stlElementTranslated, pageHtml));

                    await context.ReplacePageElementsInChannelPageAsync(pagedBuilder, stlLabelList, currentPageIndex, pageCount, totalNum);

                    filePath = await _pathManager.GetChannelPageFilePathAsync(siteInfo, context.PageChannelId,
                        currentPageIndex);
                    await GenerateFileAsync(filePath, pagedBuilder);
                }
            }
            else
            {
                await parseContext.ParseAsync(contentBuilder, filePath, false);
                await GenerateFileAsync(filePath, contentBuilder);
            }
        }

        private async Task CreateContentAsync(SiteInfo siteInfo, ChannelInfo channelInfo, int contentId)
        {
            var contentInfo = channelInfo.ContentRepository.GetContentInfo(contentId);

            if (contentInfo == null)
            {
                return;
            }

            if (!contentInfo.IsChecked)
            {
                await _fileManager.DeleteContentAsync(siteInfo, channelInfo.Id, contentId);
                return;
            }


            if (!channelInfo.ContentRepository.IsCreatable(channelInfo, contentInfo)) return;


            if (siteInfo.IsCreateStaticContentByAddDate &&
                contentInfo.AddDate < siteInfo.CreateStaticContentAddDate)
            {
                return;
            }

            var templateInfo = await _templateRepository.GetContentTemplateInfoAsync(siteInfo.Id, channelInfo.Id);
            var parseContext = new ParseContext(new PageInfo(channelInfo.Id, contentId, siteInfo, templateInfo, new Dictionary<string, object>()), _configuration, _cache, _settingsManager, _pluginManager, _pathManager, _urlManager, _fileManager, _tableManager, _siteRepository, _channelRepository, _userRepository, _tableStyleRepository, _templateRepository, _tagRepository, _errorLogRepository)
            {
                ContextType = EContextType.Content,
                ContentInfo = contentInfo
            };
            var filePath = await _pathManager.GetContentPageFilePathAsync(siteInfo, parseContext.PageChannelId, contentInfo, 0);
            var contentBuilder = new StringBuilder(await _templateRepository.GetTemplateContentAsync(siteInfo, templateInfo));

            var stlLabelList = StlParserUtility.GetStlLabelList(contentBuilder.ToString());

            //如果标签中存在<stl:content type="PageContent"></stl:content>
            if (StlParserUtility.IsStlContentElementWithTypePageContent(stlLabelList)) //内容存在
            {
                var stlElement = StlParserUtility.GetStlContentElementWithTypePageContent(stlLabelList);
                var stlElementTranslated = parseContext.StlEncrypt(stlElement);
                contentBuilder.Replace(stlElement, stlElementTranslated);

                var innerBuilder = new StringBuilder(stlElement);
                await parseContext.ParseInnerContentAsync(innerBuilder);
                var pageContentHtml = innerBuilder.ToString();
                var pageCount = StringUtils.GetCount(ContentUtility.PagePlaceHolder, pageContentHtml) + 1; //一共需要的页数

                await parseContext.ParseAsync(contentBuilder, filePath, false);

                for (var currentPageIndex = 0; currentPageIndex < pageCount; currentPageIndex++)
                {
                    var context = parseContext.Clone();

                    var index = pageContentHtml.IndexOf(ContentUtility.PagePlaceHolder, StringComparison.Ordinal);
                    var length = index == -1 ? pageContentHtml.Length : index;

                    var pageHtml = pageContentHtml.Substring(0, length);
                    var pagedBuilder =
                        new StringBuilder(contentBuilder.ToString().Replace(stlElementTranslated, pageHtml));
                    await context.ReplacePageElementsInContentPageAsync(pagedBuilder, stlLabelList, currentPageIndex, pageCount);

                    filePath = await _pathManager.GetContentPageFilePathAsync(siteInfo, context.PageChannelId, contentInfo,
                        currentPageIndex);
                    await GenerateFileAsync(filePath, pagedBuilder);

                    if (index != -1)
                    {
                        pageContentHtml = pageContentHtml.Substring(length + ContentUtility.PagePlaceHolder.Length);
                    }
                }
            }
            //如果标签中存在<stl:channel type="PageContent"></stl:channel>
            else if (StlParserUtility.IsStlChannelElementWithTypePageContent(stlLabelList)) //内容存在
            {
                var stlElement = StlParserUtility.GetStlChannelElementWithTypePageContent(stlLabelList);
                var stlElementTranslated = parseContext.StlEncrypt(stlElement);
                contentBuilder.Replace(stlElement, stlElementTranslated);

                var innerBuilder = new StringBuilder(stlElement);
                await parseContext.ParseInnerContentAsync(innerBuilder);
                var pageContentHtml = innerBuilder.ToString();
                var pageCount = StringUtils.GetCount(ContentUtility.PagePlaceHolder, pageContentHtml) + 1; //一共需要的页数

                await parseContext.ParseAsync(contentBuilder, filePath, false);

                for (var currentPageIndex = 0; currentPageIndex < pageCount; currentPageIndex++)
                {
                    var context = parseContext.Clone();

                    var index = pageContentHtml.IndexOf(ContentUtility.PagePlaceHolder, StringComparison.Ordinal);
                    var length = index == -1 ? pageContentHtml.Length : index;

                    var pageHtml = pageContentHtml.Substring(0, length);
                    var pagedBuilder =
                        new StringBuilder(contentBuilder.ToString().Replace(stlElementTranslated, pageHtml));
                    await context.ReplacePageElementsInContentPageAsync(pagedBuilder, stlLabelList, currentPageIndex, pageCount);

                    filePath = await _pathManager.GetContentPageFilePathAsync(siteInfo, context.PageChannelId, contentInfo,
                        currentPageIndex);
                    await GenerateFileAsync(filePath, pagedBuilder);

                    if (index != -1)
                    {
                        pageContentHtml = pageContentHtml.Substring(length + ContentUtility.PagePlaceHolder.Length);
                    }
                }
            }
            //如果标签中存在<stl:pageContents>
            else if (StlParserUtility.IsStlElementExists(StlPageContents.ElementName, stlLabelList))
            {
                var stlElement = StlParserUtility.GetStlElement(StlPageContents.ElementName, stlLabelList);
                var stlElementTranslated = parseContext.StlEncrypt(stlElement);

                var pageContentsElementParser = new StlPageContents();
                await pageContentsElementParser.LoadAsync(stlElement, parseContext);
                var (pageCount, totalNum) = await pageContentsElementParser.GetPageCountAsync();

                await parseContext.ParseAsync(contentBuilder, filePath, false);

                for (var currentPageIndex = 0; currentPageIndex < pageCount; currentPageIndex++)
                {
                    var context = parseContext.Clone();
                    var pageHtml = await pageContentsElementParser.ParseAsync(totalNum, currentPageIndex, pageCount, true);
                    var pagedBuilder =
                        new StringBuilder(contentBuilder.ToString().Replace(stlElementTranslated, pageHtml));

                    await context.ReplacePageElementsInContentPageAsync(pagedBuilder, stlLabelList, currentPageIndex, pageCount);

                    filePath = await _pathManager.GetContentPageFilePathAsync(siteInfo, context.PageChannelId, contentInfo,
                        currentPageIndex);
                    await GenerateFileAsync(filePath, pagedBuilder);
                }
            }
            //如果标签中存在<stl:pageChannels>
            else if (StlParserUtility.IsStlElementExists(StlPageChannels.ElementName, stlLabelList))
            {
                var stlElement = StlParserUtility.GetStlElement(StlPageChannels.ElementName, stlLabelList);
                var stlElementTranslated = parseContext.StlEncrypt(stlElement);

                var pageChannelsElementParser = new StlPageChannels();
                await pageChannelsElementParser.LoadAsync(stlElement, parseContext);
                var pageCount = pageChannelsElementParser.GetPageCount(out _);

                await parseContext.ParseAsync(contentBuilder, filePath, false);

                for (var currentPageIndex = 0; currentPageIndex < pageCount; currentPageIndex++)
                {
                    var context = parseContext.Clone();
                    var pageHtml = await pageChannelsElementParser.ParseAsync(currentPageIndex, pageCount);
                    var pagedBuilder =
                        new StringBuilder(contentBuilder.ToString().Replace(stlElementTranslated, pageHtml));

                    await context.ReplacePageElementsInContentPageAsync(pagedBuilder, stlLabelList, currentPageIndex, pageCount);

                    filePath = await _pathManager.GetContentPageFilePathAsync(siteInfo, context.PageChannelId, contentInfo,
                        currentPageIndex);
                    await GenerateFileAsync(filePath, pagedBuilder);
                }
            }
            //如果标签中存在<stl:pageSqlContents>
            else if (StlParserUtility.IsStlElementExists(StlPageSqlContents.ElementName, stlLabelList))
            {
                var stlElement = StlParserUtility.GetStlElement(StlPageSqlContents.ElementName, stlLabelList);
                var stlElementTranslated = parseContext.StlEncrypt(stlElement);

                var pageSqlContentsElementParser = new StlPageSqlContents();
                await pageSqlContentsElementParser.LoadAsync(stlElement, parseContext);
                var pageCount = pageSqlContentsElementParser.GetPageCount(out var totalNum);

                await parseContext.ParseAsync(contentBuilder, filePath, false);

                for (var currentPageIndex = 0; currentPageIndex < pageCount; currentPageIndex++)
                {
                    var context = parseContext.Clone();
                    var pageHtml = await pageSqlContentsElementParser.ParseAsync(totalNum, currentPageIndex, pageCount, true);
                    var pagedBuilder =
                        new StringBuilder(contentBuilder.ToString().Replace(stlElementTranslated, pageHtml));

                    await context.ReplacePageElementsInContentPageAsync(pagedBuilder, stlLabelList, currentPageIndex, pageCount);

                    filePath = await _pathManager.GetContentPageFilePathAsync(siteInfo, context.PageChannelId, contentInfo,
                        currentPageIndex);
                    await GenerateFileAsync(filePath, pagedBuilder);
                }
            }
            else //无翻页
            {
                await parseContext.ParseAsync(contentBuilder, filePath, false);
                await GenerateFileAsync(filePath, contentBuilder);
            }
        }

        private async Task CreateFileAsync(int siteId, int fileTemplateId)
        {
            var siteInfo = await _siteRepository.GetSiteInfoAsync(siteId);
            var templateInfo = await _templateRepository.GetTemplateInfoAsync(fileTemplateId);
            if (templateInfo == null || templateInfo.Type != TemplateType.FileTemplate)
            {
                return;
            }

            var parseContext = new ParseContext(new PageInfo(siteId, 0, siteInfo, templateInfo, new Dictionary<string, object>()), _configuration, _cache, _settingsManager, _pluginManager, _pathManager, _urlManager, _fileManager, _tableManager, _siteRepository, _channelRepository, _userRepository, _tableStyleRepository, _templateRepository, _tagRepository, _errorLogRepository);
            var filePath = _pathManager.MapPath(siteInfo, templateInfo.CreatedFileFullName);

            var contentBuilder = new StringBuilder(await _templateRepository.GetTemplateContentAsync(siteInfo, templateInfo));
            await parseContext.ParseAsync(contentBuilder, filePath, false);
            await GenerateFileAsync(filePath, contentBuilder);
        }

        private async Task CreateSpecialAsync(int siteId, int specialId)
        {
            var siteInfo = await _siteRepository.GetSiteInfoAsync(siteId);
            var templateInfoList = await _specialRepository.GetTemplateInfoListAsync(siteInfo, specialId, _pathManager);
            foreach (var templateInfo in templateInfoList)
            {
                var parseContext = new ParseContext(new PageInfo(siteId, 0, siteInfo, templateInfo, new Dictionary<string, object>()), _configuration, _cache, _settingsManager, _pluginManager, _pathManager, _urlManager, _fileManager, _tableManager, _siteRepository, _channelRepository, _userRepository, _tableStyleRepository, _templateRepository, _tagRepository, _errorLogRepository);
                var filePath = _pathManager.MapPath(siteInfo, templateInfo.CreatedFileFullName);

                var contentBuilder = new StringBuilder(templateInfo.Content);
                await parseContext.ParseAsync(contentBuilder, filePath, false);
                await GenerateFileAsync(filePath, contentBuilder);
            }
        }

        //private string CreateIncludeFile(string virtualUrl, bool isCreateIfExists)
        //{
        //    var templateInfo = new TemplateInfo(0, SiteId, string.Empty, TemplateType.FileTemplate, string.Empty, string.Empty, string.Empty, ECharsetUtils.GetEnumType(SiteInfo.Additional.Charset), false);
        //    var pageInfo = new PageInfo(SiteId, 0, SiteInfo, templateInfo, null);
        //    var parseContext = new parseContext(pageInfo);

        //    var parsedVirtualUrl = virtualUrl.Substring(0, virtualUrl.LastIndexOf('.')) + "_parsed" + virtualUrl.Substring(virtualUrl.LastIndexOf('.'));
        //    var filePath = _pathManager.MapPath(SiteInfo, parsedVirtualUrl);
        //    if (!isCreateIfExists && FileUtils.IsFileExists(filePath)) return parsedVirtualUrl;

        //    var contentBuilder = new StringBuilder(StlCacheManager.FileContent.GetIncludeContent(SiteInfo, virtualUrl, pageInfo.TemplateInfo.Charset));
        //    StlParserManager.ParseTemplateContent(contentBuilder, pageInfo, parseContext);
        //    var pageAfterBodyScripts = StlParserManager.GetPageInfoScript(pageInfo, true);
        //    var pageBeforeBodyScripts = StlParserManager.GetPageInfoScript(pageInfo, false);
        //    contentBuilder.Insert(0, pageBeforeBodyScripts);
        //    contentBuilder.Append(pageAfterBodyScripts);
        //    GenerateFile(filePath, contentBuilder);
        //    return parsedVirtualUrl;
        //}

        /// <summary>
        /// 在操作系统中创建文件，如果文件存在，重新创建此文件
        /// </summary>
        private static async Task GenerateFileAsync(string filePath, StringBuilder contentBuilder)
        {
            if (string.IsNullOrEmpty(filePath)) return;

            try
            {
                await FileUtils.WriteTextAsync(filePath, Encoding.UTF8, contentBuilder.ToString());
            }
            catch
            {
                FileUtils.RemoveReadOnlyAndHiddenIfExists(filePath);
                await FileUtils.WriteTextAsync(filePath, Encoding.UTF8, contentBuilder.ToString());
            }
        }
    }
}
