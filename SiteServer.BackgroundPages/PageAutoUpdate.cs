using System;
using System.Web.UI.WebControls;
using SiteServer.Utils;
using SiteServer.CMS.Core;

namespace SiteServer.BackgroundPages
{
    public class PageAutoUpdate : BasePage
    {
        public Literal LtlVersionInfo;

        public PlaceHolder PhStep1;
        public PlaceHolder PhStep2;
        public PlaceHolder PhStep3;
        public PlaceHolder PhStep4;
        public PlaceHolder PhStep5;
        public Literal LtlGo;

        protected override bool IsSinglePage => true;

        public static string GetRedirectUrl()
        {
            return PageUtils.GetSiteServerUrl(nameof(PageAutoUpdate), null);
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack) return;

            if (SystemManager.IsNeedInstall())
            {
                Page.Response.Write("系统未安装，向导被禁用");
                Page.Response.End();
                return;
            }

            CacheUtils.ClearAll();

            LtlVersionInfo.Text = AppManager.GetFullVersion();
            LtlGo.Text = $@"<a class=""btn btn-success m-l-5"" href=""{PageUtils.GetAdminDirectoryUrl(string.Empty)}"">进入后台</a>";
        }

        public void BtnStep1_Click(object sender, EventArgs e)
        {
            SetSetp(2);
        }

        public void BtnStep2_Click(object sender, EventArgs e)
        {
            SetSetp(3);
        }

        public void BtnStep3_Click(object sender, EventArgs e)
        {
            SetSetp(4);
        }

        public void BtnStep4_Click(object sender, EventArgs e)
        {
            SystemManager.Update();
            SetSetp(5);
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
    }
}
