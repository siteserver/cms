using System;
using System.Web.UI.WebControls;

namespace BaiRong.Core.Model.Enumerations
{
    public enum ELevelPeriodType
    {
        None,               //不限
        Once,               //一次
        Everyday,           //每天
        Hour,               //间隔小时
        Minute,             //间隔分钟
    }

    public class ELevelPeriodTypeUtils
    {
        public static string GetValue(ELevelPeriodType type)
        {
            if (type == ELevelPeriodType.None)
            {
                return "None";
            }
            else if (type == ELevelPeriodType.Once)
            {
                return "Once";
            }
            else if (type == ELevelPeriodType.Everyday)
            {
                return "Everyday";
            }
            else if (type == ELevelPeriodType.Hour)
            {
                return "Hour";
            }
            else if (type == ELevelPeriodType.Minute)
            {
                return "Minute";
            }
            else
            {
                throw new Exception();
            }
        }

        public static string GetText(ELevelPeriodType type)
        {
            if (type == ELevelPeriodType.None)
            {
                return "不限";
            }
            else if (type == ELevelPeriodType.Once)
            {
                return "一次";
            }
            else if (type == ELevelPeriodType.Everyday)
            {
                return "每天";
            }
            else if (type == ELevelPeriodType.Hour)
            {
                return "间隔小时";
            }
            else if (type == ELevelPeriodType.Minute)
            {
                return "间隔分钟";
            }
            else
            {
                throw new Exception();
            }
        }

        public static ELevelPeriodType GetEnumType(string typeStr)
        {
            var retval = ELevelPeriodType.None;

            if (Equals(ELevelPeriodType.None, typeStr))
            {
                retval = ELevelPeriodType.None;
            }
            else if (Equals(ELevelPeriodType.Once, typeStr))
            {
                retval = ELevelPeriodType.Once;
            }
            else if (Equals(ELevelPeriodType.Everyday, typeStr))
            {
                retval = ELevelPeriodType.Everyday;
            }
            else if (Equals(ELevelPeriodType.Hour, typeStr))
            {
                retval = ELevelPeriodType.Hour;
            }
            else if (Equals(ELevelPeriodType.Minute, typeStr))
            {
                retval = ELevelPeriodType.Minute;
            }

            return retval;
        }

        public static bool Equals(ELevelPeriodType type, string typeStr)
        {
            if (string.IsNullOrEmpty(typeStr)) return false;
            if (string.Equals(GetValue(type).ToLower(), typeStr.ToLower()))
            {
                return true;
            }
            return false;
        }

        public static bool Equals(string typeStr, ELevelPeriodType type)
        {
            return Equals(type, typeStr);
        }

        public static ListItem GetListItem(ELevelPeriodType type, bool selected)
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
                listControl.Items.Add(GetListItem(ELevelPeriodType.None, false));
                listControl.Items.Add(GetListItem(ELevelPeriodType.Once, false));
                listControl.Items.Add(GetListItem(ELevelPeriodType.Everyday, false));
                listControl.Items.Add(GetListItem(ELevelPeriodType.Hour, false));
                listControl.Items.Add(GetListItem(ELevelPeriodType.Minute, false));
            }
        }
    }
}
