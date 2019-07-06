using System.Collections.Generic;
using System.Threading.Tasks;
using SS.CMS.Enums;
using SS.CMS.Models;

namespace SS.CMS.Services
{
    public partial interface IFileManager
    {
        Task TranslateAsync(ICreateManager createManager, SiteInfo siteInfo, int channelId, int contentId, string translateCollection, TranslateContentType translateType);

        Task TranslateAsync(ICreateManager createManager, SiteInfo siteInfo, int channelId, int contentId, int targetSiteId, int targetChannelId, TranslateContentType translateType);

        Task DeleteAsync(SiteInfo siteInfo, ChannelInfo channelInfo, int contentId);

        string TextEditorContentEncode(SiteInfo siteInfo, string content);

        string TextEditorContentDecode(SiteInfo siteInfo, string content, bool isLocal);

        Task DeleteContentsAsync(SiteInfo siteInfo, int channelId, IList<int> contentIdList);

        Task DeleteContentAsync(SiteInfo siteInfo, int channelId, int contentId);

        Task DeleteChannelsAsync(SiteInfo siteInfo, List<int> channelIdList);

        Task DeleteChannelsByPageAsync(SiteInfo siteInfo, List<int> channelIdList);

        void DeletePagingFiles(string filePath);

        Task DeleteFilesAsync(SiteInfo siteInfo, List<int> templateIdList);
    }
}