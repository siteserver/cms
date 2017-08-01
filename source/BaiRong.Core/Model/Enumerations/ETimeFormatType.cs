using System;
using System.Web.UI.WebControls;

namespace BaiRong.Core.Model.Enumerations
{
    public enum ETimeFormatType
    {
        ShortTime,					//8:09
        LongTime					//8:09:24
    }

    public class ETimeFormatTypeUtils
    {
        public static string GetValue(ETimeFormatType type)
        {
            if (type == ETimeFormatType.ShortTime)
            {
                return "ShortTime";
            }
            else if (type == ETimeFormatType.LongTime)
            {
                return "LongTime";
            }
            else
            {
                throw new Exception();
            }
        }

        public static string GetText(ETimeFormatType type)
        {
            if (type == ETimeFormatType.ShortTime)
            {
                return "8:09";
            }
            else if (type == ETimeFormatType.LongTime)
            {
                return "8:09:24";
            }
            else
            {
                throw new Exception();
            }
        }

        public static ETimeFormatType GetEnumType(string typeStr)
        {
            var retval = ETimeFormatType.ShortTime;

            if (Equals(ETimeFormatType.ShortTime, typeStr))
            {
                retval = ETimeFormatType.ShortTime;
            }
            else if (Equals(ETimeFormatType.LongTime, typeStr))
            {
                retval = ETimeFormatType.LongTime;
            }

            return retval;
        }

        public static bool Equals(ETimeFormatType type, string typeStr)
        {
            if (string.IsNullOrEmpty(typeStr)) return false;
            if (string.Equals(GetValue(type).ToLower(), typeStr.ToLower()))
            {
                return true;
            }
            return false;
        }

        public static bool Equals(string typeStr, ETimeFormatType type)
        {
            return Equals(type, typeStr);
        }

        public static ListItem GetListItem(ETimeFormatType type, bool selected)
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
                listControl.Items.Add(GetListItem(ETimeFormatType.ShortTime, false));
                listControl.Items.Add(GetListItem(ETimeFormatType.LongTime, false));
            }
        }

    }
}
