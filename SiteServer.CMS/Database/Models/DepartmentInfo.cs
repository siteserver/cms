using System;
using Dapper.Contrib.Extensions;
using SiteServer.CMS.Database.Core;
using SiteServer.Utils;

namespace SiteServer.CMS.Database.Models
{
    [Table("siteserver_Department")]
    public class DepartmentInfo : IDataInfo
    {
        public int Id { get; set; }

        public string Guid { get; set; }

        public DateTime? LastModifiedDate { get; set; }

        public string DepartmentName { get; set; }

	    public string Code { get; set; }

	    public int ParentId { get; set; }

	    public string ParentsPath { get; set; }

	    public int ParentsCount { get; set; }

	    public int ChildrenCount { get; set; }

        private string IsLastNode { get; set; }

        [Computed]
        public bool LastNode
        {
            get => TranslateUtils.ToBool(IsLastNode);
            set => IsLastNode = value.ToString();
        }

        public int Taxis { get; set; }

	    public DateTime? AddDate { get; set; }

	    public string Summary { get; set; }

	    public int CountOfAdmin { get; set; }
	}
}
