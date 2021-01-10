using System.Collections.Generic;
using Datory;
using SSCMS.Models;
using SSCMS.Utils;

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
            ConvertValueDict = ConvertValueDict,
            Process = Process
        };

        private string NewTableName => _databaseManager.AdministratorRepository.TableName;

        private List<TableColumn> NewColumns => _databaseManager.AdministratorRepository.TableColumns;

        private static readonly Dictionary<string, string[]> ConvertKeyDict =
            new Dictionary<string, string[]>
            {
                {nameof(Administrator.SiteId), new[]{nameof(PublishmentSystemId)}}
            };

        private static readonly Dictionary<string, string> ConvertValueDict = null;

        private static Dictionary<string, object> Process(Dictionary<string, object> row)
        {
            if (row.TryGetValue(nameof(IsLockedOut), out var contentObj))
            {
                var value = TranslateUtils.ToBool(contentObj.ToString());
                row[nameof(Administrator.Locked)] = value;
            }

            return row;
        }
    }
}
