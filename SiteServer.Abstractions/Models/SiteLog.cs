using System;
using Datory;


namespace SiteServer.Abstractions
{
    [Serializable]
    [DataTable("siteserver_SiteLog")]
	public class SiteLog : Entity
    {
        [DataColumn]
	    public int SiteId { get; set; }

        [DataColumn]
        public int ChannelId { get; set; }

        [DataColumn]
        public int ContentId { get; set; }

        [DataColumn]
        public string UserName { get; set; }

        [DataColumn]
        public string IpAddress { get; set; }

        [DataColumn]
        public DateTime AddDate { get; set; }

        [DataColumn]
        public string Action { get; set; }

        [DataColumn]
        public string Summary { get; set; }
    }
}
