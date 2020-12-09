using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Configuration;
using SSCMS.Utils;
using SSCMS.Core.Utils;

namespace SSCMS.Web.Controllers.Admin.Settings.Users
{
    public partial class UsersStyleController
    {
        [HttpDelete, Route(Route)]
        public async Task<ActionResult<DeleteResult>> Delete([FromBody] DeleteRequest request)
        {
            if (!await _authManager.HasAppPermissionsAsync(MenuUtils.AppPermissions.SettingsUsersStyle))
            {
                return Unauthorized();
            }

            await _tableStyleRepository.DeleteAsync(_userRepository.TableName, 0, request.AttributeName);

            var allAttributes = _userRepository.TableColumns.Select(x => x.AttributeName).ToList();

            var styles = new List<InputStyle>();
            foreach (var style in await _tableStyleRepository.GetUserStylesAsync())
            {
                styles.Add(new InputStyle
                {
                    Id = style.Id,
                    AttributeName = style.AttributeName,
                    DisplayName = style.DisplayName,
                    InputType = style.InputType,
                    Rules = TranslateUtils.JsonDeserialize<List<InputStyleRule>>(style.RuleValues),
                    Taxis = style.Taxis,
                    IsSystem = ListUtils.ContainsIgnoreCase(allAttributes, style.AttributeName)
                });
            }

            return new DeleteResult
            {
                Styles = styles
            };
        }
    }
}
