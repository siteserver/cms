using System.Collections.Generic;
using Datory;

namespace SiteServer.CMS.Model
{
    [DataTable("siteserver_SitePermissions")]
    public class SitePermissions : Entity
    {
        [DataColumn]
        public string RoleName { get; set; }

        [DataColumn]
        public int SiteId { get; set; }

        [DataColumn(Text = true)]
        public string ChannelIdCollection { get; set; }

        [DataColumn(Text = true)]
        public string ChannelPermissions { get; set; }

        [DataColumn(Text = true)]
        public string WebsitePermissions { get; set; }

        public List<int> ChannelIdList { get; set; }

        public List<string> ChannelPermissionList { get; set; }

        public List<string> WebsitePermissionList { get; set; }
    }
}