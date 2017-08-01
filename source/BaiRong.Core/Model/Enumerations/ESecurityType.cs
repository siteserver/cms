using System;
using System.Web.UI.WebControls;

namespace BaiRong.Core.Model.Enumerations
{
    public enum ESecurityType
    {
        Public,
        Friends,
        SelfOnly
    }

    public class ESecurityTypeUtils
    {
        public static string GetValue(ESecurityType type)
        {
            if (type == ESecurityType.Public)
            {
                return "Public";
            }
            else if (type == ESecurityType.Friends)
            {
                return "Friends";
            }
            else if (type == ESecurityType.SelfOnly)
            {
                return "SelfOnly";
            }
            else
            {
                throw new Exception();
            }
        }

        public static string GetText(ESecurityType type)
        {
            if (type == ESecurityType.Public)
            {
                return "所有人";
            }
            else if (type == ESecurityType.Friends)
            {
                return "我的好友";
            }
            else if (type == ESecurityType.SelfOnly)
            {
                return "只有我自己";
            }
            else
            {
                throw new Exception();
            }
        }

        public static ESecurityType GetEnumType(string typeStr)
        {
            var retval = ESecurityType.SelfOnly;

            if (Equals(ESecurityType.Public, typeStr))
            {
                retval = ESecurityType.Public;
            }
            else if (Equals(ESecurityType.Friends, typeStr))
            {
                retval = ESecurityType.Friends;
            }
            else if (Equals(ESecurityType.SelfOnly, typeStr))
            {
                retval = ESecurityType.SelfOnly;
            }

            return retval;
        }

        public static bool Equals(ESecurityType type, string typeStr)
        {
            if (string.IsNullOrEmpty(typeStr)) return false;
            if (string.Equals(GetValue(type).ToLower(), typeStr.ToLower()))
            {
                return true;
            }
            return false;
        }

        public static bool Equals(string typeStr, ESecurityType type)
        {
            return Equals(type, typeStr);
        }

        public static ListItem GetListItem(ESecurityType type, bool selected)
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
                listControl.Items.Add(GetListItem(ESecurityType.Public, false));
                listControl.Items.Add(GetListItem(ESecurityType.Friends, false));
                listControl.Items.Add(GetListItem(ESecurityType.SelfOnly, false));
            }
        }
    }
}
