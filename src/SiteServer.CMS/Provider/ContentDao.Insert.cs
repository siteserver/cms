using System;
using System.Collections.Generic;
using System.Text;
using Datory;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.DataCache.Content;
using SiteServer.CMS.Model;
using SiteServer.CMS.Model.Attributes;
using Attr = SiteServer.CMS.Model.Attributes.ContentAttribute;

namespace SiteServer.CMS.Provider
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
