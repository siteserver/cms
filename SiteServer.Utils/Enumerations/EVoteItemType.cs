using System;
using System.Web.UI.WebControls;

namespace SiteServer.Utils.Enumerations
{
	
	public enum EVoteItemType
	{
		Text,				//文字型投票
		Image,				//图片型投票
		TextAndImage		//图文混合型投票
	}

	public class EVoteItemTypeUtils
	{
		public static string GetValue(EVoteItemType type)
		{
		    if (type == EVoteItemType.Text)
			{
				return "Text";
			}
		    if (type == EVoteItemType.Image)
		    {
		        return "Image";
		    }
		    if (type == EVoteItemType.TextAndImage)
		    {
		        return "TextAndImage";
		    }
		    throw new Exception();
		}

		public static string GetText(EVoteItemType type)
		{
		    if (type == EVoteItemType.Text)
			{
				return "文字型投票";
			}
		    if (type == EVoteItemType.Image)
		    {
		        return "图片型投票";
		    }
		    if (type == EVoteItemType.TextAndImage)
		    {
		        return "图文混合型投票";
		    }
		    throw new Exception();
		}

		public static EVoteItemType GetEnumType(string typeStr)
		{
			var retval = EVoteItemType.Text;

			if (Equals(EVoteItemType.Text, typeStr))
			{
				retval = EVoteItemType.Text;
			}
			else if (Equals(EVoteItemType.Image, typeStr))
			{
				retval = EVoteItemType.Image;
			}
			else if (Equals(EVoteItemType.TextAndImage, typeStr))
			{
				retval = EVoteItemType.TextAndImage;
			}

			return retval;
		}

		public static bool Equals(EVoteItemType type, string typeStr)
		{
			if (string.IsNullOrEmpty(typeStr)) return false;
			if (string.Equals(GetValue(type).ToLower(), typeStr.ToLower()))
			{
				return true;
			}
			return false;
		}

        public static bool Equals(string typeStr, EVoteItemType type)
        {
            return Equals(type, typeStr);
        }

		public static ListItem GetListItem(EVoteItemType type, bool selected)
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
				listControl.Items.Add(GetListItem(EVoteItemType.Text, false));
				listControl.Items.Add(GetListItem(EVoteItemType.Image, false));
				listControl.Items.Add(GetListItem(EVoteItemType.TextAndImage, false));
			}
		}

	}
}
