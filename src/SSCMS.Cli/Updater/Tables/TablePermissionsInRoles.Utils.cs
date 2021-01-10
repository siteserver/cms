using System.Collections.Generic;
using Datory;

namespace SSCMS.Cli.Updater.Tables
{
    public partial class TablePermissionsInRoles
    {
        public const string OldTableName = "bairong_PermissionsInRoles";

        public ConvertInfo Converter => new ConvertInfo
        {
            NewTableName = NewTableName,
            NewColumns = NewColumns,
            ConvertKeyDict = ConvertKeyDict,
            ConvertValueDict = ConvertValueDict
        };

        private string NewTableName => _databaseManager.PermissionsInRolesRepository.TableName;

        private List<TableColumn> NewColumns => _databaseManager.PermissionsInRolesRepository.TableColumns;

        private static readonly Dictionary<string, string[]> ConvertKeyDict = null;

        private static readonly Dictionary<string, string> ConvertValueDict = null;
    }
}
