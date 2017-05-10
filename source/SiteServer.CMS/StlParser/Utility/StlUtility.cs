using System;
using System.Collections.Specialized;
using System.Text;
using BaiRong.Core;
using BaiRong.Core.Model;
using BaiRong.Core.Model.Enumerations;
using SiteServer.CMS.Controllers.Stl;
using SiteServer.CMS.Core;
using SiteServer.CMS.Core.Advertisement;
using SiteServer.CMS.Model;
using SiteServer.CMS.Model.Enumerations;
using SiteServer.CMS.StlParser.Model;
using SiteServer.CMS.StlParser.StlElement;
using SiteServer.CMS.StlParser.StlEntity;

namespace SiteServer.CMS.StlParser.Utility
{
    public class StlUtility
    {
        public static string GetStlCurrentUrl(PageInfo pageInfo, int nodeId, int contentId, ContentInfo contentInfo)
        {
            var currentUrl = string.Empty;
            if (pageInfo.TemplateInfo.TemplateType == ETemplateType.IndexPageTemplate)
            {
                currentUrl = pageInfo.PublishmentSystemInfo.PublishmentSystemUrl;
            }
            else if (pageInfo.TemplateInfo.TemplateType == ETemplateType.ContentTemplate)
            {
                if (contentInfo == null)
                {
                    var nodeInfo = NodeManager.GetNodeInfo(pageInfo.PublishmentSystemId, nodeId);
                    currentUrl = PageUtility.GetContentUrl(pageInfo.PublishmentSystemInfo, nodeInfo, contentId, false);
                }
                else
                {
                    currentUrl = PageUtility.GetContentUrl(pageInfo.PublishmentSystemInfo, contentInfo);
                }
            }
            else if (pageInfo.TemplateInfo.TemplateType == ETemplateType.ChannelTemplate)
            {
                currentUrl = PageUtility.GetChannelUrl(pageInfo.PublishmentSystemInfo, NodeManager.GetNodeInfo(pageInfo.PublishmentSystemId, nodeId));
            }
            else if (pageInfo.TemplateInfo.TemplateType == ETemplateType.FileTemplate)
            {
                currentUrl = PageUtility.GetFileUrl(pageInfo.PublishmentSystemInfo, pageInfo.TemplateInfo.TemplateId);
            }
            //currentUrl是当前页面的地址，前后台分离的时候，不允许带上protocol
            //return PageUtils.AddProtocolToUrl(currentUrl);
            return currentUrl;
        }

        public static bool IsAdvertisementExists(PageInfo pageInfo)
        {
            var arraylists = AdvertisementManager.GetAdvertisementArrayLists(pageInfo.PublishmentSystemId);
            if (pageInfo.TemplateInfo.TemplateType == ETemplateType.IndexPageTemplate || pageInfo.TemplateInfo.TemplateType == ETemplateType.ChannelTemplate)
            {
                var arraylist = arraylists[0];
                return arraylist.Contains(pageInfo.PageNodeId);
            }
            else if (pageInfo.TemplateInfo.TemplateType == ETemplateType.ContentTemplate)
            {
                var arraylist = arraylists[1];
                return arraylist.Contains(pageInfo.PageContentId);
            }
            else if (pageInfo.TemplateInfo.TemplateType == ETemplateType.FileTemplate)
            {
                var arraylist = arraylists[2];
                return arraylist.Contains(pageInfo.TemplateInfo.TemplateId);
            }
            return false;
        }

        public static bool IsSeoMetaExists(PageInfo pageInfo)
        {
            var arraylists = SeoManager.GetSeoMetaArrayLists(pageInfo.PublishmentSystemId);
            if (pageInfo.PageContentId != 0)
            {
                var arraylist = arraylists[1];
                return arraylist.Contains(pageInfo.PageNodeId);
            }
            else
            {
                var arraylist = arraylists[0];
                return arraylist.Contains(pageInfo.PageNodeId);
            }
        }

        public static void ParseStl(PublishmentSystemInfo publishmentSystemInfo, PageInfo pageInfo, ContextInfo contextInfo, StringBuilder contentBuilder, string filePath, bool isDynamic)
        {
            if (contentBuilder.Length > 0)
            {
                StlParserManager.ParseTemplateContent(contentBuilder, pageInfo, contextInfo);
            }

            if (EFileSystemTypeUtils.IsHtml(PathUtils.GetExtension(filePath)))
            {
                if (pageInfo.TemplateInfo.TemplateType != ETemplateType.FileTemplate)
                {
                    AddSeoMetaToContent(pageInfo, contentBuilder);
                }

                AddAdvertisementsToContent(pageInfo);

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
                    var ajaxUrl = ActionsTrigger.GetUrl(pageInfo.PublishmentSystemInfo.Additional.ApiUrl, pageInfo.PublishmentSystemId, contextInfo.ChannelId, contextInfo.ContentId, fileTemplateId, true);
                    pageInfo.AddPageEndScriptsIfNotExists("CreateDoubleClick", $@"
<script type=""text/javascript"" language=""javascript"">document.ondblclick=function(x){{location.href = '{ajaxUrl}';}}</script>");
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
            }
        }

        private static void AddSeoMetaToContent(PageInfo pageInfo, StringBuilder contentBuilder)
        {
            if (IsSeoMetaExists(pageInfo))
            {
                int seoMetaId;
                if (pageInfo.PageContentId != 0)
                {
                    seoMetaId = DataProvider.SeoMetaDao.GetSeoMetaIdByNodeId(pageInfo.PageNodeId, false);
                    if (seoMetaId == 0)
                    {
                        seoMetaId = DataProvider.SeoMetaDao.GetDefaultSeoMetaId(pageInfo.PublishmentSystemId);
                    }
                }
                else
                {
                    seoMetaId = DataProvider.SeoMetaDao.GetSeoMetaIdByNodeId(pageInfo.PageNodeId, true);
                    if (seoMetaId == 0)
                    {
                        seoMetaId = DataProvider.SeoMetaDao.GetDefaultSeoMetaId(pageInfo.PublishmentSystemId);
                    }
                }
                if (seoMetaId != 0)
                {
                    var seoMetaInfo = DataProvider.SeoMetaDao.GetSeoMetaInfo(seoMetaId);
                    var seoMetaInfoFromTemplate = SeoManager.GetSeoMetaInfo(contentBuilder.ToString());
                    if (!string.IsNullOrEmpty(seoMetaInfoFromTemplate.PageTitle)) seoMetaInfo.PageTitle = string.Empty;
                    if (!string.IsNullOrEmpty(seoMetaInfoFromTemplate.Keywords)) seoMetaInfo.Keywords = string.Empty;
                    if (!string.IsNullOrEmpty(seoMetaInfoFromTemplate.Description)) seoMetaInfo.Description = string.Empty;
                    if (!string.IsNullOrEmpty(seoMetaInfoFromTemplate.Copyright)) seoMetaInfo.Copyright = string.Empty;
                    if (!string.IsNullOrEmpty(seoMetaInfoFromTemplate.Author)) seoMetaInfo.Author = string.Empty;
                    if (!string.IsNullOrEmpty(seoMetaInfoFromTemplate.Email)) seoMetaInfo.Email = string.Empty;
                    if (!string.IsNullOrEmpty(seoMetaInfoFromTemplate.Language)) seoMetaInfo.Language = string.Empty;
                    if (!string.IsNullOrEmpty(seoMetaInfoFromTemplate.Charset)) seoMetaInfo.Charset = string.Empty;
                    if (!string.IsNullOrEmpty(seoMetaInfoFromTemplate.Distribution)) seoMetaInfo.Distribution = string.Empty;
                    if (!string.IsNullOrEmpty(seoMetaInfoFromTemplate.Rating)) seoMetaInfo.Rating = string.Empty;
                    if (!string.IsNullOrEmpty(seoMetaInfoFromTemplate.Robots)) seoMetaInfo.Robots = string.Empty;
                    if (!string.IsNullOrEmpty(seoMetaInfoFromTemplate.RevisitAfter)) seoMetaInfo.RevisitAfter = string.Empty;
                    if (!string.IsNullOrEmpty(seoMetaInfoFromTemplate.Expires)) seoMetaInfo.Expires = string.Empty;

                    var metaContent = SeoManager.GetMetaContent(seoMetaInfo);

                    if (!string.IsNullOrEmpty(metaContent))
                    {
                        StringUtils.InsertBefore("</head>", contentBuilder, metaContent);
                    }
                }
            }
        }

        private static void AddAdvertisementsToContent(PageInfo pageInfo)
        {
            if (IsAdvertisementExists(pageInfo))
            {
                var advertisementNameArrayList = DataProvider.AdvertisementDao.GetAdvertisementNameArrayList(pageInfo.PublishmentSystemId);

                foreach (string advertisementName in advertisementNameArrayList)
                {
                    var adInfo = DataProvider.AdvertisementDao.GetAdvertisementInfo(advertisementName, pageInfo.PublishmentSystemId);
                    if (adInfo.IsDateLimited)
                    {
                        if (DateTime.Now < adInfo.StartDate || DateTime.Now > adInfo.EndDate)
                        {
                            continue;
                        }
                    }
                    var isToDo = false;
                    if (pageInfo.TemplateInfo.TemplateType == ETemplateType.IndexPageTemplate || pageInfo.TemplateInfo.TemplateType == ETemplateType.ChannelTemplate)
                    {
                        if (!string.IsNullOrEmpty(adInfo.NodeIDCollectionToChannel))
                        {
                            var nodeIdArrayList = TranslateUtils.StringCollectionToIntList(adInfo.NodeIDCollectionToChannel);
                            if (nodeIdArrayList.Contains(pageInfo.PageNodeId))
                            {
                                isToDo = true;
                            }
                        }
                    }
                    else if (pageInfo.TemplateInfo.TemplateType == ETemplateType.ContentTemplate)
                    {
                        if (!string.IsNullOrEmpty(adInfo.NodeIDCollectionToContent))
                        {
                            var nodeIdArrayList = TranslateUtils.StringCollectionToIntList(adInfo.NodeIDCollectionToContent);
                            if (nodeIdArrayList.Contains(pageInfo.PageContentId))
                            {
                                isToDo = true;
                            }
                        }
                    }
                    else if (pageInfo.TemplateInfo.TemplateType == ETemplateType.FileTemplate)
                    {
                        if (!string.IsNullOrEmpty(adInfo.FileTemplateIDCollection))
                        {
                            var fileTemplateIdArrayList = TranslateUtils.StringCollectionToIntList(adInfo.FileTemplateIDCollection);
                            if (fileTemplateIdArrayList.Contains(pageInfo.TemplateInfo.TemplateId))
                            {
                                isToDo = true;
                            }
                        }
                    }

                    if (isToDo)
                    {
                        var scripts = string.Empty;
                        if (adInfo.AdvertisementType == EAdvertisementType.FloatImage)
                        {
                            pageInfo.AddPageScriptsIfNotExists(PageInfo.JsStaticAdFloating);

                            var floatScript = new FloatingScript(pageInfo.PublishmentSystemInfo, pageInfo.UniqueId, adInfo);
                            scripts = floatScript.GetScript();
                        }
                        else if (adInfo.AdvertisementType == EAdvertisementType.ScreenDown)
                        {
                            pageInfo.AddPageScriptsIfNotExists(PageInfo.Components.Jquery);

                            var screenDownScript = new ScreenDownScript(pageInfo.PublishmentSystemInfo, pageInfo.UniqueId, adInfo);
                            scripts = screenDownScript.GetScript();
                        }
                        else if (adInfo.AdvertisementType == EAdvertisementType.OpenWindow)
                        {
                            var openWindowScript = new OpenWindowScript(pageInfo.PublishmentSystemInfo, pageInfo.UniqueId, adInfo);
                            scripts = openWindowScript.GetScript();
                        }

                        pageInfo.AddPageEndScriptsIfNotExists(EAdvertisementTypeUtils.GetValue(adInfo.AdvertisementType) + "_" + adInfo.AdvertisementName, scripts);
                    }
                }
            }
        }

        public static string ParseDynamicContent(int publishmentSystemId, int channelId, int contentId, int templateId, bool isPageRefresh, string templateContent, string pageUrl, int pageIndex, string ajaxDivId, NameValueCollection queryString, UserInfo userInfo)
        {
            var templateInfo = TemplateManager.GetTemplateInfo(publishmentSystemId, templateId);
            //TemplateManager.GetTemplateInfo(publishmentSystemID, channelID, templateType);
            var publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(publishmentSystemId);
            var pageInfo = new PageInfo(channelId, contentId, publishmentSystemInfo, templateInfo, userInfo);
            pageInfo.SetUniqueId(1000);
            var contextInfo = new ContextInfo(pageInfo);

            templateContent = StlRequestEntities.ParseRequestEntities(queryString, templateContent);
            var contentBuilder = new StringBuilder(templateContent);
            var stlElementList = StlParserUtility.GetStlElementList(contentBuilder.ToString());

            //如果标签中存在<stl:pageContents>
            if (StlParserUtility.IsStlElementExists(StlPageContents.ElementName, stlElementList))
            {
                var stlElement = StlParserUtility.GetStlElement(StlPageContents.ElementName, stlElementList);
                var stlPageContentsElement = stlElement;
                var stlPageContentsElementReplaceString = stlElement;

                var pageContentsElementParser = new StlPageContents(stlPageContentsElement, pageInfo, contextInfo, true);
                int totalNum;
                var pageCount = pageContentsElementParser.GetPageCount(out totalNum);

                for (var currentPageIndex = 0; currentPageIndex < pageCount; currentPageIndex++)
                {
                    if (currentPageIndex == pageIndex)
                    {
                        var pageHtml = pageContentsElementParser.Parse(totalNum, currentPageIndex, pageCount, false);
                        contentBuilder.Replace(stlPageContentsElementReplaceString, pageHtml);

                        StlParserManager.ReplacePageElementsInDynamicPage(contentBuilder, pageInfo, stlElementList, pageUrl, pageInfo.PageNodeId, currentPageIndex, pageCount, totalNum, isPageRefresh, ajaxDivId);

                        break;
                    }
                }
            }
            //如果标签中存在<stl:pageChannels>
            else if (StlParserUtility.IsStlElementExists(StlPageChannels.ElementName, stlElementList))
            {
                var stlElement = StlParserUtility.GetStlElement(StlPageChannels.ElementName, stlElementList);
                var stlPageChannelsElement = stlElement;
                var stlPageChannelsElementReplaceString = stlElement;

                var pageChannelsElementParser = new StlPageChannels(stlPageChannelsElement, pageInfo, contextInfo, true);
                int totalNum;
                var pageCount = pageChannelsElementParser.GetPageCount(out totalNum);

                for (var currentPageIndex = 0; currentPageIndex < pageCount; currentPageIndex++)
                {
                    if (currentPageIndex == pageIndex)
                    {
                        var pageHtml = pageChannelsElementParser.Parse(currentPageIndex, pageCount);
                        contentBuilder.Replace(stlPageChannelsElementReplaceString, pageHtml);

                        StlParserManager.ReplacePageElementsInDynamicPage(contentBuilder, pageInfo, stlElementList, pageUrl, pageInfo.PageNodeId, currentPageIndex, pageCount, totalNum, isPageRefresh, ajaxDivId);

                        break;
                    }
                }
            }
            //如果标签中存在<stl:pageComments>
            else if (StlParserUtility.IsStlElementExists(StlPageComments.ElementName, stlElementList))
            {
                var stlElement = StlParserUtility.GetStlElement(StlPageComments.ElementName, stlElementList);
                var stlPageCommentsElement = stlElement;
                var stlPageCommentsElementReplaceString = stlElement;

                var pageCommentsElementParser = new StlPageComments(stlPageCommentsElement, pageInfo, contextInfo, true);
                int totalNum;
                var pageCount = pageCommentsElementParser.GetPageCount(out totalNum);

                for (var currentPageIndex = 0; currentPageIndex < pageCount; currentPageIndex++)
                {
                    if (currentPageIndex == pageIndex)
                    {
                        var pageHtml = pageCommentsElementParser.Parse(currentPageIndex, pageCount);
                        contentBuilder.Replace(stlPageCommentsElementReplaceString, pageHtml);

                        StlParserManager.ReplacePageElementsInDynamicPage(contentBuilder, pageInfo, stlElementList, pageUrl, pageInfo.PageNodeId, currentPageIndex, pageCount, totalNum, isPageRefresh, ajaxDivId);

                        break;
                    }
                }
            }
            //如果标签中存在<stl:pageinputContents>
            else if (StlParserUtility.IsStlElementExists(StlPageInputContents.ElementName, stlElementList))
            {
                var stlElement = StlParserUtility.GetStlElement(StlPageInputContents.ElementName, stlElementList);
                var stlPageInputContentsElement = stlElement;
                var stlPageInputContentsElementReplaceString = stlElement;

                var pageInputContentsElementParser = new StlPageInputContents(stlPageInputContentsElement, pageInfo, contextInfo, true);
                int totalNum;
                var pageCount = pageInputContentsElementParser.GetPageCount(out totalNum);

                for (var currentPageIndex = 0; currentPageIndex < pageCount; currentPageIndex++)
                {
                    if (currentPageIndex == pageIndex)
                    {
                        var pageHtml = pageInputContentsElementParser.Parse(currentPageIndex, pageCount);
                        contentBuilder.Replace(stlPageInputContentsElementReplaceString, pageHtml);

                        StlParserManager.ReplacePageElementsInDynamicPage(contentBuilder, pageInfo, stlElementList, pageUrl, pageInfo.PageNodeId, currentPageIndex, pageCount, totalNum, isPageRefresh, ajaxDivId);

                        break;
                    }
                }
            }
            //如果标签中存在<stl:pageSqlContents>
            else if (StlParserUtility.IsStlElementExists(StlPageSqlContents.ElementName, stlElementList))
            {
                var stlElement = StlParserUtility.GetStlElement(StlPageSqlContents.ElementName, stlElementList);
                var stlPageSqlContentsElement = stlElement;
                var stlPageSqlContentsElementReplaceString = stlElement;

                var pageSqlContentsElementParser = new StlPageSqlContents(stlPageSqlContentsElement, pageInfo, contextInfo, true);
                int totalNum;
                var pageCount = pageSqlContentsElementParser.GetPageCount(out totalNum);

                for (var currentPageIndex = 0; currentPageIndex < pageCount; currentPageIndex++)
                {
                    if (currentPageIndex == pageIndex)
                    {
                        var pageHtml = pageSqlContentsElementParser.Parse(currentPageIndex, pageCount);
                        contentBuilder.Replace(stlPageSqlContentsElementReplaceString, pageHtml);

                        StlParserManager.ReplacePageElementsInDynamicPage(contentBuilder, pageInfo, stlElementList, pageUrl, pageInfo.PageNodeId, currentPageIndex, pageCount, totalNum, isPageRefresh, ajaxDivId);

                        break;
                    }
                }
            }

            else if (StlParserUtility.IsStlElementExists(StlPageItems.ElementName, stlElementList))
            {
                var pageCount = TranslateUtils.ToInt(queryString["pageCount"]);
                var totalNum = TranslateUtils.ToInt(queryString["totalNum"]);
                var pageContentsAjaxDivId = queryString["pageContentsAjaxDivID"];

                for (var currentPageIndex = 0; currentPageIndex < pageCount; currentPageIndex++)
                {
                    if (currentPageIndex == pageIndex)
                    {
                        StlParserManager.ReplacePageElementsInDynamicPage(contentBuilder, pageInfo, stlElementList, pageUrl, pageInfo.PageNodeId, currentPageIndex, pageCount, totalNum, isPageRefresh, pageContentsAjaxDivId);

                        break;
                    }
                }
            }

            StlParserManager.ParseInnerContent(contentBuilder, pageInfo, contextInfo);

            //string afterBodyScript = StlParserManager.GetPageInfoScript(pageInfo, true);
            //string beforBodyScript = StlParserManager.GetPageInfoScript(pageInfo, false);

            //return afterBodyScript + StlParserUtility.GetBackHtml(contentBuilder.ToString(), pageInfo) + beforBodyScript;

            return StlParserUtility.GetBackHtml(contentBuilder.ToString(), pageInfo);

            //return contentBuilder.ToString();
        }
    }
}
