using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;
using SiteServer.Plugin;

namespace SiteServer.Cli.Updater.Model41
{
    public partial class TableUser
    {
        [JsonProperty("userId")]
        public long UserId { get; set; }

        [JsonProperty("groupSN")]
        public string GroupSn { get; set; }

        [JsonProperty("userName")]
        public string UserName { get; set; }

        [JsonProperty("password")]
        public string Password { get; set; }

        [JsonProperty("passwordFormat")]
        public string PasswordFormat { get; set; }

        [JsonProperty("passwordSalt")]
        public string PasswordSalt { get; set; }

        [JsonProperty("groupId")]
        public long GroupId { get; set; }

        [JsonProperty("credits")]
        public long Credits { get; set; }

        [JsonProperty("createDate")]
        public DateTimeOffset CreateDate { get; set; }

        [JsonProperty("createIpAddress")]
        public string CreateIpAddress { get; set; }

        [JsonProperty("createUserName")]
        public string CreateUserName { get; set; }

        [JsonProperty("pointCount")]
        public long PointCount { get; set; }

        [JsonProperty("lastActivityDate")]
        public DateTimeOffset LastActivityDate { get; set; }

        [JsonProperty("isChecked")]
        public string IsChecked { get; set; }

        [JsonProperty("isLockedOut")]
        public string IsLockedOut { get; set; }

        [JsonProperty("isTemporary")]
        public string IsTemporary { get; set; }

        [JsonProperty("displayName")]
        public string DisplayName { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("mobile")]
        public string Mobile { get; set; }

        [JsonProperty("onlineSeconds")]
        public string OnlineSeconds { get; set; }

        [JsonProperty("avatarLarge")]
        public string AvatarLarge { get; set; }

        [JsonProperty("avatarMiddle")]
        public string AvatarMiddle { get; set; }

        [JsonProperty("avatarSmall")]
        public string AvatarSmall { get; set; }

        [JsonProperty("signature")]
        public string Signature { get; set; }

        [JsonProperty("settingsXml")]
        public string SettingsXml { get; set; }
    }

    public partial class TableUser
    {
        public const string OldTableName = "Users";

        public static ConvertInfo Converter => new ConvertInfo
        {
            NewTableName = NewTableName,
            NewColumns = NewColumns,
            ConvertKeyDict = ConvertKeyDict,
            ConvertValueDict = ConvertValueDict
        };

        private static readonly string NewTableName = DataProvider.UserDao.TableName;

        private static readonly List<TableColumn> NewColumns = DataProvider.UserDao.TableColumns;

        private static readonly Dictionary<string, string> ConvertKeyDict =
            new Dictionary<string, string>
            {
                {nameof(UserInfo.Id), nameof(UserId)},
                {nameof(UserInfo.AvatarUrl), nameof(AvatarLarge)}
            };

        private static readonly Dictionary<string, string> ConvertValueDict = null;
    }
}
