using System.Threading.Tasks;
using SSCMS.Models;

namespace SSCMS.Repositories
{
    public partial interface IAccessTokenRepository
    {
        Task<AccessToken> GetByTokenAsync(string token);
    }
}
