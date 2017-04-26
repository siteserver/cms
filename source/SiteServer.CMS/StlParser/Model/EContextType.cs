using System;

namespace SiteServer.CMS.StlParser.Model
{
    public enum EContextType
	{
        Content,
        Channel,
        Comment,
        Photo,
        Each,
        InputContent,
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
            else if (type == EContextType.Channel)
			{
                return "Channel";
            }
            else if (type == EContextType.Comment)
            {
                return "Comment";
            }
            else if (type == EContextType.Photo)
            {
                return "Photo";
            }
            else if (type == EContextType.Each)
            {
                return "Each";
            }
            else if (type == EContextType.InputContent)
            {
                return "InputContent";
            }
            else if (type == EContextType.SqlContent)
            {
                return "SqlContent";
            }
            else if (type == EContextType.Site)
            {
                return "Site";
            }
            else if (type == EContextType.Undefined)
            {
                return "Undefined";
            }
			else
			{
				throw new Exception();
			}
		}

		public static EContextType GetEnumType(string typeStr)
		{
            var retval = EContextType.Undefined;

            if (Equals(EContextType.Content, typeStr))
			{
                retval = EContextType.Content;
			}
            else if (Equals(EContextType.Channel, typeStr))
			{
                retval = EContextType.Channel;
            }
            else if (Equals(EContextType.Comment, typeStr))
            {
                retval = EContextType.Comment;
            }
            else if (Equals(EContextType.Photo, typeStr))
            {
                retval = EContextType.Photo;
            }
            else if (Equals(EContextType.Each, typeStr))
            {
                retval = EContextType.Each;
            }
            else if (Equals(EContextType.InputContent, typeStr))
            {
                retval = EContextType.InputContent;
            }
            else if (Equals(EContextType.SqlContent, typeStr))
            {
                retval = EContextType.SqlContent;
            }
            else if (Equals(EContextType.Site, typeStr))
            {
                retval = EContextType.Site;
            }
            else if (Equals(EContextType.Undefined, typeStr))
            {
                retval = EContextType.Undefined;
            }

			return retval;
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
