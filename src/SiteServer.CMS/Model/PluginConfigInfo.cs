using Datory;

namespace SiteServer.CMS.Model
{
    [Table("siteserver_PluginConfig")]
    public class PluginConfigInfo : Entity
    {
        [TableColumn]
        public string PluginId { get; set; }

        [TableColumn]
        public int SiteId { get; set; }

        [TableColumn]
        public string ConfigName { get; set; }

        [TableColumn(Text = true)]
        public string ConfigValue { get; set; }
    }
}
