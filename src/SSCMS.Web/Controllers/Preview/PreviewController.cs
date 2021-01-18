using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Configuration;
using SSCMS.Core.StlParser.StlElement;
using SSCMS.Core.StlParser.Utility;
using SSCMS.Core.Utils;
using SSCMS.Enums;
using SSCMS.Repositories;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Preview
{
    [OpenApiIgnore]
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

        public class GetRequest
        {
            public int PageIndex { get; set; }
        }

        public class GetChannelRequest
        {
            public int PageIndex { get; set; }
        }

        public class GetContentRequest
        {
            public bool IsPreview { get; set; }
            public int PageIndex { get; set; }
        }

        public class GetFileRequest
        {
            public int PageIndex { get; set; }
        }

        private FileResult GetResponse(string html)
        {
            var bytes = Encoding.UTF8.GetBytes(html);

            return File(bytes, "text/html");
        }

        private async Task<FileResult> GetResponseMessageAsync(VisualInfo visualInfo)
        {
            if (visualInfo.Site == null || visualInfo.Template == null) return null;

            var site = visualInfo.Site;
            var templateInfo = visualInfo.Template;

            await _parseManager.InitAsync(EditMode.Preview, site, visualInfo.ChannelId, visualInfo.ContentId, templateInfo);
            _parseManager.PageInfo.IsLocal = true;
            _parseManager.ContextInfo.ContextType = visualInfo.ContextType;

            var contentBuilder = new StringBuilder(await _pathManager.GetTemplateContentAsync(site, templateInfo));
            if (templateInfo.CreatedFileExtName == ".shtml")
            {
                //<!-- #include virtual="{Stl.SiteUrl}/include/head.html" -->

                var content = Regex.Replace(contentBuilder.ToString(), @"<!-- #include virtual=""([^""]+)"" -->", @"<stl:include file=""$1""></stl:include>");
                contentBuilder = new StringBuilder(content);
            }
            FileResult message = null;

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

        private async Task<FileResult> GetContentTemplateAsync(VisualInfo visualInfo, StringBuilder contentBuilder)
        {
            var content = await _parseManager.GetContentAsync();
            if (content == null) return null;

            if (!string.IsNullOrEmpty(content.LinkUrl))
            {
                HttpContext.Response.Redirect(content.LinkUrl);
                return null;
            }

            var stlLabelList = ParseUtils.GetStlLabels(contentBuilder.ToString());

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

            if (ParseUtils.IsStlElementExists(StlPageContents.ElementName, stlLabelList)) //如果标签中存在<stl:pageContents>
            {
                var stlElement = ParseUtils.GetStlElement(StlPageContents.ElementName, stlLabelList);
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
            else if (ParseUtils.IsStlElementExists(StlPageChannels.ElementName, stlLabelList)) //如果标签中存在<stl:pageChannels>
            {
                var stlElement = ParseUtils.GetStlElement(StlPageChannels.ElementName, stlLabelList);
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
            else if (ParseUtils.IsStlElementExists(StlPageSqlContents.ElementName, stlLabelList)
            ) //如果标签中存在<stl:pageSqlContents>
            {
                var stlElement = ParseUtils.GetStlElement(StlPageSqlContents.ElementName, stlLabelList);
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

        private async Task<FileResult> GetChannelTemplateAsync(VisualInfo visualInfo, StringBuilder contentBuilder)
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

            var stlLabelList = ParseUtils.GetStlLabels(contentBuilder.ToString());

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

            if (ParseUtils.IsStlElementExists(StlPageContents.ElementName, stlLabelList)) //如果标签中存在<stl:pageContents>
            {
                var stlElement = ParseUtils.GetStlElement(StlPageContents.ElementName, stlLabelList);
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
            else if (ParseUtils.IsStlElementExists(StlPageChannels.ElementName, stlLabelList)) //如果标签中存在<stl:pageChannels>
            {
                var stlElement = ParseUtils.GetStlElement(StlPageChannels.ElementName, stlLabelList);
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
            //如果标签中存在<stl:pageSqlContents>
            else if (ParseUtils.IsStlElementExists(StlPageSqlContents.ElementName, stlLabelList))
            {
                var stlElement = ParseUtils.GetStlElement(StlPageSqlContents.ElementName, stlLabelList);
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

        private async Task<FileResult> GetFileTemplateAsync(VisualInfo visualInfo, StringBuilder contentBuilder)
        {
            await _parseManager.ParseAsync(contentBuilder, visualInfo.FilePath, true);
            return GetResponse(contentBuilder.ToString());
        }
    }
}
