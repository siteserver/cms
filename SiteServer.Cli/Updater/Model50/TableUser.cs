using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;
using SiteServer.Plugin;

namespace SiteServer.Cli.Updater.Model50
{
    public partial class TableUser
    {
        [JsonProperty("userId")]
        public long UserId { get; set; }

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

        [JsonProperty("createDate")]
        public DateTimeOffset CreateDate { get; set; }

        [JsonProperty("lastResetPasswordDate")]
        public DateTimeOffset LastResetPasswordDate { get; set; }

        [JsonProperty("lastActivityDate")]
        public DateTimeOffset LastActivityDate { get; set; }

        [JsonProperty("countOfLogin")]
        public long CountOfLogin { get; set; }

        [JsonProperty("countOfFailedLogin")]
        public long CountOfFailedLogin { get; set; }

        [JsonProperty("countOfWriting")]
        public long CountOfWriting { get; set; }

        [JsonProperty("isChecked")]
        public string IsChecked { get; set; }

        [JsonProperty("isLockedOut")]
        public string IsLockedOut { get; set; }

        [JsonProperty("displayName")]
        public string DisplayName { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("mobile")]
        public string Mobile { get; set; }

        [JsonProperty("avatarUrl")]
        public string AvatarUrl { get; set; }

        [JsonProperty("organization")]
        public string Organization { get; set; }

        [JsonProperty("department")]
        public string Department { get; set; }

        [JsonProperty("position")]
        public string Position { get; set; }

        [JsonProperty("gender")]
        public string Gender { get; set; }

        [JsonProperty("birthday")]
        public string Birthday { get; set; }

        [JsonProperty("education")]
        public string Education { get; set; }

        [JsonProperty("graduation")]
        public string Graduation { get; set; }

        [JsonProperty("address")]
        public string Address { get; set; }

        [JsonProperty("weiXin")]
        public string WeiXin { get; set; }

        [JsonProperty("qq")]
        public string Qq { get; set; }

        [JsonProperty("weiBo")]
        public string WeiBo { get; set; }

        [JsonProperty("interests")]
        public string Interests { get; set; }

        [JsonProperty("signature")]
        public string Signature { get; set; }

        [JsonProperty("extendValues")]
        public string ExtendValues { get; set; }
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
                {nameof(UserInfo.Id), nameof(UserId)}
            };

        private static readonly Dictionary<string, string> ConvertValueDict = null;
    }
}
