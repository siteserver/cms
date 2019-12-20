using Datory;

namespace SiteServer.CMS.Model
{
    public class LibraryTextInfo
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public int GroupId { get; set; }

        public string ImageUrl { get; set; }

        public string Summary { get; set; }

        public string Content { get; set; }
    }
}