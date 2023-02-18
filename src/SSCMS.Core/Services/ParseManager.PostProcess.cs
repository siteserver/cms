using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Datory;
using SSCMS.Configuration;
using SSCMS.Enums;
using SSCMS.Utils;

namespace SSCMS.Core.Services
{
    public partial class ParseManager
    {
        private void PostProcess(StringBuilder contentBuilder, string filePath)
        {
            if (PageInfo.Site.IsCreateBrowserNoCache)
            {
                const string templateString = @"
<META HTTP-EQUIV=""Pragma"" CONTENT=""no-cache"">
<META HTTP-EQUIV=""Expires"" CONTENT=""-1"">";
                StringUtils.InsertAfter(new[] { "<head>", "<HEAD>" }, contentBuilder, templateString);
            }

            if (PageInfo.Site.IsCreateIe8Compatible)
            {
                const string templateString = @"
<META HTTP-EQUIV=""x-ua-compatible"" CONTENT=""ie=7"" />";
                StringUtils.InsertAfter(new[] { "<head>", "<HEAD>" }, contentBuilder, templateString);
            }

            if (PageInfo.Site.IsCreateFilterGray)
            {
                const string templateString = @"
<style>html{filter: grayscale(100%); -webkit-filter: grayscale(100%); -moz-filter: grayscale(100%); -ms-filter: grayscale(100%); -o-filter: grayscale(100%); filter:gray;}</style>";
                StringUtils.InsertAfter(new[] { "<head>", "<HEAD>" }, contentBuilder, templateString);
            }

            if (PageInfo.Site.IsCreateJsIgnoreError)
            {
                const string templateString = @"
<script type=""text/javascript"">window.onerror=function(){return true;}</script>";
                StringUtils.InsertAfter(new[] { "<head>", "<HEAD>" }, contentBuilder, templateString);
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

                    var ajaxUrl = PathManager.GetTriggerApiUrl(PageInfo.SiteId, ContextInfo.ChannelId,
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
                    StringUtils.InsertBefore(new[] { "</head>", "</HEAD>" }, contentBuilder, headCodesHtml);
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
}