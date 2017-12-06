using System;
using System.Web;
using System.Web.Http;
using BaiRong.Core;
using SiteServer.CMS.Controllers.Stl;
using SiteServer.CMS.Core;
using SiteServer.CMS.StlTemplates;
using SiteServer.CMS.Wcm.Model;

namespace SiteServer.API.Controllers.Stl
{
    [RoutePrefix("api")]
    public class StlActionsGovPublicQueryAddController : ApiController
    {
        [HttpPost, Route(ActionsGovPublicQueryAdd.Route)]
        public void Main(int publishmentSystemId, int styleId)
        {
            var publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(publishmentSystemId);

            try
            {
                var isOrganization = TranslateUtils.ToBool(HttpContext.Current.Request.Form[GovPublicApplyAttribute.IsOrganization]);
                var queryName = PageUtils.FilterSqlAndXss(HttpContext.Current.Request.Form["queryName"]);
                var queryCode = PageUtils.FilterSqlAndXss(HttpContext.Current.Request.Form[GovPublicApplyAttribute.QueryCode]);
                var applyInfo = DataProvider.GovPublicApplyDao.GetApplyInfo(publishmentSystemId, isOrganization, queryName, queryCode);
                if (applyInfo != null)
                {
                    HttpContext.Current.Response.Write(GovPublicQueryTemplate.GetCallbackScript(publishmentSystemInfo, true, applyInfo, string.Empty));
                }
                else
                {
                    HttpContext.Current.Response.Write(GovPublicQueryTemplate.GetCallbackScript(publishmentSystemInfo, false, null, "系统找不到对应的申请，请确认您的输入值是否正确"));
                }
            }
            catch (Exception ex)
            {
                //HttpContext.Current.Response.Write(GovPublicQueryTemplate.GetCallbackScript(publishmentSystemInfo, false, null, ex.Message));
                HttpContext.Current.Response.Write(GovPublicQueryTemplate.GetCallbackScript(publishmentSystemInfo, false, null, "程序错误"));
            }

            HttpContext.Current.Response.End();
        }
    }
}
