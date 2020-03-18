using System.Collections.Generic;
using Datory;
using SSCMS.Abstractions;

namespace SSCMS.Cli.Updater
{
    public partial class TableContentConverter
    {
        public ConvertInfo GetSplitConverter()
        {
            return new ConvertInfo
            {
                NewColumns = GetNewColumns(null),
                ConvertKeyDict = ConvertKeyDict,
                ConvertValueDict = ConvertValueDict
            };
        }

        public ConvertInfo GetConverter(string oldTableName, List<TableColumn> oldColumns)
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

        private List<TableColumn> GetNewColumns(List<TableColumn> oldColumns)
        {
            var columns = new List<TableColumn>();
            var repository =
                new Repository<Content>(_settingsManager.Database);
            columns.AddRange(repository.TableColumns);

            if (oldColumns != null && oldColumns.Count > 0)
            {
                foreach (var tableColumnInfo in oldColumns)
                {
                    if (StringUtils.EqualsIgnoreCase(tableColumnInfo.AttributeName, nameof(NodeId)))
                    {
                        tableColumnInfo.AttributeName = nameof(SSCMS.Abstractions.Content.ChannelId);
                    }
                    else if (StringUtils.EqualsIgnoreCase(tableColumnInfo.AttributeName, nameof(PublishmentSystemId)))
                    {
                        tableColumnInfo.AttributeName = nameof(SSCMS.Abstractions.Content.SiteId);
                    }
                    else if (StringUtils.EqualsIgnoreCase(tableColumnInfo.AttributeName, nameof(ContentGroupNameCollection)))
                    {
                        tableColumnInfo.AttributeName = nameof(SSCMS.Abstractions.Content.GroupNames);
                    }
                    else if (StringUtils.EqualsIgnoreCase(tableColumnInfo.AttributeName, nameof(GroupNameCollection)))
                    {
                        tableColumnInfo.AttributeName = nameof(SSCMS.Abstractions.Content.GroupNames);
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
                {nameof(SSCMS.Abstractions.Content.ChannelId), nameof(NodeId)},
                {nameof(SSCMS.Abstractions.Content.SiteId), nameof(PublishmentSystemId)},
                {nameof(SSCMS.Abstractions.Content.GroupNames), nameof(ContentGroupNameCollection)},
                {nameof(SSCMS.Abstractions.Content.GroupNames), nameof(GroupNameCollection)}
            };

        private static readonly Dictionary<string, string> ConvertValueDict = null;

        private static Dictionary<string, object> Process(Dictionary<string, object> row)
        {
            if (row.TryGetValue(nameof(Content), out var contentObj))
            {
                var content = contentObj.ToString();
                content = content.Replace("@upload", "@/upload");
                row[nameof(SSCMS.Abstractions.Content.Body)] = content;
            }
            if (row.TryGetValue(nameof(SettingsXml), out contentObj))
            {
                var content = contentObj.ToString();
                content = content.Replace("@upload", "@/upload");
                row["ExtendValues"] = content;
            }

            return row;
        }
    }
}
