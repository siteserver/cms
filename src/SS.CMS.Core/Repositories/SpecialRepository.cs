using System.Collections.Generic;
using System.Linq;
using SS.CMS.Data;
using SS.CMS.Models;
using SS.CMS.Repositories;
using SS.CMS.Services;
using SS.CMS.Utils;

namespace SS.CMS.Core.Repositories
{
    public partial class SpecialRepository : ISpecialRepository
    {
        private static readonly string CacheKey = StringUtils.GetCacheKey(nameof(SpecialRepository));
        private readonly Repository<SpecialInfo> _repository;
        private readonly ISettingsManager _settingsManager;
        private readonly ICacheManager _cacheManager;
        public SpecialRepository(ISettingsManager settingsManager, ICacheManager cacheManager)
        {
            _repository = new Repository<SpecialInfo>(new Database(settingsManager.DatabaseType, settingsManager.DatabaseConnectionString));
            _settingsManager = settingsManager;
            _cacheManager = cacheManager;
        }

        public IDatabase Database => _repository.Database;

        public string TableName => _repository.TableName;
        public List<TableColumn> TableColumns => _repository.TableColumns;

        private static class Attr
        {
            public const string Id = nameof(SpecialInfo.Id);
            public const string SiteId = nameof(SpecialInfo.SiteId);
            public const string Title = nameof(SpecialInfo.Title);
            public const string Url = nameof(SpecialInfo.Url);
        }

        public int Insert(SpecialInfo specialInfo)
        {
            specialInfo.Id = _repository.Insert(specialInfo);

            RemoveCache(specialInfo.SiteId);

            return specialInfo.Id;
        }

        public bool Update(SpecialInfo specialInfo)
        {
            var updated = _repository.Update(specialInfo);

            RemoveCache(specialInfo.SiteId);

            return updated;
        }

        public SpecialInfo Delete(int siteId, int specialId)
        {
            if (specialId <= 0) return null;

            var specialInfo = GetSpecialInfo(siteId, specialId);

            _repository.Delete(specialId);

            RemoveCache(siteId);

            return specialInfo;
        }

        public bool IsTitleExists(int siteId, string title)
        {
            return _repository.Exists(Q.Where(Attr.SiteId, siteId).Where(Attr.Title, title));
        }

        public bool IsUrlExists(int siteId, string url)
        {
            return _repository.Exists(Q.Where(Attr.SiteId, siteId).Where(Attr.Url, url));
        }

        public IList<SpecialInfo> GetSpecialInfoList(int siteId)
        {
            return _repository.GetAll(Q.Where(Attr.SiteId, siteId).OrderByDesc(Attr.Id)).ToList();
        }

        public IList<SpecialInfo> GetSpecialInfoList(int siteId, string keyword)
        {
            return _repository.GetAll(Q
                .Where(Attr.SiteId, siteId)
                .OrWhereContains(Attr.Title, keyword)
                .OrWhereContains(Attr.Url, keyword)
                .OrderByDesc(Attr.Id)).ToList();
        }

        private Dictionary<int, SpecialInfo> GetSpecialInfoDictionaryBySiteIdToCache(int siteId)
        {
            var specialInfoList = GetSpecialInfoList(siteId);

            return specialInfoList.ToDictionary(specialInfo => specialInfo.Id);
        }
    }
}