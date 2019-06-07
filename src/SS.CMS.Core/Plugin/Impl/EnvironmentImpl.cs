using SS.CMS.Data;
using SS.CMS.Plugin;

namespace SS.CMS.Core.Plugin.Impl
{
    public class EnvironmentImpl : IEnvironment
    {
        public EnvironmentImpl(DbContext dbContext, string homeDirectory, string adminDirectory, string contentRootPath, string webRootPath, string applicationPath, string apiUrl)
        {
            DbContext = dbContext;
            HomeDirectory = homeDirectory;
            AdminDirectory = adminDirectory;
            ContentRootPath = contentRootPath;
            WebRootPath = webRootPath;
            ApplicationPath = applicationPath;
            ApiUrl = apiUrl;
        }

        public DbContext DbContext { get; }

        public string HomeDirectory { get; }

        public string AdminDirectory { get; }

        public string ContentRootPath { get; }

        public string WebRootPath { get; }

        public string ApplicationPath { get; }

        public string ApiUrl { get; }
    }
}
