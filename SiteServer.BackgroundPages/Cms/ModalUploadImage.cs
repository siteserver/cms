using System;
using System.Collections.Specialized;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Images;
using BaiRong.Core.Model.Enumerations;
using SiteServer.CMS.Core;

namespace SiteServer.BackgroundPages.Cms
{
    public class ModalUploadImage : BasePageCms
    {
        public HtmlInputFile HifUpload;

        public CheckBox CbIsTitleImage;
        public TextBox TbTitleImageWidth;
        public TextBox TbTitleImageHeight;

        public CheckBox CbIsShowImageInTextEditor;
        public CheckBox CbIsLinkToOriginal;
        public CheckBox CbIsSmallImage;
        public TextBox TbSmallImageWidth;
        public TextBox TbSmallImageHeight;

        public Literal LtlScript;

        private string _textBoxClientId;

        public static string GetOpenWindowString(int publishmentSystemId, string textBoxClientId)
        {
            return PageUtils.GetOpenLayerString("上传图片", PageUtils.GetCmsUrl(nameof(ModalUploadImage), new NameValueCollection
            {
                {"publishmentSystemID", publishmentSystemId.ToString()},
                {"textBoxClientID", textBoxClientId}
            }), 600, 560);
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("PublishmentSystemID");
            _textBoxClientId = Body.GetQueryString("TextBoxClientID");

            if (IsPostBack) return;

            ConfigSettings(true);

            CbIsTitleImage.Attributes.Add("onclick", "checkBoxChange();");
            CbIsShowImageInTextEditor.Attributes.Add("onclick", "checkBoxChange();");
            CbIsSmallImage.Attributes.Add("onclick", "checkBoxChange();");
        }

        private void ConfigSettings(bool isLoad)
        {
            if (isLoad)
            {
                if (!string.IsNullOrEmpty(PublishmentSystemInfo.Additional.ConfigUploadImageIsTitleImage))
                {
                    CbIsTitleImage.Checked = TranslateUtils.ToBool(PublishmentSystemInfo.Additional.ConfigUploadImageIsTitleImage);
                }
                if (!string.IsNullOrEmpty(PublishmentSystemInfo.Additional.ConfigUploadImageTitleImageWidth))
                {
                    TbTitleImageWidth.Text = PublishmentSystemInfo.Additional.ConfigUploadImageTitleImageWidth;
                }
                if (!string.IsNullOrEmpty(PublishmentSystemInfo.Additional.ConfigUploadImageTitleImageHeight))
                {
                    TbTitleImageHeight.Text = PublishmentSystemInfo.Additional.ConfigUploadImageTitleImageHeight;
                }

                if (!string.IsNullOrEmpty(PublishmentSystemInfo.Additional.ConfigUploadImageIsShowImageInTextEditor))
                {
                    CbIsShowImageInTextEditor.Checked = TranslateUtils.ToBool(PublishmentSystemInfo.Additional.ConfigUploadImageIsShowImageInTextEditor);
                }
                if (!string.IsNullOrEmpty(PublishmentSystemInfo.Additional.ConfigUploadImageIsLinkToOriginal))
                {
                    CbIsLinkToOriginal.Checked = TranslateUtils.ToBool(PublishmentSystemInfo.Additional.ConfigUploadImageIsLinkToOriginal);
                }
                if (!string.IsNullOrEmpty(PublishmentSystemInfo.Additional.ConfigUploadImageIsSmallImage))
                {
                    CbIsSmallImage.Checked = TranslateUtils.ToBool(PublishmentSystemInfo.Additional.ConfigUploadImageIsSmallImage);
                }
                if (!string.IsNullOrEmpty(PublishmentSystemInfo.Additional.ConfigUploadImageSmallImageWidth))
                {
                    TbSmallImageWidth.Text = PublishmentSystemInfo.Additional.ConfigUploadImageSmallImageWidth;
                }
                if (!string.IsNullOrEmpty(PublishmentSystemInfo.Additional.ConfigUploadImageSmallImageHeight))
                {
                    TbSmallImageHeight.Text = PublishmentSystemInfo.Additional.ConfigUploadImageSmallImageHeight;
                }
            }
            else
            {
                PublishmentSystemInfo.Additional.ConfigUploadImageIsTitleImage = CbIsTitleImage.Checked.ToString();
                PublishmentSystemInfo.Additional.ConfigUploadImageTitleImageWidth = TbTitleImageWidth.Text;
                PublishmentSystemInfo.Additional.ConfigUploadImageTitleImageHeight = TbTitleImageHeight.Text;

                PublishmentSystemInfo.Additional.ConfigUploadImageIsShowImageInTextEditor = CbIsShowImageInTextEditor.Checked.ToString();
                PublishmentSystemInfo.Additional.ConfigUploadImageIsLinkToOriginal = CbIsLinkToOriginal.Checked.ToString();
                PublishmentSystemInfo.Additional.ConfigUploadImageIsSmallImage = CbIsSmallImage.Checked.ToString();
                PublishmentSystemInfo.Additional.ConfigUploadImageSmallImageWidth = TbSmallImageWidth.Text;
                PublishmentSystemInfo.Additional.ConfigUploadImageSmallImageHeight = TbSmallImageHeight.Text;

                DataProvider.PublishmentSystemDao.Update(PublishmentSystemInfo);
            }
        }

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            if (CbIsTitleImage.Checked && string.IsNullOrEmpty(TbTitleImageWidth.Text) && string.IsNullOrEmpty(TbTitleImageHeight.Text))
            {
                FailMessage("缩略图尺寸不能为空！");
                return;
            }
            if (CbIsSmallImage.Checked && string.IsNullOrEmpty(TbSmallImageWidth.Text) && string.IsNullOrEmpty(TbSmallImageHeight.Text))
            {
                FailMessage("缩略图尺寸不能为空！");
                return;
            }

            ConfigSettings(false);

            if (HifUpload.PostedFile == null || "" == HifUpload.PostedFile.FileName) return;

            var filePath = HifUpload.PostedFile.FileName;
            try
            {
                var fileExtName = PathUtils.GetExtension(filePath).ToLower();
                var localDirectoryPath = PathUtility.GetUploadDirectoryPath(PublishmentSystemInfo, fileExtName);
                var localFileName = PathUtility.GetUploadFileName(PublishmentSystemInfo, filePath);
                var localTitleFileName = StringUtils.Constants.TitleImageAppendix + localFileName;
                var localSmallFileName = StringUtils.Constants.SmallImageAppendix + localFileName;
                var localFilePath = PathUtils.Combine(localDirectoryPath, localFileName);
                var localTitleFilePath = PathUtils.Combine(localDirectoryPath, localTitleFileName);
                var localSmallFilePath = PathUtils.Combine(localDirectoryPath, localSmallFileName);

                if (!PathUtility.IsImageExtenstionAllowed(PublishmentSystemInfo, fileExtName))
                {
                    FailMessage("上传失败，上传图片格式不正确！");
                    return;
                }
                if (!PathUtility.IsImageSizeAllowed(PublishmentSystemInfo, HifUpload.PostedFile.ContentLength))
                {
                    FailMessage("上传失败，上传图片超出规定文件大小！");
                    return;
                }

                HifUpload.PostedFile.SaveAs(localFilePath);

                var isImage = EFileSystemTypeUtils.IsImage(fileExtName);

                //处理上半部分
                if (isImage)
                {
                    FileUtility.AddWaterMark(PublishmentSystemInfo, localFilePath);
                    if (CbIsTitleImage.Checked)
                    {
                        var width = TranslateUtils.ToInt(TbTitleImageWidth.Text);
                        var height = TranslateUtils.ToInt(TbTitleImageHeight.Text);
                        ImageUtils.MakeThumbnail(localFilePath, localTitleFilePath, width, height, true);
                    }
                }

                var imageUrl = PageUtility.GetPublishmentSystemUrlByPhysicalPath(PublishmentSystemInfo, localFilePath, true);
                if (CbIsTitleImage.Checked)
                {
                    imageUrl = PageUtility.GetPublishmentSystemUrlByPhysicalPath(PublishmentSystemInfo, localTitleFilePath, true);
                }

                var textBoxUrl = PageUtility.GetVirtualUrl(PublishmentSystemInfo, imageUrl);

                var script = $@"
if (parent.document.getElementById('{_textBoxClientId}'))
{{
    parent.document.getElementById('{_textBoxClientId}').value = '{textBoxUrl}';
}}
";

                //处理下半部分
                if (CbIsShowImageInTextEditor.Checked && isImage)
                {
                    imageUrl = PageUtility.GetPublishmentSystemUrlByPhysicalPath(PublishmentSystemInfo, localFilePath, true);
                    var smallImageUrl = imageUrl;
                    if (CbIsSmallImage.Checked)
                    {
                        smallImageUrl = PageUtility.GetPublishmentSystemUrlByPhysicalPath(PublishmentSystemInfo, localSmallFilePath, true);
                    }

                    if (CbIsSmallImage.Checked)
                    {
                        var width = TranslateUtils.ToInt(TbSmallImageWidth.Text);
                        var height = TranslateUtils.ToInt(TbSmallImageHeight.Text);
                        ImageUtils.MakeThumbnail(localFilePath, localSmallFilePath, width, height, true);
                    }

                    var insertHtml = CbIsLinkToOriginal.Checked ? $@"<a href=""{imageUrl}"" target=""_blank""><img src=""{smallImageUrl}"" border=""0"" /></a>" : $@"<img src=""{smallImageUrl}"" border=""0"" />";

                    script += "if(parent." + ETextEditorTypeUtils.GetEditorInstanceScript() + ") parent." + ETextEditorTypeUtils.GetInsertHtmlScript("Content", insertHtml);
                }

                LtlScript.Text = $@"
<script type=""text/javascript"" language=""javascript"">
    {script}
    {PageUtils.HidePopWin}
</script>";
            }
            catch (Exception ex)
            {
                FailMessage(ex, ex.Message);
            }
        }
    }
}
