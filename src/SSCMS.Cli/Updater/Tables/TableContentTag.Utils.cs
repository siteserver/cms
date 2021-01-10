using System.Collections.Generic;
using Datory;
using SSCMS.Models;

namespace SSCMS.Cli.Updater.Tables
{
    public partial class TableContentTag
    {
        public static readonly List<string> OldTableNames = new List<string>
        {
            "bairong_Tags",
            "siteserver_Tag"
        };

        public ConvertInfo Converter => new ConvertInfo
        {
            NewTableName = NewTableName,
            NewColumns = NewColumns,
            ConvertKeyDict = ConvertKeyDict,
            ConvertValueDict = ConvertValueDict
        };

        private string NewTableName => _databaseManager.ContentTagRepository.TableName;

        private List<TableColumn> NewColumns => _databaseManager.ContentTagRepository.TableColumns;

        private static readonly Dictionary<string, string[]> ConvertKeyDict =
            new Dictionary<string, string[]>
            {
                {nameof(ContentTag.Id), new[] {nameof(TagId)}},
                {nameof(ContentTag.SiteId), new[] {nameof(PublishmentSystemId)}},
                {nameof(ContentTag.ContentIds), new[] {nameof(ContentIdCollection)}},
                {nameof(ContentTag.TagName), new[] {nameof(Tag)}}
            };

        private static readonly Dictionary<string, string> ConvertValueDict = null;
    }
}
