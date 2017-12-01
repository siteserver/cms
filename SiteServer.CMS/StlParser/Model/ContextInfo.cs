using System.Collections.Generic;
using System.Xml;
using BaiRong.Core.Model;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;
using SiteServer.CMS.StlParser.Cache;

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
            PageItemIndex = contextInfo.PageItemIndex;
            ItemContainer = contextInfo.ItemContainer;
            ContainerClientId = contextInfo.ContainerClientId;

            StlElement = contextInfo.StlElement;
            Attributes = contextInfo.Attributes;
            InnerXml = contextInfo.InnerXml;
            ChildNodes = contextInfo.ChildNodes;
        }

        public ContextInfo Clone(string stlElement, Dictionary<string, string> attributes, string innerXml, XmlNodeList childNodes)
        {
            var contextInfo = new ContextInfo(this)
            {
                StlElement = stlElement,
                Attributes = attributes,
                InnerXml = innerXml,
                ChildNodes = childNodes
            };
            return contextInfo;
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

        public string StlElement { get; set; }

        public Dictionary<string, string> Attributes { get; set; }

        public string InnerXml { get; set; }

        public XmlNodeList ChildNodes { get; set; }

        public ContentInfo ContentInfo
        {
            get
            {
                if (_contentInfo != null) return _contentInfo;
                if (ContentId <= 0) return _contentInfo;
                var nodeInfo = NodeManager.GetNodeInfo(PublishmentSystemInfo.PublishmentSystemId, ChannelId);
                var tableStyle = NodeManager.GetTableStyle(PublishmentSystemInfo, nodeInfo);
                var tableName = NodeManager.GetTableName(PublishmentSystemInfo, nodeInfo);
                //_contentInfo = DataProvider.ContentDao.GetContentInfo(tableStyle, tableName, ContentId);
                _contentInfo = Content.GetContentInfo(tableStyle, tableName, ContentId);
                return _contentInfo;
            }
            set { _contentInfo = value; }
        }

        public bool IsInnerElement { get; set; }

        public bool IsCurlyBrace { get; set; }

        public int PageItemIndex { get; set; }

        public DbItemContainer ItemContainer { get; set; }

        public string ContainerClientId { get; set; }
    }
}
