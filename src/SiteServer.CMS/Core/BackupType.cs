using Datory.Annotations;

namespace SiteServer.CMS.Core
{
    public enum BackupType
    {
        [DataEnum(DisplayName = "显示模板")] Templates,
        [DataEnum(DisplayName = "栏目及内容")] ChannelsAndContents,
        [DataEnum(DisplayName = "文件")] Files,
        [DataEnum(DisplayName = "整站")] Site,
    }
}
