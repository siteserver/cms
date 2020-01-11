using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Datory.Annotations;

namespace SiteServer.Abstractions
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum LibraryType
    {
        [DataEnum(DisplayName = "Text")]
        Text,
        [DataEnum(DisplayName = "Image")]
        Image,
        [DataEnum(DisplayName = "Document")]
        Document,
        [DataEnum(DisplayName = "Audio")]
        Audio,
        [DataEnum(DisplayName = "Video")]
        Video,
        [DataEnum(DisplayName = "File")]
        File
    }
}