using System;
using System.Collections.Generic;
using System.Text;
using SS.CMS.Core.Api.Sys.Stl;
using SS.CMS.Core.Common;
using SS.CMS.Core.StlParser.Models;
using SS.CMS.Core.StlParser.StlElement;
using SS.CMS.Core.StlParser.Utility;
using SS.CMS.Enums;
using SS.CMS.Models;
using SS.CMS.Utils;
using SS.CMS.Utils.Enumerations;

namespace SS.CMS.Core.StlParser
{
    public partial class ParseContext
    {
        public void Parse(StringBuilder contentBuilder, string filePath, bool isDynamic)
        {
            foreach (var service in PluginManager.Services)
            {
                try
                {
                    service.OnBeforeStlParse(new ParseEventArgs
                    (
                        PageInfo.SiteId,
                        PageInfo.PageChannelId,
                        PageInfo.PageContentId,
                        ContentInfo,
                        PageInfo.TemplateInfo.Type,
                        PageInfo.TemplateInfo.Id,
                        filePath,
                        PageInfo.HeadCodes,
                        PageInfo.BodyCodes,
                        PageInfo.FootCodes,
                        contentBuilder
                    ));
                }
                catch (Exception ex)
                {
                    GetErrorMessage(service.PluginId, nameof(service.OnBeforeStlParse), ex);
                }
            }

            if (contentBuilder.Length > 0)
            {
                ParseTemplateContent(contentBuilder);
            }

            foreach (var service in PluginManager.Services)
            {
                try
                {
                    service.OnAfterStlParse(new ParseEventArgs(SiteId, PageChannelId, PageContentId, ContentInfo, TemplateInfo.Type, TemplateInfo.Id, filePath, HeadCodes, BodyCodes, FootCodes, contentBuilder));
                }
                catch (Exception ex)
                {
                    GetErrorMessage(service.PluginId, nameof(service.OnAfterStlParse), ex);
                }
            }

            if (EFileSystemTypeUtils.IsHtml(PathUtils.GetExtension(filePath)))
            {
                if (isDynamic)
                {
                    var pageUrl = UrlManager.AddProtocolToUrl(UrlManager.ParseNavigationUrl($"~/{PathUtils.GetPathDifference(SettingsManager.WebRootPath, filePath)}"));
                    string templateString = $@"
<base href=""{pageUrl}"" />";
                    StringUtils.InsertAfter(new[] { "<head>", "<HEAD>" }, contentBuilder, templateString);
                }

                if (SiteInfo.IsCreateBrowserNoCache)
                {
                    const string templateString = @"
<META HTTP-EQUIV=""Pragma"" CONTENT=""no-cache"">
<META HTTP-EQUIV=""Expires"" CONTENT=""-1"">";
                    StringUtils.InsertAfter(new[] { "<head>", "<HEAD>" }, contentBuilder, templateString);
                }

                if (SiteInfo.IsCreateIe8Compatible)
                {
                    const string templateString = @"
<META HTTP-EQUIV=""x-ua-compatible"" CONTENT=""ie=7"" />";
                    StringUtils.InsertAfter(new[] { "<head>", "<HEAD>" }, contentBuilder, templateString);
                }

                if (SiteInfo.IsCreateJsIgnoreError)
                {
                    const string templateString = @"
<script type=""text/javascript"">window.onerror=function(){return true;}</script>";
                    StringUtils.InsertAfter(new[] { "<head>", "<HEAD>" }, contentBuilder, templateString);
                }

                var isShowPageInfo = SiteInfo.IsCreateShowPageInfo;

                if (!IsLocal)
                {
                    if (SiteInfo.IsCreateDoubleClick)
                    {
                        var fileTemplateId = 0;
                        if (TemplateInfo.Type == TemplateType.FileTemplate)
                        {
                            fileTemplateId = TemplateInfo.Id;
                        }

                        var apiUrl = ApiUrl;
                        var ajaxUrl = ApiRouteActionsTrigger.GetUrl(apiUrl, SiteId, ChannelId,
                            ContentId, fileTemplateId, true);
                        if (!FootCodes.ContainsKey("CreateDoubleClick"))
                        {
                            FootCodes.Add("CreateDoubleClick", $@"
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
<!-- {TemplateInfo.RelatedFileName}({TemplateTypeUtils.GetText(TemplateInfo.Type)}) -->");
                }

                var headCodesHtml = PageInfo.HeadCodesHtml;
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

                var bodyCodesHtml = PageInfo.BodyCodesHtml;
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

                var footCodesHtml = PageInfo.FootCodesHtml;
                if (!string.IsNullOrEmpty(footCodesHtml))
                {
                    contentBuilder.Append(footCodesHtml + Constants.ReturnAndNewline);
                }
            }
        }

        public void ParseTemplateContent(StringBuilder parsedBuilder)
        {
            var isInnerElement = IsInnerElement;
            IsInnerElement = false;
            ContainerClientId = string.Empty;
            ReplaceStlElements(parsedBuilder);
            ReplaceStlEntities(parsedBuilder);
            IsInnerElement = isInnerElement;
        }

        public string ParseTemplatePreview(SiteInfo siteInfo, string template)
        {
            if (string.IsNullOrEmpty(template)) return string.Empty;

            var parsedBuilder = new StringBuilder(template);

            ParseTemplateContent(parsedBuilder);

            return PageInfo.HeadCodesHtml + PageInfo.BodyCodesHtml + parsedBuilder + PageInfo.FootCodesHtml;
        }

        public void ParseInnerContent(StringBuilder builder)
        {
            if (builder == null || builder.Length == 0) return;

            var isInnerElement = IsInnerElement;
            IsInnerElement = true;
            ReplaceStlElements(builder);
            ReplaceStlEntities(builder);
            IsInnerElement = isInnerElement;
        }

        public string ParseInnerContent(string template)
        {
            if (string.IsNullOrEmpty(template)) return string.Empty;

            var builder = new StringBuilder(template);
            ParseInnerContent(builder);
            return builder.ToString();
        }

        public string StlEncrypt(string stlElement)
        {
            return $"<!-- {SettingsManager.Encrypt(stlElement)} -->";
        }

        public void ReplacePageElementsInContentPage(StringBuilder parsedBuilder, List<string> labelList, int currentPageIndex, int pageCount)
        {
            //替换分页模板
            foreach (var labelString in labelList)
            {
                if (StlParserUtility.IsSpecifiedStlElement(labelString, StlPageItems.ElementName))
                {
                    var stlElement = labelString;
                    var pageHtml = ParseStlPageInContentPage(stlElement, currentPageIndex, pageCount);
                    parsedBuilder.Replace(StlEncrypt(stlElement), pageHtml);
                }
                else if (StlParserUtility.IsSpecifiedStlElement(labelString, StlPageItem.ElementName))
                {
                    var stlElement = labelString;
                    var pageHtml = ParseStlPageItemInContentPage(stlElement, currentPageIndex, pageCount, pageCount);
                    parsedBuilder.Replace(stlElement, pageHtml);
                }
            }
        }

        public void ReplacePageElementsInChannelPage(StringBuilder parsedBuilder, List<string> labelList, int currentPageIndex, int pageCount, int totalNum)
        {
            //替换分页模板
            foreach (var labelString in labelList)
            {
                if (StlParserUtility.IsSpecifiedStlElement(labelString, StlPageItems.ElementName))
                {
                    var stlElement = labelString;
                    var pageHtml = ParseStlPageInChannelPage(stlElement, currentPageIndex, pageCount, totalNum);
                    parsedBuilder.Replace(StlEncrypt(stlElement), pageHtml);
                }
                else if (StlParserUtility.IsSpecifiedStlElement(labelString, StlPageItem.ElementName))
                {
                    var stlElement = labelString;
                    var pageHtml = ParseStlPageItemInChannelPage(stlElement, currentPageIndex, pageCount, totalNum);
                    parsedBuilder.Replace(stlElement, pageHtml);
                }
            }
        }

        public void ReplacePageElementsInSearchPage(StringBuilder parsedBuilder, List<string> labelList, string ajaxDivId, int currentPageIndex, int pageCount, int totalNum)
        {
            //替换分页模板
            foreach (var labelString in labelList)
            {
                if (StlParserUtility.IsSpecifiedStlElement(labelString, StlPageItems.ElementName))
                {
                    var stlElement = labelString;
                    var pageHtml = ParseStlPageInSearchPage(stlElement, ajaxDivId, currentPageIndex, pageCount, totalNum);
                    parsedBuilder.Replace(stlElement, pageHtml);
                }
                else if (StlParserUtility.IsSpecifiedStlElement(labelString, StlPageItem.ElementName))
                {
                    var stlElement = labelString;
                    var pageHtml = ParseStlPageItemInSearchPage(stlElement, ajaxDivId, currentPageIndex, pageCount, totalNum);
                    parsedBuilder.Replace(stlElement, pageHtml);
                }
            }
        }

        public void ReplacePageElementsInDynamicPage(StringBuilder parsedBuilder, List<string> labelList, int currentPageIndex, int pageCount, int totalNum, bool isPageRefresh, string ajaxDivId)
        {
            //替换分页模板
            foreach (var labelString in labelList)
            {
                if (StlParserUtility.IsSpecifiedStlElement(labelString, StlPageItems.ElementName))
                {
                    var stlElement = labelString;
                    var pageHtml = ParseStlPageInDynamicPage(stlElement, currentPageIndex, pageCount, totalNum, isPageRefresh, ajaxDivId);
                    parsedBuilder.Replace(stlElement, pageHtml);
                }
                else if (StlParserUtility.IsSpecifiedStlElement(labelString, StlPageItem.ElementName))
                {
                    var stlElement = labelString;
                    var pageHtml = ParseStlPageItemInDynamicPage(stlElement, currentPageIndex, pageCount, totalNum, isPageRefresh, ajaxDivId);
                    parsedBuilder.Replace(stlElement, pageHtml);
                }
            }
        }
    }
}
