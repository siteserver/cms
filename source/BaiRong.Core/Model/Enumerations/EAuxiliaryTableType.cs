using System;
using System.Web.UI.WebControls;

namespace BaiRong.Core.Model.Enumerations
{
	public enum EAuxiliaryTableType
	{
        BackgroundContent,	    //内容
        GovPublicContent,	    //信息公开
        GovInteractContent,     //互动交流
        VoteContent,            //投票
        JobContent,	            //招聘
        Custom                  //自定义
    }

	public class EAuxiliaryTableTypeUtils
	{
		public static string GetValue(EAuxiliaryTableType type)
		{
		    if (type == EAuxiliaryTableType.BackgroundContent)
			{
				return "BackgroundContent";
            }
		    if (type == EAuxiliaryTableType.GovPublicContent)
		    {
		        return "GovPublicContent";
		    }
		    if (type == EAuxiliaryTableType.GovInteractContent)
		    {
		        return "GovInteractContent";
		    }
		    if (type == EAuxiliaryTableType.VoteContent)
		    {
		        return "VoteContent";
		    }
		    if (type == EAuxiliaryTableType.JobContent)
		    {
		        return "JobContent";
		    }
		    if (type == EAuxiliaryTableType.Custom)
		    {
		        return "Custom";
		    }
		    throw new Exception();
		}

		public static string GetText(EAuxiliaryTableType type)
		{
		    if (type == EAuxiliaryTableType.BackgroundContent)
			{
                return "内容";
            }
		    if (type == EAuxiliaryTableType.GovPublicContent)
		    {
		        return "信息公开";
		    }
		    if (type == EAuxiliaryTableType.GovInteractContent)
		    {
		        return "互动交流";
		    }
		    if (type == EAuxiliaryTableType.VoteContent)
		    {
		        return "投票";
		    }
		    if (type == EAuxiliaryTableType.JobContent)
		    {
		        return "招聘";
		    }
		    if (type == EAuxiliaryTableType.Custom)
		    {
		        return "自定义";
		    }
		    throw new Exception();
		}

		public static EAuxiliaryTableType GetEnumType(string typeStr)
		{
            var retval = EAuxiliaryTableType.BackgroundContent;

			if (Equals(EAuxiliaryTableType.BackgroundContent, typeStr))
			{
				retval = EAuxiliaryTableType.BackgroundContent;
            }
            else if (Equals(EAuxiliaryTableType.GovPublicContent, typeStr))
            {
                retval = EAuxiliaryTableType.GovPublicContent;
            }
            else if (Equals(EAuxiliaryTableType.GovInteractContent, typeStr))
            {
                retval = EAuxiliaryTableType.GovInteractContent;
            }
            else if (Equals(EAuxiliaryTableType.VoteContent, typeStr))
            {
                retval = EAuxiliaryTableType.VoteContent;
            }
            else if (Equals(EAuxiliaryTableType.JobContent, typeStr))
            {
                retval = EAuxiliaryTableType.JobContent;
            }
            else if (Equals(EAuxiliaryTableType.Custom, typeStr))
            {
                retval = EAuxiliaryTableType.Custom;
            }

			return retval;
		}

		public static bool Equals(EAuxiliaryTableType type, string typeStr)
		{
			if (string.IsNullOrEmpty(typeStr)) return false;
			if (string.Equals(GetValue(type).ToLower(), typeStr.ToLower()))
			{
				return true;
			}
			return false;
		}

        public static bool Equals(string typeStr, EAuxiliaryTableType type)
        {
            return Equals(type, typeStr);
        }

		public static ListItem GetListItem(EAuxiliaryTableType type, bool selected)
		{
			var item = new ListItem(GetText(type), GetValue(type));
			if (selected)
			{
				item.Selected = true;
			}
			return item;
		}

        public static ETableStyle GetTableStyle(EAuxiliaryTableType tableType)
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

        public static string GetDefaultTableName(EAuxiliaryTableType tableType)
        {
            if (tableType == EAuxiliaryTableType.GovPublicContent)
            {
                return "model_WCM_GovPublic";
            }
            if (tableType == EAuxiliaryTableType.GovInteractContent)
            {
                return "model_WCM_GovInteract";
            }
            if (tableType == EAuxiliaryTableType.VoteContent)
            {
                return "model_Vote";
            }
            if (tableType == EAuxiliaryTableType.JobContent)
            {
                return "model_Job";
            }
            if (tableType == EAuxiliaryTableType.Custom)
            {
                return "model_Custom";
            }
            return "model_Content";
        }
	}
}
