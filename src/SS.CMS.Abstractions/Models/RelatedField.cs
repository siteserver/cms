using Datory;
using Datory.Annotations;

namespace SS.CMS.Abstractions
{
    [DataTable("siteserver_RelatedField")]
    public class RelatedField : Entity
	{
        [DataColumn]
        public int SiteId { get; set; }

        [DataColumn]
	    public string Title { get; set; }
	}
}
