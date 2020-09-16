using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace SSCMS.Core.Utils
{
    [JsonConverter(typeof(StringEnumConverter))]
	public enum TableRule
	{
	    Choose,
	    HandWrite,
	    Create
    }
}
