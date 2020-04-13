using System.Collections.Generic;
using System.Threading.Tasks;
using Datory;
using SSCMS.Models;

namespace SSCMS.Repositories
{
    public partial interface IAccessTokenRepository : IRepository
    {
        Task<int> InsertAsync(AccessToken accessToken);

        Task<bool> UpdateAsync(AccessToken accessToken);

        Task<bool> DeleteAsync(int id);

        Task<string> RegenerateAsync(AccessToken accessToken);

        Task<bool> IsTitleExistsAsync(string title);

        Task<List<AccessToken>> GetAccessTokenListAsync();

        Task<AccessToken> GetAsync(int id);

        Task<bool> IsScopeAsync(string token, string scope);
    }
}
