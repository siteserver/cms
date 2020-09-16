using System;
using System.Threading.Tasks;
using SSCMS.Core.Utils;
using SSCMS.Models;

namespace SSCMS.Core.Repositories
{
    public partial class ErrorLogRepository
    {
        public async Task<int> AddErrorLogAsync(ErrorLog log)
        {
            try
            {
                var config = await _configRepository.GetAsync();
                if (!config.IsLogError) return 0;

                await DeleteIfThresholdAsync();

                return await InsertAsync(log);
            }
            catch
            {
                // ignored
            }

            return 0;
        }

        public async Task<int> AddErrorLogAsync(Exception ex, string summary = "")
        {
            return await AddErrorLogAsync(new ErrorLog
            {
                Id = 0,
                Category = LogUtils.CategoryAdmin,
                PluginId = string.Empty,
                Message = ex.Message,
                StackTrace = ex.StackTrace,
                Summary = summary,
            });
        }
        public async Task<int> AddErrorLogAsync(string pluginId, Exception ex, string summary = "")
        {
            return await AddErrorLogAsync(new ErrorLog
            {
                Id = 0,
                Category = LogUtils.CategoryAdmin,
                PluginId = pluginId,
                Message = ex.Message,
                StackTrace = ex.StackTrace,
                Summary = summary,
            });
        }
    }
}
