using SiteServer.Plugin.Models;

namespace SiteServer.Plugin.Apis
{
    public interface IDataApi
    {
        string DatabaseType { get; }

        string ConnectionString { get; }

        IDbHelper DbHelper { get; }
    }
}
