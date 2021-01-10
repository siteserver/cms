using System.Collections.Generic;
using Datory;
using SSCMS.Models;
using SSCMS.Utils;

namespace SSCMS.Cli.Updater.Tables
{
    public partial class TableTableStyle
    {
        public const string OldTableName = "bairong_TableStyle";

        public ConvertInfo Converter => new ConvertInfo
        {
            NewTableName = NewTableName,
            NewColumns = NewColumns,
            ConvertKeyDict = ConvertKeyDict,
            ConvertValueDict = ConvertValueDict,
            Process = Process
        };

        private string NewTableName => _databaseManager.TableStyleRepository.TableName;

        private List<TableColumn> NewColumns => _databaseManager.TableStyleRepository.TableColumns;

        private static readonly Dictionary<string, string[]> ConvertKeyDict =
            new Dictionary<string, string[]>
            {
                {nameof(TableStyle.Id), new[] {nameof(TableStyleId)}}
            };

        private Dictionary<string, string> ConvertValueDict => new Dictionary<string, string>
        {
            {UpdateUtils.GetConvertValueDictKey(nameof(TableStyle.TableName), "siteserver_PublishmentSystem"), _databaseManager.SiteRepository.TableName},
            {UpdateUtils.GetConvertValueDictKey(nameof(TableStyle.TableName), "siteserver_Node"), _databaseManager.ChannelRepository.TableName}
        };

        private Dictionary<string, object> Process(Dictionary<string, object> row)
        {
            if (row.TryGetValue(nameof(IsVisible), out var contentObj))
            {
                if (contentObj != null && StringUtils.EqualsIgnoreCase(contentObj.ToString(), "False"))
                {
                    row[nameof(TableStyle.InputType)] = Enums.InputType.Hidden.GetValue();
                }
            }
            if (row.TryGetValue(nameof(IsVisibleInList), out contentObj))
            {
                var value = TranslateUtils.ToBool(contentObj.ToString());
                row[nameof(TableStyle.List)] = value;
            }
            if (row.TryGetValue(nameof(IsHorizontal), out contentObj))
            {
                var value = TranslateUtils.ToBool(contentObj.ToString());
                row[nameof(TableStyle.Horizontal)] = value;
            }

            return row;
        }
    }
}
