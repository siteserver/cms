using System.Web.UI.WebControls;
using SiteServer.Plugin;

namespace SiteServer.Utils
{
    public class DatabaseTypeUtils
    {
        public static DatabaseType GetEnumType(string typeStr)
        {
            var retval = DatabaseType.SqlServer;

            if (Equals(DatabaseType.MySql, typeStr))
            {
                retval = DatabaseType.MySql;
            }
            else if (Equals(DatabaseType.SqlServer, typeStr))
            {
                retval = DatabaseType.SqlServer;
            }
            else if (Equals(DatabaseType.PostgreSql, typeStr))
            {
                retval = DatabaseType.PostgreSql;
            }
            else if (Equals(DatabaseType.Oracle, typeStr))
            {
                retval = DatabaseType.Oracle;
            }

            return retval;
        }

        public static bool Equals(DatabaseType type, string typeStr)
        {
            if (string.IsNullOrEmpty(typeStr)) return false;
            if (string.Equals(type.Value.ToLower(), typeStr.ToLower()))
            {
                return true;
            }
            return false;
        }

        public static bool Equals(string typeStr, DatabaseType type)
        {
            return Equals(type, typeStr);
        }

        public static ListItem GetListItem(DatabaseType type, bool selected)
        {
            var item = new ListItem(type.Value, type.Value);
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
