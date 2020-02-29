using Datory;

namespace SS.CMS.Core.Plugins
{
    public class EnvironmentImpl
    {
        public EnvironmentImpl(DatabaseType databaseType, string connectionString, string homeDirectory, string adminDirectory, string physicalApplicationPath, string apiUrl)
        {
            DatabaseType = databaseType;
            ConnectionString = connectionString;
            HomeDirectory = homeDirectory;
            AdminDirectory = adminDirectory;
            PhysicalApplicationPath = physicalApplicationPath;
            ApiUrl = apiUrl;
        }

        public DatabaseType DatabaseType { get; }

        public string ConnectionString { get; }

        public string HomeDirectory { get; }

        public string AdminDirectory { get; }

        public string PhysicalApplicationPath { get; }

        public string ApiUrl { get; }
    }
}
