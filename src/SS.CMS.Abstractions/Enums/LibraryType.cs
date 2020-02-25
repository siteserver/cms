using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Datory.Annotations;

namespace SS.CMS.Abstractions
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum LibraryType
    {
        [DataEnum(DisplayName = "ͼ��")]
        Text,
        [DataEnum(DisplayName = "ͼƬ")]
        Image,
        [DataEnum(DisplayName = "�ĵ�")]
        Document,
        [DataEnum(DisplayName = "��Ƶ")]
        Audio,
        [DataEnum(DisplayName = "��Ƶ")]
        Video,
        [DataEnum(DisplayName = "�ļ�")]
        File
    }
}