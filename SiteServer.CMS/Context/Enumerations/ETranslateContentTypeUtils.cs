using System.Web.UI.WebControls;
using SiteServer.Abstractions;

namespace SiteServer.CMS.Context.Enumerations
{
	public class ETranslateContentTypeUtilsExtensions
	{
		public static ListItem GetListItem(Abstractions.ETranslateContentType type, bool selected)
		{
			var item = new ListItem(ETranslateContentTypeUtils.GetText(type), ETranslateContentTypeUtils.GetValue(type));
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
				listControl.Items.Add(GetListItem(ETranslateContentType.Copy, false));
                if (isCut)
                {
                    listControl.Items.Add(GetListItem(ETranslateContentType.Cut, false));
                }
                listControl.Items.Add(GetListItem(ETranslateContentType.Reference, false));
                listControl.Items.Add(GetListItem(ETranslateContentType.ReferenceContent, false));
			}
		}
	}
}
