using System;
using Dapper.Contrib.Extensions;
using SiteServer.CMS.Database.Core;

namespace SiteServer.CMS.Database.Models
{
    [Table("siteserver_SiteLog")]
    public class SiteLogInfo : IDataInfo
    {
        public int Id { get; set; }

        public string Guid { get; set; }

        public DateTime? LastModifiedDate { get; set; }

        public int SiteId { get; set; }

	    public int ChannelId { get; set; }

	    public int ContentId { get; set; }

	    public string UserName { get; set; }

	    public string IpAddress { get; set; }

	    public DateTime? AddDate { get; set; }

	    public string Action { get; set; }

	    public string Summary { get; set; }
	}
}
