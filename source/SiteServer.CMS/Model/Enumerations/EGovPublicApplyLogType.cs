using System;
using System.Web.UI.WebControls;

namespace SiteServer.CMS.Model.Enumerations
{
    public enum EGovPublicApplyLogType
	{
        New,                //新申请
        Accept,             //受理
        Deny,               //拒绝
        SwitchTo,           //转办
        Comment,            //批示
        Redo,               //要求返工
        Reply,              //办理
        Check,              //审核
	}

    public class EGovPublicApplyLogTypeUtils
	{
		public static string GetValue(EGovPublicApplyLogType type)
		{
            if (type == EGovPublicApplyLogType.New)
			{
                return "New";
            }
            else if (type == EGovPublicApplyLogType.Accept)
            {
                return "Accept";
            }
            else if (type == EGovPublicApplyLogType.Deny)
			{
                return "Deny";
            }
            else if (type == EGovPublicApplyLogType.SwitchTo)
            {
                return "SwitchTo";
            }
            else if (type == EGovPublicApplyLogType.Comment)
            {
                return "Comment";
            }
            else if (type == EGovPublicApplyLogType.Redo)
            {
                return "Redo";
            }
            else if (type == EGovPublicApplyLogType.Reply)
            {
                return "Reply";
            }
            else if (type == EGovPublicApplyLogType.Check)
            {
                return "Check";
            }
			else
			{
				throw new Exception();
			}
		}

		public static string GetText(EGovPublicApplyLogType type)
		{
            if (type == EGovPublicApplyLogType.New)
            {
                return "前台 网友 提交申请";
            }
            else if (type == EGovPublicApplyLogType.Accept)
            {
                return "受理申请";
            }
            else if (type == EGovPublicApplyLogType.Deny)
            {
                return "拒绝申请";
            }
            else if (type == EGovPublicApplyLogType.SwitchTo)
            {
                return "转办申请";
            }
            else if (type == EGovPublicApplyLogType.Comment)
            {
                return "批示申请";
            }
            else if (type == EGovPublicApplyLogType.Redo)
            {
                return "要求返工";
            }
            else if (type == EGovPublicApplyLogType.Reply)
            {
                return "回复申请";
            }
            else if (type == EGovPublicApplyLogType.Check)
            {
                return "审核申请 通过";
            }
            else
            {
                throw new Exception();
            }
		}

		public static EGovPublicApplyLogType GetEnumType(string typeStr)
		{
            var retval = EGovPublicApplyLogType.New;

            if (Equals(EGovPublicApplyLogType.New, typeStr))
			{
                retval = EGovPublicApplyLogType.New;
            }
            else if (Equals(EGovPublicApplyLogType.Accept, typeStr))
            {
                retval = EGovPublicApplyLogType.Accept;
            }
            else if (Equals(EGovPublicApplyLogType.Deny, typeStr))
			{
                retval = EGovPublicApplyLogType.Deny;
            }
            else if (Equals(EGovPublicApplyLogType.SwitchTo, typeStr))
            {
                retval = EGovPublicApplyLogType.SwitchTo;
            }
            else if (Equals(EGovPublicApplyLogType.Comment, typeStr))
            {
                retval = EGovPublicApplyLogType.Comment;
            }
            else if (Equals(EGovPublicApplyLogType.Redo, typeStr))
            {
                retval = EGovPublicApplyLogType.Redo;
            }
            else if (Equals(EGovPublicApplyLogType.Reply, typeStr))
            {
                retval = EGovPublicApplyLogType.Reply;
            }
            else if (Equals(EGovPublicApplyLogType.Check, typeStr))
            {
                retval = EGovPublicApplyLogType.Check;
            }
			return retval;
		}

		public static bool Equals(EGovPublicApplyLogType type, string typeStr)
		{
			if (string.IsNullOrEmpty(typeStr)) return false;
			if (string.Equals(GetValue(type).ToLower(), typeStr.ToLower()))
			{
				return true;
			}
			return false;
		}

        public static bool Equals(string typeStr, EGovPublicApplyLogType type)
        {
            return Equals(type, typeStr);
        }

        public static ListItem GetListItem(EGovPublicApplyLogType type, bool selected)
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
                listControl.Items.Add(GetListItem(EGovPublicApplyLogType.New, false));
                listControl.Items.Add(GetListItem(EGovPublicApplyLogType.Accept, false));
                listControl.Items.Add(GetListItem(EGovPublicApplyLogType.Deny, false));
                listControl.Items.Add(GetListItem(EGovPublicApplyLogType.SwitchTo, false));
                listControl.Items.Add(GetListItem(EGovPublicApplyLogType.Comment, false));
                listControl.Items.Add(GetListItem(EGovPublicApplyLogType.Redo, false));
                listControl.Items.Add(GetListItem(EGovPublicApplyLogType.Reply, false));
                listControl.Items.Add(GetListItem(EGovPublicApplyLogType.Check, false));
            }
        }
	}
}
