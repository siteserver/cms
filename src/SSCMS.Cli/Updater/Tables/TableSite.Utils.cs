using System.Collections.Generic;
using Datory;
using SSCMS.Models;
using SSCMS.Utils;

namespace SSCMS.Cli.Updater.Tables
{
    public partial class TableSite
    {
        public static readonly List<string> OldTableNames = new List<string>
        {
            "siteserver_PublishmentSystem",
            "wcm_PublishmentSystem"
        };

        public ConvertInfo Converter => new ConvertInfo
        {
            NewTableName = NewTableName,
            NewColumns = NewColumns,
            ConvertKeyDict = ConvertKeyDict,
            ConvertValueDict = ConvertValueDict,
            Process = Process
        };

        private string NewTableName => _databaseManager.SiteRepository.TableName;

        private List<TableColumn> NewColumns => _databaseManager.SiteRepository.TableColumns;

        private static readonly Dictionary<string, string[]> ConvertKeyDict =
            new Dictionary<string, string[]>
            {
                //{nameof(Site.Id), nameof(PublishmentSystemId)},
                {nameof(Site.SiteName), new[] {nameof(PublishmentSystemName)}},
                {nameof(Site.TableName), new[] {nameof(AuxiliaryTableForContent)}},
                {nameof(Site.SiteDir), new[] {nameof(PublishmentSystemDir)}},
                {nameof(Site.ParentId), new[] {nameof(ParentPublishmentSystemId)}}
            };

        private static readonly Dictionary<string, string> ConvertValueDict = null;

        private static Dictionary<string, object> Process(Dictionary<string, object> row)
        {
            if (row.TryGetValue(nameof(IsHeadquarters), out var contentObj))
            {
                var value = TranslateUtils.ToBool(contentObj.ToString());
                row[nameof(Site.Root)] = value;
            }
            if (row.TryGetValue(nameof(IsRoot), out contentObj))
            {
                var value = TranslateUtils.ToBool(contentObj.ToString());
                row[nameof(Site.Root)] = value;
            }
            if (row.TryGetValue(nameof(PublishmentSystemId), out contentObj))
            {
                var value = TranslateUtils.ToInt(contentObj.ToString());
                row[nameof(Site.Id)] = value;
            }

            return row;
        }
    }
}
