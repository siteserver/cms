using System.Web.UI.WebControls;
using SiteServer.Abstractions;

namespace SiteServer.CMS.Context.Enumerations
{
    public class EDateFormatTypeUtilsExtensions
    {
        public static ListItem GetListItem(EDateFormatType type, bool selected)
        {
            var item = new ListItem(EDateFormatTypeUtils.GetText(type), EDateFormatTypeUtils.GetValue(type));
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
                listControl.Items.Add(GetListItem(EDateFormatType.Month, false));
                listControl.Items.Add(GetListItem(EDateFormatType.Day, false));
                listControl.Items.Add(GetListItem(EDateFormatType.Year, false));
                listControl.Items.Add(GetListItem(EDateFormatType.Chinese, false));
            }
        }

    }
}
