using System.Threading.Tasks;
using SSCMS.Core.Utils;
using SSCMS.Core.Utils.Serialization.Components;

namespace SSCMS.Core.Services
{
    public partial class FormManager
    {
        public async Task ImportFormAsync(int siteId, string directoryPath, bool overwrite)
        {
            var caching = new CacheUtils(_cacheManager);
            var formIe = new FormIe(_pathManager, _databaseManager, caching, siteId, directoryPath);
            await formIe.ImportFormAsync(siteId, directoryPath, overwrite);
        }

        public async Task ExportFormAsync(int siteId, string directoryPath, int formId)
        {
            var caching = new CacheUtils(_cacheManager);
            var formIe = new FormIe(_pathManager, _databaseManager, caching, siteId, directoryPath);
            await formIe.ExportFormAsync(siteId, directoryPath, formId);
        }
    }
}
