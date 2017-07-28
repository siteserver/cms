using System;
using BaiRong.Core;
using BaiRong.Core.Text;

namespace SiteServer.BackgroundPages.Settings
{
    public class PageDbLogDelete : BasePage
    {
        protected override bool IsAccessable => true;

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            if (!IsPostBack)
            {
                BreadCrumbSettings(AppManager.Settings.LeftMenu.Utility, "清空数据库日志", AppManager.Settings.Permission.SettingsUtility);
            }
        }

        public string GetLastExecuteDate()
        {
            var dt = BaiRongDataProvider.LogDao.GetLastRemoveLogDate(Body.AdministratorName);
            return dt == DateTime.MinValue ? "无记录" : DateUtils.GetDateAndTimeString(dt);
        }

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            if (Page.IsPostBack && Page.IsValid)
            {
                try
                {
                    BaiRongDataProvider.DatabaseDao.DeleteDbLog();

                    Body.AddAdminLog("清空数据库日志");

                    SuccessMessage("清空日志成功！");
                }
                catch (Exception ex)
                {
                    PageUtils.RedirectToErrorPage(ex.Message);
                }
            }
        }

    }
}
