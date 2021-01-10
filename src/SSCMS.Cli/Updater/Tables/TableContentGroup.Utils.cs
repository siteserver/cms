using System.Collections.Generic;
using Datory;
using SSCMS.Models;

namespace SSCMS.Cli.Updater.Tables
{
    public partial class TableContentGroup
    {
        public static readonly List<string> OldTableNames = new List<string>
        {
            "siteserver_ContentGroup",
            "wcm_ContentGroup"
        };

        public ConvertInfo Converter => new ConvertInfo
        {
            NewTableName = NewTableName,
            NewColumns = NewColumns,
            ConvertKeyDict = ConvertKeyDict,
            ConvertValueDict = ConvertValueDict
        };

        private string NewTableName => _databaseManager.ContentGroupRepository.TableName;

        private List<TableColumn> NewColumns => _databaseManager.ContentGroupRepository.TableColumns;

        private static readonly Dictionary<string, string[]> ConvertKeyDict =
            new Dictionary<string, string[]>
            {
                {nameof(ContentGroup.GroupName), new[] {nameof(ContentGroupName)}},
                {nameof(ContentGroup.SiteId), new[] {nameof(PublishmentSystemId)}}
            };

        private static readonly Dictionary<string, string> ConvertValueDict = null;
    }
}
