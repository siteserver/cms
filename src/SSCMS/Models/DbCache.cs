using Datory;
using Datory.Annotations;

namespace SSCMS.Models
{
    [DataTable("siteserver_DbCache")]
    public class DbCache : Entity
    {
        [DataColumn]
        public string CacheKey { get; set; }

        [DataColumn]
        public string CacheValue { get; set; }
    }
}
