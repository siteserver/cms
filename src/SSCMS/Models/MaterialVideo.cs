using Datory;
using Datory.Annotations;

namespace SSCMS.Models
{
    [DataTable("siteserver_MaterialVideo")]
    public class MaterialVideo : Entity
    {
        [DataColumn]
        public string MediaId { get; set; }

        [DataColumn]
        public int GroupId { get; set; }

        [DataColumn]
        public string Title { get; set; }

        [DataColumn]
        public string Description { get; set; }

        [DataColumn]
        public string FileType { get; set; }

        [DataColumn]
        public string Url { get; set; }
    }
}