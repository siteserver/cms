using System;
using System.Web.UI.WebControls;
using BaiRong.Core;

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

            VerifyAdministratorPermissions(AppManager.Permissions.Settings.Utility);
            var dt = BaiRongDataProvider.LogDao.GetLastRemoveLogDate(Body.AdminName);
            LtlLastExecuteDate.Text = dt == DateTime.MinValue ? "无记录" : DateUtils.GetDateAndTimeString(dt);
        }

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            if (!Page.IsPostBack || !Page.IsValid) return;

            BaiRongDataProvider.DatabaseDao.DeleteDbLog();

            Body.AddAdminLog("清空数据库日志");

            SuccessMessage("清空日志成功！");
        }

    }
}
