using System;
using System.Web.UI.WebControls;

namespace SiteServer.CMS.Model.Enumerations
{
    public enum EGovInteractState
	{
        New,                //新办件
        Denied,             //拒绝受理
        Accepted,           //已受理
        Redo,               //要求返工
        Replied,            //已办理
        Checked,            //已审核
	}

    public class EGovInteractStateUtils
	{
		public static string GetValue(EGovInteractState type)
		{
            if (type == EGovInteractState.New)
			{
                return "New";
			}
            else if (type == EGovInteractState.Denied)
			{
                return "Denied";
            }
            else if (type == EGovInteractState.Accepted)
            {
                return "Accepted";
            }
            else if (type == EGovInteractState.Redo)
            {
                return "Redo";
            }
            else if (type == EGovInteractState.Replied)
            {
                return "Replied";
            }
            else if (type == EGovInteractState.Checked)
            {
                return "Checked";
            }
			else
			{
				throw new Exception();
			}
		}

		public static string GetText(EGovInteractState type)
		{
            if (type == EGovInteractState.New)
			{
                return "新办件";
			}
            else if (type == EGovInteractState.Denied)
			{
                return "拒绝受理";
            }
            else if (type == EGovInteractState.Accepted)
            {
                return "已受理";
            }
            else if (type == EGovInteractState.Redo)
            {
                return "要求返工";
            }
            else if (type == EGovInteractState.Replied)
            {
                return "已办理";
            }
            else if (type == EGovInteractState.Checked)
            {
                return "处理完毕";
            }
			else
			{
				throw new Exception();
			}
		}

        public static string GetFrontText(EGovInteractState type)
        {
            if (type == EGovInteractState.Denied)
            {
                return "拒绝受理";
            }
            else if (type == EGovInteractState.Checked)
            {
                return "办理完毕";
            }
            else
            {
                return "办理中";
            }
        }

		public static EGovInteractState GetEnumType(string typeStr)
		{
            var retval = EGovInteractState.New;

            if (Equals(EGovInteractState.New, typeStr))
			{
                retval = EGovInteractState.New;
			}
            else if (Equals(EGovInteractState.Denied, typeStr))
			{
                retval = EGovInteractState.Denied;
            }
            else if (Equals(EGovInteractState.Accepted, typeStr))
            {
                retval = EGovInteractState.Accepted;
            }
            else if (Equals(EGovInteractState.Redo, typeStr))
            {
                retval = EGovInteractState.Redo;
            }
            else if (Equals(EGovInteractState.Replied, typeStr))
            {
                retval = EGovInteractState.Replied;
            }
            else if (Equals(EGovInteractState.Checked, typeStr))
            {
                retval = EGovInteractState.Checked;
            }
			return retval;
		}

		public static bool Equals(EGovInteractState type, string typeStr)
		{
			if (string.IsNullOrEmpty(typeStr)) return false;
			if (string.Equals(GetValue(type).ToLower(), typeStr.ToLower()))
			{
				return true;
			}
			return false;
		}

        public static bool Equals(string typeStr, EGovInteractState type)
        {
            return Equals(type, typeStr);
        }

        public static ListItem GetListItem(EGovInteractState type, bool selected)
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
                listControl.Items.Add(GetListItem(EGovInteractState.New, false));
                listControl.Items.Add(GetListItem(EGovInteractState.Denied, false));
                listControl.Items.Add(GetListItem(EGovInteractState.Accepted, false));
                listControl.Items.Add(GetListItem(EGovInteractState.Redo, false));
                listControl.Items.Add(GetListItem(EGovInteractState.Replied, false));
                listControl.Items.Add(GetListItem(EGovInteractState.Checked, false));
            }
        }
	}
}
