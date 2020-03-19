using System;
using Datory;
using Datory.Annotations;

namespace SSCMS
{
    [DataTable("siteserver_ContentCheck")]
	public class ContentCheck : Entity
	{
        [DataColumn]
        public int SiteId { get; set; }

        [DataColumn]
        public int ChannelId { get; set; }

        [DataColumn]
        public int ContentId { get; set; }

        [DataColumn]
        public int AdminId { get; set; }

        [DataColumn]
        public bool Checked { get; set; }

        [DataColumn]
        public int CheckedLevel { get; set; }

        [DataColumn]
        public DateTime CheckDate { get; set; }

        [DataColumn]
        public string Reasons { get; set; }
    }
}
