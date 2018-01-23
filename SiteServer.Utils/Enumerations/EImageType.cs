using System;

namespace SiteServer.Utils.Enumerations
{
	public enum EImageType
	{
		Jpg,
		Jpeg,
		Gif,
		Png,
		Bmp,
		Unknown
	}

	public class EImageTypeUtils
	{
		public static string GetValue(EImageType type)
		{
		    if (type == EImageType.Jpg)
			{
				return "jpg";
			}
		    if (type == EImageType.Jpeg)
		    {
		        return "jpeg";
		    }
		    if (type == EImageType.Gif)
		    {
		        return "gif";
		    }
		    if (type == EImageType.Png)
		    {
		        return "png";
		    }
		    if (type == EImageType.Bmp)
		    {
		        return "bmp";
		    }
		    if (type == EImageType.Unknown)
		    {
		        return "unknown";
		    }
		    throw new Exception();
		}

		public static EImageType GetEnumType(string typeStr)
		{
			var retval = EImageType.Unknown;

			if (Equals(EImageType.Jpg, typeStr))
			{
				retval = EImageType.Jpg;
			}
			else if (Equals(EImageType.Jpeg, typeStr))
			{
				retval = EImageType.Jpeg;
			}
			else if (Equals(EImageType.Gif, typeStr))
			{
				retval = EImageType.Gif;
			}
			else if (Equals(EImageType.Png, typeStr))
			{
				retval = EImageType.Png;
			}
			else if (Equals(EImageType.Bmp, typeStr))
			{
				retval = EImageType.Bmp;
			}

			return retval;
		}

		public static bool Equals(EImageType type, string typeStr)
		{
			if (string.IsNullOrEmpty(typeStr)) return false;
			if (string.Equals(GetValue(type).ToLower(), typeStr.ToLower()))
			{
				return true;
			}
			return false;
		}

	}
}
