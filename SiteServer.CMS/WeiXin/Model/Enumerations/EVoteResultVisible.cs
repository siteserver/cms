using System;
using System.Web.UI.WebControls;

namespace SiteServer.CMS.WeiXin.Model.Enumerations
{
	public enum EVoteResultVisible
	{
        Before,
        After,
        End
	}

    public class EVoteResultVisibleUtils
	{
        public static string GetValue(EVoteResultVisible type)
		{
            if (type == EVoteResultVisible.Before)
            {
                return "Before";
            }
            else if (type == EVoteResultVisible.After)
            {
                return "After";
            }
            else if (type == EVoteResultVisible.End)
            {
                return "End";
            }
			else
			{
				throw new Exception();
			}
		}

		public static string GetText(EVoteResultVisible type)
		{
            if (type == EVoteResultVisible.Before)
            {
                return "ͶƱǰ�ɼ�";
            }
            else if (type == EVoteResultVisible.After)
            {
                return "ͶƱ��ɼ�";
            }
            else if (type == EVoteResultVisible.End)
            {
                return "ͶƱ�����ɼ�";
            }
			else
			{
				throw new Exception();
			}
		}

		public static EVoteResultVisible GetEnumType(string typeStr)
		{
            var retval = EVoteResultVisible.Before;

            if (Equals(EVoteResultVisible.Before, typeStr))
            {
                retval = EVoteResultVisible.Before;
            }
            else if (Equals(EVoteResultVisible.After, typeStr))
            {
                retval = EVoteResultVisible.After;
            }
            else if (Equals(EVoteResultVisible.End, typeStr))
            {
                retval = EVoteResultVisible.End;
            }

			return retval;
		}

		public static bool Equals(EVoteResultVisible type, string typeStr)
		{
			if (string.IsNullOrEmpty(typeStr)) return false;
			if (string.Equals(GetValue(type).ToLower(), typeStr.ToLower()))
			{
				return true;
			}
			return false;
		}

        public static bool Equals(string typeStr, EVoteResultVisible type)
        {
            return Equals(type, typeStr);
        }

        public static ListItem GetListItem(EVoteResultVisible type, bool selected)
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
                listControl.Items.Add(GetListItem(EVoteResultVisible.Before, false));
                listControl.Items.Add(GetListItem(EVoteResultVisible.After, false));
                listControl.Items.Add(GetListItem(EVoteResultVisible.End, false));
            }
        }
	}
}
