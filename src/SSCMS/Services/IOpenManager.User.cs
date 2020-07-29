using System.Collections.Generic;
using System.Threading.Tasks;

namespace SSCMS.Services
{
    public partial interface IOpenManager
    {
        Task<(bool, string, string)> GetWxAccessTokenAsync(int siteId);

        Task<IEnumerable<(int Id, string Name, int Count)>> GetUserTags(string token);
    }
}
