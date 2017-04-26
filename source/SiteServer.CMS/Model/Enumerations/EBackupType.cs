using System.Web.UI.WebControls;

namespace SiteServer.CMS.Model.Enumerations
{
    public enum EBackupType
	{
        Undefined,
        Templates,              //模板页
        ChannelsAndContents,    //栏目及内容
        Files,                  //文件
        Site,	                //整站
	}

    public class EBackupTypeUtils
	{
		public static string GetValue(EBackupType type)
		{
            if (type == EBackupType.Templates)
			{
                return "Templates";
            }
            else if (type == EBackupType.ChannelsAndContents)
            {
                return "ChannelsAndContents";
            }
            else if (type == EBackupType.Files)
            {
                return "Files";
            }
            else if (type == EBackupType.Site)
			{
                return "Site";
            }
			else
			{
                return "Undefined";
			}
		}

		public static string GetText(EBackupType type)
		{
            if (type == EBackupType.Templates)
            {
                return "显示模板";
            }
            else if (type == EBackupType.ChannelsAndContents)
            {
                return "栏目及内容";
            }
            else if (type == EBackupType.Files)
            {
                return "文件";
            }
            else if (type == EBackupType.Site)
			{
                return "整站";
            }
			
			else
			{
                return "Undefined";
			}
		}

		public static EBackupType GetEnumType(string typeStr)
		{
            var retval = EBackupType.Undefined;

            if (Equals(EBackupType.Templates, typeStr))
			{
                retval = EBackupType.Templates;
            }
            else if (Equals(EBackupType.ChannelsAndContents, typeStr))
            {
                retval = EBackupType.ChannelsAndContents;
            }
            else if (Equals(EBackupType.Files, typeStr))
            {
                retval = EBackupType.Files;
            }
            else if (Equals(EBackupType.Site, typeStr))
			{
                retval = EBackupType.Site;
            }

			return retval;
		}

		public static bool Equals(EBackupType type, string typeStr)
		{
			if (string.IsNullOrEmpty(typeStr)) return false;
			if (string.Equals(GetValue(type).ToLower(), typeStr.ToLower()))
			{
				return true;
			}
			return false;
		}

		public static bool Equals(string typeStr, EBackupType type)
		{
			return Equals(type, typeStr);
		}

        public static ListItem GetListItem(EBackupType type, bool selected)
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
                listControl.Items.Add(GetListItem(EBackupType.Templates, false));
                listControl.Items.Add(GetListItem(EBackupType.ChannelsAndContents, false));
                listControl.Items.Add(GetListItem(EBackupType.Files, false));
                listControl.Items.Add(GetListItem(EBackupType.Site, false));
            }
        }

	}
}
