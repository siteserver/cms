using System;
using System.Collections.Specialized;
using System.Text;
using System.Web.UI.WebControls;
using BaiRong.Core;
using SiteServer.CMS.Core;
using SiteServer.CMS.WeiXin.Data;

namespace SiteServer.BackgroundPages.WeiXin
{
    public class PageWebMenu : BasePageCms
    {
        public Literal LtlMenu;
        public Literal LtlIFrame;
        public Button BtnStatus;

        private int _parentId;
        private int _menuId;

        public static string GetRedirectUrl(int publishmentSystemId, int parentId, int menuId)
        {
            return PageUtils.GetWeiXinUrl(nameof(PageWebMenu), new NameValueCollection
            {
                {"publishmentSystemId", publishmentSystemId.ToString()},
                {"parentId", parentId.ToString()},
                {"menuId", menuId.ToString()}
            });
        }

        public static string GetDeleteRedirectUrl(int publishmentSystemId, int parentId, int menuId)
        {
            return PageUtils.GetWeiXinUrl(nameof(PageWebMenu), new NameValueCollection
            {
                {"publishmentSystemId", publishmentSystemId.ToString()},
                {"parentId", parentId.ToString()},
                {"menuId", menuId.ToString()},
                {"Delete", true.ToString()}
            });
        }

        public static string GetSubtractRedirectUrl(int publishmentSystemId, int parentId, int menuId)
        {
            return PageUtils.GetWeiXinUrl(nameof(PageWebMenu), new NameValueCollection
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
                DataProviderWx.WebMenuDao.Delete(_menuId);
                SuccessMessage("菜单删除成功！");
            }
            if (Request.QueryString["Subtract"] != null && _menuId > 0)
            {
                DataProviderWx.WebMenuDao.UpdateTaxisToUp(PublishmentSystemId, _parentId, _menuId);
                SuccessMessage("菜单排序成功！");
            }
            if (!IsPostBack)
            {
                //base.BreadCrumbConsole(AppManager.CMS.LeftMenu.Id_Site, "底部导航菜单", AppManager.Permission.Platform_Site);

                if (PublishmentSystemInfo.Additional.WxIsWebMenu)
                {
                    BtnStatus.Text = "禁用底部导航菜单";
                    BtnStatus.CssClass = "btn btn-danger";
                }
                else
                {
                    BtnStatus.Text = "启用底部导航菜单";
                    BtnStatus.CssClass = "btn btn-success";
                }

                LtlIFrame.Text = @"<iframe frameborder=""0"" id=""menu"" name=""menu"" width=""100%"" height=""500""></iframe>";

                var menuInfoList = DataProviderWx.WebMenuDao.GetMenuInfoList(PublishmentSystemId, 0);

                var builder = new StringBuilder();

                foreach (var menuInfo in menuInfoList)
                {
                    var subMenuInfoList = DataProviderWx.WebMenuDao.GetMenuInfoList(PublishmentSystemId, menuInfo.Id);

                    var subBuilder = new StringBuilder();

                    var addSubUrl = PageWebMenuAdd.GetRedirectUrl(PublishmentSystemId, menuInfo.Id, 0);
                    subBuilder.AppendFormat(@"
                            <dd class=""add"">
                              <a href=""{0}"" target=""menu""><font>新增菜单</font></a>
                            </dd>", addSubUrl);

                    var i = 0;
                    foreach (var subMenuInfo in subMenuInfoList)
                    {
                        i++;

                        var ddClass = i == subMenuInfoList.Count ? "last" : string.Empty;
                        var editSubUrl = PageWebMenuAdd.GetRedirectUrl(PublishmentSystemId, subMenuInfo.ParentId, subMenuInfo.Id);
                        var deleteSubUrl = GetDeleteRedirectUrl(PublishmentSystemId, subMenuInfo.ParentId, subMenuInfo.Id);
                        var subtractSubUrl = GetSubtractRedirectUrl(PublishmentSystemId, subMenuInfo.ParentId, subMenuInfo.Id);

                        subBuilder.AppendFormat(@"                                                                   
                            <dd class=""{0}"">
                              <a href=""{1}"" target=""menu""><font>{2}</font></a>
                              <a href=""{3}"" onclick=""javascript:return confirm('此操作将删除子菜单“{2}”，确认吗？');"" class=""delete""><img src=""images/iphone-btn-delete.png""></a>
                              <a href=""{4}"" style='top:20px;' class=""delete""><img src=""images/iphone-btn-up.png""></a>                           
</dd>", ddClass, editSubUrl, subMenuInfo.MenuName, deleteSubUrl, subtractSubUrl);
                    }

                    var editUrl = PageWebMenuAdd.GetRedirectUrl(PublishmentSystemId, menuInfo.ParentId, menuInfo.Id);
                    var subMenuStyle = _parentId == menuInfo.Id ? string.Empty : "display:none";
                    var deleteUrl = GetDeleteRedirectUrl(PublishmentSystemId, menuInfo.ParentId, menuInfo.Id);
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
                    var addUrl = PageWebMenuAdd.GetRedirectUrl(PublishmentSystemId, 0, 0);
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
                    DataProviderWx.WebMenuDao.Sync(PublishmentSystemId);

                    SuccessMessage("成功复制微信菜单到底部导航菜单！");
                    AddWaitAndRedirectScript(GetRedirectUrl(PublishmentSystemId, _parentId, _menuId));
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
                    PublishmentSystemInfo.Additional.WxIsWebMenu = !PublishmentSystemInfo.Additional.WxIsWebMenu;
                    DataProvider.PublishmentSystemDao.Update(PublishmentSystemInfo);

                    if (PublishmentSystemInfo.Additional.WxIsWebMenu)
                    {
                        SuccessMessage("底部导航菜单启用成功");
                    }
                    else
                    {
                        SuccessMessage("底部导航菜单禁用成功");
                    }

                    AddWaitAndRedirectScript(GetRedirectUrl(PublishmentSystemId, _parentId, _menuId));
                }
                catch (Exception ex)
                {
                    FailMessage($"失败：{ex.Message}");
                }
            }
        }
    }
}
