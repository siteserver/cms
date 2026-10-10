using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Configuration;
using SSCMS.Core.Utils;
using SSCMS.Dto;

namespace SSCMS.Web.Controllers.Admin
{
    public partial class LogoutController
    {
        [HttpPost, Route(Route)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<BoolResult>> Submit()
        {
            var cacheKey = Constants.GetSessionIdCacheKey(_authManager.AdminId);
            await _dbCacheRepository.RemoveAsync(cacheKey);
            // 登出即吊销当前账号已签发的 JWT（#3894）
            await TokenRevocationUtils.RevokeAsync(_dbCacheRepository, true, _authManager.AdminId);

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
