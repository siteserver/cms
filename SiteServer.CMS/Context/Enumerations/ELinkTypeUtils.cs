using System.Web.UI.WebControls;
using SiteServer.Abstractions;

namespace SiteServer.CMS.Context.Enumerations
{

    public static class ELinkTypeUtilsExtensions
	{
		
		public static ListItem GetListItem(ELinkType type, bool selected)
		{
			var item = new ListItem(ELinkTypeUtils.GetText(type), ELinkTypeUtils.GetValue(type));
			if (selected)
			{
				item.Selected = true;
			}
			return item;
		}

		public static void AddListItems(ListControl listControl)
		{
		    if (listControl == null) return;

		    listControl.Items.Add(GetListItem(ELinkType.None, false));
		    listControl.Items.Add(GetListItem(ELinkType.NoLinkIfContentNotExists, false));
		    listControl.Items.Add(GetListItem(ELinkType.LinkToOnlyOneContent, false));
		    listControl.Items.Add(GetListItem(ELinkType.NoLinkIfContentNotExistsAndLinkToOnlyOneContent, false));
		    listControl.Items.Add(GetListItem(ELinkType.LinkToFirstContent, false));
		    listControl.Items.Add(GetListItem(ELinkType.NoLinkIfContentNotExistsAndLinkToFirstContent, false));
		    listControl.Items.Add(GetListItem(ELinkType.NoLinkIfChannelNotExists, false));
		    listControl.Items.Add(GetListItem(ELinkType.LinkToLastAddChannel, false));
		    listControl.Items.Add(GetListItem(ELinkType.LinkToFirstChannel, false));
		    listControl.Items.Add(GetListItem(ELinkType.NoLinkIfChannelNotExistsAndLinkToLastAddChannel, false));
		    listControl.Items.Add(GetListItem(ELinkType.NoLinkIfChannelNotExistsAndLinkToFirstChannel, false));
		    listControl.Items.Add(GetListItem(ELinkType.NoLink, false));
		}
	}
}
