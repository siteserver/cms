using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using SiteServer.Abstractions;
using SiteServer.CMS.Core;
using SiteServer.CMS.StlParser;
using SiteServer.CMS.StlParser.Model;
using SiteServer.CMS.StlParser.StlElement;
using SiteServer.CMS.StlParser.Utility;

namespace SiteServer.CMS.Services
{
    public partial class CreateManager
    {
        public async Task ExecuteAsync(int siteId, CreateType createType, int channelId, int contentId,
            int fileTemplateId, int specialId)
        {
            if (createType == CreateType.Channel)
            {
                await ExecuteChannelAsync(siteId, channelId);
            }
            else if (createType == CreateType.Content)
            {
                var site = await _siteRepository.GetAsync(siteId);
                var channelInfo = await _channelRepository.GetAsync(channelId);
                await ExecuteContentAsync(site, channelInfo, contentId);
            }
            else if (createType == CreateType.AllContent)
            {
                await ExecuteContentsAsync(siteId, channelId);
            }
            else if (createType == CreateType.File)
            {
                await ExecuteFileAsync(siteId, fileTemplateId);
            }
            else if (createType == CreateType.Special)
            {
                await ExecuteSpecialAsync(siteId, specialId);
            }
        }

        private async Task ExecuteContentsAsync(int siteId, int channelId)
        {
            var site = await _siteRepository.GetAsync(siteId);
            var channelInfo = await _channelRepository.GetAsync(channelId);

            var contentIdList = await _contentRepository.GetContentIdsCheckedAsync(site, channelInfo);

            foreach (var contentId in contentIdList)
            {
                await ExecuteContentAsync(site, channelInfo, contentId);
            }
        }

        private async Task ExecuteChannelAsync(int siteId, int channelId)
        {
            var site = await _siteRepository.GetAsync(siteId);
            var channelInfo = await _channelRepository.GetAsync(channelId);

            var count = await _contentRepository.GetCountAsync(site, channelInfo);
            if (!_channelRepository.IsCreatable(site, channelInfo, count)) return;

            var templateInfo = channelId == siteId
                ? await _templateRepository.GetIndexPageTemplateAsync(siteId)
                : await _templateRepository.GetChannelTemplateAsync(siteId, channelInfo);
            var filePath = await PathUtility.GetChannelPageFilePathAsync(site, channelId, 0);
            var pageInfo = await PageInfo.GetPageInfoAsync(channelId, 0, site, templateInfo, new Dictionary<string, object>());
            var contextInfo = new ContextInfo(pageInfo)
            {
                ContextType = ContextType.Channel
            };
            var contentBuilder = new StringBuilder(await _templateRepository.GetTemplateContentAsync(site, templateInfo));

            var stlLabelList = StlParserUtility.GetStlLabelList(contentBuilder.ToString());

            //如果标签中存在<stl:channel type="PageContent"></stl:channel>
            if (StlParserUtility.IsStlChannelElementWithTypePageContent(stlLabelList)) //内容存在
            {
                var stlElement = StlParserUtility.GetStlChannelElementWithTypePageContent(stlLabelList);
                var stlElementTranslated = StlParserManager.StlEncrypt(stlElement);
                contentBuilder.Replace(stlElement, stlElementTranslated);

                var innerBuilder = new StringBuilder(stlElement);
                await StlParserManager.ParseInnerContentAsync(innerBuilder, pageInfo, contextInfo);
                var pageContentHtml = innerBuilder.ToString();
                var pageCount = StringUtils.GetCount(Constants.PagePlaceHolder, pageContentHtml) + 1; //一共需要的页数

                await Parser.ParseAsync(pageInfo, contextInfo, contentBuilder, filePath, false);

                for (var currentPageIndex = 0; currentPageIndex < pageCount; currentPageIndex++)
                {
                    var thePageInfo = await pageInfo.CloneAsync();

                    var index = pageContentHtml.IndexOf(Constants.PagePlaceHolder, StringComparison.Ordinal);
                    var length = index == -1 ? pageContentHtml.Length : index;

                    var pageHtml = pageContentHtml.Substring(0, length);
                    var pagedBuilder =
                        new StringBuilder(contentBuilder.ToString().Replace(stlElementTranslated, pageHtml));
                    await StlParserManager.ReplacePageElementsInChannelPageAsync(pagedBuilder, thePageInfo, stlLabelList,
                        thePageInfo.PageChannelId, currentPageIndex, pageCount, 0);

                    filePath = await PathUtility.GetChannelPageFilePathAsync(site, thePageInfo.PageChannelId,
                        currentPageIndex);
                    await GenerateFileAsync(filePath, pagedBuilder);

                    if (index != -1)
                    {
                        pageContentHtml = pageContentHtml.Substring(length + Constants.PagePlaceHolder.Length);
                    }
                }
            }
            //如果标签中存在<stl:pageContents>
            else if (StlParserUtility.IsStlElementExists(StlPageContents.ElementName, stlLabelList))
            {
                var stlElement = StlParserUtility.GetStlElement(StlPageContents.ElementName, stlLabelList);
                var stlElementTranslated = StlParserManager.StlEncrypt(stlElement);

                var pageContentsElementParser = await StlPageContents.GetAsync(stlElement, pageInfo, contextInfo);
                var (pageCount, totalNum) = pageContentsElementParser.GetPageCount();

                await pageInfo.AddPageBodyCodeIfNotExistsAsync(PageInfo.Const.Jquery);
                await Parser.ParseAsync(pageInfo, contextInfo, contentBuilder, filePath, false);

                for (var currentPageIndex = 0; currentPageIndex < pageCount; currentPageIndex++)
                {
                    var thePageInfo = await pageInfo.CloneAsync();
                    var pageHtml = await pageContentsElementParser.ParseAsync(totalNum, currentPageIndex, pageCount, true);
                    var pagedBuilder =
                        new StringBuilder(contentBuilder.ToString().Replace(stlElementTranslated, pageHtml));

                    await StlParserManager.ReplacePageElementsInChannelPageAsync(pagedBuilder, thePageInfo, stlLabelList,
                        thePageInfo.PageChannelId, currentPageIndex, pageCount, totalNum);

                    filePath = await PathUtility.GetChannelPageFilePathAsync(site, thePageInfo.PageChannelId,
                        currentPageIndex);
                    thePageInfo.AddLastPageScript(pageInfo);

                    await GenerateFileAsync(filePath, pagedBuilder);

                    thePageInfo.ClearLastPageScript(pageInfo);
                    pageInfo.ClearLastPageScript();
                }
            }
            //如果标签中存在<stl:pageChannels>
            else if (StlParserUtility.IsStlElementExists(StlPageChannels.ElementName, stlLabelList))
            {
                var stlElement = StlParserUtility.GetStlElement(StlPageChannels.ElementName, stlLabelList);
                var stlElementTranslated = StlParserManager.StlEncrypt(stlElement);

                var pageChannelsElementParser = await StlPageChannels.GetAsync(stlElement, pageInfo, contextInfo);
                var pageCount = pageChannelsElementParser.GetPageCount(out var totalNum);

                await pageInfo.AddPageBodyCodeIfNotExistsAsync(PageInfo.Const.Jquery);
                await Parser.ParseAsync(pageInfo, contextInfo, contentBuilder, filePath, false);

                for (var currentPageIndex = 0; currentPageIndex < pageCount; currentPageIndex++)
                {
                    var thePageInfo = await pageInfo.CloneAsync();
                    var pageHtml = await pageChannelsElementParser.ParseAsync(currentPageIndex, pageCount);
                    var pagedBuilder =
                        new StringBuilder(contentBuilder.ToString().Replace(stlElementTranslated, pageHtml));

                    await StlParserManager.ReplacePageElementsInChannelPageAsync(pagedBuilder, thePageInfo, stlLabelList,
                        thePageInfo.PageChannelId, currentPageIndex, pageCount, totalNum);

                    filePath = await PathUtility.GetChannelPageFilePathAsync(site, thePageInfo.PageChannelId,
                        currentPageIndex);
                    await GenerateFileAsync(filePath, pagedBuilder);
                }
            }
            //如果标签中存在<stl:pageSqlContents>
            else if (StlParserUtility.IsStlElementExists(StlPageSqlContents.ElementName, stlLabelList))
            {
                var stlElement = StlParserUtility.GetStlElement(StlPageSqlContents.ElementName, stlLabelList);
                var stlElementTranslated = StlParserManager.StlEncrypt(stlElement);

                var pageSqlContentsElementParser = await StlPageSqlContents.GetAsync(stlElement, pageInfo, contextInfo);
                var pageCount = pageSqlContentsElementParser.GetPageCount(out var totalNum);

                await pageInfo.AddPageBodyCodeIfNotExistsAsync(PageInfo.Const.Jquery);
                await Parser.ParseAsync(pageInfo, contextInfo, contentBuilder, filePath, false);

                for (var currentPageIndex = 0; currentPageIndex < pageCount; currentPageIndex++)
                {
                    var thePageInfo = await pageInfo.CloneAsync();
                    var pageHtml = await pageSqlContentsElementParser.ParseAsync(totalNum, currentPageIndex, pageCount, true);
                    var pagedBuilder =
                        new StringBuilder(contentBuilder.ToString().Replace(stlElementTranslated, pageHtml));

                    await StlParserManager.ReplacePageElementsInChannelPageAsync(pagedBuilder, thePageInfo, stlLabelList,
                        thePageInfo.PageChannelId, currentPageIndex, pageCount, totalNum);

                    filePath = await PathUtility.GetChannelPageFilePathAsync(site, thePageInfo.PageChannelId,
                        currentPageIndex);
                    await GenerateFileAsync(filePath, pagedBuilder);
                }
            }
            else
            {
                await Parser.ParseAsync(pageInfo, contextInfo, contentBuilder, filePath, false);
                await GenerateFileAsync(filePath, contentBuilder);
            }
        }

        private async Task ExecuteContentAsync(Site site, Channel channel, int contentId)
        {
            var contentInfo = await _contentRepository.GetAsync(site, channel, contentId);

            if (contentInfo == null)
            {
                return;
            }

            if (!contentInfo.Checked)
            {
                await DeleteContentAsync(site, channel.Id, contentId);
                return;
            }

            if (!ContentUtility.IsCreatable(channel, contentInfo)) return;

            if (site.IsCreateStaticContentByAddDate &&
                contentInfo.AddDate < site.CreateStaticContentAddDate)
            {
                return;
            }

            var templateInfo = await _templateRepository.GetContentTemplateAsync(site.Id, channel);
            var pageInfo = await PageInfo.GetPageInfoAsync(channel.Id, contentId, site, templateInfo,
                new Dictionary<string, object>());
            var contextInfo = new ContextInfo(pageInfo)
            {
                ContextType = ContextType.Content
            };
            contextInfo.SetContentInfo(contentInfo);
            var filePath = await PathUtility.GetContentPageFilePathAsync(site, pageInfo.PageChannelId, contentInfo, 0);
            var contentBuilder = new StringBuilder(await _templateRepository.GetTemplateContentAsync(site, templateInfo));

            var stlLabelList = StlParserUtility.GetStlLabelList(contentBuilder.ToString());

            //如果标签中存在<stl:content type="PageContent"></stl:content>
            if (StlParserUtility.IsStlContentElementWithTypePageContent(stlLabelList)) //内容存在
            {
                var stlElement = StlParserUtility.GetStlContentElementWithTypePageContent(stlLabelList);
                var stlElementTranslated = StlParserManager.StlEncrypt(stlElement);
                contentBuilder.Replace(stlElement, stlElementTranslated);

                var innerBuilder = new StringBuilder(stlElement);
                await StlParserManager.ParseInnerContentAsync(innerBuilder, pageInfo, contextInfo);
                var pageContentHtml = innerBuilder.ToString();
                var pageCount = StringUtils.GetCount(Constants.PagePlaceHolder, pageContentHtml) + 1; //一共需要的页数

                await Parser.ParseAsync(pageInfo, contextInfo, contentBuilder, filePath, false);

                for (var currentPageIndex = 0; currentPageIndex < pageCount; currentPageIndex++)
                {
                    var thePageInfo = await pageInfo.CloneAsync();

                    var index = pageContentHtml.IndexOf(Constants.PagePlaceHolder, StringComparison.Ordinal);
                    var length = index == -1 ? pageContentHtml.Length : index;

                    var pageHtml = pageContentHtml.Substring(0, length);
                    var pagedBuilder =
                        new StringBuilder(contentBuilder.ToString().Replace(stlElementTranslated, pageHtml));
                    await StlParserManager.ReplacePageElementsInContentPageAsync(pagedBuilder, thePageInfo, stlLabelList,
                        channel.Id, contentId, currentPageIndex, pageCount);

                    filePath = await PathUtility.GetContentPageFilePathAsync(site, thePageInfo.PageChannelId, contentInfo,
                        currentPageIndex);
                    await GenerateFileAsync(filePath, pagedBuilder);

                    if (index != -1)
                    {
                        pageContentHtml = pageContentHtml.Substring(length + Constants.PagePlaceHolder.Length);
                    }
                }
            }
            //如果标签中存在<stl:channel type="PageContent"></stl:channel>
            else if (StlParserUtility.IsStlChannelElementWithTypePageContent(stlLabelList)) //内容存在
            {
                var stlElement = StlParserUtility.GetStlChannelElementWithTypePageContent(stlLabelList);
                var stlElementTranslated = StlParserManager.StlEncrypt(stlElement);
                contentBuilder.Replace(stlElement, stlElementTranslated);

                var innerBuilder = new StringBuilder(stlElement);
                await StlParserManager.ParseInnerContentAsync(innerBuilder, pageInfo, contextInfo);
                var pageContentHtml = innerBuilder.ToString();
                var pageCount = StringUtils.GetCount(Constants.PagePlaceHolder, pageContentHtml) + 1; //一共需要的页数

                await Parser.ParseAsync(pageInfo, contextInfo, contentBuilder, filePath, false);

                for (var currentPageIndex = 0; currentPageIndex < pageCount; currentPageIndex++)
                {
                    var thePageInfo = await pageInfo.CloneAsync();

                    var index = pageContentHtml.IndexOf(Constants.PagePlaceHolder, StringComparison.Ordinal);
                    var length = index == -1 ? pageContentHtml.Length : index;

                    var pageHtml = pageContentHtml.Substring(0, length);
                    var pagedBuilder =
                        new StringBuilder(contentBuilder.ToString().Replace(stlElementTranslated, pageHtml));
                    await StlParserManager.ReplacePageElementsInContentPageAsync(pagedBuilder, thePageInfo, stlLabelList,
                        channel.Id, contentId, currentPageIndex, pageCount);

                    filePath = await PathUtility.GetContentPageFilePathAsync(site, thePageInfo.PageChannelId, contentInfo,
                        currentPageIndex);
                    await GenerateFileAsync(filePath, pagedBuilder);

                    if (index != -1)
                    {
                        pageContentHtml = pageContentHtml.Substring(length + Constants.PagePlaceHolder.Length);
                    }
                }
            }
            //如果标签中存在<stl:pageContents>
            else if (StlParserUtility.IsStlElementExists(StlPageContents.ElementName, stlLabelList))
            {
                var stlElement = StlParserUtility.GetStlElement(StlPageContents.ElementName, stlLabelList);
                var stlElementTranslated = StlParserManager.StlEncrypt(stlElement);

                var pageContentsElementParser = await StlPageContents.GetAsync(stlElement, pageInfo, contextInfo);
                var (pageCount, totalNum) = pageContentsElementParser.GetPageCount();

                await Parser.ParseAsync(pageInfo, contextInfo, contentBuilder, filePath, false);

                for (var currentPageIndex = 0; currentPageIndex < pageCount; currentPageIndex++)
                {
                    var thePageInfo = await pageInfo.CloneAsync();
                    var pageHtml = await pageContentsElementParser.ParseAsync(totalNum, currentPageIndex, pageCount, true);
                    var pagedBuilder =
                        new StringBuilder(contentBuilder.ToString().Replace(stlElementTranslated, pageHtml));

                    await StlParserManager.ReplacePageElementsInContentPageAsync(pagedBuilder, thePageInfo, stlLabelList,
                        channel.Id, contentId, currentPageIndex, pageCount);

                    filePath = await PathUtility.GetContentPageFilePathAsync(site, thePageInfo.PageChannelId, contentInfo,
                        currentPageIndex);
                    await GenerateFileAsync(filePath, pagedBuilder);
                }
            }
            //如果标签中存在<stl:pageChannels>
            else if (StlParserUtility.IsStlElementExists(StlPageChannels.ElementName, stlLabelList))
            {
                var stlElement = StlParserUtility.GetStlElement(StlPageChannels.ElementName, stlLabelList);
                var stlElementTranslated = StlParserManager.StlEncrypt(stlElement);

                var pageChannelsElementParser = await StlPageChannels.GetAsync(stlElement, pageInfo, contextInfo);
                var pageCount = pageChannelsElementParser.GetPageCount(out _);

                await Parser.ParseAsync(pageInfo, contextInfo, contentBuilder, filePath, false);

                for (var currentPageIndex = 0; currentPageIndex < pageCount; currentPageIndex++)
                {
                    var thePageInfo = await pageInfo.CloneAsync();
                    var pageHtml = await pageChannelsElementParser.ParseAsync(currentPageIndex, pageCount);
                    var pagedBuilder =
                        new StringBuilder(contentBuilder.ToString().Replace(stlElementTranslated, pageHtml));

                    await StlParserManager.ReplacePageElementsInContentPageAsync(pagedBuilder, thePageInfo, stlLabelList,
                        channel.Id, contentId, currentPageIndex, pageCount);

                    filePath = await PathUtility.GetContentPageFilePathAsync(site, thePageInfo.PageChannelId, contentInfo,
                        currentPageIndex);
                    await GenerateFileAsync(filePath, pagedBuilder);
                }
            }
            //如果标签中存在<stl:pageSqlContents>
            else if (StlParserUtility.IsStlElementExists(StlPageSqlContents.ElementName, stlLabelList))
            {
                var stlElement = StlParserUtility.GetStlElement(StlPageSqlContents.ElementName, stlLabelList);
                var stlElementTranslated = StlParserManager.StlEncrypt(stlElement);

                var pageSqlContentsElementParser = await StlPageSqlContents.GetAsync(stlElement, pageInfo, contextInfo);
                var pageCount = pageSqlContentsElementParser.GetPageCount(out var totalNum);

                await Parser.ParseAsync(pageInfo, contextInfo, contentBuilder, filePath, false);

                for (var currentPageIndex = 0; currentPageIndex < pageCount; currentPageIndex++)
                {
                    var thePageInfo = await pageInfo.CloneAsync();
                    var pageHtml = await pageSqlContentsElementParser.ParseAsync(totalNum, currentPageIndex, pageCount, true);
                    var pagedBuilder =
                        new StringBuilder(contentBuilder.ToString().Replace(stlElementTranslated, pageHtml));

                    await StlParserManager.ReplacePageElementsInContentPageAsync(pagedBuilder, thePageInfo, stlLabelList,
                        channel.Id, contentId, currentPageIndex, pageCount);

                    filePath = await PathUtility.GetContentPageFilePathAsync(site, thePageInfo.PageChannelId, contentInfo,
                        currentPageIndex);
                    await GenerateFileAsync(filePath, pagedBuilder);
                }
            }
            else //无翻页
            {
                await Parser.ParseAsync(pageInfo, contextInfo, contentBuilder, filePath, false);
                await GenerateFileAsync(filePath, contentBuilder);
            }
        }

        private async Task ExecuteFileAsync(int siteId, int fileTemplateId)
        {
            var site = await _siteRepository.GetAsync(siteId);
            var templateInfo = await _templateRepository.GetAsync(fileTemplateId);
            if (templateInfo == null || templateInfo.TemplateType != TemplateType.FileTemplate)
            {
                return;
            }

            var pageInfo = await PageInfo.GetPageInfoAsync(siteId, 0, site, templateInfo, new Dictionary<string, object>());
            var contextInfo = new ContextInfo(pageInfo);
            var filePath = await PathUtility.MapPathAsync(site, templateInfo.CreatedFileFullName);

            var contentBuilder = new StringBuilder(await _templateRepository.GetTemplateContentAsync(site, templateInfo));
            await Parser.ParseAsync(pageInfo, contextInfo, contentBuilder, filePath, false);
            await GenerateFileAsync(filePath, contentBuilder);
        }

        private async Task ExecuteSpecialAsync(int siteId, int specialId)
        {
            var site = await _siteRepository.GetAsync(siteId);
            var templateInfoList = await _specialRepository.GetTemplateListAsync(site, specialId);
            foreach (var templateInfo in templateInfoList)
            {
                var pageInfo = await PageInfo.GetPageInfoAsync(siteId, 0, site, templateInfo, new Dictionary<string, object>());
                var contextInfo = new ContextInfo(pageInfo);
                var filePath = await PathUtility.MapPathAsync(site, templateInfo.CreatedFileFullName);

                var contentBuilder = new StringBuilder(templateInfo.Content);
                await Parser.ParseAsync(pageInfo, contextInfo, contentBuilder, filePath, false);
                await GenerateFileAsync(filePath, contentBuilder);
            }
        }

        private async Task GenerateFileAsync(string filePath, StringBuilder contentBuilder)
        {
            if (string.IsNullOrEmpty(filePath)) return;

            try
            {
                await FileUtils.WriteTextAsync(filePath, contentBuilder.ToString());
            }
            catch
            {
                FileUtils.RemoveReadOnlyAndHiddenIfExists(filePath);
                await FileUtils.WriteTextAsync(filePath, contentBuilder.ToString());
            }
        }
    }
}