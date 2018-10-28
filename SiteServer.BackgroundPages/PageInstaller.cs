using System;
using System.Collections.Generic;
using System.Security.Permissions;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using SiteServer.Utils;
using SiteServer.CMS.Core;
using SiteServer.Plugin;
using SiteServer.Utils.Enumerations;

namespace SiteServer.BackgroundPages
{
    public class PageInstaller : BasePage
	{
        public Literal LtlVersionInfo;

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
        public DropDownList DdlIsDefaultPort;
	    public PlaceHolder PhSqlPort;
	    public TextBox TbSqlPort;
        public TextBox TbSqlUserName;
        public TextBox TbSqlPassword;
        public HtmlInputHidden HihSqlHiddenPassword;
	    public PlaceHolder PhSqlOracleDatabase;
	    public TextBox TbSqlOracleDatabase;
        public PlaceHolder PhSql2;
        public DropDownList DdlSqlDatabaseName;

        public PlaceHolder PhStep4;
        public DropDownList DdlIsProtectData;
        public TextBox TbAdminPassword;
        public TextBox TbComfirmAdminPassword;
        public TextBox TbAdminName;

        public PlaceHolder PhStep5;
	    public Literal LtlGo;

        protected override bool IsSinglePage => true;

	    protected override bool IsAccessable => true;

	    protected override bool IsInstallerPage => true;

	    public static string GetRedirectUrl()
	    {
	        return PageUtils.GetSiteServerUrl("installer/default", null);
	    }

	    public void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack) return;

            if (!SystemManager.IsNeedInstall())
            {
                Page.Response.Write("系统已安装成功，向导被禁用");
                Page.Response.End();
                return;
            }

            LtlVersionInfo.Text = SystemManager.Version;
            SetSetp(1);

            DatabaseTypeUtils.AddListItems(DdlSqlDatabaseType);

            EBooleanUtils.AddListItems(DdlIsDefaultPort, "默认数据库端口", "自定义数据库端口");
            ControlUtils.SelectSingleItemIgnoreCase(DdlIsDefaultPort, true.ToString());

            PhSqlPort.Visible = false;

            EBooleanUtils.AddListItems(DdlIsProtectData, "加密", "不加密");
            ControlUtils.SelectSingleItemIgnoreCase(DdlIsProtectData, false.ToString());

            LtlGo.Text = $@"<a class=""btn btn-success m-l-5"" href=""{PageUtils.GetAdminUrl(string.Empty)}"">进入后台</a>";
        }

        public void DdlSqlDatabaseType_SelectedIndexChanged(object sender, EventArgs e)
        {
            var databaseType = DatabaseTypeUtils.GetEnumType(DdlSqlDatabaseType.SelectedValue);
            PhSqlOracleDatabase.Visible = databaseType == DatabaseType.Oracle;
        }

        public void DdlIsDefaultPort_SelectedIndexChanged(object sender, EventArgs e)
        {
            PhSqlPort.Visible = !TranslateUtils.ToBool(DdlIsDefaultPort.SelectedValue);
        }

        public void BtnStep1_Click(object sender, EventArgs e)
        {
            if (ChkIAgree.Checked)
            {
                BtnStep2.Visible = true;

                LtlDomain.Text = PageUtils.GetHost();
                LtlVersion.Text = SystemManager.Version;
                LtlNetVersion.Text = $"{Environment.Version.Major}.{Environment.Version.Minor}";
                LtlPhysicalApplicationPath.Text = WebConfigUtils.PhysicalApplicationPath;

                var isRootWritable = false;
                try
                {
                    var filePath = PathUtils.Combine(WebConfigUtils.PhysicalApplicationPath, "version.txt");
                    FileUtils.WriteText(filePath, ECharset.utf_8, SystemManager.Version);

                    var ioPermission = new FileIOPermission(FileIOPermissionAccess.Write, WebConfigUtils.PhysicalApplicationPath);
                    ioPermission.Demand();

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

                    var ioPermission = new FileIOPermission(FileIOPermissionAccess.Write, PathUtils.Combine(WebConfigUtils.PhysicalApplicationPath, DirectoryUtils.SiteFiles.DirectoryName));
                    ioPermission.Demand();

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
                    FailMessage("系统检测到文件夹权限不足，您需要赋予根目录 NETWORK SERVICE 以及 IIS_IUSRS 读写权限");
                    BtnStep2.Visible = false;
                }

                SetSetp(2);
            }
            else
            {
                FailMessage("您必须同意软件许可协议才能安装！");
            }
        }

        public void BtnStep2_Click(object sender, EventArgs e)
        {
            SetSetp(3);
        }

        public void BtnStep3_Click(object sender, EventArgs e)
        {
            if (PhSql1.Visible)
            {
                HihSqlHiddenPassword.Value = TbSqlPassword.Text;
                bool isConnectValid;
                string errorMessage;
                var databaseNameList = new List<string>();
                var databaseType = DatabaseTypeUtils.GetEnumType(DdlSqlDatabaseType.SelectedValue);
                if (string.IsNullOrEmpty(TbSqlServer.Text))
                {
                    isConnectValid = false;
                    errorMessage = "数据库主机必须填写。";
                }
                else if (PhSqlPort.Visible && string.IsNullOrEmpty(TbSqlPort.Text))
                {
                    isConnectValid = false;
                    errorMessage = "数据库端口必须填写。";
                }
                else if (string.IsNullOrEmpty(TbSqlUserName.Text))
                {
                    isConnectValid = false;
                    errorMessage = "数据库用户必须填写。";
                }
                else if (databaseType == DatabaseType.Oracle && string.IsNullOrEmpty(TbSqlOracleDatabase.Text))
                {
                    isConnectValid = false;
                    errorMessage = "数据库名称必须填写。";
                }
                else
                {
                    var connectionStringWithoutDatabaseName = GetConnectionString(databaseType == DatabaseType.Oracle);
                    isConnectValid = DataProvider.DatabaseDao.ConnectToServer(databaseType, connectionStringWithoutDatabaseName, out databaseNameList, out errorMessage);
                }
                
                if (isConnectValid)
                {
                    if (databaseType != DatabaseType.Oracle)
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
                        SetSetp(4);
                    }
                }
                else
                {
                    FailMessage(errorMessage);
                }
            }
            else
            {
                SetSetp(4);
            }
        }

        public void BtnStep4_Click(object sender, EventArgs e)
        {
            string errorMessage;
            if (CheckLoginValid(out errorMessage))
            {
                if (InstallDatabase(out errorMessage))
                {
                    SetSetp(5);
                }
                else
                {
                    FailMessage(errorMessage);
                }
            }
            else
            {
                FailMessage(errorMessage);
            }
        }

        public void BtnPrevious_Click(object sender, EventArgs e)
		{
            DdlSqlDatabaseType.Enabled = true;

            if (PhStep4.Visible)
            {
                SetSetp(3);
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
                    SetSetp(2);
                }
            }
            else if (PhStep2.Visible)
            {
                SetSetp(1);
            }
		}

        private void SetSetp(int step)
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
        }

        private string GetConnectionString(bool isDatabaseName)
        {
            var databaseType = DatabaseTypeUtils.GetEnumType(DdlSqlDatabaseType.SelectedValue);
            var databaseName = string.Empty;
            if (isDatabaseName)
            {
                databaseName = databaseType == DatabaseType.Oracle ? TbSqlOracleDatabase.Text : DdlSqlDatabaseName.SelectedValue;
            }
            return WebConfigUtils.GetConnectionString(databaseType, TbSqlServer.Text, TranslateUtils.ToBool(DdlIsDefaultPort.SelectedValue), TranslateUtils.ToInt(TbSqlPort.Text), TbSqlUserName.Text, HihSqlHiddenPassword.Value, databaseName);
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
            if (!EUserPasswordRestrictionUtils.IsValid(TbAdminPassword.Text, EUserPasswordRestrictionUtils.GetValue(EUserPasswordRestriction.LetterAndDigit)))
            {
                errorMessage =
                    $"密码不符合规则，请包含{EUserPasswordRestrictionUtils.GetText(EUserPasswordRestriction.LetterAndDigit)}";
                return false;
            }
            return true;
        }

        private bool InstallDatabase(out string errorMessage)
        {
            if (!UpdateWebConfig(out errorMessage)) return false;

            try
            {
                SystemManager.InstallDatabase(TbAdminName.Text, TbAdminPassword.Text);
                
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
                var databaseType = DatabaseTypeUtils.GetEnumType(DdlSqlDatabaseType.SelectedValue);
                var connectionString = GetConnectionString(true);

                WebConfigUtils.UpdateWebConfig(isProtectData, databaseType, connectionString, "api", "SiteServer", "Home", StringUtils.GetShortGuid(), false);

                DataProvider.Reset();

                returnValue = true;
            }
            catch (Exception e)
            {
                errorMessage = e.Message;
            }

            return returnValue;
        }
	}
}
