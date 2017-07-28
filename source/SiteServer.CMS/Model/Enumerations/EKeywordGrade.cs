using System;
using System.Web.UI.WebControls;

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
            else if (type == EKeywordGrade.Sensitive)
            {
                return "Sensitive";
            }
            else if (type == EKeywordGrade.Dangerous)
            {
                return "Dangerous";
            }
            else
            {
                throw new Exception();
            }
        }

        public static string GetText(EKeywordGrade type)
        {
            if (type == EKeywordGrade.Normal)
            {
                return "一般";
            }
            else if (type == EKeywordGrade.Sensitive)
            {
                return "比较敏感";
            }
            else if (type == EKeywordGrade.Dangerous)
            {
                return "危险";
            }
            else
            {
                throw new Exception();
            }
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

        public static void AddListItems(ListControl listControl)
        {
            if (listControl != null)
            {
                var item = new ListItem(GetText(EKeywordGrade.Normal), GetValue(EKeywordGrade.Normal));
                listControl.Items.Add(item);
                item = new ListItem(GetText(EKeywordGrade.Sensitive), GetValue(EKeywordGrade.Sensitive));
                listControl.Items.Add(item);
                item = new ListItem(GetText(EKeywordGrade.Dangerous), GetValue(EKeywordGrade.Dangerous));
                listControl.Items.Add(item);
            }
        }
    }
}
