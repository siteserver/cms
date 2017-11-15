namespace SiteServer.Plugin.Models
{
    public class PluginEnvironment
    {
        public PluginEnvironment(string databaseType, string connectionString, string physicalApplicationPath, bool isCommandLine)
        {
            DatabaseType = databaseType;
            ConnectionString = connectionString;
            PhysicalApplicationPath = physicalApplicationPath;
            IsCommandLine = isCommandLine;
        }

        public string DatabaseType { get; }

        public string ConnectionString { get; }

        public string PhysicalApplicationPath { get; }

        public bool IsCommandLine { get; }
    }
}
