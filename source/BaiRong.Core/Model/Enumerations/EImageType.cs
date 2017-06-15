using System;

namespace BaiRong.Core.Model.Enumerations
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
			else if (type == EImageType.Jpeg)
			{
				return "jpeg";
			}
			else if (type == EImageType.Gif)
			{
				return "gif";
			}
			else if (type == EImageType.Png)
			{
				return "png";
			}
			else if (type == EImageType.Bmp)
			{
				return "bmp";
			}
			else if (type == EImageType.Unknown)
			{
				return "unknown";
			}
			else
			{
				throw new Exception();
			}
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
