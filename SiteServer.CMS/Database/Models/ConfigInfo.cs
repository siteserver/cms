using System;
using SiteServer.CMS.Database.Wrapper;
using SiteServer.Utils;
using SiteServer.Utils.Enumerations;

namespace SiteServer.CMS.Database.Models
{
    [Table("siteserver_Config")]
    public partial class ConfigInfo : DynamicEntity
    {
        [TableColumn]
        private string IsInitialized { get; set; }

        public bool Initialized
        {
            get => IsInitialized == "True";
            set => IsInitialized = value.ToString();
        }

        [TableColumn]
        public string DatabaseVersion { get; set; }

        [TableColumn]
        public DateTime? UpdateDate { get; set; }

        [TableColumn(Text = true, Extend = true)]
        private string SystemConfig { get; set; }
    }
}
