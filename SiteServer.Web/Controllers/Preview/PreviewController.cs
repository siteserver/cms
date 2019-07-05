using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http;
using SiteServer.CMS.Api.Preview;
using SiteServer.Utils;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Model;
using SiteServer.CMS.Model.Attributes;
using SiteServer.CMS.StlParser;
using SiteServer.CMS.StlParser.Model;
using SiteServer.CMS.StlParser.StlElement;
using SiteServer.CMS.StlParser.Utility;
using SiteServer.Plugin;

namespace SiteServer.API.Controllers.Preview
{
    public class PreviewController : ApiController
    {
        [HttpGet, Route(ApiRoutePreview.Route)]
        public HttpResponseMessage Get(int siteId)
        {
            try
            {
                var pageIndex = TranslateUtils.ToInt(HttpContext.Current.Request.QueryString["pageIndex"]);

                var response = GetResponseMessage(VisualInfo.GetInstance(siteId, 0, 0, 0, pageIndex, 0));
                return response ?? Request.CreateResponse(HttpStatusCode.NotFound);
            }
            catch (Exception ex)
            {
                LogUtils.AddErrorLogAndRedirect(ex);
            }

            return Request.CreateResponse(HttpStatusCode.NotFound);
        }

        [HttpGet, Route(ApiRoutePreview.RouteChannel)]
        public HttpResponseMessage GetChannel(int siteId, int channelId)
        {
            try
            {
                var pageIndex = TranslateUtils.ToInt(HttpContext.Current.Request.QueryString["pageIndex"]);

                var response = GetResponseMessage(VisualInfo.GetInstance(siteId, channelId, 0, 0, pageIndex, 0));
                return response ?? Request.CreateResponse(HttpStatusCode.NotFound);
            }
            catch (Exception ex)
            {
                LogUtils.AddErrorLogAndRedirect(ex);
            }

            return Request.CreateResponse(HttpStatusCode.NotFound);
        }

        [HttpGet, Route(ApiRoutePreview.RouteContent)]
        public HttpResponseMessage GetContent(int siteId, int channelId, int contentId)
        {
            try
            {
                var pageIndex = TranslateUtils.ToInt(HttpContext.Current.Request.QueryString["pageIndex"]);
                var previewId = TranslateUtils.ToInt(HttpContext.Current.Request.QueryString["previewId"]);

                var response = GetResponseMessage(VisualInfo.GetInstance(siteId, channelId, contentId, 0, pageIndex, previewId));
                return response ?? Request.CreateResponse(HttpStatusCode.NotFound);
            }
            catch (Exception ex)
            {
                LogUtils.AddErrorLogAndRedirect(ex);
            }

            return Request.CreateResponse(HttpStatusCode.NotFound);
        }

        [HttpGet, Route(ApiRoutePreview.RouteFile)]
        public HttpResponseMessage GetFile(int siteId, int fileTemplateId)
        {
            try
            {
                var pageIndex = TranslateUtils.ToInt(HttpContext.Current.Request.QueryString["pageIndex"]);

                var response = GetResponseMessage(VisualInfo.GetInstance(siteId, 0, 0, fileTemplateId, pageIndex, 0));
                return response ?? Request.CreateResponse(HttpStatusCode.NotFound);
            }
            catch (Exception ex)
            {
                LogUtils.AddErrorLogAndRedirect(ex);
            }

            return Request.CreateResponse(HttpStatusCode.NotFound);
        }

        private HttpResponseMessage Response(string html, SiteInfo siteInfo)
        {
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content =
                    new StringContent(html,
                        Encoding.GetEncoding(siteInfo.Additional.Charset), "text/html")

            };
        }

        private HttpResponseMessage GetResponseMessage(VisualInfo visualInfo)
        {
            if (visualInfo.SiteInfo == null || visualInfo.TemplateInfo == null) return null;

            var siteInfo = visualInfo.SiteInfo;
            var templateInfo = visualInfo.TemplateInfo;

            var pageInfo = new PageInfo(visualInfo.ChannelId, visualInfo.ContentId, siteInfo, templateInfo, new Dictionary<string, object>())
            {
                IsLocal = true
            };
            var contextInfo = new ContextInfo(pageInfo)
            {
                ContextType = visualInfo.ContextType
            };

            var contentBuilder = new StringBuilder(TemplateManager.GetTemplateContent(siteInfo, templateInfo));
            //需要完善，考虑单页模板、内容正文、翻页及外部链接

            if (templateInfo.TemplateType == TemplateType.FileTemplate)           //单页
            {
                Parser.Parse(pageInfo, contextInfo, contentBuilder, visualInfo.FilePath, true);
                return Response(contentBuilder.ToString(), siteInfo);
            }
            if (templateInfo.TemplateType == TemplateType.IndexPageTemplate || templateInfo.TemplateType == TemplateType.ChannelTemplate)        //栏目页面
            {
                var nodeInfo = ChannelManager.GetChannelInfo(siteInfo.Id, visualInfo.ChannelId);
                if (nodeInfo == null) return null;

                if (nodeInfo.ParentId > 0)
                {
                    if (!string.IsNullOrEmpty(nodeInfo.LinkUrl))
                    {
                        PageUtils.Redirect(nodeInfo.LinkUrl);
                        return null;
                    }
                }

                var stlLabelList = StlParserUtility.GetStlLabelList(contentBuilder.ToString());

                //如果标签中存在Content
                var stlContentElement = string.Empty;

                foreach (var label in stlLabelList)
                {
                    if (StlParserUtility.IsStlChannelElement(label, ChannelAttribute.PageContent))
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
                        Parser.Parse(pageInfo, contextInfo, contentBuilder, visualInfo.FilePath, true);

                        for (var currentPageIndex = 0; currentPageIndex < pageCount; currentPageIndex++)
                        {
                            var thePageInfo = pageInfo.Clone();
                            thePageInfo.IsLocal = true;

                            var index = contentAttributeHtml.IndexOf(ContentUtility.PagePlaceHolder, StringComparison.Ordinal);
                            var length = index == -1 ? contentAttributeHtml.Length : index;

                            if (currentPageIndex == visualInfo.PageIndex)
                            {
                                var pagedContentAttributeHtml = contentAttributeHtml.Substring(0, length);
                                var pagedBuilder = new StringBuilder(contentBuilder.ToString().Replace(stlContentElement, pagedContentAttributeHtml));
                                StlParserManager.ReplacePageElementsInChannelPage(pagedBuilder, thePageInfo, stlLabelList, thePageInfo.PageChannelId, currentPageIndex, pageCount, 0);

                                return Response(pagedBuilder.ToString(), siteInfo);
                            }

                            if (index != -1)
                            {
                                contentAttributeHtml = contentAttributeHtml.Substring(length + ContentUtility.PagePlaceHolder.Length);
                            }
                        }
                        return null;
                    }
                    contentBuilder.Replace(stlContentElement, contentAttributeHtml);
                }

                if (StlParserUtility.IsStlElementExists(StlPageContents.ElementName, stlLabelList))//如果标签中存在<stl:pageContents>
                {
                    var stlElement = StlParserUtility.GetStlElement(StlPageContents.ElementName, stlLabelList);
                    var stlElementTranslated = StlParserManager.StlEncrypt(stlElement);

                    var pageContentsElementParser = new StlPageContents(stlElement, pageInfo, contextInfo);
                    var pageCount = pageContentsElementParser.GetPageCount(out var totalNum);

                    Parser.Parse(pageInfo, contextInfo, contentBuilder, visualInfo.FilePath, true);

                    for (var currentPageIndex = 0; currentPageIndex < pageCount; currentPageIndex++)
                    {
                        if (currentPageIndex == visualInfo.PageIndex)
                        {
                            var thePageInfo = pageInfo.Clone();
                            thePageInfo.IsLocal = true;

                            var pageHtml = pageContentsElementParser.Parse(totalNum, currentPageIndex, pageCount, false);
                            var pagedBuilder = new StringBuilder(contentBuilder.ToString().Replace(stlElementTranslated, pageHtml));

                            StlParserManager.ReplacePageElementsInChannelPage(pagedBuilder, thePageInfo, stlLabelList, thePageInfo.PageChannelId, currentPageIndex, pageCount, totalNum);

                            return Response(pagedBuilder.ToString(), siteInfo);
                        }
                    }
                }
                else if (StlParserUtility.IsStlElementExists(StlPageChannels.ElementName, stlLabelList))//如果标签中存在<stl:pageChannels>
                {
                    var stlElement = StlParserUtility.GetStlElement(StlPageChannels.ElementName, stlLabelList);
                    var stlElementTranslated = StlParserManager.StlEncrypt(stlElement);

                    var pageChannelsElementParser = new StlPageChannels(stlElement, pageInfo, contextInfo);
                    var pageCount = pageChannelsElementParser.GetPageCount(out var totalNum);

                    Parser.Parse(pageInfo, contextInfo, contentBuilder, visualInfo.FilePath, true);

                    for (var currentPageIndex = 0; currentPageIndex < pageCount; currentPageIndex++)
                    {
                        if (currentPageIndex == visualInfo.PageIndex)
                        {
                            var thePageInfo = pageInfo.Clone();
                            thePageInfo.IsLocal = true;

                            var pageHtml = pageChannelsElementParser.Parse(currentPageIndex, pageCount);
                            var pagedBuilder = new StringBuilder(contentBuilder.ToString().Replace(stlElementTranslated, pageHtml));

                            StlParserManager.ReplacePageElementsInChannelPage(pagedBuilder, thePageInfo, stlLabelList, thePageInfo.PageChannelId, currentPageIndex, pageCount, totalNum);

                            return Response(pagedBuilder.ToString(), siteInfo);
                        }
                    }
                }
                else if (StlParserUtility.IsStlElementExists(StlPageSqlContents.ElementName, stlLabelList))//如果标签中存在<stl:pageSqlContents>
                {
                    var stlElement = StlParserUtility.GetStlElement(StlPageSqlContents.ElementName, stlLabelList);
                    var stlElementTranslated = StlParserManager.StlEncrypt(stlElement);

                    var pageSqlContentsElementParser = new StlPageSqlContents(stlElement, pageInfo, contextInfo);
                    var pageCount = pageSqlContentsElementParser.GetPageCount(out var totalNum);

                    Parser.Parse(pageInfo, contextInfo, contentBuilder, visualInfo.FilePath, true);

                    for (var currentPageIndex = 0; currentPageIndex < pageCount; currentPageIndex++)
                    {
                        if (currentPageIndex == visualInfo.PageIndex)
                        {
                            var thePageInfo = pageInfo.Clone();
                            thePageInfo.IsLocal = true;

                            var pageHtml = pageSqlContentsElementParser.Parse(totalNum, currentPageIndex, pageCount, false);
                            var pagedBuilder = new StringBuilder(contentBuilder.ToString().Replace(stlElementTranslated, pageHtml));

                            StlParserManager.ReplacePageElementsInChannelPage(pagedBuilder, thePageInfo, stlLabelList, thePageInfo.PageChannelId, currentPageIndex, pageCount, totalNum);

                            return Response(pagedBuilder.ToString(), siteInfo);
                        }
                    }
                }

                Parser.Parse(pageInfo, contextInfo, contentBuilder, visualInfo.FilePath, true);
                return Response(contentBuilder.ToString(), siteInfo);
            }
            if (templateInfo.TemplateType == TemplateType.ContentTemplate)        //内容页面
            {
                if (contextInfo.ContentInfo == null) return null;

                if (!string.IsNullOrEmpty(contextInfo.ContentInfo.GetString(ContentAttribute.LinkUrl)))
                {
                    PageUtils.Redirect(contextInfo.ContentInfo.GetString(ContentAttribute.LinkUrl));
                    return null;
                }

                var stlLabelList = StlParserUtility.GetStlLabelList(contentBuilder.ToString());

                if (StlParserUtility.IsStlContentElementWithTypePageContent(stlLabelList))//内容存在
                {
                    var stlElement = StlParserUtility.GetStlContentElementWithTypePageContent(stlLabelList);
                    var stlElementTranslated = StlParserManager.StlEncrypt(stlElement);
                    contentBuilder.Replace(stlElement, stlElementTranslated);

                    var innerBuilder = new StringBuilder(stlElement);
                    StlParserManager.ParseInnerContent(innerBuilder, pageInfo, contextInfo);
                    var pageContentHtml = innerBuilder.ToString();
                    var pageCount = StringUtils.GetCount(ContentUtility.PagePlaceHolder, pageContentHtml) + 1;//一共需要的页数
                    Parser.Parse(pageInfo, contextInfo, contentBuilder, visualInfo.FilePath, true);

                    for (var currentPageIndex = 0; currentPageIndex < pageCount; currentPageIndex++)
                    {
                        var index = pageContentHtml.IndexOf(ContentUtility.PagePlaceHolder, StringComparison.Ordinal);
                        var length = index == -1 ? pageContentHtml.Length : index;

                        if (currentPageIndex == visualInfo.PageIndex)
                        {
                            var thePageInfo = pageInfo.Clone();
                            thePageInfo.IsLocal = true;

                            var pageHtml = pageContentHtml.Substring(0, length);

                            var pagedBuilder = new StringBuilder(contentBuilder.ToString().Replace(stlElementTranslated, pageHtml));

                            StlParserManager.ReplacePageElementsInContentPage(pagedBuilder, thePageInfo, stlLabelList, visualInfo.ChannelId, visualInfo.ContentId, currentPageIndex, pageCount);

                            return Response(pagedBuilder.ToString(), siteInfo);
                        }

                        if (index != -1)
                        {
                            pageContentHtml = pageContentHtml.Substring(length + ContentUtility.PagePlaceHolder.Length);
                        }
                    }
                }

                if (StlParserUtility.IsStlElementExists(StlPageContents.ElementName, stlLabelList))//如果标签中存在<stl:pageContents>
                {
                    var stlElement = StlParserUtility.GetStlElement(StlPageContents.ElementName, stlLabelList);
                    var stlElementTranslated = StlParserManager.StlEncrypt(stlElement);

                    var pageContentsElementParser = new StlPageContents(stlElement, pageInfo, contextInfo);
                    var pageCount = pageContentsElementParser.GetPageCount(out var totalNum);

                    Parser.Parse(pageInfo, contextInfo, contentBuilder, visualInfo.FilePath, true);

                    for (var currentPageIndex = 0; currentPageIndex < pageCount; currentPageIndex++)
                    {
                        if (currentPageIndex == visualInfo.PageIndex)
                        {
                            var thePageInfo = pageInfo.Clone();
                            thePageInfo.IsLocal = true;

                            var pageHtml = pageContentsElementParser.Parse(totalNum, currentPageIndex, pageCount, false);
                            var pagedBuilder = new StringBuilder(contentBuilder.ToString().Replace(stlElementTranslated, pageHtml));

                            StlParserManager.ReplacePageElementsInContentPage(pagedBuilder, thePageInfo, stlLabelList, visualInfo.ChannelId, visualInfo.ContentId, currentPageIndex, pageCount);

                            return Response(pagedBuilder.ToString(), siteInfo);
                        }
                    }
                }
                else if (StlParserUtility.IsStlElementExists(StlPageChannels.ElementName, stlLabelList))//如果标签中存在<stl:pageChannels>
                {
                    var stlElement = StlParserUtility.GetStlElement(StlPageChannels.ElementName, stlLabelList);
                    var stlElementTranslated = StlParserManager.StlEncrypt(stlElement);

                    var pageChannelsElementParser = new StlPageChannels(stlElement, pageInfo, contextInfo);
                    var pageCount = pageChannelsElementParser.GetPageCount(out _);

                    Parser.Parse(pageInfo, contextInfo, contentBuilder, visualInfo.FilePath, true);

                    for (var currentPageIndex = 0; currentPageIndex < pageCount; currentPageIndex++)
                    {
                        if (currentPageIndex == visualInfo.PageIndex)
                        {
                            var thePageInfo = pageInfo.Clone();
                            thePageInfo.IsLocal = true;

                            var pageHtml = pageChannelsElementParser.Parse(currentPageIndex, pageCount);
                            var pagedBuilder = new StringBuilder(contentBuilder.ToString().Replace(stlElementTranslated, pageHtml));

                            StlParserManager.ReplacePageElementsInContentPage(pagedBuilder, thePageInfo, stlLabelList, visualInfo.ChannelId, visualInfo.ContentId, currentPageIndex, pageCount);

                            return Response(pagedBuilder.ToString(), siteInfo);
                        }
                    }
                }
                else if (StlParserUtility.IsStlElementExists(StlPageSqlContents.ElementName, stlLabelList))//如果标签中存在<stl:pageSqlContents>
                {
                    var stlElement = StlParserUtility.GetStlElement(StlPageSqlContents.ElementName, stlLabelList);
                    var stlElementTranslated = StlParserManager.StlEncrypt(stlElement);

                    var pageSqlContentsElementParser = new StlPageSqlContents(stlElement, pageInfo, contextInfo);
                    var pageCount = pageSqlContentsElementParser.GetPageCount(out var totalNum);

                    Parser.Parse(pageInfo, contextInfo, contentBuilder, visualInfo.FilePath, true);

                    for (var currentPageIndex = 0; currentPageIndex < pageCount; currentPageIndex++)
                    {
                        if (currentPageIndex == visualInfo.PageIndex)
                        {
                            var thePageInfo = pageInfo.Clone();
                            thePageInfo.IsLocal = true;

                            var pageHtml = pageSqlContentsElementParser.Parse(totalNum, currentPageIndex, pageCount, false);
                            var pagedBuilder = new StringBuilder(contentBuilder.ToString().Replace(stlElementTranslated, pageHtml));

                            StlParserManager.ReplacePageElementsInContentPage(pagedBuilder, thePageInfo, stlLabelList, visualInfo.ChannelId, visualInfo.ContentId, currentPageIndex, pageCount);

                            return Response(pagedBuilder.ToString(), siteInfo);
                        }
                    }
                }

                Parser.Parse(pageInfo, contextInfo, contentBuilder, visualInfo.FilePath, true);
                StlParserManager.ReplacePageElementsInContentPage(contentBuilder, pageInfo, stlLabelList, contextInfo.ContentInfo.ChannelId, contextInfo.ContentInfo.Id, 0, 1);
                return Response(contentBuilder.ToString(), siteInfo);
            }

            return null;
        }
    }
}
