using System;
using Newtonsoft.Json;
using SSCMS.Services;

namespace SSCMS.Cli.Updater.Tables
{
    public partial class TableChannel
    {
        private readonly IDatabaseManager _databaseManager;

        public TableChannel(IDatabaseManager databaseManager)
        {
            _databaseManager = databaseManager;
        }

        [JsonProperty("nodeID")]
        public long NodeId { get; set; }

        [JsonProperty("nodeName")]
        public string NodeName { get; set; }

        [JsonProperty("nodeType")]
        public string NodeType { get; set; }

        [JsonProperty("publishmentSystemID")]
        public long PublishmentSystemId { get; set; }

        [JsonProperty("contentModelID")]
        public string ContentModelId { get; set; }

        [JsonProperty("parentID")]
        public long ParentId { get; set; }

        [JsonProperty("parentsPath")]
        public string ParentsPath { get; set; }

        [JsonProperty("parentsCount")]
        public long ParentsCount { get; set; }

        [JsonProperty("childrenCount")]
        public long ChildrenCount { get; set; }

        [JsonProperty("isLastNode")]
        public string IsLastNode { get; set; }

        [JsonProperty("nodeIndexName")]
        public string NodeIndexName { get; set; }

        [JsonProperty("nodeGroupNameCollection")]
        public string NodeGroupNameCollection { get; set; }

        [JsonProperty("groupNameCollection")]
        public string GroupNameCollection { get; set; }

        [JsonProperty("taxis")]
        public long Taxis { get; set; }

        [JsonProperty("addDate")]
        public DateTimeOffset AddDate { get; set; }

        [JsonProperty("imageUrl")]
        public string ImageUrl { get; set; }

        [JsonProperty("content")]
        public string Content { get; set; }

        [JsonProperty("contentNum")]
        public long ContentNum { get; set; }

        [JsonProperty("commentNum")]
        public long CommentNum { get; set; }

        [JsonProperty("filePath")]
        public string FilePath { get; set; }

        [JsonProperty("channelFilePathRule")]
        public string ChannelFilePathRule { get; set; }

        [JsonProperty("contentFilePathRule")]
        public string ContentFilePathRule { get; set; }

        [JsonProperty("linkUrl")]
        public string LinkUrl { get; set; }

        [JsonProperty("linkType")]
        public string LinkType { get; set; }

        [JsonProperty("channelTemplateID")]
        public long ChannelTemplateId { get; set; }

        [JsonProperty("contentTemplateID")]
        public long ContentTemplateId { get; set; }

        [JsonProperty("keywords")]
        public string Keywords { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("extendValues")]
        public string ExtendValues { get; set; }
    }
}
