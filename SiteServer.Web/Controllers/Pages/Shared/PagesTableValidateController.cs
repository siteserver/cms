using System;
using System.Web.Http;
using SiteServer.CMS.Caches;
using SiteServer.CMS.Database.Core;
using SiteServer.Utils;

namespace SiteServer.API.Controllers.Pages.Shared
{
    [RoutePrefix("pages/shared/tableValidate")]
    public class PagesTableValidateController : ApiController
    {
        private const string Route = "";

        [HttpGet, Route(Route)]
        public IHttpActionResult Get()
        {
            try
            {
                var rest = new Rest(Request);
                if (!rest.IsAdminLoggin) return Unauthorized();

                var tableName = rest.GetQueryString("tableName");
                var attributeName = rest.GetQueryString("attributeName");
                var relatedIdentities = TranslateUtils.StringCollectionToIntList(rest.GetQueryString("relatedIdentities"));

                var styleInfo = TableStyleManager.GetTableStyleInfo(tableName, attributeName, relatedIdentities);

                var veeValidate = string.Empty;
                if (styleInfo != null)
                {
                    veeValidate = styleInfo.VeeValidate;
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
        public IHttpActionResult Submit()
        {
            try
            {
                var rest = new Rest(Request);
                if (!rest.IsAdminLoggin) return Unauthorized();

                var tableName = rest.GetPostString("tableName");
                var attributeName = rest.GetPostString("attributeName");
                var relatedIdentities = TranslateUtils.StringCollectionToIntList(rest.GetPostString("relatedIdentities"));
                var value = rest.GetPostString("value");

                var styleInfo =
                    TableStyleManager.GetTableStyleInfo(tableName, attributeName, relatedIdentities);
                styleInfo.VeeValidate = value;

                //数据库中没有此项及父项的表样式 or 数据库中没有此项的表样式，但是有父项的表样式
                if (styleInfo.Id == 0 && styleInfo.RelatedIdentity == 0 || styleInfo.RelatedIdentity != relatedIdentities[0])
                {
                    DataProvider.TableStyle.Insert(styleInfo);
                    rest.AddAdminLog("添加表单显示样式", $"字段名:{styleInfo.AttributeName}");
                }
                //数据库中有此项的表样式
                else
                {
                    DataProvider.TableStyle.Update(styleInfo, false);
                    rest.AddAdminLog("修改表单显示样式", $"字段名:{styleInfo.AttributeName}");
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
