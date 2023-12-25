using System.Collections.Generic;
using System.Threading.Tasks;

namespace SSCMS.Services
{
    public partial interface IWxManager
    {
        Task<(bool success, string errorMessage)> UserGetAsync(string accessToken, List<string> openIds, string nextOpenId = null);
    }
}
