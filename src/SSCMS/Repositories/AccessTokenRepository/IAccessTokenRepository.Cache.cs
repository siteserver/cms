using System.Threading.Tasks;

namespace SSCMS.Abstractions
{
    public partial interface IAccessTokenRepository
    {
        Task<AccessToken> GetByTokenAsync(string token);
    }
}
