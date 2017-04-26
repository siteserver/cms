using System;
using System.Text;
using BaiRong.Core;
using BaiRong.Core.Data;
using BaiRong.Core.Diagnostics;

namespace SiteServer.BackgroundPages.Settings
{
	public class PageParameter : BasePage
    {
        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            if (!IsPostBack)
            {
                BreadCrumbSettings(AppManager.Settings.LeftMenu.Utility, "查看机器参数", AppManager.Settings.Permission.SettingsUtility);
            }
        }

		public string PrintParameter()
		{
			var builder = new StringBuilder();
			var hostName = ComputerUtils.GetHostName();

            builder.Append($"<tr><td>系统主机名：</td><td>{hostName}</td></tr>");

            builder.Append($"<tr><td>系统根目录地址：</td><td>{WebConfigUtils.PhysicalApplicationPath}</td></tr>");

            builder.Append($"<tr><td>系统程序目录地址：</td><td>{PathUtils.PhysicalSiteServerPath}</td></tr>");

			builder.Append($"<tr><td>计算机的网卡地址：</td><td>{ComputerUtils.GetMacAddress()}</td></tr>");

			builder.Append($"<tr><td>计算机的CPU标识：</td><td>{ComputerUtils.GetProcessorId()}</td></tr>");

			builder.Append($"<tr><td>计算机的硬盘序列号：</td><td>{ComputerUtils.GetColumnSerialNumber()}</td></tr>");

			builder.Append($"<tr><td>域名：</td><td>{PageUtils.GetHost()}</td></tr>");

            builder.Append($"<tr><td>访问IP：</td><td>{PageUtils.GetIpAddress()}</td></tr>");

            builder.Append($"<tr><td>.NET版本：</td><td>{Environment.Version}</td></tr>");

            builder.Append($"<tr><td>SiteServer 系统版本：</td><td>{AppManager.GetFullVersion()}</td></tr>");

            builder.Append(
                $"<tr><td>最近升级时间：</td><td>{DateUtils.GetDateAndTimeString(ConfigManager.Instance.UpdateDate)}</td></tr>");

            builder.Append(
                $"<tr><td>数据库类型：</td><td>{(WebConfigUtils.IsMySql ? "MySql" : "SqlServer")}</td></tr>");

            builder.Append(
                $"<tr><td>数据库名称：</td><td>{SqlUtils.GetDatabaseNameFormConnectionString(WebConfigUtils.ConnectionString)}</td></tr>");

			return builder.ToString();
		}

	}
}
