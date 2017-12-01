using System;
using System.Web.UI.WebControls;

namespace BaiRong.Core.Model.Enumerations
{
	public enum EDatabaseType
	{
        MySql,
        SqlServer,
        PostgreSql,
        Oracle
    }

    public class EDatabaseTypeUtils
	{
		public static string GetValue(EDatabaseType type)
		{
            switch (type)
            {
                case EDatabaseType.MySql:
                    return "MySql";
                case EDatabaseType.SqlServer:
                    return "SqlServer";
                case EDatabaseType.PostgreSql:
                    return "PostgreSql";
                case EDatabaseType.Oracle:
                    return "Oracle";
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
		}

		public static EDatabaseType GetEnumType(string typeStr)
		{
			var retval = EDatabaseType.SqlServer;

            if (Equals(EDatabaseType.MySql, typeStr))
            {
                retval = EDatabaseType.MySql;
            }
            else if (Equals(EDatabaseType.SqlServer, typeStr))
            {
                retval = EDatabaseType.SqlServer;
            }
            else if (Equals(EDatabaseType.PostgreSql, typeStr))
            {
                retval = EDatabaseType.PostgreSql;
            }
            else if (Equals(EDatabaseType.Oracle, typeStr))
            {
                retval = EDatabaseType.Oracle;
            }

            return retval;
		}

		public static bool Equals(EDatabaseType type, string typeStr)
		{
			if (string.IsNullOrEmpty(typeStr)) return false;
			if (string.Equals(GetValue(type).ToLower(), typeStr.ToLower()))
			{
				return true;
			}
			return false;
		}

        public static bool Equals(string typeStr, EDatabaseType type)
        {
            return Equals(type, typeStr);
        }

        public static ListItem GetListItem(EDatabaseType type, bool selected)
        {
            var item = new ListItem(GetValue(type), GetValue(type));
            if (selected)
            {
                item.Selected = true;
            }
            return item;
        }

        public static void AddListItems(ListControl listControl)
        {
            if (listControl == null) return;
            listControl.Items.Add(GetListItem(EDatabaseType.MySql, false));
            listControl.Items.Add(GetListItem(EDatabaseType.SqlServer, false));
            listControl.Items.Add(GetListItem(EDatabaseType.PostgreSql, false));
            listControl.Items.Add(GetListItem(EDatabaseType.Oracle, false));
        }
    }
}
