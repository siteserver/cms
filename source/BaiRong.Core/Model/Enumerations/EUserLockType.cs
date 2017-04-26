using System;

namespace BaiRong.Core.Model.Enumerations
{
    public enum EUserLockType
    {
        Forever, //永久锁定
        Hours,  //小时
    }

    public class EUserLockTypeUtils
    {
        public static string GetValue(EUserLockType type)
        {
            if (type == EUserLockType.Forever)
            {
                return "Forever";
            }
            else if (type == EUserLockType.Hours)
            {
                return "Hours";
            }
            else
            {
                throw new Exception();
            }
        }

        public static EUserLockType GetEnumType(string typeStr)
        {
            if (Equals(typeStr, EUserLockType.Forever))
                return EUserLockType.Forever;
            else if (Equals(typeStr, EUserLockType.Hours))
                return EUserLockType.Hours;
            else
                return EUserLockType.Forever;
        }

        public static bool Equals(string typeStr, EUserLockType type)
        {
            if (string.IsNullOrEmpty(typeStr))
                return false;
            if (string.Equals(typeStr.ToLower(), GetValue(type).ToLower()))
                return true;
            return false;
        }

        public static bool Equals(EUserLockType type, string typeStr)
        {
            return Equals(typeStr, type);
        }
    }
}
