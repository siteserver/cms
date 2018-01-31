using SiteServer.Plugin;
using SiteServer.Utils;

namespace SiteServer.CMS.Model
{
    public class TableStyleInfoExtend : ExtendedAttributes
    {
        public TableStyleInfoExtend(string settings) : base(settings)
        {

        }

        public int Height
        {
            get { return GetInt("Height"); }
            set { Set("Height", value.ToString()); }
        }

        public string Width
        {
            get
            {
                var width = GetString("Width");
                return width == "0" ? string.Empty : width;
            }
            set { Set("Width", value); }
        }

        public int Columns
        {
            get { return GetInt("Columns"); }
            set { Set("Columns", value.ToString()); }
        }

        public bool IsFormatString
        {
            get { return GetBool("IsFormatString"); }
            set { Set("IsFormatString", value.ToString()); }
        }

        public int RelatedFieldId
        {
            get { return GetInt("RelatedFieldID"); }
            set { Set("RelatedFieldID", value.ToString()); }
        }

        public string RelatedFieldStyle
        {
            get { return GetString("RelatedFieldStyle"); }
            set { Set("RelatedFieldStyle", value); }
        }

        public string CustomizeLeft
        {
            get { return GetString("CustomizeLeft"); }
            set { Set("CustomizeLeft", value); }
        }

        public string CustomizeRight
        {
            get { return GetString("CustomizeRight"); }
            set { Set("CustomizeRight", value); }
        }

        public bool IsValidate
        {
            get { return GetBool("IsValidate"); }
            set { Set("IsValidate", value.ToString()); }
        }

        public bool IsRequired
        {
            get { return GetBool("IsRequired"); }
            set { Set("IsRequired", value.ToString()); }
        }

        public int MinNum
        {
            get { return GetInt("MinNum"); }
            set { Set("MinNum", value.ToString()); }
        }

        public int MaxNum
        {
            get { return GetInt("MaxNum"); }
            set { Set("MaxNum", value.ToString()); }
        }

        public ValidateType ValidateType
        {
            get { return ValidateTypeUtils.GetEnumType(GetString("ValidateType")); }
            set { Set("ValidateType", value.Value); }
        }

        public string RegExp
        {
            get { return GetString("RegExp"); }
            set { Set("RegExp", value); }
        }

        public string ErrorMessage
        {
            get { return GetString("ErrorMessage"); }
            set { Set("ErrorMessage", value); }
        }
    }
}
