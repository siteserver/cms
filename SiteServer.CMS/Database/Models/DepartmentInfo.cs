using System;
using SiteServer.CMS.Database.Wrapper;
using SiteServer.Utils;

namespace SiteServer.CMS.Database.Models
{
    [Table("siteserver_Department")]
    public class DepartmentInfo : DynamicEntity
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
        private string IsLastNode { get; set; }

        public bool LastNode
        {
            get => IsLastNode == "True";
            set => IsLastNode = value.ToString();
        }

        [TableColumn]
        public int Taxis { get; set; }

        [TableColumn]
        public DateTime? AddDate { get; set; }

        [TableColumn]
        public string Summary { get; set; }

        [TableColumn]
        public int CountOfAdmin { get; set; }
    }
}
