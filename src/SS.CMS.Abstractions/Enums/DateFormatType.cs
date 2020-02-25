using Datory.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace SS.CMS.Abstractions
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum DateFormatType
    {
        [DataEnum(DisplayName = "6月18日")]
        Month,
        [DataEnum(DisplayName = "2006-6-18")]
        Day,
        [DataEnum(DisplayName = "2006年6月")]
        Year,
        [DataEnum(DisplayName = "2006年6月18日")]
        Chinese,
    }
}
