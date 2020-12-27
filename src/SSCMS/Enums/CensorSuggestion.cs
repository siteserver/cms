using Datory.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace SSCMS.Enums
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum CensorSuggestion
    {
        [DataEnum(DisplayName = "合规")]
        Pass,
        [DataEnum(DisplayName = "疑似")]
        Review,
        [DataEnum(DisplayName = "不合规")]
        Block
    }
}
