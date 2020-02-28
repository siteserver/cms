using System;
using System.Text;
using System.Threading.Tasks;
using Datory;
using SS.CMS.Abstractions;
using SS.CMS.Abstractions.Parse;
using SS.CMS.Api.Stl;
using SS.CMS.Core;
using SS.CMS.Plugins;

namespace SS.CMS.Services
{
    public partial class ParseManager : IParseManager
    {
        public ISettingsManager SettingsManager { get; }
        public IPathManager PathManager { get; }
        public IDatabaseManager DatabaseManager { get; }

        public ParseManager(ISettingsManager settingsManager, IPathManager pathManager, IDatabaseManager databaseManager)
        {
            SettingsManager = settingsManager;
            PathManager = pathManager;
            DatabaseManager = databaseManager;
        }

        public ParsePage PageInfo { get; private set; }
        public ParseContext ContextInfo { get; set; }

        public async Task ParseAsync(ParsePage pageInfo, ParseContext contextInfo, StringBuilder contentBuilder,
            string filePath, bool isDynamic)
        {
            PageInfo = pageInfo;
            ContextInfo = contextInfo;

            foreach (var service in await PluginManager.GetServicesAsync())
            {
                try
                {
                    service.OnBeforeStlParse(new ParseEventArgs
                    (
                        pageInfo.SiteId,
                        pageInfo.PageChannelId,
                        pageInfo.PageContentId,
                        await GetContentAsync(),
                        pageInfo.Template.TemplateType,
                        pageInfo.Template.Id,
                        filePath,
                        pageInfo.HeadCodes,
                        pageInfo.BodyCodes,
                        pageInfo.FootCodes,
                        contentBuilder
                    ));
                }
                catch (Exception ex)
                {
                    await AddStlErrorLogAsync(pageInfo, service.PluginId, nameof(service.OnBeforeStlParse),
                        ex);
                }
            }

            if (contentBuilder.Length > 0)
            {
                await ParseTemplateContentAsync(contentBuilder, pageInfo, contextInfo);
            }

            foreach (var service in await PluginManager.GetServicesAsync())
            {
                try
                {
                    service.OnAfterStlParse(new ParseEventArgs(pageInfo.SiteId, pageInfo.PageChannelId,
                        pageInfo.PageContentId, await GetContentAsync(),
                        pageInfo.Template.TemplateType, pageInfo.Template.Id, filePath, pageInfo.HeadCodes,
                        pageInfo.BodyCodes, pageInfo.FootCodes, contentBuilder));
                }
                catch (Exception ex)
                {
                    await AddStlErrorLogAsync(pageInfo, service.PluginId, nameof(service.OnAfterStlParse), ex);
                }
            }

            if (FileUtils.IsHtml(PathUtils.GetExtension(filePath)))
            {
                if (isDynamic)
                {
                    var pageUrl = PageUtils.AddProtocolToUrl(
                        PageUtils.ParseNavigationUrl(
                            $"~/{PathUtils.GetPathDifference(GlobalSettings.WebRootPath, filePath)}"));
                    string templateString = $@"
<base href=""{pageUrl}"" />";
                    StringUtils.InsertAfter(new[] {"<head>", "<HEAD>"}, contentBuilder, templateString);
                }

                if (pageInfo.Site.IsCreateBrowserNoCache)
                {
                    const string templateString = @"
<META HTTP-EQUIV=""Pragma"" CONTENT=""no-cache"">
<META HTTP-EQUIV=""Expires"" CONTENT=""-1"">";
                    StringUtils.InsertAfter(new[] {"<head>", "<HEAD>"}, contentBuilder, templateString);
                }

                if (pageInfo.Site.IsCreateIe8Compatible)
                {
                    const string templateString = @"
<META HTTP-EQUIV=""x-ua-compatible"" CONTENT=""ie=7"" />";
                    StringUtils.InsertAfter(new[] {"<head>", "<HEAD>"}, contentBuilder, templateString);
                }

                if (pageInfo.Site.IsCreateJsIgnoreError)
                {
                    const string templateString = @"
<script type=""text/javascript"">window.onerror=function(){return true;}</script>";
                    StringUtils.InsertAfter(new[] {"<head>", "<HEAD>"}, contentBuilder, templateString);
                }

                var isShowPageInfo = pageInfo.Site.IsCreateShowPageInfo;

                if (!pageInfo.IsLocal)
                {
                    if (pageInfo.Site.IsCreateDoubleClick)
                    {
                        var fileTemplateId = 0;
                        if (pageInfo.Template.TemplateType == TemplateType.FileTemplate)
                        {
                            fileTemplateId = pageInfo.Template.Id;
                        }

                        var apiUrl = pageInfo.ApiUrl;
                        var ajaxUrl = ApiRouteActionsTrigger.GetUrl(apiUrl, pageInfo.SiteId, contextInfo.ChannelId,
                            contextInfo.ContentId, fileTemplateId, true);
                        if (!pageInfo.FootCodes.ContainsKey("CreateDoubleClick"))
                        {
                            pageInfo.FootCodes.Add("CreateDoubleClick", $@"
<script type=""text/javascript"" language=""javascript"">document.ondblclick=function(x){{location.href = '{ajaxUrl}&returnUrl=' + encodeURIComponent(location.search);}}</script>");
                        }
                    }
                }
                else
                {
                    isShowPageInfo = true;
                }

                if (isShowPageInfo)
                {
                    contentBuilder.Append($@"
<!-- {pageInfo.Template.RelatedFileName}({pageInfo.Template.TemplateType.GetDisplayName()}) -->");
                }

                var headCodesHtml = pageInfo.HeadCodesHtml;
                if (!string.IsNullOrEmpty(headCodesHtml))
                {
                    if (contentBuilder.ToString().IndexOf("</head>", StringComparison.Ordinal) != -1 ||
                        contentBuilder.ToString().IndexOf("</HEAD>", StringComparison.Ordinal) != -1)
                    {
                        StringUtils.InsertBefore(new[] {"</head>", "</HEAD>"}, contentBuilder, headCodesHtml);
                    }
                    else
                    {
                        contentBuilder.Insert(0, headCodesHtml);
                    }
                }

                var bodyCodesHtml = pageInfo.BodyCodesHtml;
                if (!string.IsNullOrEmpty(bodyCodesHtml))
                {
                    if (contentBuilder.ToString().IndexOf("<body", StringComparison.Ordinal) != -1 ||
                        contentBuilder.ToString().IndexOf("<BODY", StringComparison.Ordinal) != -1)
                    {
                        var index = contentBuilder.ToString().IndexOf("<body", StringComparison.Ordinal);
                        if (index == -1)
                        {
                            index = contentBuilder.ToString().IndexOf("<BODY", StringComparison.Ordinal);
                        }

                        index = contentBuilder.ToString().IndexOf(">", index, StringComparison.Ordinal);
                        contentBuilder.Insert(index + 1,
                            Constants.ReturnAndNewline + bodyCodesHtml + Constants.ReturnAndNewline);
                    }
                    else
                    {
                        contentBuilder.Insert(0, bodyCodesHtml);
                    }
                }

                var footCodesHtml = pageInfo.FootCodesHtml;
                if (!string.IsNullOrEmpty(footCodesHtml))
                {
                    contentBuilder.Append(footCodesHtml + Constants.ReturnAndNewline);
                }
            }
        }

        public async Task<string> AddStlErrorLogAsync(ParsePage pageInfo, string elementName, string stlContent, Exception ex)
        {
            var summary = string.Empty;
            if (pageInfo != null)
            {
                summary = $@"站点名称：{pageInfo.Site.SiteName}，
模板类型：{pageInfo.Template.TemplateType.GetDisplayName()}，
模板名称：{pageInfo.Template.TemplateName}
<br />";
            }

            summary += $@"STL标签：{WebUtils.HtmlEncode(stlContent)}";
            await DatabaseManager.ErrorLogRepository.AddErrorLogAsync(new ErrorLog
            {
                Id = 0,
                Category = LogUtils.CategoryStl,
                PluginId = string.Empty,
                Message = ex.Message,
                StackTrace = ex.StackTrace,
                Summary = summary,
                AddDate = DateTime.Now
            });

            return $@"
<!--
{elementName}
error: {ex.Message}
stl: {stlContent}
-->";
        }

        public async Task<Channel> GetChannelAsync()
        {
            var channel = ContextInfo.Channel;
            if (channel != null) return channel;
            if (ContextInfo.ChannelId <= 0) return null;
            channel = await DatabaseManager.ChannelRepository.GetAsync(ContextInfo.ChannelId);
            ContextInfo.SetChannel(channel);
            return channel;
        }

        public async Task<Content> GetContentAsync()
        {
            var content = ContextInfo.Content;
            if (content != null) return content;
            if (ContextInfo.ContentId <= 0) return null;
            content = await DatabaseManager.ContentRepository.GetAsync(ContextInfo.Site, ContextInfo.ChannelId, ContextInfo.ContentId);
            ContextInfo.SetContent(content);
            return content;
        }
    }
}