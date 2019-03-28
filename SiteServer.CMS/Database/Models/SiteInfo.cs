using SiteServer.CMS.Database.Wrapper;
using SiteServer.Plugin;
using SiteServer.Utils;

namespace SiteServer.CMS.Database.Models
{
    [Table("siteserver_Site")]
    public partial class SiteInfo: DynamicEntity, ISiteInfo
    {
        [TableColumn]
        public string SiteDir { get; set; }

        [TableColumn]
        public string SiteName { get; set; }

        [TableColumn]
        public string TableName { get; set; }

        [TableColumn]
        private string IsRoot { get; set; }

        public bool Root
        {
            get => IsRoot == "True";
            set => IsRoot = value.ToString();
        }

        [TableColumn]
        public int ParentId { get; set; }

        [TableColumn]
        public int Taxis { get; set; }

        [TableColumn(Text = true, Extend = true)]
        public string SettingsXml { get; set; }
    }
}
