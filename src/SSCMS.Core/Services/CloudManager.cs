using System.Threading.Tasks;
using SSCMS.Core.Utils;
using SSCMS.Repositories;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Core.Services
{
    public partial class CloudManager : ICloudManager
    {
        private const string RouteCensor = "censor";
        private const string RouteCensorAddWhiteList = "censor/actions/addWhiteList";
        private const string RouteSpell = "spell";
        private const string RouteSpellAddWhiteList = "spell/actions/addWhiteList";
        private const string RouteSms = "sms";
        private readonly ISettingsManager _settingsManager;
        private readonly IConfigRepository _configRepository;
        private readonly IErrorLogRepository _errorLogRepository;

        public CloudManager(ISettingsManager settingsManager, IConfigRepository configRepository, IErrorLogRepository errorLogRepository)
        {
            _settingsManager = settingsManager;
            _configRepository = configRepository;
            _errorLogRepository = errorLogRepository;
        }

        public static string GetCloudUrl(string relatedUrl) => PageUtils.Combine(CloudUtils.CloudApiHost,
            "v7/cloud",
            relatedUrl);

        public async Task SetAuthentication(string userName, string token)
        {
            var config = await _configRepository.GetAsync();
            config.CloudUserName = userName;
            config.CloudToken = token;
            await _configRepository.UpdateAsync(config);
        }

        public async Task RemoveAuthentication()
        {
            var config = await _configRepository.GetAsync();
            config.CloudUserName = string.Empty;
            config.CloudToken = string.Empty;
            await _configRepository.UpdateAsync(config);
        }
    }
}
