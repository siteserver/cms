using System;

namespace SiteServer.CMS.Model.Enumerations
{
    public enum EKeywordGrade
    {
        Normal,         //一般
        Sensitive,      //比较敏感
        Dangerous       //危险
    }

    public class EKeywordGradeUtils
    {
        public static string GetValue(EKeywordGrade type)
        {
            if (type == EKeywordGrade.Normal)
            {
                return "Normal";
            }
            if (type == EKeywordGrade.Sensitive)
            {
                return "Sensitive";
            }
            if (type == EKeywordGrade.Dangerous)
            {
                return "Dangerous";
            }
            throw new Exception();
        }

        public static string GetText(EKeywordGrade type)
        {
            if (type == EKeywordGrade.Normal)
            {
                return "一般";
            }
            if (type == EKeywordGrade.Sensitive)
            {
                return "比较敏感";
            }
            if (type == EKeywordGrade.Dangerous)
            {
                return "危险";
            }
            throw new Exception();
        }

        public static EKeywordGrade GetEnumType(string typeStr)
        {
            var retval = EKeywordGrade.Normal;

            if (Equals(EKeywordGrade.Normal, typeStr))
            {
                retval = EKeywordGrade.Normal;
            }
            else if (Equals(EKeywordGrade.Sensitive, typeStr))
            {
                retval = EKeywordGrade.Sensitive;
            }
            else if (Equals(EKeywordGrade.Dangerous, typeStr))
            {
                retval = EKeywordGrade.Dangerous;
            }

            return retval;
        }

        public static bool Equals(EKeywordGrade type, string typeStr)
        {
            if (string.IsNullOrEmpty(typeStr)) return false;
            if (string.Equals(GetValue(type).ToLower(), typeStr.ToLower()))
            {
                return true;
            }
            return false;
        }

        public static bool Equals(string typeStr, EKeywordGrade type)
        {
            return Equals(type, typeStr);
        }
    }
}
