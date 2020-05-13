using System.Collections.Specialized;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.DataCache.Content;
using SiteServer.CMS.Model;

namespace SiteServer.CMS.StlParser.Model
{
    public class ContextInfo
    {
        public ContextInfo(PageInfo pageInfo)
        {
            SiteInfo = pageInfo.SiteInfo;
            ChannelId = pageInfo.PageChannelId;
            ContentId = pageInfo.PageContentId;
        }

        //用于clone
        private ContextInfo(ContextInfo contextInfo)
        {
            ContextType = contextInfo.ContextType;
            SiteInfo = contextInfo.SiteInfo;
            ChannelId = contextInfo.ChannelId;
            ContentId = contextInfo.ContentId;
            _channelInfo = contextInfo._channelInfo;
            _contentInfo = contextInfo._contentInfo;

            IsInnerElement = contextInfo.IsInnerElement;
            IsStlEntity = contextInfo.IsStlEntity;
            PageItemIndex = contextInfo.PageItemIndex;
            ItemContainer = contextInfo.ItemContainer;
            ContainerClientId = contextInfo.ContainerClientId;

            OuterHtml = contextInfo.OuterHtml;
            InnerHtml = contextInfo.InnerHtml;
            Attributes = contextInfo.Attributes;
        }

        public ContextInfo Clone(string outerHtml, string innerHtml, NameValueCollection attributes)
        {
            var contextInfo = new ContextInfo(this)
            {
                OuterHtml = outerHtml,
                InnerHtml = innerHtml,
                Attributes = attributes
            };
            return contextInfo;
        }

        public ContextInfo Clone()
        {
            var contextInfo = new ContextInfo(this);
            return contextInfo;
        }

        public EContextType ContextType { get; set; } = EContextType.Undefined;

        public SiteInfo SiteInfo { get; set; }

        public int ChannelId { get; set; }

        public int ContentId { get; set; }

        public string OuterHtml { get; set; }

        public string InnerHtml { get; set; }

        public NameValueCollection Attributes { get; set; }

        private ChannelInfo _channelInfo;
        public ChannelInfo ChannelInfo
        {
            get
            {
                if (_channelInfo != null) return _channelInfo;
                if (ChannelId <= 0) return null;
                _channelInfo = ChannelManager.GetChannelInfo(SiteInfo.Id, ChannelId);
                return _channelInfo;
            }
            set { _channelInfo = value; }
        }

        private ContentInfo _contentInfo;
        public ContentInfo ContentInfo
        {
            get
            {
                if (_contentInfo != null) return _contentInfo;
                if (ContentId <= 0) return null;
                _contentInfo = ContentManager.GetContentInfo(SiteInfo, ChannelId, ContentId);
                return _contentInfo;
            }
            set { _contentInfo = value; }
        }

        public bool IsInnerElement { get; set; }

        public bool IsStlEntity { get; set; }

        public int PageItemIndex { get; set; }

        public DbItemContainer ItemContainer { get; set; }

        public string ContainerClientId { get; set; }
    }
}
