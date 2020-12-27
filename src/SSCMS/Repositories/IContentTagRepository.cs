using System.Collections.Generic;
using System.Threading.Tasks;
using Datory;
using SSCMS.Models;

namespace SSCMS.Repositories
{
    public interface IContentTagRepository : IRepository
    {
        Task InsertAsync(int siteId, string tagName);

        Task InsertAsync(ContentTag tag);

        Task DeleteAsync(int siteId, string tagName);

        Task DeleteAsync(int siteId);

        Task UpdateTagsAsync(List<string> previousTags, List<string> nowTags, int siteId, int contentId);

        Task<List<string>> GetTagNamesAsync(int siteId);

        Task<List<ContentTag>> GetTagsAsync(int siteId);

        List<ContentTag> GetTagsByLevel(List<ContentTag> tagInfoList, int totalNum, int tagLevel);
    }
}