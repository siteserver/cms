using System;
using SiteServer.CMS.Model.Enumerations;
using SiteServer.Plugin.Models;

namespace SiteServer.CMS.Model
{
    public class NodeAttribute
    {
        protected NodeAttribute()
        {
        }

        public const string NodeId = nameof(NodeId);
        public const string NodeName = nameof(NodeName);
        public const string NodeType = nameof(NodeType);
        public const string PublishmentSystemId = nameof(PublishmentSystemId);
        public const string ContentModelId = nameof(ContentModelId);
        public const string ParentId = nameof(ParentId);
        public const string ParentsPath = nameof(ParentsPath);
        public const string ParentsCount = nameof(ParentsCount);
        public const string ChildrenCount = nameof(ChildrenCount);
        public const string IsLastNode = nameof(IsLastNode);
        public const string NodeIndexName = nameof(NodeIndexName);
        public const string NodeGroupNameCollection = nameof(NodeGroupNameCollection);
        public const string Taxis = nameof(Taxis);
        public const string AddDate = nameof(AddDate);
        public const string ImageUrl = nameof(ImageUrl);
        public const string Content = nameof(Content);
        public const string ContentNum = nameof(ContentNum);
        public const string FilePath = nameof(FilePath);
        public const string ChannelFilePathRule = nameof(ChannelFilePathRule);
        public const string ContentFilePathRule = nameof(ContentFilePathRule);
        public const string LinkUrl = nameof(LinkUrl);
        public const string LinkType = nameof(LinkType);
        public const string ChannelTemplateId = nameof(ChannelTemplateId);
        public const string ContentTemplateId = nameof(ContentTemplateId);
        public const string Keywords = nameof(Keywords);
        public const string Description = nameof(Description);
        public const string ExtendValues = nameof(ExtendValues);

        public const string Id = nameof(Id);
        public const string Title = nameof(Title);
        public const string ChannelName = nameof(ChannelName);
        public const string ChannelIndex = nameof(ChannelIndex);
        public const string ChannelGroupNameCollection = nameof(ChannelGroupNameCollection);
        public const string PageContent = nameof(PageContent);
        public const string CountOfChannels = nameof(CountOfChannels);
        public const string CountOfContents = nameof(CountOfContents);
        public const string CountOfImageContents = nameof(CountOfImageContents);
    }

	public class NodeInfo : INodeInfo
    {
	    private string _extendValues;

		public NodeInfo()
		{
			NodeId = 0;
			NodeName = string.Empty;
			NodeType = ENodeType.BackgroundNormalNode;
			PublishmentSystemId = 0;
            ContentModelId = string.Empty;
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
            ContentModelId = contentModelId;
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
            ContentModelId = nodeInfo.ContentModelId;
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

	    public string ContentModelId { get; set; }

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

        public ExtendedAttributes Attributes => Additional;
    }
}
