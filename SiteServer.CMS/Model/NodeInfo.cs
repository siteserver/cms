using System;
using SiteServer.CMS.Model.Enumerations;
using SiteServer.Plugin;

namespace SiteServer.CMS.Model
{
    public class NodeAttribute
    {
        protected NodeAttribute()
        {
        }

        public const string NodeId = nameof(NodeInfo.NodeId);
        public const string NodeName = nameof(NodeInfo.NodeName);
        public const string PublishmentSystemId = nameof(NodeInfo.PublishmentSystemId);
        public const string ContentModelPluginId = nameof(NodeInfo.ContentModelPluginId);
        public const string ContentRelatedPluginIds = nameof(NodeInfo.ContentRelatedPluginIds);
        public const string ParentId = nameof(NodeInfo.ParentId);
        public const string ParentsPath = nameof(NodeInfo.ParentsPath);
        public const string ParentsCount = nameof(NodeInfo.ParentsCount);
        public const string ChildrenCount = nameof(NodeInfo.ChildrenCount);
        public const string IsLastNode = nameof(NodeInfo.IsLastNode);
        public const string NodeIndexName = nameof(NodeInfo.NodeIndexName);
        public const string NodeGroupNameCollection = nameof(NodeInfo.NodeGroupNameCollection);
        public const string Taxis = nameof(NodeInfo.Taxis);
        public const string AddDate = nameof(NodeInfo.AddDate);
        public const string ImageUrl = nameof(NodeInfo.ImageUrl);
        public const string Content = nameof(NodeInfo.Content);
        public const string ContentNum = nameof(NodeInfo.ContentNum);
        public const string FilePath = nameof(NodeInfo.FilePath);
        public const string ChannelFilePathRule = nameof(NodeInfo.ChannelFilePathRule);
        public const string ContentFilePathRule = nameof(NodeInfo.ContentFilePathRule);
        public const string LinkUrl = nameof(NodeInfo.LinkUrl);
        public const string LinkType = nameof(NodeInfo.LinkType);
        public const string ChannelTemplateId = nameof(NodeInfo.ChannelTemplateId);
        public const string ContentTemplateId = nameof(NodeInfo.ContentTemplateId);
        public const string Keywords = nameof(NodeInfo.Keywords);
        public const string Description = nameof(NodeInfo.Description);
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
			PublishmentSystemId = 0;
            ContentModelPluginId = string.Empty;
		    ContentRelatedPluginIds = string.Empty;
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
            LinkType = string.Empty;
            ChannelTemplateId = 0;
            ContentTemplateId = 0;
            Keywords = string.Empty;
            Description = string.Empty;
            _extendValues = string.Empty;
		}

        public NodeInfo(int nodeId, string nodeName, int publishmentSystemId, string contentModelPluginId, string contentRelatedPluginIds, int parentId, string parentsPath, int parentsCount, int childrenCount, bool isLastNode, string nodeIndexName, string nodeGroupNameCollection, int taxis, DateTime addDate, string imageUrl, string content, int contentNum, string filePath, string channelFilePathRule, string contentFilePathRule, string linkUrl, ELinkType linkType, int channelTemplateId, int contentTemplateId, string keywords, string description, string extendValues) 
		{
			NodeId = nodeId;
			NodeName = nodeName;
			PublishmentSystemId = publishmentSystemId;
            ContentModelPluginId = contentModelPluginId;
		    ContentRelatedPluginIds = contentRelatedPluginIds;
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
            LinkType = ELinkTypeUtils.GetValue(linkType);
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
            PublishmentSystemId = nodeInfo.PublishmentSystemId;
            ContentModelPluginId = nodeInfo.ContentModelPluginId;
            ContentRelatedPluginIds = nodeInfo.ContentRelatedPluginIds;
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

	    public int PublishmentSystemId { get; set; }

        public string ContentModelPluginId { get; set; }

        public string ContentRelatedPluginIds { get; set; }

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

	    public string LinkType { get; set; }

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

        public IAttributes Attributes => Additional;
    }
}
