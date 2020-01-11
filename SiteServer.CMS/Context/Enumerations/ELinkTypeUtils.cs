using System.Collections.Generic;
using System.Web.UI.WebControls;
using SiteServer.Abstractions;
using SiteServer.CMS.Dto;

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

        public static List<Select<string>> GetAll()
        {
            return new List<Select<string>>
            {
				new Select<string>(ELinkType.None),
                new Select<string>(ELinkType.NoLinkIfContentNotExists),
                new Select<string>(ELinkType.LinkToOnlyOneContent),
                new Select<string>(ELinkType.NoLinkIfContentNotExistsAndLinkToOnlyOneContent),
                new Select<string>(ELinkType.LinkToFirstContent),
                new Select<string>(ELinkType.NoLinkIfContentNotExistsAndLinkToFirstContent),
                new Select<string>(ELinkType.NoLinkIfChannelNotExists),
                new Select<string>(ELinkType.LinkToLastAddChannel),
                new Select<string>(ELinkType.LinkToFirstChannel),
                new Select<string>(ELinkType.NoLinkIfChannelNotExistsAndLinkToLastAddChannel),
                new Select<string>(ELinkType.NoLinkIfChannelNotExistsAndLinkToFirstChannel),
                new Select<string>(ELinkType.NoLink)
			};
        }
	}
}
