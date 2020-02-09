using System;
using System.Web.UI.WebControls;
using Datory;
using SiteServer.Abstractions;

namespace SiteServer.CMS.Context.Enumerations
{
	public static class TranslateTypeUtils
	{
		public static string GetValue(TranslateType type)
		{
		    if (type == TranslateType.Content)
			{
				return "Body";
			}
		    if (type == TranslateType.Channel)
		    {
		        return "Channel";
		    }
		    if (type == TranslateType.All)
		    {
		        return "All";
		    }
		    throw new Exception();
		}

		public static string GetText(TranslateType type)
		{
		    if (type == TranslateType.Content)
			{
				return "仅转移内容";
			}
		    if (type == TranslateType.Channel)
		    {
		        return "仅转移栏目";
		    }
		    if (type == TranslateType.All)
		    {
		        return "转移栏目及内容";
		    }
		    throw new Exception();
		}

		public static TranslateType GetEnumType(string typeStr)
		{
			var retVal = TranslateType.Content;

			if (Equals(TranslateType.Content, typeStr))
			{
				retVal = TranslateType.Content;
			}
			else if (Equals(TranslateType.Channel, typeStr))
			{
				retVal = TranslateType.Channel;
			}
			else if (Equals(TranslateType.All, typeStr))
			{
				retVal = TranslateType.All;
			}

			return retVal;
		}

		public static bool Equals(TranslateType type, string typeStr)
		{
			if (string.IsNullOrEmpty(typeStr)) return false;
			if (string.Equals(GetValue(type).ToLower(), typeStr.ToLower()))
			{
				return true;
			}
			return false;
		}

        public static bool Equals(string typeStr, TranslateType type)
        {
            return Equals(type, typeStr);
        }

        public static ListItem GetListItem(TranslateType type, bool selected)
        {
            var item = new ListItem(type.GetDisplayName(), type.GetValue());
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
				listControl.Items.Add(GetListItem(TranslateType.Content, false));
				listControl.Items.Add(GetListItem(TranslateType.Channel, false));
                listControl.Items.Add(GetListItem(TranslateType.All, false));
            }
        }
	}
}
