using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using SiteServer.Abstractions;
using SiteServer.CMS.Core;
using SiteServer.CMS.Core.Create;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Repositories;
using SiteServer.CMS.StlParser.Model;
using SiteServer.CMS.StlParser.StlElement;
using SiteServer.CMS.StlParser.Utility;


namespace SiteServer.CMS.StlParser
{
    public static class FileSystemObjectAsync
    {
        public static async Task ExecuteAsync(int siteId, CreateType createType, int channelId, int contentId,
            int fileTemplateId, int specialId)
        {
            if (createType == CreateType.Channel)
            {
                await CreateChannelAsync(siteId, channelId);
            }
            else if (createType == CreateType.Content)
            {
                var site = await DataProvider.SiteRepository.GetAsync(siteId);
                var channelInfo = await ChannelManager.GetChannelAsync(siteId, channelId);
                await CreateContentAsync(site, channelInfo, contentId);
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

        private static async Task CreateContentsAsync(int siteId, int channelId)
        {
            var site = await DataProvider.SiteRepository.GetAsync(siteId);
            var channelInfo = await ChannelManager.GetChannelAsync(siteId, channelId);

            var tableName = await ChannelManager.GetTableNameAsync(site, channelInfo);
            var orderByString = ETaxisTypeUtils.GetContentOrderByString(TaxisType.OrderByTaxisDesc);
            var contentIdList = DataProvider.ContentRepository.GetContentIdListChecked(tableName, channelId, orderByString);

            foreach (var contentId in contentIdList)
            {
                await CreateContentAsync(site, channelInfo, contentId);
            }
        }

        private static async Task CreateChannelAsync(int siteId, int channelId)
        {
            var site = await DataProvider.SiteRepository.GetAsync(siteId);
            var channelInfo = await ChannelManager.GetChannelAsync(siteId, channelId);

            if (!await ChannelManager.IsCreatableAsync(site, channelInfo)) return;

            var templateInfo = channelId == siteId
                ? await TemplateManager.GetIndexPageTemplateAsync(siteId)
                : await TemplateManager.GetChannelTemplateAsync(siteId, channelId);
            var filePath = await PathUtility.GetChannelPageFilePathAsync(site, channelId, 0);
            var pageInfo = await PageInfo.GetPageInfoAsync(channelId, 0, site, templateInfo, new Dictionary<string, object>());
            var contextInfo = new ContextInfo(pageInfo)
            {
                ContextType = EContextType.Channel
            };
            var contentBuilder = new StringBuilder(TemplateManager.GetTemplateContent(site, templateInfo));

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
                var pageCount = StringUtils.GetCount(ContentUtility.PagePlaceHolder, pageContentHtml) + 1; //一共需要的页数

                await Parser.ParseAsync(pageInfo, contextInfo, contentBuilder, filePath, false);

                for (var currentPageIndex = 0; currentPageIndex < pageCount; currentPageIndex++)
                {
                    var thePageInfo = await pageInfo.CloneAsync();

                    var index = pageContentHtml.IndexOf(ContentUtility.PagePlaceHolder, StringComparison.Ordinal);
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
                        pageContentHtml = pageContentHtml.Substring(length + ContentUtility.PagePlaceHolder.Length);
                    }
                }
            }
            //如果标签中存在<stl:pageContents>
            else if (StlParserUtility.IsStlElementExists(StlPageContents.ElementName, stlLabelList))
            {
                var stlElement = StlParserUtility.GetStlElement(StlPageContents.ElementName, stlLabelList);
                var stlElementTranslated = StlParserManager.StlEncrypt(stlElement);

                var pageContentsElementParser = await StlPageContents.GetAsync(stlElement, pageInfo, contextInfo);
                var (pageCount, totalNum) = await pageContentsElementParser.GetPageCountAsync();

                pageInfo.AddPageBodyCodeIfNotExists(PageInfo.Const.Jquery);
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

                pageInfo.AddPageBodyCodeIfNotExists(PageInfo.Const.Jquery);
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

                pageInfo.AddPageBodyCodeIfNotExists(PageInfo.Const.Jquery);
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

        private static async Task CreateContentAsync(Site site, Channel channel, int contentId)
        {
            var contentInfo = await DataProvider.ContentRepository.GetAsync(site, channel, contentId);

            if (contentInfo == null)
            {
                return;
            }

            if (!contentInfo.Checked)
            {
                await DeleteManager.DeleteContentAsync(site, channel.Id, contentId);
                return;
            }

            if (!ContentUtility.IsCreatable(channel, contentInfo)) return;

            if (site.IsCreateStaticContentByAddDate &&
                contentInfo.AddDate < site.CreateStaticContentAddDate)
            {
                return;
            }

            var templateInfo = await TemplateManager.GetContentTemplateAsync(site.Id, channel.Id);
            var pageInfo = await PageInfo.GetPageInfoAsync(channel.Id, contentId, site, templateInfo,
                new Dictionary<string, object>());
            var contextInfo = new ContextInfo(pageInfo)
            {
                ContextType = EContextType.Content
            };
            contextInfo.SetContentInfo(contentInfo);
            var filePath = await PathUtility.GetContentPageFilePathAsync(site, pageInfo.PageChannelId, contentInfo, 0);
            var contentBuilder = new StringBuilder(TemplateManager.GetTemplateContent(site, templateInfo));

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
                var pageCount = StringUtils.GetCount(ContentUtility.PagePlaceHolder, pageContentHtml) + 1; //一共需要的页数

                await Parser.ParseAsync(pageInfo, contextInfo, contentBuilder, filePath, false);

                for (var currentPageIndex = 0; currentPageIndex < pageCount; currentPageIndex++)
                {
                    var thePageInfo = await pageInfo.CloneAsync();

                    var index = pageContentHtml.IndexOf(ContentUtility.PagePlaceHolder, StringComparison.Ordinal);
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
                        pageContentHtml = pageContentHtml.Substring(length + ContentUtility.PagePlaceHolder.Length);
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
                var pageCount = StringUtils.GetCount(ContentUtility.PagePlaceHolder, pageContentHtml) + 1; //一共需要的页数

                await Parser.ParseAsync(pageInfo, contextInfo, contentBuilder, filePath, false);

                for (var currentPageIndex = 0; currentPageIndex < pageCount; currentPageIndex++)
                {
                    var thePageInfo = await pageInfo.CloneAsync();

                    var index = pageContentHtml.IndexOf(ContentUtility.PagePlaceHolder, StringComparison.Ordinal);
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
                        pageContentHtml = pageContentHtml.Substring(length + ContentUtility.PagePlaceHolder.Length);
                    }
                }
            }
            //如果标签中存在<stl:pageContents>
            else if (StlParserUtility.IsStlElementExists(StlPageContents.ElementName, stlLabelList))
            {
                var stlElement = StlParserUtility.GetStlElement(StlPageContents.ElementName, stlLabelList);
                var stlElementTranslated = StlParserManager.StlEncrypt(stlElement);

                var pageContentsElementParser = await StlPageContents.GetAsync(stlElement, pageInfo, contextInfo);
                var (pageCount, totalNum) = await pageContentsElementParser.GetPageCountAsync();

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

        private static async Task CreateFileAsync(int siteId, int fileTemplateId)
        {
            var site = await DataProvider.SiteRepository.GetAsync(siteId);
            var templateInfo = await TemplateManager.GetTemplateAsync(siteId, fileTemplateId);
            if (templateInfo == null || templateInfo.TemplateType != TemplateType.FileTemplate)
            {
                return;
            }

            var pageInfo = await PageInfo.GetPageInfoAsync(siteId, 0, site, templateInfo, new Dictionary<string, object>());
            var contextInfo = new ContextInfo(pageInfo);
            var filePath = PathUtility.MapPath(site, templateInfo.CreatedFileFullName);

            var contentBuilder = new StringBuilder(TemplateManager.GetTemplateContent(site, templateInfo));
            await Parser.ParseAsync(pageInfo, contextInfo, contentBuilder, filePath, false);
            await GenerateFileAsync(filePath, contentBuilder);
        }

        private static async Task CreateSpecialAsync(int siteId, int specialId)
        {
            var site = await DataProvider.SiteRepository.GetAsync(siteId);
            var templateInfoList = await SpecialManager.GetTemplateListAsync(site, specialId);
            foreach (var templateInfo in templateInfoList)
            {
                var pageInfo = await PageInfo.GetPageInfoAsync(siteId, 0, site, templateInfo, new Dictionary<string, object>());
                var contextInfo = new ContextInfo(pageInfo);
                var filePath = PathUtility.MapPath(site, templateInfo.CreatedFileFullName);

                var contentBuilder = new StringBuilder(templateInfo.Content);
                await Parser.ParseAsync(pageInfo, contextInfo, contentBuilder, filePath, false);
                await GenerateFileAsync(filePath, contentBuilder);
            }
        }

        //private string CreateIncludeFile(string virtualUrl, bool isCreateIfExists)
        //{
        //    var template = new Template(0, SiteId, string.Empty, TemplateType.FileTemplate, string.Empty, string.Empty, string.Empty, ECharsetUtils.GetEnumType(Site.Charset), false);
        //    var pageInfo = new PageInfo(SiteId, 0, Site, template, null);
        //    var contextInfo = new ContextInfo(pageInfo);

        //    var parsedVirtualUrl = virtualUrl.Substring(0, virtualUrl.LastIndexOf('.')) + "_parsed" + virtualUrl.Substring(virtualUrl.LastIndexOf('.'));
        //    var filePath = PathUtility.MapPath(Site, parsedVirtualUrl);
        //    if (!isCreateIfExists && FileUtils.IsFileExists(filePath)) return parsedVirtualUrl;

        //    var contentBuilder = new StringBuilder(StlCacheManager.FileContent.GetIncludeContent(Site, virtualUrl, pageInfo.Template.Charset));
        //    StlParserManager.ParseTemplateContent(contentBuilder, pageInfo, contextInfo);
        //    var pageAfterBodyScripts = StlParserManager.GetPageInfoScript(pageInfo, true);
        //    var pageBeforeBodyScripts = StlParserManager.GetPageInfoScript(pageInfo, false);
        //    contentBuilder.Insert(0, pageBeforeBodyScripts);
        //    contentBuilder.Append(pageAfterBodyScripts);
        //    GenerateFile(filePath, pageInfo.Template.Charset, contentBuilder);
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