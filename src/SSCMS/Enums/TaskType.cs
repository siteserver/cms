using Datory.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace SSCMS.Enums
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum TaskType
    {
        [DataEnum(DisplayName = "定时生成")]
        Create,

        [DataEnum(DisplayName = "监控报警(PING)")]
        Ping,

        [DataEnum(DisplayName = "定时发布")]
        Publish,

        [DataEnum(DisplayName = "云同步")]
        CloudSync,

        [DataEnum(DisplayName = "云备份")]
        CloudBackup,
    }
}
