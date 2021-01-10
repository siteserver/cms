using System.Collections.Generic;
using Datory;
using SSCMS.Models;
using SSCMS.Utils;

namespace SSCMS.Cli.Updater.Tables
{
    public partial class TableContentCheck
    {
        public const string OldTableName = "bairong_ContentCheck";

        public ConvertInfo Converter => new ConvertInfo
        {
            NewTableName = NewTableName,
            NewColumns = NewColumns,
            ConvertKeyDict = ConvertKeyDict,
            ConvertValueDict = ConvertValueDict,
            Process = Process
        };

        private string NewTableName => _databaseManager.ContentCheckRepository.TableName;

        private List<TableColumn> NewColumns => _databaseManager.ContentCheckRepository.TableColumns;

        private static readonly Dictionary<string, string[]> ConvertKeyDict =
            new Dictionary<string, string[]>
            {
                {nameof(ContentCheck.Id), new[] {nameof(CheckId)}},
                {nameof(ContentCheck.SiteId), new[] {nameof(PublishmentSystemId)}},
                {nameof(ContentCheck.ChannelId), new[] {nameof(NodeId)}}
            };

        private static readonly Dictionary<string, string> ConvertValueDict = null;

        private static Dictionary<string, object> Process(Dictionary<string, object> row)
        {
            if (row.TryGetValue(nameof(IsChecked), out var contentObj))
            {
                var value = TranslateUtils.ToBool(contentObj.ToString());
                row[nameof(ContentCheck.Checked)] = value;
            }

            return row;
        }
    }
}
