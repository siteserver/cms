using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Model;
using BaiRong.Core.Model.Enumerations;

namespace SiteServer.BackgroundPages.User
{
    public class PageThirdLogin : BasePage
    {
        public Repeater rptInstalled;
        public Repeater rptUnInstalled;

        private List<ThirdLoginInfo> _thirdLoginInfoList;

        public static string GetRedirectUrl()
        {
            return PageUtils.GetUserUrl(nameof(PageThirdLogin), null);
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            if (Body.IsQueryExists("isInstall") && Body.IsQueryExists("thirdLoginType"))
            {
                var thirdLoginType = EThirdLoginTypeUtils.GetEnumType(Body.GetQueryString("thirdLoginType"));

                if (!BaiRongDataProvider.ThirdLoginDao.IsExists(thirdLoginType))
                {
                    //安装之后，默认不可用
                    var thirdLoginInfo = new ThirdLoginInfo(0, thirdLoginType, EThirdLoginTypeUtils.GetText(thirdLoginType), false, 0, EThirdLoginTypeUtils.GetDescription(thirdLoginType), string.Empty);

                    BaiRongDataProvider.ThirdLoginDao.Insert(thirdLoginInfo);
                    //安装之后，直接跳转到设置页面
                    Response.Redirect(PageThirdLoginConfiguration.GetRedirectUrl((int)thirdLoginType));
                    //base.SuccessMessage("登录方式安装成功");
                }
            }
            else if (Body.IsQueryExists("isDelete") && Body.IsQueryExists("thirdLoginID"))
            {
                var thirdLoginId = Body.GetQueryInt("thirdLoginID");
                if (thirdLoginId > 0)
                {
                    BaiRongDataProvider.ThirdLoginDao.Delete(thirdLoginId);
                    SuccessMessage("登录方式删除成功");
                }
            }
            else if (Body.IsQueryExists("isEnable") && Body.IsQueryExists("thirdLoginID"))
            {
                var thirdLoginId = Body.GetQueryInt("thirdLoginID");
                if (thirdLoginId > 0)
                {
                    var thirdLoginInfo = BaiRongDataProvider.ThirdLoginDao.GetThirdLoginInfo(thirdLoginId);
                    if (thirdLoginInfo != null)
                    {
                        var authInfo = new ThirdLoginAuthInfo(thirdLoginInfo.SettingsXml);
                        if (string.IsNullOrEmpty(authInfo.AppKey) || string.IsNullOrEmpty(authInfo.AppSercet) || string.IsNullOrEmpty(authInfo.CallBackUrl))
                        {
                            FailMessage("请先对第三方登录方式进行设置，设置之后才能启用！");
                        }
                        else
                        {
                            var action = thirdLoginInfo.IsEnabled ? "禁用" : "启用";
                            thirdLoginInfo.IsEnabled = !thirdLoginInfo.IsEnabled;
                            BaiRongDataProvider.ThirdLoginDao.Update(thirdLoginInfo);
                            SuccessMessage($"成功{action}登录方式");
                        }
                    }
                }
            }
            else if (Body.IsQueryExists("setTaxis"))
            {
                var thirdLoginId = Body.GetQueryInt("thirdLoginID");
                var direction = Body.GetQueryString("direction");
                if (thirdLoginId > 0)
                {

                    switch (direction.ToUpper())
                    {
                        case "UP":
                            BaiRongDataProvider.ThirdLoginDao.UpdateTaxisToUp(thirdLoginId);
                            break;
                        case "DOWN":
                            BaiRongDataProvider.ThirdLoginDao.UpdateTaxisToDown(thirdLoginId);
                            break;
                    }
                    SuccessMessage("排序成功！");
                    AddWaitAndRedirectScript(GetRedirectUrl());
                }
            }

            if (!IsPostBack)
            {
                BreadCrumbUser(AppManager.User.LeftMenu.UserConfiguration, "授权登录管理", AppManager.User.Permission.UserConfiguration);

                _thirdLoginInfoList = BaiRongDataProvider.ThirdLoginDao.GetThirdLoginInfoList();

                rptInstalled.DataSource = _thirdLoginInfoList;
                rptInstalled.ItemDataBound += rptInstalled_ItemDataBound;
                rptInstalled.DataBind();

                rptUnInstalled.DataSource = EThirdLoginTypeUtils.GetEThirdLoginTypeList();
                rptUnInstalled.ItemDataBound += rptUnInstalled_ItemDataBound;
                rptUnInstalled.DataBind();
            }
        }

        private void rptInstalled_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var thirdLoginInfo = e.Item.DataItem as ThirdLoginInfo;
                if (thirdLoginInfo == null)
                {
                    e.Item.Visible = false;
                    return;
                }

                var ltlThirdLoginName = e.Item.FindControl("ltlThirdLoginName") as Literal;
                var ltlDescription = e.Item.FindControl("ltlDescription") as Literal;
                var ltlIsEnabled = e.Item.FindControl("ltlIsEnabled") as Literal;
                var hlUpLink = e.Item.FindControl("hlUpLink") as HyperLink;
                var hlDownLink = e.Item.FindControl("hlDownLink") as HyperLink;
                var ltlConfigUrl = e.Item.FindControl("ltlConfigUrl") as Literal;
                var ltlIsEnabledUrl = e.Item.FindControl("ltlIsEnabledUrl") as Literal;
                var ltlDeleteUrl = e.Item.FindControl("ltlDeleteUrl") as Literal;

                if (ltlThirdLoginName != null)
                {
                    ltlThirdLoginName.Text = thirdLoginInfo.ThirdLoginName;
                }
                if (ltlDescription != null)
                {
                    ltlDescription.Text = StringUtils.MaxLengthText(thirdLoginInfo.Description, 200);
                }
                if (ltlIsEnabled != null)
                {
                    ltlIsEnabled.Text = StringUtils.GetTrueOrFalseImageHtml(thirdLoginInfo.IsEnabled);
                }
                if (hlUpLink != null)
                {
                    hlUpLink.NavigateUrl = GetRedirectUrl() + $"?setTaxis=True&thirdLoginID={thirdLoginInfo.Id}&direction=UP";
                }
                if (hlDownLink != null)
                {
                    hlDownLink.NavigateUrl = GetRedirectUrl() + $"?setTaxis=True&thirdLoginID={thirdLoginInfo.Id}&direction=DOWN";
                }

                var urlConfig = PageThirdLoginConfiguration.GetRedirectUrl(thirdLoginInfo.Id);
                if (ltlConfigUrl != null) ltlConfigUrl.Text = $@"<a href=""{urlConfig}"">设置</a>";

                var action = thirdLoginInfo.IsEnabled ? "禁用" : "启用";
                var urlIsEnabled = GetRedirectUrl() + $"?isEnable=True&thirdLoginID={thirdLoginInfo.Id}";
                if (ltlIsEnabledUrl != null) ltlIsEnabledUrl.Text = $@"<a href=""{urlIsEnabled}"">{action}</a>";

                var urlDelete = GetRedirectUrl() + $"?isDelete=True&thirdLoginID={thirdLoginInfo.Id}";
                if (ltlDeleteUrl != null)
                {
                    ltlDeleteUrl.Text =
                        $@"<a href=""{urlDelete}"" onclick=""javascript:return confirm('此操作将删除选定的登录方式，确认吗？');"">删除</a>";
                }
            }
        }

        private void rptUnInstalled_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var thirdLoginType = (EThirdLoginType)e.Item.DataItem;

                foreach (var thirdLoginInfo in _thirdLoginInfoList)
                {
                    if (thirdLoginInfo.ThirdLoginType == thirdLoginType)
                    {
                        e.Item.Visible = false;
                        return;
                    }
                }

                var ltlThirdLoginName = e.Item.FindControl("ltlThirdLoginName") as Literal;
                var ltlDescription = e.Item.FindControl("ltlDescription") as Literal;
                var ltlInstallUrl = e.Item.FindControl("ltlInstallUrl") as Literal;

                if (ltlThirdLoginName != null) ltlThirdLoginName.Text = EThirdLoginTypeUtils.GetText(thirdLoginType);
                if (ltlDescription != null) ltlDescription.Text = EThirdLoginTypeUtils.GetDescription(thirdLoginType);

                var urlInstall = GetRedirectUrl() + $"?isInstall=True&thirdLoginType={EThirdLoginTypeUtils.GetValue(thirdLoginType)}";
                if (ltlInstallUrl != null) ltlInstallUrl.Text = $@"<a href=""{urlInstall}"">安装</a>";
            }
        }
    }
}
