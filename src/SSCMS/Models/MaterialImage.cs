using Datory;
using Datory.Annotations;

namespace SSCMS.Models
{
    [DataTable("siteserver_MaterialImage")]
    public class MaterialImage : Entity
    {
        [DataColumn]
        public int GroupId { get; set; }

        [DataColumn]
        public string Title { get; set; }

        [DataColumn]
        public string Url { get; set; }
    }
}