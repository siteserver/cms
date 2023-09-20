using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Datory.Annotations;

namespace Datory
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum DatabaseType
    {
        [DataEnum(DisplayName = "MySql")] MySql,
        [DataEnum(DisplayName = "SqlServer")] SqlServer,
        [DataEnum(DisplayName = "PostgreSql")] PostgreSql,
        [DataEnum(DisplayName = "SQLite")] SQLite,
        [DataEnum(DisplayName = "华为Gauss")] Gauss,
        [DataEnum(DisplayName = "达梦")] Dm,
        [DataEnum(DisplayName = "人大金仓")] KingbaseES,
    }
}