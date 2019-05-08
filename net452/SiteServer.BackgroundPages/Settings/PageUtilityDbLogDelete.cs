using System;
using System.Web.UI.WebControls;
using Datory;
using SiteServer.CMS.Caches;
using SiteServer.CMS.Database.Core;
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
            var dt = DataProvider.Log.GetLastRemoveLogDate();
            LtlLastExecuteDate.Text = dt == DateTime.MinValue ? "无记录" : DateUtils.GetDateAndTimeString(dt);
        }

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            if (!Page.IsPostBack || !Page.IsValid) return;

            //DataProvider.DatabaseApi.DeleteDbLog();

            var repository = new Repository(WebConfigUtils.DatabaseType, WebConfigUtils.ConnectionString);

            if (WebConfigUtils.DatabaseType == DatabaseType.MySql)
            {
                DataProvider.DatabaseApi.Execute("PURGE MASTER LOGS BEFORE DATE_SUB( NOW( ), INTERVAL 3 DAY)");
            }
            else if (WebConfigUtils.DatabaseType == DatabaseType.SqlServer)
            {
                var databaseName = WebConfigUtils.GetDatabaseNameFormConnectionString(WebConfigUtils.DatabaseType, WebConfigUtils.ConnectionString);

                var versions = repository.Get<string>(Q.SelectRaw("SERVERPROPERTY('productversion')"));

                var version = 8;
                var arr = versions.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
                if (arr.Length > 0)
                {
                    version = TranslateUtils.ToInt(arr[0], 8);
                }
                if (version < 10)
                {
                    //2000,2005
                    var sql = $"BACKUP LOG [{databaseName}] WITH NO_LOG";
                    DataProvider.DatabaseApi.Execute(sql);
                }
                else
                {
                    //2008+
                    var sql =
                        $@"ALTER DATABASE [{databaseName}] SET RECOVERY SIMPLE;DBCC shrinkfile ([{databaseName}_log], 1); ALTER DATABASE [{databaseName}] SET RECOVERY FULL; ";
                    DataProvider.DatabaseApi.Execute(sql);
                }
            }

            AuthRequest.AddAdminLog("清空数据库日志");

            SuccessMessage("清空日志成功！");
        }

    }
}
