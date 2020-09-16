using System.Collections.Generic;
using System.Threading.Tasks;
using Datory;
using SSCMS.Models;
using SSCMS.Repositories;
using SSCMS.Services;

namespace SSCMS.Core.Repositories
{
    public class TranslateRepository : ITranslateRepository
    {
        private readonly Repository<Translate> _repository;
        private readonly ISiteRepository _siteRepository;
        private readonly IChannelRepository _channelRepository;

        public TranslateRepository(ISettingsManager settingsManager, ISiteRepository siteRepository, IChannelRepository channelRepository)
        {
            _repository = new Repository<Translate>(settingsManager.Database, settingsManager.Redis);
            _siteRepository = siteRepository;
            _channelRepository = channelRepository;
        }

        public IDatabase Database => _repository.Database;

        public string TableName => _repository.TableName;

        public List<TableColumn> TableColumns => _repository.TableColumns;

        public async Task InsertAsync(Translate translate)
        {
            await _repository.InsertAsync(translate);
        }

        public async Task DeleteAsync(int siteId, int channelId)
        {
            await _repository.DeleteAsync(Q
                .Where(nameof(Translate.SiteId), siteId)
                .Where(nameof(Translate.ChannelId), channelId)
            );
        }

        public async Task<List<Translate>> GetTranslatesAsync(int siteId, bool summary = false)
        {
            var translates =  await _repository.GetAllAsync(Q
                .Where(nameof(Translate.SiteId), siteId)
                .OrderBy(nameof(Translate.Id))
            );

            if (summary)
            {
                foreach (var translate in translates)
                {
                    translate.Summary = await GetSummaryAsync(translate);
                }
            }

            return translates;
        }

        public async Task<List<Translate>> GetTranslatesAsync(int siteId, int channelId, bool summary = false)
        {
            var translates = await _repository.GetAllAsync(Q
                .Where(nameof(Translate.SiteId), siteId)
                .Where(nameof(Translate.ChannelId), channelId)
                .OrderBy(nameof(Translate.Id))
            );

            if (summary)
            {
                foreach (var translate in translates)
                {
                    translate.Summary = await GetSummaryAsync(translate);
                }
            }

            return translates;
        }

        public async Task<string> GetSummaryAsync(Translate translate)
        {
            var name = await _channelRepository.GetChannelNameNavigationAsync(translate.TargetSiteId, translate.TargetChannelId);
            if (string.IsNullOrEmpty(name)) return null;

            if (translate.TargetSiteId != translate.SiteId)
            {
                var site = await _siteRepository.GetAsync(translate.TargetSiteId);
                if (site == null) return null;

                name = site.SiteName + " : " + name;
            }

            name += $" ({translate.TranslateType.GetDisplayName()})";
            return name;
        }
    }
}