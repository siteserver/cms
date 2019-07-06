using System;
using SS.CMS.Data;

namespace SS.CMS.Models
{
    [Serializable]
    [Table("siteserver_Plugin")]
    public class PluginInfo : Entity
    {
        [TableColumn]
        public string PluginId { get; set; }

        [TableColumn]
        public bool IsDisabled { get; set; }

        [TableColumn]
        public int Taxis { get; set; }
    }
}
