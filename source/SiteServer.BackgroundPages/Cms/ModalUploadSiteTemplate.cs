using System;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.IO;
using BaiRong.Core.Model.Enumerations;
using SiteServer.BackgroundPages.Sys;
using SiteServer.CMS.Core;

namespace SiteServer.BackgroundPages.Cms
{
	public class ModalUploadSiteTemplate : BasePageCms
    {
        public RadioButtonList rblImportType;

        public PlaceHolder phUpload;
		public HtmlInputFile myFile;

        public PlaceHolder phDownload;
        public TextBox tbDownloadUrl;

        public static string GetOpenWindowString()
        {
            return PageUtils.GetOpenWindowString("导入站点模板", PageUtils.GetCmsUrl(nameof(ModalUploadSiteTemplate), null), 460, 300);
        }

		public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            if (!Page.IsPostBack)
            {
                EBooleanUtils.AddListItems(rblImportType, "上传压缩包并导入", "从指定地址下载压缩包并导入");
                ControlUtils.SelectListItemsIgnoreCase(rblImportType, true.ToString());

                phUpload.Visible = true;
                phDownload.Visible = false;
            }
		}

        public void rblImportType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (TranslateUtils.ToBool(rblImportType.SelectedValue))
            {
                phUpload.Visible = true;
                phDownload.Visible = false;
            }
            else
            {
                phUpload.Visible = false;
                phDownload.Visible = true;
            }
        }

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            var isUpload = TranslateUtils.ToBool(rblImportType.SelectedValue);
            if (isUpload)
            {
                if (myFile.PostedFile != null && "" != myFile.PostedFile.FileName)
                {
                    var filePath = myFile.PostedFile.FileName;
                    var sExt = PathUtils.GetExtension(filePath);
                    if (!StringUtils.EqualsIgnoreCase(sExt, ".zip"))
                    {
                        FailMessage("站点模板压缩包为zip格式，请选择有效的文件上传");
                        return;
                    }
                    try
                    {
                        var directoryName = PathUtils.GetFileNameWithoutExtension(filePath);
                        var directoryPath = PathUtility.GetSiteTemplatesPath(directoryName);
                        if (DirectoryUtils.IsDirectoryExists(directoryPath))
                        {
                            FailMessage($"站点模板导入失败，文件夹{directoryName}已存在");
                            return;
                        }
                        var localFilePath = PathUtility.GetSiteTemplatesPath(directoryName + ".zip");
                        FileUtils.DeleteFileIfExists(localFilePath);

                        myFile.PostedFile.SaveAs(localFilePath);

                        ZipUtils.UnpackFiles(localFilePath, directoryPath);

                        PageUtils.CloseModalPageAndRedirect(Page, PageSiteTemplate.GetRedirectUrl());
                    }
                    catch (Exception ex)
                    {
                        FailMessage(ex, "文件上传失败！");
                    }
                }
            }
            else
            {
                var sExt = PathUtils.GetExtension(tbDownloadUrl.Text);
                if (!StringUtils.EqualsIgnoreCase(sExt, ".zip"))
                {
                    FailMessage("站点模板压缩包为zip格式，请输入有效文件地址");
                    return;
                }
                var directoryName = PathUtils.GetFileNameWithoutExtension(tbDownloadUrl.Text);
                PageUtils.Redirect(ModalProgressBar.GetRedirectUrlStringWithSiteTemplateDownload(tbDownloadUrl.Text, directoryName));
            }
		}

	}
}
