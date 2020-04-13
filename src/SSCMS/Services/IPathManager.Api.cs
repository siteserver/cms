using System.Collections.Generic;
using SSCMS.Models;

namespace SSCMS.Services
{
    public partial interface IPathManager
    {
        string GetApiUrl(string route);

        string GetApiUrl(Config config);

        string InnerApiUrl { get; }

        string GetInnerApiUrl(string route);

        string GetDownloadApiUrl(string apiUrl, int siteId, int channelId, int contentId, string fileUrl);

        string GetDownloadApiUrl(string apiUrl, int siteId, string fileUrl);

        string GetDownloadApiUrl(string apiUrl, string filePath);

        string GetDynamicApiUrl(string apiUrl);

        string GetIfApiUrl(string apiUrl);

        string GetPageContentsApiUrl(string apiUrl);

        string GetPageContentsApiParameters(int siteId, int pageChannelId, int templateId, int totalNum, int pageCount,
            int currentPageIndex, string stlPageContentsElement);

        string GetSearchApiUrl(string apiUrl);

        string GetSearchApiParameters(bool isAllSites, string siteName, string siteDir, string siteIds, string channelIndex, string channelName, string channelIds, string type, string word, string dateAttribute, string dateFrom, string dateTo, string since, int pageNum, bool isHighlight, int siteId, string ajaxDivId, string template);

        List<string> GetSearchExlcudeAttributeNames { get; }

        string GetTriggerApiUrl(string apiUrl, int siteId, int channelId, int contentId,
            int fileTemplateId, bool isRedirect);
    }
}
