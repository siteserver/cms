using System;
using System.Text;
using System.Threading.Tasks;
using SiteServer.Utils;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;
using SiteServer.CMS.Model.Enumerations;
using SiteServer.CMS.StlParser.Cache;
using SiteServer.CMS.StlParser.Model;
using SiteServer.CMS.StlParser.StlElement;
using SiteServer.CMS.StlParser.Utility;
using SiteServer.Plugin;
using SiteServer.Utils.Enumerations;

namespace SiteServer.CMS.StlParser
{
    public class FileSystemObjectAsync
    {
        //public FileSystemObject(int siteId)
        //{
        //    SiteInfo = SiteManager.GetSiteInfo(siteId);
        //    if (SiteInfo == null)
        //    {
        //        throw new ArgumentException(siteId + " 不是正确的发布系统ID！");
        //    }
        //    SiteId = siteInfo.Id;
        //    SiteDir = SiteInfo.SiteDir;
        //    SitePath = PathUtils.Combine(WebConfigUtils.PhysicalApplicationPath, SiteInfo.SiteDir);
        //    IsRoot = SiteInfo.IsRoot;
        //    DirectoryUtils.CreateDirectoryIfNotExists(SitePath);
        //}

        public static async Task ExecuteAsync(int siteId, ECreateType createType, int channelId, int contentId, int templateId)
        {
            if (createType == ECreateType.Channel)
            {
                await CreateChannelAsync(siteId, channelId);
            }
            else if (createType == ECreateType.Content)
            {
                var siteInfo = SiteManager.GetSiteInfo(siteId);
                var nodeInfo = ChannelManager.GetChannelInfo(siteId, channelId);
                var tableName = ChannelManager.GetTableName(siteInfo, nodeInfo);
                await CreateContentAsync(siteInfo, tableName, channelId, contentId);
            }
            else if (createType == ECreateType.AllContent)
            {
                await CreateContentsAsync(siteId, channelId);
            }
            else if (createType == ECreateType.File)
            {
                await CreateFileAsync(siteId, templateId);
            }
        }

        private static async Task CreateContentsAsync(int siteId, int channelId)
        {
            var siteInfo = SiteManager.GetSiteInfo(siteId);
            var nodeInfo = ChannelManager.GetChannelInfo(siteId, channelId);
            var tableName = ChannelManager.GetTableName(siteInfo, nodeInfo);
            var orderByString = ETaxisTypeUtils.GetContentOrderByString(ETaxisType.OrderByTaxisDesc);
            var contentIdList = Content.GetContentIdListChecked(tableName, channelId, orderByString);

            foreach (var contentId in contentIdList)
            {
                try
                {
                    await CreateContentAsync(siteInfo, tableName, channelId, contentId);
                }
                catch (Exception ex)
                {
                    LogUtils.AddSystemErrorLog(ex, "CreateContent");
                }
            }
        }

        private static async Task CreateChannelAsync(int siteId, int channelId)
        {
            var siteInfo = SiteManager.GetSiteInfo(siteId);
            var nodeInfo = ChannelManager.GetChannelInfo(siteId, channelId);
            if (nodeInfo == null) return;

            if (!string.IsNullOrEmpty(nodeInfo.LinkUrl))
            {
                return;
            }
            if (!ELinkTypeUtils.IsCreatable(nodeInfo))
            {
                return;
            }

            var templateInfo = channelId == siteId
                ? TemplateManager.GetIndexPageTemplateInfo(siteId)
                : TemplateManager.GetChannelTemplateInfo(siteId, channelId);
            var filePath = PathUtility.GetChannelPageFilePath(siteInfo, channelId, 0);
            var pageInfo = new PageInfo(channelId, 0, siteInfo, templateInfo);
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
            if (!string.IsNullOrEmpty(stlPageContentElement))//内容存在
            {
                var innerBuilder = new StringBuilder(stlPageContentElement);
                StlParserManager.ParseInnerContent(innerBuilder, pageInfo, contextInfo);
                var contentAttributeHtml = innerBuilder.ToString();
                var pageCount = StringUtils.GetCount(ContentUtility.PagePlaceHolder, contentAttributeHtml) + 1;//一共需要的页数

                Parser.Parse(siteInfo, pageInfo, contextInfo, contentBuilder, filePath, false);

                for (var currentPageIndex = 0; currentPageIndex < pageCount; currentPageIndex++)
                {
                    var thePageInfo = new PageInfo(pageInfo.PageChannelId, pageInfo.PageContentId, pageInfo.SiteInfo, pageInfo.TemplateInfo);
                    var index = contentAttributeHtml.IndexOf(ContentUtility.PagePlaceHolder, StringComparison.Ordinal);
                    var length = index == -1 ? contentAttributeHtml.Length : index;
                    var pagedContentAttributeHtml = contentAttributeHtml.Substring(0, length);
                    var pagedBuilder = new StringBuilder(contentBuilder.ToString().Replace(stlPageContentElement, pagedContentAttributeHtml));
                    StlParserManager.ReplacePageElementsInChannelPage(pagedBuilder, thePageInfo, stlLabelList, thePageInfo.PageChannelId, currentPageIndex, pageCount, 0);

                    filePath = PathUtility.GetChannelPageFilePath(siteInfo, thePageInfo.PageChannelId, currentPageIndex);

                    await GenerateFileAsync(filePath, pageInfo.TemplateInfo.Charset, pagedBuilder);

                    if (index != -1)
                    {
                        contentAttributeHtml = contentAttributeHtml.Substring(length + ContentUtility.PagePlaceHolder.Length);
                    }
                }
            }
            //如果标签中存在<stl:pageContents>
            else if (StlParserUtility.IsStlElementExists(StlPageContents.ElementName, stlLabelList))
            {
                var stlElement = StlParserUtility.GetStlElement(StlPageContents.ElementName, stlLabelList);
                var stlElementTranslated = StlParserManager.StlEncrypt(stlElement);

                var pageContentsElementParser = new StlPageContents(stlElement, pageInfo, contextInfo, false);
                int totalNum;
                var pageCount = pageContentsElementParser.GetPageCount(out totalNum);

                Parser.Parse(siteInfo, pageInfo, contextInfo, contentBuilder, filePath, false);

                for (var currentPageIndex = 0; currentPageIndex < pageCount; currentPageIndex++)
                {
                    var thePageInfo = new PageInfo(pageInfo.PageChannelId, pageInfo.PageContentId,
                        pageInfo.SiteInfo, pageInfo.TemplateInfo);
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

                var pageChannelsElementParser = new StlPageChannels(stlElement, pageInfo, contextInfo, false);
                int totalNum;
                var pageCount = pageChannelsElementParser.GetPageCount(out totalNum);

                Parser.Parse(siteInfo, pageInfo, contextInfo, contentBuilder, filePath, false);

                for (var currentPageIndex = 0; currentPageIndex < pageCount; currentPageIndex++)
                {
                    var thePageInfo = new PageInfo(pageInfo.PageChannelId, pageInfo.PageContentId, pageInfo.SiteInfo, pageInfo.TemplateInfo);
                    var pageHtml = pageChannelsElementParser.Parse(currentPageIndex, pageCount);
                    var pagedBuilder = new StringBuilder(contentBuilder.ToString().Replace(stlElementTranslated, pageHtml));

                    StlParserManager.ReplacePageElementsInChannelPage(pagedBuilder, thePageInfo, stlLabelList, thePageInfo.PageChannelId, currentPageIndex, pageCount, totalNum);

                    filePath = PathUtility.GetChannelPageFilePath(siteInfo, thePageInfo.PageChannelId, currentPageIndex);
                    await GenerateFileAsync(filePath, pageInfo.TemplateInfo.Charset, pagedBuilder);
                }
            }
            //如果标签中存在<stl:pageSqlContents>
            else if (StlParserUtility.IsStlElementExists(StlPageSqlContents.ElementName, stlLabelList))
            {
                var stlElement = StlParserUtility.GetStlElement(StlPageSqlContents.ElementName, stlLabelList);
                var stlElementTranslated = StlParserManager.StlEncrypt(stlElement);

                var pageSqlContentsElementParser = new StlPageSqlContents(stlElement, pageInfo, contextInfo, false);
                int totalNum;
                var pageCount = pageSqlContentsElementParser.GetPageCount(out totalNum);

                Parser.Parse(siteInfo, pageInfo, contextInfo, contentBuilder, filePath, false);

                for (var currentPageIndex = 0; currentPageIndex < pageCount; currentPageIndex++)
                {
                    var thePageInfo = new PageInfo(pageInfo.PageChannelId, pageInfo.PageContentId, pageInfo.SiteInfo, pageInfo.TemplateInfo);
                    var pageHtml = pageSqlContentsElementParser.Parse(currentPageIndex, pageCount);
                    var pagedBuilder = new StringBuilder(contentBuilder.ToString().Replace(stlElementTranslated, pageHtml));

                    StlParserManager.ReplacePageElementsInChannelPage(pagedBuilder, thePageInfo, stlLabelList, thePageInfo.PageChannelId, currentPageIndex, pageCount, totalNum);

                    filePath = PathUtility.GetChannelPageFilePath(siteInfo, thePageInfo.PageChannelId, currentPageIndex);
                    await GenerateFileAsync(filePath, pageInfo.TemplateInfo.Charset, pagedBuilder);
                }
            }
            //如果标签中存在<stl:StlPageInputContents>
            //else if (StlParserUtility.IsStlElementExists(StlPageInputContents.ElementName, stlLabelList))
            //{
            //    var stlElement = StlParserUtility.GetStlElement(StlPageInputContents.ElementName, stlLabelList);
            //    var stlElementTranslated = StlParserManager.StlEncrypt(stlElement);

            //    var pageInputContentsElementParser = new StlPageInputContents(stlElement, pageInfo, contextInfo, true);
            //    int totalNum;
            //    var pageCount = pageInputContentsElementParser.GetPageCount(out totalNum);

            //    Parser.Parse(siteInfo, pageInfo, contextInfo, contentBuilder, filePath, false);

            //    for (var currentPageIndex = 0; currentPageIndex < pageCount; currentPageIndex++)
            //    {
            //        var thePageInfo = new PageInfo(pageInfo.PageChannelId, pageInfo.PageContentId, pageInfo.SiteInfo, pageInfo.TemplateInfo, null);
            //        var pageHtml = pageInputContentsElementParser.Parse(currentPageIndex, pageCount);
            //        var pagedBuilder = new StringBuilder(contentBuilder.ToString().Replace(stlElementTranslated, pageHtml));

            //        StlParserManager.ReplacePageElementsInChannelPage(pagedBuilder, thePageInfo, stlLabelList, thePageInfo.PageChannelId, currentPageIndex, pageCount, totalNum);

            //        filePath = PathUtility.GetChannelPageFilePath(siteInfo, thePageInfo.PageChannelId, currentPageIndex);
            //        GenerateFile(filePath, pageInfo.TemplateInfo.Charset, pagedBuilder);
            //    }
            //}
            else
            {
                Parser.Parse(siteInfo, pageInfo, contextInfo, contentBuilder, filePath, false);
                await GenerateFileAsync(filePath, pageInfo.TemplateInfo.Charset, contentBuilder);
            }
        }

        private static async Task CreateContentAsync(SiteInfo siteInfo, string tableName, int channelId, int contentId)
        {
            var contentInfo = Content.GetContentInfo(tableName, contentId);

            if (contentInfo == null)
            { 
                return;
            }
            //引用链接，不需要生成内容页；引用内容，需要生成内容页；
            if (contentInfo.ReferenceId > 0 && ETranslateContentTypeUtils.GetEnumType(contentInfo.GetString(ContentAttribute.TranslateContentType)) != ETranslateContentType.ReferenceContent)
            {
                return;
            }
            if (!string.IsNullOrEmpty(contentInfo.GetString(ContentAttribute.LinkUrl)))
            {
                return;
            }
            if (!contentInfo.IsChecked)
            {
                return;
            }
            if (siteInfo.Additional.IsCreateStaticContentByAddDate && contentInfo.AddDate < siteInfo.Additional.CreateStaticContentAddDate)
            {
                return;
            }

            var templateInfo = TemplateManager.GetContentTemplateInfo(siteInfo.Id, channelId);
            var pageInfo = new PageInfo(channelId, contentId, siteInfo, templateInfo);
            var contextInfo = new ContextInfo(pageInfo)
            {
                ContextType = EContextType.Content,
                ContentInfo = contentInfo
            };
            var filePath = PathUtility.GetContentPageFilePath(siteInfo, pageInfo.PageChannelId, contentInfo, 0);
            var contentBuilder = new StringBuilder(TemplateManager.GetTemplateContent(siteInfo, templateInfo));

            var stlLabelList = StlParserUtility.GetStlLabelList(contentBuilder.ToString());

            //如果标签中存在<stl:content type="PageContent"></stl:content>
            if (StlParserUtility.IsStlContentElementWithTypePageContent(stlLabelList))//内容存在
            {
                var stlElement = StlParserUtility.GetStlContentElementWithTypePageContent(stlLabelList);
                var stlElementTranslated = StlParserManager.StlEncrypt(stlElement);
                contentBuilder.Replace(stlElement, stlElementTranslated);

                var innerBuilder = new StringBuilder(stlElement);
                StlParserManager.ParseInnerContent(innerBuilder, pageInfo, contextInfo);
                var pageContentHtml = innerBuilder.ToString();
                var pageCount = StringUtils.GetCount(ContentUtility.PagePlaceHolder, pageContentHtml) + 1;//一共需要的页数

                Parser.Parse(siteInfo, pageInfo, contextInfo, contentBuilder, filePath, false);

                for (var currentPageIndex = 0; currentPageIndex < pageCount; currentPageIndex++)
                {
                    var thePageInfo = new PageInfo(pageInfo.PageChannelId, pageInfo.PageContentId, pageInfo.SiteInfo, pageInfo.TemplateInfo);

                    var index = pageContentHtml.IndexOf(ContentUtility.PagePlaceHolder, StringComparison.Ordinal);
                    var length = index == -1 ? pageContentHtml.Length : index;

                    var pageHtml = pageContentHtml.Substring(0, length);
                    var pagedBuilder = new StringBuilder(contentBuilder.ToString().Replace(stlElementTranslated, pageHtml));
                    StlParserManager.ReplacePageElementsInContentPage(pagedBuilder, thePageInfo, stlLabelList, channelId, contentId, currentPageIndex, pageCount);

                    filePath = PathUtility.GetContentPageFilePath(siteInfo, thePageInfo.PageChannelId, contentInfo, currentPageIndex);
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

                var pageContentsElementParser = new StlPageContents(stlElement, pageInfo, contextInfo, false);
                int totalNum;
                var pageCount = pageContentsElementParser.GetPageCount(out totalNum);

                Parser.Parse(siteInfo, pageInfo, contextInfo, contentBuilder, filePath, false);

                for (var currentPageIndex = 0; currentPageIndex < pageCount; currentPageIndex++)
                {
                    var thePageInfo = new PageInfo(pageInfo.PageChannelId, pageInfo.PageContentId, pageInfo.SiteInfo, pageInfo.TemplateInfo);
                    var pageHtml = pageContentsElementParser.Parse(totalNum, currentPageIndex, pageCount, true);
                    var pagedBuilder = new StringBuilder(contentBuilder.ToString().Replace(stlElementTranslated, pageHtml));

                    StlParserManager.ReplacePageElementsInContentPage(pagedBuilder, thePageInfo, stlLabelList, channelId, contentId, currentPageIndex, pageCount);

                    filePath = PathUtility.GetContentPageFilePath(siteInfo, thePageInfo.PageChannelId, contentInfo, currentPageIndex);
                    await GenerateFileAsync(filePath, pageInfo.TemplateInfo.Charset, pagedBuilder);
                }
            }
            //如果标签中存在<stl:pageChannels>
            else if (StlParserUtility.IsStlElementExists(StlPageChannels.ElementName, stlLabelList))
            {
                var stlElement = StlParserUtility.GetStlElement(StlPageChannels.ElementName, stlLabelList);
                var stlElementTranslated = StlParserManager.StlEncrypt(stlElement);

                var pageChannelsElementParser = new StlPageChannels(stlElement, pageInfo, contextInfo, false);
                int totalNum;
                var pageCount = pageChannelsElementParser.GetPageCount(out totalNum);

                Parser.Parse(siteInfo, pageInfo, contextInfo, contentBuilder, filePath, false);

                for (var currentPageIndex = 0; currentPageIndex < pageCount; currentPageIndex++)
                {
                    var thePageInfo = new PageInfo(pageInfo.PageChannelId, pageInfo.PageContentId, pageInfo.SiteInfo, pageInfo.TemplateInfo);
                    var pageHtml = pageChannelsElementParser.Parse(currentPageIndex, pageCount);
                    var pagedBuilder = new StringBuilder(contentBuilder.ToString().Replace(stlElementTranslated, pageHtml));

                    StlParserManager.ReplacePageElementsInContentPage(pagedBuilder, thePageInfo, stlLabelList, channelId, contentId, currentPageIndex, pageCount);

                    filePath = PathUtility.GetContentPageFilePath(siteInfo, thePageInfo.PageChannelId, contentInfo, currentPageIndex);
                    await GenerateFileAsync(filePath, pageInfo.TemplateInfo.Charset, pagedBuilder);
                }
            }
            //如果标签中存在<stl:pageSqlContents>
            else if (StlParserUtility.IsStlElementExists(StlPageSqlContents.ElementName, stlLabelList))
            {
                var stlElement = StlParserUtility.GetStlElement(StlPageSqlContents.ElementName, stlLabelList);
                var stlElementTranslated = StlParserManager.StlEncrypt(stlElement);

                var pageSqlContentsElementParser = new StlPageSqlContents(stlElement, pageInfo, contextInfo, false);
                int totalNum;
                var pageCount = pageSqlContentsElementParser.GetPageCount(out totalNum);

                Parser.Parse(siteInfo, pageInfo, contextInfo, contentBuilder, filePath, false);

                for (var currentPageIndex = 0; currentPageIndex < pageCount; currentPageIndex++)
                {
                    var thePageInfo = new PageInfo(pageInfo.PageChannelId, pageInfo.PageContentId, pageInfo.SiteInfo, pageInfo.TemplateInfo);
                    var pageHtml = pageSqlContentsElementParser.Parse(currentPageIndex, pageCount);
                    var pagedBuilder = new StringBuilder(contentBuilder.ToString().Replace(stlElementTranslated, pageHtml));

                    StlParserManager.ReplacePageElementsInContentPage(pagedBuilder, thePageInfo, stlLabelList, channelId, contentId, currentPageIndex, pageCount);

                    filePath = PathUtility.GetContentPageFilePath(siteInfo, thePageInfo.PageChannelId, contentInfo, currentPageIndex);
                    await GenerateFileAsync(filePath, pageInfo.TemplateInfo.Charset, pagedBuilder);
                }
            }
            else//无翻页
            {
                Parser.Parse(siteInfo, pageInfo, contextInfo, contentBuilder, filePath, false);
                await GenerateFileAsync(filePath, pageInfo.TemplateInfo.Charset, contentBuilder);
            }
        }

        private static async Task CreateFileAsync(int siteId, int templateId)
        {
            var siteInfo = SiteManager.GetSiteInfo(siteId);
            var templateInfo = TemplateManager.GetTemplateInfo(siteId, templateId);
            if (templateInfo == null || templateInfo.TemplateType != TemplateType.FileTemplate)
            {
                return;
            }
            var pageInfo = new PageInfo(siteId, 0, siteInfo, templateInfo);
            var contextInfo = new ContextInfo(pageInfo);
            var filePath = PathUtility.MapPath(siteInfo, templateInfo.CreatedFileFullName);

            //if (siteInfo.Additional.VisualType == EVisualType.Dynamic)
            //{
            //    string pageUrl = PageUtility.GetFileUrl(siteInfo, templateID, EVisualType.Dynamic);
            //    string content = StringUtility.GetRedirectPageHtml(pageUrl);
            //    this.GenerateFile(filePath, pageInfo.TemplateInfo.Charset, content);
            //    return;
            //}

            var contentBuilder = new StringBuilder(TemplateManager.GetTemplateContent(siteInfo, templateInfo));
            Parser.Parse(siteInfo, pageInfo, contextInfo, contentBuilder, filePath, false);
            await GenerateFileAsync(filePath, pageInfo.TemplateInfo.Charset, contentBuilder);
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