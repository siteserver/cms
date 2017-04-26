using System.Web.UI.WebControls;
using BaiRong.Core;
using System;
using System.Text;
using SiteServer.CMS.BackgroundPages;
using SiteServer.WeiXin.Core;
using SiteServer.CMS.Core;

namespace SiteServer.WeiXin.BackgroundPages
{
    public class BackgroundWebMenu : BackgroundBasePage
    {
        public Literal ltlMenu;
        public Literal ltlIFrame;
        public Button btnStatus;

        private int parentID;
        private int menuID;

        public static string GetRedirectUrl(int publishmentSystemID, int parentID, int menuID)
        {
            return PageUtils.GetWXUrl(
                $"background_webMenu.aspx?publishmentSystemID={publishmentSystemID}&parentID={parentID}&menuID={menuID}");
        }

        public static string GetDeleteRedirectUrl(int publishmentSystemID, int parentID, int menuID)
        {
            return PageUtils.GetWXUrl(
                $"background_webMenu.aspx?publishmentSystemID={publishmentSystemID}&parentID={parentID}&menuID={menuID}&Delete=True");
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
                DataProviderWX.WebMenuDAO.Delete(menuID);
                SuccessMessage("菜单删除成功！");
            }
            if (Request.QueryString["Subtract"] != null && menuID > 0)
            {
                DataProviderWX.WebMenuDAO.UpdateTaxisToUp(PublishmentSystemID, parentID, menuID);
                SuccessMessage("菜单排序成功！");
            }
            if (!IsPostBack)
            {
                //base.BreadCrumbConsole(AppManager.CMS.LeftMenu.ID_Site, "底部导航菜单", AppManager.Permission.Platform_Site);

                if (PublishmentSystemInfo.Additional.WX_IsWebMenu)
                {
                    btnStatus.Text = "禁用底部导航菜单";
                    btnStatus.CssClass = "btn btn-danger";
                }
                else
                {
                    btnStatus.Text = "启用底部导航菜单";
                    btnStatus.CssClass = "btn btn-success";
                }

                ltlIFrame.Text = @"<iframe frameborder=""0"" id=""menu"" name=""menu"" width=""100%"" height=""500""></iframe>";

                var menuInfoList = DataProviderWX.WebMenuDAO.GetMenuInfoList(PublishmentSystemID, 0);

                var builder = new StringBuilder();

                foreach (var menuInfo in menuInfoList)
                {
                    var subMenuInfoList = DataProviderWX.WebMenuDAO.GetMenuInfoList(PublishmentSystemID, menuInfo.ID);

                    var subBuilder = new StringBuilder();

                    var addSubUrl = BackgroundWebMenuAdd.GetRedirectUrl(PublishmentSystemID, menuInfo.ID, 0);
                    subBuilder.AppendFormat(@"
                            <dd class=""add"">
                              <a href=""{0}"" target=""menu""><font>新增菜单</font></a>
                            </dd>", addSubUrl);

                    var i = 0;
                    foreach (var subMenuInfo in subMenuInfoList)
                    {
                        i++;

                        var ddClass = i == subMenuInfoList.Count ? "last" : string.Empty;
                        var editSubUrl = BackgroundWebMenuAdd.GetRedirectUrl(PublishmentSystemID, subMenuInfo.ParentID, subMenuInfo.ID);
                        var deleteSubUrl = GetDeleteRedirectUrl(PublishmentSystemID, subMenuInfo.ParentID, subMenuInfo.ID);
                        var subtractSubUrl = GetSubtractRedirectUrl(PublishmentSystemID, subMenuInfo.ParentID, subMenuInfo.ID);

                        subBuilder.AppendFormat(@"                                                                   
                            <dd class=""{0}"">
                              <a href=""{1}"" target=""menu""><font>{2}</font></a>
                              <a href=""{3}"" onclick=""javascript:return confirm('此操作将删除子菜单“{2}”，确认吗？');"" class=""delete""><img src=""images/iphone-btn-delete.png""></a>
                              <a href=""{4}"" style='top:20px;' class=""delete""><img src=""images/iphone-btn-up.png""></a>                           
</dd>", ddClass, editSubUrl, subMenuInfo.MenuName, deleteSubUrl, subtractSubUrl);
                    }

                    var editUrl = BackgroundWebMenuAdd.GetRedirectUrl(PublishmentSystemID, menuInfo.ParentID, menuInfo.ID);
                    var subMenuStyle = parentID == menuInfo.ID ? string.Empty : "display:none";
                    var deleteUrl = GetDeleteRedirectUrl(PublishmentSystemID, menuInfo.ParentID, menuInfo.ID);
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
                    var addUrl = BackgroundWebMenuAdd.GetRedirectUrl(PublishmentSystemID, 0, 0);
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
                    DataProviderWX.WebMenuDAO.Sync(PublishmentSystemID);

                    SuccessMessage("成功复制微信菜单到底部导航菜单！");
                    AddWaitAndRedirectScript(GetRedirectUrl(PublishmentSystemID, parentID, menuID));
                }
                catch (Exception ex)
                {
                    FailMessage($"菜单同步失败：{ex.Message}");
                }
            }
        }

        public void Status_OnClick(object sender, EventArgs e)
        {
            if (Page.IsPostBack && Page.IsValid)
            {
                try
                {
                    PublishmentSystemInfo.Additional.WX_IsWebMenu = !PublishmentSystemInfo.Additional.WX_IsWebMenu;
                    DataProvider.PublishmentSystemDAO.Update(PublishmentSystemInfo);

                    if (PublishmentSystemInfo.Additional.WX_IsWebMenu)
                    {
                        SuccessMessage("底部导航菜单启用成功");
                    }
                    else
                    {
                        SuccessMessage("底部导航菜单禁用成功");
                    }

                    AddWaitAndRedirectScript(GetRedirectUrl(PublishmentSystemID, parentID, menuID));
                }
                catch (Exception ex)
                {
                    FailMessage($"失败：{ex.Message}");
                }
            }
        }
    }
}
