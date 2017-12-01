using System;
using System.Collections.Specialized;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using BaiRong.Core;
using SiteServer.CMS.Core;

namespace SiteServer.BackgroundPages.Cms
{
    public class ModalUploadVideo : BasePageCms
    {
        public HtmlInputFile hifUpload;
        public Literal ltlScript;

        private string _currentRootPath;
        private string _textBoxClientId;

        public static string GetOpenWindowStringToTextBox(int publishmentSystemId, string textBoxClientId)
        {
            return PageUtils.GetOpenWindowString("上传视频", PageUtils.GetCmsUrl(nameof(ModalUploadVideo), new NameValueCollection
            {
                {"PublishmentSystemID", publishmentSystemId.ToString()},
                {"TextBoxClientID", textBoxClientId}
            }), 480, 220);
        }

        public static string GetOpenWindowStringToList(int publishmentSystemId, string currentRootPath)
        {
            return PageUtils.GetOpenWindowString("上传视频", PageUtils.GetCmsUrl(nameof(ModalUploadVideo), new NameValueCollection
            {
                {"PublishmentSystemID", publishmentSystemId.ToString()},
                {"CurrentRootPath", currentRootPath}
            }), 480, 220);
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("PublishmentSystemID");
            _currentRootPath = Body.GetQueryString("CurrentRootPath");
            if (!string.IsNullOrEmpty(_currentRootPath) && !_currentRootPath.StartsWith("@"))
            {
                _currentRootPath = "@/" + _currentRootPath;
            }
            _textBoxClientId = Body.GetQueryString("TextBoxClientID");
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
                    if (!string.IsNullOrEmpty(_currentRootPath))
                    {
                        localDirectoryPath = PathUtility.MapPath(PublishmentSystemInfo, _currentRootPath);
                        DirectoryUtils.CreateDirectoryIfNotExists(localDirectoryPath);
                    }
                    var localFileName = PathUtility.GetUploadFileName(PublishmentSystemInfo, filePath);
                    var localFilePath = PathUtils.Combine(localDirectoryPath, localFileName);

                    if (!PathUtility.IsVideoExtenstionAllowed(PublishmentSystemInfo, fileExtName))
                    {
                        FailMessage("上传失败，上传视频格式不正确！");
                        return;
                    }
                    if (!PathUtility.IsVideoSizeAllowed(PublishmentSystemInfo, hifUpload.PostedFile.ContentLength))
                    {
                        FailMessage("上传失败，上传视频超出规定文件大小！");
                        return;
                    }

                    hifUpload.PostedFile.SaveAs(localFilePath);

                    var videoUrl = PageUtility.GetPublishmentSystemUrlByPhysicalPath(PublishmentSystemInfo, localFilePath);
                    var textBoxUrl = PageUtility.GetVirtualUrl(PublishmentSystemInfo, videoUrl);

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
                catch (Exception ex)
                {
                    FailMessage(ex, "视频上传失败！");
                }
            }
        }

    }
}
