using System.Collections.Generic;
using Datory;
using SiteServer.CMS.Core;
using SiteServer.Plugin;
using SiteServer.Utils;

namespace SiteServer.CMS.Model
{
    [DataTable("siteserver_TableStyle")]
    public class TableStyle : Entity
    {
        [DataColumn] public int RelatedIdentity { get; set; }

        [DataColumn] public string TableName { get; set; }

        [DataColumn] public string AttributeName { get; set; }

        [DataColumn] public int Taxis { get; set; }

        [DataColumn] public string DisplayName { get; set; }

        [DataColumn] public string HelpText { get; set; }

        [DataColumn] public string IsVisibleInList { get; set; }

        public bool VisibleInList
        {
            get => TranslateUtils.ToBool(IsVisibleInList);
            set => IsVisibleInList = value.ToString();
        }

        [DataColumn] public string InputType { get; set; }

        public InputType Type
        {
            get => InputTypeUtils.GetEnumType(InputType);
            set => InputType = value.Value;
        }

        [DataColumn] public string DefaultValue { get; set; }

        [DataColumn] public string IsHorizontal { get; set; }

        public bool Horizontal
        {
            get => TranslateUtils.ToBool(IsHorizontal);
            set => IsHorizontal = value.ToString();
        }

        [DataColumn(Text = true)] public string ExtendValues { get; set; }

        public List<TableStyleItem> StyleItems { get; set; }

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

        public ValidateType ValidateType { get; set; }

        public string RegExp { get; set; }

        public string ErrorMessage { get; set; }

        public string VeeValidate { get; set; }
    }
}
