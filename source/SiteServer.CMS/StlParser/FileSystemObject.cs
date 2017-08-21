using System;
using System.Collections.Generic;
using System.Text;
using BaiRong.Core;
using BaiRong.Core.Model.Attributes;
using BaiRong.Core.Model.Enumerations;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;
using SiteServer.CMS.Model.Enumerations;
using SiteServer.CMS.StlParser.Cache;
using SiteServer.CMS.StlParser.Model;
using SiteServer.CMS.StlParser.StlElement;
using SiteServer.CMS.StlParser.Utility;
using System.Threading.Tasks;
using BaiRong.Core.Model;

namespace SiteServer.CMS.StlParser
{
    public class FileSystemObject
    {
        //public FileSystemObject(int publishmentSystemId)
        //{
        //    PublishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(publishmentSystemId);
        //    if (PublishmentSystemInfo == null)
        //    {
        //        throw new ArgumentException(publishmentSystemId + " 不是正确的发布系统ID！");
        //    }
        //    PublishmentSystemId = PublishmentSystemInfo.PublishmentSystemId;
        //    PublishmentSystemDir = PublishmentSystemInfo.PublishmentSystemDir;
        //    PublishmentSystemPath = PathUtils.Combine(WebConfigUtils.PhysicalApplicationPath, PublishmentSystemInfo.PublishmentSystemDir);
        //    IsHeadquarters = PublishmentSystemInfo.IsHeadquarters;
        //    DirectoryUtils.CreateDirectoryIfNotExists(PublishmentSystemPath);
        //}

        public static void Execute(int publishmentSystemId, ECreateType createType, int channelId, int contentId, int templateId)
        {
            if (createType == ECreateType.Channel)
            {
                CreateChannel(publishmentSystemId, channelId);
            }
            else if (createType == ECreateType.Content)
            {
                var publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(publishmentSystemId);
                var nodeInfo = NodeManager.GetNodeInfo(publishmentSystemId, channelId);
                var tableStyle = NodeManager.GetTableStyle(publishmentSystemInfo, nodeInfo);
                var tableName = NodeManager.GetTableName(publishmentSystemInfo, nodeInfo);
                CreateContent(publishmentSystemInfo, tableStyle, tableName, channelId, contentId);
            }
            else if (createType == ECreateType.AllContent)
            {
                CreateContents(publishmentSystemId, channelId);
            }
            else if (createType == ECreateType.File)
            {
                CreateFile(publishmentSystemId, templateId);
            }
        }

        private static void CreateContents(int publishmentSystemId, int nodeId)
        {
            var publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(publishmentSystemId);
            var nodeInfo = NodeManager.GetNodeInfo(publishmentSystemId, nodeId);
            var tableStyle = NodeManager.GetTableStyle(publishmentSystemInfo, nodeInfo);
            var tableName = NodeManager.GetTableName(publishmentSystemInfo, nodeInfo);
            var orderByString = ETaxisTypeUtils.GetContentOrderByString(ETaxisType.OrderByTaxisDesc);
            var contentIdList = Content.GetContentIdListChecked(tableName, nodeId, orderByString);

            if (publishmentSystemInfo.Additional.IsCreateMultiThread) // 多线程并发生成页面
            {
                for (var i = 0; i < contentIdList.Count; i = i + 3)
                {
                    var list = new List<int>
                    {
                        contentIdList[i]
                    };
                    if (i < contentIdList.Count - 1)
                    {
                        list.Add(contentIdList[i + 1]);
                    }
                    if (i < contentIdList.Count - 2)
                    {
                        list.Add(contentIdList[i + 2]);
                    }
                    Parallel.ForEach(list, contentId =>
                    {
                        try
                        {
                            CreateContent(publishmentSystemInfo, tableStyle, tableName, nodeId, contentId);
                        }
                        catch (Exception ex)
                        {
                            LogUtils.AddErrorLog(ex, "CreateContent");
                        }
                    });
                }
            }
            else  // 单线程生成页面
            {
                foreach (var contentId in contentIdList)
                {
                    try
                    {
                        CreateContent(publishmentSystemInfo, tableStyle, tableName, nodeId, contentId);
                    }
                    catch (Exception ex)
                    {
                        LogUtils.AddErrorLog(ex, "CreateContent");
                    }
                }
            }
        }

        private static void CreateChannel(int publishmentSystemId, int nodeId)
        {
            var publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(publishmentSystemId);
            var nodeInfo = NodeManager.GetNodeInfo(publishmentSystemId, nodeId);
            if (nodeInfo == null) return;

            if (!string.IsNullOrEmpty(nodeInfo.LinkUrl))
            {
                return;
            }
            if (!ELinkTypeUtils.IsCreatable(nodeInfo))
            {
                return;
            }

            var templateInfo = nodeId == publishmentSystemId ? TemplateManager.GetTemplateInfo(publishmentSystemId, 0, ETemplateType.IndexPageTemplate) : TemplateManager.GetTemplateInfo(publishmentSystemId, nodeId, ETemplateType.ChannelTemplate);
            var filePath = PathUtility.GetChannelPageFilePath(publishmentSystemInfo, nodeId, 0);
            var pageInfo = new PageInfo(nodeId, 0, publishmentSystemInfo, templateInfo, null);
            var contextInfo = new ContextInfo(pageInfo);
            var contentBuilder = new StringBuilder(TemplateManager.GetTemplateContent(publishmentSystemInfo, templateInfo));

            var stlLabelList = StlParserUtility.GetStlLabelList(contentBuilder.ToString());
            var stlPageContentElement = string.Empty;
            foreach (var label in stlLabelList)
            {
                if (!StlParserUtility.IsStlChannelElement(label, NodeAttribute.PageContent)) continue;
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

                Parser.Parse(publishmentSystemInfo, pageInfo, contextInfo, contentBuilder, filePath, false);

                for (var currentPageIndex = 0; currentPageIndex < pageCount; currentPageIndex++)
                {
                    var thePageInfo = new PageInfo(pageInfo.PageNodeId, pageInfo.PageContentId, pageInfo.PublishmentSystemInfo, pageInfo.TemplateInfo, null);
                    var index = contentAttributeHtml.IndexOf(ContentUtility.PagePlaceHolder, StringComparison.Ordinal);
                    var length = index == -1 ? contentAttributeHtml.Length : index;
                    var pagedContentAttributeHtml = contentAttributeHtml.Substring(0, length);
                    var pagedBuilder = new StringBuilder(contentBuilder.ToString().Replace(stlPageContentElement, pagedContentAttributeHtml));
                    StlParserManager.ReplacePageElementsInChannelPage(pagedBuilder, thePageInfo, stlLabelList, thePageInfo.PageNodeId, currentPageIndex, pageCount, 0);

                    filePath = PathUtility.GetChannelPageFilePath(publishmentSystemInfo, thePageInfo.PageNodeId, currentPageIndex);

                    GenerateFile(filePath, pageInfo.TemplateInfo.Charset, pagedBuilder);

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
                var stlElementTranslated = TranslateUtils.EncryptStringBySecretKey(stlElement);

                var pageContentsElementParser = new StlPageContents(stlElement, pageInfo, contextInfo, false);
                int totalNum;
                var pageCount = pageContentsElementParser.GetPageCount(out totalNum);

                Parser.Parse(publishmentSystemInfo, pageInfo, contextInfo, contentBuilder, filePath, false);

                for (var currentPageIndex = 0; currentPageIndex < pageCount; currentPageIndex++)
                {
                    var thePageInfo = new PageInfo(pageInfo.PageNodeId, pageInfo.PageContentId,
                        pageInfo.PublishmentSystemInfo, pageInfo.TemplateInfo, null);
                    var pageHtml = pageContentsElementParser.Parse(totalNum, currentPageIndex, pageCount, true);
                    var pagedBuilder =
                        new StringBuilder(contentBuilder.ToString().Replace(stlElementTranslated, pageHtml));

                    StlParserManager.ReplacePageElementsInChannelPage(pagedBuilder, thePageInfo, stlLabelList,
                        thePageInfo.PageNodeId, currentPageIndex, pageCount, totalNum);

                    filePath = PathUtility.GetChannelPageFilePath(publishmentSystemInfo, thePageInfo.PageNodeId,
                        currentPageIndex);
                    thePageInfo.AddLastPageScript(pageInfo);

                    GenerateFile(filePath, pageInfo.TemplateInfo.Charset, pagedBuilder);

                    thePageInfo.ClearLastPageScript(pageInfo);
                    pageInfo.ClearLastPageScript();
                }
            }
            //如果标签中存在<stl:pageChannels>
            else if (StlParserUtility.IsStlElementExists(StlPageChannels.ElementName, stlLabelList))
            {
                var stlElement = StlParserUtility.GetStlElement(StlPageChannels.ElementName, stlLabelList);
                var stlElementTranslated = TranslateUtils.EncryptStringBySecretKey(stlElement);

                var pageChannelsElementParser = new StlPageChannels(stlElement, pageInfo, contextInfo, false);
                int totalNum;
                var pageCount = pageChannelsElementParser.GetPageCount(out totalNum);

                Parser.Parse(publishmentSystemInfo, pageInfo, contextInfo, contentBuilder, filePath, false);

                for (var currentPageIndex = 0; currentPageIndex < pageCount; currentPageIndex++)
                {
                    var thePageInfo = new PageInfo(pageInfo.PageNodeId, pageInfo.PageContentId, pageInfo.PublishmentSystemInfo, pageInfo.TemplateInfo, null);
                    var pageHtml = pageChannelsElementParser.Parse(currentPageIndex, pageCount);
                    var pagedBuilder = new StringBuilder(contentBuilder.ToString().Replace(stlElementTranslated, pageHtml));

                    StlParserManager.ReplacePageElementsInChannelPage(pagedBuilder, thePageInfo, stlLabelList, thePageInfo.PageNodeId, currentPageIndex, pageCount, totalNum);

                    filePath = PathUtility.GetChannelPageFilePath(publishmentSystemInfo, thePageInfo.PageNodeId, currentPageIndex);
                    GenerateFile(filePath, pageInfo.TemplateInfo.Charset, pagedBuilder);
                }
            }
            //如果标签中存在<stl:pageSqlContents>
            else if (StlParserUtility.IsStlElementExists(StlPageSqlContents.ElementName, stlLabelList))
            {
                var stlElement = StlParserUtility.GetStlElement(StlPageSqlContents.ElementName, stlLabelList);
                var stlElementTranslated = TranslateUtils.EncryptStringBySecretKey(stlElement);

                var pageSqlContentsElementParser = new StlPageSqlContents(stlElement, pageInfo, contextInfo, false);
                int totalNum;
                var pageCount = pageSqlContentsElementParser.GetPageCount(out totalNum);

                Parser.Parse(publishmentSystemInfo, pageInfo, contextInfo, contentBuilder, filePath, false);

                for (var currentPageIndex = 0; currentPageIndex < pageCount; currentPageIndex++)
                {
                    var thePageInfo = new PageInfo(pageInfo.PageNodeId, pageInfo.PageContentId, pageInfo.PublishmentSystemInfo, pageInfo.TemplateInfo, null);
                    var pageHtml = pageSqlContentsElementParser.Parse(currentPageIndex, pageCount);
                    var pagedBuilder = new StringBuilder(contentBuilder.ToString().Replace(stlElementTranslated, pageHtml));

                    StlParserManager.ReplacePageElementsInChannelPage(pagedBuilder, thePageInfo, stlLabelList, thePageInfo.PageNodeId, currentPageIndex, pageCount, totalNum);

                    filePath = PathUtility.GetChannelPageFilePath(publishmentSystemInfo, thePageInfo.PageNodeId, currentPageIndex);
                    GenerateFile(filePath, pageInfo.TemplateInfo.Charset, pagedBuilder);
                }
            }
            //如果标签中存在<stl:StlPageInputContents>
            else if (StlParserUtility.IsStlElementExists(StlPageInputContents.ElementName, stlLabelList))
            {
                var stlElement = StlParserUtility.GetStlElement(StlPageInputContents.ElementName, stlLabelList);
                var stlElementTranslated = TranslateUtils.EncryptStringBySecretKey(stlElement);

                var pageInputContentsElementParser = new StlPageInputContents(stlElement, pageInfo, contextInfo, true);
                int totalNum;
                var pageCount = pageInputContentsElementParser.GetPageCount(out totalNum);

                Parser.Parse(publishmentSystemInfo, pageInfo, contextInfo, contentBuilder, filePath, false);

                for (var currentPageIndex = 0; currentPageIndex < pageCount; currentPageIndex++)
                {
                    var thePageInfo = new PageInfo(pageInfo.PageNodeId, pageInfo.PageContentId, pageInfo.PublishmentSystemInfo, pageInfo.TemplateInfo, null);
                    var pageHtml = pageInputContentsElementParser.Parse(currentPageIndex, pageCount);
                    var pagedBuilder = new StringBuilder(contentBuilder.ToString().Replace(stlElementTranslated, pageHtml));

                    StlParserManager.ReplacePageElementsInChannelPage(pagedBuilder, thePageInfo, stlLabelList, thePageInfo.PageNodeId, currentPageIndex, pageCount, totalNum);

                    filePath = PathUtility.GetChannelPageFilePath(publishmentSystemInfo, thePageInfo.PageNodeId, currentPageIndex);
                    GenerateFile(filePath, pageInfo.TemplateInfo.Charset, pagedBuilder);
                }
            }
            else
            {
                Parser.Parse(publishmentSystemInfo, pageInfo, contextInfo, contentBuilder, filePath, false);
                GenerateFile(filePath, pageInfo.TemplateInfo.Charset, contentBuilder);
            }
        }

        private static void CreateContent(PublishmentSystemInfo publishmentSystemInfo, ETableStyle tableStyle, string tableName, int nodeId, int contentId)
        {
            var contentInfo = Content.GetContentInfo(tableStyle, tableName, contentId);

            if (contentInfo == null)
            { 
                return;
            }
            //引用链接，不需要生成内容页；引用内容，需要生成内容页；
            if (contentInfo.ReferenceId > 0 && ETranslateContentTypeUtils.GetEnumType(contentInfo.GetExtendedAttribute(ContentAttribute.TranslateContentType)) != ETranslateContentType.ReferenceContent)
            {
                return;
            }
            if (tableStyle == ETableStyle.BackgroundContent && !string.IsNullOrEmpty(contentInfo.GetExtendedAttribute(BackgroundContentAttribute.LinkUrl)))
            {
                return;
            }
            if (!contentInfo.IsChecked)
            {
                return;
            }
            if (publishmentSystemInfo.Additional.IsCreateStaticContentByAddDate && contentInfo.AddDate < publishmentSystemInfo.Additional.CreateStaticContentAddDate)
            {
                return;
            }

            var templateInfo = TemplateManager.GetTemplateInfo(publishmentSystemInfo.PublishmentSystemId, nodeId, ETemplateType.ContentTemplate);
            var pageInfo = new PageInfo(nodeId, contentId, publishmentSystemInfo, templateInfo, null);
            var contextInfo = new ContextInfo(pageInfo)
            {
                ContentInfo = contentInfo
            };
            var filePath = PathUtility.GetContentPageFilePath(publishmentSystemInfo, pageInfo.PageNodeId, contentInfo, 0);
            var contentBuilder = new StringBuilder(TemplateManager.GetTemplateContent(publishmentSystemInfo, templateInfo));

            var stlLabelList = StlParserUtility.GetStlLabelList(contentBuilder.ToString());

            //如果标签中存在<stl:content type="PageContent"></stl:content>
            if (StlParserUtility.IsStlContentElementWithTypePageContent(stlLabelList))//内容存在
            {
                var stlElement = StlParserUtility.GetStlContentElementWithTypePageContent(stlLabelList);
                var stlElementTranslated = TranslateUtils.EncryptStringBySecretKey(stlElement);
                contentBuilder.Replace(stlElement, stlElementTranslated);

                var innerBuilder = new StringBuilder(stlElement);
                StlParserManager.ParseInnerContent(innerBuilder, pageInfo, contextInfo);
                var pageContentHtml = innerBuilder.ToString();
                var pageCount = StringUtils.GetCount(ContentUtility.PagePlaceHolder, pageContentHtml) + 1;//一共需要的页数

                Parser.Parse(publishmentSystemInfo, pageInfo, contextInfo, contentBuilder, filePath, false);

                for (var currentPageIndex = 0; currentPageIndex < pageCount; currentPageIndex++)
                {
                    var thePageInfo = new PageInfo(pageInfo.PageNodeId, pageInfo.PageContentId, pageInfo.PublishmentSystemInfo, pageInfo.TemplateInfo, null);

                    var index = pageContentHtml.IndexOf(ContentUtility.PagePlaceHolder, StringComparison.Ordinal);
                    var length = index == -1 ? pageContentHtml.Length : index;

                    var pageHtml = pageContentHtml.Substring(0, length);
                    var pagedBuilder = new StringBuilder(contentBuilder.ToString().Replace(stlElementTranslated, pageHtml));
                    StlParserManager.ReplacePageElementsInContentPage(pagedBuilder, thePageInfo, stlLabelList, nodeId, contentId, currentPageIndex, pageCount);

                    filePath = PathUtility.GetContentPageFilePath(publishmentSystemInfo, thePageInfo.PageNodeId, contentInfo, currentPageIndex);
                    GenerateFile(filePath, pageInfo.TemplateInfo.Charset, pagedBuilder);

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
                var stlElementTranslated = TranslateUtils.EncryptStringBySecretKey(stlElement);

                var pageContentsElementParser = new StlPageContents(stlElement, pageInfo, contextInfo, false);
                int totalNum;
                var pageCount = pageContentsElementParser.GetPageCount(out totalNum);

                Parser.Parse(publishmentSystemInfo, pageInfo, contextInfo, contentBuilder, filePath, false);

                for (var currentPageIndex = 0; currentPageIndex < pageCount; currentPageIndex++)
                {
                    var thePageInfo = new PageInfo(pageInfo.PageNodeId, pageInfo.PageContentId, pageInfo.PublishmentSystemInfo, pageInfo.TemplateInfo, null);
                    var pageHtml = pageContentsElementParser.Parse(totalNum, currentPageIndex, pageCount, true);
                    var pagedBuilder = new StringBuilder(contentBuilder.ToString().Replace(stlElementTranslated, pageHtml));

                    StlParserManager.ReplacePageElementsInContentPage(pagedBuilder, thePageInfo, stlLabelList, nodeId, contentId, currentPageIndex, pageCount);

                    filePath = PathUtility.GetContentPageFilePath(publishmentSystemInfo, thePageInfo.PageNodeId, contentInfo, currentPageIndex);
                    GenerateFile(filePath, pageInfo.TemplateInfo.Charset, pagedBuilder);
                }
            }
            //如果标签中存在<stl:pageChannels>
            else if (StlParserUtility.IsStlElementExists(StlPageChannels.ElementName, stlLabelList))
            {
                var stlElement = StlParserUtility.GetStlElement(StlPageChannels.ElementName, stlLabelList);
                var stlElementTranslated = TranslateUtils.EncryptStringBySecretKey(stlElement);

                var pageChannelsElementParser = new StlPageChannels(stlElement, pageInfo, contextInfo, false);
                int totalNum;
                var pageCount = pageChannelsElementParser.GetPageCount(out totalNum);

                Parser.Parse(publishmentSystemInfo, pageInfo, contextInfo, contentBuilder, filePath, false);

                for (var currentPageIndex = 0; currentPageIndex < pageCount; currentPageIndex++)
                {
                    var thePageInfo = new PageInfo(pageInfo.PageNodeId, pageInfo.PageContentId, pageInfo.PublishmentSystemInfo, pageInfo.TemplateInfo, null);
                    var pageHtml = pageChannelsElementParser.Parse(currentPageIndex, pageCount);
                    var pagedBuilder = new StringBuilder(contentBuilder.ToString().Replace(stlElementTranslated, pageHtml));

                    StlParserManager.ReplacePageElementsInContentPage(pagedBuilder, thePageInfo, stlLabelList, nodeId, contentId, currentPageIndex, pageCount);

                    filePath = PathUtility.GetContentPageFilePath(publishmentSystemInfo, thePageInfo.PageNodeId, contentInfo, currentPageIndex);
                    GenerateFile(filePath, pageInfo.TemplateInfo.Charset, pagedBuilder);
                }
            }
            //如果标签中存在<stl:pageSqlContents>
            else if (StlParserUtility.IsStlElementExists(StlPageSqlContents.ElementName, stlLabelList))
            {
                var stlElement = StlParserUtility.GetStlElement(StlPageSqlContents.ElementName, stlLabelList);
                var stlElementTranslated = TranslateUtils.EncryptStringBySecretKey(stlElement);

                var pageSqlContentsElementParser = new StlPageSqlContents(stlElement, pageInfo, contextInfo, false);
                int totalNum;
                var pageCount = pageSqlContentsElementParser.GetPageCount(out totalNum);

                Parser.Parse(publishmentSystemInfo, pageInfo, contextInfo, contentBuilder, filePath, false);

                for (var currentPageIndex = 0; currentPageIndex < pageCount; currentPageIndex++)
                {
                    var thePageInfo = new PageInfo(pageInfo.PageNodeId, pageInfo.PageContentId, pageInfo.PublishmentSystemInfo, pageInfo.TemplateInfo, null);
                    var pageHtml = pageSqlContentsElementParser.Parse(currentPageIndex, pageCount);
                    var pagedBuilder = new StringBuilder(contentBuilder.ToString().Replace(stlElementTranslated, pageHtml));

                    StlParserManager.ReplacePageElementsInContentPage(pagedBuilder, thePageInfo, stlLabelList, nodeId, contentId, currentPageIndex, pageCount);

                    filePath = PathUtility.GetContentPageFilePath(publishmentSystemInfo, thePageInfo.PageNodeId, contentInfo, currentPageIndex);
                    GenerateFile(filePath, pageInfo.TemplateInfo.Charset, pagedBuilder);
                }
            }
            else//无翻页
            {
                Parser.Parse(publishmentSystemInfo, pageInfo, contextInfo, contentBuilder, filePath, false);
                GenerateFile(filePath, pageInfo.TemplateInfo.Charset, contentBuilder);
            }
        }

        private static void CreateFile(int publishmentSystemId, int templateId)
        {
            var publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(publishmentSystemId);
            var templateInfo = TemplateManager.GetTemplateInfo(publishmentSystemId, templateId);
            if (templateInfo == null || templateInfo.TemplateType != ETemplateType.FileTemplate)
            {
                return;
            }
            var pageInfo = new PageInfo(publishmentSystemId, 0, publishmentSystemInfo, templateInfo, null);
            var contextInfo = new ContextInfo(pageInfo);
            var filePath = PathUtility.MapPath(publishmentSystemInfo, templateInfo.CreatedFileFullName);

            //if (publishmentSystemInfo.Additional.VisualType == EVisualType.Dynamic)
            //{
            //    string pageUrl = PageUtility.GetFileUrl(publishmentSystemInfo, templateID, EVisualType.Dynamic);
            //    string content = StringUtility.GetRedirectPageHtml(pageUrl);
            //    this.GenerateFile(filePath, pageInfo.TemplateInfo.Charset, content);
            //    return;
            //}

            var contentBuilder = new StringBuilder(TemplateManager.GetTemplateContent(publishmentSystemInfo, templateInfo));
            Parser.Parse(publishmentSystemInfo, pageInfo, contextInfo, contentBuilder, filePath, false);
            GenerateFile(filePath, pageInfo.TemplateInfo.Charset, contentBuilder);
        }

        //private string CreateIncludeFile(string virtualUrl, bool isCreateIfExists)
        //{
        //    var templateInfo = new TemplateInfo(0, PublishmentSystemId, string.Empty, ETemplateType.FileTemplate, string.Empty, string.Empty, string.Empty, ECharsetUtils.GetEnumType(PublishmentSystemInfo.Additional.Charset), false);
        //    var pageInfo = new PageInfo(PublishmentSystemId, 0, PublishmentSystemInfo, templateInfo, null);
        //    var contextInfo = new ContextInfo(pageInfo);

        //    var parsedVirtualUrl = virtualUrl.Substring(0, virtualUrl.LastIndexOf('.')) + "_parsed" + virtualUrl.Substring(virtualUrl.LastIndexOf('.'));
        //    var filePath = PathUtility.MapPath(PublishmentSystemInfo, parsedVirtualUrl);
        //    if (!isCreateIfExists && FileUtils.IsFileExists(filePath)) return parsedVirtualUrl;

        //    var contentBuilder = new StringBuilder(StlCacheManager.FileContent.GetIncludeContent(PublishmentSystemInfo, virtualUrl, pageInfo.TemplateInfo.Charset));
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
        private static void GenerateFile(string filePath, ECharset charset, StringBuilder contentBuilder)
        {
            if (string.IsNullOrEmpty(filePath)) return;

            try
            {
                FileUtils.WriteText(filePath, charset, contentBuilder.ToString());
            }
            catch
            {
                FileUtils.RemoveReadOnlyAndHiddenIfExists(filePath);
                FileUtils.WriteText(filePath, charset, contentBuilder.ToString());
            }
        }
    }
}