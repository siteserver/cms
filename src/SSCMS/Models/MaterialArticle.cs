using Datory;
using Datory.Annotations;
using SSCMS.Enums;

namespace SSCMS.Models
{
    [DataTable("siteserver_MaterialArticle")]
    public class MaterialArticle : Entity
    {
        [DataColumn] public int GroupId { get; set; }

        /// <summary>图文消息缩略图的media_id，可以在基础支持上传多媒体文件接口中获得</summary>
        [DataColumn]
        public string ThumbMediaId { get; set; }

        /// <summary>图文消息的作者</summary>
        [DataColumn]
        public string Author { get; set; }

        /// <summary>图文消息的标题</summary>
        [DataColumn]
        public string Title { get; set; }

        /// <summary>在图文消息页面点击“阅读原文”后的页面</summary>
        [DataColumn]
        public string ContentSourceUrl { get; set; }

        /// <summary>图文消息页面的内容，支持HTML标签</summary>
        [DataColumn(Text = true)]
        public string Content { get; set; }

        /// <summary>图文消息的描述</summary>
        [DataColumn]
        public string Digest { get; set; }

        /// <summary>是否显示封面，1为显示，0为不显示</summary>
        [DataColumn]
        public bool ShowCoverPic { get; set; }

        /// <summary>缩略图的URL</summary>
        [DataColumn]
        public string ThumbUrl { get; set; }

        /// <summary>是否打开评论，0不打开，1打开</summary>
        [DataColumn]
        public CommentType CommentType { get; set; }
    }
}