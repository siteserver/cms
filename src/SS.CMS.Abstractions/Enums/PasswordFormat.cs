using Datory.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace SS.CMS.Abstractions
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum PasswordFormat
    {
        [DataEnum(DisplayName = "不加密")]
        Clear,
        [DataEnum(DisplayName = "不可逆方式加密")]
        Hashed,
        [DataEnum(DisplayName = "可逆方式加密")]
        Encrypted,
    }
}