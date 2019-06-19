using SS.CMS.Data;

namespace SS.CMS.Models
{
    [Table("siteserver_TableStyleItem")]
    public class TableStyleItemInfo : Entity
    {
        [TableColumn]
        public int TableStyleId { get; set; }

        [TableColumn]
        public string ItemTitle { get; set; }

        [TableColumn]
        public string ItemValue { get; set; }

        [TableColumn]
        public bool IsSelected { get; set; }
    }
}
