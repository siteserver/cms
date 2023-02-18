using System;
using Datory;
using Datory.Annotations;

namespace SSCMS.Models
{
    [DataTable("siteserver_FormData")]
    public class FormData : Entity
    {
        [DataColumn]
        public int SiteId { get; set; }

        [DataColumn]
        public int ChannelId { get; set; }

        [DataColumn]
        public int ContentId { get; set; }

        [DataColumn]
        public int FormId { get; set; }

        [DataColumn]
        public bool IsReplied { get; set; }

        [DataColumn]
        public DateTime? ReplyDate { get; set; }

        [DataColumn(Text = true)]
        public string ReplyContent { get; set; }
    }
}