using System;
using SS.CMS.Data;

namespace SS.CMS.Models
{
    [Serializable]
    [DataTable("siteserver_Permission")]
    public class Permission : Entity
    {
        [DataColumn]
        public string RoleName { get; set; }

        [DataColumn(Text = true)]
        public string AppPermissions { get; set; }

        [DataColumn]
        public int SiteId { get; set; }
        [DataColumn(Text = true)]
        public string SitePermissions { get; set; }

        [DataColumn(Text = true)]
        public int ChannelId { get; set; }

        [DataColumn(Text = true)]
        public string ChannelPermissions { get; set; }
    }
}
