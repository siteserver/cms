using Datory;
using Datory.Annotations;

namespace SSCMS.Models
{
    [DataTable("siteserver_Tag")]
    public class ContentTag : Entity
    {
        [DataColumn]
        public string TagName { get; set; }

        [DataColumn]
        public int SiteId { get; set; }
    }
}
