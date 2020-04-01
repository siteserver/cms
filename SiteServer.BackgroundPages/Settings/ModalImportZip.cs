using System;
using System.Collections.Specialized;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using SiteServer.Utils;
using SiteServer.BackgroundPages.Cms;
using SiteServer.CMS.Core;
using SiteServer.Utils.Enumerations;

namespace SiteServer.BackgroundPages.Settings
{
	public class ModalImportZip : BasePageCms
	{
	    public const string TypeSiteTemplate = "SiteTemplate";

        public DropDownList DdlImportType;
        public PlaceHolder PhUpload;
		public HtmlInputFile HifFile;
        public PlaceHolder PhDownload;
        public TextBox TbDownloadUrl;

	    private string _type;

        public static string GetOpenWindowString(string type)
        {
            return LayerUtils.GetOpenScript(type == TypeSiteTemplate ? "导入站点模板" : "导入插件",
                PageUtils.GetSettingsUrl(nameof(ModalImportZip), new NameValueCollection
                {
                    {"type", type}
                }), 520, 240);
        }

		public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;
            _type = AuthRequest.GetQueryString("type");
            if (Page.IsPostBack) return;

            EBooleanUtils.AddListItems(DdlImportType, "上传压缩包并导入", "从指定地址下载压缩包并导入");
            ControlUtils.SelectSingleItemIgnoreCase(DdlImportType, true.ToString());

            PhUpload.Visible = true;
            PhDownload.Visible = false;
        }

        public void RblImportType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (TranslateUtils.ToBool(DdlImportType.SelectedValue))
            {
                PhUpload.Visible = true;
                PhDownload.Visible = false;
            }
            else
            {
                PhUpload.Visible = false;
                PhDownload.Visible = true;
            }
        }

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            var isUpload = TranslateUtils.ToBool(DdlImportType.SelectedValue);
            if (_type == TypeSiteTemplate)
            {
                ImportSiteTemplate(isUpload);
            }
        }

	    private void ImportSiteTemplate(bool isUpload)
	    {
	        if (isUpload)
	        {
	            if (!string.IsNullOrEmpty(HifFile.PostedFile?.FileName))
	            {
	                var filePath = HifFile.PostedFile.FileName;
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

	                    HifFile.PostedFile.SaveAs(localFilePath);

	                    ZipUtils.ExtractZip(localFilePath, directoryPath);

                        LayerUtils.CloseAndRedirect(Page, PageSiteTemplate.GetRedirectUrl());
	                }
	                catch (Exception ex)
	                {
	                    FailMessage(ex, "文件上传失败！");
	                }
	            }
	        }
	        else
	        {
	            var sExt = PathUtils.GetExtension(TbDownloadUrl.Text);
	            if (!StringUtils.EqualsIgnoreCase(sExt, ".zip"))
	            {
	                FailMessage("站点模板压缩包为zip格式，请输入有效文件地址");
	                return;
	            }
	            
	            PageUtils.Redirect(ModalProgressBar.GetRedirectUrlStringWithSiteTemplateDownload(0, TbDownloadUrl.Text));
	        }
	    }
    }
}
