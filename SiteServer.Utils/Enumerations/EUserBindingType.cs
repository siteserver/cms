using System;

namespace SiteServer.Utils.Enumerations
{
	public enum EUserBindingType
	{
		Weibo,
		QQ
	}

    public class EUserBindingTypeUtils
	{
		public static string GetValue(EUserBindingType type)
		{
		    if (type == EUserBindingType.Weibo)
			{
                return "Weibo";
			}
		    if (type == EUserBindingType.QQ)
		    {
		        return "QQ";
		    }
		    throw new Exception();
		}

		public static string GetText(EUserBindingType type)
		{
		    if (type == EUserBindingType.Weibo)
			{
				return "新浪微博";
			}
		    if (type == EUserBindingType.QQ)
		    {
		        return "QQ";
		    }
		    throw new Exception();
		}

		public static bool Equals(EUserBindingType type, string typeStr)
		{
			if (string.IsNullOrEmpty(typeStr)) return false;
			if (string.Equals(GetValue(type).ToLower(), typeStr.ToLower()))
			{
				return true;
			}
			return false;
		}

        public static bool Equals(string typeStr, EUserBindingType type)
        {
            return Equals(type, typeStr);
        }

		public static EUserBindingType GetEnumType(string typeStr)
		{
			var retval = EUserBindingType.Weibo;

            if (Equals(EUserBindingType.Weibo, typeStr))
			{
                retval = EUserBindingType.Weibo;
			}
			else if (Equals(EUserBindingType.QQ, typeStr))
			{
                retval = EUserBindingType.QQ;
			}

			return retval;
		}

	}
}
