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
        public HtmlInputFile hifUpload;

        public CheckBox cbIsTitleImage;
        public TextBox tbTitleImageWidth;
        public TextBox tbTitleImageHeight;
        public CheckBox cbIsTitleImageLessSizeNotThumb;

        public CheckBox cbIsShowImageInTextEditor;
        public CheckBox cbIsLinkToOriginal;
        public CheckBox cbIsSmallImage;
        public TextBox tbSmallImageWidth;
        public TextBox tbSmallImageHeight;
        public CheckBox cbIsSmallImageLessSizeNotThumb;

        public Literal ltlScript;

        private string _textBoxClientId;

        public static string GetOpenWindowString(int publishmentSystemId, string textBoxClientId)
        {
            return PageUtils.GetOpenWindowString("上传图片", PageUtils.GetCmsUrl(nameof(ModalUploadImage), new NameValueCollection
            {
                {"publishmentSystemID", publishmentSystemId.ToString()},
                {"textBoxClientID", textBoxClientId}
            }), 570, 540);
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("PublishmentSystemID");
            _textBoxClientId = Body.GetQueryString("TextBoxClientID");

            if (!IsPostBack)
            {
                ConfigSettings(true);

                cbIsTitleImage.Attributes.Add("onclick", "checkBoxChange();");
                cbIsShowImageInTextEditor.Attributes.Add("onclick", "checkBoxChange();");
                cbIsSmallImage.Attributes.Add("onclick", "checkBoxChange();");
            }
        }

        private void ConfigSettings(bool isLoad)
        {
            if (isLoad)
            {
                if (!string.IsNullOrEmpty(PublishmentSystemInfo.Additional.ConfigUploadImageIsTitleImage))
                {
                    cbIsTitleImage.Checked = TranslateUtils.ToBool(PublishmentSystemInfo.Additional.ConfigUploadImageIsTitleImage);
                }
                if (!string.IsNullOrEmpty(PublishmentSystemInfo.Additional.ConfigUploadImageTitleImageWidth))
                {
                    tbTitleImageWidth.Text = PublishmentSystemInfo.Additional.ConfigUploadImageTitleImageWidth;
                }
                if (!string.IsNullOrEmpty(PublishmentSystemInfo.Additional.ConfigUploadImageTitleImageHeight))
                {
                    tbTitleImageHeight.Text = PublishmentSystemInfo.Additional.ConfigUploadImageTitleImageHeight;
                }
                if (!string.IsNullOrEmpty(PublishmentSystemInfo.Additional.ConfigUploadImageIsTitleImageLessSizeNotThumb))
                {
                    cbIsTitleImageLessSizeNotThumb.Checked = TranslateUtils.ToBool(PublishmentSystemInfo.Additional.ConfigUploadImageIsTitleImageLessSizeNotThumb);
                }

                if (!string.IsNullOrEmpty(PublishmentSystemInfo.Additional.ConfigUploadImageIsShowImageInTextEditor))
                {
                    cbIsShowImageInTextEditor.Checked = TranslateUtils.ToBool(PublishmentSystemInfo.Additional.ConfigUploadImageIsShowImageInTextEditor);
                }
                if (!string.IsNullOrEmpty(PublishmentSystemInfo.Additional.ConfigUploadImageIsLinkToOriginal))
                {
                    cbIsLinkToOriginal.Checked = TranslateUtils.ToBool(PublishmentSystemInfo.Additional.ConfigUploadImageIsLinkToOriginal);
                }
                if (!string.IsNullOrEmpty(PublishmentSystemInfo.Additional.ConfigUploadImageIsSmallImage))
                {
                    cbIsSmallImage.Checked = TranslateUtils.ToBool(PublishmentSystemInfo.Additional.ConfigUploadImageIsSmallImage);
                }
                if (!string.IsNullOrEmpty(PublishmentSystemInfo.Additional.ConfigUploadImageSmallImageWidth))
                {
                    tbSmallImageWidth.Text = PublishmentSystemInfo.Additional.ConfigUploadImageSmallImageWidth;
                }
                if (!string.IsNullOrEmpty(PublishmentSystemInfo.Additional.ConfigUploadImageSmallImageHeight))
                {
                    tbSmallImageHeight.Text = PublishmentSystemInfo.Additional.ConfigUploadImageSmallImageHeight;
                }
                if (!string.IsNullOrEmpty(PublishmentSystemInfo.Additional.ConfigUploadImageIsSmallImageLessSizeNotThumb))
                {
                    cbIsSmallImageLessSizeNotThumb.Checked = TranslateUtils.ToBool(PublishmentSystemInfo.Additional.ConfigUploadImageIsSmallImageLessSizeNotThumb);
                }
            }
            else
            {
                PublishmentSystemInfo.Additional.ConfigUploadImageIsTitleImage = cbIsTitleImage.Checked.ToString();
                PublishmentSystemInfo.Additional.ConfigUploadImageTitleImageWidth = tbTitleImageWidth.Text;
                PublishmentSystemInfo.Additional.ConfigUploadImageTitleImageHeight = tbTitleImageHeight.Text;
                PublishmentSystemInfo.Additional.ConfigUploadImageIsTitleImageLessSizeNotThumb = cbIsTitleImageLessSizeNotThumb.Checked.ToString();

                PublishmentSystemInfo.Additional.ConfigUploadImageIsShowImageInTextEditor = cbIsShowImageInTextEditor.Checked.ToString();
                PublishmentSystemInfo.Additional.ConfigUploadImageIsLinkToOriginal = cbIsLinkToOriginal.Checked.ToString();
                PublishmentSystemInfo.Additional.ConfigUploadImageIsSmallImage = cbIsSmallImage.Checked.ToString();
                PublishmentSystemInfo.Additional.ConfigUploadImageSmallImageWidth = tbSmallImageWidth.Text;
                PublishmentSystemInfo.Additional.ConfigUploadImageSmallImageHeight = tbSmallImageHeight.Text;
                PublishmentSystemInfo.Additional.ConfigUploadImageIsSmallImageLessSizeNotThumb = cbIsSmallImageLessSizeNotThumb.Checked.ToString();

                DataProvider.PublishmentSystemDao.Update(PublishmentSystemInfo);
            }
        }

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            if (cbIsTitleImage.Checked && string.IsNullOrEmpty(tbTitleImageWidth.Text) && string.IsNullOrEmpty(tbTitleImageHeight.Text))
            {
                FailMessage("缩略图尺寸不能为空！");
                return;
            }
            if (cbIsSmallImage.Checked && string.IsNullOrEmpty(tbSmallImageWidth.Text) && string.IsNullOrEmpty(tbSmallImageHeight.Text))
            {
                FailMessage("缩略图尺寸不能为空！");
                return;
            }

            ConfigSettings(false);

            if (hifUpload.PostedFile != null && "" != hifUpload.PostedFile.FileName)
            {
                var filePath = hifUpload.PostedFile.FileName;
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
                    if (!PathUtility.IsImageSizeAllowed(PublishmentSystemInfo, hifUpload.PostedFile.ContentLength))
                    {
                        FailMessage("上传失败，上传图片超出规定文件大小！");
                        return;
                    }

                    hifUpload.PostedFile.SaveAs(localFilePath);

                    var isImage = EFileSystemTypeUtils.IsImage(fileExtName);

                    //处理上半部分
                    if (isImage)
                    {
                        FileUtility.AddWaterMark(PublishmentSystemInfo, localFilePath);
                        if (cbIsTitleImage.Checked)
                        {
                            var width = TranslateUtils.ToInt(tbTitleImageWidth.Text);
                            var height = TranslateUtils.ToInt(tbTitleImageHeight.Text);
                            ImageUtils.MakeThumbnail(localFilePath, localTitleFilePath, width, height, cbIsTitleImageLessSizeNotThumb.Checked);
                        }
                    }

                    var imageUrl = PageUtility.GetPublishmentSystemUrlByPhysicalPath(PublishmentSystemInfo, localFilePath);
                    if (cbIsTitleImage.Checked)
                    {
                        imageUrl = PageUtility.GetPublishmentSystemUrlByPhysicalPath(PublishmentSystemInfo, localTitleFilePath);
                    }

                    var textBoxUrl = PageUtility.GetVirtualUrl(PublishmentSystemInfo, imageUrl);

                    ltlScript.Text += $@"
if (parent.document.getElementById('{_textBoxClientId}'))
{{
    parent.document.getElementById('{_textBoxClientId}').value = '{textBoxUrl}';
}}
";

                    //处理下半部分
                    if (cbIsShowImageInTextEditor.Checked && isImage)
                    {
                        imageUrl = PageUtility.GetPublishmentSystemUrlByPhysicalPath(PublishmentSystemInfo, localFilePath);
                        var smallImageUrl = imageUrl;
                        if (cbIsSmallImage.Checked)
                        {
                            smallImageUrl = PageUtility.GetPublishmentSystemUrlByPhysicalPath(PublishmentSystemInfo, localSmallFilePath);
                        }

                        if (cbIsSmallImage.Checked)
                        {
                            var width = TranslateUtils.ToInt(tbSmallImageWidth.Text);
                            var height = TranslateUtils.ToInt(tbSmallImageHeight.Text);
                            ImageUtils.MakeThumbnail(localFilePath, localSmallFilePath, width, height, cbIsSmallImageLessSizeNotThumb.Checked);
                        }

                        var insertHtml = string.Empty;
                        if (cbIsLinkToOriginal.Checked)
                        {
                            insertHtml =
                                $@"<a href=""{imageUrl}"" target=""_blank""><img src=""{smallImageUrl}"" border=""0"" /></a>";
                        }
                        else
                        {
                            insertHtml = $@"<img src=""{smallImageUrl}"" border=""0"" />";
                        }

                        ltlScript.Text += "if(parent." + ETextEditorTypeUtils.GetEditorInstanceScript() + ") parent." + ETextEditorTypeUtils.GetInsertHtmlScript("Content", insertHtml);
                    }

                    ltlScript.Text += PageUtils.HidePopWin;
                }
                catch (Exception ex)
                {
                    FailMessage(ex, ex.Message);
                }
            }
        }
    }
}
