using System;
using Dapper.Contrib.Extensions;
using SiteServer.CMS.Database.Core;

namespace SiteServer.CMS.Database.Models
{
    [Table("siteserver_ContentTag")]
    public class ContentTagInfo : IDataInfo
    {
        public int Id { get; set; }

        public string Guid { get; set; }

        public DateTime? LastModifiedDate { get; set; }

        public string TagName { get; set; }

	    public int SiteId { get; set; }

	    public int UseNum { get; set; }
	}
}
