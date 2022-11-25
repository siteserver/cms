using Datory.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace SSCMS.Enums
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum CloudType
    {
        [DataEnum(DisplayName = "免费版")] Free,
        [DataEnum(DisplayName = "基础版")] Basic,
        [DataEnum(DisplayName = "标准版")] Standard,
        [DataEnum(DisplayName = "专业版")] Professional,
    }
}
