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
		    if (type == EGatherType.Database)
		    {
		        return "Database";
		    }
		    if (type == EGatherType.File)
		    {
		        return "File";
		    }
		    return "Undefined";
		}

		public static string GetText(EGatherType type)
		{
		    if (type == EGatherType.Web)
            {
                return "Web页面";
            }
		    if (type == EGatherType.Database)
		    {
		        return "数据库";
		    }
		    if (type == EGatherType.File)
		    {
		        return "单文件页";
		    }

		    return "Undefined";
		}

		public static EGatherType GetEnumType(string typeStr)
		{
            var retVal = EGatherType.Undefined;

            if (Equals(EGatherType.Web, typeStr))
			{
                retVal = EGatherType.Web;
            }
            else if (Equals(EGatherType.Database, typeStr))
            {
                retVal = EGatherType.Database;
            }
            else if (Equals(EGatherType.File, typeStr))
			{
                retVal = EGatherType.File;
            }

			return retVal;
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
