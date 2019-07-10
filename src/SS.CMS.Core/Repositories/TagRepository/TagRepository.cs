using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
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
        private readonly IDistributedCache _cache;
        private readonly Repository<Tag> _repository;

        public TagRepository(IDistributedCache cache, ISettingsManager settingsManager)
        {
            _cache = cache;
            _repository = new Repository<Tag>(new Database(settingsManager.DatabaseType, settingsManager.DatabaseConnectionString));
        }

        public IDatabase Database => _repository.Database;
        public string TableName => _repository.TableName;
        public List<TableColumn> TableColumns => _repository.TableColumns;

        private static class Attr
        {
            public const string SiteId = nameof(CMS.Models.Tag.SiteId);
            public const string ContentIdCollection = nameof(CMS.Models.Tag.ContentIdCollection);
            public const string Tag = nameof(CMS.Models.Tag.Value);
            public const string UseNum = nameof(CMS.Models.Tag.UseNum);
        }

        public async Task<int> InsertAsync(Tag tagInfo)
        {
            return await _repository.InsertAsync(tagInfo);
        }

        public async Task<bool> UpdateAsync(Tag tagInfo)
        {
            return await _repository.UpdateAsync(tagInfo);
        }

        public async Task<Tag> GetTagInfoAsync(int siteId, string tag)
        {
            return await _repository.GetAsync(Q.Where(Attr.SiteId, siteId).Where(Attr.Tag, tag));
        }

        public async Task<IEnumerable<Tag>> GetTagInfoListAsync(int siteId, int contentId)
        {
            var query = GetQuery(null, siteId, contentId);
            return await _repository.GetAllAsync(query);
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

        private async Task<IEnumerable<Tag>> GetTagInfoListToCacheAsync(int siteId, int contentId, bool isOrderByCount, int totalNum)
        {
            var query = GetQuery(null, siteId, contentId).Limit(totalNum);
            if (isOrderByCount)
            {
                query.OrderByDesc(Attr.UseNum);
            }

            return await _repository.GetAllAsync(query);
        }

        public async Task<IEnumerable<string>> GetTagListByStartStringAsync(int siteId, string startString, int totalNum)
        {
            return await _repository.GetAllAsync<string>(Q
                .Select(Attr.Tag)
                .Where(Attr.SiteId, siteId)
                .WhereContains(Attr.Tag, startString)
                .OrderByDesc(Attr.UseNum)
                .Distinct()
                .Limit(totalNum));
        }

        public async Task<IEnumerable<string>> GetTagListAsync(int siteId)
        {
            return await _repository.GetAllAsync<string>(Q
                .Select(Attr.Tag)
                .Where(Attr.SiteId, siteId)
                .Distinct()
                .OrderByDesc(Attr.UseNum));
        }

        public async Task DeleteTagsAsync(int siteId)
        {
            var query = GetQuery(null, siteId, 0);
            await _repository.DeleteAsync(query);
        }

        public async Task DeleteTagAsync(string tag, int siteId)
        {
            var query = GetQuery(tag, siteId, 0);
            await _repository.DeleteAsync(query);
        }

        public async Task<int> GetTagCountAsync(string tag, int siteId)
        {
            var contentIdList = await GetContentIdListByTagAsync(tag, siteId);
            return contentIdList.Count();
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

        public async Task<IEnumerable<int>> GetContentIdListByTagAsync(string tag, int siteId)
        {
            var idList = new List<int>();
            if (string.IsNullOrEmpty(tag)) return idList;

            var query = GetQuery(tag, siteId, 0);
            var contentIdCollectionList = await _repository.GetAllAsync<string>(query.Select(Attr.ContentIdCollection));
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

        private async Task<IList<int>> GetContentIdListByTagCollectionToCacheAsync(List<string> tagCollection, int siteId)
        {
            var contentIdList = new List<int>();
            if (tagCollection.Count <= 0) return contentIdList;

            var contentIdCollectionList = await _repository.GetAllAsync<string>(Q
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
