using Datory;
using Datory.Annotations;

namespace SS.CMS.Abstractions
{
    [DataTable("siteserver_PluginConfig")]
    public class PluginConfig : Entity
	{
        [DataColumn]
        public string PluginId { get; set; }

        [DataColumn]
        public int SiteId { get; set; }

        [DataColumn]
        public string ConfigName { get; set; }

        [DataColumn(Text = true)]
        public string ConfigValue { get; set; }
	}
}
