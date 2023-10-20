using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Datory;
using SSCMS.Models;
using SSCMS.Repositories;
using SSCMS.Services;

namespace SSCMS.Core.Repositories
{
    public partial class ContentRepository : IContentRepository
    {
        private readonly ISettingsManager _settingsManager;
        private readonly IChannelRepository _channelRepository;
        private readonly ISiteRepository _siteRepository;
        private readonly IErrorLogRepository _errorLogRepository;
        private readonly IStatRepository _statRepository;
        private readonly IAdministratorRepository _administratorRepository;
        private readonly IUserRepository _userRepository;
        private readonly Repository<Content> _repository;

        public ContentRepository(ISettingsManager settingsManager, IChannelRepository channelRepository, ISiteRepository siteRepository, IErrorLogRepository errorLogRepository, IStatRepository statRepository, IAdministratorRepository administratorRepository, IUserRepository userRepository)
        {
            _settingsManager = settingsManager;
            _channelRepository = channelRepository;
            _siteRepository = siteRepository;
            _errorLogRepository = errorLogRepository;
            _statRepository = statRepository;
            _administratorRepository = administratorRepository;
            _userRepository = userRepository;

            _repository = new Repository<Content>(settingsManager.Database, settingsManager.Redis);
        }

        public IDatabase Database => _repository.Database;

        public string TableName => _repository.TableName;

        public List<TableColumn> TableColumns => _repository.TableColumns;

        public const string AttrExtendValues = "ExtendValues";

        public async Task<Repository<Content>> GetRepositoryAsync(string tableName)
        {
            if (TableNameRepositories.TryGetValue(tableName, out var repository))
            {
                return repository;
            }

            repository = new Repository<Content>(_settingsManager.Database, tableName, _settingsManager.Redis);
            await repository.LoadTableColumnsAsync(tableName);

            TableNameRepositories[tableName] = repository;
            return repository;
        }

        private string Quote(string identifier)
        {
            return Database.GetQuotedIdentifier(identifier);
        }
    }
}
