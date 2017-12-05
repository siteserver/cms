using System;
using System.Web;
using System.Web.Http; 
using BaiRong.Core;
using BaiRong.Core.Model.Attributes;
using SiteServer.CMS.Controllers.Stl;
using SiteServer.CMS.Core;
using SiteServer.CMS.StlTemplates;

namespace SiteServer.API.Controllers.Stl
{
    [RoutePrefix("api")]
    public class StlActionsGovInteractQueryAddController : ApiController
    {
        [HttpPost, Route(ActionsGovInteractQueryAdd.Route)]
        public void Main(int publishmentSystemId, int nodeId)
        {
            var publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(publishmentSystemId);

            try
            {
                var queryCode = PageUtils.FilterSqlAndXss(HttpContext.Current.Request.Form[GovInteractContentAttribute.QueryCode]);
                var contentInfo = DataProvider.GovInteractContentDao.GetContentInfo(publishmentSystemInfo, nodeId, queryCode);
                if (contentInfo != null)
                {
                    HttpContext.Current.Response.Write(GovInteractQueryTemplate.GetCallbackScript(publishmentSystemInfo, true, contentInfo, string.Empty));
                }
                else
                {
                    HttpContext.Current.Response.Write(GovInteractQueryTemplate.GetCallbackScript(publishmentSystemInfo, false, null, "您输入的查询号不正确"));
                }
            }
            catch (Exception ex)
            {
                //HttpContext.Current.Response.Write(GovInteractQueryTemplate.GetCallbackScript(publishmentSystemInfo, false, null, ex.Message));
                HttpContext.Current.Response.Write(GovInteractQueryTemplate.GetCallbackScript(publishmentSystemInfo, false, null, "程序错误"));
            }

            HttpContext.Current.Response.End();
        }
    }
}
