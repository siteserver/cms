using Datory;
using Datory.Annotations;


namespace SS.CMS.Abstractions
{
    [DataTable("siteserver_DbCache")]
	public class DbCache : Entity
    {
        [DataColumn]
        public string CacheKey { get; set; }

        [DataColumn(Length = 500)]
        public string CacheValue { get; set; }
    }
}
