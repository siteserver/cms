using System;
using System.Collections.Specialized;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using SiteServer.Utils;
using SiteServer.BackgroundPages.Core;
using SiteServer.CMS.Core;
using SiteServer.CMS.Core.Create;
using SiteServer.CMS.Core.Office;
using SiteServer.CMS.Model;
using SiteServer.Utils.Enumerations;
using SiteServer.Utils.Images;

namespace SiteServer.BackgroundPages.Cms
{
    public class ModalTextEditorInsertImage : BasePageCms
    {
        public HtmlInputHidden HihFilePaths;
        public CheckBox CbIsLinkToOriginal;
        public CheckBox CbIsSmallImage;
        public TextBox TbSmallImageWidth;
        public TextBox TbSmallImageHeight;

        private string _attributeName;

        public static string GetOpenWindowString(int siteId, string attributeName)
        {
            return LayerUtils.GetOpenScript("插入图片",
                PageUtils.GetCmsUrl(siteId, nameof(ModalTextEditorInsertImage), new NameValueCollection
                {
                    {"attributeName", attributeName}
                }), 700, 550);
        }

        public string UploadUrl => ModalTextEditorInsertImageHandler.GetRedirectUrl(SiteId);

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("siteId", "attributeName");
            _attributeName = AuthRequest.GetQueryString("attributeName");

            if (IsPostBack) return;

            ConfigSettings(true);

            CbIsSmallImage.Attributes.Add("onclick", "checkBoxChange();");
        }

        private void ConfigSettings(bool isLoad)
        {
            if (isLoad)
            {
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
                if (SiteInfo.Additional.ConfigUploadImageIsLinkToOriginal != CbIsLinkToOriginal.Checked.ToString()
                     || SiteInfo.Additional.ConfigUploadImageIsSmallImage != CbIsSmallImage.Checked.ToString()
                     || SiteInfo.Additional.ConfigUploadImageSmallImageWidth != TbSmallImageWidth.Text
                     || SiteInfo.Additional.ConfigUploadImageSmallImageHeight != TbSmallImageHeight.Text)
                {
                    SiteInfo.Additional.ConfigUploadImageIsLinkToOriginal = CbIsLinkToOriginal.Checked.ToString();
                    SiteInfo.Additional.ConfigUploadImageIsSmallImage = CbIsSmallImage.Checked.ToString();
                    SiteInfo.Additional.ConfigUploadImageSmallImageWidth = TbSmallImageWidth.Text;
                    SiteInfo.Additional.ConfigUploadImageSmallImageHeight = TbSmallImageHeight.Text;

                    DataProvider.SiteDao.Update(SiteInfo);
                }
            }
        }

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            if (!Page.IsPostBack || !Page.IsValid) return;

            if (CbIsSmallImage.Checked && string.IsNullOrEmpty(TbSmallImageWidth.Text) && string.IsNullOrEmpty(TbSmallImageHeight.Text))
            {
                FailMessage("缩略图尺寸不能为空！");
                return;
            }

            ConfigSettings(false);

            var scripts = string.Empty;

            var fileNames = TranslateUtils.StringCollectionToStringList(HihFilePaths.Value);

            foreach (var filePath in fileNames)
            {
                if (!string.IsNullOrEmpty(filePath))
                {
                    var fileName = PathUtils.GetFileName(filePath);

                    var fileExtName = PathUtils.GetExtension(filePath).ToLower();
                    var localDirectoryPath = PathUtility.GetUploadDirectoryPath(SiteInfo, fileExtName);

                    var imageUrl = PageUtility.GetSiteUrlByPhysicalPath(SiteInfo, filePath, true);

                    if (CbIsSmallImage.Checked)
                    {
                        var localSmallFileName = StringUtils.Constants.SmallImageAppendix + fileName;
                        var localSmallFilePath = PathUtils.Combine(localDirectoryPath, localSmallFileName);

                        var smallImageUrl = PageUtility.GetSiteUrlByPhysicalPath(SiteInfo, localSmallFilePath, true);

                        var width = TranslateUtils.ToInt(TbSmallImageWidth.Text);
                        var height = TranslateUtils.ToInt(TbSmallImageHeight.Text);
                        ImageUtils.MakeThumbnail(filePath, localSmallFilePath, width, height, true);

                        var insertHtml = CbIsLinkToOriginal.Checked
                            ? $@"<a href=""{imageUrl}"" target=""_blank""><img src=""{smallImageUrl}"" border=""0"" /></a>"
                            : $@"<img src=""{smallImageUrl}"" border=""0"" />";

                        scripts += "if(parent." + UEditorUtils.GetEditorInstanceScript() + ") parent." +
                                   UEditorUtils.GetInsertHtmlScript("Content", insertHtml);
                    }
                    else
                    {
                        var insertHtml = $@"<img src=""{imageUrl}"" border=""0"" />";

                        scripts += "if(parent." + UEditorUtils.GetEditorInstanceScript() + ") parent." +
                                      UEditorUtils.GetInsertHtmlScript("Content", insertHtml);
                    }
                }
            }

            LayerUtils.CloseWithoutRefresh(Page, scripts);
        }
    }
}
