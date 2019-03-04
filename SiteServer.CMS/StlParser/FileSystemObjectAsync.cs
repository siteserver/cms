using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using SiteServer.Utils;
using SiteServer.CMS.Core;
using SiteServer.CMS.Core.Create;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.DataCache.Content;
using SiteServer.CMS.DataCache.Stl;
using SiteServer.CMS.Model;
using SiteServer.CMS.Model.Attributes;
using SiteServer.CMS.Model.Enumerations;
using SiteServer.CMS.StlParser.Model;
using SiteServer.CMS.StlParser.StlElement;
using SiteServer.CMS.StlParser.Utility;
using SiteServer.Plugin;
using SiteServer.Utils.Enumerations;

namespace SiteServer.CMS.StlParser
{
    public static class FileSystemObjectAsync
    {
        public static async Task ExecuteAsync(int siteId, ECreateType createType, int channelId, int contentId,
            int fileTemplateId, int specialId)
        {
            if (createType == ECreateType.Channel)
            {
                await CreateChannelAsync(siteId, channelId);
            }
            else if (createType == ECreateType.Content)
            {
                var siteInfo = SiteManager.GetSiteInfo(siteId);
                var channelInfo = ChannelManager.GetChannelInfo(siteId, channelId);
                await CreateContentAsync(siteInfo, channelInfo, contentId);
            }
            else if (createType == ECreateType.AllContent)
            {
                await CreateContentsAsync(siteId, channelId);
            }
            else if (createType == ECreateType.File)
            {
                await CreateFileAsync(siteId, fileTemplateId);
            }
            else if (createType == ECreateType.Special)
            {
                await CreateSpecialAsync(siteId, specialId);
            }
        }

        private static async Task CreateContentsAsync(int siteId, int channelId)
        {
            var siteInfo = SiteManager.GetSiteInfo(siteId);
            var channelInfo = ChannelManager.GetChannelInfo(siteId, channelId);

            var tableName = ChannelManager.GetTableName(siteInfo, channelInfo);
            var orderByString = ETaxisTypeUtils.GetContentOrderByString(ETaxisType.OrderByTaxisDesc);
            var contentIdList = StlContentCache.GetContentIdListChecked(tableName, channelId, orderByString);

            foreach (var contentId in contentIdList)
            {
                await CreateContentAsync(siteInfo, channelInfo, contentId);
            }
        }

        private static async Task CreateChannelAsync(int siteId, int channelId)
        {
            var siteInfo = SiteManager.GetSiteInfo(siteId);
            var channelInfo = ChannelManager.GetChannelInfo(siteId, channelId);

            if (!ChannelManager.IsCreatable(siteInfo, channelInfo)) return;

            var templateInfo = channelId == siteId
                ? TemplateManager.GetIndexPageTemplateInfo(siteId)
                : TemplateManager.GetChannelTemplateInfo(siteId, channelId);
            var filePath = PathUtility.GetChannelPageFilePath(siteInfo, channelId, 0);
            var pageInfo = new PageInfo(channelId, 0, siteInfo, templateInfo, new Dictionary<string, object>());
            var contextInfo = new ContextInfo(pageInfo)
            {
                ContextType = EContextType.Channel
            };
            var contentBuilder = new StringBuilder(TemplateManager.GetTemplateContent(siteInfo, templateInfo));

            var stlLabelList = StlParserUtility.GetStlLabelList(contentBuilder.ToString());
            var stlPageContentElement = string.Empty;
            foreach (var label in stlLabelList)
            {
                if (!StlParserUtility.IsStlChannelElement(label, ChannelAttribute.PageContent)) continue;
                stlPageContentElement = label;
                break;
            }

            //如果标签中存在<stl:channel type="PageContent"></stl:channel>
            if (!string.IsNullOrEmpty(stlPageContentElement)) //内容存在
            {
                var innerBuilder = new StringBuilder(stlPageContentElement);
                StlParserManager.ParseInnerContent(innerBuilder, pageInfo, contextInfo);
                var contentAttributeHtml = innerBuilder.ToString();
                var pageCount =
                    StringUtils.GetCount(ContentUtility.PagePlaceHolder, contentAttributeHtml) + 1; //一共需要的页数

                pageInfo.AddPageBodyCodeIfNotExists(PageInfo.Const.Jquery);
                Parser.Parse(pageInfo, contextInfo, contentBuilder, filePath, false);

                for (var currentPageIndex = 0; currentPageIndex < pageCount; currentPageIndex++)
                {
                    var thePageInfo = pageInfo.Clone();
                    var index = contentAttributeHtml.IndexOf(ContentUtility.PagePlaceHolder, StringComparison.Ordinal);
                    var length = index == -1 ? contentAttributeHtml.Length : index;
                    var pagedContentAttributeHtml = contentAttributeHtml.Substring(0, length);
                    var pagedBuilder = new StringBuilder(contentBuilder.ToString()
                        .Replace(stlPageContentElement, pagedContentAttributeHtml));
                    StlParserManager.ReplacePageElementsInChannelPage(pagedBuilder, thePageInfo, stlLabelList,
                        thePageInfo.PageChannelId, currentPageIndex, pageCount, 0);

                    filePath = PathUtility.GetChannelPageFilePath(siteInfo, thePageInfo.PageChannelId,
                        currentPageIndex);

                    await GenerateFileAsync(filePath, pageInfo.TemplateInfo.Charset, pagedBuilder);

                    if (index != -1)
                    {
                        contentAttributeHtml =
                            contentAttributeHtml.Substring(length + ContentUtility.PagePlaceHolder.Length);
                    }
                }
            }
            //如果标签中存在<stl:pageContents>
            else if (StlParserUtility.IsStlElementExists(StlPageContents.ElementName, stlLabelList))
            {
                var stlElement = StlParserUtility.GetStlElement(StlPageContents.ElementName, stlLabelList);
                var stlElementTranslated = StlParserManager.StlEncrypt(stlElement);

                var pageContentsElementParser = new StlPageContents(stlElement, pageInfo, contextInfo);
                var pageCount = pageContentsElementParser.GetPageCount(out var totalNum);

                pageInfo.AddPageBodyCodeIfNotExists(PageInfo.Const.Jquery);
                Parser.Parse(pageInfo, contextInfo, contentBuilder, filePath, false);

                for (var currentPageIndex = 0; currentPageIndex < pageCount; currentPageIndex++)
                {
                    var thePageInfo = pageInfo.Clone();
                    var pageHtml = pageContentsElementParser.Parse(totalNum, currentPageIndex, pageCount, true);
                    var pagedBuilder =
                        new StringBuilder(contentBuilder.ToString().Replace(stlElementTranslated, pageHtml));

                    StlParserManager.ReplacePageElementsInChannelPage(pagedBuilder, thePageInfo, stlLabelList,
                        thePageInfo.PageChannelId, currentPageIndex, pageCount, totalNum);

                    filePath = PathUtility.GetChannelPageFilePath(siteInfo, thePageInfo.PageChannelId,
                        currentPageIndex);
                    thePageInfo.AddLastPageScript(pageInfo);

                    await GenerateFileAsync(filePath, pageInfo.TemplateInfo.Charset, pagedBuilder);

                    thePageInfo.ClearLastPageScript(pageInfo);
                    pageInfo.ClearLastPageScript();
                }
            }
            //如果标签中存在<stl:pageChannels>
            else if (StlParserUtility.IsStlElementExists(StlPageChannels.ElementName, stlLabelList))
            {
                var stlElement = StlParserUtility.GetStlElement(StlPageChannels.ElementName, stlLabelList);
                var stlElementTranslated = StlParserManager.StlEncrypt(stlElement);

                var pageChannelsElementParser = new StlPageChannels(stlElement, pageInfo, contextInfo);
                var pageCount = pageChannelsElementParser.GetPageCount(out var totalNum);

                pageInfo.AddPageBodyCodeIfNotExists(PageInfo.Const.Jquery);
                Parser.Parse(pageInfo, contextInfo, contentBuilder, filePath, false);

                for (var currentPageIndex = 0; currentPageIndex < pageCount; currentPageIndex++)
                {
                    var thePageInfo = pageInfo.Clone();
                    var pageHtml = pageChannelsElementParser.Parse(currentPageIndex, pageCount);
                    var pagedBuilder =
                        new StringBuilder(contentBuilder.ToString().Replace(stlElementTranslated, pageHtml));

                    StlParserManager.ReplacePageElementsInChannelPage(pagedBuilder, thePageInfo, stlLabelList,
                        thePageInfo.PageChannelId, currentPageIndex, pageCount, totalNum);

                    filePath = PathUtility.GetChannelPageFilePath(siteInfo, thePageInfo.PageChannelId,
                        currentPageIndex);
                    await GenerateFileAsync(filePath, pageInfo.TemplateInfo.Charset, pagedBuilder);
                }
            }
            //如果标签中存在<stl:pageSqlContents>
            else if (StlParserUtility.IsStlElementExists(StlPageSqlContents.ElementName, stlLabelList))
            {
                var stlElement = StlParserUtility.GetStlElement(StlPageSqlContents.ElementName, stlLabelList);
                var stlElementTranslated = StlParserManager.StlEncrypt(stlElement);

                var pageSqlContentsElementParser = new StlPageSqlContents(stlElement, pageInfo, contextInfo);
                var pageCount = pageSqlContentsElementParser.GetPageCount(out var totalNum);

                pageInfo.AddPageBodyCodeIfNotExists(PageInfo.Const.Jquery);
                Parser.Parse(pageInfo, contextInfo, contentBuilder, filePath, false);

                for (var currentPageIndex = 0; currentPageIndex < pageCount; currentPageIndex++)
                {
                    var thePageInfo = pageInfo.Clone();
                    var pageHtml = pageSqlContentsElementParser.Parse(totalNum, currentPageIndex, pageCount, true);
                    var pagedBuilder =
                        new StringBuilder(contentBuilder.ToString().Replace(stlElementTranslated, pageHtml));

                    StlParserManager.ReplacePageElementsInChannelPage(pagedBuilder, thePageInfo, stlLabelList,
                        thePageInfo.PageChannelId, currentPageIndex, pageCount, totalNum);

                    filePath = PathUtility.GetChannelPageFilePath(siteInfo, thePageInfo.PageChannelId,
                        currentPageIndex);
                    await GenerateFileAsync(filePath, pageInfo.TemplateInfo.Charset, pagedBuilder);
                }
            }
            else
            {
                Parser.Parse(pageInfo, contextInfo, contentBuilder, filePath, false);
                await GenerateFileAsync(filePath, pageInfo.TemplateInfo.Charset, contentBuilder);
            }
        }

        private static async Task CreateContentAsync(SiteInfo siteInfo, ChannelInfo channelInfo, int contentId)
        {
            var contentInfo = ContentManager.GetContentInfo(siteInfo, channelInfo, contentId);

            if (contentInfo == null)
            {
                return;
            }

            if (!contentInfo.IsChecked)
            {
                DeleteManager.DeleteContent(siteInfo, channelInfo.Id, contentId);
                return;
            }


            if (!ContentManager.IsCreatable(channelInfo, contentInfo)) return;
            

            if (siteInfo.Additional.IsCreateStaticContentByAddDate &&
                contentInfo.AddDate < siteInfo.Additional.CreateStaticContentAddDate)
            {
                return;
            }

            var templateInfo = TemplateManager.GetContentTemplateInfo(siteInfo.Id, channelInfo.Id);
            var pageInfo = new PageInfo(channelInfo.Id, contentId, siteInfo, templateInfo,
                new Dictionary<string, object>());
            var contextInfo = new ContextInfo(pageInfo)
            {
                ContextType = EContextType.Content,
                ContentInfo = contentInfo
            };
            var filePath = PathUtility.GetContentPageFilePath(siteInfo, pageInfo.PageChannelId, contentInfo, 0);
            var contentBuilder = new StringBuilder(TemplateManager.GetTemplateContent(siteInfo, templateInfo));

            var stlLabelList = StlParserUtility.GetStlLabelList(contentBuilder.ToString());

            //如果标签中存在<stl:content type="PageContent"></stl:content>
            if (StlParserUtility.IsStlContentElementWithTypePageContent(stlLabelList)) //内容存在
            {
                var stlElement = StlParserUtility.GetStlContentElementWithTypePageContent(stlLabelList);
                var stlElementTranslated = StlParserManager.StlEncrypt(stlElement);
                contentBuilder.Replace(stlElement, stlElementTranslated);

                var innerBuilder = new StringBuilder(stlElement);
                StlParserManager.ParseInnerContent(innerBuilder, pageInfo, contextInfo);
                var pageContentHtml = innerBuilder.ToString();
                var pageCount = StringUtils.GetCount(ContentUtility.PagePlaceHolder, pageContentHtml) + 1; //一共需要的页数

                Parser.Parse(pageInfo, contextInfo, contentBuilder, filePath, false);

                for (var currentPageIndex = 0; currentPageIndex < pageCount; currentPageIndex++)
                {
                    var thePageInfo = pageInfo.Clone();

                    var index = pageContentHtml.IndexOf(ContentUtility.PagePlaceHolder, StringComparison.Ordinal);
                    var length = index == -1 ? pageContentHtml.Length : index;

                    var pageHtml = pageContentHtml.Substring(0, length);
                    var pagedBuilder =
                        new StringBuilder(contentBuilder.ToString().Replace(stlElementTranslated, pageHtml));
                    StlParserManager.ReplacePageElementsInContentPage(pagedBuilder, thePageInfo, stlLabelList,
                        channelInfo.Id, contentId, currentPageIndex, pageCount);

                    filePath = PathUtility.GetContentPageFilePath(siteInfo, thePageInfo.PageChannelId, contentInfo,
                        currentPageIndex);
                    await GenerateFileAsync(filePath, pageInfo.TemplateInfo.Charset, pagedBuilder);

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

                var pageContentsElementParser = new StlPageContents(stlElement, pageInfo, contextInfo);
                var pageCount = pageContentsElementParser.GetPageCount(out var totalNum);

                Parser.Parse(pageInfo, contextInfo, contentBuilder, filePath, false);

                for (var currentPageIndex = 0; currentPageIndex < pageCount; currentPageIndex++)
                {
                    var thePageInfo = pageInfo.Clone();
                    var pageHtml = pageContentsElementParser.Parse(totalNum, currentPageIndex, pageCount, true);
                    var pagedBuilder =
                        new StringBuilder(contentBuilder.ToString().Replace(stlElementTranslated, pageHtml));

                    StlParserManager.ReplacePageElementsInContentPage(pagedBuilder, thePageInfo, stlLabelList,
                        channelInfo.Id, contentId, currentPageIndex, pageCount);

                    filePath = PathUtility.GetContentPageFilePath(siteInfo, thePageInfo.PageChannelId, contentInfo,
                        currentPageIndex);
                    await GenerateFileAsync(filePath, pageInfo.TemplateInfo.Charset, pagedBuilder);
                }
            }
            //如果标签中存在<stl:pageChannels>
            else if (StlParserUtility.IsStlElementExists(StlPageChannels.ElementName, stlLabelList))
            {
                var stlElement = StlParserUtility.GetStlElement(StlPageChannels.ElementName, stlLabelList);
                var stlElementTranslated = StlParserManager.StlEncrypt(stlElement);

                var pageChannelsElementParser = new StlPageChannels(stlElement, pageInfo, contextInfo);
                var pageCount = pageChannelsElementParser.GetPageCount(out _);

                Parser.Parse(pageInfo, contextInfo, contentBuilder, filePath, false);

                for (var currentPageIndex = 0; currentPageIndex < pageCount; currentPageIndex++)
                {
                    var thePageInfo = pageInfo.Clone();
                    var pageHtml = pageChannelsElementParser.Parse(currentPageIndex, pageCount);
                    var pagedBuilder =
                        new StringBuilder(contentBuilder.ToString().Replace(stlElementTranslated, pageHtml));

                    StlParserManager.ReplacePageElementsInContentPage(pagedBuilder, thePageInfo, stlLabelList,
                        channelInfo.Id, contentId, currentPageIndex, pageCount);

                    filePath = PathUtility.GetContentPageFilePath(siteInfo, thePageInfo.PageChannelId, contentInfo,
                        currentPageIndex);
                    await GenerateFileAsync(filePath, pageInfo.TemplateInfo.Charset, pagedBuilder);
                }
            }
            //如果标签中存在<stl:pageSqlContents>
            else if (StlParserUtility.IsStlElementExists(StlPageSqlContents.ElementName, stlLabelList))
            {
                var stlElement = StlParserUtility.GetStlElement(StlPageSqlContents.ElementName, stlLabelList);
                var stlElementTranslated = StlParserManager.StlEncrypt(stlElement);

                var pageSqlContentsElementParser = new StlPageSqlContents(stlElement, pageInfo, contextInfo);
                var pageCount = pageSqlContentsElementParser.GetPageCount(out var totalNum);

                Parser.Parse(pageInfo, contextInfo, contentBuilder, filePath, false);

                for (var currentPageIndex = 0; currentPageIndex < pageCount; currentPageIndex++)
                {
                    var thePageInfo = pageInfo.Clone();
                    var pageHtml = pageSqlContentsElementParser.Parse(totalNum, currentPageIndex, pageCount, true);
                    var pagedBuilder =
                        new StringBuilder(contentBuilder.ToString().Replace(stlElementTranslated, pageHtml));

                    StlParserManager.ReplacePageElementsInContentPage(pagedBuilder, thePageInfo, stlLabelList,
                        channelInfo.Id, contentId, currentPageIndex, pageCount);

                    filePath = PathUtility.GetContentPageFilePath(siteInfo, thePageInfo.PageChannelId, contentInfo,
                        currentPageIndex);
                    await GenerateFileAsync(filePath, pageInfo.TemplateInfo.Charset, pagedBuilder);
                }
            }
            else //无翻页
            {
                Parser.Parse(pageInfo, contextInfo, contentBuilder, filePath, false);
                await GenerateFileAsync(filePath, pageInfo.TemplateInfo.Charset, contentBuilder);
            }
        }

        private static async Task CreateFileAsync(int siteId, int fileTemplateId)
        {
            var siteInfo = SiteManager.GetSiteInfo(siteId);
            var templateInfo = TemplateManager.GetTemplateInfo(siteId, fileTemplateId);
            if (templateInfo == null || templateInfo.TemplateType != TemplateType.FileTemplate)
            {
                return;
            }

            var pageInfo = new PageInfo(siteId, 0, siteInfo, templateInfo, new Dictionary<string, object>());
            var contextInfo = new ContextInfo(pageInfo);
            var filePath = PathUtility.MapPath(siteInfo, templateInfo.CreatedFileFullName);

            var contentBuilder = new StringBuilder(TemplateManager.GetTemplateContent(siteInfo, templateInfo));
            Parser.Parse(pageInfo, contextInfo, contentBuilder, filePath, false);
            await GenerateFileAsync(filePath, pageInfo.TemplateInfo.Charset, contentBuilder);
        }

        private static async Task CreateSpecialAsync(int siteId, int specialId)
        {
            var siteInfo = SiteManager.GetSiteInfo(siteId);
            var templateInfoList = SpecialManager.GetTemplateInfoList(siteInfo, specialId);
            foreach (var templateInfo in templateInfoList)
            {
                var pageInfo = new PageInfo(siteId, 0, siteInfo, templateInfo, new Dictionary<string, object>());
                var contextInfo = new ContextInfo(pageInfo);
                var filePath = PathUtility.MapPath(siteInfo, templateInfo.CreatedFileFullName);

                var contentBuilder = new StringBuilder(templateInfo.Content);
                Parser.Parse(pageInfo, contextInfo, contentBuilder, filePath, false);
                await GenerateFileAsync(filePath, pageInfo.TemplateInfo.Charset, contentBuilder);
            }
        }

        //private string CreateIncludeFile(string virtualUrl, bool isCreateIfExists)
        //{
        //    var templateInfo = new TemplateInfo(0, SiteId, string.Empty, TemplateType.FileTemplate, string.Empty, string.Empty, string.Empty, ECharsetUtils.GetEnumType(SiteInfo.Additional.Charset), false);
        //    var pageInfo = new PageInfo(SiteId, 0, SiteInfo, templateInfo, null);
        //    var contextInfo = new ContextInfo(pageInfo);

        //    var parsedVirtualUrl = virtualUrl.Substring(0, virtualUrl.LastIndexOf('.')) + "_parsed" + virtualUrl.Substring(virtualUrl.LastIndexOf('.'));
        //    var filePath = PathUtility.MapPath(SiteInfo, parsedVirtualUrl);
        //    if (!isCreateIfExists && FileUtils.IsFileExists(filePath)) return parsedVirtualUrl;

        //    var contentBuilder = new StringBuilder(StlCacheManager.FileContent.GetIncludeContent(SiteInfo, virtualUrl, pageInfo.TemplateInfo.Charset));
        //    StlParserManager.ParseTemplateContent(contentBuilder, pageInfo, contextInfo);
        //    var pageAfterBodyScripts = StlParserManager.GetPageInfoScript(pageInfo, true);
        //    var pageBeforeBodyScripts = StlParserManager.GetPageInfoScript(pageInfo, false);
        //    contentBuilder.Insert(0, pageBeforeBodyScripts);
        //    contentBuilder.Append(pageAfterBodyScripts);
        //    GenerateFile(filePath, pageInfo.TemplateInfo.Charset, contentBuilder);
        //    return parsedVirtualUrl;
        //}

        /// <summary>
        /// 在操作系统中创建文件，如果文件存在，重新创建此文件
        /// </summary>
        private static async Task GenerateFileAsync(string filePath, ECharset charset, StringBuilder contentBuilder)
        {
            if (string.IsNullOrEmpty(filePath)) return;

            try
            {
                await FileUtils.WriteTextAsync(filePath, ECharsetUtils.GetEncoding(charset), contentBuilder.ToString());
            }
            catch
            {
                FileUtils.RemoveReadOnlyAndHiddenIfExists(filePath);
                await FileUtils.WriteTextAsync(filePath, ECharsetUtils.GetEncoding(charset), contentBuilder.ToString());
            }
        }
    }
}