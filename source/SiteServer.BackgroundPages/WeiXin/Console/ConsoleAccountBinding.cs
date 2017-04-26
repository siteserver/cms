using System;
using System.Web.UI.WebControls;
using BaiRong.Core;
using SiteServer.CMS.BackgroundPages;
using SiteServer.WeiXin.Core;
using SiteServer.WeiXin.Model;
using BaiRong.Core.Net;

namespace SiteServer.WeiXin.BackgroundPages
{
    public class ConsoleAccountBinding : BackgroundBasePage
    {
        public PlaceHolder phStep1;
        public RadioButtonList rblWXAccountType;
        public TextBox tbWhchatID;

        public PlaceHolder phStep2;
        public Literal ltlURL;
        public Literal ltlToken;

        public PlaceHolder phStep3;
        public TextBox tbAppID;
        public TextBox tbAppSecret;

        private string returnUrl;

        public static string GetRedirectUrl(int publishmentSystemID, string returnUrl)
        {
            return PageUtils.GetWXUrl(
                $"console_accountBinding.aspx?publishmentSystemID={publishmentSystemID}&returnUrl={StringUtils.ValueToUrl(returnUrl)}");
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("PublishmentSystemID");
            returnUrl = StringUtils.ValueFromUrl(GetQueryString("returnUrl"));

            if (!IsPostBack)
            {
                //base.BreadCrumbConsole(AppManager.CMS.LeftMenu.ID_Site, "绑定微信公众帐号", AppManager.Permission.Platform_Site);


                #region 确认绑定接口没有问题
                if (!IsNet45OrNewer())
                {
                    FailMessage("请检查是否安装了.NET Framework 4.5以上版本");
                    return;
                }

                var hostUrl = PageUtils.GetHost();
                if (hostUrl.IndexOf(":") >= 0)
                {
                    var port = hostUrl.Split(new char[] { ':' })[1];
                    if (port != "80")
                    {
                        FailMessage("请检查站点是否设置为80端口");
                        return;
                    }
                }
                
                var testUrl = PageUtils.AddProtocolToUrl("/api/mp/url?id=1");
                var result = string.Empty;
                WebClientUtils.Post(testUrl, string.Empty, out result);
                if (!StringUtils.EqualsIgnoreCase(result, "failed:id=1") && !StringUtils.EqualsIgnoreCase(result, "参数错误"))
                {
                    FailMessage("绑定微信公众账号需要的api有问题! 详细错误信息：" + result);
                    return;
                }


                #endregion

                var accountInfo = WeiXinManager.GetAccountInfo(PublishmentSystemID);

                EWXAccountTypeUtils.AddListItems(rblWXAccountType);
                ControlUtils.SelectListItems(rblWXAccountType, EWXAccountTypeUtils.GetValue(EWXAccountTypeUtils.GetEnumType(accountInfo.AccountType)));

                tbWhchatID.Text = accountInfo.WeChatID;

                ltlURL.Text = PageUtilityWX.API.GetMPUrl(PublishmentSystemID);

                ltlToken.Text = accountInfo.Token;

                tbAppID.Text = accountInfo.AppID;
                tbAppSecret.Text = accountInfo.AppSecret;
            }
        }

        public static bool IsNet45OrNewer()
        {
            // Class "ReflectionContext" exists from .NET 4.5 onwards.
            return Type.GetType("System.Reflection.ReflectionContext", false) != null;
        }

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            if (Page.IsPostBack && Page.IsValid)
            {
                if (phStep1.Visible)
                {
                    phStep2.Visible = true;
                    phStep3.Visible = phStep1.Visible = false;
                }
                else if (phStep2.Visible)
                {
                    var accountType = EWXAccountTypeUtils.GetEnumType(rblWXAccountType.SelectedValue);
                    if (accountType == EWXAccountType.Subscribe)
                    {
                        var accountInfo = WeiXinManager.GetAccountInfo(PublishmentSystemID);

                        accountInfo.AccountType = rblWXAccountType.SelectedValue;
                        accountInfo.WeChatID = tbWhchatID.Text;

                        try
                        {
                            DataProviderWX.AccountDAO.Update(accountInfo);

                            LogUtils.AddLog(BaiRongDataProvider.AdministratorDao.UserName, "绑定微信公众帐号",
                                $"站点:{PublishmentSystemInfo.PublishmentSystemName}");

                            SuccessMessage("绑定微信公众帐号成功！");
                            AddWaitAndRedirectScript(returnUrl);
                        }
                        catch (Exception ex)
                        {
                            FailMessage(ex, "绑定微信公众帐号失败！");
                        }
                    }
                    else
                    {
                        phStep3.Visible = true;
                        phStep1.Visible = phStep2.Visible = false;
                    }
                }
                else
                {
                    var accountInfo = WeiXinManager.GetAccountInfo(PublishmentSystemID);

                    accountInfo.AccountType = rblWXAccountType.SelectedValue;
                    accountInfo.WeChatID = tbWhchatID.Text;
                    accountInfo.AppID = tbAppID.Text;
                    accountInfo.AppSecret = tbAppSecret.Text;

                    try
                    {
                        DataProviderWX.AccountDAO.Update(accountInfo);

                        LogUtils.AddLog(BaiRongDataProvider.AdministratorDao.UserName, "绑定微信公众帐号",
                            $"站点:{PublishmentSystemInfo.PublishmentSystemName}");

                        SuccessMessage("绑定微信公众帐号成功！");
                        AddWaitAndRedirectScript(returnUrl);
                    }
                    catch (Exception ex)
                    {
                        FailMessage(ex, "绑定微信公众帐号失败！");
                    }
                }
            }
        }

        public void Return_OnClick(object sender, EventArgs e)
        {
            PageUtils.Redirect(returnUrl);
        }
    }
}
