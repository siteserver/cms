using System;

namespace SS.CMS.Core.Common.Enums
{
    public enum EPositionType
    {
        LeftTop,                //左上
        LeftBottom,             //左下
        RightTop,               //右上
        RightBottom             //右下
    }

    public class EPositionTypeUtils
    {
        public static string GetValue(EPositionType type)
        {
            if (type == EPositionType.LeftTop)
            {
                return "LeftTop";
            }
            if (type == EPositionType.LeftBottom)
            {
                return "LeftBottom";
            }
            if (type == EPositionType.RightTop)
            {
                return "RightTop";
            }
            if (type == EPositionType.RightBottom)
            {
                return "RightBottom";
            }
            throw new Exception();
        }

        public static string GetText(EPositionType type)
        {
            if (type == EPositionType.LeftTop)
            {
                return "左上";
            }
            if (type == EPositionType.LeftBottom)
            {
                return "左下";
            }
            if (type == EPositionType.RightTop)
            {
                return "右上";
            }
            if (type == EPositionType.RightBottom)
            {
                return "右下";
            }
            throw new Exception();
        }

        public static EPositionType GetEnumType(string typeStr)
        {
            var retval = EPositionType.LeftTop;

            if (Equals(EPositionType.LeftTop, typeStr))
            {
                retval = EPositionType.LeftTop;
            }
            else if (Equals(EPositionType.LeftBottom, typeStr))
            {
                retval = EPositionType.LeftBottom;
            }
            else if (Equals(EPositionType.RightTop, typeStr))
            {
                retval = EPositionType.RightTop;
            }
            else if (Equals(EPositionType.RightBottom, typeStr))
            {
                retval = EPositionType.RightBottom;
            }

            return retval;
        }

        public static bool Equals(EPositionType type, string typeStr)
        {
            if (string.IsNullOrEmpty(typeStr)) return false;
            if (string.Equals(GetValue(type).ToLower(), typeStr.ToLower()))
            {
                return true;
            }
            return false;
        }

        public static bool Equals(string typeStr, EPositionType type)
        {
            return Equals(type, typeStr);
        }
    }
}
