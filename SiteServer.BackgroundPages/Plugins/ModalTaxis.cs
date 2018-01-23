using System;
using System.IO;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using SiteServer.Utils;
using SiteServer.Utils.Enumerations;

namespace SiteServer.BackgroundPages.Plugins
{
	public class ModalTaxis : BasePage
    {
		public HtmlInputFile HifMyFile;
		public DropDownList DdlIsOverride;

        public static string GetOpenWindowString()
        {
            return LayerUtils.GetOpenScript("手动安装插件", PageUtils.GetPluginsUrl(nameof(ModalTaxis), null), 560, 260);
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            if (!IsPostBack)
			{
			
			}
		}

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            if (HifMyFile.PostedFile == null || HifMyFile.PostedFile.FileName == "") return;

            var filePath = HifMyFile.PostedFile.FileName;
            if (EFileSystemTypeUtils.GetEnumType(Path.GetExtension(filePath)) != EFileSystemType.Zip)
            {
                FailMessage("必须上传ZIP文件");
                return;
            }

            try
            {
                //var localFilePath = PathUtils.GetTemporaryFilesPath(Path.GetFileName(filePath));

                //HifMyFile.PostedFile.SaveAs(localFilePath);

                //var importObject = new ImportObject(SiteId);
                //importObject.ImportRelatedFieldByZipFile(localFilePath, TranslateUtils.ToBool(DdlIsOverride.SelectedValue));

                //Body.AddSiteLog(SiteId, "导入联动字段");

                LayerUtils.Close(Page);
            }
            catch (Exception ex)
            {
                FailMessage(ex, "导入联动字段失败！");
            }
        }
	}
}
