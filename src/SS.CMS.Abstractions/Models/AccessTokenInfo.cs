using System;
using SS.CMS.Data;

namespace SS.CMS.Models
{
    [Serializable]
    [Table("siteserver_AccessToken")]
    public class AccessTokenInfo : Entity
    {
        [TableColumn]
        public string Title { get; set; }

        [TableColumn]
        public string Token { get; set; }

        [TableColumn]
        public int UserId { get; set; }

        [TableColumn]
        public string Scopes { get; set; }

        [TableColumn]
        public int RateLimit { get; set; }
    }
}
