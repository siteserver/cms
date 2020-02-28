using System;
using System.Collections.Generic;
using System.Text;
using Datory;
using SS.CMS.Core;

namespace SS.CMS.Cli.Updater.Tables
{
    public partial class TableAdministratorsInRoles
    {
        public const string OldTableName = "bairong_AdministratorsInRoles";

        public ConvertInfo Converter => new ConvertInfo
        {
            NewTableName = NewTableName,
            NewColumns = NewColumns,
            ConvertKeyDict = ConvertKeyDict,
            ConvertValueDict = ConvertValueDict
        };

        private string NewTableName => _databaseManager.AdministratorsInRolesRepository.TableName;

        private List<TableColumn> NewColumns => _databaseManager.AdministratorsInRolesRepository.TableColumns;

        private static readonly Dictionary<string, string> ConvertKeyDict = null;

        private static readonly Dictionary<string, string> ConvertValueDict = null;
    }
}
