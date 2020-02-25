using SS.CMS.Abstractions;
using SS.CMS.Framework;

namespace SS.CMS.Core
{
    public static class GlobalSettings
    {
        public static void Load(ISettingsManager settingsManager)
        {
            ContentRootPath = settingsManager.ContentRootPath;
            WebRootPath = settingsManager.WebRootPath;

            SettingsManager = settingsManager;

            WebConfigUtils.Load(settingsManager);
            DataProvider.Load(settingsManager);
        }

        public static string WebRootPath { get; private set; }

        public static string ContentRootPath { get; private set; }

        public static ISettingsManager SettingsManager { get; private set; }
    }
}
