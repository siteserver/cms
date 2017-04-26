using System;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Text;

namespace SiteServer.BackgroundPages.User
{
	public class ModalUserImport : BasePage
    {
        public RadioButtonList ImportType;
		public HtmlInputFile myFile;
        public RadioButtonList IsOverride;
        public TextBox ImportStart;
        public TextBox ImportCount;

        public static string GetOpenWindowString()
        {
            return PageUtils.GetOpenWindowString("导入用户", PageUtils.GetUserUrl(nameof(ModalUserImport), null), 450, 350);
        }

		public void Page_Load(object sender, EventArgs e)
		{
            if (IsForbidden) return;

		}

        public override void Submit_OnClick(object sender, EventArgs e)
        {
			if (myFile.PostedFile != null && "" != myFile.PostedFile.FileName)
			{
				try
				{
                    var filePath = myFile.PostedFile.FileName;
                    if (!StringUtils.EqualsIgnoreCase(PathUtils.GetExtension(filePath), ".xls"))
                    {
                        FailMessage("必须上传后缀为“.xls”的Excel文件");
                        return;
                    }

                    var localFilePath = PathUtils.GetTemporaryFilesPath(PathUtils.GetFileName(filePath));

                    myFile.PostedFile.SaveAs(localFilePath);

                    //this.ImportContentsByExcelFile(localFilePath, TranslateUtils.ToBool(this.IsOverride.SelectedValue), TranslateUtils.ToInt(this.ImportStart.Text), TranslateUtils.ToInt(this.ImportCount.Text));

                    Body.AddAdminLog("导入用户");

					PageUtils.CloseModalPage(Page);
				}
				catch(Exception ex)
				{
                    FailMessage(ex, $"导入用户失败，{ex.Message}");
				}
			}
		}
	}
}
