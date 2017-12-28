namespace SiteServer.Plugin.Models
{
    public class PluginEnvironment
    {
        public PluginEnvironment(string databaseType, string connectionString, string physicalApplicationPath)
        {
            DatabaseType = databaseType;
            ConnectionString = connectionString;
            PhysicalApplicationPath = physicalApplicationPath;
        }

        public string DatabaseType { get; }

        public string ConnectionString { get; }

        public string PhysicalApplicationPath { get; }
    }
}
