using Datory;
using SS.CMS.Abstractions;

namespace SS.CMS.Core
{
    public static class GlobalSettings
    {
        public static void Load(ISettingsManager settingsManager,
            IErrorLogRepository errorLogRepository,
            IContentRepository contentRepository,
            IPluginRepository pluginRepository,
            ITableStyleRepository tableStyleRepository)
        {
            ContentRootPath = settingsManager.ContentRootPath;
            WebRootPath = settingsManager.WebRootPath;
            SettingsManager = settingsManager;
            Database = settingsManager.Database;
            ErrorLogRepository = errorLogRepository;
            ContentRepository = contentRepository;
            PluginRepository = pluginRepository;
            TableStyleRepository = tableStyleRepository;
        }

        public static string WebRootPath { get; private set; }

        public static string ContentRootPath { get; private set; }

        public static ISettingsManager SettingsManager { get; private set; }

        public static IDatabase Database { get; private set; }

        public static  IErrorLogRepository ErrorLogRepository { get; private set; }

        public static IContentRepository ContentRepository { get; private set; }

        public static IPluginRepository PluginRepository { get; private set; }

        public static ITableStyleRepository TableStyleRepository { get; private set; }
    }
}
