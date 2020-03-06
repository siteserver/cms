using Datory.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace SS.CMS.Abstractions
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum TranslateType
    {
        [DataEnum(DisplayName = "仅转移内容")]
        Content,
        [DataEnum(DisplayName = "仅转移栏目")]
        Channel,
        [DataEnum(DisplayName = "转移栏目及内容")]
        All
    }
}
