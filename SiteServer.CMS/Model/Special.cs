using System;
using Datory;

namespace SiteServer.CMS.Model
{
    [DataTable("siteserver_Special")]
    public class Special : Entity
	{
	    [DataColumn]
	    public int SiteId { get; set; }

        [DataColumn]
        public string Title { get; set; }

        [DataColumn]
        public string Url { get; set; }

        [DataColumn]
        public DateTime AddDate { get; set; }
    }
}
