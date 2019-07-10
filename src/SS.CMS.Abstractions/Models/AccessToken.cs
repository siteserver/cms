using System;
using SS.CMS.Data;

namespace SS.CMS.Models
{
    [Serializable]
    [DataTable("siteserver_AccessToken")]
    public class AccessToken : Entity
    {
        [DataColumn]
        public string Title { get; set; }

        [DataColumn]
        public string Token { get; set; }

        [DataColumn]
        public int UserId { get; set; }

        [DataColumn]
        public string Scopes { get; set; }

        [DataColumn]
        public int RateLimit { get; set; }
    }
}
