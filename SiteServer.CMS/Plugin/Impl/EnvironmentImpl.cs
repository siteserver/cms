using System.Web;
using Datory;
using SiteServer.Plugin;

namespace SiteServer.CMS.Plugin.Impl
{
    public class EnvironmentImpl: IEnvironment
    {
        public EnvironmentImpl(Database database, string homeDirectory, string adminDirectory, string physicalApplicationPath)
        {
            Database = database;
            HomeDirectory = homeDirectory;
            AdminDirectory = adminDirectory;
            PhysicalApplicationPath = physicalApplicationPath;
        }

        public Database Database { get; }

        public string HomeDirectory { get; }

        public string AdminDirectory { get; }

        public string PhysicalApplicationPath { get; }
    }
}
