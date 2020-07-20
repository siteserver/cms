using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Datory;
using SSCMS.Enums;
using SSCMS.Models;
using SSCMS.Repositories;
using SSCMS.Services;

namespace SSCMS.Core.Repositories
{
    public partial class SiteRepository : ISiteRepository
    {
        private readonly Repository<Site> _repository;
        private readonly IPluginManager _pluginManager;
        private readonly IChannelRepository _channelRepository;
        private readonly IAdministratorRepository _administratorRepository;
        private readonly ITemplateRepository _templateRepository;
        private readonly ITableStyleRepository _tableStyleRepository;
        private readonly IContentGroupRepository _contentGroupRepository;
        private readonly IContentTagRepository _contentTagRepository;

        public SiteRepository(ISettingsManager settingsManager, IPluginManager pluginManager, IChannelRepository channelRepository, IAdministratorRepository administratorRepository, ITemplateRepository templateRepository, ITableStyleRepository tableStyleRepository, IContentGroupRepository contentGroupRepository, IContentTagRepository contentTagRepository)
        {
            _repository = new Repository<Site>(settingsManager.Database, settingsManager.Redis);
            _pluginManager = pluginManager;
            _channelRepository = channelRepository;
            _administratorRepository = administratorRepository;
            _templateRepository = templateRepository;
            _tableStyleRepository = tableStyleRepository;
            _contentGroupRepository = contentGroupRepository;
            _contentTagRepository = contentTagRepository;
        }

        public IDatabase Database => _repository.Database;

        public string TableName => _repository.TableName;

        public List<TableColumn> TableColumns => _repository.TableColumns;

        /// <summary>
        /// 添加后台发布节点
        /// </summary>
        public async Task<int> InsertSiteAsync(IPathManager pathManager, Channel channel, Site site, int adminId)
        {
            await _channelRepository.InsertChannelAsync(null, channel);

            site.Id = channel.Id;

            await InsertAsync(site);

            var adminEntity = await _administratorRepository.GetByUserIdAsync(adminId);
            await _administratorRepository.UpdateSiteIdAsync(adminEntity, channel.Id);

            channel.SiteId = site.Id;
            await _channelRepository.UpdateAsync(channel);

            await _templateRepository.CreateDefaultTemplateAsync(pathManager, site, adminId);

            return channel.Id;
        }

        public async Task<int> InsertAsync(Site site)
        {
            site.Taxis = await GetMaxTaxisAsync() + 1;
            site.Id = await _repository.InsertAsync(site, Q
                .AllowIdentityInsert()
                .CachingRemove(GetListKey())
            );
            return site.Id;
        }

        public async Task DeleteAsync(int siteId)
        {
            var site = await GetAsync(siteId);
            var list = await _channelRepository.GetChannelIdsAsync(siteId);
            await _tableStyleRepository.DeleteAsync(list, site.TableName);

            await _contentGroupRepository.DeleteAsync(siteId);
            await _contentTagRepository.DeleteAsync(siteId);

            await _channelRepository.DeleteAllAsync(siteId);

            await UpdateParentIdToZeroAsync(siteId);

            await _repository.DeleteAsync(siteId, Q
                .CachingRemove(GetListKey(), GetEntityKey(siteId))
            );
        }

        public async Task UpdateAsync(Site site)
        {
            var cache = await GetCacheAsync(site.Id);
            if (site.Root != cache.Root)
            {
                await UpdateAllIsRootAsync();
            }

            await _repository.UpdateAsync(site, Q
                .CachingRemove(GetListKey(), GetEntityKey(site.Id))
            );
        }

        public async Task UpdateTableNameAsync(int siteId, string tableName)
        {
            await _repository.UpdateAsync(Q
                .Set(nameof(Site.TableName), tableName)
                .Where(nameof(Site.Id), siteId)
                .CachingRemove(GetListKey(), GetEntityKey(siteId))
            );
        }

        public async Task UpdateParentIdToZeroAsync(int parentId)
        {
            var cacheKeys = new List<string>
            {
                GetListKey()
            };
            var siteIds = await GetSiteIdsAsync(parentId);
            foreach (var siteId in siteIds)
            {
                cacheKeys.Add(GetEntityKey(siteId));
            }
            
            await _repository.UpdateAsync(Q
                .Set(nameof(Site.ParentId), 0)
                .Where(nameof(Site.ParentId), parentId)
                .CachingRemove(cacheKeys.ToArray())
            );
        }

        private async Task UpdateAllIsRootAsync()
        {
            var cacheKeys = new List<string>
            {
                GetListKey()
            };
            var siteIds = await GetSiteIdsAsync();
            foreach (var siteId in siteIds)
            {
                cacheKeys.Add(GetEntityKey(siteId));
            }

            await _repository.UpdateAsync(Q
                .Set(nameof(Site.Root), false)
                .CachingRemove(cacheKeys.ToArray())
            );
        }

        public async Task<List<KeyValuePair<int, Site>>> ParserGetSitesAsync(string siteName, string siteDir, int startNum, int totalNum, ScopeType scopeType, TaxisType taxisType)
        {
            var sites = new List<Site>();
            var summaries = await GetSummariesAsync();

            Site site = null;
            if (!string.IsNullOrEmpty(siteName))
            {
                site = await GetSiteBySiteNameAsync(siteName);
            }
            else if (!string.IsNullOrEmpty(siteDir))
            {
                site = await GetSiteByDirectoryAsync(siteDir);
            }

            var siteIds = summaries.Select(x => x.Id).ToList();
            if (site != null)
            {
                siteIds = summaries.Where(x => x.ParentId == site.Id).Select(x => x.Id).ToList();
            }
            else
            {
                if (scopeType == ScopeType.Children)
                {
                    siteIds = summaries.Where(x => x.ParentId == 0 && !x.Root).Select(x => x.Id).ToList();
                }
                else if (scopeType == ScopeType.Descendant)
                {
                    siteIds = summaries.Where(x => !x.Root).Select(x => x.Id).ToList();
                }
            }

            foreach (var siteId in siteIds)
            {
                sites.Add(await GetAsync(siteId));
            }

            sites = ParserOrder(sites, taxisType);
            if (startNum > 1 && totalNum > 0)
            {
                sites = sites.Skip(startNum - 1).Take(totalNum).ToList();
            }
            else if (startNum > 1)
            {
                sites = sites.Skip(startNum - 1).ToList();
            }
            else if (totalNum > 0)
            {
                sites = sites.Take(totalNum).ToList();
            }

            var list = new List<KeyValuePair<int, Site>>();
            var i = 0;
            foreach (var entity in sites)
            {
                list.Add(new KeyValuePair<int, Site>(i++, entity));
            }

            return list;
        }

        private static List<Site> ParserOrder(List<Site> sites, TaxisType taxisType)
        {
            if (taxisType == TaxisType.OrderById)
            {
                return sites.OrderBy(x => x.Id).ToList();
            }

            if (taxisType == TaxisType.OrderByIdDesc)
            {
                return sites.OrderByDescending(x => x.Id).ToList();
            }

            if (taxisType == TaxisType.OrderByAddDate)
            {
                return sites.OrderBy(x => x.CreatedDate).ToList();
            }

            if (taxisType == TaxisType.OrderByAddDateDesc)
            {
                return sites.OrderByDescending(x => x.CreatedDate).ToList();
            }

            if (taxisType == TaxisType.OrderByLastModifiedDate)
            {
                return sites.OrderBy(x => x.LastModifiedDate).ToList();
            }

            if (taxisType == TaxisType.OrderByLastModifiedDate)
            {
                return sites.OrderByDescending(x => x.LastModifiedDate).ToList();
            }

            if (taxisType == TaxisType.OrderByTaxis)
            {
                return sites.OrderBy(x => x.Taxis).ToList();
            }

            if (taxisType == TaxisType.OrderByTaxisDesc)
            {
                return sites.OrderByDescending(x => x.Taxis).ToList();
            }

            if (taxisType == TaxisType.OrderByRandom)
            {
                return sites.OrderBy(x => Guid.NewGuid()).ToList();
            }

            return sites.OrderBy(x => x.Taxis).ToList();
        }
    }
}
