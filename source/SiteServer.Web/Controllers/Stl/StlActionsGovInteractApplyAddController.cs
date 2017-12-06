using System;
using System.Web;
using System.Web.Http;
using BaiRong.Core;
using BaiRong.Core.Model.Enumerations;
using SiteServer.CMS.Controllers.Stl;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;
using SiteServer.CMS.StlTemplates;
using SiteServer.CMS.Wcm.GovInteract;

namespace SiteServer.API.Controllers.Stl
{
    [RoutePrefix("api")]
    public class StlActionsGovInteractApplyAddController : ApiController
    {
        [HttpPost, Route(ActionsGovInteractApplyAdd.Route)]
        public void Main(int publishmentSystemId, int nodeId, int styleId)
        {
            var body = new RequestBody();

            var publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(publishmentSystemId);

            var tagStyleInfo = TagStyleManager.GetTagStyleInfo(styleId) ?? new TagStyleInfo();
            var tagStyleGovInteractApplyInfo = new TagStyleGovInteractApplyInfo(tagStyleInfo.SettingsXML);

            try
            {
                if (!tagStyleGovInteractApplyInfo.IsAnomynous && !body.IsUserLoggin)
                {
                    throw new Exception("请先登录系统!");
                }

                var contentInfo = DataProvider.GovInteractContentDao.GetContentInfo(publishmentSystemInfo, nodeId, HttpContext.Current.Request.Form);

                if (HttpContext.Current.Request.Files.Count > 0)
                {
                    foreach (var attributeName in HttpContext.Current.Request.Files.AllKeys)
                    {
                        var myFile = HttpContext.Current.Request.Files[attributeName];
                        if (myFile != null && "" != myFile.FileName)
                        {
                            var fileUrl = UploadFile(publishmentSystemInfo, myFile);
                            contentInfo.SetExtendedAttribute(attributeName, fileUrl);
                        }
                    }
                }

                var contentId = DataProvider.ContentDao.Insert(publishmentSystemInfo.AuxiliaryTableForGovInteract, publishmentSystemInfo, contentInfo);

                var realName = contentInfo.RealName;
                var toDepartmentName = string.Empty;
                if (contentInfo.DepartmentId > 0)
                {
                    toDepartmentName = "至" + contentInfo.DepartmentName;
                }
                GovInteractApplyManager.LogNew(publishmentSystemId, nodeId, contentId, realName, toDepartmentName);

                MessageManager.SendSMS(publishmentSystemInfo, tagStyleGovInteractApplyInfo, ETableStyle.GovInteractContent, publishmentSystemInfo.AuxiliaryTableForGovInteract, nodeId, contentInfo);

                HttpContext.Current.Response.Write(GovInteractApplyTemplate.GetCallbackScript(publishmentSystemInfo, nodeId, true, contentInfo.QueryCode, string.Empty));
            }
            catch (Exception ex)
            {
                //HttpContext.Current.Response.Write(GovInteractApplyTemplate.GetCallbackScript(publishmentSystemInfo, nodeId, false, string.Empty, ex.Message));
                HttpContext.Current.Response.Write(GovInteractApplyTemplate.GetCallbackScript(publishmentSystemInfo, nodeId, false, string.Empty, "程序错误"));
            }

            HttpContext.Current.Response.End();
        }

        private static string UploadFile(PublishmentSystemInfo publishmentSystemInfo, HttpPostedFile myFile)
        {
            var fileUrl = string.Empty;

            var filePath = myFile.FileName;
            try
            {
                var fileExtName = PathUtils.GetExtension(filePath);
                var localDirectoryPath = PathUtility.GetUploadDirectoryPath(publishmentSystemInfo, fileExtName);
                var localFileName = PathUtility.GetUploadFileName(publishmentSystemInfo, filePath);

                var localFilePath = PathUtils.Combine(localDirectoryPath, localFileName);

                if (!PathUtility.IsFileExtenstionAllowed(publishmentSystemInfo, fileExtName))
                {
                    return string.Empty;
                }
                if (!PathUtility.IsFileSizeAllowed(publishmentSystemInfo, myFile.ContentLength))
                {
                    return string.Empty;
                }

                myFile.SaveAs(localFilePath);
                FileUtility.AddWaterMark(publishmentSystemInfo, localFilePath);

                fileUrl = PageUtility.GetPublishmentSystemUrlByPhysicalPath(publishmentSystemInfo, localFilePath);
                fileUrl = PageUtility.GetVirtualUrl(publishmentSystemInfo, fileUrl);
            }
            catch
            {
                // ignored
            }

            return fileUrl;
        }
    }
}
