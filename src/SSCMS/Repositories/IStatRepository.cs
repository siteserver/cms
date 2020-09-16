using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Datory;
using SSCMS.Enums;
using SSCMS.Models;

namespace SSCMS.Repositories
{
    public interface IStatRepository : IRepository
    {
        Task AddCountAsync(StatType statType, int siteId = 0);

        Task<List<Stat>> GetStatsAsync(DateTime lowerDate, DateTime higherDate, StatType statType,
            int siteId = 0);
    }
}