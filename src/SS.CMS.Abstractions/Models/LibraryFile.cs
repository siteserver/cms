using Datory;
using Datory.Annotations;

namespace SS.CMS.Abstractions
{
    [DataTable("siteserver_LibraryFile")]
    public class LibraryFile : Entity
    {
        [DataColumn]
        public string Title { get; set; }

        [DataColumn]
        public int GroupId { get; set; }

        [DataColumn]
        public string Type { get; set; }

        [DataColumn]
        public string Url { get; set; }
    }
}