using System;
using System.Web.UI.WebControls;

namespace SiteServer.Utils.Enumerations
{
    public enum EDateFormatType
    {
        Month,				//6月18日
        Day,				//2006-6-18
        Year,				//2006年6月
        Chinese,		    //2006年6月18日
    }

    public class EDateFormatTypeUtils
    {
        public static string GetValue(EDateFormatType type)
        {
            if (type == EDateFormatType.Month)
            {
                return "Month";
            }
            if (type == EDateFormatType.Day)
            {
                return "Day";
            }
            if (type == EDateFormatType.Year)
            {
                return "Year";
            }
            if (type == EDateFormatType.Chinese)
            {
                return "Chinese";
            }
            throw new Exception();
        }

        public static string GetText(EDateFormatType type)
        {
            if (type == EDateFormatType.Month)
            {
                return "6月18日";
            }
            if (type == EDateFormatType.Day)
            {
                return "2006-6-18";
            }
            if (type == EDateFormatType.Year)
            {
                return "2006年6月";
            }
            if (type == EDateFormatType.Chinese)
            {
                return "2006年6月18日";
            }
            throw new Exception();
        }

        public static EDateFormatType GetEnumType(string typeStr)
        {
            var retval = EDateFormatType.Month;

            if (Equals(EDateFormatType.Month, typeStr))
            {
                retval = EDateFormatType.Month;
            }
            else if (Equals(EDateFormatType.Day, typeStr))
            {
                retval = EDateFormatType.Day;
            }
            else if (Equals(EDateFormatType.Year, typeStr))
            {
                retval = EDateFormatType.Year;
            }
            else if (Equals(EDateFormatType.Chinese, typeStr))
            {
                retval = EDateFormatType.Chinese;
            }

            return retval;
        }

        public static bool Equals(EDateFormatType type, string typeStr)
        {
            if (string.IsNullOrEmpty(typeStr)) return false;
            if (string.Equals(GetValue(type).ToLower(), typeStr.ToLower()))
            {
                return true;
            }
            return false;
        }

        public static bool Equals(string typeStr, EDateFormatType type)
        {
            return Equals(type, typeStr);
        }

        public static ListItem GetListItem(EDateFormatType type, bool selected)
        {
            var item = new ListItem(GetText(type), GetValue(type));
            if (selected)
            {
                item.Selected = true;
            }
            return item;
        }

        public static void AddListItems(ListControl listControl)
        {
            if (listControl != null)
            {
                listControl.Items.Add(GetListItem(EDateFormatType.Month, false));
                listControl.Items.Add(GetListItem(EDateFormatType.Day, false));
                listControl.Items.Add(GetListItem(EDateFormatType.Year, false));
                listControl.Items.Add(GetListItem(EDateFormatType.Chinese, false));
            }
        }

    }
}
