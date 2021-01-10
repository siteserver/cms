using System.Collections.Generic;
using Datory;
using SSCMS.Models;

namespace SSCMS.Cli.Updater.Tables
{
    public partial class TableRelatedField
    {
        public static readonly List<string> OldTableNames = new List<string>
        {
            "siteserver_RelatedField",
            "wcm_RelatedField"
        };

        public ConvertInfo Converter => new ConvertInfo
        {
            NewTableName = NewTableName,
            NewColumns = NewColumns,
            ConvertKeyDict = ConvertKeyDict,
            ConvertValueDict = ConvertValueDict
        };

        private string NewTableName => _databaseManager.RelatedFieldRepository.TableName;

        private List<TableColumn> NewColumns => _databaseManager.RelatedFieldRepository.TableColumns;

        private static readonly Dictionary<string, string[]> ConvertKeyDict =
            new Dictionary<string, string[]>
            {
                {nameof(RelatedField.Id), new[] {nameof(RelatedFieldId)}},
                {nameof(RelatedField.Title), new[] {nameof(RelatedFieldName)}},
                {nameof(RelatedField.SiteId), new[] {nameof(PublishmentSystemId)}}
            };

        private static readonly Dictionary<string, string> ConvertValueDict = null;
    }
}
