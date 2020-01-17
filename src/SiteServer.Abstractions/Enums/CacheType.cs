using System;
using Datory.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;


namespace SiteServer.Abstractions
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum CacheType
    {
        [DataEnum(DisplayName = "Memory")]
        Memory,
        [DataEnum(DisplayName = "Redis")]
        Redis,
        [DataEnum(DisplayName = "SqlServer")]
        SqlServer,
    }
}
