using System.Web.UI.WebControls;
using SiteServer.Abstractions;

namespace SiteServer.CMS.Context.Enumerations
{
	public class ECrossSiteTransTypeUtilsExtensions
	{
		public static ListItem GetListItem(ECrossSiteTransType type, bool selected)
        {
            var item = new ListItem(ECrossSiteTransTypeUtils.GetText(type), ECrossSiteTransTypeUtils.GetValue(type));
            if (selected)
            {
                item.Selected = true;
            }
            return item;
        }

        public static void AddListItems(ListControl listControl)
        {
            if (listControl == null) return;

            listControl.Items.Add(GetListItem(ECrossSiteTransType.None, false));
            listControl.Items.Add(GetListItem(ECrossSiteTransType.SelfSite, false));
            listControl.Items.Add(GetListItem(ECrossSiteTransType.ParentSite, false));
            listControl.Items.Add(GetListItem(ECrossSiteTransType.AllParentSite, false));
            listControl.Items.Add(GetListItem(ECrossSiteTransType.AllSite, false));
        }

        public static void AddAllListItems(ListControl listControl, bool isParentSite)
        {
            if (listControl == null) return;

            listControl.Items.Add(GetListItem(ECrossSiteTransType.None, false));
            listControl.Items.Add(GetListItem(ECrossSiteTransType.SelfSite, false));
            listControl.Items.Add(GetListItem(ECrossSiteTransType.SpecifiedSite, false));
            if (isParentSite)
            {
                listControl.Items.Add(GetListItem(ECrossSiteTransType.ParentSite, false));
                listControl.Items.Add(GetListItem(ECrossSiteTransType.AllParentSite, false));
            }
            listControl.Items.Add(GetListItem(ECrossSiteTransType.AllSite, false));
        }
	}
}
