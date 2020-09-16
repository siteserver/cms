using System.Collections.Generic;
using System.Threading.Tasks;
using Datory;
using SSCMS.Models;

namespace SSCMS.Repositories
{
    public interface IMaterialMessageRepository : IRepository
    {
        Task InsertAsync(int groupId, string title, string imageUrl, string summary, string body);

        Task<int> InsertAsync(int groupId, string mediaId, List<MaterialMessageItem> items);

        Task UpdateAsync(int messageId, int groupId, List<MaterialMessageItem> items);

        Task UpdateAsync(int messageId, int groupId);

        Task UpdateMediaIdAsync(int id, string mediaId);

        Task<bool> DeleteAsync(int id);

        Task<int> GetCountAsync(int groupId, string keyword);

        Task<List<MaterialMessage>> GetAllAsync(int groupId, string keyword, int page, int perPage);

        Task<MaterialMessage> GetAsync(int id);

        Task<bool> IsExistsAsync(string mediaId);
    }
}