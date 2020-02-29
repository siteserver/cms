using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Datory;
using SS.CMS.Abstractions;
using SS.CMS.Abstractions.Parse;
using SS.CMS.Core;

namespace SS.CMS.Services
{
    public partial class ParseManager : IParseManager
    {
        public ISettingsManager SettingsManager { get; }
        public IPathManager PathManager { get; }
        public IDatabaseManager DatabaseManager { get; }
        public IPluginManager PluginManager { get; }

        public ParseManager(ISettingsManager settingsManager, IPathManager pathManager, IDatabaseManager databaseManager, IPluginManager pluginManager)
        {
            SettingsManager = settingsManager;
            PathManager = pathManager;
            DatabaseManager = databaseManager;
            PluginManager = pluginManager;
        }

        public ParsePage PageInfo { get; set; }
        public ParseContext ContextInfo { get; set; }

        public async Task InitAsync(Site site, int pageChannelId, int pageContentId, Template template)
        {
            var config = await DatabaseManager.ConfigRepository.GetAsync();
            PageInfo = new ParsePage(PathManager, config, pageChannelId, pageContentId, site, template, new Dictionary<string, object>());
            ContextInfo = new ParseContext(PageInfo);
        }

        public async Task ParseAsync(StringBuilder contentBuilder, string filePath, bool isDynamic)
        {
            foreach (var service in await PluginManager.GetServicesAsync())
            {
                try
                {
                    service.OnBeforeStlParse(new ParseEventArgs
                    (
                        PageInfo.SiteId,
                        PageInfo.PageChannelId,
                        PageInfo.PageContentId,
                        await GetContentAsync(),
                        PageInfo.Template.TemplateType,
                        PageInfo.Template.Id,
                        filePath,
                        PageInfo.HeadCodes,
                        PageInfo.BodyCodes,
                        PageInfo.FootCodes,
                        contentBuilder
                    ));
                }
                catch (Exception ex)
                {
                    await AddStlErrorLogAsync(service.PluginId, nameof(service.OnBeforeStlParse),
                        ex);
                }
            }

            if (contentBuilder.Length > 0)
            {
                await ParseTemplateContentAsync(contentBuilder);
            }

            foreach (var service in await PluginManager.GetServicesAsync())
            {
                try
                {
                    service.OnAfterStlParse(new ParseEventArgs(PageInfo.SiteId, PageInfo.PageChannelId,
                        PageInfo.PageContentId, await GetContentAsync(),
                        PageInfo.Template.TemplateType, PageInfo.Template.Id, filePath, PageInfo.HeadCodes,
                        PageInfo.BodyCodes, PageInfo.FootCodes, contentBuilder));
                }
                catch (Exception ex)
                {
                    await AddStlErrorLogAsync(service.PluginId, nameof(service.OnAfterStlParse), ex);
                }
            }

            if (FileUtils.IsHtml(PathUtils.GetExtension(filePath)))
            {
                if (isDynamic)
                {
                    var pageUrl = PageUtils.AddProtocolToUrl(
                        PathManager.ParseNavigationUrl(
                            $"~/{PathUtils.GetPathDifference(SettingsManager.WebRootPath, filePath)}"));
                    string templateString = $@"
<base href=""{pageUrl}"" />";
                    StringUtils.InsertAfter(new[] {"<head>", "<HEAD>"}, contentBuilder, templateString);
                }

                if (PageInfo.Site.IsCreateBrowserNoCache)
                {
                    const string templateString = @"
<META HTTP-EQUIV=""Pragma"" CONTENT=""no-cache"">
<META HTTP-EQUIV=""Expires"" CONTENT=""-1"">";
                    StringUtils.InsertAfter(new[] {"<head>", "<HEAD>"}, contentBuilder, templateString);
                }

                if (PageInfo.Site.IsCreateIe8Compatible)
                {
                    const string templateString = @"
<META HTTP-EQUIV=""x-ua-compatible"" CONTENT=""ie=7"" />";
                    StringUtils.InsertAfter(new[] {"<head>", "<HEAD>"}, contentBuilder, templateString);
                }

                if (PageInfo.Site.IsCreateJsIgnoreError)
                {
                    const string templateString = @"
<script type=""text/javascript"">window.onerror=function(){return true;}</script>";
                    StringUtils.InsertAfter(new[] {"<head>", "<HEAD>"}, contentBuilder, templateString);
                }

                var isShowPageInfo = PageInfo.Site.IsCreateShowPageInfo;

                if (!PageInfo.IsLocal)
                {
                    if (PageInfo.Site.IsCreateDoubleClick)
                    {
                        var fileTemplateId = 0;
                        if (PageInfo.Template.TemplateType == TemplateType.FileTemplate)
                        {
                            fileTemplateId = PageInfo.Template.Id;
                        }

                        var apiUrl = PageInfo.ApiUrl;
                        var ajaxUrl = PathManager.GetTriggerApiUrl(apiUrl, PageInfo.SiteId, ContextInfo.ChannelId,
                            ContextInfo.ContentId, fileTemplateId, true);
                        if (!PageInfo.FootCodes.ContainsKey("CreateDoubleClick"))
                        {
                            PageInfo.FootCodes.Add("CreateDoubleClick", $@"
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
<!-- {PageInfo.Template.RelatedFileName}({PageInfo.Template.TemplateType.GetDisplayName()}) -->");
                }

                var headCodesHtml = PageInfo.HeadCodesHtml;
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

                var bodyCodesHtml = PageInfo.BodyCodesHtml;
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

                var footCodesHtml = PageInfo.FootCodesHtml;
                if (!string.IsNullOrEmpty(footCodesHtml))
                {
                    contentBuilder.Append(footCodesHtml + Constants.ReturnAndNewline);
                }
            }
        }

        public async Task<string> AddStlErrorLogAsync(string elementName, string stlContent, Exception ex)
        {
            var summary = $@"站点名称：{PageInfo.Site.SiteName}，
模板类型：{PageInfo.Template.TemplateType.GetDisplayName()}，
模板名称：{PageInfo.Template.TemplateName}
<br />";

            summary += $@"STL标签：{StringUtils.HtmlEncode(stlContent)}";
            await DatabaseManager.ErrorLogRepository.AddErrorLogAsync(new ErrorLog
            {
                Id = 0,
                Category = LogUtils.CategoryStl,
                PluginId = string.Empty,
                Message = ex.Message,
                StackTrace = ex.StackTrace,
                Summary = summary,
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