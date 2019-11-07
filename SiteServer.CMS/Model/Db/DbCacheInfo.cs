using System;

namespace SiteServer.CMS.Model.Db
{
	public class DbCacheInfo
    {
        public int Id { get; set; }

        public string CacheKey { get; set; }

	    public string CacheValue { get; set; }

        public DateTime AddDate { get; set; }
    }
}
