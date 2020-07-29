using Datory.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace SSCMS.Enums
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum CommentType
    {
        [DataEnum(DisplayName = "禁止留言")] Block,
        [DataEnum(DisplayName = "所有人均可留言")] Everyone,
        [DataEnum(DisplayName = "仅关注后可留言")] OnlyFans
    }
}
