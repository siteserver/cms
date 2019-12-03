using System;
using Datory;


namespace SiteServer.Abstractions
{
    [Serializable]
    [DataTable("siteserver_DbCache")]
	public class DbCache : Entity
    {
        [DataColumn]
        public string CacheKey { get; set; }

        [DataColumn(Length = 500)]
        public string CacheValue { get; set; }

        [DataColumn]
        public DateTime AddDate { get; set; }
    }
}
