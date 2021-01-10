using Datory.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace SSCMS.Enums
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum EditMode
    {
        [DataEnum(DisplayName = "默认模式")]
        Default,
        [DataEnum(DisplayName = "可视化编辑模式")]
        Visual,
        [DataEnum(DisplayName = "预览模式")]
        Preview
    }
}
