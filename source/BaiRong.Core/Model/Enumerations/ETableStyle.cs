using System;
using System.Web.UI.WebControls;

namespace BaiRong.Core.Model.Enumerations
{
    public enum ETableStyle
    {
        BackgroundContent,	    //内容        
        GovPublicContent,	    //信息公开
        GovInteractContent,	    //互动交流
        VoteContent,	        //投票
        JobContent,	            //招聘
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
            if (type == ETableStyle.GovPublicContent)
            {
                return "GovPublicContent";
            }
            if (type == ETableStyle.GovInteractContent)
            {
                return "GovInteractContent";
            }
            if (type == ETableStyle.VoteContent)
            {
                return "VoteContent";
            }
            if (type == ETableStyle.JobContent)
            {
                return "JobContent";
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
            if (type == ETableStyle.GovPublicContent)
            {
                return "信息公开";
            }
            if (type == ETableStyle.GovInteractContent)
            {
                return "互动交流";
            }
            if (type == ETableStyle.VoteContent)
            {
                return "投票";
            }
            if (type == ETableStyle.JobContent)
            {
                return "招聘";
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
            else if (Equals(ETableStyle.GovPublicContent, typeStr))
            {
                retval = ETableStyle.GovPublicContent;
            }
            else if (Equals(ETableStyle.GovInteractContent, typeStr))
            {
                retval = ETableStyle.GovInteractContent;
            }
            else if (Equals(ETableStyle.VoteContent, typeStr))
            {
                retval = ETableStyle.VoteContent;
            }
            else if (Equals(ETableStyle.JobContent, typeStr))
            {
                retval = ETableStyle.JobContent;
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
            if (tableStyle == ETableStyle.BackgroundContent || tableStyle == ETableStyle.GovPublicContent || tableStyle == ETableStyle.GovInteractContent || tableStyle == ETableStyle.VoteContent || tableStyle == ETableStyle.JobContent || tableStyle == ETableStyle.Custom || tableStyle == ETableStyle.Channel)
            {
                return true;
            }
            return false;
        }

        public static bool IsContent(ETableStyle tableStyle)
        {
            if (tableStyle == ETableStyle.BackgroundContent || tableStyle == ETableStyle.GovPublicContent || tableStyle == ETableStyle.GovInteractContent || tableStyle == ETableStyle.VoteContent || tableStyle == ETableStyle.JobContent || tableStyle == ETableStyle.Custom)
            {
                return true;
            }
            return false;
        }

        public static EAuxiliaryTableType GetTableType(ETableStyle tableStyle)
        {
            if (tableStyle == ETableStyle.GovPublicContent)
            {
                return EAuxiliaryTableType.GovPublicContent;
            }
            if (tableStyle == ETableStyle.GovInteractContent)
            {
                return EAuxiliaryTableType.GovInteractContent;
            }
            if (tableStyle == ETableStyle.VoteContent)
            {
                return EAuxiliaryTableType.VoteContent;
            }
            if (tableStyle == ETableStyle.JobContent)
            {
                return EAuxiliaryTableType.JobContent;
            }
            if (tableStyle == ETableStyle.Custom)
            {
                return EAuxiliaryTableType.Custom;
            }
            return EAuxiliaryTableType.BackgroundContent;
        }

        public static ETableStyle GetStyleType(EAuxiliaryTableType tableType)
        {
            if (tableType == EAuxiliaryTableType.GovPublicContent)
            {
                return ETableStyle.GovPublicContent;
            }
            if (tableType == EAuxiliaryTableType.GovInteractContent)
            {
                return ETableStyle.GovInteractContent;
            }
            if (tableType == EAuxiliaryTableType.VoteContent)
            {
                return ETableStyle.VoteContent;
            }
            if (tableType == EAuxiliaryTableType.JobContent)
            {
                return ETableStyle.JobContent;
            }
            if (tableType == EAuxiliaryTableType.Custom)
            {
                return ETableStyle.Custom;
            }
            return ETableStyle.BackgroundContent;
        }
    }
}
