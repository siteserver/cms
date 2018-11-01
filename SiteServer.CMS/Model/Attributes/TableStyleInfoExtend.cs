using SiteServer.CMS.Plugin.Impl;
using SiteServer.Plugin;
using SiteServer.Utils;

namespace SiteServer.CMS.Model.Attributes
{
    public class TableStyleInfoExtend : AttributesImpl
    {
        public TableStyleInfoExtend(string settings) : base(settings)
        {

        }

        public int Height
        {
            get => GetInt(nameof(Height));
            set => Set(nameof(Height), value);
        }

        public string Width
        {
            get
            {
                var width = GetString(nameof(Width));
                return width == "0" ? string.Empty : width;
            }
            set => Set(nameof(Width), value);
        }

        public int Columns
        {
            get => GetInt(nameof(Columns));
            set => Set(nameof(Columns), value);
        }

        public bool IsFormatString
        {
            get => GetBool(nameof(IsFormatString));
            set => Set(nameof(IsFormatString), value);
        }

        public int RelatedFieldId
        {
            get => GetInt(nameof(RelatedFieldId));
            set => Set(nameof(RelatedFieldId), value);
        }

        public string RelatedFieldStyle
        {
            get => GetString(nameof(RelatedFieldStyle));
            set => Set(nameof(RelatedFieldStyle), value);
        }

        public string CustomizeLeft
        {
            get => GetString(nameof(CustomizeLeft));
            set => Set(nameof(CustomizeLeft), value);
        }

        public string CustomizeRight
        {
            get => GetString(nameof(CustomizeRight));
            set => Set(nameof(CustomizeRight), value);
        }

        public bool IsValidate
        {
            get => GetBool(nameof(IsValidate));
            set => Set(nameof(IsValidate), value);
        }

        public bool IsRequired
        {
            get => GetBool(nameof(IsRequired));
            set => Set(nameof(IsRequired), value);
        }

        public int MinNum
        {
            get => GetInt(nameof(MinNum));
            set => Set(nameof(MinNum), value);
        }

        public int MaxNum
        {
            get => GetInt(nameof(MaxNum));
            set => Set(nameof(MaxNum), value);
        }

        public ValidateType ValidateType
        {
            get => ValidateTypeUtils.GetEnumType(GetString(nameof(ValidateType)));
            set => Set(nameof(ValidateType), value != null ? value.Value : ValidateType.None.Value);
        }

        public string RegExp
        {
            get => GetString(nameof(RegExp));
            set => Set(nameof(RegExp), value);
        }

        public string ErrorMessage
        {
            get => GetString(nameof(ErrorMessage));
            set => Set(nameof(ErrorMessage), value);
        }

        public string VeeValidate
        {
            get => GetString(nameof(VeeValidate));
            set => Set(nameof(VeeValidate), value);
        }
    }
}
