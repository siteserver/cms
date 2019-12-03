using System.Web.UI.WebControls;
using Datory;
using SiteServer.Abstractions;

namespace SiteServer.CMS.Context
{
    public class DatabaseTypeUtils
    {
        public static ListItem GetListItem(DatabaseType type, bool selected)
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
            listControl.Items.Add(GetListItem(DatabaseType.MySql, false));
            listControl.Items.Add(GetListItem(DatabaseType.SqlServer, false));
            listControl.Items.Add(GetListItem(DatabaseType.PostgreSql, false));
            listControl.Items.Add(GetListItem(DatabaseType.Oracle, false));
        }
    }
}
