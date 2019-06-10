using System;
using SS.CMS.Core.Cache.Content;
using SS.CMS.Core.Common;
using SS.CMS.Core.Models;

namespace SS.CMS.Core.Repositories
{
    public partial class ContentDao
    {
        public int Insert(SiteInfo siteInfo, ChannelInfo channelInfo, ContentInfo contentInfo)
        {
            var taxis = 0;
            if (contentInfo.SourceId == SourceManager.Preview)
            {
                channelInfo.IsPreviewContentsExists = true;
                DataProvider.ChannelDao.UpdateExtend(channelInfo);
            }
            else
            {
                taxis = GetTaxisToInsert(contentInfo.ChannelId, contentInfo.Top);
            }
            return InsertWithTaxis(siteInfo, channelInfo, contentInfo, taxis);
        }

        public int InsertPreview(SiteInfo siteInfo, ChannelInfo channelInfo, ContentInfo contentInfo)
        {
            channelInfo.IsPreviewContentsExists = true;
            DataProvider.ChannelDao.UpdateExtend(channelInfo);

            contentInfo.SourceId = SourceManager.Preview;
            return InsertWithTaxis(siteInfo, channelInfo, contentInfo, 0);
        }

        public int InsertWithTaxis(SiteInfo siteInfo, ChannelInfo channelInfo, ContentInfo contentInfo, int taxis)
        {
            if (siteInfo.IsAutoPageInTextEditor && !string.IsNullOrEmpty(contentInfo.Content))
            {
                contentInfo.Content = ContentUtility.GetAutoPageContent(contentInfo.Content, siteInfo.AutoPageWordNum);
            }
            contentInfo.Taxis = taxis;
            contentInfo.LastEditDate = DateTime.Now;
            contentInfo.SiteId = siteInfo.Id;
            contentInfo.ChannelId = channelInfo.Id;

            contentInfo.Id = _repository.Insert(contentInfo);

            ContentManager.InsertCache(siteInfo, channelInfo, contentInfo);

            return contentInfo.Id;
        }
    }
}
