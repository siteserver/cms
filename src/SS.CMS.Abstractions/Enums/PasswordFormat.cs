using Datory.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace SS.CMS.Abstractions
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum PasswordFormat
    {
        [DataEnum(DisplayName = "������")]
        Clear,
        [DataEnum(DisplayName = "�����淽ʽ����")]
        Hashed,
        [DataEnum(DisplayName = "���淽ʽ����")]
        Encrypted,
    }
}