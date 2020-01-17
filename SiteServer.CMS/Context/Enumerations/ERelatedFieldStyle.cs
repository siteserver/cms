using System;

namespace SiteServer.CMS.Context.Enumerations
{
    public enum ERelatedFieldStyle
	{
		Horizontal,                 //水平显示
        Virtical,                   //垂直显示
    }

    public static class ERelatedFieldStyleUtils
	{
		public static string GetValue(ERelatedFieldStyle type)
		{
		    if (type == ERelatedFieldStyle.Horizontal)
			{
                return "Horizontal";
			}
		    if (type == ERelatedFieldStyle.Virtical)
		    {
		        return "Virtical";
		    }
		    throw new Exception();
		}

		public static string GetText(ERelatedFieldStyle type)
		{
		    if (type == ERelatedFieldStyle.Horizontal)
			{
                return "水平显示";
			}
		    if (type == ERelatedFieldStyle.Virtical)
		    {
		        return "垂直显示";
		    }
		    throw new Exception();
		}

		public static ERelatedFieldStyle GetEnumType(string typeStr)
		{
            var retVal = ERelatedFieldStyle.Horizontal;

            if (Equals(ERelatedFieldStyle.Horizontal, typeStr))
			{
                retVal = ERelatedFieldStyle.Horizontal;
			}
			else if (Equals(ERelatedFieldStyle.Virtical, typeStr))
			{
                retVal = ERelatedFieldStyle.Virtical;
			}
			
			return retVal;
		}

		public static bool Equals(ERelatedFieldStyle type, string typeStr)
		{
			if (string.IsNullOrEmpty(typeStr)) return false;
			if (string.Equals(GetValue(type).ToLower(), typeStr.ToLower()))
			{
				return true;
			}
			return false;
		}

        public static bool Equals(string typeStr, ERelatedFieldStyle type)
        {
            return Equals(type, typeStr);
        }
	}
}
