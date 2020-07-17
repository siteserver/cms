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
    public class StatRepository : IStatRepository
    {
        private readonly Repository<Stat> _repository;

        public StatRepository(ISettingsManager settingsManager)
        {
            _repository = new Repository<Stat>(settingsManager.Database, settingsManager.Redis);
        }

        public IDatabase Database => _repository.Database;

        public string TableName => _repository.TableName;

        public List<TableColumn> TableColumns => _repository.TableColumns;

        public async Task AddCountAsync(StatType statType, int siteId = 0)
        {
            var lowerDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            var higherDate = lowerDate.AddDays(1);

            var query = Q
                .Where(nameof(Stat.SiteId), siteId)
                .Where(nameof(Stat.StatType), statType.GetValue())
                .WhereBetween(nameof(Stat.CreatedDate), lowerDate, higherDate);

            if (await _repository.ExistsAsync(query))
            {
                await _repository.IncrementAsync(nameof(Stat.Count), query);
            }
            else
            {
                await _repository.InsertAsync(new Stat
                {
                    StatType = statType,
                    SiteId = siteId,
                    Count = 1
                });
            }
        }

        public async Task<List<Stat>> GetStatsAsync(DateTime lowerDate, DateTime higherDate, StatType statType, int siteId = 0)
        {
            var query = Q
                .Where(nameof(Stat.StatType), statType.GetValue())
                .WhereBetween(nameof(Stat.CreatedDate), lowerDate, higherDate.AddDays(1));

            if (siteId > 0)
            {
                query.Where(nameof(Stat.SiteId), siteId);
            }

            return await _repository.GetAllAsync(query);
        }
    }
}
