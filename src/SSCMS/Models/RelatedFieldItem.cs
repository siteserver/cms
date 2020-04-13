using Datory;
using Datory.Annotations;

namespace SSCMS.Models
{
    [DataTable("siteserver_RelatedFieldItem")]
    public class RelatedFieldItem : Entity
	{
        [DataColumn]
        public int SiteId { get; set; }

        [DataColumn]
	    public int RelatedFieldId { get; set; }

        [DataColumn]
        public string Label { get; set; }

        [DataColumn]
        public string Value { get; set; }

        [DataColumn]
        public int ParentId { get; set; }

        [DataColumn]
        public int Taxis { get; set; }
	}
}
