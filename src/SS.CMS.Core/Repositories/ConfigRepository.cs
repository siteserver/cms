using System.Collections.Generic;
using SS.CMS.Core.Common;
using SS.CMS.Data;
using SS.CMS.Models;
using SS.CMS.Repositories;
using SS.CMS.Services.ICacheManager;
using SS.CMS.Services.ISettingsManager;
using SS.CMS.Utils;

namespace SS.CMS.Core.Repositories
{
    public class ConfigRepository : IConfigRepository
    {
        private readonly ISettingsManager _settingsManager;
        private readonly ICacheManager _cacheManager;
        private readonly Repository<ConfigInfo> _repository;
        private readonly string CacheKey = StringUtils.GetCacheKey(nameof(ConfigRepository));

        public ConfigRepository(ISettingsManager settingsManager, ICacheManager cacheManager)
        {
            _repository = new Repository<ConfigInfo>(new Db(settingsManager.DatabaseType, settingsManager.DatabaseConnectionString));
            _settingsManager = settingsManager;
            _cacheManager = cacheManager;
        }

        public IDb Db => _repository.Db;
        public string TableName => _repository.TableName;
        public List<TableColumn> TableColumns => _repository.TableColumns;

        private static class Attr
        {
            public const string Id = nameof(ConfigInfo.Id);
            public const string IsInitialized = "IsInitialized";
            public const string DatabaseVersion = nameof(ConfigInfo.DatabaseVersion);
            public const string UpdateDate = nameof(ConfigInfo.UpdateDate);
            public const string SystemConfig = "SystemConfig";
        }

        public int Insert(ConfigInfo configInfo)
        {
            configInfo.Id = _repository.Insert(configInfo);
            if (configInfo.Id > 0)
            {
                IsChanged = true;
            }

            return configInfo.Id;
        }

        public bool Update(ConfigInfo configInfo)
        {
            var updated = _repository.Update(configInfo);
            if (updated)
            {
                IsChanged = true;
            }

            return updated;
        }

        public bool IsInitialized()
        {
            try
            {
                var isInitialized = _repository.Get<string>(Q
                    .Select(Attr.IsInitialized)
                    .OrderBy(Attr.Id));

                return TranslateUtils.ToBool(isInitialized);
            }
            catch
            {
                // ignored
            }

            return false;
        }

        public ConfigInfo GetConfigInfo()
        {
            ConfigInfo info = null;

            try
            {
                info = _repository.Get(Q.OrderBy(Attr.Id));
            }
            catch
            {
                info = _repository.Get(Q.OrderBy(Attr.Id));
            }

            return info;
        }

        public ConfigInfo Instance
        {
            get
            {
                var retVal = _cacheManager.Get<ConfigInfo>(CacheKey);
                if (retVal != null) return retVal;

                retVal = _cacheManager.Get<ConfigInfo>(CacheKey);
                if (retVal == null)
                {
                    retVal = GetConfigInfo();
                    _cacheManager.Insert(CacheKey, retVal);
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
                    _cacheManager.Remove(CacheKey);
                }
            }
        }
    }
}