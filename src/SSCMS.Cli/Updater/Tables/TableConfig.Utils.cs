using System.Collections.Generic;
using Datory;

namespace SSCMS.Cli.Updater.Tables
{
    public partial class TableConfig
    {
        public const string OldTableName = "bairong_Config";

        public ConvertInfo Converter => new ConvertInfo
        {
            NewTableName = NewTableName,
            NewColumns = NewColumns,
            ConvertKeyDict = ConvertKeyDict,
            ConvertValueDict = ConvertValueDict
        };

        private string NewTableName => _databaseManager.ConfigRepository.TableName;

        private List<TableColumn> NewColumns => _databaseManager.ConfigRepository.TableColumns;

        private static readonly Dictionary<string, string[]> ConvertKeyDict =
            new Dictionary<string, string[]>
            {
                {"SystemConfig", new[] {nameof(SettingsXml)}}
            };

        private static readonly Dictionary<string, string> ConvertValueDict = null;
    }
}
