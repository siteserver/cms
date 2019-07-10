using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using SS.CMS.Data;
using SS.CMS.Enums;

namespace SS.CMS.Models
{
    [Serializable]
    [DataTable("siteserver_TableStyle")]
    public class TableStyle : Entity
    {
        [DataColumn]
        public int RelatedIdentity { get; set; }

        [DataColumn]
        public string TableName { get; set; }

        [DataColumn]
        public string AttributeName { get; set; }

        [DataColumn]
        public int Taxis { get; set; }

        [DataColumn]
        public string DisplayName { get; set; }

        [DataColumn]
        public string HelpText { get; set; }

        [DataColumn]
        public bool IsVisibleInList { get; set; }

        private string InputType { get; set; }

        [JsonIgnore]
        [DataIgnore]
        public InputType Type
        {
            get => Enums.InputType.Parse(InputType);
            set => InputType = value.Value;
        }

        [DataColumn]
        public string DefaultValue { get; set; }

        [DataColumn]
        public bool IsHorizontal { get; set; }

        [DataColumn(Text = true, Extend = true)]
        public string ExtendValues { get; set; }

        [DataIgnore]
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

        public string ValidateType { get; set; } = Enums.ValidateType.None.Value;

        public string RegExp { get; set; }

        public string ErrorMessage { get; set; }

        public string VeeValidate { get; set; }
    }
}
