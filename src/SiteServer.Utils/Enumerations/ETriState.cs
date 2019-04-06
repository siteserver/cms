using System;

namespace SiteServer.Utils.Enumerations
{
    public enum ETriState
	{
        All, 
        True,
        False
	}

    public class ETriStateUtils
	{
		public static string GetValue(ETriState type)
		{
		    if (type == ETriState.All)
			{
                return "All";
			}
		    if (type == ETriState.True)
		    {
		        return "True";
		    }
		    if (type == ETriState.False)
		    {
		        return "False";
		    }
		    throw new Exception();
		}

        public static string GetText(ETriState type, string allText, string trueText, string falseText)
        {
            if (type == ETriState.All)
            {
                return allText;
            }
            if (type == ETriState.False)
            {
                return falseText;
            }
            return trueText;
        }

		public static ETriState GetEnumType(string typeStr)
		{
            var retval = ETriState.All;

            if (Equals(ETriState.All, typeStr))
			{
                retval = ETriState.All;
			}
            else if (Equals(ETriState.True, typeStr))
			{
                retval = ETriState.True;
            }
            else if (Equals(ETriState.False, typeStr))
            {
                retval = ETriState.False;
            }

			return retval;
		}

		public static bool Equals(ETriState type, string typeStr)
		{
			if (string.IsNullOrEmpty(typeStr)) return false;
			if (string.Equals(GetValue(type).ToLower(), typeStr.ToLower()))
			{
				return true;
			}
			return false;
		}

        public static bool Equals(string typeStr, ETriState type)
        {
            return Equals(type, typeStr);
        }

        
	}
}
