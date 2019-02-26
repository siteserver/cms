using System;
using Dapper.Contrib.Extensions;
using SiteServer.CMS.Database.Core;
using SiteServer.Plugin;

namespace SiteServer.CMS.Database.Models
{
    [Table("siteserver_UserLog")]
    public class UserLogInfo : IDataInfo, ILogInfo
    {
        public int Id { get; set; }

        public string Guid { get; set; }

        public DateTime? LastModifiedDate { get; set; }

	    public string UserName { get; set; }

	    public string IpAddress { get; set; }

	    public DateTime? AddDate { get; set; }

	    public string Action { get; set; }

	    public string Summary { get; set; }
	}
}
