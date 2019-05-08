using Datory;

namespace SiteServer.Utils
{
    public class DatabaseTypeUtils
    {
        public static DatabaseType GetEnumType(string typeStr)
        {
            var retVal = DatabaseType.SqlServer;

            if (Equals(DatabaseType.MySql, typeStr))
            {
                retVal = DatabaseType.MySql;
            }
            else if (Equals(DatabaseType.SqlServer, typeStr))
            {
                retVal = DatabaseType.SqlServer;
            }
            else if (Equals(DatabaseType.PostgreSql, typeStr))
            {
                retVal = DatabaseType.PostgreSql;
            }
            else if (Equals(DatabaseType.Oracle, typeStr))
            {
                retVal = DatabaseType.Oracle;
            }

            return retVal;
        }

        public static bool Equals(DatabaseType type, string typeStr)
        {
            if (string.IsNullOrEmpty(typeStr)) return false;
            if (string.Equals(type.Value.ToLower(), typeStr.ToLower()))
            {
                return true;
            }
            return false;
        }

        public static bool Equals(string typeStr, DatabaseType type)
        {
            return Equals(type, typeStr);
        }

        
    }
}
