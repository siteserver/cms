using System;
using System.Collections.Generic;
using SS.CMS.Data;
using SS.CMS.Enums;

namespace SS.CMS.Models
{
    [Serializable]
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
        public bool IsVisibleInList { get; set; }

        private string InputType { get; set; }

        public InputType Type
        {
            get => Enums.InputType.Parse(InputType);
            set => InputType = value.Value;
        }

        [TableColumn]
        public string DefaultValue { get; set; }

        [TableColumn]
        public bool IsHorizontal { get; set; }

        [TableColumn(Text = true, Extend = true)]
        public string ExtendValues { get; set; }

        public List<TableStyleItemInfo> StyleItems { get; set; }

        public int Height { get; set; }

        public string Width { get; set; }

        public int Columns { get; set; }

        public bool IsFormatString { get; set; }

        public int RelatedFieldId { get; set; }

        public string RelatedFieldStyle { get; set; }

        public string CustomizeLeft { get; set; }

        public string CustomizeRight { get; set; }

        public bool IsValidate { get; set; }

        public bool IsRequired { get; set; }

        public int MinNum { get; set; }

        public int MaxNum { get; set; }

        public string ValidateType { get; set; } = Enums.ValidateType.None.Value;

        public string RegExp { get; set; }

        public string ErrorMessage { get; set; }

        public string VeeValidate { get; set; }
    }
}
