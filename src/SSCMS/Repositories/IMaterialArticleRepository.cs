using System.Collections.Generic;
using System.Threading.Tasks;
using Datory;
using SSCMS.Models;

namespace SSCMS.Repositories
{
    public interface IMaterialArticleRepository : IRepository
    {
        Task<int> InsertAsync(MaterialArticle article);

        Task<bool> UpdateAsync(MaterialArticle article);

        Task UpdateUrlAsync(int id, string url);

        Task<bool> DeleteAsync(int id);

        Task<int> GetCountAsync(int groupId, string keyword, List<int> articleIds = null);

        Task<List<MaterialArticle>> GetAllAsync(int groupId, string keyword, int page, int perPage, List<int> articleIds = null);

        Task<MaterialArticle> GetAsync(int id);

        Task<string> GetBodyByIdAsync(int id);

        Task<string> GetBodyByTitleAsync(string title);
    }
}