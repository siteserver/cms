using System;
using System.Web.UI.WebControls;

namespace SiteServer.CMS.Model.Enumerations
{

    public enum ERelatedFieldStyle
	{
		Horizontal,                 //水平显示
        Virtical,                   //垂直显示
    }

    public class ERelatedFieldStyleUtils
	{
		public static string GetValue(ERelatedFieldStyle type)
		{
            if (type == ERelatedFieldStyle.Horizontal)
			{
                return "Horizontal";
			}
            else if (type == ERelatedFieldStyle.Virtical)
			{
                return "Virtical";
			}
			else
			{
				throw new Exception();
			}
		}

		public static string GetText(ERelatedFieldStyle type)
		{
            if (type == ERelatedFieldStyle.Horizontal)
			{
                return "水平显示";
			}
            else if (type == ERelatedFieldStyle.Virtical)
			{
                return "垂直显示";
			}
			else
			{
				throw new Exception();
			}
		}

		public static ERelatedFieldStyle GetEnumType(string typeStr)
		{
            var retval = ERelatedFieldStyle.Horizontal;

            if (Equals(ERelatedFieldStyle.Horizontal, typeStr))
			{
                retval = ERelatedFieldStyle.Horizontal;
			}
			else if (Equals(ERelatedFieldStyle.Virtical, typeStr))
			{
                retval = ERelatedFieldStyle.Virtical;
			}
			
			return retval;
		}

		public static bool Equals(ERelatedFieldStyle type, string typeStr)
		{
			if (string.IsNullOrEmpty(typeStr)) return false;
			if (string.Equals(GetValue(type).ToLower(), typeStr.ToLower()))
			{
				return true;
			}
			return false;
		}

        public static bool Equals(string typeStr, ERelatedFieldStyle type)
        {
            return Equals(type, typeStr);
        }

		public static ListItem GetListItem(ERelatedFieldStyle type, bool selected)
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
                listControl.Items.Add(GetListItem(ERelatedFieldStyle.Horizontal, false));
				listControl.Items.Add(GetListItem(ERelatedFieldStyle.Virtical, false));
			}
		}

	}
}
