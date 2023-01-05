using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Mono.Options;
using SSCMS.Cli.Abstractions;
using SSCMS.Cli.Core;
using SSCMS.Dto;
using SSCMS.Plugins;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Cli.Jobs
{
    public class DataSyncJob : IJobService
    {
        public string CommandName => "data sync";

        private string _directory;
        private string _from;
        private string _to;
        private List<string> _includes;
        private List<string> _excludes;
        private int _maxRows;
        private int _pageSize = CliConstants.DefaultPageSize;
        private bool _isHelp;

        private readonly ISettingsManager _settingsManager;
        private readonly IDatabaseManager _databaseManager;
        private readonly OptionSet _options;

        public DataSyncJob(ISettingsManager settingsManager, IDatabaseManager databaseManager)
        {
            _settingsManager = settingsManager;
            _databaseManager = databaseManager;

            _options = new OptionSet
            {
                {
                    "d|directory=", "Backup folder name",
                    v => _directory = v
                },
                {
                    "from=", "Specify the path or file name of sscms.json configuration file that you want to backup",
                    v => _from = v
                },
                {
                    "to=", "Specify the path or file name of sscms.json configuration file that you want to restore",
                    v => _to = v
                },
                {
                    "includes=", "Include table names, separated by commas, default backup all tables",
                    v => _includes = v == null ? null : ListUtils.GetStringList(v)
                },
                {
                    "excludes=", "Exclude table names, separated by commas",
                    v => _excludes = v == null ? null : ListUtils.GetStringList(v)
                },
                {
                    "max-rows=", "Maximum number of rows to backup, all data is backed up by default",
                    v => _maxRows = v == null ? 0 : TranslateUtils.ToInt(v)
                },
                {
                    "page-size=", "The number of rows fetch at a time, 1000 by default",
                    v => _pageSize = v == null ? CliConstants.DefaultPageSize : TranslateUtils.ToInt(v)
                },
                {
                    "h|help", "Display help",
                    v => _isHelp = v != null
                }
            };
        }

        public async Task WriteUsageAsync(IConsoleUtils console)
        {
            await console.WriteLineAsync($"Usage: sscms {CommandName}");
            await console.WriteLineAsync("Summary: sync backup files to database");
            await console.WriteLineAsync("Options:");
            _options.WriteOptionDescriptions(console.Out);
            await console.WriteLineAsync();
        }

        public async Task ExecuteAsync(IPluginJobContext context)
        {
            if (!CliUtils.ParseArgs(_options, context.Args)) return;

            using var console = new ConsoleUtils(false);
            if (_isHelp)
            {
                await WriteUsageAsync(console);
                return;
            }

            var directory = _directory;
            if (string.IsNullOrEmpty(directory))
            {
                directory = $"backup/{DateTime.Now:yyyy-MM-dd}";
            }

            var tree = new Tree(_settingsManager, directory);
            DirectoryUtils.CreateDirectoryIfNotExists(tree.DirectoryPath);

            var backupConfigPath = PathUtils.Combine(_settingsManager.ContentRootPath, _from);
            if (!FileUtils.IsFileExists(backupConfigPath))
            {
                await console.WriteErrorAsync($"The sscms configuration file does not exist: {backupConfigPath}");
                return;
            }

            var (isConnectionWorks, errorMessage) = await _settingsManager.Database.IsConnectionWorksAsync();
            if (!isConnectionWorks)
            {
                await console.WriteErrorAsync($"Unable to connect to database, error message:{errorMessage}");
                return;
            }

            if (_excludes == null)
            {
                _excludes = new List<string>();
            }
            _excludes.Add("bairong_Log");
            _excludes.Add("bairong_ErrorLog");
            _excludes.Add("siteserver_ErrorLog");
            _excludes.Add("siteserver_Log");
            _excludes.Add("siteserver_Tracking");

            var errorLogFilePath = CliUtils.DeleteErrorLogFileIfExists(_settingsManager);
            await _databaseManager.BackupAsync(console, _includes, _excludes, _maxRows, _pageSize, tree, errorLogFilePath);

            var restoreConfigPath = PathUtils.Combine(_settingsManager.ContentRootPath, _to);
            if (!FileUtils.IsFileExists(restoreConfigPath))
            {
                await console.WriteErrorAsync($"The sscms configuration file does not exist: {restoreConfigPath}");
                return;
            }

            (isConnectionWorks, errorMessage) = await _settingsManager.Database.IsConnectionWorksAsync();
            if (!isConnectionWorks)
            {
                await console.WriteErrorAsync($"Unable to connect to database, error message:{errorMessage}");
                return;
            }

            await _databaseManager.RestoreAsync(console, _includes, _excludes, tree.DirectoryPath, tree, errorLogFilePath);

            await console.WriteRowLineAsync();
            await console.WriteSuccessAsync("sync database successfully!");
        }
    }
}
