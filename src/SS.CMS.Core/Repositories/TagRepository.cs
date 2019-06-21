using System.Collections.Generic;
using System.Linq;
using System.Text;
using SqlKata;
using SS.CMS.Data;
using SS.CMS.Models;
using SS.CMS.Repositories;
using SS.CMS.Services;
using SS.CMS.Utils;

namespace SS.CMS.Core.Repositories
{
    public partial class TagRepository : ITagRepository
    {
        private readonly ICacheManager _cacheManager;
        private readonly Repository<TagInfo> _repository;

        public TagRepository(ISettingsManager settingsManager, ICacheManager cacheManager)
        {
            _repository = new Repository<TagInfo>(new Database(settingsManager.DatabaseType, settingsManager.DatabaseConnectionString));
            _cacheManager = cacheManager;
        }

        public IDatabase Database => _repository.Database;
        public string TableName => _repository.TableName;
        public List<TableColumn> TableColumns => _repository.TableColumns;

        private static class Attr
        {
            public const string SiteId = nameof(TagInfo.SiteId);
            public const string ContentIdCollection = nameof(TagInfo.ContentIdCollection);
            public const string Tag = nameof(TagInfo.Tag);
            public const string UseNum = nameof(TagInfo.UseNum);
        }

        public void Insert(TagInfo tagInfo)
        {
            _repository.Insert(tagInfo);
        }

        public void Update(TagInfo tagInfo)
        {
            _repository.Update(tagInfo);
        }

        public TagInfo GetTagInfo(int siteId, string tag)
        {
            return _repository.Get(Q.Where(Attr.SiteId, siteId).Where(Attr.Tag, tag));
        }

        public IList<TagInfo> GetTagInfoList(int siteId, int contentId)
        {
            var query = GetQuery(null, siteId, contentId);
            return _repository.GetAll(query).ToList();
        }

        // public string GetSqlString(int siteId, int contentId, bool isOrderByCount, int totalNum)
        // {
        //     var whereString = GetWhereString(null, siteId, contentId);
        //     var orderString = string.Empty;
        //     if (isOrderByCount)
        //     {
        //         orderString = "ORDER BY UseNum DESC";
        //     }

        //     return DatabaseUtils.GetPageSqlString("siteserver_Tag", string.Join(",", new string[]
        //     {
        //         nameof(TagInfo.Id),
        //         nameof(TagInfo.SiteId),
        //         nameof(TagInfo.ContentIdCollection),
        //         nameof(TagInfo.Tag),
        //         nameof(TagInfo.UseNum)
        //     }), whereString, orderString, 0, totalNum);
        // }

        private IList<TagInfo> GetTagInfoListToCache(int siteId, int contentId, bool isOrderByCount, int totalNum)
        {
            var query = GetQuery(null, siteId, contentId).Limit(totalNum);
            if (isOrderByCount)
            {
                query.OrderByDesc(Attr.UseNum);
            }

            return _repository.GetAll(query).ToList();
        }

        public IList<string> GetTagListByStartString(int siteId, string startString, int totalNum)
        {
            return _repository.GetAll<string>(Q
                .Select(Attr.Tag)
                .Where(Attr.SiteId, siteId)
                .WhereContains(Attr.Tag, startString)
                .OrderByDesc(Attr.UseNum)
                .Distinct()
                .Limit(totalNum)).ToList();
        }

        public IList<string> GetTagList(int siteId)
        {
            return _repository.GetAll<string>(Q
                .Select(Attr.Tag)
                .Where(Attr.SiteId, siteId)
                .Distinct()
                .OrderByDesc(Attr.UseNum)).ToList();
        }

        public void DeleteTags(int siteId)
        {
            var query = GetQuery(null, siteId, 0);
            _repository.Delete(query);
        }

        public void DeleteTag(string tag, int siteId)
        {
            var query = GetQuery(tag, siteId, 0);
            _repository.Delete(query);
        }

        public int GetTagCount(string tag, int siteId)
        {
            var contentIdList = GetContentIdListByTag(tag, siteId);
            return contentIdList.Count;
        }

        private string GetWhereString(string tag, int siteId, int contentId)
        {
            var builder = new StringBuilder();
            builder.Append($" WHERE SiteId = {siteId} ");
            if (!string.IsNullOrEmpty(tag))
            {
                builder.Append($"AND Tag = '{AttackUtils.FilterSql(tag)}' ");
            }
            if (contentId > 0)
            {
                builder.Append(
                    $"AND (ContentIdCollection = '{contentId}' OR ContentIdCollection LIKE '{contentId},%' OR ContentIdCollection LIKE '%,{contentId},%' OR ContentIdCollection LIKE '%,{contentId}')");
            }

            return builder.ToString();
        }

        private Query GetQuery(string tag, int siteId, int contentId)
        {
            var query = Q.Where(Attr.SiteId, siteId);
            if (!string.IsNullOrEmpty(tag))
            {
                query.Where(Attr.Tag, tag);
            }
            if (contentId > 0)
            {
                query.Where(q => q
                    .Where(Attr.ContentIdCollection, contentId.ToString())
                    .OrWhereStarts(Attr.ContentIdCollection, $"{contentId},")
                    .OrWhereContains(Attr.ContentIdCollection, $",{contentId},")
                    .OrWhereEnds(Attr.ContentIdCollection, $",{contentId}"));
            }

            return query;
        }

        public List<int> GetContentIdListByTag(string tag, int siteId)
        {
            var idList = new List<int>();
            if (string.IsNullOrEmpty(tag)) return idList;

            var query = GetQuery(tag, siteId, 0);
            var contentIdCollectionList = _repository.GetAll<string>(query.Select(Attr.ContentIdCollection));
            foreach (var contentIdCollection in contentIdCollectionList)
            {
                var contentIdList = TranslateUtils.StringCollectionToIntList(contentIdCollection);
                foreach (var contentId in contentIdList)
                {
                    if (contentId > 0 && !idList.Contains(contentId))
                    {
                        idList.Add(contentId);
                    }
                }
            }

            return idList;
        }

        private IList<int> GetContentIdListByTagCollectionToCache(List<string> tagCollection, int siteId)
        {
            var contentIdList = new List<int>();
            if (tagCollection.Count <= 0) return contentIdList;

            var contentIdCollectionList = _repository.GetAll<string>(Q
                .Select(Attr.ContentIdCollection)
                .Where(Attr.SiteId, siteId)
                .WhereIn(Attr.Tag, tagCollection));

            foreach (var contentIdCollection in contentIdCollectionList)
            {
                var list = TranslateUtils.StringCollectionToIntList(contentIdCollection);
                foreach (var contentId in list)
                {
                    if (contentId > 0 && !contentIdList.Contains(contentId))
                    {
                        contentIdList.Add(contentId);
                    }
                }
            }

            return contentIdList;
        }
    }
}
