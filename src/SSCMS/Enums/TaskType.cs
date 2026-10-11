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

        // 云助手已移除，保留枚举值以兼容插件与历史数据
        [System.Obsolete("云助手已移除")]
        [DataEnum(DisplayName = "云同步")]
        CloudSync,

        [System.Obsolete("云助手已移除")]
        [DataEnum(DisplayName = "云备份")]
        CloudBackup,
    }
}
