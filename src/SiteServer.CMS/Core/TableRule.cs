using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace SiteServer.CMS.Core
{
    [JsonConverter(typeof(StringEnumConverter))]
	public enum TableRule
	{
	    Choose,
	    HandWrite,
	    Create
    }
}
