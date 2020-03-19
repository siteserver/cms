using Datory;
using Datory.Annotations;

namespace SSCMS
{
    [DataTable("siteserver_LibraryVideo")]
    public class LibraryVideo : Entity
    {
        [DataColumn]
        public string Title { get; set; }

        [DataColumn]
        public int GroupId { get; set; }

        [DataColumn]
        public string FileType { get; set; }

        [DataColumn]
        public string Url { get; set; }
    }
}