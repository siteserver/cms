using System.Web.UI.WebControls;
using Datory;
using SiteServer.Abstractions;

namespace SiteServer.CMS.Context.Enumerations
{
	public class ECrossSiteTransTypeUtilsExtensions
	{
		public static ListItem GetListItem(TransType type, bool selected)
        {
            var item = new ListItem(type.GetDisplayName(), type.GetValue());
            if (selected)
            {
                item.Selected = true;
            }
            return item;
        }

        public static void AddAllListItems(ListControl listControl, bool isParentSite)
        {
            if (listControl == null) return;

            listControl.Items.Add(GetListItem(TransType.None, false));
            listControl.Items.Add(GetListItem(TransType.SelfSite, false));
            listControl.Items.Add(GetListItem(TransType.SpecifiedSite, false));
            if (isParentSite)
            {
                listControl.Items.Add(GetListItem(TransType.ParentSite, false));
                listControl.Items.Add(GetListItem(TransType.AllParentSite, false));
            }
            listControl.Items.Add(GetListItem(TransType.AllSite, false));
        }
	}
}
