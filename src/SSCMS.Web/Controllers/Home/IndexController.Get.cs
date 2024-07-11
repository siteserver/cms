using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Home
{
    public partial class IndexController
    {
        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get()
        {
            var config = await _configRepository.GetAsync();
            if (config.IsHomeClosed) return this.Error("对不起，用户中心已被禁用！");

            var user = await _authManager.GetUserAsync();
            if (user == null)
            {
                return Unauthorized();
            }

            var menus = await GetMenusAsync(user);

            return new GetResult
            {
                User = user,
                HomeTitle = config.HomeTitle,
                HomeLogoUrl = config.HomeLogoUrl,
                Menus = menus
            };
        }
    }
}
