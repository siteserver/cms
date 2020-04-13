using Datory.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace SSCMS.Enums
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum TranslateContentType
    {
        [DataEnum(DisplayName = "复制")]
        Copy,
        [DataEnum(DisplayName = "剪切")]
        Cut,
        [DataEnum(DisplayName = "引用地址")]
        Reference,
        [DataEnum(DisplayName = "引用内容")]
        ReferenceContent,
    }
}
