using System;
using System.Collections.Specialized;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using SiteServer.Utils;
using SiteServer.CMS.Core;

namespace SiteServer.BackgroundPages.Cms
{
    public class ModalUploadVideo : BasePageCms
    {
        public HtmlInputFile HifUpload;
        public Literal LtlScript;

        private string _currentRootPath;
        private string _textBoxClientId;

        public static string GetOpenWindowStringToTextBox(int siteId, string textBoxClientId)
        {
            return LayerUtils.GetOpenScript("上传视频", PageUtils.GetCmsUrl(siteId, nameof(ModalUploadVideo), new NameValueCollection
            {
                {"TextBoxClientID", textBoxClientId}
            }), 520, 220);
        }

        public static string GetOpenWindowStringToList(int siteId, string currentRootPath)
        {
            return LayerUtils.GetOpenScript("上传视频", PageUtils.GetCmsUrl(siteId, nameof(ModalUploadVideo), new NameValueCollection
            {
                {"CurrentRootPath", currentRootPath}
            }), 520, 220);
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("siteId");
            _currentRootPath = AuthRequest.GetQueryString("CurrentRootPath");
            if (!string.IsNullOrEmpty(_currentRootPath) && !_currentRootPath.StartsWith("@"))
            {
                _currentRootPath = "@/" + _currentRootPath;
            }
            _textBoxClientId = AuthRequest.GetQueryString("TextBoxClientID");
        }

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            if (HifUpload.PostedFile == null || "" == HifUpload.PostedFile.FileName) return;

            var filePath = HifUpload.PostedFile.FileName;
            try
            {
                var fileExtName = PathUtils.GetExtension(filePath).ToLower();
                var localDirectoryPath = PathUtility.GetUploadDirectoryPath(SiteInfo, fileExtName);
                if (!string.IsNullOrEmpty(_currentRootPath))
                {
                    localDirectoryPath = PathUtility.MapPath(SiteInfo, _currentRootPath);
                    DirectoryUtils.CreateDirectoryIfNotExists(localDirectoryPath);
                }
                var localFileName = PathUtility.GetUploadFileName(SiteInfo, filePath);
                var localFilePath = PathUtils.Combine(localDirectoryPath, localFileName);

                if (!PathUtility.IsVideoExtenstionAllowed(SiteInfo, fileExtName))
                {
                    FailMessage("上传失败，上传视频格式不正确！");
                    return;
                }
                if (!PathUtility.IsVideoSizeAllowed(SiteInfo, HifUpload.PostedFile.ContentLength))
                {
                    FailMessage("上传失败，上传视频超出规定文件大小！");
                    return;
                }

                HifUpload.PostedFile.SaveAs(localFilePath);

                var videoUrl = PageUtility.GetSiteUrlByPhysicalPath(SiteInfo, localFilePath, true);
                var textBoxUrl = PageUtility.GetVirtualUrl(SiteInfo, videoUrl);

                if (string.IsNullOrEmpty(_textBoxClientId))
                {
                    LayerUtils.Close(Page);
                }
                else
                {
                    LtlScript.Text = $@"
<script type=""text/javascript"" language=""javascript"">
    if (parent.document.getElementById('{_textBoxClientId}') != null)
    {{
        parent.document.getElementById('{_textBoxClientId}').value = '{textBoxUrl}';
    }}
    {LayerUtils.CloseScript}
</script>";
                }
            }
            catch (Exception ex)
            {
                FailMessage(ex, "视频上传失败！");
            }
        }

    }
}
