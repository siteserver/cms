using System;
using Datory;

namespace SiteServer.CMS.Model
{
    [Table("siteserver_AccessToken")]
    public class AccessTokenInfo : Entity
    {
        [TableColumn]
        public string Title { get; set; }

        [TableColumn]
        public string Token { get; set; }

        [TableColumn]
        public string AdminName { get; set; }

        [TableColumn]
        public string Scopes { get; set; }

        [TableColumn]
        public int RateLimit { get; set; }

        [TableColumn]
        public DateTimeOffset? AddDate { get; set; }
    }
}
