using Datory.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace SSCMS.Enums
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum MaterialType
    {
        [DataEnum(DisplayName = "内容")]
        Content,
        [DataEnum(DisplayName = "图文消息")]
        Message,
        [DataEnum(DisplayName = "图文消息")]
        Article,
        [DataEnum(DisplayName = "文字")]
        Text,
        [DataEnum(DisplayName = "图片")]
        Image,
        [DataEnum(DisplayName = "视频")]
        Video,
        [DataEnum(DisplayName = "音频")]
        Audio,
        [DataEnum(DisplayName = "文件")]
        File
    }
}