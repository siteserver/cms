using System;
using System.Text;
using SiteServer.CMS.Api.Sys.Stl;
using SiteServer.Utils;
using SiteServer.CMS.Core;
using SiteServer.CMS.Plugin;
using SiteServer.CMS.StlParser.Model;
using SiteServer.CMS.StlParser.Utility;
using SiteServer.Plugin;
using SiteServer.Utils.Enumerations;

namespace SiteServer.CMS.StlParser
{
	public static class Parser
	{
        public static void Parse(PageInfo pageInfo, ContextInfo contextInfo, StringBuilder contentBuilder, string filePath, bool isDynamic)
        {
            foreach (var service in PluginManager.Services)
            {
                try
                {
                    service.OnBeforeStlParse(new ParseEventArgs
                    (
                        pageInfo.SiteId,
                        pageInfo.PageChannelId,
                        pageInfo.PageContentId,
                        contextInfo.ContentInfo,
                        pageInfo.TemplateInfo.TemplateType,
                        pageInfo.TemplateInfo.Id,
                        filePath,
                        pageInfo.HeadCodes,
                        pageInfo.BodyCodes,
                        pageInfo.FootCodes,
                        contentBuilder
                    ));
                }
                catch (Exception ex)
                {
                    LogUtils.AddStlErrorLog(pageInfo, service.PluginId, nameof(service.OnBeforeStlParse), ex);
                }
            }

            if (contentBuilder.Length > 0)
            {
                StlParserManager.ParseTemplateContent(contentBuilder, pageInfo, contextInfo);
            }

            foreach (var service in PluginManager.Services)
            {
                try
                {
                    service.OnAfterStlParse(new ParseEventArgs(pageInfo.SiteId, pageInfo.PageChannelId, pageInfo.PageContentId, contextInfo.ContentInfo, pageInfo.TemplateInfo.TemplateType, pageInfo.TemplateInfo.Id, filePath, pageInfo.HeadCodes, pageInfo.BodyCodes, pageInfo.FootCodes, contentBuilder));
                }
                catch (Exception ex)
                {
                    LogUtils.AddStlErrorLog(pageInfo, service.PluginId, nameof(service.OnAfterStlParse), ex);
                }
            }

            if (EFileSystemTypeUtils.IsHtml(PathUtils.GetExtension(filePath)))
            {
                if (isDynamic)
                {
                    var pageUrl = PageUtils.AddProtocolToUrl(PageUtils.ParseNavigationUrl($"~/{PathUtils.GetPathDifference(WebConfigUtils.PhysicalApplicationPath, filePath)}"));
                    string templateString = $@"
<base href=""{pageUrl}"" />";
                    StringUtils.InsertAfter(new[] { "<head>", "<HEAD>" }, contentBuilder, templateString);
                }

                if (pageInfo.SiteInfo.Additional.IsCreateBrowserNoCache)
                {
                    const string templateString = @"
<META HTTP-EQUIV=""Pragma"" CONTENT=""no-cache"">
<META HTTP-EQUIV=""Expires"" CONTENT=""-1"">";
                    StringUtils.InsertAfter(new[] { "<head>", "<HEAD>" }, contentBuilder, templateString);
                }

                if (pageInfo.SiteInfo.Additional.IsCreateIe8Compatible)
                {
                    const string templateString = @"
<META HTTP-EQUIV=""x-ua-compatible"" CONTENT=""ie=7"" />";
                    StringUtils.InsertAfter(new[] { "<head>", "<HEAD>" }, contentBuilder, templateString);
                }

                if (pageInfo.SiteInfo.Additional.IsCreateJsIgnoreError)
                {
                    const string templateString = @"
<script type=""text/javascript"">window.onerror=function(){return true;}</script>";
                    StringUtils.InsertAfter(new[] { "<head>", "<HEAD>" }, contentBuilder, templateString);
                }

                var isShowPageInfo = pageInfo.SiteInfo.Additional.IsCreateShowPageInfo;

                if (!pageInfo.IsLocal)
                {
                    if (pageInfo.SiteInfo.Additional.IsCreateDoubleClick)
                    {
                        var fileTemplateId = 0;
                        if (pageInfo.TemplateInfo.TemplateType == TemplateType.FileTemplate)
                        {
                            fileTemplateId = pageInfo.TemplateInfo.Id;
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
<!-- {pageInfo.TemplateInfo.RelatedFileName}({TemplateTypeUtils.GetText(pageInfo.TemplateInfo.TemplateType)}) -->");
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
                        contentBuilder.Insert(index + 1, StringUtils.Constants.ReturnAndNewline + bodyCodesHtml + StringUtils.Constants.ReturnAndNewline);
                    }
                    else
                    {
                        contentBuilder.Insert(0, bodyCodesHtml);
                    }
                }

                var footCodesHtml = pageInfo.FootCodesHtml;
                if (!string.IsNullOrEmpty(footCodesHtml))
                {
                    contentBuilder.Append(footCodesHtml + StringUtils.Constants.ReturnAndNewline);
                }
            }
        }
    }
}
