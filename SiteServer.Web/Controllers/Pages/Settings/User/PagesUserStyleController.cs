using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using SiteServer.Abstractions;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Repositories;

namespace SiteServer.API.Controllers.Pages.Settings.User
{
    
    [RoutePrefix("pages/settings/userStyle")]
    public class PagesUserStyleController : ApiController
    {
        private const string Route = "";

        [HttpGet, Route(Route)]
        public async Task<IHttpActionResult> Get()
        {
            try
            {
                var request = await AuthenticatedRequest.GetAuthAsync();
                if (!request.IsAdminLoggin ||
                    !await request.AdminPermissionsImpl.HasSystemPermissionsAsync(Constants.AppPermissions.SettingsUserStyle))
                {
                    return Unauthorized();
                }

                var allAttributes = DataProvider.UserRepository.TableColumns.Select(x => x.AttributeName).ToList();

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
                    DataProvider.UserRepository.TableName,
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
                var request = await AuthenticatedRequest.GetAuthAsync();
                if (!request.IsAdminLoggin ||
                    !await request.AdminPermissionsImpl.HasSystemPermissionsAsync(Constants.AppPermissions.SettingsUserStyle))
                {
                    return Unauthorized();
                }

                var attributeName = request.GetPostString("attributeName");

                await DataProvider.TableStyleRepository.DeleteAsync(0, DataProvider.UserRepository.TableName, attributeName);

                var allAttributes = DataProvider.UserRepository.TableColumns.Select(x => x.AttributeName).ToList();

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
