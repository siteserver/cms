using System.Collections.Generic;
using Datory;

namespace SSCMS.Cli.Updater.Tables
{
    public partial class TableRelatedFieldItem
    {
        public static readonly List<string> OldTableNames = new List<string>
        {
            "siteserver_RelatedFieldItem",
            "wcm_RelatedFieldItem"
        };

        public ConvertInfo Converter => new ConvertInfo
        {
            NewTableName = NewTableName,
            NewColumns = NewColumns,
            ConvertKeyDict = ConvertKeyDict,
            ConvertValueDict = ConvertValueDict
        };

        private string NewTableName => _databaseManager.RelatedFieldItemRepository.TableName;

        private List<TableColumn> NewColumns => _databaseManager.RelatedFieldItemRepository.TableColumns;

        private static readonly Dictionary<string, string[]> ConvertKeyDict = null;

        private static readonly Dictionary<string, string> ConvertValueDict = null;
    }
}
