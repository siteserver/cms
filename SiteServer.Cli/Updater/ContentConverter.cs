using System;
using System.Collections.Generic;
using Datory;
using Newtonsoft.Json;
using SiteServer.Abstractions;

namespace SiteServer.Cli.Updater
{
    public partial class ContentConverter
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("nodeID")]
        public long NodeId { get; set; }

        [JsonProperty("publishmentSystemID")]
        public long PublishmentSystemId { get; set; }

        [JsonProperty("addUserName")]
        public string AddUserName { get; set; }

        [JsonProperty("lastEditUserName")]
        public string LastEditUserName { get; set; }

        [JsonProperty("lastEditDate")]
        public DateTimeOffset LastEditDate { get; set; }

        [JsonProperty("taxis")]
        public long Taxis { get; set; }

        [JsonProperty("contentGroupNameCollection")]
        public string ContentGroupNameCollection { get; set; }

        [JsonProperty("tags")]
        public string Tags { get; set; }

        [JsonProperty("sourceID")]
        public long SourceId { get; set; }

        [JsonProperty("referenceID")]
        public long ReferenceId { get; set; }

        [JsonProperty("isChecked")]
        public string IsChecked { get; set; }

        [JsonProperty("checkedLevel")]
        public long CheckedLevel { get; set; }

        [JsonProperty("comments")]
        public long Comments { get; set; }

        [JsonProperty("hits")]
        public long Hits { get; set; }

        [JsonProperty("hitsByDay")]
        public long HitsByDay { get; set; }

        [JsonProperty("hitsByWeek")]
        public long HitsByWeek { get; set; }

        [JsonProperty("hitsByMonth")]
        public long HitsByMonth { get; set; }

        [JsonProperty("lastHitsDate")]
        public DateTimeOffset LastHitsDate { get; set; }

        [JsonProperty("settingsXML")]
        public string SettingsXml { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("subTitle")]
        public string SubTitle { get; set; }

        [JsonProperty("imageUrl")]
        public string ImageUrl { get; set; }

        [JsonProperty("videoUrl")]
        public string VideoUrl { get; set; }

        [JsonProperty("fileUrl")]
        public string FileUrl { get; set; }

        [JsonProperty("linkUrl")]
        public string LinkUrl { get; set; }

        [JsonProperty("content")]
        public string Content { get; set; }

        [JsonProperty("summary")]
        public string Summary { get; set; }

        [JsonProperty("author")]
        public string Author { get; set; }

        [JsonProperty("source")]
        public string Source { get; set; }

        [JsonProperty("isRecommend")]
        public string IsRecommend { get; set; }

        [JsonProperty("isHot")]
        public string IsHot { get; set; }

        [JsonProperty("isColor")]
        public string IsColor { get; set; }

        [JsonProperty("isTop")]
        public string IsTop { get; set; }

        [JsonProperty("addDate")]
        public DateTimeOffset AddDate { get; set; }
    }

    public partial class ContentConverter
    {
        public static ConvertInfo GetSplitConverter()
        {
            return new ConvertInfo
            {
                NewColumns = GetNewColumns(null),
                ConvertKeyDict = ConvertKeyDict,
                ConvertValueDict = ConvertValueDict
            };
        }

        public static ConvertInfo GetConverter(string oldTableName, List<TableColumn> oldColumns)
        {
            return new ConvertInfo
            {
                NewTableName = oldTableName,
                NewColumns = GetNewColumns(oldColumns),
                ConvertKeyDict = ConvertKeyDict,
                ConvertValueDict = ConvertValueDict,
                Process = Process
            };
        }

        private static List<TableColumn> GetNewColumns(List<TableColumn> oldColumns)
        {
            var columns = new List<TableColumn>();
            var repository =
                new Repository<Content>(new Database(WebConfigUtils.DatabaseType, WebConfigUtils.ConnectionString));
            columns.AddRange(repository.TableColumns);

            if (oldColumns != null && oldColumns.Count > 0)
            {
                foreach (var tableColumnInfo in oldColumns)
                {
                    if (StringUtils.EqualsIgnoreCase(tableColumnInfo.AttributeName, nameof(NodeId)))
                    {
                        tableColumnInfo.AttributeName = nameof(Abstractions.Content.ChannelId);
                    }
                    else if (StringUtils.EqualsIgnoreCase(tableColumnInfo.AttributeName, nameof(PublishmentSystemId)))
                    {
                        tableColumnInfo.AttributeName = nameof(Abstractions.Content.SiteId);
                    }
                    else if (StringUtils.EqualsIgnoreCase(tableColumnInfo.AttributeName, nameof(ContentGroupNameCollection)))
                    {
                        tableColumnInfo.AttributeName = ContentAttribute.GroupNameCollection;
                    }

                    if (!columns.Exists(c => StringUtils.EqualsIgnoreCase(c.AttributeName, tableColumnInfo.AttributeName)))
                    {
                        columns.Add(tableColumnInfo);
                    }
                }
            }

            return columns;
        }

        private static readonly Dictionary<string, string> ConvertKeyDict =
            new Dictionary<string, string>
            {
                {nameof(Abstractions.Content.ChannelId), nameof(NodeId)},
                {nameof(Abstractions.Content.SiteId), nameof(PublishmentSystemId)},
                {ContentAttribute.GroupNameCollection, nameof(ContentGroupNameCollection)}
            };

        private static readonly Dictionary<string, string> ConvertValueDict = null;

        private static Dictionary<string, object> Process(Dictionary<string, object> row)
        {
            if (row.TryGetValue(ContentAttribute.Content, out var contentObj))
            {
                var content = contentObj.ToString();
                content = content.Replace("@upload", "@/upload");
                row[ContentAttribute.Content] = content;
            }
            if (row.TryGetValue(ContentAttribute.SettingsXml, out contentObj))
            {
                var content = contentObj.ToString();
                content = content.Replace("@upload", "@/upload");
                row[ContentAttribute.SettingsXml] = content;
            }

            return row;
        }
    }
}
