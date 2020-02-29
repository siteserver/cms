using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Mvc;
using SS.CMS.Abstractions;
using SS.CMS.Core;
using SS.CMS.StlParser.StlElement;
using SS.CMS.StlParser.Utility;
using SS.CMS.Web.Controllers.Admin;

namespace SS.CMS.Web.Controllers.Preview
{
    public partial class PreviewController : ControllerBase
    {
        private readonly IPathManager _pathManager;
        private readonly IParseManager _parseManager;
        private readonly IDatabaseManager _databaseManager;
        private readonly IChannelRepository _channelRepository;

        public PreviewController(IPathManager pathManager, IParseManager parseManager, IDatabaseManager databaseManager, IChannelRepository channelRepository)
        {
            _pathManager = pathManager;
            _parseManager = parseManager;
            _databaseManager = databaseManager;
            _channelRepository = channelRepository;
        }

        [HttpGet, Route(Constants.RoutePreview)]
        public async Task<HttpResponseMessage> Get([FromQuery]GetRequest request)
        {
            try
            {
                return await GetResponseMessageAsync(await VisualInfo.GetInstanceAsync(_pathManager, _databaseManager, request.SiteId, 0, 0, 0, request.PageIndex, 0));
            }
            catch (Exception ex)
            {
                HttpContext.Response.Redirect(_pathManager.GetAdminUrl(ErrorController.Route) + "/?message=" + HttpUtility.UrlPathEncode(ex.Message));
            }

            return null;
        }

        [HttpGet, Route(Constants.RoutePreviewChannel)]
        public async Task<HttpResponseMessage> GetChannel([FromQuery]GetChannelRequest request)
        {
            try
            {
                var response = await GetResponseMessageAsync(await VisualInfo.GetInstanceAsync(_pathManager, _databaseManager, request.SiteId, request.ChannelId, 0, 0, request.PageIndex, 0));
                return response;
            }
            catch (Exception ex)
            {
                HttpContext.Response.Redirect(_pathManager.GetAdminUrl(ErrorController.Route) + "/?message=" + HttpUtility.UrlPathEncode(ex.Message));
            }

            return null;
        }

        [HttpGet, Route(Constants.RoutePreviewContent)]
        public async Task<HttpResponseMessage> GetContent([FromQuery] GetContentRequest request)
        {
            try
            {
                var response = await GetResponseMessageAsync(await VisualInfo.GetInstanceAsync(_pathManager, _databaseManager, request.SiteId, request.ChannelId, request.ContentId, 0, request.PageIndex, request.PreviewId));
                return response;
            }
            catch (Exception ex)
            {
                HttpContext.Response.Redirect(_pathManager.GetAdminUrl(ErrorController.Route) + "/?message=" + HttpUtility.UrlPathEncode(ex.Message));
            }

            return null;
        }

        [HttpGet, Route(Constants.RoutePreviewFile)]
        public async Task<HttpResponseMessage> GetFile([FromQuery]GetFileRequest request)
        {
            try
            {
                var response = await GetResponseMessageAsync(await VisualInfo.GetInstanceAsync(_pathManager, _databaseManager, request.SiteId, 0, 0, request.FileTemplateId, request.PageIndex, 0));
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

            await _parseManager.InitAsync(site, visualInfo.ChannelId, visualInfo.ContentId, templateInfo);
            _parseManager.PageInfo.IsLocal = true;
            _parseManager.ContextInfo.ContextType = visualInfo.ContextType;

            var contentBuilder = new StringBuilder(await _pathManager.GetTemplateContentAsync(site, templateInfo));
            if (templateInfo.CreatedFileExtName == ".shtml")
            {
                //<!-- #include virtual="{Stl.SiteUrl}/include/head.html" -->

                var content = Regex.Replace(contentBuilder.ToString(), @"<!-- #include virtual=""([^""]+)"" -->", @"<stl:include file=""$1""></stl:include>");
                contentBuilder = new StringBuilder(content);
            }
            HttpResponseMessage message = null;

            if (templateInfo.TemplateType == TemplateType.FileTemplate)           //单页
            {
                message = await GetFileTemplateAsync(visualInfo, contentBuilder);
            }
            else if (templateInfo.TemplateType == TemplateType.IndexPageTemplate || templateInfo.TemplateType == TemplateType.ChannelTemplate)        //栏目页面
            {
                message = await GetChannelTemplateAsync(visualInfo, contentBuilder);
            }
            else if (templateInfo.TemplateType == TemplateType.ContentTemplate)        //内容页面
            {
                message = await GetContentTemplateAsync(visualInfo, contentBuilder);
            }

            return message;
        }

        private async Task<HttpResponseMessage> GetContentTemplateAsync(VisualInfo visualInfo, StringBuilder contentBuilder)
        {
            var content = await _parseManager.GetContentAsync();
            if (content == null) return null;

            if (!string.IsNullOrEmpty(content.LinkUrl))
            {
                HttpContext.Response.Redirect(content.LinkUrl);
                return null;
            }

            var stlLabelList = StlParserUtility.GetStlLabelList(contentBuilder.ToString());

            if (StlParserUtility.IsStlContentElementWithTypePageContent(stlLabelList)) //内容存在
            {
                var stlElement = StlParserUtility.GetStlContentElementWithTypePageContent(stlLabelList);
                var stlElementTranslated = _parseManager.StlEncrypt(stlElement);
                contentBuilder.Replace(stlElement, stlElementTranslated);

                var innerBuilder = new StringBuilder(stlElement);
                await _parseManager.ParseInnerContentAsync(innerBuilder);
                var pageContentHtml = innerBuilder.ToString();
                var pageCount = StringUtils.GetCount(Constants.PagePlaceHolder, pageContentHtml) + 1; //一共需要的页数
                await _parseManager.ParseAsync(contentBuilder, visualInfo.FilePath, true);

                for (var currentPageIndex = 0; currentPageIndex < pageCount; currentPageIndex++)
                {
                    var index = pageContentHtml.IndexOf(Constants.PagePlaceHolder, StringComparison.Ordinal);
                    var length = index == -1 ? pageContentHtml.Length : index;

                    if (currentPageIndex == visualInfo.PageIndex)
                    {
                        var pageHtml = pageContentHtml.Substring(0, length);

                        var pagedBuilder = new StringBuilder(contentBuilder.ToString().Replace(stlElementTranslated, pageHtml));

                        await _parseManager.ReplacePageElementsInContentPageAsync(pagedBuilder, stlLabelList,
                            currentPageIndex, pageCount);

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
                var stlElementTranslated = _parseManager.StlEncrypt(stlElement);

                var pageContentsElementParser = await StlPageContents.GetAsync(stlElement, _parseManager);
                var (pageCount, totalNum) = pageContentsElementParser.GetPageCount();

                await _parseManager.ParseAsync(contentBuilder, visualInfo.FilePath, true);

                for (var currentPageIndex = 0; currentPageIndex < pageCount; currentPageIndex++)
                {
                    if (currentPageIndex == visualInfo.PageIndex)
                    {
                        var pageHtml = await pageContentsElementParser.ParseAsync(totalNum, currentPageIndex, pageCount, false);
                        var pagedBuilder = new StringBuilder(contentBuilder.ToString().Replace(stlElementTranslated, pageHtml));

                        await _parseManager.ReplacePageElementsInContentPageAsync(pagedBuilder, stlLabelList, currentPageIndex, pageCount);

                        return GetResponse(pagedBuilder.ToString());
                    }
                }
            }
            else if (StlParserUtility.IsStlElementExists(StlPageChannels.ElementName, stlLabelList)) //如果标签中存在<stl:pageChannels>
            {
                var stlElement = StlParserUtility.GetStlElement(StlPageChannels.ElementName, stlLabelList);
                var stlElementTranslated = _parseManager.StlEncrypt(stlElement);

                var pageChannelsElementParser = await StlPageChannels.GetAsync(stlElement, _parseManager);
                var pageCount = pageChannelsElementParser.GetPageCount(out _);

                await _parseManager.ParseAsync(contentBuilder, visualInfo.FilePath, true);

                for (var currentPageIndex = 0; currentPageIndex < pageCount; currentPageIndex++)
                {
                    if (currentPageIndex == visualInfo.PageIndex)
                    {
                        var pageHtml = await pageChannelsElementParser.ParseAsync(currentPageIndex, pageCount);
                        var pagedBuilder =
                            new StringBuilder(contentBuilder.ToString().Replace(stlElementTranslated, pageHtml));

                        await _parseManager.ReplacePageElementsInContentPageAsync(pagedBuilder, stlLabelList, currentPageIndex, pageCount);

                        return GetResponse(pagedBuilder.ToString());
                    }
                }
            }
            else if (StlParserUtility.IsStlElementExists(StlPageSqlContents.ElementName, stlLabelList)
            ) //如果标签中存在<stl:pageSqlContents>
            {
                var stlElement = StlParserUtility.GetStlElement(StlPageSqlContents.ElementName, stlLabelList);
                var stlElementTranslated = _parseManager.StlEncrypt(stlElement);

                var pageSqlContentsElementParser = await StlPageSqlContents.GetAsync(stlElement, _parseManager);
                var pageCount = pageSqlContentsElementParser.GetPageCount(out var totalNum);

                await _parseManager.ParseAsync(contentBuilder, visualInfo.FilePath, true);

                for (var currentPageIndex = 0; currentPageIndex < pageCount; currentPageIndex++)
                {
                    if (currentPageIndex == visualInfo.PageIndex)
                    {
                        var pageHtml = await pageSqlContentsElementParser.ParseAsync(totalNum, currentPageIndex, pageCount, false);
                        var pagedBuilder = new StringBuilder(contentBuilder.ToString().Replace(stlElementTranslated, pageHtml));

                        await _parseManager.ReplacePageElementsInContentPageAsync(pagedBuilder, stlLabelList, currentPageIndex, pageCount);

                        return GetResponse(pagedBuilder.ToString());
                    }
                }
            }

            await _parseManager.ParseAsync(contentBuilder, visualInfo.FilePath, true);
            await _parseManager.ReplacePageElementsInContentPageAsync(contentBuilder, stlLabelList, 0, 1);
            return GetResponse(contentBuilder.ToString());
        }

        private async Task<HttpResponseMessage> GetChannelTemplateAsync(VisualInfo visualInfo, StringBuilder contentBuilder)
        {
            var nodeInfo = await _channelRepository.GetAsync(visualInfo.ChannelId);
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
                await _parseManager.ParseInnerContentAsync(innerBuilder);
                var contentAttributeHtml = innerBuilder.ToString();
                var pageCount = StringUtils.GetCount(Constants.PagePlaceHolder, contentAttributeHtml) + 1; //一共需要的页数
                if (pageCount > 1)
                {
                    await _parseManager.ParseAsync(contentBuilder, visualInfo.FilePath, true);

                    for (var currentPageIndex = 0; currentPageIndex < pageCount; currentPageIndex++)
                    {
                        var index = contentAttributeHtml.IndexOf(Constants.PagePlaceHolder, StringComparison.Ordinal);
                        var length = index == -1 ? contentAttributeHtml.Length : index;

                        if (currentPageIndex == visualInfo.PageIndex)
                        {
                            var pagedContentAttributeHtml = contentAttributeHtml.Substring(0, length);
                            var pagedBuilder = new StringBuilder(contentBuilder.ToString()
                                .Replace(stlContentElement, pagedContentAttributeHtml));
                            await _parseManager.ReplacePageElementsInChannelPageAsync(pagedBuilder, stlLabelList, currentPageIndex, pageCount, 0);

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
                var stlElementTranslated = _parseManager.StlEncrypt(stlElement);

                var pageContentsElementParser = await StlPageContents.GetAsync(stlElement, _parseManager);
                var (pageCount, totalNum) = pageContentsElementParser.GetPageCount();

                await _parseManager.ParseAsync(contentBuilder, visualInfo.FilePath, true);

                for (var currentPageIndex = 0; currentPageIndex < pageCount; currentPageIndex++)
                {
                    if (currentPageIndex == visualInfo.PageIndex)
                    {
                        var pageHtml = await pageContentsElementParser.ParseAsync(totalNum, currentPageIndex, pageCount, false);
                        var pagedBuilder = new StringBuilder(contentBuilder.ToString().Replace(stlElementTranslated, pageHtml));

                        await _parseManager.ReplacePageElementsInChannelPageAsync(pagedBuilder, stlLabelList, currentPageIndex, pageCount, totalNum);

                        return GetResponse(pagedBuilder.ToString());
                    }
                }
            }
            else if (StlParserUtility.IsStlElementExists(StlPageChannels.ElementName, stlLabelList)) //如果标签中存在<stl:pageChannels>
            {
                var stlElement = StlParserUtility.GetStlElement(StlPageChannels.ElementName, stlLabelList);
                var stlElementTranslated = _parseManager.StlEncrypt(stlElement);

                var pageChannelsElementParser = await StlPageChannels.GetAsync(stlElement, _parseManager);
                var pageCount = pageChannelsElementParser.GetPageCount(out var totalNum);

                await _parseManager.ParseAsync(contentBuilder, visualInfo.FilePath, true);

                for (var currentPageIndex = 0; currentPageIndex < pageCount; currentPageIndex++)
                {
                    if (currentPageIndex == visualInfo.PageIndex)
                    {
                        var pageHtml = await pageChannelsElementParser.ParseAsync(currentPageIndex, pageCount);
                        var pagedBuilder = new StringBuilder(contentBuilder.ToString().Replace(stlElementTranslated, pageHtml));

                        await _parseManager.ReplacePageElementsInChannelPageAsync(pagedBuilder, stlLabelList, currentPageIndex, pageCount, totalNum);

                        return GetResponse(pagedBuilder.ToString());
                    }
                }
            }
            else if (StlParserUtility.IsStlElementExists(StlPageSqlContents.ElementName, stlLabelList)
            ) //如果标签中存在<stl:pageSqlContents>
            {
                var stlElement = StlParserUtility.GetStlElement(StlPageSqlContents.ElementName, stlLabelList);
                var stlElementTranslated = _parseManager.StlEncrypt(stlElement);

                var pageSqlContentsElementParser = await StlPageSqlContents.GetAsync(stlElement, _parseManager);
                var pageCount = pageSqlContentsElementParser.GetPageCount(out var totalNum);

                await _parseManager.ParseAsync(contentBuilder, visualInfo.FilePath, true);

                for (var currentPageIndex = 0; currentPageIndex < pageCount; currentPageIndex++)
                {
                    if (currentPageIndex == visualInfo.PageIndex)
                    {
                        var pageHtml = await pageSqlContentsElementParser.ParseAsync(totalNum, currentPageIndex, pageCount, false);
                        var pagedBuilder = new StringBuilder(contentBuilder.ToString().Replace(stlElementTranslated, pageHtml));

                        await _parseManager.ReplacePageElementsInChannelPageAsync(pagedBuilder, stlLabelList, currentPageIndex, pageCount, totalNum);

                        return GetResponse(pagedBuilder.ToString());
                    }
                }
            }

            await _parseManager.ParseAsync(contentBuilder, visualInfo.FilePath, true);
            return GetResponse(contentBuilder.ToString());
        }

        private async Task<HttpResponseMessage> GetFileTemplateAsync(VisualInfo visualInfo, StringBuilder contentBuilder)
        {
            await _parseManager.ParseAsync(contentBuilder, visualInfo.FilePath, true);
            return GetResponse(contentBuilder.ToString());
        }
    }
}
