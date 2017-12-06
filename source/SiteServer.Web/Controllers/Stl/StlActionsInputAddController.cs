using System;
using System.Web;
using System.Web.Http;
using BaiRong.Core;
using BaiRong.Core.Model.Enumerations;
using SiteServer.CMS.Controllers.Stl;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;
using SiteServer.CMS.StlTemplates;

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

                    InputTypeParser.AddValuesToAttributes(ETableStyle.InputContent, DataProvider.InputContentDao.TableName, publishmentSystemInfo, relatedIdentities, HttpContext.Current.Request.Form, contentInfo.Attributes, false);

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

                    DataProvider.InputContentDao.Insert(contentInfo);

                    string message;
                    if (string.IsNullOrEmpty(HttpContext.Current.Request.Form["successTemplateString"]))
                    {
                        if (string.IsNullOrEmpty(inputInfo.Additional.MessageSuccess))
                        {
                            message = "表单提交成功，正在审核。";
                            if (contentInfo.IsChecked)
                            {
                                message = "表单提交成功。";
                            }
                        }
                        else
                        {
                            message = inputInfo.Additional.MessageSuccess;
                        }
                    }
                    else
                    {
                        message = TranslateUtils.DecryptStringBySecretKey(HttpContext.Current.Request.Form["successTemplateString"]);
                    }

                    HttpContext.Current.Response.Write(InputTemplate.GetInputCallbackScript(publishmentSystemInfo, inputId, true, message));
                    HttpContext.Current.Response.End();

                    //if (contentInfo.IsChecked == EBoolean.True)
                    //{
                    //    FileSystemObject FSO = new FileSystemObject(base.PublishmentSystemID);
                    //    FSO.CreateImmediately(EChangedType.Add, ETemplateTypeUtils.GetEnumType(templateType), channelID, contentID, fileTemplateID);
                    //}
                }
                catch (Exception ex)
                {
                    string message;
                    if (string.IsNullOrEmpty(HttpContext.Current.Request.Form["failureTemplateString"]))
                    {
                        if (string.IsNullOrEmpty(inputInfo.Additional.MessageFailure))
                        {
                            //message = "表单提交失败，" + ex.Message;
                            message = "表单提交失败，程序出错。";
                        }
                        else
                        {
                            message = inputInfo.Additional.MessageFailure;
                        }
                    }
                    else
                    {
                        message = TranslateUtils.DecryptStringBySecretKey(HttpContext.Current.Request.Form["failureTemplateString"]);
                    }

                    HttpContext.Current.Response.Write(InputTemplate.GetInputCallbackScript(publishmentSystemInfo, inputId, false, message));
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
