using System;


namespace SiteServer.Abstractions
{
	/// <summary>
	/// 批量转移类型
	/// </summary>
	public enum ETranslateType
	{
		Content,				//仅转移内容
		Channel,				//仅转移栏目
		All						//转移栏目及内容
	}

	public static class ETranslateTypeUtils
	{
		public static string GetValue(ETranslateType type)
		{
		    if (type == ETranslateType.Content)
			{
				return "Body";
			}
		    if (type == ETranslateType.Channel)
		    {
		        return "Channel";
		    }
		    if (type == ETranslateType.All)
		    {
		        return "All";
		    }
		    throw new Exception();
		}

		public static string GetText(ETranslateType type)
		{
		    if (type == ETranslateType.Content)
			{
				return "仅转移内容";
			}
		    if (type == ETranslateType.Channel)
		    {
		        return "仅转移栏目";
		    }
		    if (type == ETranslateType.All)
		    {
		        return "转移栏目及内容";
		    }
		    throw new Exception();
		}

		public static ETranslateType GetEnumType(string typeStr)
		{
			var retVal = ETranslateType.Content;

			if (Equals(ETranslateType.Content, typeStr))
			{
				retVal = ETranslateType.Content;
			}
			else if (Equals(ETranslateType.Channel, typeStr))
			{
				retVal = ETranslateType.Channel;
			}
			else if (Equals(ETranslateType.All, typeStr))
			{
				retVal = ETranslateType.All;
			}

			return retVal;
		}

		public static bool Equals(ETranslateType type, string typeStr)
		{
			if (string.IsNullOrEmpty(typeStr)) return false;
			if (string.Equals(GetValue(type).ToLower(), typeStr.ToLower()))
			{
				return true;
			}
			return false;
		}

        public static bool Equals(string typeStr, ETranslateType type)
        {
            return Equals(type, typeStr);
        }
    }
}
