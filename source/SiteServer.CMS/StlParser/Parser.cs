using System;
using System.Text;
using BaiRong.Core;
using BaiRong.Core.Model.Enumerations;
using SiteServer.CMS.Controllers.Stl;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;
using SiteServer.CMS.Model.Enumerations;
using SiteServer.CMS.Plugin;
using SiteServer.CMS.StlParser.Model;
using SiteServer.CMS.StlParser.Utility;
using SiteServer.Plugin.Models;

namespace SiteServer.CMS.StlParser
{
	public class Parser
	{
		private Parser()
		{
		}

        public static void Parse(PublishmentSystemInfo publishmentSystemInfo, PageInfo pageInfo, ContextInfo contextInfo, StringBuilder contentBuilder, string filePath, bool isDynamic)
        {
            if (contentBuilder.Length > 0)
            {
                StlParserManager.ParseTemplateContent(contentBuilder, pageInfo, contextInfo);
            }

            if (EFileSystemTypeUtils.IsHtml(PathUtils.GetExtension(filePath)))
            {
                if (pageInfo.TemplateInfo.TemplateType != ETemplateType.FileTemplate)
                {
                    StlUtility.AddSeoMetaToContent(pageInfo, contentBuilder);
                }

                StlUtility.AddAdvertisementsToContent(pageInfo);

                if (isDynamic)
                {
                    var pageUrl = PageUtils.AddProtocolToUrl(PageUtility.GetPublishmentSystemUrlByPhysicalPath(publishmentSystemInfo, filePath));
                    string templateString = $@"
<base href=""{pageUrl}"" />";
                    StringUtils.InsertAfter(new[] { "<head>", "<HEAD>" }, contentBuilder, templateString);
                }

                if (pageInfo.PublishmentSystemInfo.Additional.IsCreateBrowserNoCache)
                {
                    const string templateString = @"
<META HTTP-EQUIV=""Pragma"" CONTENT=""no-cache"">
<META HTTP-EQUIV=""Expires"" CONTENT=""-1"">";
                    StringUtils.InsertAfter(new[] { "<head>", "<HEAD>" }, contentBuilder, templateString);
                }

                if (pageInfo.PublishmentSystemInfo.Additional.IsCreateIe8Compatible)
                {
                    const string templateString = @"
<META HTTP-EQUIV=""x-ua-compatible"" CONTENT=""ie=7"" />";
                    StringUtils.InsertAfter(new[] { "<head>", "<HEAD>" }, contentBuilder, templateString);
                }

                if (pageInfo.PublishmentSystemInfo.Additional.IsCreateJsIgnoreError)
                {
                    const string templateString = @"
<script type=""text/javascript"">window.onerror=function(){return true;}</script>";
                    StringUtils.InsertAfter(new[] { "<head>", "<HEAD>" }, contentBuilder, templateString);
                }

                if (pageInfo.PageContentId > 0 && pageInfo.PublishmentSystemInfo.Additional.IsCountHits && !pageInfo.IsPageScriptsExists(PageInfo.JsAdStlCountHits))
                {
                    pageInfo.AddPageEndScriptsIfNotExists(PageInfo.JsAdStlCountHits, $@"
<script src=""{ActionsAddContentHits.GetUrl(pageInfo.ApiUrl, pageInfo.PublishmentSystemId, pageInfo.PageNodeId, pageInfo.PageContentId)}"" type=""text/javascript""></script>");
                }

                if (pageInfo.PublishmentSystemInfo.Additional.IsTracker && !pageInfo.IsPageScriptsExists(PageInfo.JsAdAddTracker))
                {
                    pageInfo.AddPageEndScriptsIfNotExists(PageInfo.JsAdAddTracker, $@"
<script src=""{SiteFilesAssets.Tracker.GetScriptUrl(pageInfo.ApiUrl)}"" type=""text/javascript""></script>
<script type=""text/javascript"">AddTrackerCount('{ActionsAddTrackerCount.GetUrl(pageInfo.ApiUrl, pageInfo.PublishmentSystemId, pageInfo.PageNodeId, pageInfo.PageContentId)}', {pageInfo.PublishmentSystemId});</script>");
                }

                var headScripts = StlParserManager.GetPageInfoHeadScript(pageInfo, contextInfo);
                if (!string.IsNullOrEmpty(headScripts))
                {
                    if (contentBuilder.ToString().IndexOf("</head>", StringComparison.Ordinal) != -1 || contentBuilder.ToString().IndexOf("</HEAD>", StringComparison.Ordinal) != -1)
                    {
                        StringUtils.InsertBefore(new[] { "</head>", "</HEAD>" }, contentBuilder, headScripts);
                    }
                    else
                    {
                        contentBuilder.Insert(0, headScripts);
                    }
                }

                var afterBodyScripts = StlParserManager.GetPageInfoScript(pageInfo, true);

                if (!string.IsNullOrEmpty(afterBodyScripts))
                {
                    if (contentBuilder.ToString().IndexOf("<body", StringComparison.Ordinal) != -1 || contentBuilder.ToString().IndexOf("<BODY", StringComparison.Ordinal) != -1)
                    {
                        var index = contentBuilder.ToString().IndexOf("<body", StringComparison.Ordinal);
                        if (index == -1)
                        {
                            index = contentBuilder.ToString().IndexOf("<BODY", StringComparison.Ordinal);
                        }
                        index = contentBuilder.ToString().IndexOf(">", index, StringComparison.Ordinal);
                        contentBuilder.Insert(index + 1, StringUtils.Constants.ReturnAndNewline + afterBodyScripts + StringUtils.Constants.ReturnAndNewline);
                    }
                    else
                    {
                        contentBuilder.Insert(0, afterBodyScripts);
                    }
                }

                var beforeBodyScripts = StlParserManager.GetPageInfoScript(pageInfo, false);

                if (!string.IsNullOrEmpty(beforeBodyScripts))
                {
                    if (contentBuilder.ToString().IndexOf("</body>", StringComparison.Ordinal) != -1 || contentBuilder.ToString().IndexOf("</BODY>", StringComparison.Ordinal) != -1)
                    {
                        var index = contentBuilder.ToString().IndexOf("</body>", StringComparison.Ordinal);
                        if (index == -1)
                        {
                            index = contentBuilder.ToString().IndexOf("</BODY>", StringComparison.Ordinal);
                        }
                        contentBuilder.Insert(index, StringUtils.Constants.ReturnAndNewline + beforeBodyScripts + StringUtils.Constants.ReturnAndNewline);
                    }
                    else
                    {
                        contentBuilder.Append(beforeBodyScripts);
                    }
                }

                if (pageInfo.PublishmentSystemInfo.Additional.IsCreateDoubleClick)
                {
                    var fileTemplateId = 0;
                    if (pageInfo.TemplateInfo.TemplateType == ETemplateType.FileTemplate)
                    {
                        fileTemplateId = pageInfo.TemplateInfo.TemplateId;
                    }

                    var apiUrl = pageInfo.ApiUrl;
                    var ajaxUrl = ActionsTrigger.GetUrl(apiUrl, pageInfo.PublishmentSystemId, contextInfo.ChannelId, contextInfo.ContentId, fileTemplateId, true);
                    pageInfo.AddPageEndScriptsIfNotExists("CreateDoubleClick", $@"
<script type=""text/javascript"" language=""javascript"">document.ondblclick=function(x){{location.href = '{ajaxUrl}&returnUrl=' + encodeURIComponent(location.search);}}</script>");
                }

                if (pageInfo.PageEndScriptKeys.Count > 0)
                {
                    var endScriptBuilder = new StringBuilder();
                    foreach (string scriptKey in pageInfo.PageEndScriptKeys)
                    {
                        endScriptBuilder.Append(pageInfo.GetPageEndScripts(scriptKey));
                    }
                    endScriptBuilder.Append(StringUtils.Constants.ReturnAndNewline);

                    //contentBuilder.Append(endScriptBuilder.ToString());
                    //StringUtils.InsertBeforeOrAppend(new string[] { "</body>", "</BODY>" }, contentBuilder, endScriptBuilder.ToString());
                    StringUtils.InsertAfterOrAppend(new[] { "</html>", "</html>" }, contentBuilder, endScriptBuilder.ToString());
                }

                if (pageInfo.PublishmentSystemInfo.Additional.IsCreateShowPageInfo)
                {
                    contentBuilder.Append($@"
<!-- {pageInfo.TemplateInfo.RelatedFileName}({ETemplateTypeUtils.GetText(pageInfo.TemplateInfo.TemplateType)}) -->");
                }

                var renders = PluginCache.GetRenders();
                if (renders.Count > 0)
                {
                    var html = contentBuilder.ToString();
                    foreach (var render in renders)
                    {
                        try
                        {
                            var context = new PluginRenderContext(html, pageInfo.PublishmentSystemId, pageInfo.PageNodeId, pageInfo.PageContentId);
                            html = render(context);
                        }
                        catch (Exception ex)
                        {
                            LogUtils.AddErrorLog(ex, "plugin render");
                        }
                    }

                    contentBuilder.Clear();
                    contentBuilder.Append(html);
                }
            }
        }
    }
}
