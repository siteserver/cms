using System;
using System.Collections.Specialized;
using System.Web;
using System.Web.Http;
using BaiRong.Core;
using BaiRong.Core.AuxiliaryTable;
using BaiRong.Core.Model.Attributes;
using BaiRong.Core.Model.Enumerations;
using SiteServer.CMS.Controllers.Stl;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;
using SiteServer.CMS.StlParser.StlElement;

namespace SiteServer.API.Controllers.Stl
{
    [RoutePrefix("api")]
    public class StlActionsInputAddController : ApiController
    {
        [HttpPost, Route(ActionsInputAdd.Route)]
        public void Main(int publishmentSystemId, int inputId)
        {
            var body = new RequestBody();

            var publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(publishmentSystemId);
            InputInfo inputInfo = null;
            if (inputId > 0)
            {
                inputInfo = DataProvider.InputDao.GetInputInfo(inputId);
            }
            if (inputInfo != null)
            {
                var relatedIdentities = RelatedIdentities.GetRelatedIdentities(ETableStyle.InputContent, publishmentSystemId, inputInfo.InputId);

                var ipAddress = PageUtils.GetIpAddress();

                var contentInfo = new InputContentInfo(0, inputInfo.InputId, 0, inputInfo.IsChecked, body.UserName, ipAddress, DateTime.Now, string.Empty);

                try
                {
                    if (!inputInfo.Additional.IsAnomynous && !body.IsUserLoggin)
                    {
                        throw new Exception("请先登录系统!");
                    }

                    InputTypeParser.AddValuesToAttributes(ETableStyle.InputContent, DataProvider.InputContentDao.TableName, publishmentSystemInfo, relatedIdentities, HttpContext.Current.Request.Form, contentInfo.NameValues, false);

                    if (HttpContext.Current.Request.Files.Count > 0)
                    {
                        foreach (var attributeName in HttpContext.Current.Request.Files.AllKeys)
                        {
                            var myFile = HttpContext.Current.Request.Files[attributeName];
                            if (myFile == null || "" == myFile.FileName) continue;

                            var fileUrl = UploadFile(publishmentSystemInfo, myFile);
                            contentInfo.SetExtendedAttribute(attributeName, fileUrl);
                        }
                    }

                    contentInfo.Id = DataProvider.InputContentDao.Insert(contentInfo);

                    if (inputInfo.Additional.IsAdministratorSmsNotify)
                    {
                        var keys =
                            TranslateUtils.StringCollectionToStringList(inputInfo.Additional.AdministratorSmsNotifyKeys);
                        if (keys.Count > 0)
                        {
                            var parameters = new NameValueCollection();
                            if (keys.Contains(InputContentAttribute.Id))
                            {
                                parameters.Add(InputContentAttribute.Id, contentInfo.Id.ToString());
                            }
                            if (keys.Contains(InputContentAttribute.AddDate))
                            {
                                parameters.Add(InputContentAttribute.AddDate, DateUtils.GetDateAndTimeString(contentInfo.AddDate));
                            }
                            var styleInfoList = TableStyleManager.GetTableStyleInfoList(ETableStyle.InputContent, DataProvider.InputContentDao.TableName, relatedIdentities);
                            foreach (var styleInfo in styleInfoList)
                            {
                                if (keys.Contains(styleInfo.AttributeName))
                                {
                                    var value = contentInfo.GetExtendedAttribute(styleInfo.AttributeName);
                                    parameters.Add(styleInfo.AttributeName, value);
                                }
                            }

                            string errorMessage;
                            SmsManager.SendNotify(inputInfo.Additional.AdministratorSmsNotifyMobile,
                                inputInfo.Additional.AdministratorSmsNotifyTplId, parameters, out errorMessage);
                        }
                    }

                    HttpContext.Current.Response.Write(StlInput.GetPostMessageScript(inputId, true));
                    HttpContext.Current.Response.End();
                }
                catch (Exception)
                {
                    HttpContext.Current.Response.Write(StlInput.GetPostMessageScript(inputId, false));
                    HttpContext.Current.Response.End();
                }
            }
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
