using BaiRong.Core.Model;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;

namespace SiteServer.CMS.StlParser.Model
{
    public class ContextInfo
    {
        private EContextType contextType = EContextType.Undefined;
        private PublishmentSystemInfo publishmentSystemInfo;
        private int channelID;
        private int contentID;
        private ContentInfo contentInfo;

        private bool isInnerElement;
        private bool isCurlyBrace;
        private int titleWordNum;
        private int totalNum;           //用于缓存列表内容总数
        private int pageItemIndex;
        private DbItemContainer itemContainer;
        private string containerClientID;

        public ContextInfo(PageInfo pageInfo)
        {
            publishmentSystemInfo = pageInfo.PublishmentSystemInfo;
            channelID = pageInfo.PageNodeId;
            contentID = pageInfo.PageContentId;
        }

        public ContextInfo(EContextType contextType, PublishmentSystemInfo publishmentSystemInfo, int channelID, int contentID, ContentInfo contentInfo)
        {
            this.contextType = contextType;
            this.publishmentSystemInfo = publishmentSystemInfo;
            this.channelID = channelID;
            this.contentID = contentID;
            this.contentInfo = contentInfo;
        }

        //用于clone
        private ContextInfo(ContextInfo contextInfo)
        {
            contextType = contextInfo.contextType;
            publishmentSystemInfo = contextInfo.publishmentSystemInfo;
            channelID = contextInfo.channelID;
            contentID = contextInfo.contentID;
            contentInfo = contextInfo.contentInfo;

            isInnerElement = contextInfo.isInnerElement;
            isCurlyBrace = contextInfo.isCurlyBrace;
            titleWordNum = contextInfo.titleWordNum;
            pageItemIndex = contextInfo.pageItemIndex;
            totalNum = contextInfo.totalNum;
            itemContainer = contextInfo.itemContainer;
            containerClientID = contextInfo.containerClientID;
        }

        public ContextInfo Clone()
        {
            var contextInfo = new ContextInfo(this);
            return contextInfo;
        }

        public EContextType ContextType
        {
            get { return contextType; }
            set { contextType = value; }
        }

        public PublishmentSystemInfo PublishmentSystemInfo
        {
            get { return publishmentSystemInfo; }
            set { publishmentSystemInfo = value; }
        }

        public int ChannelID
        {
            get { return channelID; }
            set { channelID = value; }
        }

        public int ContentID
        {
            get { return contentID; }
            set { contentID = value; }
        }

        public ContentInfo ContentInfo
        {
            get
            {
                if (contentInfo == null)
                {
                    if (contentID > 0)
                    {
                        var nodeInfo = NodeManager.GetNodeInfo(publishmentSystemInfo.PublishmentSystemId, channelID);
                        var tableStyle = NodeManager.GetTableStyle(publishmentSystemInfo, nodeInfo);
                        var tableName = NodeManager.GetTableName(publishmentSystemInfo, nodeInfo);
                        contentInfo = DataProvider.ContentDao.GetContentInfo(tableStyle, tableName, contentID);
                    }
                }
                return contentInfo;
            }
            set { contentInfo = value; }
        }

        public bool IsInnerElement
        {
            get { return isInnerElement; }
            set { isInnerElement = value; }
        }

        public bool IsCurlyBrace
        {
            get { return isCurlyBrace; }
            set { isCurlyBrace = value; }
        }

        public int TitleWordNum
        {
            get { return titleWordNum; }
            set { titleWordNum = value; }
        }

        public int TotalNum
        {
            get { return totalNum; }
            set { totalNum = value; }
        }

        public int PageItemIndex
        {
            get { return pageItemIndex; }
            set { pageItemIndex = value; }
        }

        public DbItemContainer ItemContainer
        {
            get { return itemContainer; }
            set { itemContainer = value; }
        }

        public string ContainerClientID
        {
            get { return containerClientID; }
            set { containerClientID = value; }
        }
    }
}
