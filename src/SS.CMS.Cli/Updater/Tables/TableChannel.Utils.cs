using System;
using System.Collections.Generic;
using System.Text;
using Datory;
using SS.CMS.Abstractions;
using SS.CMS.Core;

namespace SS.CMS.Cli.Updater.Tables
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

        private static readonly Dictionary<string, string> ConvertKeyDict =
            new Dictionary<string, string>
            {
                {nameof(Channel.Id), nameof(NodeId)},
                {nameof(Channel.ChannelName), nameof(NodeName)},
                {nameof(Channel.SiteId), nameof(PublishmentSystemId)},
                {nameof(Channel.IndexName), nameof(NodeIndexName)},
                {"GroupNameCollection", nameof(NodeGroupNameCollection)},
                {nameof(Channel.ContentModelPluginId), nameof(ContentModelId)}
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
