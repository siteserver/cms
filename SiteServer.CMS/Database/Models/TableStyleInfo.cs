using SiteServer.CMS.Core;
using SiteServer.CMS.Database.Wrapper;
using SiteServer.Plugin;

namespace SiteServer.CMS.Database.Models
{
    [Table("siteserver_TableStyle")]
    public partial class TableStyleInfo : DynamicEntity
    {
        [TableColumn]
        public int RelatedIdentity { get; set; }

        [TableColumn]
        public string TableName { get; set; }

        [TableColumn]
        public string AttributeName { get; set; }

        [TableColumn]
        public int Taxis { get; set; }

        [TableColumn]
        public string DisplayName { get; set; }

        [TableColumn]
        public string HelpText { get; set; }

        [TableColumn]
        private string IsVisibleInList { get; set; }

        public bool VisibleInList
        {
            get => IsVisibleInList == "True";
            set => IsVisibleInList = value.ToString();
        }

        [TableColumn]
        private string InputType { get; set; }

        public InputType Type
        {
            get => InputTypeUtils.GetEnumType(InputType);
            set => InputType = value.Value;
        }

        [TableColumn]
        public string DefaultValue { get; set; }

        [TableColumn]
        private string IsHorizontal { get; set; }

        public bool Horizontal
        {
            get => IsHorizontal == "True";
            set => IsHorizontal = value.ToString();
        }

        [TableColumn(Text = true, Extend = true)]
        public string ExtendValues { get; set; }
    }
}
