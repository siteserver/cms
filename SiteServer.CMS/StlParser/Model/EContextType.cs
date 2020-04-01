using System;
using System.Collections.Generic;

namespace SiteServer.CMS.StlParser.Model
{
    public enum EContextType
	{
        Content,
        Channel,
        Each,
        SqlContent,
        Site,
        Undefined
	}

    public class EContextTypeUtils
	{
		public static string GetValue(EContextType type)
		{
		    if (type == EContextType.Content)
            {
                return "Content";
            }
		    if (type == EContextType.Channel)
		    {
		        return "Channel";
		    }
		    if (type == EContextType.Each)
		    {
		        return "Each";
		    }
		    if (type == EContextType.SqlContent)
		    {
		        return "SqlContent";
		    }
		    if (type == EContextType.Site)
		    {
		        return "Site";
		    }
		    if (type == EContextType.Undefined)
		    {
		        return "Undefined";
		    }

		    throw new Exception();
		}

		public static EContextType GetEnumType(string typeStr)
		{
            var retVal = EContextType.Undefined;

            if (Equals(EContextType.Content, typeStr))
			{
                retVal = EContextType.Content;
			}
            else if (Equals(EContextType.Channel, typeStr))
			{
                retVal = EContextType.Channel;
            }
            else if (Equals(EContextType.Each, typeStr))
            {
                retVal = EContextType.Each;
            }
            else if (Equals(EContextType.SqlContent, typeStr))
            {
                retVal = EContextType.SqlContent;
            }
            else if (Equals(EContextType.Site, typeStr))
            {
                retVal = EContextType.Site;
            }
            else if (Equals(EContextType.Undefined, typeStr))
            {
                retVal = EContextType.Undefined;
            }

			return retVal;
		}

		public static bool Equals(EContextType type, string typeStr)
		{
			if (string.IsNullOrEmpty(typeStr)) return false;
			if (string.Equals(GetValue(type).ToLower(), typeStr.ToLower()))
			{
				return true;
			}
			return false;
		}

        public static bool Equals(string typeStr, EContextType type)
        {
            return Equals(type, typeStr);
        }
	}
}
