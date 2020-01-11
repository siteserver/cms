using Datory.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace SiteServer.Abstractions
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum ELinkType
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

    public static class ELinkTypeUtils
	{
		public static string GetValue(ELinkType type)
		{
		    if (type == ELinkType.NoLinkIfContentNotExists)
		    {
		        return "NoLinkIfContentNotExists";
		    }
		    if (type == ELinkType.LinkToOnlyOneContent)
		    {
		        return "LinkToOnlyOneContent";
		    }
		    if (type == ELinkType.NoLinkIfContentNotExistsAndLinkToOnlyOneContent)
		    {
		        return "NoLinkIfContentNotExistsAndLinkToOnlyOneContent";
		    }
		    if (type == ELinkType.LinkToFirstContent)
		    {
		        return "LinkToFirstContent";
		    }
		    if (type == ELinkType.NoLinkIfContentNotExistsAndLinkToFirstContent)
		    {
		        return "NoLinkIfContentNotExistsAndLinkToFirstContent";
		    }
		    if (type == ELinkType.NoLinkIfChannelNotExists)
		    {
		        return "NoLinkIfChannelNotExists";
		    }
		    if (type == ELinkType.LinkToLastAddChannel)
		    {
		        return "LinkToLastAddChannel";
		    }
		    if (type == ELinkType.LinkToFirstChannel)
		    {
		        return "LinkToFirstChannel";
		    }
		    if (type == ELinkType.NoLinkIfChannelNotExistsAndLinkToLastAddChannel)
		    {
		        return "NoLinkIfChannelNotExistsAndLinkToLastAddChannel";
		    }
		    if (type == ELinkType.NoLinkIfChannelNotExistsAndLinkToFirstChannel)
		    {
		        return "NoLinkIfChannelNotExistsAndLinkToFirstChannel";
		    }
		    if (type == ELinkType.NoLink)
		    {
		        return "NoLink";
		    }

            return "None";
        }

	    public static string GetText(ELinkType type)
		{
		    if (type == ELinkType.NoLinkIfContentNotExists)
			{
				return "无内容时不可链接";
			}
		    if (type == ELinkType.LinkToOnlyOneContent)
		    {
		        return "仅一条内容时链接到此内容";
		    }
		    if (type == ELinkType.NoLinkIfContentNotExistsAndLinkToOnlyOneContent)
		    {
		        return "无内容时不可链接，仅一条内容时链接到此内容";
		    }
		    if (type == ELinkType.LinkToFirstContent)
		    {
		        return "链接到第一条内容";
		    }
		    if (type == ELinkType.NoLinkIfContentNotExistsAndLinkToFirstContent)
		    {
		        return "无内容时不可链接，有内容时链接到第一条内容";
		    }
		    if (type == ELinkType.NoLinkIfChannelNotExists)
		    {
		        return "无栏目时不可链接";
		    }
		    if (type == ELinkType.LinkToLastAddChannel)
		    {
		        return "链接到最近增加的子栏目";
		    }
		    if (type == ELinkType.LinkToFirstChannel)
		    {
		        return "链接到第一个子栏目";
		    }
		    if (type == ELinkType.NoLinkIfChannelNotExistsAndLinkToLastAddChannel)
		    {
		        return "无栏目时不可链接，有栏目时链接到最近增加的子栏目";
		    }
		    if (type == ELinkType.NoLinkIfChannelNotExistsAndLinkToFirstChannel)
		    {
		        return "无栏目时不可链接，有栏目时链接到第一个子栏目";
		    }
		    if (type == ELinkType.NoLink)
		    {
		        return "不可链接";
		    }

            return "默认";
        }

		public static ELinkType GetEnumType(string typeStr)
		{
		    if (string.IsNullOrEmpty(typeStr)) return ELinkType.None;

			var retVal = ELinkType.None;

			if (Equals(ELinkType.NoLinkIfContentNotExists, typeStr))
			{
				retVal = ELinkType.NoLinkIfContentNotExists;
			}
			else if (Equals(ELinkType.LinkToOnlyOneContent, typeStr))
			{
				retVal = ELinkType.LinkToOnlyOneContent;
			}
			else if (Equals(ELinkType.NoLinkIfContentNotExistsAndLinkToOnlyOneContent, typeStr))
			{
				retVal = ELinkType.NoLinkIfContentNotExistsAndLinkToOnlyOneContent;
			}
			else if (Equals(ELinkType.LinkToFirstContent, typeStr))
			{
				retVal = ELinkType.LinkToFirstContent;
			}
			else if (Equals(ELinkType.NoLinkIfContentNotExistsAndLinkToFirstContent, typeStr))
			{
				retVal = ELinkType.NoLinkIfContentNotExistsAndLinkToFirstContent;
			}
			else if (Equals(ELinkType.NoLinkIfChannelNotExists, typeStr))
			{
				retVal = ELinkType.NoLinkIfChannelNotExists;
			}
			else if (Equals(ELinkType.LinkToLastAddChannel, typeStr))
			{
				retVal = ELinkType.LinkToLastAddChannel;
			}
			else if (Equals(ELinkType.LinkToFirstChannel, typeStr))
			{
				retVal = ELinkType.LinkToFirstChannel;
			}
			else if (Equals(ELinkType.NoLinkIfChannelNotExistsAndLinkToLastAddChannel, typeStr))
			{
				retVal = ELinkType.NoLinkIfChannelNotExistsAndLinkToLastAddChannel;
			}
			else if (Equals(ELinkType.NoLinkIfChannelNotExistsAndLinkToFirstChannel, typeStr))
			{
				retVal = ELinkType.NoLinkIfChannelNotExistsAndLinkToFirstChannel;
			}
			else if (Equals(ELinkType.NoLink, typeStr))
			{
				retVal = ELinkType.NoLink;
			}

			return retVal;
		}

		public static bool Equals(ELinkType type, string typeStr)
		{
			if (string.IsNullOrEmpty(typeStr)) return false;
			if (string.Equals(GetValue(type).ToLower(), typeStr.ToLower()))
			{
				return true;
			}
			return false;
		}

        public static bool Equals(string typeStr, ELinkType type)
        {
            return Equals(type, typeStr);
        }
	}
}
