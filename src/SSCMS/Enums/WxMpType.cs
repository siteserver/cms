using Datory.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace SSCMS.Enums
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum WxMpType
    {
        [DataEnum(DisplayName = "未认证订阅号")]
        Subscription,

        [DataEnum(DisplayName = "已认证订阅号")]
        SubscriptionAuthenticated,

        [DataEnum(DisplayName = "未认证服务号")]
        Service,

        [DataEnum(DisplayName = "已认证服务号")]
        ServiceAuthenticated
    }
}
