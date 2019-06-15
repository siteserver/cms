using System;
using SS.CMS.Data;

namespace SS.CMS.Abstractions.Models
{
    [Table("siteserver_Department")]
    public class DepartmentInfo : Entity
    {
        [TableColumn]
        public string DepartmentName { get; set; }

        [TableColumn]
        public string Code { get; set; }

        [TableColumn]
        public int ParentId { get; set; }

        [TableColumn]
        public string ParentsPath { get; set; }

        [TableColumn]
        public int ParentsCount { get; set; }

        [TableColumn]
        public int ChildrenCount { get; set; }

        [TableColumn]
        public bool LastNode { get; set; }

        [TableColumn]
        public int Taxis { get; set; }

        [TableColumn]
        public DateTime? AddDate { get; set; }

        [TableColumn]
        public string Summary { get; set; }
    }
}
