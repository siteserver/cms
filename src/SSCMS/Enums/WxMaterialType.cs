using Datory.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace SSCMS.Enums
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum WxMaterialType
    {
        [DataEnum(DisplayName = "图片，2M，支持bmp/png/jpeg/jpg/gif格式", Value = "image")]
        Image,
        [DataEnum(DisplayName = "音频，2M，播放长度不超過60s，mp3/wma/wav/amr格式", Value = "voice")]
        Voice,
        [DataEnum(DisplayName = "视频，10MB，支持MP4格式", Value = "video")]
        Video,
    }
}