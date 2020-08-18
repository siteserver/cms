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

        private static readonly Dictionary<string, string> ConvertKeyDict =
            new Dictionary<string, string>
            {
                {nameof(TableStyle.Id), nameof(TableStyleId)},
                {nameof(TableStyle.TableName), nameof(TableName)}
            };

        private Dictionary<string, string> ConvertValueDict => new Dictionary<string, string>
        {
            {UpdateUtils.GetConvertValueDictKey(nameof(TableStyle.TableName), "siteserver_PublishmentSystem"), _databaseManager.SiteRepository.TableName},
            {UpdateUtils.GetConvertValueDictKey(nameof(TableStyle.TableName), "siteserver_Node"), _databaseManager.ChannelRepository.TableName}
        };

        private static Dictionary<string, object> Process(Dictionary<string, object> row)
        {
            if (row.TryGetValue("IsVisible", out var isVisible))
            {
                if (isVisible != null && StringUtils.EqualsIgnoreCase(isVisible.ToString(), "False"))
                {
                    row[nameof(TableStyle.InputType)] = Enums.InputType.Hidden.GetValue();
                }
            }

            return row;
        }
    }
}
