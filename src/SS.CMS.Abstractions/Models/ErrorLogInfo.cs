using System;
using SS.CMS.Data;

namespace SS.CMS.Models
{
    [Serializable]
    [Table("siteserver_ErrorLog")]
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
    }
}
