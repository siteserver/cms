using Datory.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Datory
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum DataType
    {
        [DataEnum(DisplayName = "布尔值")] Boolean,
        [DataEnum(DisplayName = "日期")] DateTime,
        [DataEnum(DisplayName = "小数")] Decimal,
        [DataEnum(DisplayName = "整数")] Integer,
        [DataEnum(DisplayName = "备注")] Text,
        [DataEnum(DisplayName = "字符串")] VarChar
    }
}