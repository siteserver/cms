using SiteServer.CMS.Database.Wrapper;
using SiteServer.Utils;

namespace SiteServer.CMS.Database.Models
{
    [Table("siteserver_Area")]
    public class AreaInfo : DynamicEntity
    {
        [TableColumn]
        public string AreaName { get; set; }

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
        public int CountOfAdmin { get; set; }
    }
}
