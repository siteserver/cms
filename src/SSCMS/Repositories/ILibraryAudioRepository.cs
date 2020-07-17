using System.Collections.Generic;
using System.Threading.Tasks;
using Datory;
using SSCMS.Models;

namespace SSCMS.Repositories
{
    public interface ILibraryAudioRepository : IRepository
    {
        Task<int> InsertAsync(LibraryAudio library);

        Task<bool> UpdateAsync(LibraryAudio library);

        Task<bool> DeleteAsync(int libraryId);

        Task<int> GetCountAsync(int groupId, string keyword);

        Task<List<LibraryAudio>> GetAllAsync(int groupId, string keyword, int page, int perPage);

        Task<LibraryAudio> GetAsync(int libraryId);

        Task<string> GetUrlByIdAsync(int id);

        Task<string> GetUrlByTitleAsync(string title);
    }
}
