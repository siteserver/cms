using SiteServer.CMS.Database.Wrapper;
using SiteServer.Utils;

namespace SiteServer.CMS.Database.Models
{
    [Table("siteserver_Plugin")]
    public class PluginInfo : DynamicEntity
    {
        [TableColumn]
        public string PluginId { get; set; }

        [TableColumn]
        private string IsDisabled { get; set; }

        public bool Disabled
        {
            get => IsDisabled == "True";
            set => IsDisabled = value.ToString();
        }

        [TableColumn]
        public int Taxis { get; set; }
    }
}
