using System.Collections.Generic;
using System.Threading.Tasks;
using SS.CMS.Abstractions.Models;
using SS.CMS.Data;

namespace SS.CMS.Abstractions.Repositories
{
    public interface IAccessTokenRepository : IRepository
    {
        Task<int> InsertAsync(AccessTokenInfo accessTokenInfo);

        Task<bool> UpdateAsync(AccessTokenInfo accessTokenInfo);

        Task<bool> DeleteAsync(int id);

        Task<string> RegenerateAsync(AccessTokenInfo accessTokenInfo);

        Task<bool> IsScopeAsync(string token, string scope);

        Task<AccessTokenInfo> GetAsync(string token);

        Task<AccessTokenInfo> GetAsync(int id);

        Task<IEnumerable<AccessTokenInfo>> GetAllAsync();

        Task<bool> IsTitleExistsAsync(string title);

        string ScopeContents { get; }
        string ScopeAdministrators { get; }
        string ScopeUsers { get; }
        string ScopeStl { get; }

        List<string> ScopeList { get; }
    }
}
