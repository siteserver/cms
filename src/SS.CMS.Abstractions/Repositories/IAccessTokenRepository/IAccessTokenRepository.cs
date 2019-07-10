using System.Collections.Generic;
using System.Threading.Tasks;
using SS.CMS.Data;
using SS.CMS.Models;

namespace SS.CMS.Repositories
{
    public interface IAccessTokenRepository : IRepository
    {
        Task<int> InsertAsync(AccessToken accessTokenInfo);

        Task<bool> UpdateAsync(AccessToken accessTokenInfo);

        Task<bool> DeleteAsync(int id);

        Task<string> RegenerateAsync(AccessToken accessTokenInfo);

        Task<bool> IsScopeAsync(string token, string scope);

        Task<AccessToken> GetAsync(string token);

        Task<AccessToken> GetAsync(int id);

        Task<IEnumerable<AccessToken>> GetAllAsync();

        Task<bool> IsTitleExistsAsync(string title);

        string ScopeContents { get; }
        string ScopeAdministrators { get; }
        string ScopeUsers { get; }
        string ScopeStl { get; }

        List<string> ScopeList { get; }
    }
}
