using System;
using Datory;


namespace SiteServer.Abstractions
{
    [Serializable]
    [DataTable("siteserver_Plugin")]
    public class Plugin : Entity
    {
        [DataColumn]
        public string PluginId { get; set; }

        [DataColumn]
        public string IsDisabled { get; set; }

        public bool Disabled
        {
            get => TranslateUtils.ToBool(IsDisabled);
            set => IsDisabled = value.ToString();
        }

        [DataColumn]
        public int Taxis { get; set; }
    }
}