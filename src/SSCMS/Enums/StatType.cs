using Datory.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace SSCMS.Enums
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum StatType
    {
        [DataEnum(DisplayName = "管理员登录成功")]
        AdminLoginSuccess,
        [DataEnum(DisplayName = "管理员登录失败")]
        AdminLoginFailure,
        [DataEnum(DisplayName = "用户登录")]
        UserLogin,
        [DataEnum(DisplayName = "用户注册")]
        UserRegister,
        [DataEnum(DisplayName = "内容新增")]
        ContentAdd,
        [DataEnum(DisplayName = "内容编辑")]
        ContentEdit
    }
}
