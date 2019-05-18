using System;

namespace SiteServer.CMS.Model.Enumerations
{
    /// <summary>
    /// 批量转移类型
    /// </summary>
    public enum ETranslateType
    {
        Content,                //仅转移内容
        Channel,                //仅转移栏目
        All                     //转移栏目及内容
    }

    public class ETranslateTypeUtils
    {
        public static string GetValue(ETranslateType type)
        {
            if (type == ETranslateType.Content)
            {
                return "Content";
            }
            if (type == ETranslateType.Channel)
            {
                return "Channel";
            }
            if (type == ETranslateType.All)
            {
                return "All";
            }
            throw new Exception();
        }

        public static string GetText(ETranslateType type)
        {
            if (type == ETranslateType.Content)
            {
                return "仅转移内容";
            }
            if (type == ETranslateType.Channel)
            {
                return "仅转移栏目";
            }
            if (type == ETranslateType.All)
            {
                return "转移栏目及内容";
            }
            throw new Exception();
        }

        public static ETranslateType GetEnumType(string typeStr)
        {
            var retval = ETranslateType.Content;

            if (Equals(ETranslateType.Content, typeStr))
            {
                retval = ETranslateType.Content;
            }
            else if (Equals(ETranslateType.Channel, typeStr))
            {
                retval = ETranslateType.Channel;
            }
            else if (Equals(ETranslateType.All, typeStr))
            {
                retval = ETranslateType.All;
            }

            return retval;
        }

        public static bool Equals(ETranslateType type, string typeStr)
        {
            if (string.IsNullOrEmpty(typeStr)) return false;
            if (string.Equals(GetValue(type).ToLower(), typeStr.ToLower()))
            {
                return true;
            }
            return false;
        }

        public static bool Equals(string typeStr, ETranslateType type)
        {
            return Equals(type, typeStr);
        }
    }
}
