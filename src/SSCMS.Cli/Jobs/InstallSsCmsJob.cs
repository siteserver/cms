using System;
using System.Threading.Tasks;
using Datory;
using Mono.Options;
using SSCMS.Cli.Abstractions;
using SSCMS.Cli.Core;
using SSCMS.Repositories;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Cli.Jobs
{
    public class InstallSsCmsJob : IJobService
    {
        public string CommandName => "install sscms";

        private string _userName;
        private string _password;
        private bool _isHelp;

        private readonly ISettingsManager _settingsManager;
        private readonly IDatabaseManager _databaseManager;
        private readonly IPathManager _pathManager;
        private readonly IOldPluginManager _pluginManager;
        private readonly IConfigRepository _configRepository;
        private readonly IAdministratorRepository _administratorRepository;
        private readonly OptionSet _options;

        public InstallSsCmsJob(ISettingsManager settingsManager, IDatabaseManager databaseManager, IPathManager pathManager, IOldPluginManager pluginManager, IConfigRepository configRepository, IAdministratorRepository administratorRepository)
        {
            _settingsManager = settingsManager;
            _databaseManager = databaseManager;
            _pathManager = pathManager;
            _pluginManager = pluginManager;
            _configRepository = configRepository;
            _administratorRepository = administratorRepository;

            _options = new OptionSet {
                { "u|userName",  "Super administrator username",
                    v => _userName = v },
                { "p|password",  "Super administrator username",
                    v => _password = v },
                { "h|help",  "命令说明",
                    v => _isHelp = v != null }
            };
        }

        public void PrintUsage()
        {
            Console.WriteLine($"Usage: sscms-cli {CommandName}");
            Console.WriteLine("Summary: install sscms");
            Console.WriteLine("Options:");
            _options.WriteOptionDescriptions(Console.Out);
            Console.WriteLine();
        }

        public async Task ExecuteAsync(IJobContext context)
        {
            if (!CliUtils.ParseArgs(_options, context.Args)) return;

            if (_isHelp)
            {
                PrintUsage();
                return;
            }

            if (!await _configRepository.IsNeedInstallAsync())
            {
                await WriteUtils.PrintErrorAsync($"SS CMS has been installed in {_settingsManager.ContentRootPath}");
                return;
            }

            var userName = string.IsNullOrEmpty(_userName)
                ? ReadUtils.GetString("Super administrator username:")
                : _userName;
            var password = string.IsNullOrEmpty(_password)
                ? ReadUtils.GetPassword("Super administrator password:")
                : _password;

            var (valid, message) =
                await _administratorRepository.InsertValidateAsync(userName, password, string.Empty, string.Empty);
            if (!valid)
            {
                await WriteUtils.PrintErrorAsync(message);
                return;
            }

            if (_settingsManager.DatabaseType == DatabaseType.SQLite)
            {
                var filePath = PathUtils.Combine(_settingsManager.ContentRootPath, Constants.DefaultLocalDbFileName);
                if (!FileUtils.IsFileExists(filePath))
                {
                    await FileUtils.WriteTextAsync(filePath, string.Empty);
                }
            }

            (valid, message) = await _databaseManager.InstallAsync(_pluginManager, userName, password, string.Empty, string.Empty);
            if (!valid)
            {
                await WriteUtils.PrintErrorAsync(message);
                return;
            }

            await FileUtils.WriteTextAsync(_pathManager.GetWebRootPath("index.html"), Constants.Html5Empty);

            await WriteUtils.PrintSuccessAsync("Congratulations, SS CMS was installed successfully!");
        }
    }
}
