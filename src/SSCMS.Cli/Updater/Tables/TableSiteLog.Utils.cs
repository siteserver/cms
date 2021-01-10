using System.Collections.Generic;
using Datory;
using SSCMS.Models;

namespace SSCMS.Cli.Updater.Tables
{
    public partial class TableSiteLog
    {
        public static readonly List<string> OldTableNames = new List<string>
        {
            "siteserver_Log",
            "wcm_Log"
        };

        public ConvertInfo Converter => new ConvertInfo
        {
            NewTableName = NewTableName,
            NewColumns = NewColumns,
            ConvertKeyDict = ConvertKeyDict,
            ConvertValueDict = ConvertValueDict
        };

        private string NewTableName => _databaseManager.SiteLogRepository.TableName;

        private List<TableColumn> NewColumns => _databaseManager.SiteLogRepository.TableColumns;

        private static readonly Dictionary<string, string[]> ConvertKeyDict =
            new Dictionary<string, string[]>
            {
                {nameof(SiteLog.SiteId), new[] {nameof(PublishmentSystemId)}}
            };

        private static readonly Dictionary<string, string> ConvertValueDict = null;
    }
}
