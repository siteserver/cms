using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;

namespace SiteServer.Cli.Updater.Model50
{
    public partial class TableChannel
    {
        [JsonProperty("nodeId")]
        public long NodeId { get; set; }

        [JsonProperty("nodeName")]
        public string NodeName { get; set; }

        [JsonProperty("nodeType")]
        public string NodeType { get; set; }

        [JsonProperty("publishmentSystemId")]
        public long PublishmentSystemId { get; set; }

        [JsonProperty("contentModelId")]
        public string ContentModelId { get; set; }

        [JsonProperty("parentId")]
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

        [JsonProperty("channelTemplateId")]
        public long ChannelTemplateId { get; set; }

        [JsonProperty("contentTemplateId")]
        public long ContentTemplateId { get; set; }

        [JsonProperty("keywords")]
        public string Keywords { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("extendValues")]
        public string ExtendValues { get; set; }
    }

    public partial class TableChannel
    {
        public const string OldTableName = "Node";

        public static readonly string NewTableName = DataProvider.ChannelDao.TableName;

        public static readonly List<TableColumnInfo> NewColumns = DataProvider.ChannelDao.TableColumns;

        public static readonly Dictionary<string, string> ConvertDict =
            new Dictionary<string, string>
            {
                {nameof(ChannelInfo.Id), nameof(NodeId)},
                {nameof(ChannelInfo.ChannelName), nameof(NodeName)},
                {nameof(ChannelInfo.SiteId), nameof(PublishmentSystemId)},
                {nameof(ChannelInfo.IndexName), nameof(NodeIndexName)},
                {nameof(ChannelInfo.GroupNameCollection), nameof(NodeGroupNameCollection)}
            };
    }
}
