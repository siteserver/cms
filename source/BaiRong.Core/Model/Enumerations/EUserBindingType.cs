using System;

namespace BaiRong.Core.Model.Enumerations
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
            else if (type == EUserBindingType.QQ)
			{
                return "QQ";
			}
			else
			{
				throw new Exception();
			}
		}

		public static string GetText(EUserBindingType type)
		{
            if (type == EUserBindingType.Weibo)
			{
				return "新浪微博";
			}
            else if (type == EUserBindingType.QQ)
			{
                return "QQ";
			}
			else
			{
				throw new Exception();
			}
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
