using System;

namespace SiteServer.CMS.Model.Enumerations
{
    public enum EProgressType
    {
        CreateChannels,
        CreateContents,
    }

    public class EProgressTypeUtils
    {
        public static string GetValue(EProgressType type)
        {
            if (type == EProgressType.CreateChannels)
            {
                return "CreateChannels";
            }
            else if (type == EProgressType.CreateContents)
            {
                return "CreateContents";
            }
            else
            {
                throw new Exception();
            }
        }

        public static EProgressType GetEnumType(string typeStr)
        {
            var retval = EProgressType.CreateChannels;

            if (Equals(EProgressType.CreateChannels, typeStr))
            {
                retval = EProgressType.CreateChannels;
            }
            else if (Equals(EProgressType.CreateContents, typeStr))
            {
                retval = EProgressType.CreateContents;
            }

            return retval;
        }

        public static bool Equals(EProgressType type, string typeStr)
        {
            if (string.IsNullOrEmpty(typeStr)) return false;
            if (string.Equals(GetValue(type).ToLower(), typeStr.ToLower()))
            {
                return true;
            }
            return false;
        }

        public static bool Equals(string typeStr, EProgressType type)
        {
            return Equals(type, typeStr);
        }

        public const string CACHE_TOTAL_COUNT = "_TotalCount";
        public const string CACHE_CURRENT_COUNT = "_CurrentCount";
        public const string CACHE_MESSAGE = "_Message";
    }
}
