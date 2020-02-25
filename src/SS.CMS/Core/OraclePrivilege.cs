using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace SS.CMS.Core
{
    [JsonConverter(typeof(StringEnumConverter))]
	public enum OraclePrivilege
	{
		Normal,
		SYSDBA,
		SYSOPER
	}
}
