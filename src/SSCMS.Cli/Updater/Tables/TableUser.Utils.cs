using System.Collections.Generic;
using Datory;
using SSCMS.Models;
using SSCMS.Utils;

namespace SSCMS.Cli.Updater.Tables
{
    public partial class TableUser
    {
        public const string OldTableName = "bairong_Users";

        public ConvertInfo Converter => new ConvertInfo
        {
            NewTableName = NewTableName,
            NewColumns = NewColumns,
            ConvertKeyDict = ConvertKeyDict,
            ConvertValueDict = ConvertValueDict,
            Process = Process
        };

        private string NewTableName => _databaseManager.UserRepository.TableName;

        private List<TableColumn> NewColumns => _databaseManager.UserRepository.TableColumns;

        private static readonly Dictionary<string, string[]> ConvertKeyDict =
            new Dictionary<string, string[]>
            {
                {nameof(User.Id), new []{nameof(UserId)}},
                {nameof(User.AvatarUrl), new []{nameof(AvatarLarge)}}
            };

        private static readonly Dictionary<string, string> ConvertValueDict = null;

        private static Dictionary<string, object> Process(Dictionary<string, object> row)
        {
            if (row.TryGetValue(nameof(IsChecked), out var contentObj))
            {
                var value = TranslateUtils.ToBool(contentObj.ToString());
                row[nameof(User.Checked)] = value;
            }
            if (row.TryGetValue(nameof(IsLockedOut), out contentObj))
            {
                var value = TranslateUtils.ToBool(contentObj.ToString());
                row[nameof(User.Locked)] = value;
            }

            return row;
        }
    }
}
