using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Model;
using BaiRong.Core.Model.Enumerations;
using SiteServer.BackgroundPages.Controls;

namespace SiteServer.BackgroundPages.User
{
    public class PageThirdLoginConfiguration : BasePage
    {
        public TextBox tbThirdLoginName;
        public Literal ltlThirdLoginType;

        public PlaceHolder phLoginAuth;
        public TextBox tbLoginAuthAppKey;
        public TextBox tbLoginAuthAppSercet;
        public TextBox tbLoginAuthCallBackUrl;
        public Literal ltlLoginAuthCallBackUrl;

        public UEditor breDescription;

        private int _thirdLoginId;

        public static string GetRedirectUrl(int thirdLoginId)
        {
            return PageUtils.GetUserUrl(nameof(PageThirdLoginConfiguration), new NameValueCollection
            {
                {"thirdLoginID", thirdLoginId.ToString()}
            });
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            _thirdLoginId = Body.GetQueryInt("thirdLoginID");

            if (!IsPostBack)
            {
                BreadCrumbUser(AppManager.User.LeftMenu.UserConfiguration, "授权登录管理", AppManager.User.Permission.UserConfiguration);

                //var publishmentSystemUrl = AssetsUtils.GetUrl(PageUtils.GetApiUrl(), "iframe/authLogin.html");
                var publishmentSystemUrl = string.Empty;
                ltlLoginAuthCallBackUrl.Text = "<span style='color:red;'>" + publishmentSystemUrl + "</span>";
                tbLoginAuthCallBackUrl.Text = publishmentSystemUrl;
                if (_thirdLoginId > 0)
                {
                    var thirdLoginInfo = BaiRongDataProvider.ThirdLoginDao.GetThirdLoginInfo(_thirdLoginId);
                    if (thirdLoginInfo != null)
                    {
                        tbThirdLoginName.Text = thirdLoginInfo.ThirdLoginName;
                        ltlThirdLoginType.Text = EThirdLoginTypeUtils.GetText(thirdLoginInfo.ThirdLoginType);

                        var authInfo = new ThirdLoginAuthInfo(thirdLoginInfo.SettingsXml);

                        phLoginAuth.Visible = true;
                        tbLoginAuthAppKey.Text = authInfo.AppKey;
                        tbLoginAuthAppSercet.Text = authInfo.AppSercet;
                        tbLoginAuthCallBackUrl.Text = authInfo.CallBackUrl;

                        breDescription.Text = thirdLoginInfo.Description;
                    }
                }
            }
        }

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            try
            {
                var thirdLoginInfo = new ThirdLoginInfo();
                if (_thirdLoginId > 0)
                {
                    thirdLoginInfo = BaiRongDataProvider.ThirdLoginDao.GetThirdLoginInfo(_thirdLoginId);
                }

                thirdLoginInfo.ThirdLoginName = tbThirdLoginName.Text;

                var authInfo = new ThirdLoginAuthInfo(thirdLoginInfo.SettingsXml)
                {
                    AppKey = tbLoginAuthAppKey.Text,
                    AppSercet = tbLoginAuthAppSercet.Text
                };
                ;
                authInfo.CallBackUrl = tbLoginAuthCallBackUrl.Text; ;

                thirdLoginInfo.SettingsXml = authInfo.ToString();

                thirdLoginInfo.Description = breDescription.Text;
                thirdLoginInfo.IsEnabled = true;//设置成功之后，启用

                if (_thirdLoginId > 0)
                {
                    BaiRongDataProvider.ThirdLoginDao.Update(thirdLoginInfo);
                }
                else
                {
                    BaiRongDataProvider.ThirdLoginDao.Insert(thirdLoginInfo);
                }

                SuccessMessage("配置登录方式成功！");

                AddWaitAndRedirectScript(PageThirdLogin.GetRedirectUrl());
            }
            catch (Exception ex)
            {
                FailMessage(ex, ex.Message);
            }
        }
    }
}
