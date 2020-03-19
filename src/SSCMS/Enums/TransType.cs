using Datory.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace SSCMS
{
    [JsonConverter(typeof(StringEnumConverter))]
	public enum TransType
	{
		[DataEnum(DisplayName = "不转发")]
        None,
        [DataEnum(DisplayName = "可向本站转发")]
        SelfSite,
		[DataEnum(DisplayName = "可向指定站点转发")]
        SpecifiedSite,
		[DataEnum(DisplayName = "可向上一级站点转发")]
        ParentSite,
		[DataEnum(DisplayName = "可向所有上级站点转发")]
        AllParentSite,
		[DataEnum(DisplayName = "可向所有站点转发")]
        AllSite
	}
}
