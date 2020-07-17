using System.Collections.Generic;
using Datory;
using SSCMS.Models;
using SSCMS.Repositories;
using SSCMS.Services;

namespace SSCMS.Core.Repositories
{
    public partial class ContentRepository : IContentRepository
    {
        private readonly ISettingsManager _settingsManager;
        private readonly IAdministratorRepository _administratorRepository;
        private readonly IChannelRepository _channelRepository;
        private readonly ISiteRepository _siteRepository;
        private readonly IErrorLogRepository _errorLogRepository;
        private readonly IStatRepository _statRepository;
        private readonly Repository<Content> _repository;

        public ContentRepository(ISettingsManager settingsManager, IAdministratorRepository administratorRepository, IChannelRepository channelRepository, ISiteRepository siteRepository, IErrorLogRepository errorLogRepository, IStatRepository statRepository)
        {
            _settingsManager = settingsManager;
            _administratorRepository = administratorRepository;
            _channelRepository = channelRepository;
            _siteRepository = siteRepository;
            _errorLogRepository = errorLogRepository;
            _statRepository = statRepository;
            _repository = new Repository<Content>(settingsManager.Database, settingsManager.Redis);
        }

        public IDatabase Database => _repository.Database;

        public string TableName => _repository.TableName;

        public List<TableColumn> TableColumns => _repository.TableColumns;
    }
}
