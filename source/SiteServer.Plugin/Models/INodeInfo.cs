using System;

namespace SiteServer.Plugin.Models
{
    public interface INodeInfo
    {
        int NodeId { get; set; }

        string NodeName { get; set; }

        int PublishmentSystemId { get; set; }

        string ContentModelId { get; set; }

        int ParentId { get; set; }

        string ParentsPath { get; set; }

        int ParentsCount { get; set; }

        int ChildrenCount { get; set; }

        bool IsLastNode { get; set; }

        string NodeIndexName { get; set; }

        string NodeGroupNameCollection { get; set; }

        int Taxis { get; set; }

        DateTime AddDate { get; set; }

        string ImageUrl { get; set; }

        string Content { get; set; }

        int ContentNum { get; set; }

        string FilePath { get; set; }

        string ChannelFilePathRule { get; set; }

        string ContentFilePathRule { get; set; }

        string LinkUrl { get; set; }

        int ChannelTemplateId { get; set; }

        int ContentTemplateId { get; set; }

        string Keywords { get; set; }

        string Description { get; set; }

        ExtendedAttributes Attributes { get; }
    }
}
