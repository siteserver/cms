using System.Collections.Generic;
using Datory;
using SSCMS.Models;

namespace SSCMS.Cli.Updater.Tables
{
    public partial class TableChannel
    {
        public static readonly List<string> OldTableNames = new List<string>
        {
            "siteserver_Node",
            "wcm_Node"
        };

        public ConvertInfo Converter => new ConvertInfo
        {
            NewTableName = NewTableName,
            NewColumns = NewColumns,
            ConvertKeyDict = ConvertKeyDict,
            ConvertValueDict = ConvertValueDict,
            Process = Process
        };

        private string NewTableName => _databaseManager.ChannelRepository.TableName;

        private List<TableColumn> NewColumns => _databaseManager.ChannelRepository.TableColumns;

        private static readonly Dictionary<string, string[]> ConvertKeyDict =
            new Dictionary<string, string[]>
            {
                {nameof(Channel.Id), new[] {nameof(NodeId)}},
                {nameof(Channel.ChannelName), new[] {nameof(NodeName)}},
                {nameof(Channel.SiteId), new[] {nameof(PublishmentSystemId)}},
                {nameof(Channel.IndexName), new[] {nameof(NodeIndexName)}},
                {
                    nameof(Channel.GroupNames),
                    new[] { nameof(GroupNameCollection), nameof(NodeGroupNameCollection) }
                },
                {nameof(Channel.ContentModelPluginId), new[] {nameof(ContentModelId)}}
            };

        private static readonly Dictionary<string, string> ConvertValueDict = new Dictionary<string, string>
        {
            {UpdateUtils.GetConvertValueDictKey(nameof(Channel.ContentModelPluginId), "GovInteract"), "SS.GovInteract"},
            {UpdateUtils.GetConvertValueDictKey(nameof(Channel.ContentModelPluginId), "GovPublic"), "SS.GovPublic"},
            {UpdateUtils.GetConvertValueDictKey(nameof(Channel.ContentModelPluginId), "Job"), "SS.Jobs"},
        };

        private static Dictionary<string, object> Process(Dictionary<string, object> row)
        {
            if (row.TryGetValue(nameof(Channel.Content), out var contentObj))
            {
                var content = contentObj.ToString();
                content = content.Replace("@upload", "@/upload");
                row[nameof(Channel.Content)] = content;
            }

            return row;
        }
    }
}
