using System.Collections.Generic;
using Newtonsoft.Json;
using SS.CMS.Models;
using SS.CMS.Repositories;

namespace SS.CMS.Cli.Updater.Tables
{
    public partial class TableSitePermissions
    {
        [JsonProperty("roleName")]
        public string RoleName { get; set; }

        [JsonProperty("publishmentSystemID")]
        public long PublishmentSystemId { get; set; }

        [JsonProperty("nodeIDCollection")]
        public string NodeIdCollection { get; set; }

        [JsonProperty("channelIdCollection")]
        public string ChannelIdCollection { get; set; }

        [JsonProperty("channelPermissions")]
        public string ChannelPermissions { get; set; }

        [JsonProperty("websitePermissions")]
        public string WebsitePermissions { get; set; }

        [JsonProperty("id")]
        public long Id { get; set; }
    }

    public partial class TableSitePermissions
    {
        public static readonly List<string> OldTableNames = new List<string>
        {
            "siteserver_SystemPermissions",
            "wcm_SystemPermissions"
        };

        public static ConvertInfo GetConverter(IPermissionRepository permissionRepository) => new ConvertInfo
        {
            NewTableName = permissionRepository.TableName,
            NewColumns = permissionRepository.TableColumns,
            ConvertKeyDict = ConvertKeyDict,
            ConvertValueDict = ConvertValueDict
        };

        private static readonly Dictionary<string, string> ConvertKeyDict =
            new Dictionary<string, string>
            {
                {nameof(Models.Permission.SiteId), nameof(PublishmentSystemId)},
                {nameof(Models.Permission.SitePermissions), nameof(WebsitePermissions)},
                {nameof(Models.Permission.ChannelId), nameof(ChannelIdCollection)},
                {nameof(Models.Permission.ChannelId), nameof(NodeIdCollection)}
            };

        private static readonly Dictionary<string, string> ConvertValueDict = null;
    }
}
