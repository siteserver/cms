using Datory;
using Datory.Annotations;

namespace SSCMS.Models
{
    [DataTable("siteserver_LibraryImage")]
    public class LibraryImage : Entity
    {
        [DataColumn]
        public string Title { get; set; }

        [DataColumn]
        public int GroupId { get; set; }

        [DataColumn]
        public string Url { get; set; }
    }
}