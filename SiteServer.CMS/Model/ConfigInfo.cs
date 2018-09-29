using System;
using SiteServer.CMS.Model.Attributes;

namespace SiteServer.CMS.Model
{
	public class ConfigInfo
	{
        public ConfigInfo(int id, bool isInitialized, string databaseVersion, DateTime updateDate, string systemConfig)
        {
            Id = id;
            IsInitialized = isInitialized;
            DatabaseVersion = databaseVersion;
            UpdateDate = updateDate;
            SystemConfig = systemConfig;
		}

        public int Id { get; set; }

        public bool IsInitialized { get; set; }

	    public string DatabaseVersion { get; set; }

	    public DateTime UpdateDate { get; set; }

	    public string SystemConfig { get; set; }

	    private SystemConfigInfo _systemConfigInfo;
	    public SystemConfigInfo SystemConfigInfo => _systemConfigInfo ?? (_systemConfigInfo = new SystemConfigInfo(SystemConfig));
	}
}
