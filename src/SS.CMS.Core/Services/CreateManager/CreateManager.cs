using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using SS.CMS.Abstractions;
using SS.CMS.Abstractions.Enums;
using SS.CMS.Abstractions.Models;
using SS.CMS.Abstractions.Repositories;
using SS.CMS.Abstractions.Services;
using SS.CMS.Core.Cache;
using SS.CMS.Core.Cache.Stl;
using SS.CMS.Core.Common;
using SS.CMS.Core.Common.Create;
using SS.CMS.Core.Models.Attributes;
using SS.CMS.Core.Models.Enumerations;
using SS.CMS.Core.StlParser;
using SS.CMS.Core.StlParser.Models;
using SS.CMS.Core.StlParser.StlElement;
using SS.CMS.Core.StlParser.Utility;
using SS.CMS.Utils;

namespace SS.CMS.Core.Services
{
    public class CreateManager : ICreateManager
    {
        private readonly IConfiguration _configuration;
        private readonly ISettingsManager _settingsManager;
        private readonly IPluginManager _pluginManager;
        private readonly IUrlManager _urlManager;
        private readonly IPathManager _pathManager;
        private readonly IFileManager _fileManager;
        private readonly ISiteRepository _siteRepository;
        private readonly ISpecialRepository _specialRepository;
        private readonly IUserRepository _userRepository;
        private readonly ITableStyleRepository _tableStyleRepository;
        private readonly ITemplateRepository _templateRepository;

        public CreateManager(IConfiguration configuration, ISettingsManager settingsManager, IPluginManager pluginManager, IUrlManager urlManager, IPathManager pathManager, IFileManager fileManager, ISiteRepository siteRepository, ISpecialRepository specialRepository, IUserRepository userRepository, ITableStyleRepository tableStyleRepository, ITemplateRepository templateRepository)
        {
            _configuration = configuration;
            _settingsManager = settingsManager;
            _pluginManager = pluginManager;
            _urlManager = urlManager;
            _pathManager = pathManager;
            _fileManager = fileManager;
            _siteRepository = siteRepository;
            _specialRepository = specialRepository;
            _userRepository = userRepository;
            _tableStyleRepository = tableStyleRepository;
            _templateRepository = templateRepository;
        }

        private string GetTaskName(CreateType createType, int siteId, int channelId, int contentId,
            int fileTemplateId, int specialId, out int pageCount)
        {
            pageCount = 0;
            var name = string.Empty;
            if (createType == CreateType.Channel)
            {
                name = channelId == siteId ? "首页" : ChannelManager.GetChannelName(siteId, channelId);
                if (!string.IsNullOrEmpty(name))
                {
                    pageCount = 1;
                }
            }
            else if (createType == CreateType.AllContent)
            {
                var siteInfo = _siteRepository.GetSiteInfo(siteId);
                var channelInfo = ChannelManager.GetChannelInfo(siteId, channelId);

                if (channelInfo != null)
                {
                    var count = channelInfo.ContentRepository.GetCount(_pluginManager, siteInfo, channelInfo, true);
                    if (count > 0)
                    {
                        pageCount = count;
                        name = $"{channelInfo.ChannelName}下所有内容页，共 {pageCount} 项";
                    }
                }
            }
            else if (createType == CreateType.Content)
            {
                var channelInfo = ChannelManager.GetChannelInfo(siteId, channelId);
                var tuple = channelInfo.ContentRepository.GetValueWithChannelId<string>(contentId, ContentAttribute.Title);
                if (tuple != null)
                {
                    name = tuple.Item2;
                    pageCount = 1;
                }
            }
            else if (createType == CreateType.File)
            {
                name = _templateRepository.GetTemplateName(siteId, fileTemplateId);
                if (!string.IsNullOrEmpty(name))
                {
                    pageCount = 1;
                }
            }
            else if (createType == CreateType.Special)
            {
                name = _specialRepository.GetTitle(siteId, specialId);
                if (!string.IsNullOrEmpty(name))
                {
                    pageCount = 1;
                }
            }
            return name;
        }

        public void CreateByAll(int siteId)
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

            foreach (var specialId in _specialRepository.GetAllSpecialIdList(siteId))
            {
                CreateSpecial(siteId, specialId);
            }

            foreach (var fileTemplateId in _templateRepository.GetAllFileTemplateIdList(siteId))
            {
                CreateFile(siteId, fileTemplateId);
            }
        }

        public void CreateByTemplate(int siteId, int templateId)
        {
            var templateInfo = _templateRepository.GetTemplateInfo(siteId, templateId);

            if (templateInfo.Type == TemplateType.IndexPageTemplate)
            {
                CreateChannel(siteId, siteId);
            }
            else if (templateInfo.Type == TemplateType.ChannelTemplate)
            {
                var channelIdList = DataProvider.ChannelRepository.GetChannelIdList(templateInfo);
                foreach (var channelId in channelIdList)
                {
                    CreateChannel(siteId, channelId);
                }
            }
            else if (templateInfo.Type == TemplateType.ContentTemplate)
            {
                var channelIdList = DataProvider.ChannelRepository.GetChannelIdList(templateInfo);
                foreach (var channelId in channelIdList)
                {
                    CreateAllContent(siteId, channelId);
                }
            }
            else if (templateInfo.Type == TemplateType.FileTemplate)
            {
                CreateFile(siteId, templateId);
            }
        }

        public void CreateChannel(int siteId, int channelId)
        {
            if (siteId <= 0 || channelId <= 0) return;

            int pageCount;
            var taskName = GetTaskName(CreateType.Channel, siteId, channelId, 0, 0, 0, out pageCount);
            if (pageCount == 0) return;

            var taskInfo = new CreateTaskInfo(0, taskName, CreateType.Channel, siteId, channelId, 0, 0, 0, pageCount);
            CreateTaskManager.AddPendingTask(taskInfo);
        }

        public void CreateContent(int siteId, int channelId, int contentId)
        {
            if (siteId <= 0 || channelId <= 0 || contentId <= 0) return;

            int pageCount;
            var taskName = GetTaskName(CreateType.Content, siteId, channelId, contentId, 0, 0, out pageCount);
            if (pageCount == 0) return;

            var taskInfo = new CreateTaskInfo(0, taskName, CreateType.Content, siteId, channelId, contentId, 0, 0, pageCount);
            CreateTaskManager.AddPendingTask(taskInfo);
        }

        public void CreateAllContent(int siteId, int channelId)
        {
            if (siteId <= 0 || channelId <= 0) return;

            int pageCount;
            var taskName = GetTaskName(CreateType.AllContent, siteId, channelId, 0, 0, 0, out pageCount);
            if (pageCount == 0) return;

            var taskInfo = new CreateTaskInfo(0, taskName, CreateType.AllContent, siteId, channelId, 0, 0, 0, pageCount);
            CreateTaskManager.AddPendingTask(taskInfo);
        }

        public void CreateFile(int siteId, int fileTemplateId)
        {
            if (siteId <= 0 || fileTemplateId <= 0) return;

            int pageCount;
            var taskName = GetTaskName(CreateType.File, siteId, 0, 0, fileTemplateId, 0, out pageCount);
            if (pageCount == 0) return;

            var taskInfo = new CreateTaskInfo(0, taskName, CreateType.File, siteId, 0, 0, fileTemplateId, 0, pageCount);
            CreateTaskManager.AddPendingTask(taskInfo);
        }

        public void CreateSpecial(int siteId, int specialId)
        {
            if (siteId <= 0 || specialId <= 0) return;

            int pageCount;
            var taskName = GetTaskName(CreateType.Special, siteId, 0, 0, 0, specialId, out pageCount);
            if (pageCount == 0) return;

            var taskInfo = new CreateTaskInfo(0, taskName, CreateType.Special, siteId, 0, 0, 0, specialId, pageCount);
            CreateTaskManager.AddPendingTask(taskInfo);
        }

        public void TriggerContentChangedEvent(int siteId, int channelId)
        {
            if (siteId <= 0 || channelId <= 0) return;

            var channelInfo = ChannelManager.GetChannelInfo(siteId, channelId);
            var channelIdList = TranslateUtils.StringCollectionToIntList(channelInfo.CreateChannelIdsIfContentChanged);
            if (channelInfo.IsCreateChannelIfContentChanged && !channelIdList.Contains(channelId))
            {
                channelIdList.Add(channelId);
            }
            foreach (var theChannelId in channelIdList)
            {
                CreateChannel(siteId, theChannelId);
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
                var siteInfo = _siteRepository.GetSiteInfo(siteId);
                var channelInfo = ChannelManager.GetChannelInfo(siteId, channelId);
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
            var siteInfo = _siteRepository.GetSiteInfo(siteId);
            var channelInfo = ChannelManager.GetChannelInfo(siteId, channelId);

            var contentIdList = channelInfo.ContentRepository.StlGetContentIdListChecked(channelInfo, TaxisType.OrderByTaxisDesc);

            foreach (var contentId in contentIdList)
            {
                await CreateContentAsync(siteInfo, channelInfo, contentId);
            }
        }

        private async Task CreateChannelAsync(int siteId, int channelId)
        {
            var siteInfo = _siteRepository.GetSiteInfo(siteId);
            var channelInfo = ChannelManager.GetChannelInfo(siteId, channelId);

            if (!ChannelManager.IsCreatable(_pluginManager, siteInfo, channelInfo)) return;

            var templateInfo = channelId == siteId
                ? _templateRepository.GetIndexPageTemplateInfo(siteId)
                : _templateRepository.GetChannelTemplateInfo(siteId, channelId);
            var filePath = _pathManager.GetChannelPageFilePath(siteInfo, channelId, 0);
            var parseContext = new ParseContext(new PageInfo(_urlManager.ApiUrl, channelId, 0, siteInfo, templateInfo, new Dictionary<string, object>()), _configuration, _settingsManager, _pluginManager, _pathManager, _urlManager, _fileManager, _siteRepository, _userRepository, _tableStyleRepository, _templateRepository)
            {
                ContextType = EContextType.Channel
            };
            var contentBuilder = new StringBuilder(_templateRepository.GetTemplateContent(siteInfo, templateInfo));

            var stlLabelList = StlParserUtility.GetStlLabelList(contentBuilder.ToString());

            //如果标签中存在<stl:channel type="PageContent"></stl:channel>
            if (StlParserUtility.IsStlChannelElementWithTypePageContent(stlLabelList)) //内容存在
            {
                var stlElement = StlParserUtility.GetStlChannelElementWithTypePageContent(stlLabelList);
                var stlElementTranslated = parseContext.StlEncrypt(stlElement);
                contentBuilder.Replace(stlElement, stlElementTranslated);

                var innerBuilder = new StringBuilder(stlElement);
                parseContext.ParseInnerContent(innerBuilder);
                var pageContentHtml = innerBuilder.ToString();
                var pageCount = StringUtils.GetCount(ContentUtility.PagePlaceHolder, pageContentHtml) + 1; //一共需要的页数

                parseContext.Parse(contentBuilder, filePath, false);

                for (var currentPageIndex = 0; currentPageIndex < pageCount; currentPageIndex++)
                {
                    parseContext = parseContext.Clone();

                    var index = pageContentHtml.IndexOf(ContentUtility.PagePlaceHolder, StringComparison.Ordinal);
                    var length = index == -1 ? pageContentHtml.Length : index;

                    var pageHtml = pageContentHtml.Substring(0, length);
                    var pagedBuilder =
                        new StringBuilder(contentBuilder.ToString().Replace(stlElementTranslated, pageHtml));
                    parseContext.ReplacePageElementsInChannelPage(pagedBuilder, stlLabelList, currentPageIndex, pageCount, 0);

                    filePath = _pathManager.GetChannelPageFilePath(siteInfo, parseContext.PageChannelId,
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

                var pageContentsElementParser = new StlPageContents(stlElement, parseContext);
                var pageCount = pageContentsElementParser.GetPageCount(out var totalNum);

                parseContext.PageInfo.AddPageBodyCodeIfNotExists(_urlManager, PageInfo.Const.Jquery);
                parseContext.Parse(contentBuilder, filePath, false);

                for (var currentPageIndex = 0; currentPageIndex < pageCount; currentPageIndex++)
                {
                    var context = parseContext.Clone();
                    var pageHtml = pageContentsElementParser.Parse(totalNum, currentPageIndex, pageCount, true);
                    var pagedBuilder =
                        new StringBuilder(contentBuilder.ToString().Replace(stlElementTranslated, pageHtml));

                    context.ReplacePageElementsInChannelPage(pagedBuilder, stlLabelList,
                        currentPageIndex, pageCount, totalNum);

                    filePath = _pathManager.GetChannelPageFilePath(siteInfo, context.PageChannelId,
                        currentPageIndex);

                    await GenerateFileAsync(filePath, pagedBuilder);
                }
            }
            //如果标签中存在<stl:pageChannels>
            else if (StlParserUtility.IsStlElementExists(StlPageChannels.ElementName, stlLabelList))
            {
                var stlElement = StlParserUtility.GetStlElement(StlPageChannels.ElementName, stlLabelList);
                var stlElementTranslated = parseContext.StlEncrypt(stlElement);

                var pageChannelsElementParser = new StlPageChannels(stlElement, parseContext);
                var pageCount = pageChannelsElementParser.GetPageCount(out var totalNum);

                parseContext.PageInfo.AddPageBodyCodeIfNotExists(_urlManager, PageInfo.Const.Jquery);
                parseContext.Parse(contentBuilder, filePath, false);

                for (var currentPageIndex = 0; currentPageIndex < pageCount; currentPageIndex++)
                {
                    var context = parseContext.Clone();
                    var pageHtml = pageChannelsElementParser.Parse(currentPageIndex, pageCount);
                    var pagedBuilder =
                        new StringBuilder(contentBuilder.ToString().Replace(stlElementTranslated, pageHtml));

                    parseContext.ReplacePageElementsInChannelPage(pagedBuilder, stlLabelList,
                        currentPageIndex, pageCount, totalNum);

                    filePath = _pathManager.GetChannelPageFilePath(siteInfo, parseContext.PageChannelId,
                        currentPageIndex);
                    await GenerateFileAsync(filePath, pagedBuilder);
                }
            }
            //如果标签中存在<stl:pageSqlContents>
            else if (StlParserUtility.IsStlElementExists(StlPageSqlContents.ElementName, stlLabelList))
            {
                var stlElement = StlParserUtility.GetStlElement(StlPageSqlContents.ElementName, stlLabelList);
                var stlElementTranslated = parseContext.StlEncrypt(stlElement);

                var pageSqlContentsElementParser = new StlPageSqlContents(stlElement, parseContext);
                var pageCount = pageSqlContentsElementParser.GetPageCount(out var totalNum);

                parseContext.PageInfo.AddPageBodyCodeIfNotExists(_urlManager, PageInfo.Const.Jquery);
                parseContext.Parse(contentBuilder, filePath, false);

                for (var currentPageIndex = 0; currentPageIndex < pageCount; currentPageIndex++)
                {
                    var context = parseContext.Clone();
                    var pageHtml = pageSqlContentsElementParser.Parse(totalNum, currentPageIndex, pageCount, true);
                    var pagedBuilder =
                        new StringBuilder(contentBuilder.ToString().Replace(stlElementTranslated, pageHtml));

                    context.ReplacePageElementsInChannelPage(pagedBuilder, stlLabelList, currentPageIndex, pageCount, totalNum);

                    filePath = _pathManager.GetChannelPageFilePath(siteInfo, context.PageChannelId,
                        currentPageIndex);
                    await GenerateFileAsync(filePath, pagedBuilder);
                }
            }
            else
            {
                parseContext.Parse(contentBuilder, filePath, false);
                await GenerateFileAsync(filePath, contentBuilder);
            }
        }

        private async Task CreateContentAsync(SiteInfo siteInfo, ChannelInfo channelInfo, int contentId)
        {
            var contentInfo = channelInfo.ContentRepository.GetContentInfo(siteInfo, channelInfo, contentId);

            if (contentInfo == null)
            {
                return;
            }

            if (!contentInfo.Checked)
            {
                _fileManager.DeleteContent(siteInfo, channelInfo.Id, contentId);
                return;
            }


            if (!channelInfo.ContentRepository.IsCreatable(channelInfo, contentInfo)) return;


            if (siteInfo.IsCreateStaticContentByAddDate &&
                contentInfo.AddDate < siteInfo.CreateStaticContentAddDate)
            {
                return;
            }

            var templateInfo = _templateRepository.GetContentTemplateInfo(siteInfo.Id, channelInfo.Id);
            var parseContext = new ParseContext(new PageInfo(_urlManager.ApiUrl, channelInfo.Id, contentId, siteInfo, templateInfo, new Dictionary<string, object>()), _configuration, _settingsManager, _pluginManager, _pathManager, _urlManager, _fileManager, _siteRepository, _userRepository, _tableStyleRepository, _templateRepository)
            {
                ContextType = EContextType.Content,
                ContentInfo = contentInfo
            };
            var filePath = _pathManager.GetContentPageFilePath(siteInfo, parseContext.PageChannelId, contentInfo, 0);
            var contentBuilder = new StringBuilder(_templateRepository.GetTemplateContent(siteInfo, templateInfo));

            var stlLabelList = StlParserUtility.GetStlLabelList(contentBuilder.ToString());

            //如果标签中存在<stl:content type="PageContent"></stl:content>
            if (StlParserUtility.IsStlContentElementWithTypePageContent(stlLabelList)) //内容存在
            {
                var stlElement = StlParserUtility.GetStlContentElementWithTypePageContent(stlLabelList);
                var stlElementTranslated = parseContext.StlEncrypt(stlElement);
                contentBuilder.Replace(stlElement, stlElementTranslated);

                var innerBuilder = new StringBuilder(stlElement);
                parseContext.ParseInnerContent(innerBuilder);
                var pageContentHtml = innerBuilder.ToString();
                var pageCount = StringUtils.GetCount(ContentUtility.PagePlaceHolder, pageContentHtml) + 1; //一共需要的页数

                parseContext.Parse(contentBuilder, filePath, false);

                for (var currentPageIndex = 0; currentPageIndex < pageCount; currentPageIndex++)
                {
                    var context = parseContext.Clone();

                    var index = pageContentHtml.IndexOf(ContentUtility.PagePlaceHolder, StringComparison.Ordinal);
                    var length = index == -1 ? pageContentHtml.Length : index;

                    var pageHtml = pageContentHtml.Substring(0, length);
                    var pagedBuilder =
                        new StringBuilder(contentBuilder.ToString().Replace(stlElementTranslated, pageHtml));
                    context.ReplacePageElementsInContentPage(pagedBuilder, stlLabelList, currentPageIndex, pageCount);

                    filePath = _pathManager.GetContentPageFilePath(siteInfo, context.PageChannelId, contentInfo,
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
                parseContext.ParseInnerContent(innerBuilder);
                var pageContentHtml = innerBuilder.ToString();
                var pageCount = StringUtils.GetCount(ContentUtility.PagePlaceHolder, pageContentHtml) + 1; //一共需要的页数

                parseContext.Parse(contentBuilder, filePath, false);

                for (var currentPageIndex = 0; currentPageIndex < pageCount; currentPageIndex++)
                {
                    var context = parseContext.Clone();

                    var index = pageContentHtml.IndexOf(ContentUtility.PagePlaceHolder, StringComparison.Ordinal);
                    var length = index == -1 ? pageContentHtml.Length : index;

                    var pageHtml = pageContentHtml.Substring(0, length);
                    var pagedBuilder =
                        new StringBuilder(contentBuilder.ToString().Replace(stlElementTranslated, pageHtml));
                    context.ReplacePageElementsInContentPage(pagedBuilder, stlLabelList, currentPageIndex, pageCount);

                    filePath = _pathManager.GetContentPageFilePath(siteInfo, context.PageChannelId, contentInfo,
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

                var pageContentsElementParser = new StlPageContents(stlElement, parseContext);
                var pageCount = pageContentsElementParser.GetPageCount(out var totalNum);

                parseContext.Parse(contentBuilder, filePath, false);

                for (var currentPageIndex = 0; currentPageIndex < pageCount; currentPageIndex++)
                {
                    var context = parseContext.Clone();
                    var pageHtml = pageContentsElementParser.Parse(totalNum, currentPageIndex, pageCount, true);
                    var pagedBuilder =
                        new StringBuilder(contentBuilder.ToString().Replace(stlElementTranslated, pageHtml));

                    context.ReplacePageElementsInContentPage(pagedBuilder, stlLabelList, currentPageIndex, pageCount);

                    filePath = _pathManager.GetContentPageFilePath(siteInfo, context.PageChannelId, contentInfo,
                        currentPageIndex);
                    await GenerateFileAsync(filePath, pagedBuilder);
                }
            }
            //如果标签中存在<stl:pageChannels>
            else if (StlParserUtility.IsStlElementExists(StlPageChannels.ElementName, stlLabelList))
            {
                var stlElement = StlParserUtility.GetStlElement(StlPageChannels.ElementName, stlLabelList);
                var stlElementTranslated = parseContext.StlEncrypt(stlElement);

                var pageChannelsElementParser = new StlPageChannels(stlElement, parseContext);
                var pageCount = pageChannelsElementParser.GetPageCount(out _);

                parseContext.Parse(contentBuilder, filePath, false);

                for (var currentPageIndex = 0; currentPageIndex < pageCount; currentPageIndex++)
                {
                    var context = parseContext.Clone();
                    var pageHtml = pageChannelsElementParser.Parse(currentPageIndex, pageCount);
                    var pagedBuilder =
                        new StringBuilder(contentBuilder.ToString().Replace(stlElementTranslated, pageHtml));

                    context.ReplacePageElementsInContentPage(pagedBuilder, stlLabelList, currentPageIndex, pageCount);

                    filePath = _pathManager.GetContentPageFilePath(siteInfo, context.PageChannelId, contentInfo,
                        currentPageIndex);
                    await GenerateFileAsync(filePath, pagedBuilder);
                }
            }
            //如果标签中存在<stl:pageSqlContents>
            else if (StlParserUtility.IsStlElementExists(StlPageSqlContents.ElementName, stlLabelList))
            {
                var stlElement = StlParserUtility.GetStlElement(StlPageSqlContents.ElementName, stlLabelList);
                var stlElementTranslated = parseContext.StlEncrypt(stlElement);

                var pageSqlContentsElementParser = new StlPageSqlContents(stlElement, parseContext);
                var pageCount = pageSqlContentsElementParser.GetPageCount(out var totalNum);

                parseContext.Parse(contentBuilder, filePath, false);

                for (var currentPageIndex = 0; currentPageIndex < pageCount; currentPageIndex++)
                {
                    var context = parseContext.Clone();
                    var pageHtml = pageSqlContentsElementParser.Parse(totalNum, currentPageIndex, pageCount, true);
                    var pagedBuilder =
                        new StringBuilder(contentBuilder.ToString().Replace(stlElementTranslated, pageHtml));

                    context.ReplacePageElementsInContentPage(pagedBuilder, stlLabelList, currentPageIndex, pageCount);

                    filePath = _pathManager.GetContentPageFilePath(siteInfo, context.PageChannelId, contentInfo,
                        currentPageIndex);
                    await GenerateFileAsync(filePath, pagedBuilder);
                }
            }
            else //无翻页
            {
                parseContext.Parse(contentBuilder, filePath, false);
                await GenerateFileAsync(filePath, contentBuilder);
            }
        }

        private async Task CreateFileAsync(int siteId, int fileTemplateId)
        {
            var siteInfo = _siteRepository.GetSiteInfo(siteId);
            var templateInfo = _templateRepository.GetTemplateInfo(siteId, fileTemplateId);
            if (templateInfo == null || templateInfo.Type != TemplateType.FileTemplate)
            {
                return;
            }

            var parseContext = new ParseContext(new PageInfo(_urlManager.ApiUrl, siteId, 0, siteInfo, templateInfo, new Dictionary<string, object>()), _configuration, _settingsManager, _pluginManager, _pathManager, _urlManager, _fileManager, _siteRepository, _userRepository, _tableStyleRepository, _templateRepository);
            var filePath = _pathManager.MapPath(siteInfo, templateInfo.CreatedFileFullName);

            var contentBuilder = new StringBuilder(_templateRepository.GetTemplateContent(siteInfo, templateInfo));
            parseContext.Parse(contentBuilder, filePath, false);
            await GenerateFileAsync(filePath, contentBuilder);
        }

        private async Task CreateSpecialAsync(int siteId, int specialId)
        {
            var siteInfo = _siteRepository.GetSiteInfo(siteId);
            var templateInfoList = _specialRepository.GetTemplateInfoList(siteInfo, specialId, _pathManager);
            foreach (var templateInfo in templateInfoList)
            {
                var parseContext = new ParseContext(new PageInfo(_urlManager.ApiUrl, siteId, 0, siteInfo, templateInfo, new Dictionary<string, object>()), _configuration, _settingsManager, _pluginManager, _pathManager, _urlManager, _fileManager, _siteRepository, _userRepository, _tableStyleRepository, _templateRepository);
                var filePath = _pathManager.MapPath(siteInfo, templateInfo.CreatedFileFullName);

                var contentBuilder = new StringBuilder(templateInfo.Content);
                parseContext.Parse(contentBuilder, filePath, false);
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
