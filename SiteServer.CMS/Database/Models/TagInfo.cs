using System;
using Dapper.Contrib.Extensions;
using SiteServer.CMS.Database.Core;

namespace SiteServer.CMS.Database.Models
{
    [Table("siteserver_Tag")]
    public class TagInfo : IDataInfo
    {
        public int Id { get; set; }

        public string Guid { get; set; }

        public DateTime? LastModifiedDate { get; set; }

        public int SiteId { get; set; }

	    public string ContentIdCollection { get; set; }

	    public string Tag { get; set; }

	    public int UseNum { get; set; }

	    public int Level { get; set; }
	}
}
