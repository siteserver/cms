using System.Collections.Generic;
using SS.CMS.Data;
using SS.CMS.Models;
using SS.CMS.Repositories;
using SS.CMS.Services;
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
            public const string DatabaseVersion = nameof(ConfigInfo.DatabaseVersion);
            public const string UpdateDate = nameof(ConfigInfo.UpdateDate);
            public const string ExtendValues = nameof(ConfigInfo.ExtendValues);
        }

        public void Insert(ConfigInfo configInfo)
        {
            if (!_repository.Exists())
            {
                configInfo.Id = _repository.Insert(configInfo);
                if (configInfo.Id > 0)
                {
                    ClearCache();
                }
            }
        }

        public bool Update(ConfigInfo configInfo)
        {
            var updated = _repository.Update(configInfo);
            if (updated)
            {
                ClearCache();
            }

            return updated;
        }

        private ConfigInfo GetConfigInfo()
        {
            ConfigInfo info = null;

            try
            {
                info = _repository.Get(Q.OrderBy(Attr.Id));
            }
            catch
            {
                try
                {
                    info = _repository.Get(Q.OrderBy(Attr.Id));
                }
                catch
                {
                    // ignored
                }
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

        private void ClearCache()
        {
            _cacheManager.Remove(CacheKey);
        }
    }
}