using System;
using System.Web.Http;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Plugin.Impl;
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
                var request = new RequestImpl();
                if (!request.IsAdminLoggin) return Unauthorized();

                var tableName = request.GetQueryString("tableName");
                var attributeName = request.GetQueryString("attributeName");
                var relatedIdentities = TranslateUtils.StringCollectionToIntList(request.GetQueryString("relatedIdentities"));

                var styleInfo = TableStyleManager.GetTableStyleInfo(tableName, attributeName, relatedIdentities);

                var veeValidate = string.Empty;
                if (styleInfo != null)
                {
                    veeValidate = styleInfo.Additional.VeeValidate;
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
                var request = new RequestImpl();
                if (!request.IsAdminLoggin) return Unauthorized();

                var tableName = request.GetPostString("tableName");
                var attributeName = request.GetPostString("attributeName");
                var relatedIdentities = TranslateUtils.StringCollectionToIntList(request.GetPostString("relatedIdentities"));
                var value = request.GetPostString("value");

                var styleInfo =
                    TableStyleManager.GetTableStyleInfo(tableName, attributeName, relatedIdentities);
                styleInfo.Additional.VeeValidate = value;

                //数据库中没有此项及父项的表样式 or 数据库中没有此项的表样式，但是有父项的表样式
                if (styleInfo.Id == 0 && styleInfo.RelatedIdentity == 0 || styleInfo.RelatedIdentity != relatedIdentities[0])
                {
                    DataProvider.TableStyleDao.Insert(styleInfo);
                    request.AddAdminLog("添加表单显示样式", $"字段名:{styleInfo.AttributeName}");
                }
                //数据库中有此项的表样式
                else
                {
                    DataProvider.TableStyleDao.Update(styleInfo, false);
                    request.AddAdminLog("修改表单显示样式", $"字段名:{styleInfo.AttributeName}");
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
