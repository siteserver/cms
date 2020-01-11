using Datory.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;


namespace SiteServer.Abstractions
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum CreateType
    {
        [DataEnum(DisplayName = "栏目页")]
        Channel,
        [DataEnum(DisplayName = "内容页")]
        Content,
        [DataEnum(DisplayName = "文件页")]
        File,
        [DataEnum(DisplayName = "专题页")]
        Special,
        [DataEnum(DisplayName = "栏目下所有内容页")]
        AllContent
    }
}
