using BaiRong.Core.Model;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;

namespace SiteServer.CMS.StlParser.Model
{
    public class ContextInfo
    {
        private ContentInfo _contentInfo;

        public ContextInfo(PageInfo pageInfo)
        {
            PublishmentSystemInfo = pageInfo.PublishmentSystemInfo;
            ChannelId = pageInfo.PageNodeId;
            ContentId = pageInfo.PageContentId;
        }

        public ContextInfo(EContextType contextType, PublishmentSystemInfo publishmentSystemInfo, int channelId, int contentId, ContentInfo contentInfo)
        {
            ContextType = contextType;
            PublishmentSystemInfo = publishmentSystemInfo;
            ChannelId = channelId;
            ContentId = contentId;
            _contentInfo = contentInfo;
        }

        //用于clone
        private ContextInfo(ContextInfo contextInfo)
        {
            ContextType = contextInfo.ContextType;
            PublishmentSystemInfo = contextInfo.PublishmentSystemInfo;
            ChannelId = contextInfo.ChannelId;
            ContentId = contextInfo.ContentId;
            _contentInfo = contextInfo._contentInfo;

            IsInnerElement = contextInfo.IsInnerElement;
            IsCurlyBrace = contextInfo.IsCurlyBrace;
            TitleWordNum = contextInfo.TitleWordNum;
            PageItemIndex = contextInfo.PageItemIndex;
            TotalNum = contextInfo.TotalNum;
            ItemContainer = contextInfo.ItemContainer;
            ContainerClientId = contextInfo.ContainerClientId;
        }

        public ContextInfo Clone()
        {
            var contextInfo = new ContextInfo(this);
            return contextInfo;
        }

        public EContextType ContextType { get; set; } = EContextType.Undefined;

        public PublishmentSystemInfo PublishmentSystemInfo { get; set; }

        public int ChannelId { get; set; }

        public int ContentId { get; set; }

        public ContentInfo ContentInfo
        {
            get
            {
                if (_contentInfo != null) return _contentInfo;
                if (ContentId <= 0) return _contentInfo;
                var nodeInfo = NodeManager.GetNodeInfo(PublishmentSystemInfo.PublishmentSystemId, ChannelId);
                var tableStyle = NodeManager.GetTableStyle(PublishmentSystemInfo, nodeInfo);
                var tableName = NodeManager.GetTableName(PublishmentSystemInfo, nodeInfo);
                _contentInfo = DataProvider.ContentDao.GetContentInfo(tableStyle, tableName, ContentId);
                return _contentInfo;
            }
            set { _contentInfo = value; }
        }

        public bool IsInnerElement { get; set; }

        public bool IsCurlyBrace { get; set; }

        public int TitleWordNum { get; set; }

        public int TotalNum { get; set; }

        public int PageItemIndex { get; set; }

        public DbItemContainer ItemContainer { get; set; }

        public string ContainerClientId { get; set; }
    }
}
