using SS.CMS.Data;

namespace SS.CMS.Abstractions.Models
{
    [Table("siteserver_Plugin")]
    public class PluginInfo : Entity
    {
        [TableColumn]
        public string PluginId { get; set; }

        [TableColumn]
        public bool Disabled { get; set; }

        [TableColumn]
        public int Taxis { get; set; }
    }
}
