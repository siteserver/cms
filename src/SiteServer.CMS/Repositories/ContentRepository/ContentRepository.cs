using System.Collections.Generic;
using Datory;
using SiteServer.Abstractions;

namespace SiteServer.CMS.Repositories
{
    public partial class ContentRepository : IContentRepository
    {
        private readonly Repository<Content> _repository;
        private readonly IAdministratorRepository _administratorRepository;
        private readonly IChannelRepository _channelRepository;
        private readonly ISiteRepository _siteRepository;

        public ContentRepository(ISettingsManager settingsManager, IAdministratorRepository administratorRepository, IChannelRepository channelRepository, ISiteRepository siteRepository)
        {
            _repository = new Repository<Content>(settingsManager.Database, settingsManager.Redis);
            _administratorRepository = administratorRepository;
            _channelRepository = channelRepository;
            _siteRepository = siteRepository;
        }

        public IDatabase Database => _repository.Database;

        public string TableName => _repository.TableName;

        public List<TableColumn> TableColumns => _repository.TableColumns;
    }
}
