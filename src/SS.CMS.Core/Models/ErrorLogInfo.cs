using System;
using SS.CMS.Data;

namespace SS.CMS.Core.Models
{
    [Table("siteserver_ErrorLog")]
    [Serializable]
    public class ErrorLogInfo : Entity
    {
        [TableColumn]
        public string Category { get; set; }

        [TableColumn]
        public string PluginId { get; set; }

        [TableColumn]
        public string Message { get; set; }

        [TableColumn(Text = true)]
        public string Stacktrace { get; set; }

        [TableColumn(Text = true)]
        public string Summary { get; set; }

        [TableColumn]
        public DateTime? AddDate { get; set; }
    }
}
