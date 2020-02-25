using System.Collections.Generic;
using Datory;
using Datory.Annotations;

namespace SS.CMS.Abstractions
{
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
        public bool List { get; set; }

        [DataColumn] 
        public InputType InputType { get; set; }

        [DataColumn] 
        public string DefaultValue { get; set; }

        [DataColumn]
        public bool Horizontal { get; set; }

        [DataIgnore]
        public List<TableStyleItem> Items { get; set; }

        public string ItemValues { get; set; }

        public string RuleValues { get; set; }

        public int Height { get; set; }

        public string Width { get; set; }

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
