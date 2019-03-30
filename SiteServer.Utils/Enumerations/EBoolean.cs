using System;

namespace SiteServer.Utils.Enumerations
{
	public enum EBoolean
	{
		True,
		False
	}

	public class EBooleanUtils
	{
	    public static string GetValue(EBoolean type)
		{
		    if (type == EBoolean.True)
			{
				return "True";
			}
		    if (type == EBoolean.False)
		    {
		        return "False";
		    }
		    throw new Exception();
		}

        public static string GetText(EBoolean type)
        {
            if (type == EBoolean.True)
			{
				return "是";
			}
            if (type == EBoolean.False)
            {
                return "否";
            }
            throw new Exception();
        }

		public static EBoolean GetEnumType(string typeStr)
		{
			var retval = EBoolean.False;

			if (Equals(EBoolean.True, typeStr))
			{
				retval = EBoolean.True;
			}
			else if (Equals(EBoolean.False, typeStr))
			{
				retval = EBoolean.False;
			}

			return retval;
		}

		public static bool Equals(EBoolean type, string typeStr)
		{
			if (string.IsNullOrEmpty(typeStr)) return false;
			if (string.Equals(GetValue(type).ToLower(), typeStr.ToLower()))
			{
				return true;
			}
			return false;
		}

		public static bool Equals(string typeStr, EBoolean type)
		{
			return Equals(type, typeStr);
		}

		

	}
}
