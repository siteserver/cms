using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Net;
using SiteServer.CMS.WeiXin.Data;
using SiteServer.CMS.WeiXin.IO;
using SiteServer.CMS.WeiXin.Manager;
using SiteServer.CMS.WeiXin.Model.Enumerations;

namespace SiteServer.BackgroundPages.WeiXin
{
    public class PageAccountBinding : BasePageCms
    {
        public PlaceHolder PhStep1;
        public RadioButtonList RblWxAccountType;
        public TextBox TbWhchatId;

        public PlaceHolder PhStep2;
        public Literal LtlUrl;
        public Literal LtlToken;

        public PlaceHolder PhStep3;
        public TextBox TbAppId;
        public TextBox TbAppSecret;

        private string _returnUrl;

        public static string GetRedirectUrl(int publishmentSystemId, string returnUrl)
        {
            return PageUtils.GetWeiXinUrl(nameof(PageAccountBinding), new NameValueCollection
            {
                {"publishmentSystemId", publishmentSystemId.ToString()},
                {"returnUrl", StringUtils.ValueToUrl(returnUrl)}
            });
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("PublishmentSystemId");
            _returnUrl = StringUtils.ValueFromUrl(Body.GetQueryString("returnUrl"));

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

                var accountInfo = WeiXinManager.GetAccountInfo(PublishmentSystemId);

                EWxAccountTypeUtils.AddListItems(RblWxAccountType);
                ControlUtils.SelectListItems(RblWxAccountType, EWxAccountTypeUtils.GetValue(EWxAccountTypeUtils.GetEnumType(accountInfo.AccountType)));

                TbWhchatId.Text = accountInfo.WeChatId;

                LtlUrl.Text = PageUtilityWX.API.GetMPUrl(PublishmentSystemId);

                LtlToken.Text = accountInfo.Token;

                TbAppId.Text = accountInfo.AppId;
                TbAppSecret.Text = accountInfo.AppSecret;
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
                if (PhStep1.Visible)
                {
                    PhStep2.Visible = true;
                    PhStep3.Visible = PhStep1.Visible = false;
                }
                else if (PhStep2.Visible)
                {
                    var accountType = EWxAccountTypeUtils.GetEnumType(RblWxAccountType.SelectedValue);
                    if (accountType == EWxAccountType.Subscribe)
                    {
                        var accountInfo = WeiXinManager.GetAccountInfo(PublishmentSystemId);

                        accountInfo.AccountType = RblWxAccountType.SelectedValue;
                        accountInfo.WeChatId = TbWhchatId.Text;

                        try
                        {
                            DataProviderWx.AccountDao.Update(accountInfo);

                            LogUtils.AddAdminLog(Body.AdministratorName, "绑定微信公众帐号", $"站点:{PublishmentSystemInfo.PublishmentSystemName}");

                            SuccessMessage("绑定微信公众帐号成功！");
                            AddWaitAndRedirectScript(_returnUrl);
                        }
                        catch (Exception ex)
                        {
                            FailMessage(ex, "绑定微信公众帐号失败！");
                        }
                    }
                    else
                    {
                        PhStep3.Visible = true;
                        PhStep1.Visible = PhStep2.Visible = false;
                    }
                }
                else
                {
                    var accountInfo = WeiXinManager.GetAccountInfo(PublishmentSystemId);

                    accountInfo.AccountType = RblWxAccountType.SelectedValue;
                    accountInfo.WeChatId = TbWhchatId.Text;
                    accountInfo.AppId = TbAppId.Text;
                    accountInfo.AppSecret = TbAppSecret.Text;

                    try
                    {
                        DataProviderWx.AccountDao.Update(accountInfo);

                        LogUtils.AddAdminLog(Body.AdministratorName, "绑定微信公众帐号",
                            $"站点:{PublishmentSystemInfo.PublishmentSystemName}");

                        SuccessMessage("绑定微信公众帐号成功！");
                        AddWaitAndRedirectScript(_returnUrl);
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
            PageUtils.Redirect(_returnUrl);
        }
    }
}
