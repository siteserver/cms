using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using SSCMS.Cli.Abstractions;
using SSCMS.Cli.Models;
using SSCMS.Utils;

namespace SSCMS.Cli.Services
{
    public class ConfigService : IConfigService
    {
        private readonly IConfiguration _config;

        public ConfigService(IConfiguration config)
        {
            _config = config;
        }

        public ConfigStatus Status => _config.GetSection(nameof(Status)).Get<ConfigStatus>();

        public async Task SaveStatusAsync(ConfigStatus status)
        {
            var configPath = PathUtils.GetOsUserProfileDirectoryPath(Constants.OsUserProfileTypeConfig);

            var config = new Config
            {
                Status = status
            };

            await FileUtils.WriteTextAsync(configPath, TranslateUtils.JsonSerialize(config));
        }
    }
}
