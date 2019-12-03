using System.Collections.Specialized;
using System.Threading.Tasks;
using SiteServer.CMS.DataCache;
using SiteServer.Abstractions;
using SiteServer.CMS.Repositories;

namespace SiteServer.CMS.StlParser.Model
{
    public class ContextInfo
    {
        public ContextInfo(PageInfo pageInfo)
        {
            Site = pageInfo.Site;
            ChannelId = pageInfo.PageChannelId;
            ContentId = pageInfo.PageContentId;
        }

        //用于clone
        private ContextInfo(ContextInfo contextInfo)
        {
            ContextType = contextInfo.ContextType;
            Site = contextInfo.Site;
            ChannelId = contextInfo.ChannelId;
            ContentId = contextInfo.ContentId;
            _channel = contextInfo._channel;
            _content = contextInfo._content;

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

        public Site Site { get; set; }

        public int ChannelId { get; set; }

        public int ContentId { get; set; }

        public string OuterHtml { get; set; }

        public string InnerHtml { get; set; }

        public NameValueCollection Attributes { get; set; }

        private Channel _channel;
        public async Task<Channel> GetChannelAsync()
        {
            if (_channel != null) return _channel;
            if (ChannelId <= 0) return null;
            _channel = await ChannelManager.GetChannelAsync(Site.Id, ChannelId);
            return _channel;
        }
        public void SetChannel(Channel value)
        {
            _channel = value;
        }

        private Content _content;
        public async Task<Content> GetContentAsync()
        {
            if (_content != null) return _content;
            if (ContentId <= 0) return null;
            _content = await DataProvider.ContentRepository.GetAsync(Site, ChannelId, ContentId);
            return _content;
        }
        public void SetContentInfo(Content value)
        {
            _content = value;
        }

        public bool IsInnerElement { get; set; }

        public bool IsStlEntity { get; set; }

        public int PageItemIndex { get; set; }

        public DbItemContainer ItemContainer { get; set; }

        public string ContainerClientId { get; set; }
    }
}
