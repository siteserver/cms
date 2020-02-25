using Datory.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace SS.CMS.Abstractions
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum TranslateContentType
    {
        [DataEnum(DisplayName = "��������")]
        Copy,
        [DataEnum(DisplayName = "����")]
        Cut,
        [DataEnum(DisplayName = "���õ�ַ")]
        Reference,
        [DataEnum(DisplayName = "��������")]
        ReferenceContent,
    }
}
