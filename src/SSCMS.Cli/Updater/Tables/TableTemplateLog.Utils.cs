using System.Collections.Generic;
using Datory;
using SSCMS.Models;

namespace SSCMS.Cli.Updater.Tables
{
    public partial class TableTemplateLog
    {
        public static readonly List<string> OldTableNames = new List<string>
        {
            "siteserver_TemplateLog",
            "wcm_TemplateLog"
        };

        public ConvertInfo Converter => new ConvertInfo
        {
            NewTableName = NewTableName,
            NewColumns = NewColumns,
            ConvertKeyDict = ConvertKeyDict,
            ConvertValueDict = ConvertValueDict
        };

        private string NewTableName => _databaseManager.TemplateLogRepository.TableName;

        private List<TableColumn> NewColumns => _databaseManager.TemplateLogRepository.TableColumns;

        private static readonly Dictionary<string, string[]> ConvertKeyDict =
            new Dictionary<string, string[]>
            {
                {nameof(TemplateLog.SiteId), new[] {nameof(PublishmentSystemId)}}
            };

        private static readonly Dictionary<string, string> ConvertValueDict = null;
    }
}
