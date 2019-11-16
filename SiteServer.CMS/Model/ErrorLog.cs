using System;
using Datory;

namespace SiteServer.CMS.Model
{
    [DataTable("siteserver_ErrorLog")]
    public class ErrorLog : Entity
    {
        [DataColumn]
        public string Category { get; set; }

        [DataColumn]
        public string PluginId { get; set; }

        [DataColumn]
        public string Message { get; set; }

        [DataColumn(Text = true)]
        public string Stacktrace { get; set; }

        [DataColumn(Text = true)]
        public string Summary { get; set; }

        [DataColumn]
        public DateTime AddDate { get; set; }
    }
}
