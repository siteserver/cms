using System.Collections.Generic;
using Datory;
using SSCMS;

namespace SSCMS.Core.Repositories.ContentRepository
{
    public partial class ContentRepository : IContentRepository
    {
        private readonly ISettingsManager _settingsManager;
        private readonly Repository<Content> _repository;
        private readonly IAdministratorRepository _administratorRepository;
        private readonly IChannelRepository _channelRepository;
        private readonly ISiteRepository _siteRepository;
        private readonly IErrorLogRepository _errorLogRepository;

        public ContentRepository(ISettingsManager settingsManager, IAdministratorRepository administratorRepository, IChannelRepository channelRepository, ISiteRepository siteRepository, IErrorLogRepository errorLogRepository)
        {
            _settingsManager = settingsManager;
            _repository = new Repository<Content>(settingsManager.Database, settingsManager.Redis);
            _administratorRepository = administratorRepository;
            _channelRepository = channelRepository;
            _siteRepository = siteRepository;
            _errorLogRepository = errorLogRepository;
        }

        public IDatabase Database => _repository.Database;

        public string TableName => _repository.TableName;

        public List<TableColumn> TableColumns => _repository.TableColumns;
    }
}
