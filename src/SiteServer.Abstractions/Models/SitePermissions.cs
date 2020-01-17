using System.Collections.Generic;
using Datory;
using Datory.Annotations;
using Newtonsoft.Json;

namespace SiteServer.Abstractions
{
    [DataTable("siteserver_SitePermissions")]
    public class SitePermissions : Entity
    {
        [DataColumn]
        public string RoleName { get; set; }

        [DataColumn]
        public int SiteId { get; set; }

        [DataColumn(Text = true)]
        [JsonIgnore]
        private string ChannelIdCollection { get; set; }

        public List<int> ChannelIdList
        {
            get => StringUtils.GetIntList(ChannelIdCollection);
            set => ChannelIdCollection = StringUtils.Join(value);
        }

        [DataColumn(Text = true)]
        [JsonIgnore]
        private string ChannelPermissions { get; set; }

        public List<string> ChannelPermissionList
        {
            get => StringUtils.GetStringList(ChannelPermissions);
            set => ChannelPermissions = StringUtils.Join(value);
        }

        [DataColumn(Text = true)]
        [JsonIgnore]
        private string WebsitePermissions { get; set; }

        public List<string> WebsitePermissionList
        {
            get => StringUtils.GetStringList(WebsitePermissions);
            set => WebsitePermissions = StringUtils.Join(value);
        }
    }
}