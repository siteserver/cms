using Datory.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace SSCMS.Core.Utils
{
    [JsonConverter(typeof(StringEnumConverter))]
	public enum OraclePrivilege
	{
        [DataEnum(DisplayName = "Normal")] Normal,
        [DataEnum(DisplayName = "SYSDBA")] SYSDBA,
        [DataEnum(DisplayName = "SYSOPER")] SYSOPER
	}
}
