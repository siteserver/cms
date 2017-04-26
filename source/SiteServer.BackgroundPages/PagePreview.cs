using System.Text;
using BaiRong.Core;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;
using System;
using System.Collections.Specialized;
using System.Web.UI;
using BaiRong.Core.Model.Attributes;
using SiteServer.CMS.Model.Enumerations;
using SiteServer.CMS.StlParser.Model;
using SiteServer.CMS.StlParser.StlElement;
using SiteServer.CMS.StlParser.Utility;

namespace SiteServer.BackgroundPages
{
    public class PagePreview : Page
    {
        public static string GetRedirectUrl(int publishmentSystemId, int channelId, int contentId, int fileTemplateId, int pageIndex)
        {
            var queryString = new NameValueCollection
            {
                {"s", publishmentSystemId.ToString()}
            };
            if (channelId > 0)
            {
                queryString.Add("n", channelId.ToString());
            }
            if (contentId > 0)
            {
                queryString.Add("c", contentId.ToString());
            }
            if (fileTemplateId > 0)
            {
                queryString.Add("f", fileTemplateId.ToString());
            }
            if (pageIndex > 0)
            {
                queryString.Add("p", pageIndex.ToString());
            }
            
            return PageUtils.GetSiteServerUrl(nameof(PagePreview), queryString);
        }

        public void Page_Load(object sender, EventArgs e)
        {
            try
            {
                var visualInfo = VisualInfo.GetInstance();

                var publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(visualInfo.PublishmentSystemID);

                var templateInfo = TemplateManager.GetTemplateInfo(visualInfo.PublishmentSystemID, visualInfo.ChannelID, visualInfo.TemplateType);
                if (templateInfo == null)
                {
                    templateInfo = TemplateManager.GetTemplateInfo(visualInfo.PublishmentSystemID, visualInfo.FileTemplateID);
                }

                var pageInfo = new PageInfo(visualInfo.ChannelID, visualInfo.ContentID, publishmentSystemInfo, templateInfo, null);
                var contextInfo = new ContextInfo(pageInfo);

                StringBuilder contentBuilder = null;
                contentBuilder = new StringBuilder(StlCacheManager.FileContent.GetTemplateContent(publishmentSystemInfo, templateInfo));
                //需要完善，考虑单页模板、内容正文、翻页及外部链接

                if (visualInfo.TemplateType == ETemplateType.IndexPageTemplate)             //首页
                {
                    WriteResponse(contentBuilder, publishmentSystemInfo, pageInfo, contextInfo, visualInfo);
                    return;
                }
                else if (visualInfo.TemplateType == ETemplateType.FileTemplate)           //单页
                {
                    var fileTemplateInfo = TemplateManager.GetTemplateInfo(visualInfo.PublishmentSystemID, visualInfo.FileTemplateID);
                    var filePageInfo = new PageInfo(visualInfo.ChannelID, visualInfo.ContentID, publishmentSystemInfo, fileTemplateInfo, null);
                    var fileContentBuilder = new StringBuilder(StlCacheManager.FileContent.GetTemplateContent(publishmentSystemInfo, fileTemplateInfo));
                    WriteResponse(fileContentBuilder, publishmentSystemInfo, filePageInfo, contextInfo, visualInfo);
                    return;
                }
                else if (visualInfo.TemplateType == ETemplateType.ChannelTemplate)        //栏目页面
                {
                    var nodeInfo = NodeManager.GetNodeInfo(visualInfo.PublishmentSystemID, visualInfo.ChannelID);
                    if (nodeInfo == null) return;

                    if (nodeInfo.NodeType != ENodeType.BackgroundPublishNode)
                    {
                        if (!string.IsNullOrEmpty(nodeInfo.LinkUrl))
                        {
                            Response.Redirect(nodeInfo.LinkUrl);
                            return;
                        }
                    }

                    var stlLabelList = StlParserUtility.GetStlLabelList(contentBuilder.ToString());

                    //如果标签中存在Content
                    var stlContentElement = string.Empty;

                    foreach (var label in stlLabelList)
                    {
                        if (StlParserUtility.IsStlChannelElement(label, NodeAttribute.PageContent))
                        {
                            stlContentElement = label;
                            break;
                        }
                    }

                    if (!string.IsNullOrEmpty(stlContentElement))//内容存在
                    {
                        var innerBuilder = new StringBuilder(stlContentElement);
                        StlParserManager.ParseInnerContent(innerBuilder, pageInfo, contextInfo);
                        var contentAttributeHtml = innerBuilder.ToString();
                        var pageCount = StringUtils.GetCount(ContentUtility.PagePlaceHolder, contentAttributeHtml) + 1;//一共需要的页数
                        if (pageCount > 1)
                        {
                            for (var currentPageIndex = 0; currentPageIndex < pageCount; currentPageIndex++)
                            {
                                var thePageInfo = new PageInfo(pageInfo.PageNodeId, pageInfo.PageContentId, pageInfo.PublishmentSystemInfo, pageInfo.TemplateInfo, null);
                                var index = contentAttributeHtml.IndexOf(ContentUtility.PagePlaceHolder);
                                var length = (index == -1) ? contentAttributeHtml.Length : index;

                                if (currentPageIndex == visualInfo.PageIndex)
                                {
                                    var pagedContentAttributeHtml = contentAttributeHtml.Substring(0, length);
                                    var pagedBuilder = new StringBuilder(contentBuilder.ToString().Replace(stlContentElement, pagedContentAttributeHtml));
                                    StlParserManager.ReplacePageElementsInChannelPage(pagedBuilder, thePageInfo, stlLabelList, thePageInfo.PageNodeId, currentPageIndex, pageCount, 0);

                                    WriteResponse(pagedBuilder, publishmentSystemInfo, thePageInfo, contextInfo, visualInfo);
                                    return;
                                }

                                if (index != -1)
                                {
                                    contentAttributeHtml = contentAttributeHtml.Substring(length + ContentUtility.PagePlaceHolder.Length);
                                }
                            }
                            return;
                        }
                        contentBuilder.Replace(stlContentElement, contentAttributeHtml);
                    }

                    if (StlParserUtility.IsStlElementExists(StlPageContents.ElementName, stlLabelList))//如果标签中存在<stl:pageContents>
                    {
                        var stlElement = StlParserUtility.GetStlElement(StlPageContents.ElementName, stlLabelList);
                        var stlPageContentsElement = stlElement;
                        var stlPageContentsElementReplaceString = stlElement;

                        var pageContentsElementParser = new StlPageContents(stlPageContentsElement, pageInfo, contextInfo, false);
                        var totalNum = 0;
                        var pageCount = pageContentsElementParser.GetPageCount(out totalNum);

                        for (var currentPageIndex = 0; currentPageIndex < pageCount; currentPageIndex++)
                        {
                            if (currentPageIndex == visualInfo.PageIndex)
                            {
                                var thePageInfo = new PageInfo(pageInfo.PageNodeId, pageInfo.PageContentId, pageInfo.PublishmentSystemInfo, pageInfo.TemplateInfo, null);
                                var pageHtml = pageContentsElementParser.Parse(totalNum, currentPageIndex, pageCount, false);
                                var pagedBuilder = new StringBuilder(contentBuilder.ToString().Replace(stlPageContentsElementReplaceString, pageHtml));

                                StlParserManager.ReplacePageElementsInChannelPage(pagedBuilder, thePageInfo, stlLabelList, thePageInfo.PageNodeId, currentPageIndex, pageCount, totalNum);

                                WriteResponse(pagedBuilder, publishmentSystemInfo, thePageInfo, contextInfo, visualInfo);
                                return;
                            }
                        }
                    }
                    else if (StlParserUtility.IsStlElementExists(StlPageChannels.ElementName, stlLabelList))//如果标签中存在<stl:pageChannels>
                    {
                        var stlElement = StlParserUtility.GetStlElement(StlPageChannels.ElementName, stlLabelList);
                        var stlPageChannelsElement = stlElement;
                        var stlPageChannelsElementReplaceString = stlElement;

                        var pageChannelsElementParser = new StlPageChannels(stlPageChannelsElement, pageInfo, contextInfo, false);
                        var totalNum = 0;
                        var pageCount = pageChannelsElementParser.GetPageCount(out totalNum);

                        for (var currentPageIndex = 0; currentPageIndex < pageCount; currentPageIndex++)
                        {
                            if (currentPageIndex == visualInfo.PageIndex)
                            {
                                var thePageInfo = new PageInfo(pageInfo.PageNodeId, pageInfo.PageContentId, pageInfo.PublishmentSystemInfo, pageInfo.TemplateInfo, null);
                                var pageHtml = pageChannelsElementParser.Parse(currentPageIndex, pageCount);
                                var pagedBuilder = new StringBuilder(contentBuilder.ToString().Replace(stlPageChannelsElementReplaceString, pageHtml));

                                StlParserManager.ReplacePageElementsInChannelPage(pagedBuilder, thePageInfo, stlLabelList, thePageInfo.PageNodeId, currentPageIndex, pageCount, totalNum);

                                WriteResponse(pagedBuilder, publishmentSystemInfo, thePageInfo, contextInfo, visualInfo);
                                return;
                            }
                        }
                    }
                    else if (StlParserUtility.IsStlElementExists(StlPageSqlContents.ElementName, stlLabelList))//如果标签中存在<stl:pageSqlContents>
                    {
                        var stlElement = StlParserUtility.GetStlElement(StlPageSqlContents.ElementName, stlLabelList);
                        var stlPageSqlContentsElement = stlElement;
                        var stlPageSqlContentsElementReplaceString = stlElement;

                        var pageSqlContentsElementParser = new StlPageSqlContents(stlPageSqlContentsElement, pageInfo, contextInfo, false);
                        var totalNum = 0;
                        var pageCount = pageSqlContentsElementParser.GetPageCount(out totalNum);

                        for (var currentPageIndex = 0; currentPageIndex < pageCount; currentPageIndex++)
                        {
                            if (currentPageIndex == visualInfo.PageIndex)
                            {
                                var thePageInfo = new PageInfo(pageInfo.PageNodeId, pageInfo.PageContentId, pageInfo.PublishmentSystemInfo, pageInfo.TemplateInfo, null);
                                var pageHtml = pageSqlContentsElementParser.Parse(currentPageIndex, pageCount);
                                var pagedBuilder = new StringBuilder(contentBuilder.ToString().Replace(stlPageSqlContentsElementReplaceString, pageHtml));

                                StlParserManager.ReplacePageElementsInChannelPage(pagedBuilder, thePageInfo, stlLabelList, thePageInfo.PageNodeId, currentPageIndex, pageCount, totalNum);

                                WriteResponse(pagedBuilder, publishmentSystemInfo, thePageInfo, contextInfo, visualInfo);
                                return;
                            }
                        }
                    }
                    else
                    {
                        WriteResponse(contentBuilder, publishmentSystemInfo, pageInfo, contextInfo, visualInfo);
                        return;
                    }
                }
                else if (visualInfo.TemplateType == ETemplateType.ContentTemplate)        //内容页面
                {
                    if (contextInfo.ContentInfo == null) return;

                    if (!string.IsNullOrEmpty(contextInfo.ContentInfo.GetExtendedAttribute(BackgroundContentAttribute.LinkUrl)))
                    {
                        Response.Redirect(contextInfo.ContentInfo.GetExtendedAttribute(BackgroundContentAttribute.LinkUrl));
                        return;
                    }

                    var filePath = PathUtility.GetContentPageFilePath(publishmentSystemInfo, pageInfo.PageNodeId, pageInfo.PageContentId, 0);

                    var stlLabelList = StlParserUtility.GetStlLabelList(contentBuilder.ToString());

                    //如果标签中存在Content
                    var stlContentElement = string.Empty;

                    foreach (var label in stlLabelList)
                    {
                        if (StlParserUtility.IsStlContentElement(label, BackgroundContentAttribute.PageContent))
                        {
                            stlContentElement = label;
                            break;
                        }
                    }

                    if (!string.IsNullOrEmpty(stlContentElement))//内容存在
                    {
                        var innerBuilder = new StringBuilder(stlContentElement);
                        StlParserManager.ParseInnerContent(innerBuilder, pageInfo, contextInfo);
                        var contentAttributeHtml = innerBuilder.ToString();
                        var pageCount = StringUtils.GetCount(ContentUtility.PagePlaceHolder, contentAttributeHtml) + 1;//一共需要的页数
                        if (pageCount > 1)
                        {
                            for (var currentPageIndex = 0; currentPageIndex < pageCount; currentPageIndex++)
                            {
                                var index = contentAttributeHtml.IndexOf(ContentUtility.PagePlaceHolder);
                                var length = (index == -1) ? contentAttributeHtml.Length : index;

                                if (currentPageIndex == visualInfo.PageIndex)
                                {
                                    var thePageInfo = new PageInfo(pageInfo.PageNodeId, pageInfo.PageContentId, pageInfo.PublishmentSystemInfo, pageInfo.TemplateInfo, null);
                                    var pagedContentAttributeHtml = contentAttributeHtml.Substring(0, length);
                                    var pagedBuilder = new StringBuilder(contentBuilder.ToString().Replace(stlContentElement, pagedContentAttributeHtml));
                                    StlParserManager.ReplacePageElementsInContentPage(pagedBuilder, thePageInfo, stlLabelList, visualInfo.ChannelID, visualInfo.ContentID, currentPageIndex, pageCount);

                                    WriteResponse(pagedBuilder, publishmentSystemInfo, thePageInfo, contextInfo, visualInfo);
                                    return;
                                }

                                if (index != -1)
                                {
                                    contentAttributeHtml = contentAttributeHtml.Substring(length + ContentUtility.PagePlaceHolder.Length);
                                }
                            }
                            return;
                        }
                        contentBuilder.Replace(stlContentElement, contentAttributeHtml);
                    }

                    if (StlParserUtility.IsStlElementExists(StlPageContents.ElementName, stlLabelList))//如果标签中存在<stl:pageContents>
                    {
                        var stlElement = StlParserUtility.GetStlElement(StlPageContents.ElementName, stlLabelList);
                        var stlPageContentsElement = stlElement;
                        var stlPageContentsElementReplaceString = stlElement;

                        var pageContentsElementParser = new StlPageContents(stlPageContentsElement, pageInfo, contextInfo, false);
                        var totalNum = 0;
                        var pageCount = pageContentsElementParser.GetPageCount(out totalNum);

                        for (var currentPageIndex = 0; currentPageIndex < pageCount; currentPageIndex++)
                        {
                            if (currentPageIndex == visualInfo.PageIndex)
                            {
                                var thePageInfo = new PageInfo(pageInfo.PageNodeId, pageInfo.PageContentId, pageInfo.PublishmentSystemInfo, pageInfo.TemplateInfo, null);
                                var pageHtml = pageContentsElementParser.Parse(totalNum, currentPageIndex, pageCount, false);
                                var pagedBuilder = new StringBuilder(contentBuilder.ToString().Replace(stlPageContentsElementReplaceString, pageHtml));

                                StlParserManager.ReplacePageElementsInContentPage(pagedBuilder, thePageInfo, stlLabelList, visualInfo.ChannelID, visualInfo.ContentID, currentPageIndex, pageCount);

                                WriteResponse(pagedBuilder, publishmentSystemInfo, thePageInfo, contextInfo, visualInfo);
                                return;
                            }
                        }
                    }
                    else if (StlParserUtility.IsStlElementExists(StlPageChannels.ElementName, stlLabelList))//如果标签中存在<stl:pageChannels>
                    {
                        var stlElement = StlParserUtility.GetStlElement(StlPageChannels.ElementName, stlLabelList);
                        var stlPageChannelsElement = stlElement;
                        var stlPageChannelsElementReplaceString = stlElement;

                        var pageChannelsElementParser = new StlPageChannels(stlPageChannelsElement, pageInfo, contextInfo, false);
                        var totalNum = 0;
                        var pageCount = pageChannelsElementParser.GetPageCount(out totalNum);

                        for (var currentPageIndex = 0; currentPageIndex < pageCount; currentPageIndex++)
                        {
                            if (currentPageIndex == visualInfo.PageIndex)
                            {
                                var thePageInfo = new PageInfo(pageInfo.PageNodeId, pageInfo.PageContentId, pageInfo.PublishmentSystemInfo, pageInfo.TemplateInfo, null);
                                var pageHtml = pageChannelsElementParser.Parse(currentPageIndex, pageCount);
                                var pagedBuilder = new StringBuilder(contentBuilder.ToString().Replace(stlPageChannelsElementReplaceString, pageHtml));

                                StlParserManager.ReplacePageElementsInContentPage(pagedBuilder, thePageInfo, stlLabelList, visualInfo.ChannelID, visualInfo.ContentID, currentPageIndex, pageCount);

                                WriteResponse(pagedBuilder, publishmentSystemInfo, thePageInfo, contextInfo, visualInfo);
                                return;
                            }
                        }
                    }
                    else if (StlParserUtility.IsStlElementExists(StlPageSqlContents.ElementName, stlLabelList))//如果标签中存在<stl:pageSqlContents>
                    {
                        var stlElement = StlParserUtility.GetStlElement(StlPageSqlContents.ElementName, stlLabelList);
                        var stlPageSqlContentsElement = stlElement;
                        var stlPageSqlContentsElementReplaceString = stlElement;

                        var pageSqlContentsElementParser = new StlPageSqlContents(stlPageSqlContentsElement, pageInfo, contextInfo, false);
                        var totalNum = 0;
                        var pageCount = pageSqlContentsElementParser.GetPageCount(out totalNum);

                        for (var currentPageIndex = 0; currentPageIndex < pageCount; currentPageIndex++)
                        {
                            if (currentPageIndex == visualInfo.PageIndex)
                            {
                                var thePageInfo = new PageInfo(pageInfo.PageNodeId, pageInfo.PageContentId, pageInfo.PublishmentSystemInfo, pageInfo.TemplateInfo, null);
                                var pageHtml = pageSqlContentsElementParser.Parse(currentPageIndex, pageCount);
                                var pagedBuilder = new StringBuilder(contentBuilder.ToString().Replace(stlPageSqlContentsElementReplaceString, pageHtml));

                                StlParserManager.ReplacePageElementsInContentPage(pagedBuilder, thePageInfo, stlLabelList, visualInfo.ChannelID, visualInfo.ContentID, currentPageIndex, pageCount);

                                WriteResponse(pagedBuilder, publishmentSystemInfo, thePageInfo, contextInfo, visualInfo);
                                return;
                            }
                        }
                    }
                    else//无翻页
                    {
                        WriteResponse(contentBuilder, publishmentSystemInfo, pageInfo, contextInfo, visualInfo);
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                Response.Write(ex.Message);
            }
        }

        private void WriteResponse(StringBuilder contentBuilder, PublishmentSystemInfo publishmentSystemInfo, PageInfo pageInfo, ContextInfo contextInfo, VisualInfo visualInfo)
        {
            StlUtility.ParseStl(publishmentSystemInfo, pageInfo, contextInfo, contentBuilder, visualInfo.FilePath, true);//生成页面

            Response.ContentEncoding = Encoding.GetEncoding(publishmentSystemInfo.Additional.Charset);

            Response.Write(contentBuilder.ToString());
        }
    }
}
