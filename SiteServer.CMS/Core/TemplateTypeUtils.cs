using System.Web.UI.WebControls;
using SiteServer.Plugin;

namespace SiteServer.CMS.Core
{
	public class TemplateTypeUtils
	{
		public static string GetValue(TemplateType type)
		{
		    return type.Value;
		}

		public static TemplateType GetEnumType(string typeStr)
		{
			var retval = TemplateType.IndexPageTemplate;

			if (Equals(TemplateType.ChannelTemplate, typeStr))
			{
				retval = TemplateType.ChannelTemplate;
			}
			else if (Equals(TemplateType.IndexPageTemplate, typeStr))
			{
				retval = TemplateType.IndexPageTemplate;
			}
			else if (Equals(TemplateType.ContentTemplate, typeStr))
			{
				retval = TemplateType.ContentTemplate;
			}
			else if (Equals(TemplateType.FileTemplate, typeStr))
			{
				retval = TemplateType.FileTemplate;
			}
			return retval;
		}

		public static bool Equals(TemplateType type, string typeStr)
		{
			if (string.IsNullOrEmpty(typeStr)) return false;
			if (string.Equals(GetValue(type).ToLower(), typeStr.ToLower()))
			{
				return true;
			}
			return false;
		}

        public static bool Equals(string typeStr, TemplateType type)
        {
            return Equals(type, typeStr);
        }

		public static ListItem GetListItem(TemplateType type, bool selected)
		{
			var item = new ListItem(TemplateType.GetText(type), GetValue(type));
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
				listControl.Items.Add(GetListItem(TemplateType.IndexPageTemplate, false));
				listControl.Items.Add(GetListItem(TemplateType.ChannelTemplate, false));
				listControl.Items.Add(GetListItem(TemplateType.ContentTemplate, false));
				listControl.Items.Add(GetListItem(TemplateType.FileTemplate, false));
			}
		}

	}
}
