using System.Collections.Generic;
using Datory;
using SSCMS;
using SSCMS.Models;

namespace SSCMS.Cli.Updater.Tables
{
    public partial class TableContentCheck
    {
        public const string OldTableName = "bairong_ContentCheck";

        public ConvertInfo Converter => new ConvertInfo
        {
            NewTableName = NewTableName,
            NewColumns = NewColumns,
            ConvertKeyDict = ConvertKeyDict,
            ConvertValueDict = ConvertValueDict
        };

        private string NewTableName => _databaseManager.ContentCheckRepository.TableName;

        private List<TableColumn> NewColumns => _databaseManager.ContentCheckRepository.TableColumns;

        private static readonly Dictionary<string, string> ConvertKeyDict =
            new Dictionary<string, string>
            {
                {nameof(ContentCheck.Id), nameof(CheckId)},
                {nameof(ContentCheck.SiteId), nameof(PublishmentSystemId)},
                {nameof(ContentCheck.ChannelId), nameof(NodeId)}
            };

        private static readonly Dictionary<string, string> ConvertValueDict = null;
    }
}
