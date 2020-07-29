using System.Collections.Generic;
using System.Threading.Tasks;
using Datory;
using SSCMS.Models;

namespace SSCMS.Repositories
{
    public interface IMaterialAudioRepository : IRepository
    {
        Task<int> InsertAsync(MaterialAudio audio);

        Task<bool> UpdateAsync(MaterialAudio audio);

        Task<bool> DeleteAsync(int id);

        Task<int> GetCountAsync(int groupId, string keyword);

        Task<List<MaterialAudio>> GetAllAsync(int groupId, string keyword, int page, int perPage);

        Task<MaterialAudio> GetAsync(int id);

        Task<string> GetUrlByIdAsync(int id);

        Task<string> GetUrlByTitleAsync(string title);

        Task<bool> IsExistsAsync(string mediaId);
    }
}
