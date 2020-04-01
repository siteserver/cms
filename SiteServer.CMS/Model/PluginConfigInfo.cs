namespace SiteServer.CMS.Model
{
	public class PluginConfigInfo
	{
	    public PluginConfigInfo()
		{
            Id = 0;
		    PluginId = string.Empty;
            SiteId = 0;
            ConfigName = string.Empty;
            ConfigValue = string.Empty;
		}

        public PluginConfigInfo(int id, string pluginId, int siteId, string configName, string configValue) 
		{
            Id = id;
            PluginId = pluginId;
            SiteId = siteId;
            ConfigName = configName;
            ConfigValue = configValue;
        }

        public int Id { get; set; }

        public string PluginId { get; set; }

        public int SiteId { get; set; }

	    public string ConfigName { get; set; }

	    public string ConfigValue { get; set; }
	}
}
