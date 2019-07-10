using System;
using SS.CMS.Data;

namespace SS.CMS.Models
{
    [Serializable]
    [DataTable("siteserver_ContentCheck")]
    public class ContentCheck : Entity
    {
        [DataColumn]
        public string TableName { get; set; }

        [DataColumn]
        public int SiteId { get; set; }

        [DataColumn]
        public int ChannelId { get; set; }

        [DataColumn]
        public int ContentId { get; set; }

        [DataColumn]
        public int UserId { get; set; }

        [DataColumn]
        public bool IsChecked { get; set; }

        [DataColumn]
        public int CheckedLevel { get; set; }

        [DataColumn]
        public DateTime? CheckDate { get; set; }

        [DataColumn]
        public string Reasons { get; set; }
    }
}
