using Datory;
using SiteServer.CMS.Context.Enumerations;
using SiteServer.CMS.Core;
using SiteServer.Plugin;
using SiteServer.Utils;

namespace SiteServer.CMS.Model
{
    [DataTable("siteserver_Template")]
    public class Template : Entity
    {
        [DataColumn] public int SiteId { get; set; }

        [DataColumn] public string TemplateName { get; set; }

        [DataColumn] public string TemplateType { get; set; }

        public TemplateType Type
        {
            get => TemplateTypeUtils.GetEnumType(TemplateType);
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