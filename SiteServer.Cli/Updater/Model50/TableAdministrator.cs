using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;
using SiteServer.Plugin;

namespace SiteServer.Cli.Updater.Model50
{
    public partial class TableAdministrator
    {
        [JsonProperty("userName")]
        public string UserName { get; set; }

        [JsonProperty("password")]
        public string Password { get; set; }

        [JsonProperty("passwordFormat")]
        public string PasswordFormat { get; set; }

        [JsonProperty("passwordSalt")]
        public string PasswordSalt { get; set; }

        [JsonProperty("creationDate")]
        public DateTimeOffset CreationDate { get; set; }

        [JsonProperty("lastActivityDate")]
        public DateTimeOffset LastActivityDate { get; set; }

        [JsonProperty("countOfLogin")]
        public long CountOfLogin { get; set; }

        [JsonProperty("countOfFailedLogin")]
        public long CountOfFailedLogin { get; set; }

        [JsonProperty("creatorUserName")]
        public string CreatorUserName { get; set; }

        [JsonProperty("isLockedOut")]
        public string IsLockedOut { get; set; }

        [JsonProperty("publishmentSystemIdCollection")]
        public string PublishmentSystemIdCollection { get; set; }

        [JsonProperty("publishmentSystemId")]
        public long PublishmentSystemId { get; set; }

        [JsonProperty("departmentId")]
        public long DepartmentId { get; set; }

        [JsonProperty("areaId")]
        public long AreaId { get; set; }

        [JsonProperty("displayName")]
        public string DisplayName { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("mobile")]
        public string Mobile { get; set; }

        [JsonProperty("id")]
        public long Id { get; set; }
    }

    public partial class TableAdministrator
    {
        public const string OldTableName = "Administrator";

        public static ConvertInfo Converter => new ConvertInfo
        {
            NewTableName = NewTableName,
            NewColumns = NewColumns,
            ConvertKeyDict = ConvertKeyDict,
            ConvertValueDict = ConvertValueDict
        };

        private static readonly string NewTableName = DataProvider.AdministratorDao.TableName;

        private static readonly List<TableColumn> NewColumns = DataProvider.AdministratorDao.TableColumns;

        private static readonly Dictionary<string, string> ConvertKeyDict =
            new Dictionary<string, string>
            {
                {nameof(AdministratorInfo.SiteId), nameof(PublishmentSystemId)},
                {nameof(AdministratorInfo.SiteIdCollection), nameof(PublishmentSystemIdCollection)}
            };

        private static readonly Dictionary<string, string> ConvertValueDict = null;
    }
}
