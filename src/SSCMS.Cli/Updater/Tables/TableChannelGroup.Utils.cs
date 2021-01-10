using System.Collections.Generic;
using Datory;
using SSCMS.Models;

namespace SSCMS.Cli.Updater.Tables
{
    public partial class TableChannelGroup
    {
        public static readonly List<string> OldTableNames = new List<string>
        {
            "siteserver_NodeGroup",
            "wcm_NodeGroup"
        };

        public ConvertInfo Converter => new ConvertInfo
        {
            NewTableName = NewTableName,
            NewColumns = NewColumns,
            ConvertKeyDict = ConvertKeyDict,
            ConvertValueDict = ConvertValueDict
        };

        private string NewTableName => _databaseManager.ChannelGroupRepository.TableName;

        private List<TableColumn> NewColumns => _databaseManager.ChannelGroupRepository.TableColumns;

        private static readonly Dictionary<string, string[]> ConvertKeyDict =
            new Dictionary<string, string[]>
            {
                {nameof(ChannelGroup.GroupName), new []{nameof(NodeGroupName)}},
                {nameof(ChannelGroup.SiteId), new []{nameof(PublishmentSystemId)}}
            };

        private static readonly Dictionary<string, string> ConvertValueDict = null;
    }
}
