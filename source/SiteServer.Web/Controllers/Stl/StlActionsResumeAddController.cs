using System;
using System.Web;
using System.Web.Http;
using BaiRong.Core;
using SiteServer.CMS.Controllers.Stl;
using SiteServer.CMS.Core;
using SiteServer.CMS.StlTemplates;

namespace SiteServer.API.Controllers.Stl
{
    [RoutePrefix("api")]
    public class StlActionsResumeAddController : ApiController
    {
        [HttpPost, Route(ActionsResumeAdd.Route)]
        public void Main(int publishmentSystemId)
        {
            var publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(publishmentSystemId);
            try
            {
                var contentInfo = DataProvider.ResumeContentDao.GetContentInfo(publishmentSystemId, HttpContext.Current.Request.Form);

                DataProvider.ResumeContentDao.Insert(contentInfo);

                string message;

                if (string.IsNullOrEmpty(HttpContext.Current.Request.Form["successTemplateString"]))
                {
                    message = "简历添加成功。";
                }
                else
                {
                    message = TranslateUtils.DecryptStringBySecretKey(HttpContext.Current.Request.Form["successTemplateString"]);
                }

                HttpContext.Current.Response.Write(ResumeTemplate.GetCallbackScript(publishmentSystemInfo, true, message));
                HttpContext.Current.Response.End();
            }
            catch (Exception ex)
            {
                string message;

                if (string.IsNullOrEmpty(HttpContext.Current.Request.Form["failureTemplateString"]))
                {
                    //message = "简历添加失败，" + ex.Message;
                    message = "简历添加失败，程序出错。";
                }
                else
                {
                    message = TranslateUtils.DecryptStringBySecretKey(HttpContext.Current.Request.Form["failureTemplateString"]);
                }

                HttpContext.Current.Response.Write(ResumeTemplate.GetCallbackScript(publishmentSystemInfo, false, message));
                HttpContext.Current.Response.End();
            }
        }
    }
}
