using System;
using System.Collections.Specialized;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using SiteServer.Utils;
using SiteServer.Utils.Images;
using SiteServer.CMS.Core;
using SiteServer.Utils.Enumerations;

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

        public static string GetOpenWindowString(int siteId, string textBoxClientId)
        {
            return LayerUtils.GetOpenScript("上传图片", PageUtils.GetCmsUrl(siteId, nameof(ModalUploadImage), new NameValueCollection
            {
                {"textBoxClientID", textBoxClientId}
            }), 600, 560);
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("siteId");
            _textBoxClientId = AuthRequest.GetQueryString("TextBoxClientID");

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
                if (!string.IsNullOrEmpty(Site.Additional.ConfigUploadImageIsTitleImage))
                {
                    CbIsTitleImage.Checked = TranslateUtils.ToBool(Site.Additional.ConfigUploadImageIsTitleImage);
                }
                if (!string.IsNullOrEmpty(Site.Additional.ConfigUploadImageTitleImageWidth))
                {
                    TbTitleImageWidth.Text = Site.Additional.ConfigUploadImageTitleImageWidth;
                }
                if (!string.IsNullOrEmpty(Site.Additional.ConfigUploadImageTitleImageHeight))
                {
                    TbTitleImageHeight.Text = Site.Additional.ConfigUploadImageTitleImageHeight;
                }

                if (!string.IsNullOrEmpty(Site.Additional.ConfigUploadImageIsShowImageInTextEditor))
                {
                    CbIsShowImageInTextEditor.Checked = TranslateUtils.ToBool(Site.Additional.ConfigUploadImageIsShowImageInTextEditor);
                }
                if (!string.IsNullOrEmpty(Site.Additional.ConfigUploadImageIsLinkToOriginal))
                {
                    CbIsLinkToOriginal.Checked = TranslateUtils.ToBool(Site.Additional.ConfigUploadImageIsLinkToOriginal);
                }
                if (!string.IsNullOrEmpty(Site.Additional.ConfigUploadImageIsSmallImage))
                {
                    CbIsSmallImage.Checked = TranslateUtils.ToBool(Site.Additional.ConfigUploadImageIsSmallImage);
                }
                if (!string.IsNullOrEmpty(Site.Additional.ConfigUploadImageSmallImageWidth))
                {
                    TbSmallImageWidth.Text = Site.Additional.ConfigUploadImageSmallImageWidth;
                }
                if (!string.IsNullOrEmpty(Site.Additional.ConfigUploadImageSmallImageHeight))
                {
                    TbSmallImageHeight.Text = Site.Additional.ConfigUploadImageSmallImageHeight;
                }
            }
            else
            {
                Site.Additional.ConfigUploadImageIsTitleImage = CbIsTitleImage.Checked.ToString();
                Site.Additional.ConfigUploadImageTitleImageWidth = TbTitleImageWidth.Text;
                Site.Additional.ConfigUploadImageTitleImageHeight = TbTitleImageHeight.Text;

                Site.Additional.ConfigUploadImageIsShowImageInTextEditor = CbIsShowImageInTextEditor.Checked.ToString();
                Site.Additional.ConfigUploadImageIsLinkToOriginal = CbIsLinkToOriginal.Checked.ToString();
                Site.Additional.ConfigUploadImageIsSmallImage = CbIsSmallImage.Checked.ToString();
                Site.Additional.ConfigUploadImageSmallImageWidth = TbSmallImageWidth.Text;
                Site.Additional.ConfigUploadImageSmallImageHeight = TbSmallImageHeight.Text;

                DataProvider.SiteDao.UpdateAsync(Site).GetAwaiter().GetResult();
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
                var localDirectoryPath = PathUtility.GetUploadDirectoryPath(Site, fileExtName);
                var localFileName = PathUtility.GetUploadFileName(Site, filePath);
                var localTitleFileName = Constants.TitleImageAppendix + localFileName;
                var localSmallFileName = Constants.SmallImageAppendix + localFileName;
                var localFilePath = PathUtils.Combine(localDirectoryPath, localFileName);
                var localTitleFilePath = PathUtils.Combine(localDirectoryPath, localTitleFileName);
                var localSmallFilePath = PathUtils.Combine(localDirectoryPath, localSmallFileName);

                if (!PathUtility.IsImageExtenstionAllowed(Site, fileExtName))
                {
                    FailMessage("上传失败，上传图片格式不正确！");
                    return;
                }
                if (!PathUtility.IsImageSizeAllowed(Site, HifUpload.PostedFile.ContentLength))
                {
                    FailMessage("上传失败，上传图片超出规定文件大小！");
                    return;
                }

                HifUpload.PostedFile.SaveAs(localFilePath);

                var isImage = EFileSystemTypeUtils.IsImage(fileExtName);

                //处理上半部分
                if (isImage)
                {
                    FileUtility.AddWaterMark(Site, localFilePath);
                    if (CbIsTitleImage.Checked)
                    {
                        var width = TranslateUtils.ToInt(TbTitleImageWidth.Text);
                        var height = TranslateUtils.ToInt(TbTitleImageHeight.Text);
                        ImageUtils.MakeThumbnail(localFilePath, localTitleFilePath, width, height, true);
                    }
                }

                var imageUrl = PageUtility.GetSiteUrlByPhysicalPathAsync(Site, localFilePath, true).GetAwaiter().GetResult();
                if (CbIsTitleImage.Checked)
                {
                    imageUrl = PageUtility.GetSiteUrlByPhysicalPathAsync(Site, localTitleFilePath, true).GetAwaiter().GetResult();
                }

                var textBoxUrl = PageUtility.GetVirtualUrl(Site, imageUrl);

                var script = $@"
if (parent.document.getElementById('{_textBoxClientId}'))
{{
    parent.document.getElementById('{_textBoxClientId}').value = '{textBoxUrl}';
}}
";

                //处理下半部分
                if (CbIsShowImageInTextEditor.Checked && isImage)
                {
                    imageUrl = PageUtility.GetSiteUrlByPhysicalPathAsync(Site, localFilePath, true).GetAwaiter().GetResult();
                    var smallImageUrl = imageUrl;
                    if (CbIsSmallImage.Checked)
                    {
                        smallImageUrl = PageUtility.GetSiteUrlByPhysicalPathAsync(Site, localSmallFilePath, true).GetAwaiter().GetResult();
                    }

                    if (CbIsSmallImage.Checked)
                    {
                        var width = TranslateUtils.ToInt(TbSmallImageWidth.Text);
                        var height = TranslateUtils.ToInt(TbSmallImageHeight.Text);
                        ImageUtils.MakeThumbnail(localFilePath, localSmallFilePath, width, height, true);
                    }

                    var insertHtml = CbIsLinkToOriginal.Checked ? $@"<a href=""{imageUrl}"" target=""_blank""><img src=""{smallImageUrl}"" border=""0"" /></a>" : $@"<img src=""{smallImageUrl}"" border=""0"" />";

                    script += "if(parent." + UEditorUtils.GetEditorInstanceScript() + ") parent." + UEditorUtils.GetInsertHtmlScript("Content", insertHtml);
                }

                LtlScript.Text = $@"
<script type=""text/javascript"" language=""javascript"">
    {script}
    {LayerUtils.CloseScript}
</script>";
            }
            catch (Exception ex)
            {
                FailMessage(ex, ex.Message);
            }
        }
    }
}
