using Datory.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace SSCMS
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum PasswordRestriction
    {
        [DataEnum(DisplayName = "不限制")]
        None,
        [DataEnum(DisplayName = "字母和数字组合")]
        LetterAndDigit,
        [DataEnum(DisplayName = "字母、数字以及符号组合")]
        LetterAndDigitAndSymbol
    }
}
