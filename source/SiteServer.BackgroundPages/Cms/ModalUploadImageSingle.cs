using System;
using System.Collections.Specialized;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Model.Enumerations;
using SiteServer.CMS.Core;

namespace SiteServer.BackgroundPages.Cms
{
    public class ModalUploadImageSingle : BasePageCms
    {
        public HtmlInputFile hifUpload;
        public Literal ltlScript;

        private string _currentRootPath;
        private string _textBoxClientId;
        //是否需要水印（广告物料不需要）
        private bool _isNeedWaterMark = true;

        protected override bool IsSinglePage => true;

        public static string GetOpenWindowStringToTextBox(int publishmentSystemId, string textBoxClientId)
        {
            return PageUtils.GetOpenWindowString("上传图片", PageUtils.GetCmsUrl(nameof(ModalUploadImageSingle), new NameValueCollection
            {
                {"PublishmentSystemID", publishmentSystemId.ToString()},
                {"TextBoxClientID", textBoxClientId}
            }), 480, 220);
        }

        public static string GetOpenWindowStringToTextBox(int publishmentSystemId, string textBoxClientId, bool isNeedWaterMark)
        {
            return PageUtils.GetOpenWindowString("上传图片", PageUtils.GetCmsUrl(nameof(ModalUploadImageSingle), new NameValueCollection
            {
                {"PublishmentSystemID", publishmentSystemId.ToString()},
                {"TextBoxClientID", textBoxClientId},
                {"IsNeedWaterMark", isNeedWaterMark.ToString()}
            }), 480, 220);
        }

        public static string GetOpenWindowStringToList(int publishmentSystemId, string currentRootPath)
        {
            return PageUtils.GetOpenWindowString("上传图片", PageUtils.GetCmsUrl(nameof(ModalUploadImageSingle), new NameValueCollection
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
            _isNeedWaterMark = Body.GetQueryBool("IsNeedWaterMark", true);
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

                    if (!PathUtility.IsImageExtenstionAllowed(PublishmentSystemInfo, fileExtName))
                    {
                        FailMessage("上传失败，上传图片格式不正确！");
                        return;
                    }
                    if (!PathUtility.IsImageSizeAllowed(PublishmentSystemInfo, hifUpload.PostedFile.ContentLength))
                    {
                        FailMessage("上传失败，上传图片超出规定文件大小！");
                        return;
                    }

                    hifUpload.PostedFile.SaveAs(localFilePath);

                    var isImage = EFileSystemTypeUtils.IsImage(fileExtName);

                    if (isImage && _isNeedWaterMark)
                    {
                        FileUtility.AddWaterMark(PublishmentSystemInfo, localFilePath);
                    }

                    if (string.IsNullOrEmpty(_textBoxClientId))
                    {
                        PageUtils.CloseModalPage(Page);
                    }
                    else
                    {
                        var imageUrl = PageUtility.GetPublishmentSystemUrlByPhysicalPath(PublishmentSystemInfo, localFilePath);
                        var textBoxUrl = PageUtility.GetVirtualUrl(PublishmentSystemInfo, imageUrl);

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
                    FailMessage(ex, "图片上传失败！");
                }
            }
        }

    }
}
