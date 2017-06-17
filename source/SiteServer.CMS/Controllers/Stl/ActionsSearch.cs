using System.Collections.Generic;
using BaiRong.Core;
using SiteServer.CMS.StlParser.StlElement;

namespace SiteServer.CMS.Controllers.Stl
{
    public class ActionsSearch
    {
        public const string Route = "stl/actions/search";

        public static string GetUrl(string apiUrl)
        {
            return PageUtils.Combine(apiUrl, Route);
        }

        public static string GetParameters(bool isAllSites, string siteName, string siteDir, string siteIds, string channelIndex, string channelName, string channelIds, string type, string word, string dateAttribute, string dateFrom, string dateTo, string since, int pageNum, bool isHighlight, bool isDefaultDisplay, int publishmentSystemId, string ajaxDivId, string template)
        {
            return $@"
{{
    {StlSearch.AttributeIsAllSites.ToLower()}: {isAllSites.ToString().ToLower()},
    {StlSearch.AttributeSiteName.ToLower()}: '{siteName}',
    {StlSearch.AttributeSiteDir.ToLower()}: '{siteDir}',
    {StlSearch.AttributeSiteIds.ToLower()}: '{siteIds}',
    {StlSearch.AttributeChannelIndex.ToLower()}: '{channelIndex}',
    {StlSearch.AttributeChannelName.ToLower()}: '{channelName}',
    {StlSearch.AttributeChannelIds.ToLower()}: '{channelIds}',
    {StlSearch.AttributeType.ToLower()}: '{type}',
    {StlSearch.AttributeWord.ToLower()}: '{word}',
    {StlSearch.AttributeDateAttribute.ToLower()}: '{dateAttribute}',
    {StlSearch.AttributeDateFrom.ToLower()}: '{dateFrom}',
    {StlSearch.AttributeDateTo.ToLower()}: '{dateTo}',
    {StlSearch.AttributeSince.ToLower()}: '{since}',
    {StlSearch.AttributePageNum.ToLower()}: {pageNum},
    {StlSearch.AttributeIsHighlight.ToLower()}: {isHighlight.ToString().ToLower()},
    {StlSearch.AttributeIsDefaultDisplay.ToLower()}: {isDefaultDisplay.ToString().ToLower()},
    publishmentsystemid: '{publishmentSystemId}',
    ajaxdivid: '{ajaxDivId}',
    template: '{TranslateUtils.EncryptStringBySecretKey(template)}',
}}";
        }

        public static List<string> ExlcudeAttributeNames => new List<string>
        {
            StlSearch.AttributeIsAllSites.ToLower(),
            StlSearch.AttributeSiteName.ToLower(),
            StlSearch.AttributeSiteDir.ToLower(),
            StlSearch.AttributeSiteIds.ToLower(),
            StlSearch.AttributeChannelIndex.ToLower(),
            StlSearch.AttributeChannelName.ToLower(),
            StlSearch.AttributeChannelIds.ToLower(),
            StlSearch.AttributeType.ToLower(),
            StlSearch.AttributeWord.ToLower(),
            StlSearch.AttributeDateAttribute.ToLower(),
            StlSearch.AttributeDateFrom.ToLower(),
            StlSearch.AttributeDateTo.ToLower(),
            StlSearch.AttributeSince.ToLower(),
            StlSearch.AttributePageNum.ToLower(),
            StlSearch.AttributeIsHighlight.ToLower(),
            StlSearch.AttributeIsDefaultDisplay.ToLower(),
            "publishmentsystemid",
            "ajaxdivid",
            "template",
        };
    }
}