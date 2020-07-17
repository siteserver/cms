using Datory.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace SSCMS.Enums
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum ValidateType
    {
        [DataEnum(DisplayName = "无验证")]
        None,
        [DataEnum(DisplayName = "字段为必填项")]
        Required,
        [DataEnum(DisplayName = "字段必须是有效的电子邮件")]
        Email,
        [DataEnum(DisplayName = "字段必须是有效的手机号码")]
        Mobile,
        [DataEnum(DisplayName = "字段必须是有效的url")]
        Url,
        [DataEnum(DisplayName = "字段只能包含英文字母")]
        Alpha,
        [DataEnum(DisplayName = "字段只能包含英文字母、数字、破折号或下划线")]
        AlphaDash,
        [DataEnum(DisplayName = "字段只能包含英文字母或数字")]
        AlphaNum,
        [DataEnum(DisplayName = "字段只能包含英文字母或空格")]
        AlphaSpaces,
        [DataEnum(DisplayName = "字段必须是有效的信用卡")]
        CreditCard,
        [DataEnum(DisplayName = "字段必须有一个以最小值和最大值为界的数值")]
        Between,
        [DataEnum(DisplayName = "字段必须是数字，并且可能包含指定数量的小数点")]
        Decimal,
        [DataEnum(DisplayName = "字段必须是整数，并且具有指定的位数")]
        Digits,
        [DataEnum(DisplayName = "字段必须具有指定列表中的值")]
        Included,
        [DataEnum(DisplayName = "字段不能具有指定列表中的值")]
        Excluded,
        [DataEnum(DisplayName = "字段不能超过指定的长度")]
        Max,
        [DataEnum(DisplayName = "字段必须是数值，并且不能大于指定的值")]
        MaxValue,
        [DataEnum(DisplayName = "字段不能低于指定的长度")]
        Min,
        [DataEnum(DisplayName = "字段必须是数值，并且不能小于指定的值")]
        MinValue,
        [DataEnum(DisplayName = "字段必须匹配指定的正则表达式")]
        Regex,
        [DataEnum(DisplayName = "字段必须是中文")]
        Chinese,
        [DataEnum(DisplayName = "字段必须是货币格式")]
        Currency,
        [DataEnum(DisplayName = "字段必须是邮政编码")]
        Zip,
        [DataEnum(DisplayName = "字段必须是身份证号码")]
        IdCard
    }
}