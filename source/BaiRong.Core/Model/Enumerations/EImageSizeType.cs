using System;
using System.Collections;
using System.Drawing;

namespace BaiRong.Core.Model.Enumerations
{
	public enum EImageSizeType
	{
		Square,
		Thumbnail,
		Small,
		Medium,
		Original
	}

	public class EImageSizeTypeUtils
	{
		public static string GetValue(EImageSizeType type)
		{
			if (type == EImageSizeType.Square)
			{
				return "Square";
			}
			else if (type == EImageSizeType.Thumbnail)
			{
				return "Thumbnail";
			}
			else if (type == EImageSizeType.Small)
			{
				return "Small";
			}
			else if (type == EImageSizeType.Medium)
			{
				return "Medium";
			}
			else if (type == EImageSizeType.Original)
			{
				return "Original";
			}
			else
			{
				throw new Exception();
			}
		}

		public static string GetText(EImageSizeType type)
		{
			if (type == EImageSizeType.Square)
			{
				return "����";
			}
			else if (type == EImageSizeType.Thumbnail)
			{
				return "Сͼ";
			}
			else if (type == EImageSizeType.Small)
			{
				return "��С�ߴ�";
			}
			else if (type == EImageSizeType.Medium)
			{
				return "�еȳߴ�";
			}
			else if (type == EImageSizeType.Original)
			{
				return "ԭʼ�ߴ�";
			}
			else
			{
				throw new Exception();
			}
		}

		public static EImageSizeType GetEnumType(string typeStr)
		{
			var retval = EImageSizeType.Original;

			if (Equals(EImageSizeType.Square, typeStr))
			{
				retval = EImageSizeType.Square;
			}
			else if (Equals(EImageSizeType.Thumbnail, typeStr))
			{
				retval = EImageSizeType.Thumbnail;
			}
			else if (Equals(EImageSizeType.Small, typeStr))
			{
				retval = EImageSizeType.Small;
			}
			else if (Equals(EImageSizeType.Medium, typeStr))
			{
				retval = EImageSizeType.Medium;
			}
			else if (Equals(EImageSizeType.Original, typeStr))
			{
				retval = EImageSizeType.Original;
			}

			return retval;
		}

		public static string GetAppendix(EImageSizeType type)
		{
			var retval = string.Empty;

			if (type == EImageSizeType.Square)
			{
				retval = "_s";
			}
			else if (type == EImageSizeType.Thumbnail)
			{
				retval = "_t";
			}
			else if (type == EImageSizeType.Small)
			{
				retval = "_m";
			}
			else if (type == EImageSizeType.Medium)
			{
				retval = "_e";
			}
			else if (type == EImageSizeType.Original)
			{
				retval = "_o";
			}

			return retval;
		}

		public const int Size_Max_Medium = 500;
		public const int Size_Max_Small = 240;
		public const int Size_Max_Thumbnail = 100;

        public const int Size_Square = 75;

        public static int GetMaxSize(EImageSizeType type)
        {
            var size = Size_Max_Medium;

            if (type == EImageSizeType.Square)
            {
                size = Size_Square;
            }
            else if (type == EImageSizeType.Thumbnail)
            {
                size = Size_Max_Thumbnail;
            }
            else if (type == EImageSizeType.Small)
            {
                size = Size_Max_Small;
            }

            return size;
        }

		public static ArrayList GetEImageSizeTypeArrayListByLargerInt(int largerInt)
		{
			var arraylist = new ArrayList();

            arraylist.Add(EImageSizeType.Square);

            if (largerInt > Size_Max_Thumbnail)
            {
                arraylist.Add(EImageSizeType.Thumbnail);
            }

            arraylist.Add(EImageSizeType.Small);

            if (largerInt > Size_Max_Medium)
            {
                arraylist.Add(EImageSizeType.Medium);
            }

			arraylist.Add(EImageSizeType.Original);

			return arraylist;
		}

		private static int GetSmallerInt(Size originalSize, EImageSizeType sizeType, bool isWidthLarger, int largerInt)
		{
			var retval = 0;
			if (isWidthLarger)
			{
				retval = Convert.ToInt32((Convert.ToDouble(largerInt) / Convert.ToDouble(originalSize.Width)) * Convert.ToDouble(originalSize.Height));
			}
			else
			{
				retval = Convert.ToInt32((Convert.ToDouble(largerInt) / Convert.ToDouble(originalSize.Height)) * Convert.ToDouble(originalSize.Width));
			}
			return retval;
		}

		public static Size GetSize(Size originalSize, EImageSizeType sizeType)
		{
			var size = new Size(originalSize.Width, originalSize.Height);
			var isWidthLarger = (originalSize.Width > originalSize.Height);
			var largerInt = Math.Max(originalSize.Width, originalSize.Height);

			if (sizeType == EImageSizeType.Medium)
			{
				largerInt = Math.Min(Size_Max_Medium, largerInt);
			}
			else if (sizeType == EImageSizeType.Small)
			{
				largerInt = Math.Min(Size_Max_Small, largerInt);
			}
			else if (sizeType == EImageSizeType.Thumbnail)
			{
				largerInt = Math.Min(Size_Max_Thumbnail, largerInt);
			}
			else if (sizeType == EImageSizeType.Square)
			{
                var squareWidth = Size_Square;
                var squareHeight = Size_Square;
                if (originalSize.Width < Size_Square)
                {
                    squareWidth = originalSize.Width;
                }
                if (originalSize.Height < Size_Square)
                {
                    squareHeight = originalSize.Height;
                }
                return new Size(squareWidth, squareHeight);
			}

			if (largerInt > 0)
			{
				if (isWidthLarger)
				{
					size.Width = largerInt;
					size.Height = GetSmallerInt(originalSize, sizeType, isWidthLarger, largerInt);
				}
				else
				{
					size.Height = largerInt;
					size.Width = GetSmallerInt(originalSize, sizeType, isWidthLarger, largerInt);
				}
			}

			return size;
		}

		public static bool Equals(EImageSizeType type, string typeStr)
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
