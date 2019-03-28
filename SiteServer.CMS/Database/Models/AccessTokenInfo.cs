using System;
using SiteServer.CMS.Database.Wrapper;

namespace SiteServer.CMS.Database.Models
{
    [Table("siteserver_AccessToken")]
    public class AccessTokenInfo : DynamicEntity
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
