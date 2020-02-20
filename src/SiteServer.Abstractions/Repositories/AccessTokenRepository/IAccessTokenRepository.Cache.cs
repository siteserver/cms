using System.Threading.Tasks;

namespace SiteServer.Abstractions
{
    public partial interface IAccessTokenRepository
    {
        Task<AccessToken> GetByTokenAsync(string token);
    }
}
