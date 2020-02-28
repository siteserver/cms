using System;
using System.Text;
using System.Threading.Tasks;
using Datory;
using SS.CMS.Abstractions;
using SS.CMS;
using SS.CMS.Abstractions.Parse;
using SS.CMS.StlParser.Model;
using SS.CMS.StlParser.Utility;
using SS.CMS.Api.Stl;
using SS.CMS.Core;
using SS.CMS.Plugins;

namespace SS.CMS.StlParser
{
	public static class Parser
	{
        public static async Task ParseAsync(ParsePage pageInfo, ParseContext contextInfo, StringBuilder contentBuilder, string filePath, bool isDynamic)
        {
            foreach (var service in await PluginManager.GetServicesAsync())
            {
                try
                {
                    service.OnBeforeStlParse(new ParseEventArgs
                    (
                        pageInfo.SiteId,
                        pageInfo.PageChannelId,
                        pageInfo.PageContentId,
                        await contextInfo.GetContentAsync(),
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
                    await LogUtils.AddStlErrorLogAsync(pageInfo, service.PluginId, nameof(service.OnBeforeStlParse), ex);
                }
            }

            if (contentBuilder.Length > 0)
            {
                await StlParserManager.ParseTemplateContentAsync(contentBuilder, pageInfo, contextInfo);
            }

            foreach (var service in await PluginManager.GetServicesAsync())
            {
                try
                {
                    service.OnAfterStlParse(new ParseEventArgs(pageInfo.SiteId, pageInfo.PageChannelId, pageInfo.PageContentId, await contextInfo.GetContentAsync(), pageInfo.Template.TemplateType, pageInfo.Template.Id, filePath, pageInfo.HeadCodes, pageInfo.BodyCodes, pageInfo.FootCodes, contentBuilder));
                }
                catch (Exception ex)
                {
                    await LogUtils.AddStlErrorLogAsync(pageInfo, service.PluginId, nameof(service.OnAfterStlParse), ex);
                }
            }

            if (FileUtils.IsHtml(PathUtils.GetExtension(filePath)))
            {
                if (isDynamic)
                {
                    var pageUrl = PageUtils.AddProtocolToUrl(PageUtils.ParseNavigationUrl($"~/{PathUtils.GetPathDifference(GlobalSettings.WebRootPath, filePath)}"));
                    string templateString = $@"
<base href=""{pageUrl}"" />";
                    StringUtils.InsertAfter(new[] { "<head>", "<HEAD>" }, contentBuilder, templateString);
                }

                if (pageInfo.Site.IsCreateBrowserNoCache)
                {
                    const string templateString = @"
<META HTTP-EQUIV=""Pragma"" CONTENT=""no-cache"">
<META HTTP-EQUIV=""Expires"" CONTENT=""-1"">";
                    StringUtils.InsertAfter(new[] { "<head>", "<HEAD>" }, contentBuilder, templateString);
                }

                if (pageInfo.Site.IsCreateIe8Compatible)
                {
                    const string templateString = @"
<META HTTP-EQUIV=""x-ua-compatible"" CONTENT=""ie=7"" />";
                    StringUtils.InsertAfter(new[] { "<head>", "<HEAD>" }, contentBuilder, templateString);
                }

                if (pageInfo.Site.IsCreateJsIgnoreError)
                {
                    const string templateString = @"
<script type=""text/javascript"">window.onerror=function(){return true;}</script>";
                    StringUtils.InsertAfter(new[] { "<head>", "<HEAD>" }, contentBuilder, templateString);
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
                    if (contentBuilder.ToString().IndexOf("</head>", StringComparison.Ordinal) != -1 || contentBuilder.ToString().IndexOf("</HEAD>", StringComparison.Ordinal) != -1)
                    {
                        StringUtils.InsertBefore(new[] { "</head>", "</HEAD>" }, contentBuilder, headCodesHtml);
                    }
                    else
                    {
                        contentBuilder.Insert(0, headCodesHtml);
                    }
                }

                var bodyCodesHtml = pageInfo.BodyCodesHtml;
                if (!string.IsNullOrEmpty(bodyCodesHtml))
                {
                    if (contentBuilder.ToString().IndexOf("<body", StringComparison.Ordinal) != -1 || contentBuilder.ToString().IndexOf("<BODY", StringComparison.Ordinal) != -1)
                    {
                        var index = contentBuilder.ToString().IndexOf("<body", StringComparison.Ordinal);
                        if (index == -1)
                        {
                            index = contentBuilder.ToString().IndexOf("<BODY", StringComparison.Ordinal);
                        }
                        index = contentBuilder.ToString().IndexOf(">", index, StringComparison.Ordinal);
                        contentBuilder.Insert(index + 1, Constants.ReturnAndNewline + bodyCodesHtml + Constants.ReturnAndNewline);
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
    }
}
