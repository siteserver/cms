using System;
using Dapper.Contrib.Extensions;
using SiteServer.CMS.Database.Core;

namespace SiteServer.CMS.Database.Models
{
    [Table("siteserver_AccessToken")]
    public class AccessTokenInfo : IDataInfo
    {
        public int Id { get; set; }

        public string Guid { get; set; }

        public DateTime? LastModifiedDate { get; set; }

        public string Title { get; set; }

        public string Token { get; set; }

        public string AdminName { get; set; }

        public string Scopes { get; set; }

        public int RateLimit { get; set; }

        public DateTimeOffset? AddDate { get; set; }
    }
}
