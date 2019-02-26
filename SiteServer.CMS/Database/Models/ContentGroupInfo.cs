using System;
using Dapper.Contrib.Extensions;
using SiteServer.CMS.Database.Core;

namespace SiteServer.CMS.Database.Models
{
    [Table("siteserver_ContentGroup")]
    public class ContentGroupInfo : IDataInfo
    {
        public int Id { get; set; }

        public string Guid { get; set; }

        public DateTime? LastModifiedDate { get; set; }

        public string GroupName { get; set; }

	    public int SiteId { get; set; }

	    public int Taxis { get; set; }

        [Text]
        public string Description { get; set; }
	}
}
