using System;
using System.Web.UI.WebControls;

namespace BaiRong.Core.Model.Enumerations
{
    public enum ETableStyle
    {
        BackgroundContent,	    //内容        
        Custom,                 //自定义
        Channel,			    //栏目
        InputContent,           //提交表单
        Site,                   //站点
    }

    public class ETableStyleUtils
    {
        public static string GetValue(ETableStyle type)
        {
            if (type == ETableStyle.BackgroundContent)
            {
                return "BackgroundContent";
            }
            if (type == ETableStyle.Custom)
            {
                return "Custom";
            }
            if (type == ETableStyle.Channel)
            {
                return "Channel";
            }
            if (type == ETableStyle.InputContent)
            {
                return "InputContent";
            }
            if (type == ETableStyle.Site)
            {
                return "Site";
            }
            throw new Exception();
        }

        public static string GetText(ETableStyle type)
        {
            if (type == ETableStyle.BackgroundContent)
            {
                return "内容";
            }
            if (type == ETableStyle.Custom)
            {
                return "自定义";
            }
            if (type == ETableStyle.Channel)
            {
                return "栏目";
            }
            if (type == ETableStyle.InputContent)
            {
                return "提交表单";
            }
            if (type == ETableStyle.Site)
            {
                return "站点";
            }
            throw new Exception();
        }

        public static ETableStyle GetEnumType(string typeStr)
        {
            var retval = ETableStyle.BackgroundContent;

            if (Equals(ETableStyle.BackgroundContent, typeStr))
            {
                retval = ETableStyle.BackgroundContent;
            }
            else if (Equals(ETableStyle.Custom, typeStr))
            {
                retval = ETableStyle.Custom;
            }
            else if (Equals(ETableStyle.Channel, typeStr))
            {
                retval = ETableStyle.Channel;
            }
            else if (Equals(ETableStyle.InputContent, typeStr))
            {
                retval = ETableStyle.InputContent;
            }
            else if (Equals(ETableStyle.Site, typeStr))
            {
                retval = ETableStyle.Site;
            }
            return retval;
        }

        public static bool Equals(ETableStyle type, string typeStr)
        {
            if (string.IsNullOrEmpty(typeStr)) return false;
            if (string.Equals(GetValue(type).ToLower(), typeStr.ToLower()))
            {
                return true;
            }
            return false;
        }

        public static bool Equals(string typeStr, ETableStyle type)
        {
            return Equals(type, typeStr);
        }

        public static ListItem GetListItem(ETableStyle type, bool selected)
        {
            var item = new ListItem(GetText(type), GetValue(type));
            if (selected)
            {
                item.Selected = true;
            }
            return item;
        }

        public static bool IsNodeRelated(ETableStyle tableStyle)
        {
            if (tableStyle == ETableStyle.BackgroundContent || tableStyle == ETableStyle.Custom || tableStyle == ETableStyle.Channel)
            {
                return true;
            }
            return false;
        }

        public static bool IsContent(ETableStyle tableStyle)
        {
            if (tableStyle == ETableStyle.BackgroundContent || tableStyle == ETableStyle.Custom)
            {
                return true;
            }
            return false;
        }

        public static EAuxiliaryTableType GetTableType(ETableStyle tableStyle)
        {
            if (tableStyle == ETableStyle.Custom)
            {
                return EAuxiliaryTableType.Custom;
            }
            return EAuxiliaryTableType.BackgroundContent;
        }

        public static ETableStyle GetStyleType(EAuxiliaryTableType tableType)
        {
            if (tableType == EAuxiliaryTableType.Custom)
            {
                return ETableStyle.Custom;
            }
            return ETableStyle.BackgroundContent;
        }
    }
}
