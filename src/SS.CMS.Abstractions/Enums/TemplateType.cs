using Datory.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace SS.CMS.Abstractions
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum TemplateType
    {
        [DataEnum(DisplayName = "首页模板")]
        IndexPageTemplate,
        [DataEnum(DisplayName = "栏目模板")]
        ChannelTemplate,
        [DataEnum(DisplayName = "内容模板")]
        ContentTemplate,
        [DataEnum(DisplayName = "单页模板")]
        FileTemplate
    }
}