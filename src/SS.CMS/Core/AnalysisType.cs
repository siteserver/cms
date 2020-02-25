using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace SS.CMS.Core
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum AnalysisType
    {
        Hour,          //小时
        Day,            //日
        Month,       //月
        Year,           //年
    }
}
