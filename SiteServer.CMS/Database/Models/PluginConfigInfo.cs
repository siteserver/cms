using SiteServer.CMS.Database.Wrapper;

namespace SiteServer.CMS.Database.Models
{
    [Table("siteserver_PluginConfig")]
    public class PluginConfigInfo : DynamicEntity
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
