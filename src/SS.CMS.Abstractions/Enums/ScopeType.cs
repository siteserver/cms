using Datory.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;


namespace SS.CMS.Abstractions
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum ScopeType
    {
        [DataEnum(DisplayName = "Self")]
        Self,
        [DataEnum(DisplayName = "Children")]
        Children,
        [DataEnum(DisplayName = "SelfAndChildren")]
        SelfAndChildren,
        [DataEnum(DisplayName = "Descendant")]
        Descendant,
        [DataEnum(DisplayName = "All")]
        All,
    }
}
