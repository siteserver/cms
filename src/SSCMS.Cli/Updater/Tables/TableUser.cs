using System;
using Newtonsoft.Json;
using SSCMS.Services;

namespace SSCMS.Cli.Updater.Tables
{
    public partial class TableUser
    {
        private readonly IDatabaseManager _databaseManager;

        public TableUser(IDatabaseManager databaseManager)
        {
            _databaseManager = databaseManager;
        }

        [JsonProperty("UserId")]
        public long UserId { get; set; }

        [JsonProperty("userName")]
        public string UserName { get; set; }

        [JsonProperty("password")]
        public string Password { get; set; }

        [JsonProperty("passwordFormat")]
        public string PasswordFormat { get; set; }

        [JsonProperty("passwordSalt")]
        public string PasswordSalt { get; set; }

        [JsonProperty("createDate")]
        public DateTimeOffset CreateDate { get; set; }

        [JsonProperty("createIPAddress")]
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

        [JsonProperty("typeID")]
        public long TypeId { get; set; }

        [JsonProperty("departmentID")]
        public long DepartmentId { get; set; }

        [JsonProperty("areaID")]
        public long AreaId { get; set; }

        [JsonProperty("onlineSeconds")]
        public long OnlineSeconds { get; set; }

        [JsonProperty("avatarLarge")]
        public string AvatarLarge { get; set; }

        [JsonProperty("avatarMiddle")]
        public string AvatarMiddle { get; set; }

        [JsonProperty("avatarSmall")]
        public string AvatarSmall { get; set; }

        [JsonProperty("signature")]
        public string Signature { get; set; }

        [JsonProperty("settingsXML")]
        public string SettingsXml { get; set; }
    }
}
