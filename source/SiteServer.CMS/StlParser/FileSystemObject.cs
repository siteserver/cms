using System;
using System.Text;
using BaiRong.Core;
using BaiRong.Core.Model.Attributes;
using BaiRong.Core.Model.Enumerations;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;
using SiteServer.CMS.Model.Enumerations;
using SiteServer.CMS.StlParser.Model;
using SiteServer.CMS.StlParser.StlElement;
using SiteServer.CMS.StlParser.Utility;

namespace SiteServer.CMS.StlParser
{
    public class FileSystemObject
    {
        public FileSystemObject(int publishmentSystemId)
        {
            PublishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(publishmentSystemId);
            if (PublishmentSystemInfo == null)
            {
                throw new ArgumentException(publishmentSystemId + " 不是正确的发布系统ID！");
            }
            PublishmentSystemId = PublishmentSystemInfo.PublishmentSystemId;
            PublishmentSystemDir = PublishmentSystemInfo.PublishmentSystemDir;
            PublishmentSystemPath = PathUtils.Combine(WebConfigUtils.PhysicalApplicationPath, PublishmentSystemInfo.PublishmentSystemDir);
            IsHeadquarters = PublishmentSystemInfo.IsHeadquarters;
            DirectoryUtils.CreateDirectoryIfNotExists(PublishmentSystemPath);
        }

        public PublishmentSystemInfo PublishmentSystemInfo { get; }

        public int PublishmentSystemId { get; }

        public string PublishmentSystemDir { get; }

        public string PublishmentSystemPath { get; }

        public bool IsHeadquarters { get; }

        public void Execute(CreateTaskInfo taskInfo)
        {
            if (taskInfo == null) return;

            if (taskInfo.CreateType == ECreateType.Index)
            {
                CreateChannel(taskInfo.PublishmentSystemID);
            }
            else if (taskInfo.CreateType == ECreateType.Channel)
            {
                CreateChannel(taskInfo.ChannelID);
            }
            else if (taskInfo.CreateType == ECreateType.Content)
            {
                var nodeInfo = NodeManager.GetNodeInfo(PublishmentSystemId, taskInfo.ChannelID);
                var tableStyle = NodeManager.GetTableStyle(PublishmentSystemInfo, nodeInfo);
                var tableName = NodeManager.GetTableName(PublishmentSystemInfo, nodeInfo);
                CreateContent(tableStyle, tableName, taskInfo.ChannelID, taskInfo.ContentID);
            }
            else if (taskInfo.CreateType == ECreateType.AllContent)
            {
                CreateContents(taskInfo.ChannelID);
            }
            else if (taskInfo.CreateType == ECreateType.File)
            {
                CreateFile(taskInfo.TemplateID);
            }
        }

        public void CreateAll()
        {
            var nodeIdList = DataProvider.NodeDao.GetNodeIdListByPublishmentSystemId(PublishmentSystemId);
            foreach (var nodeId in nodeIdList)
            {
                CreateChannel(nodeId);
                CreateContents(nodeId);
            }
            var templateIdArrayList = DataProvider.TemplateDao.GetTemplateIdArrayListByType(PublishmentSystemId, ETemplateType.FileTemplate);
            foreach (int templateId in templateIdArrayList)
            {
                CreateFile(templateId);
            }
        }

        //public void CreateIndex()
        //{
        //    var templateInfo = TemplateManager.GetTemplateInfo(PublishmentSystemId, 0, ETemplateType.IndexPageTemplate);
        //    if (templateInfo == null) return;

        //    var pageInfo = new PageInfo(PublishmentSystemId, 0, PublishmentSystemInfo, templateInfo, null);
        //    var contextInfo = new ContextInfo(pageInfo);
        //    var filePath = PathUtility.GetIndexPageFilePath(PublishmentSystemInfo, templateInfo.CreatedFileFullName, IsHeadquarters);

        //    var contentBuilder = new StringBuilder(StlCacheManager.FileContent.GetTemplateContent(PublishmentSystemInfo, templateInfo));
        //    StlUtility.ParseStl(PublishmentSystemInfo, pageInfo, contextInfo, contentBuilder, filePath, false);
        //    GenerateFile(filePath, pageInfo.TemplateInfo.Charset, contentBuilder);
        //}

        public void CreateChannel(int nodeId)
        {
            var nodeInfo = NodeManager.GetNodeInfo(PublishmentSystemId, nodeId);
            if (nodeInfo == null) return;

            //if (nodeId == PublishmentSystemId)
            //{
            //    CreateIndex();
            //    return;
            //}
            if (!string.IsNullOrEmpty(nodeInfo.LinkUrl))
            {
                return;
            }
            if (!ELinkTypeUtils.IsCreatable(nodeInfo))
            {
                return;
            }

            var templateInfo = nodeId == PublishmentSystemId ? TemplateManager.GetTemplateInfo(PublishmentSystemId, 0, ETemplateType.IndexPageTemplate) : TemplateManager.GetTemplateInfo(PublishmentSystemId, nodeId, ETemplateType.ChannelTemplate);
            var filePath = PathUtility.GetChannelPageFilePath(PublishmentSystemInfo, nodeId, 0);
            var pageInfo = new PageInfo(nodeId, 0, PublishmentSystemInfo, templateInfo, null);
            var contextInfo = new ContextInfo(pageInfo);
            var contentBuilder = new StringBuilder(StlCacheManager.FileContent.GetTemplateContent(PublishmentSystemInfo, templateInfo));

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

                StlUtility.ParseStl(PublishmentSystemInfo, pageInfo, contextInfo, contentBuilder, filePath, false);

                for (var currentPageIndex = 0; currentPageIndex < pageCount; currentPageIndex++)
                {
                    var thePageInfo = new PageInfo(pageInfo.PageNodeId, pageInfo.PageContentId, pageInfo.PublishmentSystemInfo, pageInfo.TemplateInfo, null);
                    var index = contentAttributeHtml.IndexOf(ContentUtility.PagePlaceHolder, StringComparison.Ordinal);
                    var length = index == -1 ? contentAttributeHtml.Length : index;
                    var pagedContentAttributeHtml = contentAttributeHtml.Substring(0, length);
                    var pagedBuilder = new StringBuilder(contentBuilder.ToString().Replace(stlPageContentElement, pagedContentAttributeHtml));
                    StlParserManager.ReplacePageElementsInChannelPage(pagedBuilder, thePageInfo, stlLabelList, thePageInfo.PageNodeId, currentPageIndex, pageCount, 0);

                    filePath = PathUtility.GetChannelPageFilePath(PublishmentSystemInfo, thePageInfo.PageNodeId, currentPageIndex);

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
                var stlElementTranslated = StlPageContents.Translate(stlElement);

                var pageContentsElementParser = new StlPageContents(stlElement, pageInfo, contextInfo, false);
                int totalNum;
                var pageCount = pageContentsElementParser.GetPageCount(out totalNum);

                StlUtility.ParseStl(PublishmentSystemInfo, pageInfo, contextInfo, contentBuilder, filePath, false);

                for (var currentPageIndex = 0; currentPageIndex < pageCount; currentPageIndex++)
                {
                    var thePageInfo = new PageInfo(pageInfo.PageNodeId, pageInfo.PageContentId, pageInfo.PublishmentSystemInfo, pageInfo.TemplateInfo, null);
                    var pageHtml = pageContentsElementParser.Parse(totalNum, currentPageIndex, pageCount, true);
                    var pagedBuilder = new StringBuilder(contentBuilder.ToString().Replace(stlElementTranslated, pageHtml));

                    StlParserManager.ReplacePageElementsInChannelPage(pagedBuilder, thePageInfo, stlLabelList, thePageInfo.PageNodeId, currentPageIndex, pageCount, totalNum);

                    filePath = PathUtility.GetChannelPageFilePath(PublishmentSystemInfo, thePageInfo.PageNodeId, currentPageIndex);
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
                var stlElementTranslated = StlPageChannels.Translate(stlElement);

                var pageChannelsElementParser = new StlPageChannels(stlElement, pageInfo, contextInfo, false);
                int totalNum;
                var pageCount = pageChannelsElementParser.GetPageCount(out totalNum);

                StlUtility.ParseStl(PublishmentSystemInfo, pageInfo, contextInfo, contentBuilder, filePath, false);

                for (var currentPageIndex = 0; currentPageIndex < pageCount; currentPageIndex++)
                {
                    var thePageInfo = new PageInfo(pageInfo.PageNodeId, pageInfo.PageContentId, pageInfo.PublishmentSystemInfo, pageInfo.TemplateInfo, null);
                    var pageHtml = pageChannelsElementParser.Parse(currentPageIndex, pageCount);
                    var pagedBuilder = new StringBuilder(contentBuilder.ToString().Replace(stlElementTranslated, pageHtml));

                    StlParserManager.ReplacePageElementsInChannelPage(pagedBuilder, thePageInfo, stlLabelList, thePageInfo.PageNodeId, currentPageIndex, pageCount, totalNum);

                    filePath = PathUtility.GetChannelPageFilePath(PublishmentSystemInfo, thePageInfo.PageNodeId, currentPageIndex);
                    GenerateFile(filePath, pageInfo.TemplateInfo.Charset, pagedBuilder);
                }
            }
            //如果标签中存在<stl:pageSqlContents>
            else if (StlParserUtility.IsStlElementExists(StlPageSqlContents.ElementName, stlLabelList))
            {
                var stlElement = StlParserUtility.GetStlElement(StlPageSqlContents.ElementName, stlLabelList);
                var stlElementTranslated = StlPageSqlContents.Translate(stlElement);

                var pageSqlContentsElementParser = new StlPageSqlContents(stlElement, pageInfo, contextInfo, false);
                int totalNum;
                var pageCount = pageSqlContentsElementParser.GetPageCount(out totalNum);

                StlUtility.ParseStl(PublishmentSystemInfo, pageInfo, contextInfo, contentBuilder, filePath, false);

                for (var currentPageIndex = 0; currentPageIndex < pageCount; currentPageIndex++)
                {
                    var thePageInfo = new PageInfo(pageInfo.PageNodeId, pageInfo.PageContentId, pageInfo.PublishmentSystemInfo, pageInfo.TemplateInfo, null);
                    var pageHtml = pageSqlContentsElementParser.Parse(currentPageIndex, pageCount);
                    var pagedBuilder = new StringBuilder(contentBuilder.ToString().Replace(stlElementTranslated, pageHtml));

                    StlParserManager.ReplacePageElementsInChannelPage(pagedBuilder, thePageInfo, stlLabelList, thePageInfo.PageNodeId, currentPageIndex, pageCount, totalNum);

                    filePath = PathUtility.GetChannelPageFilePath(PublishmentSystemInfo, thePageInfo.PageNodeId, currentPageIndex);
                    GenerateFile(filePath, pageInfo.TemplateInfo.Charset, pagedBuilder);
                }
            }
            //如果标签中存在<stl:StlPageInputContents>
            else if (StlParserUtility.IsStlElementExists(StlPageInputContents.ElementName, stlLabelList))
            {
                var stlElement = StlParserUtility.GetStlElement(StlPageInputContents.ElementName, stlLabelList);
                var stlElementTranslated = StlPageInputContents.Translate(stlElement);

                var pageInputContentsElementParser = new StlPageInputContents(stlElement, pageInfo, contextInfo, true);
                int totalNum;
                var pageCount = pageInputContentsElementParser.GetPageCount(out totalNum);

                StlUtility.ParseStl(PublishmentSystemInfo, pageInfo, contextInfo, contentBuilder, filePath, false);

                for (var currentPageIndex = 0; currentPageIndex < pageCount; currentPageIndex++)
                {
                    var thePageInfo = new PageInfo(pageInfo.PageNodeId, pageInfo.PageContentId, pageInfo.PublishmentSystemInfo, pageInfo.TemplateInfo, null);
                    var pageHtml = pageInputContentsElementParser.Parse(currentPageIndex, pageCount);
                    var pagedBuilder = new StringBuilder(contentBuilder.ToString().Replace(stlElementTranslated, pageHtml));

                    StlParserManager.ReplacePageElementsInChannelPage(pagedBuilder, thePageInfo, stlLabelList, thePageInfo.PageNodeId, currentPageIndex, pageCount, totalNum);

                    filePath = PathUtility.GetChannelPageFilePath(PublishmentSystemInfo, thePageInfo.PageNodeId, currentPageIndex);
                    GenerateFile(filePath, pageInfo.TemplateInfo.Charset, pagedBuilder);
                }
            }
            else
            {
                StlUtility.ParseStl(PublishmentSystemInfo, pageInfo, contextInfo, contentBuilder, filePath, false);
                GenerateFile(filePath, pageInfo.TemplateInfo.Charset, contentBuilder);
            }
        }

        public void CreateContents(int nodeId)
        {
            var nodeInfo = NodeManager.GetNodeInfo(PublishmentSystemId, nodeId);
            var tableStyle = NodeManager.GetTableStyle(PublishmentSystemInfo, nodeInfo);
            var tableName = NodeManager.GetTableName(PublishmentSystemInfo, nodeInfo);
            var orderByString = ETaxisTypeUtils.GetContentOrderByString(ETaxisType.OrderByTaxisDesc);
            var contentIdList = DataProvider.ContentDao.GetContentIdListChecked(tableName, nodeId, orderByString);
            foreach (var contentId in contentIdList)
            {
                CreateContent(tableStyle, tableName, nodeId, contentId);
            }
        }

        public void CreateContent(ETableStyle tableStyle, string tableName, int nodeId, int contentId)
        {
            var contentInfo = DataProvider.ContentDao.GetContentInfo(tableStyle, tableName, contentId);
            if (contentInfo == null) return;
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
            if (PublishmentSystemInfo.Additional.IsCreateStaticContentByAddDate && contentInfo.AddDate < PublishmentSystemInfo.Additional.CreateStaticContentAddDate)
            {
                return;
            }

            var templateInfo = TemplateManager.GetTemplateInfo(PublishmentSystemId, nodeId, ETemplateType.ContentTemplate);
            var pageInfo = new PageInfo(nodeId, contentId, PublishmentSystemInfo, templateInfo, null);
            var contextInfo = new ContextInfo(pageInfo)
            {
                ContentInfo = contentInfo
            };
            var filePath = PathUtility.GetContentPageFilePath(PublishmentSystemInfo, pageInfo.PageNodeId, contentInfo, 0);
            var contentBuilder = new StringBuilder(StlCacheManager.FileContent.GetTemplateContent(PublishmentSystemInfo, templateInfo));

            var stlLabelList = StlParserUtility.GetStlLabelList(contentBuilder.ToString());

            //如果标签中存在<stl:content type="PageContent"></stl:content>
            if (StlParserUtility.IsStlContentElementWithTypePageContent(stlLabelList))//内容存在
            {
                var stlElement = StlParserUtility.GetStlContentElementWithTypePageContent(stlLabelList);
                var stlElementTranslated = StlPageContents.Translate(stlElement);
                contentBuilder.Replace(stlElement, stlElementTranslated);

                var innerBuilder = new StringBuilder(stlElement);
                StlParserManager.ParseInnerContent(innerBuilder, pageInfo, contextInfo);
                var pageContentHtml = innerBuilder.ToString();
                var pageCount = StringUtils.GetCount(ContentUtility.PagePlaceHolder, pageContentHtml) + 1;//一共需要的页数
                
                StlUtility.ParseStl(PublishmentSystemInfo, pageInfo, contextInfo, contentBuilder, filePath, false);

                for (var currentPageIndex = 0; currentPageIndex < pageCount; currentPageIndex++)
                {
                    var thePageInfo = new PageInfo(pageInfo.PageNodeId, pageInfo.PageContentId, pageInfo.PublishmentSystemInfo, pageInfo.TemplateInfo, null);

                    var index = pageContentHtml.IndexOf(ContentUtility.PagePlaceHolder, StringComparison.Ordinal);
                    var length = index == -1 ? pageContentHtml.Length : index;

                    var pageHtml = pageContentHtml.Substring(0, length);
                    var pagedBuilder = new StringBuilder(contentBuilder.ToString().Replace(stlElementTranslated, pageHtml));
                    StlParserManager.ReplacePageElementsInContentPage(pagedBuilder, thePageInfo, stlLabelList, nodeId, contentId, currentPageIndex, pageCount);

                    filePath = PathUtility.GetContentPageFilePath(PublishmentSystemInfo, thePageInfo.PageNodeId, contentInfo, currentPageIndex);
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
                var stlElementTranslated = StlPageContents.Translate(stlElement);

                var pageContentsElementParser = new StlPageContents(stlElement, pageInfo, contextInfo, false);
                int totalNum;
                var pageCount = pageContentsElementParser.GetPageCount(out totalNum);

                StlUtility.ParseStl(PublishmentSystemInfo, pageInfo, contextInfo, contentBuilder, filePath, false);

                for (var currentPageIndex = 0; currentPageIndex < pageCount; currentPageIndex++)
                {
                    var thePageInfo = new PageInfo(pageInfo.PageNodeId, pageInfo.PageContentId, pageInfo.PublishmentSystemInfo, pageInfo.TemplateInfo, null);
                    var pageHtml = pageContentsElementParser.Parse(totalNum, currentPageIndex, pageCount, true);
                    var pagedBuilder = new StringBuilder(contentBuilder.ToString().Replace(stlElementTranslated, pageHtml));

                    StlParserManager.ReplacePageElementsInContentPage(pagedBuilder, thePageInfo, stlLabelList, nodeId, contentId, currentPageIndex, pageCount);

                    filePath = PathUtility.GetContentPageFilePath(PublishmentSystemInfo, thePageInfo.PageNodeId, contentInfo, currentPageIndex);
                    GenerateFile(filePath, pageInfo.TemplateInfo.Charset, pagedBuilder);
                }
            }
            //如果标签中存在<stl:pageChannels>
            else if (StlParserUtility.IsStlElementExists(StlPageChannels.ElementName, stlLabelList))
            {
                var stlElement = StlParserUtility.GetStlElement(StlPageChannels.ElementName, stlLabelList);
                var stlElementTranslated = StlPageChannels.Translate(stlElement);

                var pageChannelsElementParser = new StlPageChannels(stlElement, pageInfo, contextInfo, false);
                int totalNum;
                var pageCount = pageChannelsElementParser.GetPageCount(out totalNum);

                StlUtility.ParseStl(PublishmentSystemInfo, pageInfo, contextInfo, contentBuilder, filePath, false);

                for (var currentPageIndex = 0; currentPageIndex < pageCount; currentPageIndex++)
                {
                    var thePageInfo = new PageInfo(pageInfo.PageNodeId, pageInfo.PageContentId, pageInfo.PublishmentSystemInfo, pageInfo.TemplateInfo, null);
                    var pageHtml = pageChannelsElementParser.Parse(currentPageIndex, pageCount);
                    var pagedBuilder = new StringBuilder(contentBuilder.ToString().Replace(stlElementTranslated, pageHtml));

                    StlParserManager.ReplacePageElementsInContentPage(pagedBuilder, thePageInfo, stlLabelList, nodeId, contentId, currentPageIndex, pageCount);

                    filePath = PathUtility.GetContentPageFilePath(PublishmentSystemInfo, thePageInfo.PageNodeId, contentInfo, currentPageIndex);
                    GenerateFile(filePath, pageInfo.TemplateInfo.Charset, pagedBuilder);
                }
            }
            //如果标签中存在<stl:pageSqlContents>
            else if (StlParserUtility.IsStlElementExists(StlPageSqlContents.ElementName, stlLabelList))
            {
                var stlElement = StlParserUtility.GetStlElement(StlPageSqlContents.ElementName, stlLabelList);
                var stlElementTranslated = StlPageSqlContents.Translate(stlElement);

                var pageSqlContentsElementParser = new StlPageSqlContents(stlElement, pageInfo, contextInfo, false);
                int totalNum;
                var pageCount = pageSqlContentsElementParser.GetPageCount(out totalNum);

                StlUtility.ParseStl(PublishmentSystemInfo, pageInfo, contextInfo, contentBuilder, filePath, false);

                for (var currentPageIndex = 0; currentPageIndex < pageCount; currentPageIndex++)
                {
                    var thePageInfo = new PageInfo(pageInfo.PageNodeId, pageInfo.PageContentId, pageInfo.PublishmentSystemInfo, pageInfo.TemplateInfo, null);
                    var pageHtml = pageSqlContentsElementParser.Parse(currentPageIndex, pageCount);
                    var pagedBuilder = new StringBuilder(contentBuilder.ToString().Replace(stlElementTranslated, pageHtml));

                    StlParserManager.ReplacePageElementsInContentPage(pagedBuilder, thePageInfo, stlLabelList, nodeId, contentId, currentPageIndex, pageCount);

                    filePath = PathUtility.GetContentPageFilePath(PublishmentSystemInfo, thePageInfo.PageNodeId, contentInfo, currentPageIndex);
                    GenerateFile(filePath, pageInfo.TemplateInfo.Charset, pagedBuilder);
                }
            }
            else//无翻页
            {
                StlUtility.ParseStl(PublishmentSystemInfo, pageInfo, contextInfo, contentBuilder, filePath, false);
                GenerateFile(filePath, pageInfo.TemplateInfo.Charset, contentBuilder);
            }
        }

        public void CreateFile(int templateId)
        {
            var templateInfo = TemplateManager.GetTemplateInfo(PublishmentSystemId, templateId);
            if (templateInfo == null || templateInfo.TemplateType != ETemplateType.FileTemplate)
            {
                return;
            }
            var pageInfo = new PageInfo(PublishmentSystemId, 0, PublishmentSystemInfo, templateInfo, null);
            var contextInfo = new ContextInfo(pageInfo);
            var filePath = PathUtility.MapPath(PublishmentSystemInfo, templateInfo.CreatedFileFullName);

            //if (publishmentSystemInfo.Additional.VisualType == EVisualType.Dynamic)
            //{
            //    string pageUrl = PageUtility.GetFileUrl(publishmentSystemInfo, templateID, EVisualType.Dynamic);
            //    string content = StringUtility.GetRedirectPageHtml(pageUrl);
            //    this.GenerateFile(filePath, pageInfo.TemplateInfo.Charset, content);
            //    return;
            //}

            var contentBuilder = new StringBuilder(StlCacheManager.FileContent.GetTemplateContent(PublishmentSystemInfo, templateInfo));
            StlUtility.ParseStl(PublishmentSystemInfo, pageInfo, contextInfo, contentBuilder, filePath, false);
            GenerateFile(filePath, pageInfo.TemplateInfo.Charset, contentBuilder);
        }

        public string CreateIncludeFile(string virtualUrl, bool isCreateIfExists)
        {
            var templateInfo = new TemplateInfo(0, PublishmentSystemId, string.Empty, ETemplateType.FileTemplate, string.Empty, string.Empty, string.Empty, ECharsetUtils.GetEnumType(PublishmentSystemInfo.Additional.Charset), false);
            var pageInfo = new PageInfo(PublishmentSystemId, 0, PublishmentSystemInfo, templateInfo, null);
            var contextInfo = new ContextInfo(pageInfo);

            var parsedVirtualUrl = virtualUrl.Substring(0, virtualUrl.LastIndexOf('.')) + "_parsed" + virtualUrl.Substring(virtualUrl.LastIndexOf('.'));
            var filePath = PathUtility.MapPath(PublishmentSystemInfo, parsedVirtualUrl);
            if (!isCreateIfExists && FileUtils.IsFileExists(filePath)) return parsedVirtualUrl;

            var contentBuilder = new StringBuilder(StlCacheManager.FileContent.GetIncludeContent(PublishmentSystemInfo, virtualUrl, pageInfo.TemplateInfo.Charset));
            StlParserManager.ParseTemplateContent(contentBuilder, pageInfo, contextInfo);
            var pageAfterBodyScripts = StlParserManager.GetPageInfoScript(pageInfo, true);
            var pageBeforeBodyScripts = StlParserManager.GetPageInfoScript(pageInfo, false);
            contentBuilder.Insert(0, pageBeforeBodyScripts);
            contentBuilder.Append(pageAfterBodyScripts);
            GenerateFile(filePath, pageInfo.TemplateInfo.Charset, contentBuilder);
            return parsedVirtualUrl;
        }

        /// <summary>
        /// 在操作系统中创建文件，如果文件存在，重新创建此文件
        /// </summary>
        public void GenerateFile(string filePath, ECharset charset, StringBuilder contentBuilder)
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