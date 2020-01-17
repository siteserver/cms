using System.Collections.Generic;
using System.Web.UI.WebControls;
using Datory;
using SiteServer.Abstractions;
using SiteServer.CMS.Dto;

namespace SiteServer.CMS.Context.Enumerations
{

    public static class ELinkTypeUtilsExtensions
	{
		
		public static ListItem GetListItem(LinkType type, bool selected)
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
		    if (listControl == null) return;

		    listControl.Items.Add(GetListItem(LinkType.None, false));
		    listControl.Items.Add(GetListItem(LinkType.NoLinkIfContentNotExists, false));
		    listControl.Items.Add(GetListItem(LinkType.LinkToOnlyOneContent, false));
		    listControl.Items.Add(GetListItem(LinkType.NoLinkIfContentNotExistsAndLinkToOnlyOneContent, false));
		    listControl.Items.Add(GetListItem(LinkType.LinkToFirstContent, false));
		    listControl.Items.Add(GetListItem(LinkType.NoLinkIfContentNotExistsAndLinkToFirstContent, false));
		    listControl.Items.Add(GetListItem(LinkType.NoLinkIfChannelNotExists, false));
		    listControl.Items.Add(GetListItem(LinkType.LinkToLastAddChannel, false));
		    listControl.Items.Add(GetListItem(LinkType.LinkToFirstChannel, false));
		    listControl.Items.Add(GetListItem(LinkType.NoLinkIfChannelNotExistsAndLinkToLastAddChannel, false));
		    listControl.Items.Add(GetListItem(LinkType.NoLinkIfChannelNotExistsAndLinkToFirstChannel, false));
		    listControl.Items.Add(GetListItem(LinkType.NoLink, false));
		}

        public static List<Select<string>> GetAll()
        {
            return new List<Select<string>>
            {
				new Select<string>(LinkType.None),
                new Select<string>(LinkType.NoLinkIfContentNotExists),
                new Select<string>(LinkType.LinkToOnlyOneContent),
                new Select<string>(LinkType.NoLinkIfContentNotExistsAndLinkToOnlyOneContent),
                new Select<string>(LinkType.LinkToFirstContent),
                new Select<string>(LinkType.NoLinkIfContentNotExistsAndLinkToFirstContent),
                new Select<string>(LinkType.NoLinkIfChannelNotExists),
                new Select<string>(LinkType.LinkToLastAddChannel),
                new Select<string>(LinkType.LinkToFirstChannel),
                new Select<string>(LinkType.NoLinkIfChannelNotExistsAndLinkToLastAddChannel),
                new Select<string>(LinkType.NoLinkIfChannelNotExistsAndLinkToFirstChannel),
                new Select<string>(LinkType.NoLink)
			};
        }
	}
}
