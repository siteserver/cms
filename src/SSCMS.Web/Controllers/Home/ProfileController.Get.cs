using System.Linq;
using System.Threading.Tasks;
using Datory;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Configuration;
using SSCMS.Core.Utils;
using SSCMS.Enums;
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
            var entity = new Entity();
            var userStyles = await _tableStyleRepository.GetUserStylesAsync();
            var styles = userStyles.Select(x => new InputStyle(x));

            foreach (var style in styles)
            {
                if (style.InputType == InputType.Image ||
                    style.InputType == InputType.Video ||
                    style.InputType == InputType.File)
                {
                    var count = user.Get(ColumnsManager.GetCountName(style.AttributeName),
                        0);
                    entity.Set(ColumnsManager.GetCountName(style.AttributeName), count);
                    for (var n = 0; n <= count; n++)
                    {
                        var extendName = ColumnsManager.GetExtendName(style.AttributeName, n);
                        entity.Set(extendName, user.Get(extendName));
                    }
                }
                else if (style.InputType == InputType.CheckBox ||
                         style.InputType == InputType.SelectMultiple)
                {
                    var list = ListUtils.GetStringList(user.Get(style.AttributeName,
                        string.Empty));
                    entity.Set(style.AttributeName, list);
                }
                else
                {
                    entity.Set(style.AttributeName, user.Get(style.AttributeName));
                }
            }

            entity.Set(nameof(Models.User.Id), user.Id);
            entity.Set(nameof(Models.User.UserName), user.UserName);
            entity.Set(nameof(Models.User.DisplayName), user.DisplayName);
            entity.Set(nameof(Models.User.AvatarUrl), user.AvatarUrl);
            entity.Set(nameof(Models.User.Mobile), user.Mobile);
            entity.Set(nameof(Models.User.Email), user.Email);

            var isUserVerifyMobile = false;
            var smsSettings = await _smsManager.GetSmsSettingsAsync();
            var isSmsEnabled = smsSettings.IsSms && smsSettings.IsSmsUser;
            if (isSmsEnabled && config.IsUserForceVerifyMobile)
            {
                isUserVerifyMobile = true;
            }

            var settings = new Settings
            {
                IsCloudImages = await _cloudManager.IsImagesAsync(),
            };

            return new GetResult
            {
                IsSmsEnabled = isSmsEnabled,
                IsUserVerifyMobile = isUserVerifyMobile,
                Entity = entity,
                Styles = styles,
                Settings = settings
            };
        }
    }
}
