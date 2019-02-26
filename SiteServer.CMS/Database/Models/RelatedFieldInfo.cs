using System;
using Dapper.Contrib.Extensions;
using SiteServer.CMS.Database.Core;

namespace SiteServer.CMS.Database.Models
{
    [Table("siteserver_RelatedField")]
    public class RelatedFieldInfo : IDataInfo
    {
        public int Id { get; set; }

        public string Guid { get; set; }

        public DateTime? LastModifiedDate { get; set; }

        public string Title { get; set; }

	    public int SiteId { get; set; }

	    public int TotalLevel { get; set; }

	    public string Prefixes { get; set; }

	    public string Suffixes { get; set; }
	}
}
