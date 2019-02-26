using System;
using Dapper.Contrib.Extensions;
using SiteServer.CMS.Database.Core;

namespace SiteServer.CMS.Database.Models
{
    [Table("siteserver_PluginConfig")]
    public class PluginConfigInfo : IDataInfo
    {
        public int Id { get; set; }

        public string Guid { get; set; }

        public DateTime? LastModifiedDate { get; set; }

        public string PluginId { get; set; }

        public int SiteId { get; set; }

	    public string ConfigName { get; set; }

        [Text]
	    public string ConfigValue { get; set; }
	}
}
