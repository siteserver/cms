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
                if (!string.IsNullOrEmpty(SiteInfo.Additional.ConfigUploadImageIsTitleImage))
                {
                    CbIsTitleImage.Checked = TranslateUtils.ToBool(SiteInfo.Additional.ConfigUploadImageIsTitleImage);
                }
                if (!string.IsNullOrEmpty(SiteInfo.Additional.ConfigUploadImageTitleImageWidth))
                {
                    TbTitleImageWidth.Text = SiteInfo.Additional.ConfigUploadImageTitleImageWidth;
                }
                if (!string.IsNullOrEmpty(SiteInfo.Additional.ConfigUploadImageTitleImageHeight))
                {
                    TbTitleImageHeight.Text = SiteInfo.Additional.ConfigUploadImageTitleImageHeight;
                }

                if (!string.IsNullOrEmpty(SiteInfo.Additional.ConfigUploadImageIsShowImageInTextEditor))
                {
                    CbIsShowImageInTextEditor.Checked = TranslateUtils.ToBool(SiteInfo.Additional.ConfigUploadImageIsShowImageInTextEditor);
                }
                if (!string.IsNullOrEmpty(SiteInfo.Additional.ConfigUploadImageIsLinkToOriginal))
                {
                    CbIsLinkToOriginal.Checked = TranslateUtils.ToBool(SiteInfo.Additional.ConfigUploadImageIsLinkToOriginal);
                }
                if (!string.IsNullOrEmpty(SiteInfo.Additional.ConfigUploadImageIsSmallImage))
                {
                    CbIsSmallImage.Checked = TranslateUtils.ToBool(SiteInfo.Additional.ConfigUploadImageIsSmallImage);
                }
                if (!string.IsNullOrEmpty(SiteInfo.Additional.ConfigUploadImageSmallImageWidth))
                {
                    TbSmallImageWidth.Text = SiteInfo.Additional.ConfigUploadImageSmallImageWidth;
                }
                if (!string.IsNullOrEmpty(SiteInfo.Additional.ConfigUploadImageSmallImageHeight))
                {
                    TbSmallImageHeight.Text = SiteInfo.Additional.ConfigUploadImageSmallImageHeight;
                }
            }
            else
            {
                SiteInfo.Additional.ConfigUploadImageIsTitleImage = CbIsTitleImage.Checked.ToString();
                SiteInfo.Additional.ConfigUploadImageTitleImageWidth = TbTitleImageWidth.Text;
                SiteInfo.Additional.ConfigUploadImageTitleImageHeight = TbTitleImageHeight.Text;

                SiteInfo.Additional.ConfigUploadImageIsShowImageInTextEditor = CbIsShowImageInTextEditor.Checked.ToString();
                SiteInfo.Additional.ConfigUploadImageIsLinkToOriginal = CbIsLinkToOriginal.Checked.ToString();
                SiteInfo.Additional.ConfigUploadImageIsSmallImage = CbIsSmallImage.Checked.ToString();
                SiteInfo.Additional.ConfigUploadImageSmallImageWidth = TbSmallImageWidth.Text;
                SiteInfo.Additional.ConfigUploadImageSmallImageHeight = TbSmallImageHeight.Text;

                DataProvider.SiteDao.Update(SiteInfo);
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
                var localDirectoryPath = PathUtility.GetUploadDirectoryPath(SiteInfo, fileExtName);
                var localFileName = PathUtility.GetUploadFileName(SiteInfo, filePath);
                var localTitleFileName = StringUtils.Constants.TitleImageAppendix + localFileName;
                var localSmallFileName = StringUtils.Constants.SmallImageAppendix + localFileName;
                var localFilePath = PathUtils.Combine(localDirectoryPath, localFileName);
                var localTitleFilePath = PathUtils.Combine(localDirectoryPath, localTitleFileName);
                var localSmallFilePath = PathUtils.Combine(localDirectoryPath, localSmallFileName);

                if (!PathUtility.IsImageExtenstionAllowed(SiteInfo, fileExtName))
                {
                    FailMessage("上传失败，上传图片格式不正确！");
                    return;
                }
                if (!PathUtility.IsImageSizeAllowed(SiteInfo, HifUpload.PostedFile.ContentLength))
                {
                    FailMessage("上传失败，上传图片超出规定文件大小！");
                    return;
                }

                HifUpload.PostedFile.SaveAs(localFilePath);

                var isImage = EFileSystemTypeUtils.IsImage(fileExtName);

                //处理上半部分
                if (isImage)
                {
                    FileUtility.AddWaterMark(SiteInfo, localFilePath);
                    if (CbIsTitleImage.Checked)
                    {
                        var width = TranslateUtils.ToInt(TbTitleImageWidth.Text);
                        var height = TranslateUtils.ToInt(TbTitleImageHeight.Text);
                        ImageUtils.MakeThumbnail(localFilePath, localTitleFilePath, width, height, true);
                    }
                }

                var imageUrl = PageUtility.GetSiteUrlByPhysicalPath(SiteInfo, localFilePath, true);
                if (CbIsTitleImage.Checked)
                {
                    imageUrl = PageUtility.GetSiteUrlByPhysicalPath(SiteInfo, localTitleFilePath, true);
                }

                var textBoxUrl = PageUtility.GetVirtualUrl(SiteInfo, imageUrl);

                var script = $@"
if (parent.document.getElementById('{_textBoxClientId}'))
{{
    parent.document.getElementById('{_textBoxClientId}').value = '{textBoxUrl}';
}}
";

                //处理下半部分
                if (CbIsShowImageInTextEditor.Checked && isImage)
                {
                    imageUrl = PageUtility.GetSiteUrlByPhysicalPath(SiteInfo, localFilePath, true);
                    var smallImageUrl = imageUrl;
                    if (CbIsSmallImage.Checked)
                    {
                        smallImageUrl = PageUtility.GetSiteUrlByPhysicalPath(SiteInfo, localSmallFilePath, true);
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
