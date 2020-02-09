using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using SiteServer.Abstractions;
using SiteServer.CMS.Api.Preview;
using SiteServer.CMS.Context;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Repositories;
using SiteServer.CMS.StlParser;
using SiteServer.CMS.StlParser.Model;
using SiteServer.CMS.StlParser.StlElement;
using SiteServer.CMS.StlParser.Utility;


namespace SiteServer.API.Controllers.Preview
{
    public class PreviewController : ApiController
    {
        [HttpGet, Route(ApiRoutePreview.Route)]
        public async Task<HttpResponseMessage> Get(int siteId)
        {
            try
            {
                var pageIndex = TranslateUtils.ToInt(HttpContext.Current.Request.QueryString["pageIndex"]);

                var response = await GetResponseMessageAsync(await VisualInfo.GetInstanceAsync(siteId, 0, 0, 0, pageIndex, 0));
                return response ?? Request.CreateResponse(HttpStatusCode.NotFound);
            }
            catch (Exception ex)
            {
                await LogUtils.AddErrorLogAndRedirectAsync(ex);
            }

            return Request.CreateResponse(HttpStatusCode.NotFound);
        }

        [HttpGet, Route(ApiRoutePreview.RouteChannel)]
        public async Task<HttpResponseMessage> GetChannel(int siteId, int channelId)
        {
            try
            {
                var pageIndex = TranslateUtils.ToInt(HttpContext.Current.Request.QueryString["pageIndex"]);

                var response = await GetResponseMessageAsync(await VisualInfo.GetInstanceAsync(siteId, channelId, 0, 0, pageIndex, 0));
                return response ?? Request.CreateResponse(HttpStatusCode.NotFound);
            }
            catch (Exception ex)
            {
                await LogUtils.AddErrorLogAndRedirectAsync(ex);
            }

            return Request.CreateResponse(HttpStatusCode.NotFound);
        }

        [HttpGet, Route(ApiRoutePreview.RouteContent)]
        public async Task<HttpResponseMessage> GetContent(int siteId, int channelId, int contentId)
        {
            try
            {
                var pageIndex = TranslateUtils.ToInt(HttpContext.Current.Request.QueryString["pageIndex"]);
                var previewId = TranslateUtils.ToInt(HttpContext.Current.Request.QueryString["previewId"]);

                var response = await GetResponseMessageAsync(await VisualInfo.GetInstanceAsync(siteId, channelId, contentId, 0, pageIndex, previewId));
                return response ?? Request.CreateResponse(HttpStatusCode.NotFound);
            }
            catch (Exception ex)
            {
                await LogUtils.AddErrorLogAndRedirectAsync(ex);
            }

            return Request.CreateResponse(HttpStatusCode.NotFound);
        }

        [HttpGet, Route(ApiRoutePreview.RouteFile)]
        public async Task<HttpResponseMessage> GetFile(int siteId, int fileTemplateId)
        {
            try
            {
                var pageIndex = TranslateUtils.ToInt(HttpContext.Current.Request.QueryString["pageIndex"]);

                var response = await GetResponseMessageAsync(await VisualInfo.GetInstanceAsync(siteId, 0, 0, fileTemplateId, pageIndex, 0));
                return response ?? Request.CreateResponse(HttpStatusCode.NotFound);
            }
            catch (Exception ex)
            {
                await LogUtils.AddErrorLogAndRedirectAsync(ex);
            }

            return Request.CreateResponse(HttpStatusCode.NotFound);
        }

        private HttpResponseMessage Response(string html)
        {
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content =
                    new StringContent(html, Encoding.UTF8, "text/html")
            };
        }

        private async Task<HttpResponseMessage> GetResponseMessageAsync(VisualInfo visualInfo)
        {
            if (visualInfo.Site == null || visualInfo.Template == null) return null;

            var site = visualInfo.Site;
            var templateInfo = visualInfo.Template;

            var pageInfo = await PageInfo.GetPageInfoAsync(visualInfo.ChannelId, visualInfo.ContentId, site, templateInfo, new Dictionary<string, object>());

            pageInfo.IsLocal = true;

            var contextInfo = new ContextInfo(pageInfo)
            {
                ContextType = visualInfo.ContextType
            };

            var contentBuilder = new StringBuilder(DataProvider.TemplateRepository.GetTemplateContent(site, templateInfo));
            if (templateInfo.CreatedFileExtName == ".shtml")
            {
                //<!-- #include virtual="{Stl.SiteUrl}/include/head.html" -->

                var content = Regex.Replace(contentBuilder.ToString(), @"<!-- #include virtual=""([^""]+)"" -->", @"<stl:include file=""$1""></stl:include>");
                contentBuilder = new StringBuilder(content);
            }
            HttpResponseMessage message = null;

            if (templateInfo.TemplateType == TemplateType.FileTemplate)           //单页
            {
                message = await GetFileTemplateAsync(visualInfo, pageInfo, contextInfo, contentBuilder);
            }
            else if (templateInfo.TemplateType == TemplateType.IndexPageTemplate || templateInfo.TemplateType == TemplateType.ChannelTemplate)        //栏目页面
            {
                message = await GetChannelTemplateAsync(visualInfo, site, contentBuilder, pageInfo, contextInfo);
            }
            else if (templateInfo.TemplateType == TemplateType.ContentTemplate)        //内容页面
            {
                message = await GetContentTemplateAsync(visualInfo, contextInfo, contentBuilder, pageInfo);
            }

            return message;
        }

        private async Task<HttpResponseMessage> GetContentTemplateAsync(VisualInfo visualInfo, ContextInfo contextInfo, StringBuilder contentBuilder, PageInfo pageInfo)
        {
            var contentInfo = await contextInfo.GetContentAsync();
            if (contentInfo == null) return null;

            if (!string.IsNullOrEmpty(contentInfo.LinkUrl))
            {
                PageUtils.Redirect(contentInfo.LinkUrl);
                return null;
            }

            var stlLabelList = StlParserUtility.GetStlLabelList(contentBuilder.ToString());

            if (StlParserUtility.IsStlContentElementWithTypePageContent(stlLabelList)) //内容存在
            {
                var stlElement = StlParserUtility.GetStlContentElementWithTypePageContent(stlLabelList);
                var stlElementTranslated = StlParserManager.StlEncrypt(stlElement);
                contentBuilder.Replace(stlElement, stlElementTranslated);

                var innerBuilder = new StringBuilder(stlElement);
                await StlParserManager.ParseInnerContentAsync(innerBuilder, pageInfo, contextInfo);
                var pageContentHtml = innerBuilder.ToString();
                var pageCount = StringUtils.GetCount(ContentUtility.PagePlaceHolder, pageContentHtml) + 1; //一共需要的页数
                await Parser.ParseAsync(pageInfo, contextInfo, contentBuilder, visualInfo.FilePath, true);

                for (var currentPageIndex = 0; currentPageIndex < pageCount; currentPageIndex++)
                {
                    var index = pageContentHtml.IndexOf(ContentUtility.PagePlaceHolder, StringComparison.Ordinal);
                    var length = index == -1 ? pageContentHtml.Length : index;

                    if (currentPageIndex == visualInfo.PageIndex)
                    {
                        var thePageInfo = await pageInfo.CloneAsync();
                        thePageInfo.IsLocal = true;

                        var pageHtml = pageContentHtml.Substring(0, length);

                        var pagedBuilder = new StringBuilder(contentBuilder.ToString().Replace(stlElementTranslated, pageHtml));

                        await StlParserManager.ReplacePageElementsInContentPageAsync(pagedBuilder, thePageInfo, stlLabelList,
                            visualInfo.ChannelId, visualInfo.ContentId, currentPageIndex, pageCount);

                        return Response(pagedBuilder.ToString());
                    }

                    if (index != -1)
                    {
                        pageContentHtml = pageContentHtml.Substring(length + ContentUtility.PagePlaceHolder.Length);
                    }
                }
            }

            if (StlParserUtility.IsStlElementExists(StlPageContents.ElementName, stlLabelList)) //如果标签中存在<stl:pageContents>
            {
                var stlElement = StlParserUtility.GetStlElement(StlPageContents.ElementName, stlLabelList);
                var stlElementTranslated = StlParserManager.StlEncrypt(stlElement);

                var pageContentsElementParser = await StlPageContents.GetAsync(stlElement, pageInfo, contextInfo);
                var (pageCount, totalNum) = await pageContentsElementParser.GetPageCountAsync();

                await Parser.ParseAsync(pageInfo, contextInfo, contentBuilder, visualInfo.FilePath, true);

                for (var currentPageIndex = 0; currentPageIndex < pageCount; currentPageIndex++)
                {
                    if (currentPageIndex == visualInfo.PageIndex)
                    {
                        var thePageInfo = await pageInfo.CloneAsync();
                        thePageInfo.IsLocal = true;

                        var pageHtml = await pageContentsElementParser.ParseAsync(totalNum, currentPageIndex, pageCount, false);
                        var pagedBuilder = new StringBuilder(contentBuilder.ToString().Replace(stlElementTranslated, pageHtml));

                        await StlParserManager.ReplacePageElementsInContentPageAsync(pagedBuilder, thePageInfo, stlLabelList,
                            visualInfo.ChannelId, visualInfo.ContentId, currentPageIndex, pageCount);

                        return Response(pagedBuilder.ToString());
                    }
                }
            }
            else if (StlParserUtility.IsStlElementExists(StlPageChannels.ElementName, stlLabelList)) //如果标签中存在<stl:pageChannels>
            {
                var stlElement = StlParserUtility.GetStlElement(StlPageChannels.ElementName, stlLabelList);
                var stlElementTranslated = StlParserManager.StlEncrypt(stlElement);

                var pageChannelsElementParser = await StlPageChannels.GetAsync(stlElement, pageInfo, contextInfo);
                var pageCount = pageChannelsElementParser.GetPageCount(out _);

                await Parser.ParseAsync(pageInfo, contextInfo, contentBuilder, visualInfo.FilePath, true);

                for (var currentPageIndex = 0; currentPageIndex < pageCount; currentPageIndex++)
                {
                    if (currentPageIndex == visualInfo.PageIndex)
                    {
                        var thePageInfo = await pageInfo.CloneAsync();
                        thePageInfo.IsLocal = true;

                        var pageHtml = await pageChannelsElementParser.ParseAsync(currentPageIndex, pageCount);
                        var pagedBuilder = new StringBuilder(contentBuilder.ToString().Replace(stlElementTranslated, pageHtml));

                        await StlParserManager.ReplacePageElementsInContentPageAsync(pagedBuilder, thePageInfo, stlLabelList,
                            visualInfo.ChannelId, visualInfo.ContentId, currentPageIndex, pageCount);

                        return Response(pagedBuilder.ToString());
                    }
                }
            }
            else if (StlParserUtility.IsStlElementExists(StlPageSqlContents.ElementName, stlLabelList)
            ) //如果标签中存在<stl:pageSqlContents>
            {
                var stlElement = StlParserUtility.GetStlElement(StlPageSqlContents.ElementName, stlLabelList);
                var stlElementTranslated = StlParserManager.StlEncrypt(stlElement);

                var pageSqlContentsElementParser = await StlPageSqlContents.GetAsync(stlElement, pageInfo, contextInfo);
                var pageCount = pageSqlContentsElementParser.GetPageCount(out var totalNum);

                await Parser.ParseAsync(pageInfo, contextInfo, contentBuilder, visualInfo.FilePath, true);

                for (var currentPageIndex = 0; currentPageIndex < pageCount; currentPageIndex++)
                {
                    if (currentPageIndex == visualInfo.PageIndex)
                    {
                        var thePageInfo = await pageInfo.CloneAsync();
                        thePageInfo.IsLocal = true;

                        var pageHtml = await pageSqlContentsElementParser.ParseAsync(totalNum, currentPageIndex, pageCount, false);
                        var pagedBuilder = new StringBuilder(contentBuilder.ToString().Replace(stlElementTranslated, pageHtml));

                        await StlParserManager.ReplacePageElementsInContentPageAsync(pagedBuilder, thePageInfo, stlLabelList,
                            visualInfo.ChannelId, visualInfo.ContentId, currentPageIndex, pageCount);

                        return Response(pagedBuilder.ToString());
                    }
                }
            }

            await Parser.ParseAsync(pageInfo, contextInfo, contentBuilder, visualInfo.FilePath, true);
            await StlParserManager.ReplacePageElementsInContentPageAsync(contentBuilder, pageInfo, stlLabelList,
                contentInfo.ChannelId, contentInfo.Id, 0, 1);
            return Response(contentBuilder.ToString());
        }

        private async Task<HttpResponseMessage> GetChannelTemplateAsync(VisualInfo visualInfo, Site site, StringBuilder contentBuilder, PageInfo pageInfo, ContextInfo contextInfo)
        {
            var nodeInfo = await DataProvider.ChannelRepository.GetAsync(visualInfo.ChannelId);
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
                if (StlParserUtility.IsStlChannelElement(label, StlParserUtility.PageContent))
                {
                    stlContentElement = label;
                    break;
                }
            }

            if (!string.IsNullOrEmpty(stlContentElement)) //内容存在
            {
                var innerBuilder = new StringBuilder(stlContentElement);
                await StlParserManager.ParseInnerContentAsync(innerBuilder, pageInfo, contextInfo);
                var contentAttributeHtml = innerBuilder.ToString();
                var pageCount = StringUtils.GetCount(ContentUtility.PagePlaceHolder, contentAttributeHtml) + 1; //一共需要的页数
                if (pageCount > 1)
                {
                    await Parser.ParseAsync(pageInfo, contextInfo, contentBuilder, visualInfo.FilePath, true);

                    for (var currentPageIndex = 0; currentPageIndex < pageCount; currentPageIndex++)
                    {
                        var thePageInfo = await pageInfo.CloneAsync();
                        thePageInfo.IsLocal = true;

                        var index = contentAttributeHtml.IndexOf(ContentUtility.PagePlaceHolder, StringComparison.Ordinal);
                        var length = index == -1 ? contentAttributeHtml.Length : index;

                        if (currentPageIndex == visualInfo.PageIndex)
                        {
                            var pagedContentAttributeHtml = contentAttributeHtml.Substring(0, length);
                            var pagedBuilder = new StringBuilder(contentBuilder.ToString()
                                .Replace(stlContentElement, pagedContentAttributeHtml));
                            await StlParserManager.ReplacePageElementsInChannelPageAsync(pagedBuilder, thePageInfo, stlLabelList,
                                thePageInfo.PageChannelId, currentPageIndex, pageCount, 0);

                            return Response(pagedBuilder.ToString());
                        }

                        if (index != -1)
                        {
                            contentAttributeHtml =
                                contentAttributeHtml.Substring(length + ContentUtility.PagePlaceHolder.Length);
                        }
                    }

                    return null;
                }

                contentBuilder.Replace(stlContentElement, contentAttributeHtml);
            }

            if (StlParserUtility.IsStlElementExists(StlPageContents.ElementName, stlLabelList)) //如果标签中存在<stl:pageContents>
            {
                var stlElement = StlParserUtility.GetStlElement(StlPageContents.ElementName, stlLabelList);
                var stlElementTranslated = StlParserManager.StlEncrypt(stlElement);

                var pageContentsElementParser = await StlPageContents.GetAsync(stlElement, pageInfo, contextInfo);
                var (pageCount, totalNum) = await pageContentsElementParser.GetPageCountAsync();

                await Parser.ParseAsync(pageInfo, contextInfo, contentBuilder, visualInfo.FilePath, true);

                for (var currentPageIndex = 0; currentPageIndex < pageCount; currentPageIndex++)
                {
                    if (currentPageIndex == visualInfo.PageIndex)
                    {
                        var thePageInfo = await pageInfo.CloneAsync();
                        thePageInfo.IsLocal = true;

                        var pageHtml = await pageContentsElementParser.ParseAsync(totalNum, currentPageIndex, pageCount, false);
                        var pagedBuilder = new StringBuilder(contentBuilder.ToString().Replace(stlElementTranslated, pageHtml));

                        await StlParserManager.ReplacePageElementsInChannelPageAsync(pagedBuilder, thePageInfo, stlLabelList,
                            thePageInfo.PageChannelId, currentPageIndex, pageCount, totalNum);

                        return Response(pagedBuilder.ToString());
                    }
                }
            }
            else if (StlParserUtility.IsStlElementExists(StlPageChannels.ElementName, stlLabelList)) //如果标签中存在<stl:pageChannels>
            {
                var stlElement = StlParserUtility.GetStlElement(StlPageChannels.ElementName, stlLabelList);
                var stlElementTranslated = StlParserManager.StlEncrypt(stlElement);

                var pageChannelsElementParser = await StlPageChannels.GetAsync(stlElement, pageInfo, contextInfo);
                var pageCount = pageChannelsElementParser.GetPageCount(out var totalNum);

                await Parser.ParseAsync(pageInfo, contextInfo, contentBuilder, visualInfo.FilePath, true);

                for (var currentPageIndex = 0; currentPageIndex < pageCount; currentPageIndex++)
                {
                    if (currentPageIndex == visualInfo.PageIndex)
                    {
                        var thePageInfo = await pageInfo.CloneAsync();
                        thePageInfo.IsLocal = true;

                        var pageHtml = await pageChannelsElementParser.ParseAsync(currentPageIndex, pageCount);
                        var pagedBuilder = new StringBuilder(contentBuilder.ToString().Replace(stlElementTranslated, pageHtml));

                        await StlParserManager.ReplacePageElementsInChannelPageAsync(pagedBuilder, thePageInfo, stlLabelList,
                            thePageInfo.PageChannelId, currentPageIndex, pageCount, totalNum);

                        return Response(pagedBuilder.ToString());
                    }
                }
            }
            else if (StlParserUtility.IsStlElementExists(StlPageSqlContents.ElementName, stlLabelList)
            ) //如果标签中存在<stl:pageSqlContents>
            {
                var stlElement = StlParserUtility.GetStlElement(StlPageSqlContents.ElementName, stlLabelList);
                var stlElementTranslated = StlParserManager.StlEncrypt(stlElement);

                var pageSqlContentsElementParser = await StlPageSqlContents.GetAsync(stlElement, pageInfo, contextInfo);
                var pageCount = pageSqlContentsElementParser.GetPageCount(out var totalNum);

                await Parser.ParseAsync(pageInfo, contextInfo, contentBuilder, visualInfo.FilePath, true);

                for (var currentPageIndex = 0; currentPageIndex < pageCount; currentPageIndex++)
                {
                    if (currentPageIndex == visualInfo.PageIndex)
                    {
                        var thePageInfo = await pageInfo.CloneAsync();
                        thePageInfo.IsLocal = true;

                        var pageHtml = await pageSqlContentsElementParser.ParseAsync(totalNum, currentPageIndex, pageCount, false);
                        var pagedBuilder = new StringBuilder(contentBuilder.ToString().Replace(stlElementTranslated, pageHtml));

                        await StlParserManager.ReplacePageElementsInChannelPageAsync(pagedBuilder, thePageInfo, stlLabelList,
                            thePageInfo.PageChannelId, currentPageIndex, pageCount, totalNum);

                        return Response(pagedBuilder.ToString());
                    }
                }
            }

            await Parser.ParseAsync(pageInfo, contextInfo, contentBuilder, visualInfo.FilePath, true);
            return Response(contentBuilder.ToString());
        }

        private async Task<HttpResponseMessage> GetFileTemplateAsync(VisualInfo visualInfo, PageInfo pageInfo, ContextInfo contextInfo, StringBuilder contentBuilder)
        {
            await Parser.ParseAsync(pageInfo, contextInfo, contentBuilder, visualInfo.FilePath, true);
            return Response(contentBuilder.ToString());
        }
    }
}
