using System;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using BaiRong.Core;

namespace SiteServer.BackgroundPages
{
    public class PageUpgrade : Page
    {
        public Literal LtlVersionInfo;
        public Literal LtlStepTitle;
        public Literal LtlErrorMessage;
        public PlaceHolder PhStep1;
        public CheckBox ChkIAgree;
        public PlaceHolder PhStep2;
        public PlaceHolder PhStep3;

        private string GetSetpTitleString(int step)
        {
            PhStep1.Visible = PhStep2.Visible = PhStep3.Visible = false;
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
            }

            var builder = new StringBuilder();

            for (var i = 1; i <= 3; i++)
            {
                var liClass = string.Empty;
                if (i == step)
                {
                    liClass = @" class=""current""";
                }
                string imageUrl = $"../installer/images/step{i}{((i <= step) ? "a" : "b")}.gif";
                var title = string.Empty;
                if (i == 1)
                {
                    title = "许可协议";
                }
                else if (i == 2)
                {
                    title = "系统升级";
                }
                else if (i == 3)
                {
                    title = "升级完成";
                }
                builder.Append($@"<li{liClass}><img src=""{imageUrl}"" />{title}</li>");
            }

            return builder.ToString();
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (TranslateUtils.ToBool(Request.QueryString["done"]))
            {
                LtlStepTitle.Text = GetSetpTitleString(3);
            }
            else
            {
                if (IsNeedUpgrade(Page))
                {
                    if (!IsPostBack)
                    {
                        LtlVersionInfo.Text = $"SITESERVER {AppManager.GetFullVersion()}";
                        LtlStepTitle.Text = GetSetpTitleString(1);
                    }
                }
            }
        }

        public bool IsNeedUpgrade(Page page)
        {
            CacheUtils.Clear();

            if (BaiRongDataProvider.ConfigDao.GetDatabaseVersion() == AppManager.Version)
            {
                page.Response.Write($"<h2>当前版本为“{AppManager.Version}”,数据库版本与系统版本一致，无需升级</h2>");
                page.Response.End();
                return false;
            }

            return true;
        }

        protected void PhStep1_Click(object sender, EventArgs e)
        {
            if (ChkIAgree.Checked)
            {
                LtlErrorMessage.Text = string.Empty;

                string errorMessage;
                AppManager.Upgrade(AppManager.Version, out errorMessage);
                LtlStepTitle.Text = GetSetpTitleString(2);
                LtlErrorMessage.Text = errorMessage;
            }
            else
            {
                ShowErrorMessage("您必须同意软件许可协议才能进行升级！");
            }
        }

        public string GetSiteServerUrl()
        {
            return PageUtils.GetAdminDirectoryUrl(string.Empty);
        }

        private void ShowErrorMessage(string errorMessage)
        {
            LtlErrorMessage.Text = $@"<img src=""../installer/images/check_error.gif"" /> {errorMessage}";
        }
    }
}
