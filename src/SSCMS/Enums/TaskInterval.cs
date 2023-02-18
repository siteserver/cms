using Datory.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace SSCMS.Enums
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum TaskInterval
    {
        [DataEnum(DisplayName = "一次")]
        Once,
        [DataEnum(DisplayName = "每小时")]
        EveryHour,
        [DataEnum(DisplayName = "每天")]
        EveryDay,
        [DataEnum(DisplayName = "每周")]
        EveryWeek,
    }
}
