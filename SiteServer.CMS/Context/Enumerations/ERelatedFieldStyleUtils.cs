using System.Web.UI.WebControls;
using SiteServer.Abstractions;

namespace SiteServer.CMS.Context.Enumerations
{

    public class ERelatedFieldStyleUtilsExtensions
	{
		public static ListItem GetListItem(ERelatedFieldStyle type, bool selected)
		{
			var item = new ListItem(ERelatedFieldStyleUtils.GetText(type), ERelatedFieldStyleUtils.GetValue(type));
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
