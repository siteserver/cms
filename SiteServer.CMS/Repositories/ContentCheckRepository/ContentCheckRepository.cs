using System.Collections.Generic;
using System.Threading.Tasks;
using Datory;
using SiteServer.Abstractions;

namespace SiteServer.CMS.Repositories
{
	public class ContentCheckRepository : IRepository
    {
        private readonly Repository<ContentCheck> _repository;

        public ContentCheckRepository()
        {
            _repository = new Repository<ContentCheck>(new Database(WebConfigUtils.DatabaseType, WebConfigUtils.ConnectionString));
        }

        public IDatabase Database => _repository.Database;

        public string TableName => _repository.TableName;

        public List<TableColumn> TableColumns => _repository.TableColumns;

        public async Task InsertAsync(ContentCheck check)
        {
            await _repository.InsertAsync(check);
        }

		public async Task<List<ContentCheck>> GetCheckListAsync(int siteId, int channelId, int contentId)
		{
            return await _repository.GetAllAsync(Q
                .Where(nameof(ContentCheck.SiteId), siteId)
                .Where(nameof(ContentCheck.ChannelId), channelId)
                .Where(nameof(ContentCheck.ContentId), contentId)
                .OrderByDesc(nameof(ContentCheck.Id))
            );
		}
	}
}