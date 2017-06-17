using System;

namespace BaiRong.Core.Model.Enumerations
{
	
	public enum ECountType
	{
		View,
		Download
	}

	public class ECountTypeUtils
	{
		public static string GetValue(ECountType type)
		{
			if (type == ECountType.View)
			{
				return "View";
			}
			else if (type == ECountType.Download)
			{
				return "Download";
			}
			else
			{
				throw new Exception();
			}
		}

		public static ECountType GetEnumType(string typeStr)
		{
			var retval = ECountType.View;

			if (Equals(ECountType.View, typeStr))
			{
				retval = ECountType.View;
			}
			else if (Equals(ECountType.Download, typeStr))
			{
				retval = ECountType.Download;
			}

			return retval;
		}

		public static bool Equals(ECountType type, string typeStr)
		{
			if (string.IsNullOrEmpty(typeStr)) return false;
			if (string.Equals(GetValue(type).ToLower(), typeStr.ToLower()))
			{
				return true;
			}
			return false;
		}

        public static bool Equals(string typeStr, ECountType type)
        {
            return Equals(type, typeStr);
        }
	}
}
