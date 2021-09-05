using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Configuration;
using SSCMS.Dto;

namespace SSCMS.Web.Controllers.V1
{
    public partial class UsersController
    {
        [HttpPost, Route(RouteActionsLogout)]
        public async Task<ActionResult<BoolResult>> Logout()
        {
            var cacheKey = Constants.GetSessionIdCacheKey(_authManager.UserId);
            await _dbCacheRepository.RemoveAsync(cacheKey);

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
