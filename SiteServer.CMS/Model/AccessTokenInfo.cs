using System;

namespace SiteServer.CMS.Model
{
    public class AccessTokenInfo
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Token { get; set; }

        public string AdminName { get; set; }

        public string Scopes { get; set; }

        public int RateLimit { get; set; }

        public DateTime AddDate { get; set; }

        public DateTime UpdatedDate { get; set; }
    }
}
