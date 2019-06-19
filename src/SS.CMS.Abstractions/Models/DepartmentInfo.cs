using SS.CMS.Data;

namespace SS.CMS.Models
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
        public bool IsLastNode { get; set; }

        [TableColumn]
        public int Taxis { get; set; }

        [TableColumn]
        public string Summary { get; set; }
    }
}
