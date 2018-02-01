using System;
using System.Web.UI.WebControls;
using SiteServer.CMS.Core;
using SiteServer.Utils;

namespace SiteServer.BackgroundPages.Settings
{
    public class PageUtilityDbLogDelete : BasePage
    {
        public Literal LtlLastExecuteDate;

        protected override bool IsAccessable => true;

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            if (IsPostBack) return;

            VerifyAdministratorPermissions(ConfigManager.Permissions.Settings.Utility);
            var dt = DataProvider.LogDao.GetLastRemoveLogDate(Body.AdminName);
            LtlLastExecuteDate.Text = dt == DateTime.MinValue ? "无记录" : DateUtils.GetDateAndTimeString(dt);
        }

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            if (!Page.IsPostBack || !Page.IsValid) return;

            DataProvider.DatabaseDao.DeleteDbLog();

            Body.AddAdminLog("清空数据库日志");

            SuccessMessage("清空日志成功！");
        }

    }
}
