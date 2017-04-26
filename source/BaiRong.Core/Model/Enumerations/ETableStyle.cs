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
        UserDefined,            //自定义
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
            else if (type == ETableStyle.GovPublicContent)
            {
                return "GovPublicContent";
            }
            else if (type == ETableStyle.GovInteractContent)
            {
                return "GovInteractContent";
            }
            else if (type == ETableStyle.VoteContent)
            {
                return "VoteContent";
            }
            else if (type == ETableStyle.JobContent)
            {
                return "JobContent";
            }
            else if (type == ETableStyle.UserDefined)
            {
                return "UserDefined";
            }
            else if (type == ETableStyle.Channel)
            {
                return "Channel";
            }
            else if (type == ETableStyle.InputContent)
            {
                return "InputContent";
            }
            else if (type == ETableStyle.Site)
            {
                return "Site";
            }
            else
            {
                throw new Exception();
            }
        }

        public static string GetText(ETableStyle type)
        {
            if (type == ETableStyle.BackgroundContent)
            {
                return "内容";
            }
            else if (type == ETableStyle.GovPublicContent)
            {
                return "信息公开";
            }
            else if (type == ETableStyle.GovInteractContent)
            {
                return "互动交流";
            }
            else if (type == ETableStyle.VoteContent)
            {
                return "投票";
            }
            else if (type == ETableStyle.JobContent)
            {
                return "招聘";
            }
            else if (type == ETableStyle.UserDefined)
            {
                return "自定义";
            }
            else if (type == ETableStyle.Channel)
            {
                return "栏目";
            }
            else if (type == ETableStyle.InputContent)
            {
                return "提交表单";
            }
            else if (type == ETableStyle.Site)
            {
                return "站点";
            }
            else
            {
                throw new Exception();
            }
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
            else if (Equals(ETableStyle.UserDefined, typeStr))
            {
                retval = ETableStyle.UserDefined;
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
            if (tableStyle == ETableStyle.BackgroundContent || tableStyle == ETableStyle.GovPublicContent || tableStyle == ETableStyle.GovInteractContent || tableStyle == ETableStyle.VoteContent || tableStyle == ETableStyle.JobContent || tableStyle == ETableStyle.UserDefined || tableStyle == ETableStyle.Channel)
            {
                return true;
            }
            return false;
        }

        public static bool IsContent(ETableStyle tableStyle)
        {
            if (tableStyle == ETableStyle.BackgroundContent || tableStyle == ETableStyle.GovPublicContent || tableStyle == ETableStyle.GovInteractContent || tableStyle == ETableStyle.VoteContent || tableStyle == ETableStyle.JobContent || tableStyle == ETableStyle.UserDefined)
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
            else if (tableStyle == ETableStyle.GovInteractContent)
            {
                return EAuxiliaryTableType.GovInteractContent;
            }
            else if (tableStyle == ETableStyle.VoteContent)
            {
                return EAuxiliaryTableType.VoteContent;
            }
            else if (tableStyle == ETableStyle.JobContent)
            {
                return EAuxiliaryTableType.JobContent;
            }
            else if (tableStyle == ETableStyle.UserDefined)
            {
                return EAuxiliaryTableType.UserDefined;
            }
            return EAuxiliaryTableType.BackgroundContent;
        }

        public static ETableStyle GetStyleType(EAuxiliaryTableType tableType)
        {
            if (tableType == EAuxiliaryTableType.GovPublicContent)
            {
                return ETableStyle.GovPublicContent;
            }
            else if (tableType == EAuxiliaryTableType.GovInteractContent)
            {
                return ETableStyle.GovInteractContent;
            }
            else if (tableType == EAuxiliaryTableType.VoteContent)
            {
                return ETableStyle.VoteContent;
            }
            else if (tableType == EAuxiliaryTableType.JobContent)
            {
                return ETableStyle.JobContent;
            }
            else if (tableType == EAuxiliaryTableType.UserDefined)
            {
                return ETableStyle.UserDefined;
            }
            return ETableStyle.BackgroundContent;
        }
    }
}
