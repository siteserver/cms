using System;
using Dapper.Contrib.Extensions;
using SiteServer.CMS.Database.Core;

namespace SiteServer.CMS.Database.Models
{
    [Table("siteserver_RelatedFieldItem")]
    public class RelatedFieldItemInfo : IDataInfo
    {
        public int Id { get; set; }

        public string Guid { get; set; }

        public DateTime? LastModifiedDate { get; set; }

        public int RelatedFieldId { get; set; }

	    public string ItemName { get; set; }

	    public string ItemValue { get; set; }

	    public int ParentId { get; set; }

	    public int Taxis { get; set; }
	}
}
