using System;
using System.Web.UI.WebControls;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
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

            VerifySystemPermissions(ConfigManager.SettingsPermissions.Utility);
            var dt = DataProvider.LogDao.GetLastRemoveLogDate(AuthRequest.AdminName);
            LtlLastExecuteDate.Text = dt == DateTime.MinValue ? "无记录" : DateUtils.GetDateAndTimeString(dt);
        }

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            if (!Page.IsPostBack || !Page.IsValid) return;

            DataProvider.DatabaseDao.DeleteDbLog();

            AuthRequest.AddAdminLog("清空数据库日志");

            SuccessMessage("清空日志成功！");
        }

    }
}
