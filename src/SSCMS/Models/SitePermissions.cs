using System.Collections.Generic;
using Datory;
using Datory.Annotations;

namespace SSCMS
{
    [DataTable("siteserver_SitePermissions")]
    public class SitePermissions : Entity
    {
        [DataColumn]
        public string RoleName { get; set; }

        [DataColumn]
        public int SiteId { get; set; }

        [DataColumn(Text = true)]
        public List<int> ChannelIds { get; set; }

        [DataColumn(Text = true)]
        public List<string> ChannelPermissions { get; set; }

        [DataColumn(Text = true)]
        public List<string> WebsitePermissions { get; set; }
    }
}