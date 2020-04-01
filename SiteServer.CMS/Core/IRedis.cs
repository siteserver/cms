using System.Threading.Tasks;

namespace SiteServer.CMS.Core
{
    public interface IRedis
    {
        string ConnectionString { get; }
        string Host { get; }
        int Port { get; }
        string Password { get; }
        int Database { get; }
        bool AllowAdmin { get; }
        (bool IsConnectionWorks, string ErrorMessage) IsConnectionWorks();
    }
}