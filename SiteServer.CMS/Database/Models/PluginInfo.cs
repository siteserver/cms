using System;
using Dapper.Contrib.Extensions;
using SiteServer.CMS.Database.Core;
using SiteServer.Utils;

namespace SiteServer.CMS.Database.Models
{
    [Table("siteserver_Plugin")]
    public class PluginInfo : IDataInfo
    {
        public int Id { get; set; }

        public string Guid { get; set; }

        public DateTime? LastModifiedDate { get; set; }

        public string PluginId { get; set; }

	    private string IsDisabled { get; set; }

        [Computed]
        public bool Disabled
        {
            get => TranslateUtils.ToBool(IsDisabled);
            set => IsDisabled = value.ToString();
        }

        public int Taxis { get; set; }
	}
}
