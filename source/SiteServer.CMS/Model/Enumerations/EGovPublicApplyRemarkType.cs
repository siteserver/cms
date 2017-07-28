using System;
using System.Web.UI.WebControls;

namespace SiteServer.CMS.Model.Enumerations
{
    public enum EGovPublicApplyRemarkType
	{
        Accept,             //受理
        SwitchTo,           //转办
        Comment,            //批示
        Redo                //要求返工
	}

    public class EGovPublicApplyRemarkTypeUtils
	{
		public static string GetValue(EGovPublicApplyRemarkType type)
		{
            if (type == EGovPublicApplyRemarkType.Accept)
			{
                return "Accept";
			}
            else if (type == EGovPublicApplyRemarkType.SwitchTo)
            {
                return "SwitchTo";
            }
            else if (type == EGovPublicApplyRemarkType.Comment)
            {
                return "Comment";
            }
            else if (type == EGovPublicApplyRemarkType.Redo)
            {
                return "Redo";
            }
			else
			{
				throw new Exception();
			}
		}

		public static string GetText(EGovPublicApplyRemarkType type)
		{
            if (type == EGovPublicApplyRemarkType.Accept)
			{
                return "受理";
			}
            else if (type == EGovPublicApplyRemarkType.SwitchTo)
            {
                return "转办";
            }
            else if (type == EGovPublicApplyRemarkType.Comment)
            {
                return "批示";
            }
            else if (type == EGovPublicApplyRemarkType.Redo)
            {
                return "要求返工";
            }
			else
			{
				throw new Exception();
			}
		}

		public static EGovPublicApplyRemarkType GetEnumType(string typeStr)
		{
            var retval = EGovPublicApplyRemarkType.Accept;

            if (Equals(EGovPublicApplyRemarkType.Accept, typeStr))
			{
                retval = EGovPublicApplyRemarkType.Accept;
			}
            else if (Equals(EGovPublicApplyRemarkType.SwitchTo, typeStr))
            {
                retval = EGovPublicApplyRemarkType.SwitchTo;
            }
            else if (Equals(EGovPublicApplyRemarkType.Comment, typeStr))
            {
                retval = EGovPublicApplyRemarkType.Comment;
            }
            else if (Equals(EGovPublicApplyRemarkType.Redo, typeStr))
            {
                retval = EGovPublicApplyRemarkType.Redo;
            }
			return retval;
		}

		public static bool Equals(EGovPublicApplyRemarkType type, string typeStr)
		{
			if (string.IsNullOrEmpty(typeStr)) return false;
			if (string.Equals(GetValue(type).ToLower(), typeStr.ToLower()))
			{
				return true;
			}
			return false;
		}

        public static bool Equals(string typeStr, EGovPublicApplyRemarkType type)
        {
            return Equals(type, typeStr);
        }

        public static ListItem GetListItem(EGovPublicApplyRemarkType type, bool selected)
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
                listControl.Items.Add(GetListItem(EGovPublicApplyRemarkType.Accept, false));
                listControl.Items.Add(GetListItem(EGovPublicApplyRemarkType.SwitchTo, false));
                listControl.Items.Add(GetListItem(EGovPublicApplyRemarkType.Comment, false));
                listControl.Items.Add(GetListItem(EGovPublicApplyRemarkType.Redo, false));
            }
        }
	}
}
