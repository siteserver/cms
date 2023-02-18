using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Home
{
    public partial class LoginController
    {
        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get()
        {
            var config = await _configRepository.GetAsync();
            if (config.IsHomeClosed) return this.Error("对不起，用户中心已被禁用！");

            var smsSettings = await _smsManager.GetSmsSettingsAsync();
            var isSmsEnabled = smsSettings.IsSms && smsSettings.IsSmsUser;

            return new GetResult
            {
                HomeTitle = config.HomeTitle,
                IsSmsEnabled = isSmsEnabled
            };
        }
    }
}
