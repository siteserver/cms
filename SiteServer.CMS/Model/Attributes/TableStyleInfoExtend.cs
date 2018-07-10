using SiteServer.Plugin;
using SiteServer.Utils;

namespace SiteServer.CMS.Model.Attributes
{
    public class TableStyleInfoExtend : ExtendedAttributes
    {
        public TableStyleInfoExtend(string settings) : base(settings)
        {

        }

        public int Height
        {
            get => GetInt("Height");
            set => Set("Height", value);
        }

        public string Width
        {
            get
            {
                var width = GetString("Width");
                return width == "0" ? string.Empty : width;
            }
            set => Set("Width", value);
        }

        public int Columns
        {
            get => GetInt("Columns");
            set => Set("Columns", value);
        }

        public bool IsFormatString
        {
            get => GetBool("IsFormatString");
            set => Set("IsFormatString", value);
        }

        public int RelatedFieldId
        {
            get => GetInt("RelatedFieldID");
            set => Set("RelatedFieldID", value);
        }

        public string RelatedFieldStyle
        {
            get => GetString("RelatedFieldStyle");
            set => Set("RelatedFieldStyle", value);
        }

        public string CustomizeLeft
        {
            get => GetString("CustomizeLeft");
            set => Set("CustomizeLeft", value);
        }

        public string CustomizeRight
        {
            get => GetString("CustomizeRight");
            set => Set("CustomizeRight", value);
        }

        public bool IsValidate
        {
            get => GetBool("IsValidate");
            set => Set("IsValidate", value);
        }

        public bool IsRequired
        {
            get => GetBool("IsRequired");
            set => Set("IsRequired", value);
        }

        public int MinNum
        {
            get => GetInt("MinNum");
            set => Set("MinNum", value);
        }

        public int MaxNum
        {
            get => GetInt("MaxNum");
            set => Set("MaxNum", value);
        }

        public ValidateType ValidateType
        {
            get => ValidateTypeUtils.GetEnumType(GetString("ValidateType"));
            set => Set("ValidateType", value != null ? value.Value : ValidateType.None.Value);
        }

        public string RegExp
        {
            get => GetString("RegExp");
            set => Set("RegExp", value);
        }

        public string ErrorMessage
        {
            get => GetString("ErrorMessage");
            set => Set("ErrorMessage", value);
        }
    }
}
