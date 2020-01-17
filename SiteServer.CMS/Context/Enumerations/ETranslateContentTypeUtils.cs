using System.Web.UI.WebControls;
using Datory;
using SiteServer.Abstractions;

namespace SiteServer.CMS.Context.Enumerations
{
	public class ETranslateContentTypeUtilsExtensions
	{
		public static ListItem GetListItem(Abstractions.TranslateContentType type, bool selected)
		{
			var item = new ListItem(type.GetDisplayName(), type.GetValue());
			if (selected)
			{
				item.Selected = true;
			}
			return item;
		}

		public static void AddListItems(ListControl listControl, bool isCut)
		{
			if (listControl != null)
			{
				listControl.Items.Add(GetListItem(TranslateContentType.Copy, false));
                if (isCut)
                {
                    listControl.Items.Add(GetListItem(TranslateContentType.Cut, false));
                }
                listControl.Items.Add(GetListItem(TranslateContentType.Reference, false));
                listControl.Items.Add(GetListItem(TranslateContentType.ReferenceContent, false));
			}
		}
	}
}
