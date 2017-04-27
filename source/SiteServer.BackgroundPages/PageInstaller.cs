using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Model.Enumerations;

namespace SiteServer.BackgroundPages
{
    public class PageInstaller : Page
	{
        public Literal LtlVersionInfo;
        public Literal LtlStepTitle;

        public Literal LtlErrorMessage;

        public PlaceHolder PhStep1;
        public CheckBox ChkIAgree;

        public PlaceHolder PhStep2;
        public Literal LtlDomain;
        public Literal LtlVersion;
        public Literal LtlNetVersion;
        public Literal LtlPhysicalApplicationPath;
        public Literal LtlRootWrite;
        public Literal LtlSiteFielsWrite;
        public Button BtnStep2;

        public PlaceHolder PhStep3;
	    public DropDownList DdlSqlDatabaseType;

        public PlaceHolder PhSql1;
        public TextBox TbSqlServer;
        public TextBox TbSqlUserName;
        public TextBox TbSqlPassword;
        public HtmlInputHidden HihSqlHiddenPassword;
        public PlaceHolder PhSql2;
        public DropDownList DdlSqlDatabaseName;

        public PlaceHolder PhStep4;
        public DropDownList DdlIsProtectData;
        public TextBox TbAdminPassword;
        public TextBox TbComfirmAdminPassword;
        public TextBox TbAdminName;

        public PlaceHolder PhStep5;

        private string GetSetpTitleString(int step)
        {
            PhStep1.Visible = PhStep2.Visible = PhStep3.Visible = PhStep4.Visible = PhStep5.Visible = false;
            if (step == 1)
            {
                PhStep1.Visible = true;
            }
            else if (step == 2)
            {
                PhStep2.Visible = true;
            }
            else if (step == 3)
            {
                PhStep3.Visible = true;
                PhSql1.Visible = true;
                PhSql2.Visible = false;
            }
            else if (step == 4)
            {
                PhStep4.Visible = true;
            }
            else if (step == 5)
            {
                PhStep5.Visible = true;
            }

            var builder = new StringBuilder();

            for (var i = 1; i <= 5; i++)
            {
                var liClass = string.Empty;
                if (i == step)
                {
                    liClass = @" class=""current""";
                }
                string imageUrl = $"images/step{i}{((i <= step) ? "a" : "b")}.gif";
                var title = string.Empty;
                if (i == 1)
                {
                    title = "许可协议";
                }
                else if (i == 2)
                {
                    title = "环境检测";
                }
                else if (i == 3)
                {
                    title = "数据库设置";
                }
                else if (i == 4)
                {
                    title = "安装产品";
                }
                else if (i == 5)
                {
                    title = "安装完成";
                }
                builder.Append($@"<li{liClass}><img src=""{imageUrl}"" />{title}</li>");
            }

            return builder.ToString();
        }
	
		public void Page_Load(object sender, EventArgs e)
		{
            if (!IsPostBack)
            {
                var isInstalled = !AppManager.IsNeedInstall();

                if (isInstalled)
                {
                    Page.Response.Write("系统已安装成功，向导被禁用");
                    Page.Response.End();
                    return;
                }

                LtlVersionInfo.Text = $"SITESERVER {AppManager.GetFullVersion()}";
                LtlStepTitle.Text = GetSetpTitleString(1);

                DdlSqlDatabaseType.Items.Add(new ListItem
                {
                    Text = "SqlServer",
                    Value = "SqlServer"
                });
                DdlSqlDatabaseType.Items.Add(new ListItem
                {
                    Text = "MySql",
                    Value = "MySql"
                });
                EBooleanUtils.AddListItems(DdlIsProtectData, "加密", "不加密");
                ControlUtils.SelectListItemsIgnoreCase(DdlIsProtectData, true.ToString());
            }
		}

        protected void btnStep1_Click(object sender, EventArgs e)
        {
            if (ChkIAgree.Checked)
            {
                BtnStep2.Visible = true;
                LtlErrorMessage.Text = string.Empty;

                LtlDomain.Text = PageUtils.GetHost();
                LtlVersion.Text = AppManager.GetFullVersion();
                LtlNetVersion.Text = $"{Environment.Version.Major}.{Environment.Version.Minor}";
                LtlPhysicalApplicationPath.Text = WebConfigUtils.PhysicalApplicationPath;

                var isRootWritable = false;
                try
                {
                    var filePath = PathUtils.Combine(WebConfigUtils.PhysicalApplicationPath, "robots.txt");
                    FileUtils.WriteText(filePath, ECharset.utf_8, @"User-agent: *
Disallow: /SiteServer/
Disallow: /SiteFiles/
Disallow: /home/");
                    isRootWritable = true;
                }
                catch
                {
                    // ignored
                }
                var isSiteFilesWritable = false;
                try
                {
                    var filePath = PathUtils.Combine(WebConfigUtils.PhysicalApplicationPath, DirectoryUtils.SiteFiles.DirectoryName, "index.htm");
                    FileUtils.WriteText(filePath, ECharset.utf_8, StringUtils.Constants.Html5Empty);
                    isSiteFilesWritable = true;
                }
                catch
                {
                    // ignored
                }

                LtlRootWrite.Text = isRootWritable ? "<FONT color=green>[√]</FONT>" : "<FONT color=red>[×]</FONT>";

                LtlSiteFielsWrite.Text = isSiteFilesWritable ? "<FONT color=green>[√]</FONT>" : "<FONT color=red>[×]</FONT>";

                if (!isRootWritable || !isSiteFilesWritable)
                {
                    ShowErrorMessage("系统检测到文件夹权限不足，您需要赋予可写权限");
                    BtnStep2.Visible = false;
                }

                LtlStepTitle.Text = GetSetpTitleString(2);
            }
            else
            {
                ShowErrorMessage("您必须同意软件许可协议才能安装！");
            }
        }

        protected void BtnStep2_Click(object sender, EventArgs e)
        {
            LtlErrorMessage.Text = string.Empty;
            LtlStepTitle.Text = GetSetpTitleString(3);
        }

        protected void btnStep3_Click(object sender, EventArgs e)
        {
            LtlErrorMessage.Text = string.Empty;

            if (PhSql1.Visible)
            {
                HihSqlHiddenPassword.Value = TbSqlPassword.Text;
                bool isConnectValid;
                string errorMessage;
                List<string> databaseNameList = new List<string>();
                if (string.IsNullOrEmpty(TbSqlServer.Text) || string.IsNullOrEmpty(TbSqlUserName.Text))
                {
                    isConnectValid = false;
                    errorMessage = "数据库主机及数据库用户必须填写。";
                }
                else
                {
                    var connectionStringWithoutDatabaseName = GetConnectionString(false);
                    var isMySql = StringUtils.EqualsIgnoreCase(DdlSqlDatabaseType.SelectedValue, "MySql");
                    isConnectValid = BaiRongDataProvider.DatabaseDao.ConnectToServer(isMySql, connectionStringWithoutDatabaseName, out databaseNameList, out errorMessage);
                }
                
                if (isConnectValid)
                {
                    DdlSqlDatabaseName.Items.Clear();

                    foreach (var databaseName in databaseNameList)
                    {
                        DdlSqlDatabaseName.Items.Add(databaseName);
                    }

                    DdlSqlDatabaseType.Enabled = false;
                    PhSql1.Visible = false;
                    PhSql2.Visible = true;
                }
                else
                {
                    ShowErrorMessage(errorMessage);
                }
            }
            else
            {
                LtlStepTitle.Text = GetSetpTitleString(4);
            }
        }

        protected void btnStep4_Click(object sender, EventArgs e)
        {
            LtlErrorMessage.Text = string.Empty;

            string errorMessage;
            if (CheckLoginValid(out errorMessage))
            {
                if (InstallDatabase(out errorMessage))
                {
                    LtlStepTitle.Text = GetSetpTitleString(5);
                }
                else
                {
                    ShowErrorMessage(errorMessage);
                }
            }
            else
            {
                ShowErrorMessage(errorMessage);
            }
        }

        protected void btnPrevious_Click(object sender, EventArgs e)
		{
            LtlErrorMessage.Text = string.Empty;
            DdlSqlDatabaseType.Enabled = true;

            if (PhStep4.Visible)
            {
                LtlStepTitle.Text = GetSetpTitleString(3);
                PhSql1.Visible = true;
                PhSql2.Visible = false;
            }
            else if (PhStep3.Visible)
            {
                if (PhSql2.Visible)
                {
                    PhSql1.Visible = true;
                    PhSql2.Visible = false;
                }
                else
                {
                    LtlStepTitle.Text = GetSetpTitleString(2);
                }
            }
            else if (PhStep2.Visible)
            {
                LtlStepTitle.Text = GetSetpTitleString(1);
            }
		}

        private string GetConnectionString(bool isDatabaseName)
        {
            string connectionString = $"server={TbSqlServer.Text};uid={TbSqlUserName.Text};pwd={HihSqlHiddenPassword.Value}";
            if (isDatabaseName)
            {
                connectionString += $";database={DdlSqlDatabaseName.SelectedValue}";
            }
            return connectionString;
        }

        private bool CheckLoginValid(out string errorMessage)
        {
            errorMessage = string.Empty;

            if (string.IsNullOrEmpty(TbAdminName.Text))
            {
                errorMessage = "管理员用户名不能为空！";
                return false;
            }

            if (string.IsNullOrEmpty(TbAdminPassword.Text))
            {
                errorMessage = "管理员密码不能为空！";
                return false;
            }

            if (TbAdminPassword.Text.Length < 6)
            {
                errorMessage = "管理员密码必须大于6位！";
                return false;
            }

            if (TbAdminPassword.Text != TbComfirmAdminPassword.Text)
            {
                errorMessage = "两次输入的管理员密码不一致！";
                return false;
            }

            return true;
        }

        private bool InstallDatabase(out string errorMessage)
        {
            if (!UpdateWebConfig(out errorMessage)) return false;

            try
            {
                var errorBuilder = new StringBuilder();
                BaiRongDataProvider.DatabaseDao.Install(errorBuilder);

                BaiRongDataProvider.ConfigDao.InitializeConfig();
                BaiRongDataProvider.ConfigDao.InitializeUserRole(TbAdminName.Text, TbAdminPassword.Text);
                
                return true;
            }
            catch (Exception e)
            {
                errorMessage = e.Message;
                return false;
            }
        }

        private bool UpdateWebConfig(out string errorMessage)
        {
            errorMessage = string.Empty;

            var returnValue = false;
            
            try
            {
                var isProtectData = TranslateUtils.ToBool(DdlIsProtectData.SelectedValue);
                var isMySql = StringUtils.EqualsIgnoreCase(DdlSqlDatabaseType.SelectedValue, "MySql");
                var connectionString = GetConnectionString(true);

                WebConfigUtils.UpdateWebConfig(isProtectData, isMySql, connectionString);

                returnValue = true;
            }
            catch (Exception e)
            {
                errorMessage = e.Message;
            }

            return returnValue;
        }

        public string GetSiteServerUrl()
        {
            return PageUtils.GetAdminDirectoryUrl(string.Empty);
        }

        private void ShowErrorMessage(string errorMessage)
        {
            LtlErrorMessage.Text = $@"<img src=""images/check_error.gif"" /> {errorMessage}";
        }
	}
}
