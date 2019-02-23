using System;
using Dapper.Contrib.Extensions;
using SiteServer.CMS.Database.Core;

namespace SiteServer.CMS.Database.Models
{
    [Table("siteserver_SitePermissions")]
    public class SitePermissionsInfo : IDataInfo
    {
        public int Id { get; set; }

        public string Guid { get; set; }

        public DateTime? LastModifiedDate { get; set; }

        public string RoleName { get; set; }

        public int SiteId { get; set; }

        [Text]
		public string ChannelIdCollection { get; set; }

        [Text]
        public string ChannelPermissions { get; set; }

        [Text]
        public string WebsitePermissions { get; set; }
    }
}
