using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Datory;
using Mono.Options;
using SSCMS.Cli.Abstractions;
using SSCMS.Cli.Core;
using SSCMS.Dto;
using SSCMS.Plugins;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Cli.Jobs
{
    public class DataBackupJob : IJobService
    {
        public string CommandName => "data backup";

        private string _directory;
        private List<string> _includes;
        private List<string> _excludes;
        private int _maxRows;
        private int _pageSize = CliConstants.DefaultPageSize;
        private bool _isHelp;

        private readonly ISettingsManager _settingsManager;
        private readonly IDatabaseManager _databaseManager;
        private readonly OptionSet _options;

        public DataBackupJob(ISettingsManager settingsManager, IDatabaseManager databaseManager)
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
            await console.WriteLineAsync("Summary: backup database to folder");
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

            var configPath = CliUtils.GetConfigPath(_settingsManager);
            if (!FileUtils.IsFileExists(configPath))
            {
                await console.WriteErrorAsync($"The sscms.json file does not exist: {configPath}");
                return;
            }

            await console.WriteLineAsync($"Database type: {_settingsManager.DatabaseType.GetDisplayName()}");
            await console.WriteLineAsync($"Database connection string: {_settingsManager.DatabaseConnectionString}");
            await console.WriteLineAsync($"Backup folder: {tree.DirectoryPath}");

            var (isConnectionWorks, errorMessage) = await _settingsManager.Database.IsConnectionWorksAsync();
            if (!isConnectionWorks)
            {
                await console.WriteErrorAsync($"Unable to connect to database, error message:{errorMessage}");
                return;
            }

            var errorLogFilePath = CliUtils.DeleteErrorLogFileIfExists(_settingsManager);
            var errorTableNames = await _databaseManager.BackupAsync(console, _includes, _excludes, _maxRows, _pageSize, tree, errorLogFilePath);

            await console.WriteRowLineAsync();
            if (errorTableNames.Count == 0)
            {
                await console.WriteSuccessAsync("backup database to folder successfully!");
            }
            else
            {
                await console.WriteErrorAsync($"Database backup failed and the following table was not successfully backed up: {ListUtils.ToString(errorTableNames)}");
            }
        }
    }
}
