using System.Collections.Generic;
using Datory;

namespace SSCMS.Cli.Updater.Tables
{
    public partial class TableRole
    {
        public const string OldTableName = "bairong_Roles";

        public ConvertInfo Converter => new ConvertInfo
        {
            NewTableName = NewTableName,
            NewColumns = NewColumns,
            ConvertKeyDict = ConvertKeyDict,
            ConvertValueDict = ConvertValueDict
        };

        private string NewTableName => _databaseManager.RoleRepository.TableName;

        private List<TableColumn> NewColumns => _databaseManager.RoleRepository.TableColumns;

        private static readonly Dictionary<string, string[]> ConvertKeyDict = null;

        private static readonly Dictionary<string, string> ConvertValueDict = null;
    }
}
