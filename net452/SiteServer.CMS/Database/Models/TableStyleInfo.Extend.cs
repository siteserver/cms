using System.Collections.Generic;

namespace SiteServer.CMS.Database.Models
{
    public partial class TableStyleInfo
    {   
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

        public string ValidateType { get; set; } = SiteServer.Plugin.ValidateType.None.Value;

        public string RegExp { get; set; }

        public string ErrorMessage { get; set; }

        public string VeeValidate { get; set; }
    }
}
