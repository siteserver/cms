using System;
using Dapper.Contrib.Extensions;
using Newtonsoft.Json;
using SiteServer.CMS.Database.Core;
using SiteServer.CMS.Database.Extends;
using SiteServer.Utils;

namespace SiteServer.CMS.Database.Models
{
    [Table("siteserver_Config")]
    public class ConfigInfo : IDataInfo
    {
	    public int Id { get; set; }

	    public string Guid { get; set; }

	    public DateTime? LastModifiedDate { get; set; }

        private string IsInitialized { get; set; }

        [Computed]
        public bool Initialized
        {
            get => TranslateUtils.ToBool(IsInitialized);
            set => IsInitialized = value.ToString();
        }

        public string DatabaseVersion { get; set; }

	    public DateTime? UpdateDate { get; set; }

        [Text]
	    private string SystemConfig { get; set; }

        public string GetSystemConfig()
        {
            return SystemConfig;
        }

        public string SetSystemConfig(string json)
        {
            return SystemConfig = json;
        }

        private ConfigInfoSystemExtend _systemExtend;

        [Computed]
        [JsonIgnore]
        public ConfigInfoSystemExtend SystemExtend => _systemExtend ?? (_systemExtend = new ConfigInfoSystemExtend(this));
    }
}
