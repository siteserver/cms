using System;
using System.Web.UI.WebControls;

namespace BaiRong.Core.Model.Enumerations
{
    public enum EPasswordFormat
    {
        Clear,
        Hashed,
        Encrypted
    }

    public class EPasswordFormatUtils
    {
        public static string GetValue(EPasswordFormat type)
        {
            if (type == EPasswordFormat.Clear)
            {
                return "Clear";
            }
            else if (type == EPasswordFormat.Hashed)
            {
                return "Hashed";
            }
            else if (type == EPasswordFormat.Encrypted)
            {
                return "Encrypted";
            }
            else
            {
                throw new Exception();
            }
        }

        public static string GetText(EPasswordFormat type)
        {
            if (type == EPasswordFormat.Clear)
            {
                return "不加密";
            }
            else if (type == EPasswordFormat.Hashed)
            {
                return "不可逆方式加密";
            }
            else if (type == EPasswordFormat.Encrypted)
            {
                return "可逆方式加密";
            }
            else
            {
                throw new Exception();
            }
        }

        public static EPasswordFormat GetEnumType(string typeStr)
        {
            var retval = EPasswordFormat.Clear;

            if (Equals(EPasswordFormat.Clear, typeStr))
            {
                retval = EPasswordFormat.Clear;
            }
            else if (Equals(EPasswordFormat.Hashed, typeStr))
            {
                retval = EPasswordFormat.Hashed;
            }
            else if (Equals(EPasswordFormat.Encrypted, typeStr))
            {
                retval = EPasswordFormat.Encrypted;
            }

            return retval;
        }

        public static bool Equals(EPasswordFormat type, string typeStr)
        {
            if (string.IsNullOrEmpty(typeStr)) return false;
            if (string.Equals(GetValue(type).ToLower(), typeStr.ToLower()))
            {
                return true;
            }
            return false;
        }

        public static bool Equals(string typeStr, EPasswordFormat type)
        {
            return Equals(type, typeStr);
        }

        public static ListItem GetListItem(EPasswordFormat type, bool selected)
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
                listControl.Items.Add(GetListItem(EPasswordFormat.Clear, false));
                listControl.Items.Add(GetListItem(EPasswordFormat.Encrypted, false));
                listControl.Items.Add(GetListItem(EPasswordFormat.Hashed, false));
            }
        }
    }
}
