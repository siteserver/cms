using System;
using System.Web.UI.WebControls;

namespace BaiRong.Core.Model.Enumerations
{
    public enum EMonth
    {
        LastMonth,  //近一个月
        MonthAgo    //一个月前
    }

    public class EMonthUtils
    {
        public static string GetValue(EMonth type)
        {
            if (type == EMonth.LastMonth)
            {
                return "LastMonth";
            }
            else if (type == EMonth.MonthAgo)
            {
                return "MonthAgo";
            }
            else
            {
                throw new Exception();
            }
        }

        //public static string GetText(EMonth type,  string trueText, string falseText)
        //{
        //    if (type == EMonth.LastMonth)
        //    {
        //        return allText;
        //    }
        //    else if (type == EMonth.LastMonth)
        //    {
        //        return falseText;
        //    }
        //    else
        //    {
        //        return trueText;
        //    }
        //}

        public static EMonth GetEnumType(string typeStr)
        {
            var retval = EMonth.LastMonth;

            if (Equals(EMonth.LastMonth, typeStr))
            {
                retval = EMonth.LastMonth;
            }
            else if (Equals(EMonth.MonthAgo, typeStr))
            {
                retval = EMonth.MonthAgo;
            }

            return retval;
        }

        public static bool Equals(EMonth type, string typeStr)
        {
            if (string.IsNullOrEmpty(typeStr)) return false;
            if (string.Equals(GetValue(type).ToLower(), typeStr.ToLower()))
            {
                return true;
            }
            return false;
        }

        public static bool Equals(string typeStr, EMonth type)
        {
            return Equals(type, typeStr);
        }

        public static void AddListItems(ListControl listControl, string trueText, string falseText)
        {
            if (listControl != null)
            {
                var item = new ListItem(trueText, GetValue(EMonth.LastMonth));
                listControl.Items.Add(item);
                item = new ListItem(falseText, GetValue(EMonth.MonthAgo));
                listControl.Items.Add(item);
            }
        }
    }
}
