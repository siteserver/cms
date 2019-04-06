using System;
using System.Collections.Generic;
using System.Web.Http;
using SiteServer.CMS.Caches;
using SiteServer.CMS.Core;
using SiteServer.CMS.Database.Attributes;
using SiteServer.CMS.Database.Core;
using SiteServer.Utils;

namespace SiteServer.API.Controllers.Pages.Settings
{
    [RoutePrefix("pages/settings/userStyle")]
    public class PagesUserStyleController : ApiController
    {
        private const string Route = "";

        [HttpGet, Route(Route)]
        public IHttpActionResult Get()
        {
            try
            {
                var rest = new Rest(Request);
                if (!rest.IsAdminLoggin ||
                    !rest.AdminPermissionsImpl.HasSystemPermissions(ConfigManager.SettingsPermissions.User))
                {
                    return Unauthorized();
                }

                var list = new List<object>();
                foreach (var styleInfo in TableStyleManager.GetUserStyleInfoList())
                {
                    list.Add(new
                    {
                        styleInfo.Id,
                        styleInfo.AttributeName,
                        styleInfo.DisplayName,
                        InputType = InputTypeUtils.GetText(styleInfo.Type),
                        Validate = styleInfo.VeeValidate,
                        styleInfo.Taxis,
                        IsSystem = StringUtils.ContainsIgnoreCase(UserAttribute.AllAttributes.Value, styleInfo.AttributeName)
                    });
                }

                return Ok(new
                {
                    Value = list,
                    DataProvider.User.TableName,
                    RelatedIdentities = TableStyleManager.EmptyRelatedIdentities
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpDelete, Route(Route)]
        public IHttpActionResult Delete()
        {
            try
            {
                var rest = new Rest(Request);
                if (!rest.IsAdminLoggin ||
                    !rest.AdminPermissionsImpl.HasSystemPermissions(ConfigManager.SettingsPermissions.Admin))
                {
                    return Unauthorized();
                }

                var attributeName = rest.GetQueryString("attributeName");

                DataProvider.TableStyle.Delete(0, DataProvider.User.TableName, attributeName);

                var list = new List<object>();
                foreach (var styleInfo in TableStyleManager.GetUserStyleInfoList())
                {
                    list.Add(new
                    {
                        styleInfo.Id,
                        styleInfo.AttributeName,
                        styleInfo.DisplayName,
                        InputType = InputTypeUtils.GetText(styleInfo.Type),
                        Validate = TableStyleManager.GetValidateInfo(styleInfo),
                        styleInfo.Taxis,
                        IsSystem = StringUtils.ContainsIgnoreCase(UserAttribute.AllAttributes.Value, styleInfo.AttributeName)
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
