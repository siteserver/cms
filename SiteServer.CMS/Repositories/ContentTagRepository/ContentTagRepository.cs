using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Datory;
using SiteServer.Abstractions;
using SqlKata;

namespace SiteServer.CMS.Repositories
{
    public class ContentTagRepository : DataProviderBase, IRepository
    {
        private readonly Repository<ContentTag> _repository;

        public ContentTagRepository()
        {
            _repository = new Repository<ContentTag>(new Database(WebConfigUtils.DatabaseType, WebConfigUtils.ConnectionString));
        }

        public IDatabase Database => _repository.Database;

        public string TableName => _repository.TableName;

        public List<TableColumn> TableColumns => _repository.TableColumns;

        public async Task InsertAsync(ContentTag tag)
        {
            await _repository.InsertAsync(tag);
        }

        public async Task UpdateAsync(ContentTag tag)
        {
            await _repository.UpdateAsync(tag); ;
        }

        public async Task<ContentTag> GetTagAsync(int siteId, string tag)
        {
            var tagEntity = await _repository.GetAsync(Q
                .Where(nameof(ContentTag.SiteId), siteId)
                .Where(nameof(ContentTag.Tag), tag)
            );
            return tagEntity;
        }

        public async Task<List<ContentTag>> GetTagListAsync(int siteId, int contentId)
        {
            var query = GetQuery(null, siteId, contentId);
            var list = await _repository.GetAllAsync(query);
            return list.ToList();
        }

        public string GetSqlString(int siteId, int contentId, bool isOrderByCount, int totalNum)
        {
            var orderString = string.Empty;
            if (isOrderByCount)
            {
                orderString = "ORDER BY UseNum DESC";
            }

            return SqlUtils.ToTopSqlString("siteserver_Tag", "Id, SiteId, ContentIdCollection, Tag, UseNum", string.Empty, orderString, totalNum);
        }

        public async Task<List<ContentTag>> GetTagListAsync(int siteId, int contentId, bool isOrderByCount, int totalNum)
        {
            var query = GetQuery(null, siteId, contentId);
            if (isOrderByCount)
            {
                query.OrderByDesc(nameof(ContentTag.UseNum));
            }

            query.Limit(totalNum);

            var list = await _repository.GetAllAsync(query);
            return list.ToList();
        }

        public async Task<List<string>> GetTagListByStartStringAsync(int siteId, string startString, int totalNum)
        {
            var list = await _repository.GetAllAsync<string>(Q
                .Select(nameof(ContentTag.Tag))
                .Where(nameof(ContentTag.SiteId), siteId)
                .WhereContains(nameof(ContentTag.Tag), startString)
                .OrderByDesc(nameof(ContentTag.UseNum))
                .Distinct()
                .Limit(totalNum));
            return list.ToList();
        }

        public async Task<IEnumerable<string>> GetTagListAsync(int siteId)
        {
            return await _repository.GetAllAsync<string>(Q
                .Where(nameof(ContentTag.SiteId), siteId)
                .OrderByDesc(nameof(ContentTag.UseNum))
                .Distinct()
            );
        }

        public async Task DeleteTagsAsync(int siteId)
        {
            await _repository.DeleteAsync(Q.Where(nameof(ContentTag.SiteId), siteId));
        }

        public async Task DeleteTagAsync(string tag, int siteId)
        {
            var query = GetQuery(tag, siteId, 0);
            await _repository.DeleteAsync(query);
        }

        public async Task<int> GetTagCountAsync(string tag, int siteId)
        {
            var query = GetQuery(tag, siteId, 0);
            return await _repository.CountAsync(query);
        }

        private Query GetQuery(string tag, int siteId, int contentId)
        {
            var query = Q.Where(nameof(ContentTag.SiteId), siteId);
            if (!string.IsNullOrEmpty(tag))
            {
                query.Where(nameof(ContentTag.Tag), tag);
            }
            if (contentId > 0)
            {
                query.Where(q => q
                    .Where(nameof(ContentTag.ContentIdCollection), contentId)
                    .OrWhereLike(nameof(ContentTag.ContentIdCollection), $"{contentId},%")
                    .OrWhereLike(nameof(ContentTag.ContentIdCollection), $"%,{contentId},%")
                    .OrWhereLike(nameof(ContentTag.ContentIdCollection), $"%,{contentId}")
                );
            }

            query.OrderBy(nameof(ContentTag.Tag));

            return query;
        }

        public async Task<List<int>> GetContentIdListByTagAsync(string tag, int siteId)
        {
            var idList = new List<int>();
            if (string.IsNullOrEmpty(tag)) return idList;

            var query = GetQuery(tag, siteId, 0);
            query.Select(nameof(ContentTag.ContentIdCollection));

            var list = await _repository.GetAllAsync<string>(query);
            foreach (var contentIdCollection in list)
            {
                var contentIdList = StringUtils.GetIntList(contentIdCollection);
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

        public async Task<List<int>> GetContentIdListByTagCollectionAsync(List<string> tagList, int siteId)
        {
            var contentIdList = new List<int>();
            if (tagList.Count > 0)
            {
                var query = Q.Where(nameof(ContentTag.SiteId), siteId);
                query.WhereIn(nameof(ContentTag.Tag), tagList);
                query.Select(nameof(ContentTag.ContentIdCollection));

                var allList = await _repository.GetAllAsync<string>(query);
                foreach (var contentIdCollection in allList)
                {
                    var list = StringUtils.GetIntList(contentIdCollection);
                    foreach (var contentId in list)
                    {
                        if (contentId > 0 && !contentIdList.Contains(contentId))
                        {
                            contentIdList.Add(contentId);
                        }
                    }
                }
            }
            return contentIdList;
        }
	}
}
