using System;
using Datory;

namespace SSCMS.Services
{
    public interface ISettingsManager
    {
        string ContentRootPath { get; }
        string WebRootPath { get; }
        string Version { get; }
        string FrameworkDescription { get; }
        string OSDescription { get; }
        bool Containerized { get; }
        int CPUCores { get; }
        bool IsNightlyUpdate { get; }
        bool IsProtectData { get; }
        string SecurityKey { get; }
        string ApiHost { get; }
        DatabaseType DatabaseType { get; }
        string DatabaseConnectionString { get; }
        IDatabase Database { get; }
        string RedisConnectionString { get; }
        IRedis Redis { get; }
        string Encrypt(string inputString, string securityKey = null);
        string Decrypt(string inputString, string securityKey = null);
        void SaveSettings(bool isNightlyUpdate, bool isProtectData, DatabaseType databaseType, string databaseConnectionString, string redisConnectionString);
        IServiceProvider BuildServiceProvider();
    }
}
