using Datory;
using Datory.Annotations;

namespace SSCMS.Models
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