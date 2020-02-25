using Datory.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace SS.CMS.Core
{
    [JsonConverter(typeof(StringEnumConverter))]
	public enum PredefinedRole
	{
		[DataEnum(DisplayName = "超级管理员")]
        ConsoleAdministrator,
        [DataEnum(DisplayName = "站点管理员")]
		SystemAdministrator,
        [DataEnum(DisplayName = "普通管理员")]
		Administrator
	}
}
