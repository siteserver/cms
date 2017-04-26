using System;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Model.Enumerations;
using SiteServer.CMS.Controllers.Stl;
using SiteServer.CMS.Core.Office;

namespace SiteServer.BackgroundPages.User
{
	public class ModalUserExport : BasePage
    {
        public PlaceHolder PhExport;
        public RadioButtonList RblCheckedState;

        public static string GetOpenWindowString()
        {
            return PageUtils.GetOpenWindowString("导出用户", PageUtils.GetUserUrl(nameof(ModalUserExport), null), 380, 250);
        }

		public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

			if (!IsPostBack)
			{
                PhExport.Visible = true;
                ETriStateUtils.AddListItems(RblCheckedState, "全部", "审核通过", "未审核");
                ControlUtils.SelectListItems(RblCheckedState, ETriStateUtils.GetValue(ETriState.All));
			}
		}

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            try
            {
                PhExport.Visible = false;

                const string fileName = "users.csv";
                var filePath = PathUtils.GetTemporaryFilesPath(fileName);

                ExcelObject.CreateExcelFileForUsers(filePath, ETriStateUtils.GetEnumType(RblCheckedState.SelectedValue));

                var link = new HyperLink
                {
                    NavigateUrl = ActionsDownload.GetUrl(PageUtils.GetApiUrl(), filePath),
                    Text = "下载"
                };
                var successMessage = "成功导出文件！&nbsp;&nbsp;" + ControlUtils.GetControlRenderHtml(link);
                SuccessMessage(successMessage);
            }
            catch (Exception ex)
            {
                var failedMessage = "文件导出失败！<br/><br/>原因为：" + ex.Message;
                FailMessage(ex, failedMessage);
            }
		}
	}
}
