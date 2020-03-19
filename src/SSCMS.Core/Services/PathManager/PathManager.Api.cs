using System.Collections.Generic;
using System.Collections.Specialized;
using SSCMS;
using SSCMS.Core.StlParser.StlElement;
using SSCMS.Utils;

namespace SSCMS.Core.Services.PathManager
{
    public partial class PathManager
    {
        public string GetApiUrl(string route)
        {
            return PageUtils.Combine(WebUrl, Constants.ApiPrefix, route);
        }

        public string GetApiUrl(Config config)
        {
            return config.IsSeparatedApi ? config.SeparatedApiUrl : ParseNavigationUrl("~/api");
        }

        public string InnerApiUrl => ParseNavigationUrl("~/api");

        public string GetInnerApiUrl(string route)
        {
            return PageUtils.Combine(InnerApiUrl, route);
        }

        public string GetDownloadApiUrl(string apiUrl, int siteId, int channelId, int contentId, string fileUrl)
        {
            return PageUtils.AddQueryString(PageUtils.Combine(apiUrl, Constants.RouteActionsDownload), new NameValueCollection
            {
                {"siteId", siteId.ToString()},
                {"channelId", channelId.ToString()},
                {"contentId", contentId.ToString()},
                {"fileUrl", _settingsManager.Encrypt(fileUrl)}
            });
        }

        public string GetDownloadApiUrl(string apiUrl, int siteId, string fileUrl)
        {
            return PageUtils.AddQueryString(PageUtils.Combine(apiUrl, Constants.RouteActionsDownload), new NameValueCollection
            {
                {"siteId", siteId.ToString()},
                {"fileUrl", _settingsManager.Encrypt(fileUrl)}
            });
        }

        public string GetDownloadApiUrl(string apiUrl, string filePath)
        {
            return PageUtils.AddQueryString(PageUtils.Combine(apiUrl, Constants.RouteActionsDownload), new NameValueCollection
            {
                {"filePath", _settingsManager.Encrypt(filePath)}
            });
        }

        public string GetDynamicApiUrl(string apiUrl)
        {
            return PageUtils.Combine(apiUrl, Constants.RouteActionsDynamic);
        }

        public string GetIfApiUrl(string apiUrl)
        {
            return PageUtils.Combine(apiUrl, Constants.RouteRouteActionsIf);
        }

        public string GetPageContentsApiUrl(string apiUrl)
        {
            return PageUtils.Combine(apiUrl, Constants.RouteActionsPageContents);
        }

        public string GetPageContentsApiParameters(int siteId, int pageChannelId, int templateId, int totalNum, int pageCount,
            int currentPageIndex, string stlPageContentsElement)
        {
            return $@"
{{
    siteId: {siteId},
    pageChannelId: {pageChannelId},
    templateId: {templateId},
    totalNum: {totalNum},
    pageCount: {pageCount},
    currentPageIndex: {currentPageIndex},
    stlPageContentsElement: '{_settingsManager.Encrypt(stlPageContentsElement)}'
}}";
        }

        public string GetSearchApiUrl(string apiUrl)
        {
            return PageUtils.Combine(apiUrl, Constants.RouteActionsSearch);
        }

        public string GetSearchApiParameters(bool isAllSites, string siteName, string siteDir, string siteIds, string channelIndex, string channelName, string channelIds, string type, string word, string dateAttribute, string dateFrom, string dateTo, string since, int pageNum, bool isHighlight, int siteId, string ajaxDivId, string template)
        {
            return $@"
{{
    {StlSearch.IsAllSites.ToLower()}: {isAllSites.ToString().ToLower()},
    {StlSearch.SiteName.ToLower()}: '{siteName}',
    {StlSearch.SiteDir.ToLower()}: '{siteDir}',
    {StlSearch.SiteIds.ToLower()}: '{siteIds}',
    {StlSearch.ChannelIndex.ToLower()}: '{channelIndex}',
    {StlSearch.ChannelName.ToLower()}: '{channelName}',
    {StlSearch.ChannelIds.ToLower()}: '{channelIds}',
    {StlSearch.Type.ToLower()}: '{type}',
    {StlSearch.Word.ToLower()}: '{word}',
    {StlSearch.DateAttribute.ToLower()}: '{dateAttribute}',
    {StlSearch.DateFrom.ToLower()}: '{dateFrom}',
    {StlSearch.DateTo.ToLower()}: '{dateTo}',
    {StlSearch.Since.ToLower()}: '{since}',
    {StlSearch.PageNum.ToLower()}: {pageNum},
    {StlSearch.IsHighlight.ToLower()}: {isHighlight.ToString().ToLower()},
    siteid: '{siteId}',
    ajaxdivid: '{ajaxDivId}',
    template: '{_settingsManager.Encrypt(template)}',
}}";
        }

        public List<string> GetSearchExlcudeAttributeNames => new List<string>
        {
            StlSearch.IsAllSites.ToLower(),
            StlSearch.SiteName.ToLower(),
            StlSearch.SiteDir.ToLower(),
            StlSearch.SiteIds.ToLower(),
            StlSearch.ChannelIndex.ToLower(),
            StlSearch.ChannelName.ToLower(),
            StlSearch.ChannelIds.ToLower(),
            StlSearch.Type.ToLower(),
            StlSearch.Word.ToLower(),
            StlSearch.DateAttribute.ToLower(),
            StlSearch.DateFrom.ToLower(),
            StlSearch.DateTo.ToLower(),
            StlSearch.Since.ToLower(),
            StlSearch.PageNum.ToLower(),
            StlSearch.IsHighlight.ToLower(),
            "siteid",
            "ajaxdivid",
            "template",
        };

        public string GetTriggerApiUrl(string apiUrl, int siteId, int channelId, int contentId,
            int fileTemplateId, bool isRedirect)
        {
            return PageUtils.AddQueryString(PageUtils.Combine(apiUrl, Constants.RouteActionsTrigger), new NameValueCollection
            {
                {"siteId", siteId.ToString()},
                {"channelId", channelId.ToString()},
                {"contentId", contentId.ToString()},
                {"fileTemplateId", fileTemplateId.ToString()},
                {"isRedirect", isRedirect.ToString()}
            });
        }
    }
}
