using System.Text.Json.Serialization;
using Datory.Annotations;
// using Newtonsoft.Json;
// using Newtonsoft.Json.Converters;

namespace SSCMS.Enums
{
    // [JsonConverter(typeof(StringEnumConverter))]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum BadWordsType
    {
        [DataEnum(DisplayName = "违禁违规")] Illegal,
        [DataEnum(DisplayName = "文本色情")] Porn,
        [DataEnum(DisplayName = "政治敏感")] Sensitive,
        [DataEnum(DisplayName = "低俗辱骂")] Vulgar,
        [DataEnum(DisplayName = "低质灌水")] Irrigation,
        [DataEnum(DisplayName = "恶意推广")] Promotion,
    }
}