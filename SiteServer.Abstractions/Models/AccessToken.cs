using System;
using Datory;

namespace SiteServer.Abstractions
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
        public string AdminName { get; set; }

        [DataColumn]
        public string Scopes { get; set; }

        [DataColumn]
        public int RateLimit { get; set; }

        [DataColumn]
        public DateTime AddDate { get; set; }

        [DataColumn]
        public DateTime UpdatedDate { get; set; }
    }
}
