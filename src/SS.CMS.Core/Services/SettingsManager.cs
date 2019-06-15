using Microsoft.Extensions.Configuration;
using SS.CMS.Abstractions.Models;
using SS.CMS.Abstractions.Services;
using SS.CMS.Core.Cache.Core;
using SS.CMS.Core.Repositories;
using SS.CMS.Data;
using SS.CMS.Utils;

namespace SS.CMS.Core.Services
{
    public class SettingsManager : ISettingsManager
    {
        private readonly string CacheKey = StringUtils.GetCacheKey(nameof(SettingsManager));
        private readonly object _lockObject = new object();

        public SettingsManager(IConfiguration config)
        {
            IsProtectData = config.GetValue<bool>("SS:IsProtectData");
            ApiPrefix = config.GetValue<string>("SS:ApiPrefix");
            AdminDirectory = config.GetValue<string>("SS:AdminDirectory");
            HomeDirectory = config.GetValue<string>("SS:HomeDirectory");
            SecretKey = config.GetValue<string>("SS:SecretKey");
            IsNightlyUpdate = config.GetValue<bool>("SS:IsNightlyUpdate");

            if (string.IsNullOrEmpty(SecretKey))
            {
                SecretKey = StringUtils.GetShortGuid();
            }

            if (IsProtectData)
            {
                DatabaseType = DatabaseType.GetDatabaseType(TranslateUtils.DecryptStringBySecretKey(config.GetValue<string>("SS:Database:Type"), SecretKey));
                DatabaseConnectionString = TranslateUtils.DecryptStringBySecretKey(config.GetValue<string>("SS:Database:ConnectionString"), SecretKey);
                RedisConnectionString = TranslateUtils.DecryptStringBySecretKey(config.GetValue<string>("SS:Redis:ConnectionString"), SecretKey);
            }
            else
            {
                DatabaseType = DatabaseType.GetDatabaseType(config.GetValue<string>("SS:Database:Type"));
                DatabaseConnectionString = config.GetValue<string>("SS:Database:ConnectionString");
                RedisConnectionString = config.GetValue<string>("SS:Redis:ConnectionString");
            }
        }

        public string ContentRootPath => ServiceCollectionExtensions.ContentRootPath;

        public string WebRootPath => ServiceCollectionExtensions.WebRootPath;

        public bool IsProtectData { get; }
        public string ApiPrefix { get; }
        public string AdminDirectory { get; }
        public string HomeDirectory { get; }
        public string SecretKey { get; }
        public bool IsNightlyUpdate { get; }
        public DatabaseType DatabaseType { get; }
        public string DatabaseConnectionString { get; }
        public string RedisConnectionString { get; }

        public ConfigInfo ConfigInfo
        {
            get
            {
                var retVal = DataCacheManager.Get<ConfigInfo>(CacheKey);
                if (retVal != null) return retVal;

                lock (_lockObject)
                {
                    retVal = DataCacheManager.Get<ConfigInfo>(CacheKey);
                    if (retVal == null)
                    {
                        var configRepository = new ConfigRepository(this);
                        retVal = configRepository.GetConfigInfo();
                        DataCacheManager.Insert(CacheKey, retVal);
                    }
                }

                return retVal;
            }
        }

        public bool IsChanged
        {
            set
            {
                if (value)
                {
                    DataCacheManager.Remove(CacheKey);
                }
            }
        }

        public string Encrypt(string inputString)
        {
            return TranslateUtils.EncryptStringBySecretKey(inputString, SecretKey);
        }

        public string Decrypt(string inputString)
        {
            return TranslateUtils.DecryptStringBySecretKey(inputString, SecretKey);
        }
    }
}
