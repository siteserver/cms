using System.Collections.Generic;
using System.Threading.Tasks;


namespace SS.CMS.Abstractions
{
    public partial interface IFileManager
    {
        Task TranslateAsync(ICreateManager createManager, Site siteInfo, int channelId, int contentId, string translateCollection, TranslateContentType translateType);

        Task TranslateAsync(ICreateManager createManager, Site siteInfo, int channelId, int contentId, int targetSiteId, int targetChannelId, TranslateContentType translateType);

        Task DeleteAsync(Site siteInfo, Channel channelInfo, int contentId);

        string TextEditorContentEncode(Site siteInfo, string content);

        string TextEditorContentDecode(Site siteInfo, string content, bool isLocal);

        Task DeleteContentsAsync(Site siteInfo, int channelId, IEnumerable<int> contentIdList);

        Task DeleteContentAsync(Site siteInfo, int channelId, int contentId);

        Task DeleteChannelsAsync(Site siteInfo, List<int> channelIdList);

        Task DeleteChannelsByPageAsync(Site siteInfo, List<int> channelIdList);

        void DeletePagingFiles(string filePath);

        Task DeleteFilesAsync(Site siteInfo, List<int> templateIdList);
    }
}