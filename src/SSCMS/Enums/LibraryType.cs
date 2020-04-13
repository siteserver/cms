using Datory.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace SSCMS.Enums
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum LibraryType
    {
        [DataEnum(DisplayName = "图文")]
        Text,
        [DataEnum(DisplayName = "图片")]
        Image,
        [DataEnum(DisplayName = "文档")]
        Document,
        [DataEnum(DisplayName = "音频")]
        Audio,
        [DataEnum(DisplayName = "视频")]
        Video,
        [DataEnum(DisplayName = "文件")]
        File
    }
}