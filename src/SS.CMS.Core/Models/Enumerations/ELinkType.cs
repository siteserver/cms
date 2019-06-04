namespace SS.CMS.Core.Models.Enumerations
{

    public enum ELinkType
    {
        None,                                                               //默认
        NoLinkIfContentNotExists,                                           //无内容时不可链接
        LinkToOnlyOneContent,                                               //仅一条内容时链接到此内容
        NoLinkIfContentNotExistsAndLinkToOnlyOneContent,                    //无内容时不可链接，仅一条内容时链接到此内容
        LinkToFirstContent,                                                 //链接到第一条内容
        NoLinkIfContentNotExistsAndLinkToFirstContent,                      //无内容时不可链接，有内容时链接到第一条内容
        NoLinkIfChannelNotExists,                                           //无栏目时不可链接
        LinkToLastAddChannel,                                               //链接到最近增加的子栏目
        LinkToFirstChannel,                                                 //链接到第一个子栏目
        NoLinkIfChannelNotExistsAndLinkToLastAddChannel,                    //无栏目时不可链接，有栏目时链接到最近增加的子栏目
        NoLinkIfChannelNotExistsAndLinkToFirstChannel,                      //无栏目时不可链接，有栏目时链接到第一个子栏目
        NoLink                                                              //不可链接
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

            var retval = ELinkType.None;

            if (Equals(ELinkType.NoLinkIfContentNotExists, typeStr))
            {
                retval = ELinkType.NoLinkIfContentNotExists;
            }
            else if (Equals(ELinkType.LinkToOnlyOneContent, typeStr))
            {
                retval = ELinkType.LinkToOnlyOneContent;
            }
            else if (Equals(ELinkType.NoLinkIfContentNotExistsAndLinkToOnlyOneContent, typeStr))
            {
                retval = ELinkType.NoLinkIfContentNotExistsAndLinkToOnlyOneContent;
            }
            else if (Equals(ELinkType.LinkToFirstContent, typeStr))
            {
                retval = ELinkType.LinkToFirstContent;
            }
            else if (Equals(ELinkType.NoLinkIfContentNotExistsAndLinkToFirstContent, typeStr))
            {
                retval = ELinkType.NoLinkIfContentNotExistsAndLinkToFirstContent;
            }
            else if (Equals(ELinkType.NoLinkIfChannelNotExists, typeStr))
            {
                retval = ELinkType.NoLinkIfChannelNotExists;
            }
            else if (Equals(ELinkType.LinkToLastAddChannel, typeStr))
            {
                retval = ELinkType.LinkToLastAddChannel;
            }
            else if (Equals(ELinkType.LinkToFirstChannel, typeStr))
            {
                retval = ELinkType.LinkToFirstChannel;
            }
            else if (Equals(ELinkType.NoLinkIfChannelNotExistsAndLinkToLastAddChannel, typeStr))
            {
                retval = ELinkType.NoLinkIfChannelNotExistsAndLinkToLastAddChannel;
            }
            else if (Equals(ELinkType.NoLinkIfChannelNotExistsAndLinkToFirstChannel, typeStr))
            {
                retval = ELinkType.NoLinkIfChannelNotExistsAndLinkToFirstChannel;
            }
            else if (Equals(ELinkType.NoLink, typeStr))
            {
                retval = ELinkType.NoLink;
            }

            return retval;
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
