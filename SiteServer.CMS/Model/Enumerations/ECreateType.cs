using System.Web.UI.WebControls;

namespace SiteServer.CMS.Model.Enumerations
{
    public enum ECreateType
	{
        None,
        Channel,
        Content,
        File,
        AllContent
    }

    public class ECreateTypeUtils
	{
		public static string GetValue(ECreateType type)
		{
		    if (type == ECreateType.Channel)
		    {
		        return "Channel";
		    }
		    if (type == ECreateType.Content)
		    {
		        return "Content";
		    }
		    if (type == ECreateType.File)
		    {
		        return "File";
		    }
		    if (type == ECreateType.AllContent)
		    {
		        return "AllContent";
		    }
		    return string.Empty;
		}

		public static ECreateType GetEnumType(string typeStr)
		{
            var retval = ECreateType.None;

            if (Equals(ECreateType.Channel, typeStr))
            {
                retval = ECreateType.Channel;
            }
            else if (Equals(ECreateType.Content, typeStr))
            {
                retval = ECreateType.Content;
            }
            else if (Equals(ECreateType.File, typeStr))
            {
                retval = ECreateType.File;
            }
            else if (Equals(ECreateType.AllContent, typeStr))
            {
                retval = ECreateType.AllContent;
            }

            return retval;
		}

        public static string GetText(ECreateType createType)
        {
            if (createType == ECreateType.Channel)
            {
                return "栏目页";
            }
            if (createType == ECreateType.Content)
            {
                return "内容页";
            }
            if (createType == ECreateType.File)
            {
                return "文件页";
            }
            if (createType == ECreateType.AllContent)
            {
                return "栏目下所有内容页";
            }

            return string.Empty;
        }

        public static bool Equals(ECreateType type, string typeStr)
		{
			if (string.IsNullOrEmpty(typeStr)) return false;
			if (string.Equals(GetValue(type).ToLower(), typeStr.ToLower()))
			{
				return true;
			}
			return false;
		}

		public static bool Equals(string typeStr, ECreateType type)
		{
			return Equals(type, typeStr);
		}

        public static ListItem GetListItem(ECreateType type, bool selected)
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
                listControl.Items.Add(GetListItem(ECreateType.Channel, false));
                listControl.Items.Add(GetListItem(ECreateType.Content, false));
                listControl.Items.Add(GetListItem(ECreateType.File, false));
            }
        }

	}
}
