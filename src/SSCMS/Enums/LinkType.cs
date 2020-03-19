using Datory.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace SSCMS
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum LinkType
    {
        [DataEnum(DisplayName = "默认")] None,

        [DataEnum(DisplayName = "无内容时不可链接")] NoLinkIfContentNotExists,

        [DataEnum(DisplayName = "仅一条内容时链接到此内容")]
        LinkToOnlyOneContent,

        [DataEnum(DisplayName = "无内容时不可链接，仅一条内容时链接到此内容")]
        NoLinkIfContentNotExistsAndLinkToOnlyOneContent,
        [DataEnum(DisplayName = "链接到第一条内容")] LinkToFirstContent,

        [DataEnum(DisplayName = "无内容时不可链接，有内容时链接到第一条内容")]
        NoLinkIfContentNotExistsAndLinkToFirstContent,

        [DataEnum(DisplayName = "无栏目时不可链接")] NoLinkIfChannelNotExists,

        [DataEnum(DisplayName = "链接到最近增加的子栏目")]
        LinkToLastAddChannel,

        [DataEnum(DisplayName = "链接到第一个子栏目")] LinkToFirstChannel,

        [DataEnum(DisplayName = "无栏目时不可链接，有栏目时链接到最近增加的子栏目")]
        NoLinkIfChannelNotExistsAndLinkToLastAddChannel,

        [DataEnum(DisplayName = "无栏目时不可链接，有栏目时链接到第一个子栏目")]
        NoLinkIfChannelNotExistsAndLinkToFirstChannel,

        [DataEnum(DisplayName = "不可链接")] NoLink
    }
}
