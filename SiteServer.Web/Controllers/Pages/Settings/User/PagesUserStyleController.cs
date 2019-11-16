using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using NSwag.Annotations;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.Utils;

namespace SiteServer.API.Controllers.Pages.Settings.User
{
    [OpenApiIgnore]
    [RoutePrefix("pages/settings/userStyle")]
    public class PagesUserStyleController : ApiController
    {
        private const string Route = "";

        [HttpGet, Route(Route)]
        public async Task<IHttpActionResult> Get()
        {
            try
            {
                var request = await AuthenticatedRequest.GetRequestAsync();
                if (!request.IsAdminLoggin ||
                    !await request.AdminPermissionsImpl.HasSystemPermissionsAsync(ConfigManager.SettingsPermissions.User))
                {
                    return Unauthorized();
                }

                var allAttributes = DataProvider.UserDao.TableColumns.Select(x => x.AttributeName).ToList();

                var list = new List<object>();
                foreach (var style in await TableStyleManager.GetUserStyleListAsync())
                {
                    list.Add(new
                    {
                        style.Id,
                        style.AttributeName,
                        style.DisplayName,
                        InputType = InputTypeUtils.GetText(style.Type),
                        Validate = style.VeeValidate,
                        style.Taxis,
                        IsSystem = StringUtils.ContainsIgnoreCase(allAttributes, style.AttributeName)
                    });
                }

                return Ok(new
                {
                    Value = list,
                    DataProvider.UserDao.TableName,
                    RelatedIdentities = TableStyleManager.EmptyRelatedIdentities
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpDelete, Route(Route)]
        public async Task<IHttpActionResult> Delete()
        {
            try
            {
                var request = await AuthenticatedRequest.GetRequestAsync();
                if (!request.IsAdminLoggin ||
                    !await request.AdminPermissionsImpl.HasSystemPermissionsAsync(ConfigManager.SettingsPermissions.Admin))
                {
                    return Unauthorized();
                }

                var attributeName = request.GetPostString("attributeName");

                await DataProvider.TableStyleDao.DeleteAsync(0, DataProvider.UserDao.TableName, attributeName);

                var allAttributes = DataProvider.UserDao.TableColumns.Select(x => x.AttributeName).ToList();

                var list = new List<object>();
                foreach (var style in await TableStyleManager.GetUserStyleListAsync())
                {
                    list.Add(new
                    {
                        style.Id,
                        style.AttributeName,
                        style.DisplayName,
                        InputType = InputTypeUtils.GetText(style.Type),
                        Validate = TableStyleManager.GetValidateInfo(style),
                        style.Taxis,
                        IsSystem = StringUtils.ContainsIgnoreCase(allAttributes, style.AttributeName)
                    });
                }

                return Ok(new
                {
                    Value = list
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
