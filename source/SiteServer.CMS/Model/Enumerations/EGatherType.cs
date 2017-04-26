using System.Web.UI.WebControls;

namespace SiteServer.CMS.Model.Enumerations
{
    public enum EGatherType
	{
        Undefined,
        Web,              //Web页面
        Database,	      //数据库
        File			  //单文件页
	}

    public class EGatherTypeUtils
	{
		public static string GetValue(EGatherType type)
		{
            if (type == EGatherType.Web)
			{
                return "Web";
            }
            else if (type == EGatherType.Database)
            {
                return "Database";
            }
            else if (type == EGatherType.File)
			{
                return "File";
            }
			else
			{
                return "Undefined";
			}
		}

		public static string GetText(EGatherType type)
		{
            if (type == EGatherType.Web)
            {
                return "Web页面";
            }
            else if (type == EGatherType.Database)
            {
                return "数据库";
            }
            else if (type == EGatherType.File)
			{
                return "单文件页";
            }
			
			else
			{
                return "Undefined";
			}
		}

		public static EGatherType GetEnumType(string typeStr)
		{
            var retval = EGatherType.Undefined;

            if (Equals(EGatherType.Web, typeStr))
			{
                retval = EGatherType.Web;
            }
            else if (Equals(EGatherType.Database, typeStr))
            {
                retval = EGatherType.Database;
            }
            else if (Equals(EGatherType.File, typeStr))
			{
                retval = EGatherType.File;
            }

			return retval;
		}

		public static bool Equals(EGatherType type, string typeStr)
		{
			if (string.IsNullOrEmpty(typeStr)) return false;
			if (string.Equals(GetValue(type).ToLower(), typeStr.ToLower()))
			{
				return true;
			}
			return false;
		}

		public static bool Equals(string typeStr, EGatherType type)
		{
			return Equals(type, typeStr);
		}

        public static ListItem GetListItem(EGatherType type, bool selected)
        {
            var item = new ListItem(GetText(type), GetValue(type));
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
                listControl.Items.Add(GetListItem(EGatherType.Web, false));
                listControl.Items.Add(GetListItem(EGatherType.Database, false));
                listControl.Items.Add(GetListItem(EGatherType.File, false));
            }
        }

	}
}
