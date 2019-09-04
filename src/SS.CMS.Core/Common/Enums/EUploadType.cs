using System;
using SS.CMS.Enums;

namespace SS.CMS.Core.Common.Enums
{
    public class UploadTypeUtils
    {
        public static string GetValue(UploadType type)
        {
            if (type == UploadType.Image)
            {
                return "Image";
            }
            if (type == UploadType.Video)
            {
                return "Video";
            }
            if (type == UploadType.File)
            {
                return "File";
            }
            if (type == UploadType.Special)
            {
                return "Special";
            }
            throw new Exception();
        }

        public static string GetText(UploadType type)
        {
            if (type == UploadType.Image)
            {
                return "图片";
            }
            if (type == UploadType.Video)
            {
                return "视频";
            }
            if (type == UploadType.File)
            {
                return "文件";
            }
            if (type == UploadType.Special)
            {
                return "专题";
            }
            throw new Exception();
        }

        public static UploadType GetEnumType(string typeStr)
        {
            var retval = UploadType.Image;

            if (Equals(UploadType.Image, typeStr))
            {
                retval = UploadType.Image;
            }
            else if (Equals(UploadType.Video, typeStr))
            {
                retval = UploadType.Video;
            }
            else if (Equals(UploadType.File, typeStr))
            {
                retval = UploadType.File;
            }
            else if (Equals(UploadType.Special, typeStr))
            {
                retval = UploadType.Special;
            }
            return retval;
        }

        public static bool Equals(UploadType type, string typeStr)
        {
            if (string.IsNullOrEmpty(typeStr)) return false;
            if (string.Equals(GetValue(type).ToLower(), typeStr.ToLower()))
            {
                return true;
            }
            return false;
        }

        public static bool Equals(string typeStr, UploadType type)
        {
            return Equals(type, typeStr);
        }
    }
}
