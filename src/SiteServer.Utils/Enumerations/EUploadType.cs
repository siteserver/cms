using System;

namespace SiteServer.Utils.Enumerations
{
	public enum EUploadType
	{
		Image,
		Video,
		File,
        Special,
        AdvImage
	}

    public class EUploadTypeUtils
	{
		public static string GetValue(EUploadType type)
		{
		    if (type == EUploadType.Image)
			{
                return "Image";
			}
		    if (type == EUploadType.Video)
		    {
		        return "Video";
		    }
		    if (type == EUploadType.File)
		    {
		        return "File";
		    }
		    if (type == EUploadType.Special)
		    {
		        return "Special";
		    }
		    if (type == EUploadType.AdvImage)
		    {
		        return "AdvImage";
		    }
		    throw new Exception();
		}

		public static string GetText(EUploadType type)
		{
		    if (type == EUploadType.Image)
			{
				return "图片";
			}
		    if (type == EUploadType.Video)
		    {
		        return "视频";
		    }
		    if (type == EUploadType.File)
		    {
		        return "文件";
		    }
		    if (type == EUploadType.Special)
		    {
		        return "专题";
		    }
		    if (type == EUploadType.AdvImage)
		    {
		        return "广告";
		    }
		    throw new Exception();
		}

		public static EUploadType GetEnumType(string typeStr)
		{
			var retval = EUploadType.Image;

            if (Equals(EUploadType.Image, typeStr))
			{
                retval = EUploadType.Image;
			}
			else if (Equals(EUploadType.Video, typeStr))
			{
                retval = EUploadType.Video;
            }
            else if (Equals(EUploadType.File, typeStr))
            {
                retval = EUploadType.File;
            }
            else if (Equals(EUploadType.Special, typeStr))
            {
                retval = EUploadType.Special;
            }
            else if (Equals(EUploadType.AdvImage, typeStr))
            {
                retval = EUploadType.AdvImage;
            }
			return retval;
		}

		public static bool Equals(EUploadType type, string typeStr)
		{
			if (string.IsNullOrEmpty(typeStr)) return false;
			if (string.Equals(GetValue(type).ToLower(), typeStr.ToLower()))
			{
				return true;
			}
			return false;
		}

        public static bool Equals(string typeStr, EUploadType type)
        {
            return Equals(type, typeStr);
        }
	}
}
