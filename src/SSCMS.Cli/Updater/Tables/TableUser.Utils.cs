using System.Collections.Generic;
using Datory;
using SSCMS;
using SSCMS.Models;

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
            ConvertValueDict = ConvertValueDict
        };

        private string NewTableName => _databaseManager.UserRepository.TableName;

        private List<TableColumn> NewColumns => _databaseManager.UserRepository.TableColumns;

        private static readonly Dictionary<string, string> ConvertKeyDict =
            new Dictionary<string, string>
            {
                {nameof(User.AvatarUrl), nameof(AvatarLarge)}
            };

        private static readonly Dictionary<string, string> ConvertValueDict = null;
    }
}
