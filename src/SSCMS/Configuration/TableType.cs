using Datory.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace SSCMS.Configuration
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum TableType
    {
        [DataEnum] Default,
        [DataEnum] Content,
        [DataEnum] Custom,
    }
}
