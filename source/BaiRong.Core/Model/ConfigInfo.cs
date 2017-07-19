using System;

namespace BaiRong.Core.Model
{
	public class ConfigInfo
	{
	    public ConfigInfo()
		{
            IsInitialized = false;
            DatabaseVersion = string.Empty;
            UpdateDate = DateTime.Now;
            UserConfig = string.Empty;
            SystemConfig = string.Empty;
		}

        public ConfigInfo(bool isInitialized, string databaseVersion, DateTime updateDate, string userConfig, string systemConfig) 
		{
            IsInitialized = isInitialized;
            DatabaseVersion = databaseVersion;
            UpdateDate = updateDate;
            UserConfig = userConfig;
            SystemConfig = systemConfig;
		}

        public bool IsInitialized { get; set; }

	    public string DatabaseVersion { get; set; }

	    public DateTime UpdateDate { get; set; }

	    public string UserConfig { get; set; }

	    public string SystemConfig { get; set; }

	    private UserConfigInfo _userConfigInfo;
        public UserConfigInfo UserConfigInfo => _userConfigInfo ?? (_userConfigInfo = new UserConfigInfo(UserConfig));

	    private SystemConfigInfo _systemConfigInfo;
	    public SystemConfigInfo SystemConfigInfo => _systemConfigInfo ?? (_systemConfigInfo = new SystemConfigInfo(SystemConfig));
	}
}
