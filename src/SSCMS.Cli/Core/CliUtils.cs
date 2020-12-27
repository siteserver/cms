using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Mono.Options;
using SSCMS.Configuration;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Cli.Core
{
    public static class CliUtils
    {
        // https://stackoverflow.com/questions/491595/best-way-to-parse-command-line-arguments-in-c
        public static bool ParseArgs(OptionSet options, string[] args)
        {
            try
            {
                options.Parse(args);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static string DeleteErrorLogFileIfExists(ISettingsManager settingsManager)
        {
            var filePath = PathUtils.Combine(settingsManager.ContentRootPath, "sscms-cli.error.log");
            FileUtils.DeleteFileIfExists(filePath);
            return filePath;
        }

        public static async Task AppendErrorLogsAsync(string filePath, List<TextLogInfo> logs)
        {
            if (logs == null || logs.Count <= 0) return;

            if (!FileUtils.IsFileExists(filePath))
            {
                await FileUtils.WriteTextAsync(filePath, string.Empty);
            }

            var builder = new StringBuilder();

            foreach (var log in logs)
            {
                builder.AppendLine();
                builder.Append(log);
                builder.AppendLine();
            }

            await FileUtils.AppendTextAsync(filePath, Encoding.UTF8, builder.ToString());
        }

        public static async Task AppendErrorLogAsync(string filePath, TextLogInfo log)
        {
            if (log == null) return;

            if (!FileUtils.IsFileExists(filePath))
            {
                await FileUtils.WriteTextAsync(filePath, string.Empty);
            }

            var builder = new StringBuilder();

            builder.AppendLine();
            builder.Append(log);
            builder.AppendLine();

            await FileUtils.AppendTextAsync(filePath, Encoding.UTF8, builder.ToString());
        }

        public static string GetConfigPath(ISettingsManager settingsManager)
        {
            return PathUtils.Combine(settingsManager.ContentRootPath, Constants.ConfigFileName);
        }

        public static bool IsSsCmsExists(string directoryPath)
        {
            return FileUtils.IsFileExists(PathUtils.Combine(directoryPath, Constants.ConfigFileName)) && FileUtils.IsFileExists(PathUtils.Combine(directoryPath, "appsettings.json")) && DirectoryUtils.IsDirectoryExists(Constants.WwwrootDirectory);
        }

        public static string GetOsUserConfigFilePath()
        {
            return PathUtils.GetOsUserProfileDirectoryPath("config.json");
        }

        public static string GetOsUserPluginsDirectoryPath(params string[] paths)
        {
            return PathUtils.GetOsUserProfileDirectoryPath("plugins", PageUtils.Combine(paths));
        }
    }
}
