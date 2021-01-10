using System.Collections.Generic;
using Datory;
using SSCMS.Models;
using SSCMS.Utils;

namespace SSCMS.Cli.Updater.Tables
{
    public partial class TableTemplate
    {
        public static readonly List<string> OldTableNames = new List<string>
        {
            "siteserver_Template",
            "wcm_Template"
        };

        public ConvertInfo Converter => new ConvertInfo
        {
            NewTableName = NewTableName,
            NewColumns = NewColumns,
            ConvertKeyDict = ConvertKeyDict,
            ConvertValueDict = ConvertValueDict,
            Process = Process
        };

        private string NewTableName => _databaseManager.TemplateRepository.TableName;

        private List<TableColumn> NewColumns => _databaseManager.TemplateRepository.TableColumns;

        private static readonly Dictionary<string, string[]> ConvertKeyDict =
            new Dictionary<string, string[]>
            {
                {nameof(Template.Id), new[] {nameof(TemplateId)}},
                {nameof(Template.SiteId), new[] {nameof(PublishmentSystemId)}}
            };

        private static readonly Dictionary<string, string> ConvertValueDict = null;

        private static Dictionary<string, object> Process(Dictionary<string, object> row)
        {
            if (row.TryGetValue(nameof(IsDefault), out var contentObj))
            {
                var value = TranslateUtils.ToBool(contentObj.ToString());
                row[nameof(Template.DefaultTemplate)] = value;
            }

            return row;
        }
    }
}
