namespace SiteServer.CMS.Core
{
    public static class Caching
    {
        public static string GetAllKey(string tableName)
        {
            return $"ss:{tableName}:all";
        }

        public static string GetAllKey(string tableName, int siteId)
        {
            return $"ss:{tableName}:all:{siteId}";
        }

        public static string GetAllKey(string tableName, string type, string identity)
        {
            return $"ss:{tableName}:all:{type}:{identity}";
        }

        public static string GetEntityKey(string tableName)
        {
            return $"ss:{tableName}:entity:only";
        }

        public static string GetEntityKey(string tableName, int id)
        {
            return $"ss:{tableName}:entity:{id}";
        }

        public static string GetEntityKey(string tableName, string type, string identity)
        {
            return $"ss:{tableName}:entity:{type}:{identity}";
        }

        public static string GetListKey(string tableName)
        {
            return $"ss:{tableName}:list";
        }

        public static string GetListKey(string tableName, int siteId)
        {
            return $"ss:{tableName}:list:{siteId}";
        }

        public static string GetListKey(string tableName, string type)
        {
            return $"ss:{tableName}:list:{type}";
        }

        public static string GetCountKey(string tableName, int siteId)
        {
            return $"ss:{tableName}:count:{siteId}";
        }

        public static string GetCountKey(string tableName, int siteId, int channelId)
        {
            return $"ss:{tableName}:count:{siteId}:{channelId}";
        }

        public static string GetCountKey(string tableName, int siteId, int channelId, int adminId)
        {
            return $"ss:{tableName}:count:{siteId}:{channelId}:{adminId}";
        }
    }
}
