using System;
using System.Threading.Tasks;
using Datory;
using SSCMS;

namespace SSCMS.Core.Repositories.ConfigRepository
{
    public partial class ConfigRepository
    {
        public async Task<Config> GetAsync()
        {
            Config config = null;
            try
            {
                config = await _repository.GetAsync(Q
                    .OrderBy(nameof(Config.Id))
                    .CachingGet(_cacheKey)
                );
            }
            catch
            {
                // ignored
            }

            return config ?? new Config
            {
                Id = 0,
                DatabaseVersion = string.Empty,
                UpdateDate = DateTime.Now
            };
        }
    }
}
