using System;
using System.Collections;
using BaiRong.Core.Model.Enumerations;
using SiteServer.CMS.Model.Enumerations;

namespace SiteServer.CMS.Model
{
    public class NodeAttribute
    {
        protected NodeAttribute()
        {
        }

        public const string NodeId = "NodeID";
        public const string NodeName = "NodeName";
        public const string NodeType = "NodeType";
        public const string PublishmentSystemId = "PublishmentSystemID";
        public const string ContentModelId = "ContentModelID";
        public const string ParentId = "ParentID";
        public const string ParentsPath = "ParentsPath";
        public const string ParentsCount = "ParentsCount";
        public const string ChildrenCount = "ChildrenCount";
        public const string IsLastNode = "IsLastNode";
        public const string NodeIndexName = "NodeIndexName";
        public const string NodeGroupNameCollection = "NodeGroupNameCollection";
        public const string Taxis = "Taxis";
        public const string AddDate = "AddDate";
        public const string ImageUrl = "ImageUrl";
        public const string Content = "Content";
        public const string ContentNum = "ContentNum";
        public const string FilePath = "FilePath";
        public const string ChannelFilePathRule = "ChannelFilePathRule";
        public const string ContentFilePathRule = "ContentFilePathRule";
        public const string LinkUrl = "LinkUrl";
        public const string LinkType = "LinkType";
        public const string ChannelTemplateId = "ChannelTemplateID";
        public const string ContentTemplateId = "ContentTemplateID";
        public const string Keywords = "Keywords";
        public const string Description = "Description";
        public const string ExtendValues = "ExtendValues";

        public static string Id = "ID";
        public static string Title = "Title";
        public static string ChannelName = "ChannelName";
        public static string ChannelIndex = "ChannelIndex";
        public static string ChannelGroupNameCollection = "ChannelGroupNameCollection";
        public const string PageContent = "PageContent";
        public const string CountOfChannels = "CountOfChannels";
        public const string CountOfContents = "CountOfContents";
        public const string CountOfImageContents = "CountOfImageContents";

        private static ArrayList _lowerDefaultAttributes;
        public static ArrayList LowerDefaultAttributes
        {
            get
            {
                if (_lowerDefaultAttributes != null) return _lowerDefaultAttributes;

                _lowerDefaultAttributes = new ArrayList
                {
                    NodeId.ToLower(),
                    NodeName.ToLower(),
                    NodeType.ToLower(),
                    PublishmentSystemId.ToLower(),
                    ParentId.ToLower(),
                    ParentsPath.ToLower(),
                    ParentsCount.ToLower(),
                    ChildrenCount.ToLower(),
                    IsLastNode.ToLower(),
                    NodeIndexName.ToLower(),
                    NodeGroupNameCollection.ToLower(),
                    Taxis.ToLower(),
                    AddDate.ToLower(),
                    ImageUrl.ToLower(),
                    Content.ToLower(),
                    ContentNum.ToLower(),
                    FilePath.ToLower(),
                    ChannelFilePathRule.ToLower(),
                    ContentFilePathRule.ToLower(),
                    LinkUrl.ToLower(),
                    LinkType.ToLower(),
                    ChannelTemplateId.ToLower(),
                    ContentTemplateId.ToLower(),
                    ExtendValues.ToLower()
                };

                return _lowerDefaultAttributes;
            }
        }
    }

	public class NodeInfo
	{
	    private string _contentModelId;
	    private string _extendValues;

		public NodeInfo()
		{
			NodeId = 0;
			NodeName = string.Empty;
			NodeType = ENodeType.BackgroundNormalNode;
			PublishmentSystemId = 0;
            _contentModelId = EContentModelTypeUtils.GetValue(EContentModelType.Content);
			ParentId = 0;
			ParentsPath = string.Empty;
			ParentsCount = 0;
			ChildrenCount = 0;
			IsLastNode = false;
			NodeIndexName = string.Empty;
			NodeGroupNameCollection = string.Empty;
			Taxis = 0;
			AddDate = DateTime.Now;
			ImageUrl = string.Empty;
			Content = string.Empty;
			ContentNum = 0;
            FilePath = string.Empty;
            ChannelFilePathRule = string.Empty;
            ContentFilePathRule = string.Empty;
            LinkUrl = string.Empty;
            LinkType = ELinkType.LinkNoRelatedToChannelAndContent;
            ChannelTemplateId = 0;
            ContentTemplateId = 0;
            Keywords = string.Empty;
            Description = string.Empty;
            _extendValues = string.Empty;
		}

        public NodeInfo(int nodeId, string nodeName, ENodeType nodeType, int publishmentSystemId, string contentModelId, int parentId, string parentsPath, int parentsCount, int childrenCount, bool isLastNode, string nodeIndexName, string nodeGroupNameCollection, int taxis, DateTime addDate, string imageUrl, string content, int contentNum, string filePath, string channelFilePathRule, string contentFilePathRule, string linkUrl, ELinkType linkType, int channelTemplateId, int contentTemplateId, string keywords, string description, string extendValues) 
		{
			NodeId = nodeId;
			NodeName = nodeName;
			NodeType = nodeType;
			PublishmentSystemId = publishmentSystemId;
            _contentModelId = contentModelId;
			ParentId = parentId;
			ParentsPath = parentsPath;
			ParentsCount = parentsCount;
			ChildrenCount = childrenCount;
			IsLastNode = isLastNode;
			NodeIndexName = nodeIndexName;
			NodeGroupNameCollection = nodeGroupNameCollection;
			Taxis = taxis;
			AddDate = addDate;
			ImageUrl = imageUrl;
			Content = content;
			ContentNum = contentNum;
            FilePath = filePath;
            ChannelFilePathRule = channelFilePathRule;
            ContentFilePathRule = contentFilePathRule;
            LinkUrl = linkUrl;
            LinkType = linkType;
            ChannelTemplateId = channelTemplateId;
            ContentTemplateId = contentTemplateId;
            Keywords = keywords;
            Description = description;
            _extendValues = extendValues;
		}

        public NodeInfo(NodeInfo nodeInfo)
        {
            NodeId = nodeInfo.NodeId;
            NodeName = nodeInfo.NodeName;
            NodeType = nodeInfo.NodeType;
            PublishmentSystemId = nodeInfo.PublishmentSystemId;
            _contentModelId = nodeInfo._contentModelId;
            ParentId = nodeInfo.ParentId;
            ParentsPath = nodeInfo.ParentsPath;
            ParentsCount = nodeInfo.ParentsCount;
            ChildrenCount = nodeInfo.ChildrenCount;
            IsLastNode = nodeInfo.IsLastNode;
            NodeIndexName = nodeInfo.NodeIndexName;
            NodeGroupNameCollection = nodeInfo.NodeGroupNameCollection;
            Taxis = nodeInfo.Taxis;
            AddDate = nodeInfo.AddDate;
            ImageUrl = nodeInfo.ImageUrl;
            Content = nodeInfo.Content;
            ContentNum = nodeInfo.ContentNum;
            FilePath = nodeInfo.FilePath;
            ChannelFilePathRule = nodeInfo.ChannelFilePathRule;
            ContentFilePathRule = nodeInfo.ContentFilePathRule;
            LinkUrl = nodeInfo.LinkUrl;
            LinkType = nodeInfo.LinkType;
            ChannelTemplateId = nodeInfo.ChannelTemplateId;
            ContentTemplateId = nodeInfo.ContentTemplateId;
            Keywords = nodeInfo.Keywords;
            Description = nodeInfo.Description;
            _extendValues = nodeInfo._extendValues;
        }

		public int NodeId { get; set; }

	    public string NodeName { get; set; }

	    public ENodeType NodeType { get; set; }

	    public int PublishmentSystemId { get; set; }

	    public string ContentModelId
        {
            get
            {
                if (string.IsNullOrEmpty(_contentModelId))
                {
                    _contentModelId = EContentModelTypeUtils.GetValue(EContentModelType.Content);
                }
                return _contentModelId;
            }
            set { _contentModelId = value; }
        }

		public int ParentId { get; set; }

	    public string ParentsPath { get; set; }

	    public int ParentsCount { get; set; }

	    public int ChildrenCount { get; set; }

	    public bool IsLastNode { get; set; }

	    public string NodeIndexName { get; set; }

	    public string NodeGroupNameCollection { get; set; }

	    public int Taxis { get; set; }

	    public DateTime AddDate { get; set; }

	    public string ImageUrl { get; set; }

	    public string Content { get; set; }

	    public int ContentNum { get; set; }

        public string FilePath { get; set; }

	    public string ChannelFilePathRule { get; set; }

	    public string ContentFilePathRule { get; set; }

	    public string LinkUrl { get; set; }

	    public ELinkType LinkType { get; set; }

	    public int ChannelTemplateId { get; set; }

	    public int ContentTemplateId { get; set; }

	    public string Keywords { get; set; }

	    public string Description { get; set; }

        public void SetExtendValues(string extendValues)
        {
            _extendValues = extendValues;
        }

        private NodeInfoExtend _additional;
        public NodeInfoExtend Additional => _additional ?? (_additional = new NodeInfoExtend(_extendValues));
	}
}
