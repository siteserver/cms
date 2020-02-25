using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Mvc;
using SS.CMS.Abstractions;
using SS.CMS.Api.Preview;
using SS.CMS.Core;
using SS.CMS.Framework;
using SS.CMS.StlParser;
using SS.CMS.StlParser.Model;
using SS.CMS.StlParser.StlElement;
using SS.CMS.StlParser.Utility;
using SS.CMS.Web.Controllers.Admin;

namespace SS.CMS.Web.Controllers.Preview
{
    public partial class PreviewController : ControllerBase
    {
        private readonly IAuthManager _authManager;
        private readonly ICreateManager _createManager;
        private readonly IPathManager _pathManager;
        private readonly IConfigRepository _configRepository;
        private readonly IAdministratorRepository _administratorRepository;
        private readonly ISiteRepository _siteRepository;
        private readonly IChannelRepository _channelRepository;
        private readonly IContentRepository _contentRepository;
        private readonly IDbCacheRepository _dbCacheRepository;

        public PreviewController(IAuthManager authManager, ICreateManager createManager, IPathManager pathManager, IConfigRepository configRepository, IAdministratorRepository administratorRepository, ISiteRepository siteRepository, IChannelRepository channelRepository, IContentRepository contentRepository, IDbCacheRepository dbCacheRepository)
        {
            _authManager = authManager;
            _createManager = createManager;
            _pathManager = pathManager;
            _configRepository = configRepository;
            _administratorRepository = administratorRepository;
            _siteRepository = siteRepository;
            _channelRepository = channelRepository;
            _contentRepository = contentRepository;
            _dbCacheRepository = dbCacheRepository;
        }

        [HttpGet, Route(ApiRoutePreview.Route)]
        public async Task<HttpResponseMessage> Get([FromQuery]GetRequest request)
        {
            try
            {
                return await GetResponseMessageAsync(await VisualInfo.GetInstanceAsync(request.SiteId, 0, 0, 0, request.PageIndex, 0));
            }
            catch (Exception ex)
            {
                HttpContext.Response.Redirect(_pathManager.GetAdminUrl(ErrorController.Route) + "/?message=" + HttpUtility.UrlPathEncode(ex.Message));
            }

            return null;
        }

        [HttpGet, Route(ApiRoutePreview.RouteChannel)]
        public async Task<HttpResponseMessage> GetChannel([FromQuery]GetChannelRequest request)
        {
            try
            {
                var response = await GetResponseMessageAsync(await VisualInfo.GetInstanceAsync(request.SiteId, request.ChannelId, 0, 0, request.PageIndex, 0));
                return response;
            }
            catch (Exception ex)
            {
                HttpContext.Response.Redirect(_pathManager.GetAdminUrl(ErrorController.Route) + "/?message=" + HttpUtility.UrlPathEncode(ex.Message));
            }

            return null;
        }

        [HttpGet, Route(ApiRoutePreview.RouteContent)]
        public async Task<HttpResponseMessage> GetContent([FromQuery] GetContentRequest request)
        {
            try
            {
                var response = await GetResponseMessageAsync(await VisualInfo.GetInstanceAsync(request.SiteId, request.ChannelId, request.ContentId, 0, request.PageIndex, request.PreviewId));
                return response;
            }
            catch (Exception ex)
            {
                HttpContext.Response.Redirect(_pathManager.GetAdminUrl(ErrorController.Route) + "/?message=" + HttpUtility.UrlPathEncode(ex.Message));
            }

            return null;
        }

        [HttpGet, Route(ApiRoutePreview.RouteFile)]
        public async Task<HttpResponseMessage> GetFile([FromQuery]GetFileRequest request)
        {
            try
            {
                var response = await GetResponseMessageAsync(await VisualInfo.GetInstanceAsync(request.SiteId, 0, 0, request.FileTemplateId, request.PageIndex, 0));
                return response;
            }
            catch (Exception ex)
            {
                HttpContext.Response.Redirect(_pathManager.GetAdminUrl(ErrorController.Route) + "/?message=" + HttpUtility.UrlPathEncode(ex.Message));
            }

            return null;
        }

        private HttpResponseMessage GetResponse(string html)
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

            var contentBuilder = new StringBuilder(await DataProvider.TemplateRepository.GetTemplateContentAsync(site, templateInfo));
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
                HttpContext.Response.Redirect(contentInfo.LinkUrl);
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
                var pageCount = StringUtils.GetCount(Constants.PagePlaceHolder, pageContentHtml) + 1; //一共需要的页数
                await Parser.ParseAsync(pageInfo, contextInfo, contentBuilder, visualInfo.FilePath, true);

                for (var currentPageIndex = 0; currentPageIndex < pageCount; currentPageIndex++)
                {
                    var index = pageContentHtml.IndexOf(Constants.PagePlaceHolder, StringComparison.Ordinal);
                    var length = index == -1 ? pageContentHtml.Length : index;

                    if (currentPageIndex == visualInfo.PageIndex)
                    {
                        var thePageInfo = await pageInfo.CloneAsync();
                        thePageInfo.IsLocal = true;

                        var pageHtml = pageContentHtml.Substring(0, length);

                        var pagedBuilder = new StringBuilder(contentBuilder.ToString().Replace(stlElementTranslated, pageHtml));

                        await StlParserManager.ReplacePageElementsInContentPageAsync(pagedBuilder, thePageInfo, stlLabelList,
                            visualInfo.ChannelId, visualInfo.ContentId, currentPageIndex, pageCount);

                        return GetResponse(pagedBuilder.ToString());
                    }

                    if (index != -1)
                    {
                        pageContentHtml = pageContentHtml.Substring(length + Constants.PagePlaceHolder.Length);
                    }
                }
            }

            if (StlParserUtility.IsStlElementExists(StlPageContents.ElementName, stlLabelList)) //如果标签中存在<stl:pageContents>
            {
                var stlElement = StlParserUtility.GetStlElement(StlPageContents.ElementName, stlLabelList);
                var stlElementTranslated = StlParserManager.StlEncrypt(stlElement);

                var pageContentsElementParser = await StlPageContents.GetAsync(stlElement, pageInfo, contextInfo);
                var (pageCount, totalNum) = pageContentsElementParser.GetPageCount();

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

                        return GetResponse(pagedBuilder.ToString());
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

                        return GetResponse(pagedBuilder.ToString());
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

                        return GetResponse(pagedBuilder.ToString());
                    }
                }
            }

            await Parser.ParseAsync(pageInfo, contextInfo, contentBuilder, visualInfo.FilePath, true);
            await StlParserManager.ReplacePageElementsInContentPageAsync(contentBuilder, pageInfo, stlLabelList,
                contentInfo.ChannelId, contentInfo.Id, 0, 1);
            return GetResponse(contentBuilder.ToString());
        }

        private async Task<HttpResponseMessage> GetChannelTemplateAsync(VisualInfo visualInfo, Site site, StringBuilder contentBuilder, PageInfo pageInfo, ContextInfo contextInfo)
        {
            var nodeInfo = await DataProvider.ChannelRepository.GetAsync(visualInfo.ChannelId);
            if (nodeInfo == null) return null;

            if (nodeInfo.ParentId > 0)
            {
                if (!string.IsNullOrEmpty(nodeInfo.LinkUrl))
                {
                    HttpContext.Response.Redirect(nodeInfo.LinkUrl);
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
                var pageCount = StringUtils.GetCount(Constants.PagePlaceHolder, contentAttributeHtml) + 1; //一共需要的页数
                if (pageCount > 1)
                {
                    await Parser.ParseAsync(pageInfo, contextInfo, contentBuilder, visualInfo.FilePath, true);

                    for (var currentPageIndex = 0; currentPageIndex < pageCount; currentPageIndex++)
                    {
                        var thePageInfo = await pageInfo.CloneAsync();
                        thePageInfo.IsLocal = true;

                        var index = contentAttributeHtml.IndexOf(Constants.PagePlaceHolder, StringComparison.Ordinal);
                        var length = index == -1 ? contentAttributeHtml.Length : index;

                        if (currentPageIndex == visualInfo.PageIndex)
                        {
                            var pagedContentAttributeHtml = contentAttributeHtml.Substring(0, length);
                            var pagedBuilder = new StringBuilder(contentBuilder.ToString()
                                .Replace(stlContentElement, pagedContentAttributeHtml));
                            await StlParserManager.ReplacePageElementsInChannelPageAsync(pagedBuilder, thePageInfo, stlLabelList,
                                thePageInfo.PageChannelId, currentPageIndex, pageCount, 0);

                            return GetResponse(pagedBuilder.ToString());
                        }

                        if (index != -1)
                        {
                            contentAttributeHtml =
                                contentAttributeHtml.Substring(length + Constants.PagePlaceHolder.Length);
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
                var (pageCount, totalNum) = pageContentsElementParser.GetPageCount();

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

                        return GetResponse(pagedBuilder.ToString());
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

                        return GetResponse(pagedBuilder.ToString());
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

                        return GetResponse(pagedBuilder.ToString());
                    }
                }
            }

            await Parser.ParseAsync(pageInfo, contextInfo, contentBuilder, visualInfo.FilePath, true);
            return GetResponse(contentBuilder.ToString());
        }

        private async Task<HttpResponseMessage> GetFileTemplateAsync(VisualInfo visualInfo, PageInfo pageInfo, ContextInfo contextInfo, StringBuilder contentBuilder)
        {
            await Parser.ParseAsync(pageInfo, contextInfo, contentBuilder, visualInfo.FilePath, true);
            return GetResponse(contentBuilder.ToString());
        }
    }
}
