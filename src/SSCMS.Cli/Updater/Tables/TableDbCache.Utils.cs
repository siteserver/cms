using System.Collections.Generic;
using Datory;

namespace SSCMS.Cli.Updater.Tables
{
    public partial class TableDbCache
    {
        public const string OldTableName = "bairong_Cache";

        public ConvertInfo Converter => new ConvertInfo
        {
            NewTableName = NewTableName,
            NewColumns = NewColumns,
            ConvertKeyDict = ConvertKeyDict,
            ConvertValueDict = ConvertValueDict
        };

        private string NewTableName => _databaseManager.DbCacheRepository.TableName;

        private List<TableColumn> NewColumns => _databaseManager.DbCacheRepository.TableColumns;

        private static readonly Dictionary<string, string[]> ConvertKeyDict = null;

        private static readonly Dictionary<string, string> ConvertValueDict = null;
    }
}
