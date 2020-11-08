using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Configuration;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Home
{
    public partial class ProfileController
    {
        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get()
        {
            var config = await _configRepository.GetAsync();
            if (config.IsHomeClosed) return this.Error("对不起，用户中心已被禁用！");

            var user = await _authManager.GetUserAsync();
            var userStyles = await _tableStyleRepository.GetUserStylesAsync();
            var styles = userStyles.Select(x => new InputStyle(x));

            var isUserVerifyMobile = false;
            var isSmsEnabled = await _smsManager.IsEnabledAsync();
            if (isSmsEnabled && config.IsUserForceVerifyMobile)
            {
                isUserVerifyMobile = true;
            }

            return new GetResult
            {
                IsSmsEnabled = isSmsEnabled,
                IsUserVerifyMobile = isUserVerifyMobile,
                User = user,
                Styles = styles
            };
        }
    }
}
