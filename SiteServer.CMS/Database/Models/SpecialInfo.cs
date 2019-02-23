using System;
using Dapper.Contrib.Extensions;
using SiteServer.CMS.Database.Core;

namespace SiteServer.CMS.Database.Models
{
    [Table("siteserver_Special")]
    public class SpecialInfo : IDataInfo
    {
        public int Id { get; set; }

        public string Guid { get; set; }

        public DateTime? LastModifiedDate { get; set; }

        public int SiteId { get; set; }

	    public string Title { get; set; }

	    public string Url { get; set; }

	    public DateTime? AddDate { get; set; }
    }
}
