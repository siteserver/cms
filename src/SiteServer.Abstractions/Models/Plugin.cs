using System;
using Datory;
using Datory.Annotations;


namespace SiteServer.Abstractions
{
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