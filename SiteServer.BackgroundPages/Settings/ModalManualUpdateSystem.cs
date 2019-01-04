using System;
using System.IO;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using SiteServer.Utils;
using SiteServer.Utils.Enumerations;

namespace SiteServer.BackgroundPages.Settings
{
	public class ModalManualUpdateSystem : BasePage
	{
	    public RadioButtonList RblInstallType;

	    public PlaceHolder PhUpload;
        public HtmlInputFile HifFile;

        public PlaceHolder PhVersion;
	    public TextBox TbVersion;

        protected override bool IsSinglePage => true;

        public static string GetOpenWindowString()
        {
            return LayerUtils.GetOpenScript("手动升级 SiteServer CMS 版本", PageUtils.GetSettingsUrl(nameof(ModalManualUpdateSystem), null), 560, 430);
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            if (IsPostBack) return;

            EBooleanUtils.AddListItems(RblInstallType, "上传升级包", "指定版本号");
            ControlUtils.SelectSingleItem(RblInstallType, true.ToString());
        }

        public void RblInstallType_SelectedIndexChanged(object sender, EventArgs e)
        {
            PhUpload.Visible = TranslateUtils.ToBool(RblInstallType.SelectedValue);
            PhVersion.Visible = !PhUpload.Visible;
        }

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            if (TranslateUtils.ToBool(RblInstallType.SelectedValue))
            {
                UpdateByUpload();
            }
            else
            {
                UpdateByVersion();
            }
        }

	    private void UpdateByVersion()
	    {
	        //string errorMessage;
	        //if (!SystemManager.GetPackageAndUpdate(TbVersion.Text, out errorMessage))
	        //{
	        //    FailMessage($"手动升级 SiteServer CMS 版本失败：{errorMessage}");
	        //    return;
	        //}

	        AuthRequest.AddAdminLog($"手动升级 SiteServer CMS 版本：{TbVersion.Text}");

	        LayerUtils.CloseAndRedirect(Page, PageUtils.GetAdminUrl(PageUtils.Combine("plugins/manage.cshtml")));
	    }

	    private void UpdateByUpload()
	    {
	        if (HifFile.PostedFile == null || HifFile.PostedFile.FileName == "") return;

	        var filePath = HifFile.PostedFile.FileName;
	        if (!StringUtils.EqualsIgnoreCase(Path.GetExtension(filePath), ".nupkg"))
	        {
	            FailMessage("必须上传后缀为.nupkg的文件");
	            return;
	        }

	        var idAndVersion = Path.GetFileNameWithoutExtension(filePath);
	        var directoryPath = PathUtils.GetPackagesPath(idAndVersion);
	        var localFilePath = PathUtils.Combine(directoryPath, idAndVersion + ".nupkg");

	        if (!Directory.Exists(directoryPath))
	        {
	            Directory.CreateDirectory(directoryPath);
	        }

	        HifFile.PostedFile.SaveAs(localFilePath);

	        ZipUtils.ExtractZip(localFilePath, directoryPath);

	        AuthRequest.AddAdminLog("手动升级 SiteServer CMS 版本：" + idAndVersion);

	        LayerUtils.CloseAndRedirect(Page, PageUtils.GetAdminUrl(PageUtils.Combine("plugins/manage.cshtml")));
	    }
	}
}
