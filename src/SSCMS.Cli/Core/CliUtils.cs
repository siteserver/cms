using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Mono.Options;
using SSCMS.Cli.Abstractions;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Cli.Core
{
    public static class CliUtils
    {
        private static ServiceProvider Provider { get; set; }
        public static void SetProvider(ServiceProvider provider)
        {
            Provider = provider;
        }

        public static Application GetApplication()
        {
            return Provider.GetRequiredService<Application>();
        }

        public static IJobService GetJobService(string commandName)
        {
            var services = Provider.GetServices<IJobService>();
            return services.FirstOrDefault(x => StringUtils.EqualsIgnoreCase(x.CommandName, commandName));
        }

        public static List<string> GetJobServiceCommandNames()
        {
            var services = Provider.GetServices<IJobService>();
            return services.Select(x => x.CommandName).ToList();
        }

        public static IEnumerable<IJobService> GetJobServices()
        {
            return Provider.GetServices<IJobService>();
        }

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

        public static string CreateErrorLogFile(string commandName, ISettingsManager settingsManager)
        {
            var filePath = PathUtils.Combine(settingsManager.ContentRootPath, $"{commandName}.error.log");
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

        public static string GetWebConfigPath(string configFile, ISettingsManager settingsManager)
        {
            return PathUtils.IsFilePath(configFile)
                ? configFile
                : PathUtils.Combine(settingsManager.ContentRootPath,
                    !string.IsNullOrEmpty(configFile) ? configFile : Constants.ConfigFileName);
        }

        public static bool IsSsCmsExists(string directoryPath)
        {
            return FileUtils.IsFileExists(PathUtils.Combine(directoryPath, Constants.ConfigFileName)) && FileUtils.IsFileExists(PathUtils.Combine(directoryPath, "appsettings.json")) && DirectoryUtils.IsDirectoryExists("wwwroot");
        }
    }
}
