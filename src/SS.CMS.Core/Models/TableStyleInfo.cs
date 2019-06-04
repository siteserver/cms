using System.Collections.Generic;
using SS.CMS.Core.Common;
using SS.CMS.Plugin;
using SS.CMS.Plugin.Data;

namespace SS.CMS.Core.Models
{
    [Table("siteserver_TableStyle")]
    public class TableStyleInfo : Entity
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

        public List<TableStyleItemInfo> StyleItems { get; set; }

        public int Height { get; set; }

        public string Width { get; set; }

        public int Columns { get; set; }

        public bool FormatString { get; set; }

        public int RelatedFieldId { get; set; }

        public string RelatedFieldStyle { get; set; }

        public string CustomizeLeft { get; set; }

        public string CustomizeRight { get; set; }

        public bool Validate { get; set; }

        public bool Required { get; set; }

        public int MinNum { get; set; }

        public int MaxNum { get; set; }

        public string ValidateType { get; set; } = SS.CMS.Plugin.ValidateType.None.Value;

        public string RegExp { get; set; }

        public string ErrorMessage { get; set; }

        public string VeeValidate { get; set; }
    }
}
