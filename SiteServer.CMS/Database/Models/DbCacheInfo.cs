using System;
using Dapper.Contrib.Extensions;
using SiteServer.CMS.Database.Core;

namespace SiteServer.CMS.Database.Models
{
    [Table("siteserver_DbCache")]
    public class DbCacheInfo : IDataInfo
    {
        public int Id { get; set; }

        public string Guid { get; set; }

        public DateTime? LastModifiedDate { get; set; }

        public string CacheKey { get; set; }

	    public string CacheValue { get; set; }

        public DateTime? AddDate { get; set; }
    }
}
