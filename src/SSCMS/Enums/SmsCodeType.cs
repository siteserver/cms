using Datory.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace SSCMS.Enums
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum SmsCodeType
    {
        [DataEnum(DisplayName = "身份验证")] Authentication,

        [DataEnum(DisplayName = "登录确认")] LoginConfirmation,

        [DataEnum(DisplayName = "登录异常")] AbnormalLogin,

        [DataEnum(DisplayName = "用户注册")] Registration,

        [DataEnum(DisplayName = "修改密码")] ChangePassword,

        [DataEnum(DisplayName = "信息变更")] InformationChanges
    }
}
