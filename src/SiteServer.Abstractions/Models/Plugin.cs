using System;
using Datory;
using Datory.Annotations;
using Newtonsoft.Json;


namespace SiteServer.Abstractions
{
    [DataTable("siteserver_Plugin")]
    public class Plugin : Entity
    {
        [DataColumn]
        public string PluginId { get; set; }

        [DataColumn]
        [JsonIgnore]
        private string IsDisabled { get; set; }

        public bool Disabled
        {
            get => TranslateUtils.ToBool(IsDisabled);
            set => IsDisabled = value.ToString();
        }

        [DataColumn]
        public int Taxis { get; set; }
    }
}