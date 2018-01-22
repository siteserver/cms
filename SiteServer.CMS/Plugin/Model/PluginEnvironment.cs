using SiteServer.Plugin;

namespace SiteServer.CMS.Plugin.Model
{
    public class PluginEnvironment: IEnvironment
    {
        public PluginEnvironment(string databaseType, string connectionString, string adminDirectory, string physicalApplicationPath)
        {
            DatabaseType = databaseType;
            ConnectionString = connectionString;
            AdminDirectory = adminDirectory;
            PhysicalApplicationPath = physicalApplicationPath;
        }

        public string DatabaseType { get; }

        public string ConnectionString { get; }

        public string AdminDirectory { get; }

        public string PhysicalApplicationPath { get; }
    }
}
