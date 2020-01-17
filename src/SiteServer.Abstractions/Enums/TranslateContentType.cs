using Datory.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace SiteServer.Abstractions
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum TranslateContentType
    {
        [DataEnum(DisplayName = "Copy")]
        Copy,
        [DataEnum(DisplayName = "Cut")]
        Cut,
        [DataEnum(DisplayName = "Reference")]
        Reference,
        [DataEnum(DisplayName = "ReferenceContent")]
        ReferenceContent,
    }
}
