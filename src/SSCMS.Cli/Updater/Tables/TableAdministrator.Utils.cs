using System.Collections.Generic;
using Datory;
using SSCMS;

namespace SSCMS.Cli.Updater.Tables
{
    public partial class TableAdministrator
    {
        public const string OldTableName = "bairong_Administrator";

        public ConvertInfo Converter => new ConvertInfo
        {
            NewTableName = NewTableName,
            NewColumns = NewColumns,
            ConvertKeyDict = ConvertKeyDict,
            ConvertValueDict = ConvertValueDict
        };

        private string NewTableName => _databaseManager.AdministratorRepository.TableName;

        private List<TableColumn> NewColumns => _databaseManager.AdministratorRepository.TableColumns;

        private static readonly Dictionary<string, string> ConvertKeyDict =
            new Dictionary<string, string>
            {
                {nameof(Administrator.SiteId), nameof(PublishmentSystemId)}
            };

        private static readonly Dictionary<string, string> ConvertValueDict = null;
    }
}
