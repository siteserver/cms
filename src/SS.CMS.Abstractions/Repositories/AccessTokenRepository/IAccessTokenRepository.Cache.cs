using System.Threading.Tasks;

namespace SS.CMS.Abstractions
{
    public partial interface IAccessTokenRepository
    {
        Task<AccessToken> GetByTokenAsync(string token);
    }
}
