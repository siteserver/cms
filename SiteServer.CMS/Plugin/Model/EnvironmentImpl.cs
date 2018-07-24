using System.Web;
using SiteServer.Plugin;

namespace SiteServer.CMS.Plugin.Model
{
    public class EnvironmentImpl: IEnvironment
    {
        public EnvironmentImpl(DatabaseType databaseType, string connectionString, string adminDirectory, string physicalApplicationPath)
        {
            DatabaseType = databaseType;
            ConnectionString = connectionString;
            AdminDirectory = adminDirectory;
            PhysicalApplicationPath = physicalApplicationPath;
        }

        public DatabaseType DatabaseType { get; }

        public string ConnectionString { get; }

        public string AdminDirectory { get; }

        public string PhysicalApplicationPath { get; }

        public IRequest Request => HttpContext.Current != null ? new AuthRequest(HttpContext.Current.Request) : null;
    }
}
