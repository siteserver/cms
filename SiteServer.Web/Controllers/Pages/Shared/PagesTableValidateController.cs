using System;
using System.Threading.Tasks;
using System.Web.Http;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.Abstractions;
using SiteServer.CMS.Repositories;

namespace SiteServer.API.Controllers.Pages.Shared
{
    
    [RoutePrefix("pages/shared/tableValidate")]
    public class PagesTableValidateController : ApiController
    {
        private const string Route = "";

        [HttpGet, Route(Route)]
        public async Task<IHttpActionResult> Get()
        {
            try
            {
                var request = await AuthenticatedRequest.GetAuthAsync();
                if (!request.IsAdminLoggin) return Unauthorized();

                var tableName = request.GetQueryString("tableName");
                var attributeName = request.GetQueryString("attributeName");
                var relatedIdentities = StringUtils.GetIntList(request.GetQueryString("relatedIdentities"));

                var style = await TableStyleManager.GetTableStyleAsync(tableName, attributeName, relatedIdentities);

                var veeValidate = string.Empty;
                if (style != null)
                {
                    veeValidate = style.VeeValidate;
                }

                return Ok(new
                {
                    Value = veeValidate
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost, Route(Route)]
        public async Task<IHttpActionResult> Submit()
        {
            try
            {
                var request = await AuthenticatedRequest.GetAuthAsync();
                if (!request.IsAdminLoggin) return Unauthorized();

                var tableName = request.GetPostString("tableName");
                var attributeName = request.GetPostString("attributeName");
                var relatedIdentities = StringUtils.GetIntList(request.GetPostString("relatedIdentities"));
                var value = request.GetPostString("value");

                var style =
                    await TableStyleManager.GetTableStyleAsync(tableName, attributeName, relatedIdentities);
                style.VeeValidate = value;

                //数据库中没有此项及父项的表样式 or 数据库中没有此项的表样式，但是有父项的表样式
                if (style.Id == 0 && style.RelatedIdentity == 0 || style.RelatedIdentity != relatedIdentities[0])
                {
                    await DataProvider.TableStyleRepository.InsertAsync(style);
                    await request.AddAdminLogAsync("添加表单显示样式", $"字段名:{style.AttributeName}");
                }
                //数据库中有此项的表样式
                else
                {
                    await DataProvider.TableStyleRepository.UpdateAsync(style, false);
                    await request.AddAdminLogAsync("修改表单显示样式", $"字段名:{style.AttributeName}");
                }

                return Ok(new{});
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
