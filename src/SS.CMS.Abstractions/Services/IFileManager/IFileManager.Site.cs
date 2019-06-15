using System.Collections.Generic;
using SS.CMS.Abstractions.Enums;
using SS.CMS.Abstractions.Models;

namespace SS.CMS.Abstractions.Services
{
    public partial interface IFileManager
    {
        void Translate(ICreateManager createManager, SiteInfo siteInfo, int channelId, int contentId, string translateCollection, TranslateContentType translateType);

        void Translate(ICreateManager createManager, SiteInfo siteInfo, int channelId, int contentId, int targetSiteId, int targetChannelId, TranslateContentType translateType);

        void Delete(SiteInfo siteInfo, ChannelInfo channelInfo, int contentId);

        string TextEditorContentEncode(SiteInfo siteInfo, string content);

        string TextEditorContentDecode(SiteInfo siteInfo, string content, bool isLocal);

        void DeleteContents(SiteInfo siteInfo, int channelId, IList<int> contentIdList);

        void DeleteContent(SiteInfo siteInfo, int channelId, int contentId);

        void DeleteChannels(SiteInfo siteInfo, List<int> channelIdList);

        void DeleteChannelsByPage(SiteInfo siteInfo, List<int> channelIdList);

        void DeletePagingFiles(string filePath);

        void DeleteFiles(SiteInfo siteInfo, List<int> templateIdList);
    }
}