using System;
using System.Web.UI.WebControls;

namespace SiteServer.CMS.Model.Enumerations
{
    public enum ESigninPriority
    {
        Lower,    //低
        Normal,   //普通
        High   //高
    }

    public class ESigninPriorityUtils
    {
        public static string GetValue(ESigninPriority type)
        {
            if (type == ESigninPriority.Lower)
            {
                return "1";
            }
            else if (type == ESigninPriority.Normal)
            {
                return "2";
            }
            else if (type == ESigninPriority.High)
            {
                return "3";
            }
            else
            {
                throw new Exception();
            }
        }

        public static string GetText(ESigninPriority type)
        {
            if (type == ESigninPriority.Lower)
            {
                return "低";
            }
            else if (type == ESigninPriority.Normal)
            {
                return "普通";
            }
            else if (type == ESigninPriority.High)
            {
                return "高";
            }
            else
            {
                throw new Exception();
             }
        }

        public static ESigninPriority GetEnumType(string typeStr)
        {
            var retval = ESigninPriority.Lower;

            if (Equals(ESigninPriority.Lower, typeStr))
            {
                retval = ESigninPriority.Lower;
            }
            else if (Equals(ESigninPriority.Normal, typeStr))
            {
                retval = ESigninPriority.Normal;
            }
            else if (Equals(ESigninPriority.High, typeStr))
            {
                retval = ESigninPriority.High;
            }

            return retval;
        }

        public static bool Equals(ESigninPriority type, string typeStr)
        {
            if (string.IsNullOrEmpty(typeStr)) return false;
            if (string.Equals(GetValue(type).ToLower(), typeStr.ToLower()))
            {
                return true;
            }
            return false;
        }

        public static void AddListItems(ListControl listControl)
        {
            if (listControl != null)
            {
                var item = new ListItem(GetText(ESigninPriority.Lower), GetValue(ESigninPriority.Lower));
                listControl.Items.Add(item);
                item = new ListItem(GetText(ESigninPriority.Normal), GetValue(ESigninPriority.Normal));
                listControl.Items.Add(item);
                item = new ListItem(GetText(ESigninPriority.High), GetValue(ESigninPriority.High));
                listControl.Items.Add(item);
            }
        }
    }
}
