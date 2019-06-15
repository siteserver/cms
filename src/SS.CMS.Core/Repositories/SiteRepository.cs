using System.Collections.Generic;
using System.Linq;
using Dapper;
using SS.CMS.Abstractions.Enums;
using SS.CMS.Abstractions.Models;
using SS.CMS.Abstractions.Repositories;
using SS.CMS.Abstractions.Services;
using SS.CMS.Core.Cache;
using SS.CMS.Core.Common;
using SS.CMS.Core.Security;
using SS.CMS.Core.StlParser.Models;
using SS.CMS.Data;

namespace SS.CMS.Core.Repositories
{
    public partial class SiteRepository : ISiteRepository
    {
        private readonly Repository<SiteInfo> _repository;
        private readonly ISettingsManager _settingsManager;
        public SiteRepository(ISettingsManager settingsManager)
        {
            _repository = new Repository<SiteInfo>(new Db(settingsManager.DatabaseType, settingsManager.DatabaseConnectionString));
            _settingsManager = settingsManager;
        }

        public IDb Db => _repository.Db;
        public string TableName => _repository.TableName;
        public List<TableColumn> TableColumns => _repository.TableColumns;

        private static class Attr
        {
            public const string Id = nameof(SiteInfo.Id);
            public const string SiteDir = nameof(SiteInfo.SiteDir);
            public const string TableName = nameof(SiteInfo.TableName);
            public const string IsRoot = "IsRoot";
            public const string ParentId = nameof(SiteInfo.ParentId);
            public const string Taxis = nameof(SiteInfo.Taxis);
        }

        public int Insert(SiteInfo siteInfo)
        {
            siteInfo.Taxis = GetMaxTaxis() + 1;
            siteInfo.Id = _repository.Insert(siteInfo);

            ClearCache();

            return siteInfo.Id;
        }

        public bool Delete(int siteId)
        {
            var siteInfo = GetSiteInfo(siteId);
            var list = ChannelManager.GetChannelIdList(siteId);
            DataProvider.TableStyleRepository.Delete(list, siteInfo.TableName);

            DataProvider.TagRepository.DeleteTags(siteId);

            DataProvider.ChannelRepository.DeleteAll(siteId);

            UpdateParentIdToZero(siteId);

            _repository.Delete(siteId);

            ClearCache();
            ChannelManager.RemoveCacheBySiteId(siteId);
            Permissions.ClearAllCache();

            return true;
        }

        public bool Update(SiteInfo siteInfo)
        {
            if (siteInfo.Root)
            {
                UpdateAllIsRoot();
            }

            var updated = _repository.Update(siteInfo);

            ClearCache();

            return updated;
        }

        public void UpdateTableName(int siteId, string tableName)
        {
            _repository.Update(Q
                .Set(Attr.TableName, tableName)
                .Where(Attr.Id, siteId)
            );

            ClearCache();
        }

        public void UpdateParentIdToZero(int parentId)
        {
            _repository.Update(Q
                .Set(Attr.ParentId, 0)
                .Where(Attr.ParentId, parentId)
            );

            ClearCache();
        }

        public IList<string> GetLowerSiteDirListThatNotIsRoot()
        {
            var list = _repository.GetAll<string>(Q
                .Select(Attr.SiteDir)
                .WhereNot(Attr.IsRoot, true.ToString()));

            return list.Select(x => x.ToLower()).ToList();
        }

        private void UpdateAllIsRoot()
        {
            _repository.Update(Q
                .Set(Attr.IsRoot, false.ToString())
            );

            ClearCache();
        }

        private List<KeyValuePair<int, SiteInfo>> GetSiteInfoKeyValuePairListToCache()
        {
            var list = new List<KeyValuePair<int, SiteInfo>>();

            var siteInfoList = GetSiteInfoListToCache();
            foreach (var siteInfo in siteInfoList)
            {
                var entry = new KeyValuePair<int, SiteInfo>(siteInfo.Id, siteInfo);
                list.Add(entry);
            }

            return list;
        }

        private IList<SiteInfo> GetSiteInfoListToCache()
        {
            return _repository.GetAll(Q.OrderBy(Attr.Taxis, Attr.Id)).ToList();
        }

        public int GetIdByIsRoot()
        {
            return _repository.Get<int>(Q
                .Select(Attr.Id)
                .Where(Attr.IsRoot, true.ToString()));
        }

        public int GetIdBySiteDir(string siteDir)
        {
            return _repository.Get<int>(Q
                .Select(Attr.Id)
                .Where(Attr.SiteDir, siteDir));
        }

        /// <summary>
        /// 得到所有系统文件夹的列表，以小写表示。
        /// </summary>
        public List<string> GetLowerSiteDirList(int parentId)
        {
            return _repository.GetAll<string>(Q
                    .Select(Attr.SiteDir)
                    .Where(Attr.ParentId, parentId))
                .Select(x => x.ToLower())
                .ToList();
        }

        public List<KeyValuePair<int, SiteInfo>> GetContainerSiteList(string siteName, string siteDir, int startNum, int totalNum, ScopeType scopeType, string orderByString)
        {
            var query = Q.NewQuery();

            SiteInfo siteInfo = null;
            if (!string.IsNullOrEmpty(siteName))
            {
                siteInfo = GetSiteInfoBySiteName(siteName);
            }
            else if (!string.IsNullOrEmpty(siteDir))
            {
                siteInfo = GetSiteInfoByDirectory(siteDir);
            }

            if (siteInfo != null)
            {
                query.Where(Attr.ParentId, siteInfo.Id);
            }
            else
            {
                if (scopeType == ScopeType.Children)
                {
                    query.Where(Attr.ParentId, 0).Where(Attr.IsRoot, false);
                }
                else if (scopeType == ScopeType.Descendant)
                {
                    query.Where(Attr.IsRoot, false);
                }
            }

            query.OrderByDesc(Attr.IsRoot).OrderBy(Attr.ParentId).OrderByDesc(Attr.Taxis).OrderBy(Attr.Id);

            query.Offset(startNum - 1).Limit(totalNum);

            var list = new List<KeyValuePair<int, SiteInfo>>();
            var itemIndex = 0;
            var minSiteInfoList = _repository.GetAll(query);

            foreach (var minSiteInfo in minSiteInfoList)
            {
                list.Add(new KeyValuePair<int, SiteInfo>(itemIndex++, minSiteInfo));
            }

            return list;
        }

        private int GetMaxTaxis()
        {
            return _repository.Max(Attr.Taxis) ?? 0;
        }

        public int GetTableCount(string tableName)
        {
            return _repository.Count(Q.Where(Attr.TableName, tableName));
        }
    }
}