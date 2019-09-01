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
			var retVal = EOraclePrivilege.Normal;

			if (Equals(EOraclePrivilege.Normal, typeStr))
			{
				retVal = EOraclePrivilege.Normal;
			}
			else if (Equals(EOraclePrivilege.SYSDBA, typeStr))
			{
				retVal = EOraclePrivilege.SYSDBA;
			}
			else if (Equals(EOraclePrivilege.SYSOPER, typeStr))
			{
				retVal = EOraclePrivilege.SYSOPER;
			}

			return retVal;
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
