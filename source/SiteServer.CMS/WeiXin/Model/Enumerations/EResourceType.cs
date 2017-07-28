using System;
using System.Web.UI.WebControls;

namespace SiteServer.CMS.WeiXin.Model.Enumerations
{
	public enum EResourceType
	{
        Site,
        Content,
        Url
	}

    public class EResourceTypeUtils
	{
        public static string GetValue(EResourceType type)
		{
            if (type == EResourceType.Site)
            {
                return "Site";
            }
            else if (type == EResourceType.Url)
            {
                return "Url";
            }
            else if (type == EResourceType.Content)
            {
                return "Content";
            }
			else
			{
				throw new Exception();
			}
		}

		public static string GetText(EResourceType type)
		{
            if (type == EResourceType.Site)
            {
                return "微网站页面";
            }
            else if (type == EResourceType.Content)
            {
                return "正文";
            }
            else if (type == EResourceType.Url)
            {
                return "指定网址";
            }
			else
			{
				throw new Exception();
			}
		}

		public static EResourceType GetEnumType(string typeStr)
		{
            var retval = EResourceType.Site;

            if (Equals(EResourceType.Site, typeStr))
            {
                retval = EResourceType.Site;
            }
            else if (Equals(EResourceType.Url, typeStr))
            {
                retval = EResourceType.Url;
            }
            else if (Equals(EResourceType.Content, typeStr))
            {
                retval = EResourceType.Content;
            }

			return retval;
		}

		public static bool Equals(EResourceType type, string typeStr)
		{
			if (string.IsNullOrEmpty(typeStr)) return false;
			if (string.Equals(GetValue(type).ToLower(), typeStr.ToLower()))
			{
				return true;
			}
			return false;
		}

        public static bool Equals(string typeStr, EResourceType type)
        {
            return Equals(type, typeStr);
        }

        public static ListItem GetListItem(EResourceType type, bool selected)
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
                listControl.Items.Add(GetListItem(EResourceType.Site, false));
                listControl.Items.Add(GetListItem(EResourceType.Content, false));
                listControl.Items.Add(GetListItem(EResourceType.Url, false));
            }
        }
	}
}
