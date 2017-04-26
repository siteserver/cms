using System;
using System.Collections.Specialized;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Model.Enumerations;
using SiteServer.CMS.Core;

namespace SiteServer.BackgroundPages.Cms
{
    public class ModalUploadFile : BasePageCms
    {
        public HtmlInputFile hifUpload;
        public RadioButtonList rblIsFileUploadChangeFileName;
        public Literal ltlScript;

        private EUploadType _uploadType;
        private string _realtedPath;
        private string _textBoxClientId;

        public static string GetOpenWindowStringToTextBox(int publishmentSystemId, EUploadType uploadType, string textBoxClientId)
        {
            return PageUtils.GetOpenWindowString("上传附件", PageUtils.GetCmsUrl(nameof(ModalUploadFile), new NameValueCollection
            {
                {"PublishmentSystemID", publishmentSystemId.ToString()},
                {"uploadType", EUploadTypeUtils.GetValue(uploadType)},
                {"TextBoxClientID", textBoxClientId}
            }), 480, 300);
        }

        public static string GetOpenWindowStringToList(int publishmentSystemId, EUploadType uploadType, string realtedPath)
        {
            return PageUtils.GetOpenWindowString("上传附件", PageUtils.GetCmsUrl(nameof(ModalUploadFile), new NameValueCollection
            {
                {"PublishmentSystemID", publishmentSystemId.ToString()},
                {"uploadType", EUploadTypeUtils.GetValue(uploadType)},
                {"realtedPath", realtedPath}
            }), 480, 300);
        }

		public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("PublishmentSystemID");
            _uploadType = EUploadTypeUtils.GetEnumType(Body.GetQueryString("uploadType"));
            _realtedPath = Body.GetQueryString("realtedPath");
            _textBoxClientId = Body.GetQueryString("TextBoxClientID");

            if (!IsPostBack)
            {
                EBooleanUtils.AddListItems(rblIsFileUploadChangeFileName, "采用系统生成文件名", "采用原有文件名");
                ControlUtils.SelectListItemsIgnoreCase(rblIsFileUploadChangeFileName, PublishmentSystemInfo.Additional.IsFileUploadChangeFileName.ToString());
            }
		}

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            if (hifUpload.PostedFile != null && "" != hifUpload.PostedFile.FileName)
            {
                var filePath = hifUpload.PostedFile.FileName;
                
				try
				{
                    var fileExtName = PathUtils.GetExtension(filePath).ToLower();
                    var localDirectoryPath = PathUtility.GetUploadDirectoryPath(PublishmentSystemInfo, fileExtName);
                    if (!string.IsNullOrEmpty(_realtedPath))
					{
                        localDirectoryPath = PathUtility.MapPath(PublishmentSystemInfo, _realtedPath);
                        DirectoryUtils.CreateDirectoryIfNotExists(localDirectoryPath);
					}
                    var localFileName = PathUtility.GetUploadFileName(PublishmentSystemInfo, filePath, DateTime.Now, TranslateUtils.ToBool(rblIsFileUploadChangeFileName.SelectedValue));

                    var localFilePath = PathUtils.Combine(localDirectoryPath, localFileName);

                    if (_uploadType == EUploadType.Image && !EFileSystemTypeUtils.IsImageOrFlashOrPlayer(fileExtName))
                    {
                        FailMessage("此格式不允许上传，此文件夹只允许上传图片以及音视频文件！");
                        return;
                    }
                    if (_uploadType == EUploadType.Video && !EFileSystemTypeUtils.IsImageOrFlashOrPlayer(fileExtName))
                    {
                        FailMessage("此格式不允许上传，此文件夹只允许上传图片以及音视频文件！");
                        return;
                    }
                    if (_uploadType == EUploadType.File && !PathUtility.IsFileExtenstionAllowed(PublishmentSystemInfo, fileExtName))
                    {
                        FailMessage("此格式不允许上传，请选择有效的文件！");
                        return;
                    }
                    
                    if (!PathUtility.IsFileSizeAllowed(PublishmentSystemInfo, hifUpload.PostedFile.ContentLength))
                    {
                        FailMessage("上传失败，上传文件超出规定文件大小！");
                        return;
                    }

                    hifUpload.PostedFile.SaveAs(localFilePath);

					FileUtility.AddWaterMark(PublishmentSystemInfo, localFilePath);

                    var fileUrl = PageUtility.GetPublishmentSystemUrlByPhysicalPath(PublishmentSystemInfo, localFilePath);
                    var textBoxUrl = PageUtility.GetVirtualUrl(PublishmentSystemInfo, fileUrl);

                    if (string.IsNullOrEmpty(_textBoxClientId))
                    {
                        PageUtils.CloseModalPage(Page);
                    }
                    else
                    {
                        ltlScript.Text += $@"
if (parent.document.getElementById('{_textBoxClientId}') != null)
{{
    parent.document.getElementById('{_textBoxClientId}').value = '{textBoxUrl}';
}}
";

                        ltlScript.Text += PageUtils.HidePopWin;
                    }
				}
				catch(Exception ex)
				{
                    FailMessage(ex, "文件上传失败");
				}
			}
		}

	}
}
