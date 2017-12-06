using System;
using System.Web;
using System.Web.Http;
using SiteServer.CMS.Controllers.Stl;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;
using SiteServer.CMS.StlTemplates;
using SiteServer.CMS.Wcm.GovPublic;
using SiteServer.CMS.Wcm.Model;

namespace SiteServer.API.Controllers.Stl
{
    [RoutePrefix("api")]
    public class StlActionsGovPublicApplyAddController : ApiController
    {
        [HttpPost, Route(ActionsGovPublicApplyAdd.Route)]
        public void Main(int publishmentSystemId, int styleId)
        {
            var publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(publishmentSystemId);

            var tagStyleInfo = TagStyleManager.GetTagStyleInfo(styleId) ?? new TagStyleInfo();
            var tagStyleGovPublicApplyInfo = new TagStyleGovPublicApplyInfo(tagStyleInfo.SettingsXML);

            try
            {
                var applyInfo = DataProvider.GovPublicApplyDao.GetApplyInfo(publishmentSystemId, styleId, HttpContext.Current.Request.Form);

                var applyId = DataProvider.GovPublicApplyDao.Insert(applyInfo);

                var fromName = applyInfo.GetExtendedAttribute(GovPublicApplyAttribute.CivicName);
                if (applyInfo.IsOrganization)
                {
                    fromName = applyInfo.GetExtendedAttribute(GovPublicApplyAttribute.OrgName);
                }
                var toDepartmentName = string.Empty;
                if (applyInfo.DepartmentId > 0)
                {
                    toDepartmentName = "至" + applyInfo.DepartmentName;
                }
                GovPublicApplyManager.LogNew(publishmentSystemId, applyId, fromName, toDepartmentName);

                MessageManager.SendSMSByGovPublicApply(publishmentSystemInfo, tagStyleGovPublicApplyInfo, applyInfo);

                HttpContext.Current.Response.Write(GovPublicApplyTemplate.GetCallbackScript(publishmentSystemInfo, true, applyInfo.QueryCode, string.Empty));
                HttpContext.Current.Response.End();
            }
            catch (Exception ex)
            {
                //HttpContext.Current.Response.Write(GovPublicApplyTemplate.GetCallbackScript(publishmentSystemInfo, false, string.Empty, ex.Message));
                HttpContext.Current.Response.Write(GovPublicApplyTemplate.GetCallbackScript(publishmentSystemInfo, false, string.Empty, "程序错误"));
                HttpContext.Current.Response.End();
            }
        }
    }
}
