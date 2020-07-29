using Datory;
using Datory.Annotations;
using SSCMS.Enums;

namespace SSCMS.Models
{
    [DataTable("siteserver_MaterialMessageItem")]
    public class MaterialMessageItem : Entity
    {
        [DataColumn]
        public int MessageId { get; set; }

        [DataColumn]
        public MaterialType MaterialType { get; set; }

        [DataColumn]
        public int MaterialId { get; set; }

        [DataColumn]
        public int Taxis { get; set; }

        [DataIgnore]
        public string ThumbMediaId { get; set; }

        [DataIgnore]
        public string Author { get; set; }

        [DataIgnore]
        public string Title { get; set; }

        [DataIgnore]
        public string ContentSourceUrl { get; set; }

        [DataIgnore]
        public string Content { get; set; }

        [DataIgnore]
        public string Digest { get; set; }

        [DataIgnore]
        public bool ShowCoverPic { get; set; }

        [DataIgnore]
        public string ThumbUrl { get; set; }

        [DataIgnore]
        public CommentType CommentType { get; set; }
    }
}