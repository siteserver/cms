using System.Collections.Generic;
using Datory;

namespace SSCMS.Cli.Updater.Tables
{
    public partial class TableUserLog
    {
        public const string OldTableName = "bairong_UserLog";

        public ConvertInfo Converter => new ConvertInfo
        {
            NewTableName = NewTableName,
            NewColumns = NewColumns,
            ConvertKeyDict = null,
            ConvertValueDict = null
        };

        private string NewTableName => _databaseManager.UserLogRepository.TableName;

        private List<TableColumn> NewColumns => _databaseManager.UserLogRepository.TableColumns;
    }
}
