using System.Web.UI.WebControls;
using Datory;
using SiteServer.Abstractions;

namespace SiteServer.CMS.Core
{
    public static class TemplateTypeUtils
	{


		public static ListItem GetListItem(TemplateType type, bool selected)
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
				listControl.Items.Add(GetListItem(TemplateType.IndexPageTemplate, false));
				listControl.Items.Add(GetListItem(TemplateType.ChannelTemplate, false));
				listControl.Items.Add(GetListItem(TemplateType.ContentTemplate, false));
				listControl.Items.Add(GetListItem(TemplateType.FileTemplate, false));
			}
		}
    }
}
