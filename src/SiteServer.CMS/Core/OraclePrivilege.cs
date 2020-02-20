using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace SiteServer.CMS.Core
{
    [JsonConverter(typeof(StringEnumConverter))]
	public enum OraclePrivilege
	{
		Normal,
		SYSDBA,
		SYSOPER
	}
}
