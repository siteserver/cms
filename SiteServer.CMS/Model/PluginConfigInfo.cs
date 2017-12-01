namespace SiteServer.CMS.Model
{
	public class PluginConfigInfo
	{
	    public PluginConfigInfo()
		{
            Id = 0;
		    PluginId = string.Empty;
            PublishmentSystemId = 0;
            ConfigName = string.Empty;
            ConfigValue = string.Empty;
		}

        public PluginConfigInfo(int id, string pluginId, int publishmentSystemId, string configName, string configValue) 
		{
            Id = id;
            PluginId = pluginId;
            PublishmentSystemId = publishmentSystemId;
            ConfigName = configName;
            ConfigValue = configValue;
        }

        public int Id { get; set; }

        public string PluginId { get; set; }

        public int PublishmentSystemId { get; set; }

	    public string ConfigName { get; set; }

	    public string ConfigValue { get; set; }
	}
}
