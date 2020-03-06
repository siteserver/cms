using Datory;
using Datory.Annotations;


namespace SS.CMS.Abstractions
{
    [DataTable("siteserver_Plugin")]
    public class Plugin : Entity
    {
        [DataColumn]
        public string PluginId { get; set; }

        [DataColumn]
        public bool Disabled { get; set; }

        [DataColumn]
        public int Taxis { get; set; }
    }
}