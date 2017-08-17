using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Model;
using SiteServer.CMS.Core;
using SiteServer.CMS.WeiXin.Data;
using SiteServer.CMS.WeiXin.Manager;
using SiteServer.CMS.WeiXin.Model;
using SiteServer.CMS.WeiXin.Model.Enumerations;
using SiteServer.CMS.WeiXin.MP;
using SiteServer.CMS.WeiXin.WeiXinMP.CommonAPIs;
using SiteServer.CMS.WeiXin.WeiXinMP.Entities.JsonResult;
using System.Collections.Specialized;

namespace SiteServer.BackgroundPages.WeiXin
{
    public class PageMenu : BasePageCms
    {
        public Literal LtlMenu;
        public Literal LtlIFrame;

        private int _parentId;
        private int _menuId;

        public static string GetRedirectUrl(int publishmentSystemId, int parentId, int menuId)
        {
            return PageUtils.GetWeiXinUrl(nameof(PageMenu), new NameValueCollection
            {
                {"publishmentSystemId", publishmentSystemId.ToString()},
                {"parentId", parentId.ToString()},
                {"menuId", menuId.ToString()}
            });
        }

        public static string GetDeleteRedirectUrl(int publishmentSystemId, int parentId, int menuId)
        {
            return PageUtils.GetWeiXinUrl(nameof(PageMenu), new NameValueCollection
            {
                {"publishmentSystemId", publishmentSystemId.ToString()},
                {"parentId", parentId.ToString()},
                {"menuId", menuId.ToString()},
                {"Delete", true.ToString()}
            });
        }

        public static string GetSubtractRedirectUrl(int publishmentSystemId, int parentId, int menuId)
        {
            return PageUtils.GetWeiXinUrl(nameof(PageMenu), new NameValueCollection
            {
                {"publishmentSystemId", publishmentSystemId.ToString()},
                {"parentId", parentId.ToString()},
                {"menuId", menuId.ToString()},
                {"Subtract", true.ToString()}
            });
        }


        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            _menuId = Body.GetQueryInt("menuID");
            _parentId = Body.GetQueryInt("parentID");

            if (Request.QueryString["Delete"] != null && _menuId > 0)
            {
                DataProviderWx.MenuDao.Delete(_menuId);
                SuccessMessage("菜单删除成功！");
            }
            if (Request.QueryString["Subtract"] != null && _menuId > 0)
            {
                DataProviderWx.MenuDao.UpdateTaxisToUp(_parentId, _menuId);
                SuccessMessage("菜单排序成功！");
            }
            if (!IsPostBack)
            {

                BreadCrumb(AppManager.WeiXin.LeftMenu.IdAccounts, string.Empty, AppManager.WeiXin.Permission.WebSite.Menu);
                var accountInfo = WeiXinManager.GetAccountInfo(PublishmentSystemId);
                if (EWxAccountTypeUtils.Equals(accountInfo.AccountType, EWxAccountType.Subscribe))
                {
                    PageUtils.RedirectToErrorPage(@"您的微信公众账号类型为订阅号（非认证），微信目前不支持订阅号自定义菜单。如果您的公众账号类型不是订阅号，请到账户信息中设置微信绑定账号。");
                    return;
                }

                LtlIFrame.Text = @"<iframe frameborder=""0"" id=""menu"" name=""menu"" width=""100%"" height=""500""></iframe>";

                var menuInfoList = DataProviderWx.MenuDao.GetMenuInfoList(PublishmentSystemId, 0);

                var builder = new StringBuilder();

                foreach (var menuInfo in menuInfoList)
                {
                    var subMenuInfoList = DataProviderWx.MenuDao.GetMenuInfoList(PublishmentSystemId, menuInfo.MenuId);

                    var subBuilder = new StringBuilder();

                    if (subMenuInfoList.Count < 5)
                    {
                        var addSubUrl = PageMenuAdd.GetRedirectUrl(PublishmentSystemId, menuInfo.MenuId, 0);
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
                        var editSubUrl = PageMenuAdd.GetRedirectUrl(PublishmentSystemId, subMenuInfo.ParentId, subMenuInfo.MenuId);
                        var deleteSubUrl = GetDeleteRedirectUrl(PublishmentSystemId, subMenuInfo.ParentId, subMenuInfo.MenuId);
                        var subtractSubUrl = GetSubtractRedirectUrl(PublishmentSystemId, subMenuInfo.ParentId, subMenuInfo.MenuId);


                        subBuilder.AppendFormat(@"                                                                   
                            <dd class=""{0}"">
                              <a href=""{1}"" target=""menu""><font>{2}</font></a>
                              <a href=""{3}"" onclick=""javascript:return confirm('此操作将删除子菜单“{2}”，确认吗？');"" class=""delete""><img src=""images/iphone-btn-delete.png""></a>
                              <a href=""{4}"" style='top:20px;' class=""delete""><img src=""images/iphone-btn-up.png""></a>
                            </dd>", ddClass, editSubUrl, subMenuInfo.MenuName, deleteSubUrl, subtractSubUrl);
                    }

                    var editUrl = PageMenuAdd.GetRedirectUrl(PublishmentSystemId, menuInfo.ParentId, menuInfo.MenuId);
                    var subMenuStyle = _parentId == menuInfo.MenuId ? string.Empty : "display:none";
                    var deleteUrl = GetDeleteRedirectUrl(PublishmentSystemId, menuInfo.ParentId, menuInfo.MenuId);
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
                    </li>", editUrl, menuInfo.MenuName, subMenuStyle, subBuilder, deleteUrl);
                }

                if (menuInfoList.Count < 3)
                {
                    var addUrl = PageMenuAdd.GetRedirectUrl(PublishmentSystemId, 0, 0);
                    builder.AppendFormat(@"
                    <li class=""secondMenu addMain"">
                        <a href=""{0}"" class=""mainMenu"" target=""menu""><font>新增菜单</font></a>
                    </li>", addUrl);
                }

                LtlMenu.Text = builder.ToString();
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

                    var accountInfo = WeiXinManager.GetAccountInfo(PublishmentSystemId);

                    if (WeiXinManager.IsBinding(accountInfo))
                    {
                        var resultFull = new GetMenuResultFull();
                        resultFull.menu = new MenuFull_ButtonGroup();
                        resultFull.menu.button = new List<MenuFull_RootButton>();

                        var publishmentSystemUrl = PageUtils.AddProtocolToUrl(PageUtility.GetPublishmentSystemUrl(PublishmentSystemInfo, string.Empty));

                        var menuInfoList = DataProviderWx.MenuDao.GetMenuInfoList(PublishmentSystemId, 0);
                        foreach (var menuInfo in menuInfoList)
                        {
                            var rootButton = new MenuFull_RootButton();
                            rootButton.name = menuInfo.MenuName;

                            rootButton.sub_button = new List<MenuFull_RootButton>();
                            var subMenuInfoList = DataProviderWx.MenuDao.GetMenuInfoList(PublishmentSystemId, menuInfo.MenuId);

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
                                        if (subMenuInfo.ContentId > 0)
                                        {
                                            var tableStyle = NodeManager.GetTableStyle(PublishmentSystemInfo, subMenuInfo.ChannelId);
                                            var tableName = NodeManager.GetTableName(PublishmentSystemInfo, subMenuInfo.ChannelId);

                                            var contentInfo = DataProvider.ContentDao.GetContentInfoNotTrash(tableStyle, tableName, subMenuInfo.ContentId);
                                            if (contentInfo != null)
                                            {
                                                pageUrl = PageUtility.GetContentUrl(PublishmentSystemInfo, contentInfo, true);
                                            }
                                        }
                                        else
                                        {
                                            var nodeInfo = NodeManager.GetNodeInfo(PublishmentSystemId, subMenuInfo.ChannelId);
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
                                        if (KeywordManager.IsExists(PublishmentSystemId, subMenuInfo.Keyword))
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
                                    if (menuInfo.ContentId > 0)
                                    {
                                        var tableStyle = NodeManager.GetTableStyle(PublishmentSystemInfo, menuInfo.ChannelId);
                                        var tableName = NodeManager.GetTableName(PublishmentSystemInfo, menuInfo.ChannelId);

                                        var contentInfo = DataProvider.ContentDao.GetContentInfoNotTrash(tableStyle, tableName, menuInfo.ContentId);
                                        if (contentInfo != null)
                                        {
                                            pageUrl = PageUtility.GetContentUrl(PublishmentSystemInfo, contentInfo, true);
                                        }
                                    }
                                    else
                                    {
                                        var nodeInfo = NodeManager.GetNodeInfo(PublishmentSystemId, menuInfo.ChannelId);
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
                                    if (KeywordManager.IsExists(PublishmentSystemId, menuInfo.Keyword))
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
                    var accountInfo = WeiXinManager.GetAccountInfo(PublishmentSystemId);

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
