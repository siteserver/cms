using System.ComponentModel.DataAnnotations;

namespace SiteServer.Abstractions
{
    public enum EBackupType
	{
        [Display(Name = "It's autumn")]
        Undefined,
        Templates,              //模板页
        ChannelsAndContents,    //栏目及内容
        Files,                  //文件
        Site,	                //整站
	}

    public static class EBackupTypeUtils
	{
		public static string GetValue(EBackupType type)
		{
		    if (type == EBackupType.Templates)
			{
                return "Templates";
            }
		    if (type == EBackupType.ChannelsAndContents)
		    {
		        return "ChannelsAndContents";
		    }
		    if (type == EBackupType.Files)
		    {
		        return "Files";
		    }
		    if (type == EBackupType.Site)
		    {
		        return "Site";
		    }
		    return "Undefined";
		}

		public static string GetText(EBackupType type)
		{
		    if (type == EBackupType.Templates)
            {
                return "显示模板";
            }
		    if (type == EBackupType.ChannelsAndContents)
		    {
		        return "栏目及内容";
		    }
		    if (type == EBackupType.Files)
		    {
		        return "文件";
		    }
		    if (type == EBackupType.Site)
		    {
		        return "整站";
		    }

		    return "Undefined";
		}

		public static EBackupType GetEnumType(string typeStr)
		{
            var retVal = EBackupType.Undefined;

            if (Equals(EBackupType.Templates, typeStr))
			{
                retVal = EBackupType.Templates;
            }
            else if (Equals(EBackupType.ChannelsAndContents, typeStr))
            {
                retVal = EBackupType.ChannelsAndContents;
            }
            else if (Equals(EBackupType.Files, typeStr))
            {
                retVal = EBackupType.Files;
            }
            else if (Equals(EBackupType.Site, typeStr))
			{
                retVal = EBackupType.Site;
            }

			return retVal;
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
    }
}
