using System.Collections.Generic;
using System.Threading.Tasks;
using Datory;
using SS.CMS.Abstractions;

namespace SS.CMS.Repositories
{
	public class ContentCheckRepository : IContentCheckRepository
    {
        private readonly Repository<ContentCheck> _repository;

        public ContentCheckRepository(ISettingsManager settingsManager)
        {
            _repository = new Repository<ContentCheck>(settingsManager.Database, settingsManager.Redis);
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