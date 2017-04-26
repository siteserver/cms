using System;
using System.Web.UI.WebControls;

namespace BaiRong.Core.Model.Enumerations
{
    public enum EUserVerifyType
    {
        None,
        Mobile
    }

    public class EUserVerifyTypeUtils
    {
        public static string GetValue(EUserVerifyType type)
        {
            if (type == EUserVerifyType.None)
            {
                return "None";
            }
            else if (type == EUserVerifyType.Mobile)
            {
                return "Mobile";
            }
            else
            {
                throw new Exception();
            }
        }

        public static string GetText(EUserVerifyType type)
        {
            if (type == EUserVerifyType.None)
            {
                return "无验证";
            }
            else if (type == EUserVerifyType.Mobile)
            {
                return "短信验证";
            }
            else
            {
                throw new Exception();
            }
        }

        public static EUserVerifyType GetEnumType(string typeStr)
        {
            var retval = EUserVerifyType.None;

            if (Equals(EUserVerifyType.None, typeStr))
            {
                retval = EUserVerifyType.None;
            }
            else if (Equals(EUserVerifyType.Mobile, typeStr))
            {
                retval = EUserVerifyType.Mobile;
            }

            return retval;
        }

        public static bool Equals(EUserVerifyType type, string typeStr)
        {
            if (string.IsNullOrEmpty(typeStr)) return false;
            if (string.Equals(GetValue(type).ToLower(), typeStr.ToLower()))
            {
                return true;
            }
            return false;
        }

        public static bool Equals(string typeStr, EUserVerifyType type)
        {
            return Equals(type, typeStr);
        }

        public static ListItem GetListItem(EUserVerifyType type, bool selected)
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
                listControl.Items.Add(GetListItem(EUserVerifyType.None, false));
                listControl.Items.Add(GetListItem(EUserVerifyType.Mobile, false));
            }
        }
    }
}
