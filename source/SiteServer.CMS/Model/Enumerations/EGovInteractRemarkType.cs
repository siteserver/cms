using System;
using System.Web.UI.WebControls;

namespace SiteServer.CMS.Model.Enumerations
{
    public enum EGovInteractRemarkType
	{
        Accept,             //受理
        SwitchTo,           //转办
        Translate,          //转移
        Comment,            //批示
        Redo                //要求返工
	}

    public class EGovInteractRemarkTypeUtils
	{
		public static string GetValue(EGovInteractRemarkType type)
		{
            if (type == EGovInteractRemarkType.Accept)
			{
                return "Accept";
			}
            else if (type == EGovInteractRemarkType.SwitchTo)
            {
                return "SwitchTo";
            }
            else if (type == EGovInteractRemarkType.Translate)
            {
                return "Translate";
            }
            else if (type == EGovInteractRemarkType.Comment)
            {
                return "Comment";
            }
            else if (type == EGovInteractRemarkType.Redo)
            {
                return "Redo";
            }
			else
			{
				throw new Exception();
			}
		}

		public static string GetText(EGovInteractRemarkType type)
		{
            if (type == EGovInteractRemarkType.Accept)
			{
                return "受理";
			}
            else if (type == EGovInteractRemarkType.SwitchTo)
            {
                return "转办";
            }
            else if (type == EGovInteractRemarkType.Translate)
            {
                return "转移";
            }
            else if (type == EGovInteractRemarkType.Comment)
            {
                return "批示";
            }
            else if (type == EGovInteractRemarkType.Redo)
            {
                return "要求返工";
            }
			else
			{
				throw new Exception();
			}
		}

		public static EGovInteractRemarkType GetEnumType(string typeStr)
		{
            var retval = EGovInteractRemarkType.Accept;

            if (Equals(EGovInteractRemarkType.Accept, typeStr))
			{
                retval = EGovInteractRemarkType.Accept;
			}
            else if (Equals(EGovInteractRemarkType.SwitchTo, typeStr))
            {
                retval = EGovInteractRemarkType.SwitchTo;
            }
            else if (Equals(EGovInteractRemarkType.Translate, typeStr))
            {
                retval = EGovInteractRemarkType.Translate;
            }
            else if (Equals(EGovInteractRemarkType.Comment, typeStr))
            {
                retval = EGovInteractRemarkType.Comment;
            }
            else if (Equals(EGovInteractRemarkType.Redo, typeStr))
            {
                retval = EGovInteractRemarkType.Redo;
            }
			return retval;
		}

		public static bool Equals(EGovInteractRemarkType type, string typeStr)
		{
			if (string.IsNullOrEmpty(typeStr)) return false;
			if (string.Equals(GetValue(type).ToLower(), typeStr.ToLower()))
			{
				return true;
			}
			return false;
		}

        public static bool Equals(string typeStr, EGovInteractRemarkType type)
        {
            return Equals(type, typeStr);
        }

        public static ListItem GetListItem(EGovInteractRemarkType type, bool selected)
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
                listControl.Items.Add(GetListItem(EGovInteractRemarkType.Accept, false));
                listControl.Items.Add(GetListItem(EGovInteractRemarkType.SwitchTo, false));
                listControl.Items.Add(GetListItem(EGovInteractRemarkType.Translate, false));
                listControl.Items.Add(GetListItem(EGovInteractRemarkType.Comment, false));
                listControl.Items.Add(GetListItem(EGovInteractRemarkType.Redo, false));
            }
        }
	}
}
