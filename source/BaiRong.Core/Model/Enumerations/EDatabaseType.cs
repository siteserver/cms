using System;

namespace BaiRong.Core.Model.Enumerations
{
	public enum EDatabaseType
	{
        MySql,
        SqlServer
	}

	public class EDatabaseTypeUtils
	{
		public static string GetValue(EDatabaseType type)
		{
		    if (type == EDatabaseType.MySql)
			{
				return "MySql";
			}
		    if (type == EDatabaseType.SqlServer)
		    {
		        return "SqlServer";
		    }
		    throw new Exception();
		}

		public static EDatabaseType GetEnumType(string typeStr)
		{
			var retval = EDatabaseType.MySql;

			if (Equals(EDatabaseType.MySql, typeStr))
			{
				retval = EDatabaseType.MySql;
			}
			else if (Equals(EDatabaseType.SqlServer, typeStr))
			{
				retval = EDatabaseType.SqlServer;
			}

			return retval;
		}

		public static bool Equals(EDatabaseType type, string typeStr)
		{
			if (string.IsNullOrEmpty(typeStr)) return false;
			if (string.Equals(GetValue(type).ToLower(), typeStr.ToLower()))
			{
				return true;
			}
			return false;
		}

        public static bool Equals(string typeStr, EDatabaseType type)
        {
            return Equals(type, typeStr);
        }
	}
}
