using System.Collections.Generic;
using System.Threading.Tasks;
using Datory;
using SiteServer.Abstractions;

namespace SiteServer.CMS.Repositories
{
    public partial class ContentTagRepository : IRepository
    {
        private readonly Repository<ContentTag> _repository;

        public ContentTagRepository()
        {
            _repository = new Repository<ContentTag>(new Database(WebConfigUtils.DatabaseType, WebConfigUtils.ConnectionString), new Redis(WebConfigUtils.RedisConnectionString));
        }

        public IDatabase Database => _repository.Database;

        public string TableName => _repository.TableName;

        public List<TableColumn> TableColumns => _repository.TableColumns;

        public async Task InsertAsync(int siteId, string tagName)
        {
            var tagNames = await GetTagNamesAsync(siteId);
            if (!tagNames.Contains(tagName))
            {
                await _repository.InsertAsync(new ContentTag
                    {
                        SiteId = siteId,
                        TagName = tagName
                    }, Q.CachingRemove(GetCacheKey(siteId))
                );
            }
        }

        public async Task UpdateAsync(int siteId, string oldTagName, string newTagName)
        {
            if (oldTagName != newTagName)
            {
                await DeleteAsync(siteId, oldTagName);
                await InsertAsync(siteId, newTagName);
            }
        }

        public async Task DeleteAsync(int siteId, string tagName)
        {
            await _repository.DeleteAsync(Q
                .Where(nameof(ContentTag.SiteId), siteId)
                .Where(nameof(ContentTag.TagName), tagName)
                .CachingRemove(GetCacheKey(siteId))
            );
        }

        public async Task DeleteAsync(int siteId)
        {
            await _repository.DeleteAsync(Q
                .Where(nameof(ContentTag.SiteId), siteId)
                .CachingRemove(GetCacheKey(siteId))
            );
        }
    }
}