using SS.CMS.Abstractions;
using SS.CMS.Data;

namespace SS.CMS.Core.Components
{
    public class EnvironmentImpl : IEnvironment
    {
        public EnvironmentImpl(IDb db, string homeDirectory, string adminDirectory, string contentRootPath, string webRootPath, string applicationPath, string apiUrl)
        {
            Db = db;
            HomeDirectory = homeDirectory;
            AdminDirectory = adminDirectory;
            ContentRootPath = contentRootPath;
            WebRootPath = webRootPath;
            ApplicationPath = applicationPath;
            ApiUrl = apiUrl;
        }

        public IDb Db { get; }

        public string HomeDirectory { get; }

        public string AdminDirectory { get; }

        public string ContentRootPath { get; }

        public string WebRootPath { get; }

        public string ApplicationPath { get; }

        public string ApiUrl { get; }
    }
}
