using System;
using System.Web.UI.WebControls;
using BaiRong.Core;
using SiteServer.CMS.Core;

namespace SiteServer.BackgroundPages.Service
{
    public class PageStatus : BasePage
    {
        public Literal LtlMessage;

        public Literal LtlCreateWatch;
        public Literal LtlTaskCreate;
        public Literal LtlTaskGather;
        public Literal LtlTaskBackup;

		public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;
            if (IsPostBack) return;

            BreadCrumbService(AppManager.Service.LeftMenu.Status, "服务状态", AppManager.Service.Permission.ServiceStatus);

            if (ServiceManager.IsServiceOnline())
            {
                LtlMessage.Text = GetMessage(true, "siteserver.exe 服务组件已启动并处于正常运行状态");

                LtlCreateWatch.Text = GetStatus(true, "服务组件生成处于正常运行状态");
                LtlTaskCreate.Text = GetStatus(true, "定时生成任务处于正常运行状态");
                LtlTaskGather.Text = GetStatus(true, "定时采集任务处于正常运行状态");
                LtlTaskBackup.Text = GetStatus(true, "定时备份任务处于正常运行状态");
            }
            else
            {
                LtlMessage.Text = GetMessage(false, "siteserver.exe 服务组件未启动");

                LtlCreateWatch.Text = GetStatus(false, "服务组件生成未启动");
                LtlTaskCreate.Text = GetStatus(false, "定时生成任务未启动");
                LtlTaskGather.Text = GetStatus(false, "定时采集任务未启动");
                LtlTaskBackup.Text = GetStatus(false, "定时备份任务未启动");
            }
        }

        private string GetMessage(bool isOk, string text)
        {
            if (isOk)
            {
                return $@"<h4 class=""alert alert-success"">{text}</h4>";
            }
            return $@"<div class=""alert alert-danger"">
                  <h4 style=""padding-top: 10px;"">{text}</h4>
                  <p style=""margin: 10px 0 10px; font-size: 14px;"">
                    siteserver.exe 服务组件未启动，请在SiteServer系统根目录下双击运行siteserver.exe程序启用服务。
                  </p>
                </div>";
        }

        private string GetStatus(bool isNormal, string text)
        {
            return
                $@"<td><span class=""{(isNormal ? "normal" : "issue")}""></span><span class=""text"">{text}</span></td>";
        }
	}
}
