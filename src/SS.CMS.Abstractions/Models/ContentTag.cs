using Datory;
using Datory.Annotations;

namespace SS.CMS.Abstractions
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
