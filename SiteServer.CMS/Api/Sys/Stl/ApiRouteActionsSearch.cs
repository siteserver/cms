using System.Collections.Generic;
using SiteServer.CMS.StlParser.StlElement;
using SiteServer.Utils;

namespace SiteServer.CMS.Api.Sys.Stl
{
    public static class ApiRouteActionsSearch
    {
        public const string Route = "sys/stl/actions/search";

        public static string GetUrl(string apiUrl)
        {
            return PageUtils.Combine(apiUrl, Route);
        }

        public static string GetParameters(bool isAllSites, string siteName, string siteDir, string siteIds, string channelIndex, string channelName, string channelIds, string type, string word, string dateAttribute, string dateFrom, string dateTo, string since, int pageNum, bool isHighlight, int siteId, string ajaxDivId, string template)
        {
            return $@"
{{
    {StlSearch.IsAllSites.Name.ToLower()}: {isAllSites.ToString().ToLower()},
    {StlSearch.SiteName.Name.ToLower()}: '{siteName}',
    {StlSearch.SiteDir.Name.ToLower()}: '{siteDir}',
    {StlSearch.SiteIds.Name.ToLower()}: '{siteIds}',
    {StlSearch.ChannelIndex.Name.ToLower()}: '{channelIndex}',
    {StlSearch.ChannelName.Name.ToLower()}: '{channelName}',
    {StlSearch.ChannelIds.Name.ToLower()}: '{channelIds}',
    {StlSearch.Type.Name.ToLower()}: '{type}',
    {StlSearch.Word.Name.ToLower()}: '{word}',
    {StlSearch.DateAttribute.Name.ToLower()}: '{dateAttribute}',
    {StlSearch.DateFrom.Name.ToLower()}: '{dateFrom}',
    {StlSearch.DateTo.Name.ToLower()}: '{dateTo}',
    {StlSearch.Since.Name.ToLower()}: '{since}',
    {StlSearch.PageNum.Name.ToLower()}: {pageNum},
    {StlSearch.IsHighlight.Name.ToLower()}: {isHighlight.ToString().ToLower()},
    siteid: '{siteId}',
    ajaxdivid: '{ajaxDivId}',
    template: '{TranslateUtils.EncryptStringBySecretKey(template)}',
}}";
        }

        public static List<string> ExlcudeAttributeNames => new List<string>
        {
            StlSearch.IsAllSites.Name.ToLower(),
            StlSearch.SiteName.Name.ToLower(),
            StlSearch.SiteDir.Name.ToLower(),
            StlSearch.SiteIds.Name.ToLower(),
            StlSearch.ChannelIndex.Name.ToLower(),
            StlSearch.ChannelName.Name.ToLower(),
            StlSearch.ChannelIds.Name.ToLower(),
            StlSearch.Type.Name.ToLower(),
            StlSearch.Word.Name.ToLower(),
            StlSearch.DateAttribute.Name.ToLower(),
            StlSearch.DateFrom.Name.ToLower(),
            StlSearch.DateTo.Name.ToLower(),
            StlSearch.Since.Name.ToLower(),
            StlSearch.PageNum.Name.ToLower(),
            StlSearch.IsHighlight.Name.ToLower(),
            "siteid",
            "ajaxdivid",
            "template",
        };
    }
}