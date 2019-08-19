using System;

namespace SiteServer.Utils.Enumerations
{
	public enum EOraclePrivilege
	{
		Normal,
		SYSDBA,
		SYSOPER
	}

	public class EOraclePrivilegeUtils
	{
		public static string GetValue(EOraclePrivilege type)
		{
		    if (type == EOraclePrivilege.Normal)
			{
				return "Normal";
			}
		    if (type == EOraclePrivilege.SYSDBA)
		    {
		        return "SYSDBA";
		    }
		    if (type == EOraclePrivilege.SYSOPER)
		    {
		        return "SYSOPER";
		    }
		    throw new Exception();
		}

		public static EOraclePrivilege GetEnumType(string typeStr)
		{
			var retval = EOraclePrivilege.Normal;

			if (Equals(EOraclePrivilege.Normal, typeStr))
			{
				retval = EOraclePrivilege.Normal;
			}
			else if (Equals(EOraclePrivilege.SYSDBA, typeStr))
			{
				retval = EOraclePrivilege.SYSDBA;
			}
			else if (Equals(EOraclePrivilege.SYSOPER, typeStr))
			{
				retval = EOraclePrivilege.SYSOPER;
			}

			return retval;
		}

		public static bool Equals(EOraclePrivilege type, string typeStr)
		{
			if (string.IsNullOrEmpty(typeStr)) return false;
			if (string.Equals(GetValue(type).ToLower(), typeStr.ToLower()))
			{
				return true;
			}
			return false;
		}

        public static bool Equals(string typeStr, EOraclePrivilege type)
        {
            return Equals(type, typeStr);
        }
	}
}
