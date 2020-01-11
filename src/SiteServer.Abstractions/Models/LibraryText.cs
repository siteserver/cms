using Datory;
using Datory.Annotations;

namespace SiteServer.Abstractions
{
    [DataTable("siteserver_LibraryText")]
    public class LibraryText : Entity
    {
        [DataColumn]
        public string Title { get; set; }

        [DataColumn]
        public int GroupId { get; set; }

        [DataColumn]
        public string ImageUrl { get; set; }

        [DataColumn]
        public string Summary { get; set; }

        [DataColumn(Text = true)]
        public string Content { get; set; }
    }
}