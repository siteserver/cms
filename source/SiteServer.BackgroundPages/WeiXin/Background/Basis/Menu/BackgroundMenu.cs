using System.Web.UI.WebControls;
using BaiRong.Core;
using System;
using BaiRong.Core.Model;
using System.Text;
using SiteServer.WeiXin.Core;
using System.Collections.Generic;
using SiteServer.WeiXin.Model;
using Senparc.Weixin.MP.CommonAPIs;
using Senparc.Weixin.MP;
using SiteServer.CMS.Core;

namespace SiteServer.WeiXin.BackgroundPages
{
    public class BackgroundMenu : BackgroundBasePageWX
    {
        public Literal ltlMenu;
        public Literal ltlIFrame;

        private int parentID;
        private int menuID;

        public static string GetRedirectUrl(int publishmentSystemID, int parentID, int menuID)
        {
            return PageUtils.GetWXUrl(
                $"background_menu.aspx?publishmentSystemID={publishmentSystemID}&parentID={parentID}&menuID={menuID}");
        }

        public static string GetDeleteRedirectUrl(int publishmentSystemID, int parentID, int menuID)
        {
            return PageUtils.GetWXUrl(
                $"background_menu.aspx?publishmentSystemID={publishmentSystemID}&parentID={parentID}&menuID={menuID}&Delete=True");
        }

        public static string GetSubtractRedirectUrl(int publishmentSystemID, int parentID, int menuID)
        {
            return PageUtils.GetWXUrl(
                $"background_menu.aspx?publishmentSystemID={publishmentSystemID}&parentID={parentID}&menuID={menuID}&Subtract=True");
        }


        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            menuID = TranslateUtils.ToInt(GetQueryString("menuID"));
            parentID = TranslateUtils.ToInt(GetQueryString("parentID"));

            if (Request.QueryString["Delete"] != null && menuID > 0)
            {
                DataProviderWX.MenuDAO.Delete(menuID);
                SuccessMessage("菜单删除成功！");
            }
            if (Request.QueryString["Subtract"] != null && menuID > 0)
            {
                DataProviderWX.MenuDAO.UpdateTaxisToUp(parentID, menuID);
                SuccessMessage("菜单排序成功！");
            }
            if (!IsPostBack)
            {

                BreadCrumb(AppManager.WeiXin.LeftMenu.ID_Accounts, AppManager.WeiXin.LeftMenu.Function.ID_Menu, string.Empty, AppManager.WeiXin.Permission.WebSite.Menu);
                var accountInfo = WeiXinManager.GetAccountInfo(PublishmentSystemID);
                if (EWXAccountTypeUtils.Equals(accountInfo.AccountType, EWXAccountType.Subscribe))
                {
                    PageUtils.RedirectToErrorPage(@"您的微信公众账号类型为订阅号（非认证），微信目前不支持订阅号自定义菜单。如果您的公众账号类型不是订阅号，请到账户信息中设置微信绑定账号。");
                    return;
                }

                ltlIFrame.Text = @"<iframe frameborder=""0"" id=""menu"" name=""menu"" width=""100%"" height=""500""></iframe>";

                var menuInfoList = DataProviderWX.MenuDAO.GetMenuInfoList(PublishmentSystemID, 0);

                var builder = new StringBuilder();

                foreach (var menuInfo in menuInfoList)
                {
                    var subMenuInfoList = DataProviderWX.MenuDAO.GetMenuInfoList(PublishmentSystemID, menuInfo.MenuID);

                    var subBuilder = new StringBuilder();

                    if (subMenuInfoList.Count < 5)
                    {
                        var addSubUrl = BackgroundMenuAdd.GetRedirectUrl(PublishmentSystemID, menuInfo.MenuID, 0);
                        subBuilder.AppendFormat(@"
                            <dd class=""add"">
                              <a href=""{0}"" target=""menu""><font>新增菜单</font></a>
                            </dd>", addSubUrl);
                    }

                    var i = 0;
                    foreach (var subMenuInfo in subMenuInfoList)
                    {
                        i++;

                        var ddClass = i == subMenuInfoList.Count ? "last" : string.Empty;
                        var editSubUrl = BackgroundMenuAdd.GetRedirectUrl(PublishmentSystemID, subMenuInfo.ParentID, subMenuInfo.MenuID);
                        var deleteSubUrl = GetDeleteRedirectUrl(PublishmentSystemID, subMenuInfo.ParentID, subMenuInfo.MenuID);
                        var subtractSubUrl = GetSubtractRedirectUrl(PublishmentSystemID, subMenuInfo.ParentID, subMenuInfo.MenuID);


                        subBuilder.AppendFormat(@"                                                                   
                            <dd class=""{0}"">
                              <a href=""{1}"" target=""menu""><font>{2}</font></a>
                              <a href=""{3}"" onclick=""javascript:return confirm('此操作将删除子菜单“{2}”，确认吗？');"" class=""delete""><img src=""images/iphone-btn-delete.png""></a>
                              <a href=""{4}"" style='top:20px;' class=""delete""><img src=""images/iphone-btn-up.png""></a>
                            </dd>", ddClass, editSubUrl, subMenuInfo.MenuName, deleteSubUrl, subtractSubUrl);
                    }

                    var editUrl = BackgroundMenuAdd.GetRedirectUrl(PublishmentSystemID, menuInfo.ParentID, menuInfo.MenuID);
                    var subMenuStyle = parentID == menuInfo.MenuID ? string.Empty : "display:none";
                    var deleteUrl = GetDeleteRedirectUrl(PublishmentSystemID, menuInfo.ParentID, menuInfo.MenuID);
                    builder.AppendFormat(@"
                    <li class=""secondMenu"">
                        <a href=""{0}"" class=""mainMenu"" target=""menu""><font>{1}</font></a>
                        <dl class=""subMenus"" style=""{2}"">
                            <span>
                                <img width=""9"" height=""6"" src=""images/iphone-btn-tri.png"">
                            </span>
                            {3}
                        </dl>
                        <a href=""{4}"" onclick=""javascript:return confirm('此操作将删除主菜单“{1}”，确认吗？');"" class=""delete""><img src=""images/iphone-btn-delete.png""></a>
                    </li>", editUrl, menuInfo.MenuName, subMenuStyle, subBuilder.ToString(), deleteUrl);
                }

                if (menuInfoList.Count < 3)
                {
                    var addUrl = BackgroundMenuAdd.GetRedirectUrl(PublishmentSystemID, 0, 0);
                    builder.AppendFormat(@"
                    <li class=""secondMenu addMain"">
                        <a href=""{0}"" class=""mainMenu"" target=""menu""><font>新增菜单</font></a>
                    </li>", addUrl);
                }

                ltlMenu.Text = builder.ToString();
            }
        }

        public void Sync_OnClick(object sender, EventArgs e)
        {
            if (Page.IsPostBack && Page.IsValid)
            {
                try
                {
                    var isSync = false;
                    var errorMessage = string.Empty;

                    var accountInfo = WeiXinManager.GetAccountInfo(PublishmentSystemID);

                    if (WeiXinManager.IsBinding(accountInfo))
                    {
                        var resultFull = new GetMenuResultFull();
                        resultFull.menu = new MenuFull_ButtonGroup();
                        resultFull.menu.button = new List<MenuFull_RootButton>();

                        var publishmentSystemUrl = PageUtils.AddProtocolToUrl(PageUtility.GetPublishmentSystemUrl(PublishmentSystemInfo, string.Empty));

                        var menuInfoList = DataProviderWX.MenuDAO.GetMenuInfoList(PublishmentSystemID, 0);
                        foreach (var menuInfo in menuInfoList)
                        {
                            var rootButton = new MenuFull_RootButton();
                            rootButton.name = menuInfo.MenuName;

                            rootButton.sub_button = new List<MenuFull_RootButton>();
                            var subMenuInfoList = DataProviderWX.MenuDAO.GetMenuInfoList(PublishmentSystemID, menuInfo.MenuID);

                            if (subMenuInfoList.Count > 0)
                            {
                                foreach (var subMenuInfo in subMenuInfoList)
                                {
                                    var subButton = new MenuFull_RootButton();

                                    var isExists = false;

                                    subButton.name = subMenuInfo.MenuName;
                                    if (subMenuInfo.MenuType == EMenuType.Site)
                                    {
                                        var pageUrl = string.Empty;
                                        if (subMenuInfo.ContentID > 0)
                                        {
                                            var tableStyle = NodeManager.GetTableStyle(PublishmentSystemInfo, subMenuInfo.ChannelID);
                                            var tableName = NodeManager.GetTableName(PublishmentSystemInfo, subMenuInfo.ChannelID);

                                            var contentInfo = DataProvider.ContentDAO.GetContentInfoNotTrash(tableStyle, tableName, subMenuInfo.ContentID);
                                            if (contentInfo != null)
                                            {
                                                pageUrl = PageUtility.GetContentUrl(PublishmentSystemInfo, contentInfo, true);
                                            }
                                        }
                                        else
                                        {
                                            var nodeInfo = NodeManager.GetNodeInfo(PublishmentSystemID, subMenuInfo.ChannelID);
                                            if (nodeInfo != null)
                                            {
                                                pageUrl = PageUtility.GetChannelUrl(PublishmentSystemInfo, nodeInfo);
                                            }
                                        }

                                        if (!string.IsNullOrEmpty(pageUrl))
                                        {
                                            isExists = true;
                                            subButton.type = "view";
                                            subButton.url = PageUtils.AddProtocolToUrl(pageUrl);
                                        }
                                    }
                                    else if (subMenuInfo.MenuType == EMenuType.Keyword)
                                    {
                                        if (KeywordManager.IsExists(PublishmentSystemID, subMenuInfo.Keyword))
                                        {
                                            isExists = true;
                                            subButton.type = "click";
                                            subButton.key = subMenuInfo.Keyword;
                                        }
                                    }
                                    else if (subMenuInfo.MenuType == EMenuType.Url)
                                    {
                                        if (!string.IsNullOrEmpty(subMenuInfo.Url))
                                        {
                                            isExists = true;
                                            subButton.type = "view";
                                            subButton.url = subMenuInfo.Url;
                                        }
                                    }

                                    if (!isExists)
                                    {
                                        subButton.type = "view";
                                        subButton.url = publishmentSystemUrl;
                                    }

                                    rootButton.sub_button.Add(subButton);
                                }
                            }
                            else
                            {
                                var isExists = false;

                                if (menuInfo.MenuType == EMenuType.Site)
                                {
                                    var pageUrl = string.Empty;
                                    if (menuInfo.ContentID > 0)
                                    {
                                        var tableStyle = NodeManager.GetTableStyle(PublishmentSystemInfo, menuInfo.ChannelID);
                                        var tableName = NodeManager.GetTableName(PublishmentSystemInfo, menuInfo.ChannelID);

                                        var contentInfo = DataProvider.ContentDAO.GetContentInfoNotTrash(tableStyle, tableName, menuInfo.ContentID);
                                        if (contentInfo != null)
                                        {
                                            pageUrl = PageUtility.GetContentUrl(PublishmentSystemInfo, contentInfo, true);
                                        }
                                    }
                                    else
                                    {
                                        var nodeInfo = NodeManager.GetNodeInfo(PublishmentSystemID, menuInfo.ChannelID);
                                        if (nodeInfo != null)
                                        {
                                            pageUrl = PageUtility.GetChannelUrl(PublishmentSystemInfo, nodeInfo);
                                        }
                                    }

                                    if (!string.IsNullOrEmpty(pageUrl))
                                    {
                                        isExists = true;
                                        rootButton.type = "view";
                                        rootButton.url = PageUtils.AddProtocolToUrl(pageUrl);
                                    }
                                }
                                else if (menuInfo.MenuType == EMenuType.Keyword)
                                {
                                    if (KeywordManager.IsExists(PublishmentSystemID, menuInfo.Keyword))
                                    {
                                        isExists = true;
                                        rootButton.type = "click";
                                        rootButton.key = menuInfo.Keyword;
                                    }
                                }
                                else if (menuInfo.MenuType == EMenuType.Url)
                                {
                                    if (!string.IsNullOrEmpty(menuInfo.Url))
                                    {
                                        isExists = true;
                                        rootButton.type = "view";
                                        rootButton.url = menuInfo.Url;
                                    }
                                }

                                if (!isExists)
                                {
                                    rootButton.type = "view";
                                    rootButton.url = publishmentSystemUrl;
                                }
                            }

                            resultFull.menu.button.Add(rootButton);
                        }

                        isSync = SyncMenu(resultFull, accountInfo, out errorMessage);
                    }
                    else
                    {
                        errorMessage = "您的微信公众号未绑定，请先绑定之后同步菜单";
                    }

                    if (isSync)
                    {
                        SuccessMessage("菜单同步成功，取消关注公众账号后再次关注，可以立即看到创建后的效果");
                    }
                    else
                    {
                        FailMessage($"菜单同步失败：{errorMessage}");
                        var logInfo = new ErrorLogInfo(0, DateTime.Now, errorMessage, string.Empty, "微信同步菜单错误");
                        LogUtils.AddErrorLog(logInfo);
                    }
                }
                catch (Exception ex)
                {
                    FailMessage($"菜单同步失败：{ex.Message}");

                    var logInfo = new ErrorLogInfo(0, DateTime.Now, ex.Message, ex.StackTrace, "微信同步菜单错误");
                    LogUtils.AddErrorLog(logInfo);
                }
            }
        }

        private bool SyncMenu(GetMenuResultFull resultFull, AccountInfo accountInfo, out string errorMessage)
        {
            var isSync = false;
            errorMessage = string.Empty;

            var bg = CommonApi.GetMenuFromJsonResult(resultFull).menu;
            var accessToken = MPUtils.GetAccessToken(accountInfo);
            var result = CommonApi.CreateMenu(accessToken, bg);

            if (result.errmsg == "ok")
            {
                isSync = true;
            }
            else
            {
                isSync = false;
                errorMessage = result.errmsg;
            }

            return isSync;
        }

        public void Delete_OnClick(object sender, EventArgs e)
        {
            if (Page.IsPostBack && Page.IsValid)
            {
                try
                {
                    var accountInfo = WeiXinManager.GetAccountInfo(PublishmentSystemID);

                    var accessToken = MPUtils.GetAccessToken(accountInfo);

                    var result = CommonApi.DeleteMenu(accessToken);

                    if (result.errmsg == "ok")
                    {
                        SuccessMessage("菜单禁用成功，取消关注公众账号后再次关注，可以立即看到禁用后的效果");
                    }
                    else
                    {
                        FailMessage($"菜单禁用失败：{result.errmsg}");
                    }
                }
                catch (Exception ex)
                {
                    FailMessage($"菜单禁用失败：{ex.Message}");
                }
            }
        }
    }
}
