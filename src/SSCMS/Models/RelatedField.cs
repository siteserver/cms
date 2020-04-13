using Datory;
using Datory.Annotations;

namespace SSCMS.Models
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
