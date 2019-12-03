using System;
using Datory;


namespace SiteServer.Abstractions
{
    [Serializable]
    [DataTable("siteserver_Template")]
    public class Template : Entity
    {
        [DataColumn] public int SiteId { get; set; }

        [DataColumn] public string TemplateName { get; set; }

        [DataColumn] public string TemplateType { get; set; }

        public TemplateType Type
        {
            get => Abstractions.TemplateType.Parse(TemplateType);
            set => TemplateType = value.Value;
        }

        [DataColumn] public string RelatedFileName { get; set; }

        [DataColumn] public string CreatedFileFullName { get; set; }

        [DataColumn] public string CreatedFileExtName { get; set; }

        [DataColumn] public string Charset { get; set; }

        public ECharset CharsetType
        {
            get => ECharsetUtils.GetEnumType(Charset);
            set => Charset = ECharsetUtils.GetValue(value);
        }

        [DataColumn] public string IsDefault { get; set; }

        public bool Default
        {
            get => TranslateUtils.ToBool(IsDefault);
            set => IsDefault = value.ToString();
        }

        public string Content { get; set; }
    }
}