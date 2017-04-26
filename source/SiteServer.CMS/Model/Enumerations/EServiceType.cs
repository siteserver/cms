using System;
using System.Web.UI.WebControls;

namespace SiteServer.CMS.Model.Enumerations
{
    public enum EServiceType
	{
		Create,	        //生成
        Gather,			//采集
        Backup,			//备份
	}

    public class EServiceTypeUtils
	{
		public static string GetValue(EServiceType type)
		{
            if (type == EServiceType.Create)
			{
                return "Create";
			}
            else if (type == EServiceType.Gather)
			{
                return "Gather";
			}
            else if (type == EServiceType.Backup)
			{
                return "Backup";
			}
			else
			{
				throw new Exception();
			}
		}

        public static string GetClassName(EServiceType type)
        {
            return GetValue(type) + "Execution";
        }

		public static string GetText(EServiceType type)
		{
            if (type == EServiceType.Create)
			{
                return "定时生成";
			}
            else if (type == EServiceType.Gather)
			{
                return "定时采集";
			}
            else if (type == EServiceType.Backup)
			{
                return "定时备份";
			}
			else
			{
				throw new Exception();
			}
		}

		public static EServiceType GetEnumType(string typeStr)
		{
			var retval = EServiceType.Create;

            if (Equals(EServiceType.Create, typeStr))
			{
                retval = EServiceType.Create;
			}
            else if (Equals(EServiceType.Gather, typeStr))
			{
                retval = EServiceType.Gather;
			}
            else if (Equals(EServiceType.Backup, typeStr))
			{
                retval = EServiceType.Backup;
			}

			return retval;
		}

		public static bool Equals(EServiceType type, string typeStr)
		{
			if (string.IsNullOrEmpty(typeStr)) return false;
			if (string.Equals(GetValue(type).ToLower(), typeStr.ToLower()))
			{
				return true;
			}
			return false;
		}

		public static bool Equals(string typeStr, EServiceType type)
		{
			return Equals(type, typeStr);
		}

        public static ListItem GetListItem(EServiceType type, bool selected)
        {
            var item = new ListItem(GetText(type), GetValue(type));
            if (selected)
            {
                item.Selected = true;
            }
            return item;
        }
	}
}
