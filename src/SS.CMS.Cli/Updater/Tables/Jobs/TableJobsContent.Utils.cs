using System.Collections.Generic;
using Datory;
using SS.CMS.Abstractions;

namespace SS.CMS.Cli.Updater.Tables.Jobs
{
    public partial class TableJobsContent
    {
        public static readonly string NewTableName = "ss_jobs";

        private static List<TableColumn> NewColumns => new List<TableColumn>
        {
            new TableColumn
            {
                AttributeName = "Department",
                DataType = DataType.VarChar,
                DataLength = 200
            },
            new TableColumn
            {
                AttributeName = "Location",
                DataType = DataType.VarChar,
                DataLength = 200
            },
            new TableColumn
            {
                AttributeName = "NumberOfPeople",
                DataType = DataType.VarChar,
                DataLength = 200
            },
            new TableColumn
            {
                AttributeName = "Responsibility",
                DataType = DataType.Text
            },
            new TableColumn
            {
                AttributeName = "Requirement",
                DataType = DataType.Text
            }
        };

        private List<TableColumn> GetNewColumns(List<TableColumn> oldColumns)
        {
            var columns = new List<TableColumn>();
            var repository =
                new Repository<Content>(_settingsManager.Database);
            columns.AddRange(repository.TableColumns);
            columns.AddRange(NewColumns);

            foreach (var tableColumnInfo in oldColumns)
            {
                if (StringUtils.EqualsIgnoreCase(tableColumnInfo.AttributeName, nameof(NodeId)))
                {
                    tableColumnInfo.AttributeName = nameof(SS.CMS.Abstractions.Content.ChannelId);
                }
                else if (StringUtils.EqualsIgnoreCase(tableColumnInfo.AttributeName, nameof(PublishmentSystemId)))
                {
                    tableColumnInfo.AttributeName = nameof(SS.CMS.Abstractions.Content.SiteId);
                }
                else if (StringUtils.EqualsIgnoreCase(tableColumnInfo.AttributeName, nameof(ContentGroupNameCollection)))
                {
                    tableColumnInfo.AttributeName = nameof(SS.CMS.Abstractions.Content.GroupNames);
                }
                else if (StringUtils.EqualsIgnoreCase(tableColumnInfo.AttributeName, nameof(GroupNameCollection)))
                {
                    tableColumnInfo.AttributeName = nameof(SS.CMS.Abstractions.Content.GroupNames);
                }

                if (!columns.Exists(c => StringUtils.EqualsIgnoreCase(c.AttributeName, tableColumnInfo.AttributeName)))
                {
                    columns.Add(tableColumnInfo);
                }
            }

            return columns;
        }

        public ConvertInfo GetConverter(List<TableColumn> oldColumns)
        {
            return new ConvertInfo
            {
                NewTableName = NewTableName,
                NewColumns = GetNewColumns(oldColumns),
                ConvertKeyDict = ConvertKeyDict,
                ConvertValueDict = ConvertValueDict
            };
        }

        private static readonly Dictionary<string, string> ConvertKeyDict =
            new Dictionary<string, string>
            {
                {nameof(SS.CMS.Abstractions.Content.ChannelId), nameof(NodeId)},
                {nameof(SS.CMS.Abstractions.Content.SiteId), nameof(PublishmentSystemId)},
                {nameof(SS.CMS.Abstractions.Content.GroupNames), nameof(ContentGroupNameCollection)},
                {nameof(SS.CMS.Abstractions.Content.GroupNames), nameof(GroupNameCollection)}
            };

        private static readonly Dictionary<string, string> ConvertValueDict = null;
    }
}
