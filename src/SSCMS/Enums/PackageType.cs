using Datory.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace SSCMS.Enums
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum PackageType
    {
        [DataEnum(DisplayName = "SsCms")]
        SsCms,
        [DataEnum(DisplayName = "Plugin")]
        Plugin,
        [DataEnum(DisplayName = "Library")]
        Library,
    }
}
