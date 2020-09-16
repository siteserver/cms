using Datory.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace SSCMS.Enums
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum CreateType
    {
        [DataEnum(DisplayName = "首页")]
        Index,
        [DataEnum(DisplayName = "栏目页")]
        Channel,
        [DataEnum(DisplayName = "内容页")]
        Content,
        [DataEnum(DisplayName = "文件页")]
        File,
        [DataEnum(DisplayName = "专题页")]
        Special,
        [DataEnum(DisplayName = "栏目下所有内容页")]
        AllContent,
        [DataEnum(DisplayName = "所有页面")]
        All
    }
}
