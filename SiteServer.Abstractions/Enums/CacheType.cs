using System;


namespace SiteServer.Abstractions
{
    public sealed class CacheType
    {
        public static readonly CacheType Memory = new CacheType("Memory");
        public static readonly CacheType Redis = new CacheType("Redis");
        public static readonly CacheType SqlServer = new CacheType("SqlServer");

        private CacheType(string value)
        {
            Value = value;
        }

        public string Value { get; private set; }

        public static CacheType Parse(string val)
        {
            if (string.Equals(Memory.Value, val, StringComparison.OrdinalIgnoreCase))
            {
                return Memory;
            }
            if (string.Equals(Redis.Value, val, StringComparison.OrdinalIgnoreCase))
            {
                return Redis;
            }
            if (string.Equals(SqlServer.Value, val, StringComparison.OrdinalIgnoreCase))
            {
                return SqlServer;
            }
            return Memory;
        }
    }
}
