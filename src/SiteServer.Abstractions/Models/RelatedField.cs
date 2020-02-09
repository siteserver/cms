using Datory;
using Datory.Annotations;

namespace SiteServer.Abstractions
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
