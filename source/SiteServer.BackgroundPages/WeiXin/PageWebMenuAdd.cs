using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;
using SiteServer.BackgroundPages.Cms;
using SiteServer.CMS.Core;
using SiteServer.CMS.WeiXin.Data;
using SiteServer.CMS.WeiXin.IO;
using SiteServer.CMS.WeiXin.Manager;
using SiteServer.CMS.WeiXin.Model;
using SiteServer.CMS.WeiXin.Model.Enumerations;

namespace SiteServer.BackgroundPages.WeiXin
{
	public class PageWebMenuAdd : BasePageCms
    {
        public Literal LtlPageTitle;
        public TextBox TbMenuName;

        public PlaceHolder PhNavigationType;
        public DropDownList DdlNavigationType;

        public PlaceHolder PhFunction;
        public Button BtnFunctionSelect;

        public PlaceHolder PhUrl;
        public TextBox TbUrl;

        public PlaceHolder PhSite;
        public Button BtnContentSelect;
        public Button BtnChannelSelect;

        public Literal LtlScript;

        private int _parentId;
        private int _menuId;

        public static string GetRedirectUrl(int publishmentSystemId, int parentId, int menuId)
        {
            return PageUtils.GetWeiXinUrl(nameof(PageWebMenuAdd), new NameValueCollection
            {
                {"publishmentSystemId", publishmentSystemId.ToString()},
                {"parentId", parentId.ToString()},
                {"menuId", menuId.ToString()}
            });
        }

		public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("PublishmentSystemId");
            _menuId = Body.GetQueryInt("menuID");
            _parentId = Body.GetQueryInt("parentID");

			if (!IsPostBack)
			{
                ENavigationTypeUtils.AddListItems(DdlNavigationType);

                var menuInfo = DataProviderWx.WebMenuDao.GetMenuInfo(_menuId);
                if (menuInfo == null)
                {
                    _menuId = 0;
                }

                if (_menuId == 0)
                {
                    LtlPageTitle.Text = $"添加{(_parentId == 0 ? "主" : "子")}菜单";
                }
                else
                {
                    LtlPageTitle.Text = $"修改{(_parentId == 0 ? "主" : "子")}菜单（{menuInfo.MenuName}）";

                    TbMenuName.Text = menuInfo.MenuName;
                    ControlUtils.SelectListItems(DdlNavigationType, menuInfo.NavigationType);
                    TbUrl.Text = menuInfo.Url;
                    LtlScript.Text = $"<script>{GetFunctionOrChannelOrContentSelectScript(menuInfo)}</script>";
                }

                ddlNavigationType_OnSelectedIndexChanged(null, EventArgs.Empty);

                BtnFunctionSelect.Attributes.Add("onclick", "parent." + ModalFunctionSelect.GetOpenWindowString(PublishmentSystemId, "selectKeyword"));

                BtnContentSelect.Attributes.Add("onclick", "parent." + ModalContentSelect.GetOpenWindowString(PublishmentSystemId, false, "contentSelect"));
                BtnChannelSelect.Attributes.Add("onclick", "parent." + ModalChannelSelect.GetOpenWindowString(PublishmentSystemId));
			}
		}

        public string GetFunctionOrChannelOrContentSelectScript(WebMenuInfo menuInfo)
        {
            if (ENavigationTypeUtils.Equals(menuInfo.NavigationType, ENavigationType.Function))
            {
                if (menuInfo.FunctionId > 0)
                {
                    var functionName = KeywordManager.GetFunctionName(EKeywordTypeUtils.GetEnumType(menuInfo.KeywordType), menuInfo.FunctionId);
                    return $@"selectKeyword(""{menuInfo.KeywordType},{menuInfo.FunctionId},{functionName}"")";
                }
            }
            else if (ENavigationTypeUtils.Equals(menuInfo.NavigationType, ENavigationType.Site))
            {
                if (menuInfo.ContentId > 0)
                {
                    var tableStyle = NodeManager.GetTableStyle(PublishmentSystemInfo, menuInfo.ChannelId);
                    var tableName = NodeManager.GetTableName(PublishmentSystemInfo, menuInfo.ChannelId);

                    var contentInfo = DataProvider.ContentDao.GetContentInfo(tableStyle, tableName, menuInfo.ContentId);

                    var pageUrl = PageUtilityWX.GetContentUrl(PublishmentSystemInfo, contentInfo);

                    return
                        $@"contentSelect(""{contentInfo.Title}"", ""{menuInfo.ChannelId}"", ""{menuInfo.ContentId}"", ""{pageUrl}"")";
                }
                else if (menuInfo.ChannelId > 0)
                {
                    var nodeNames = NodeManager.GetNodeNameNavigation(PublishmentSystemId, menuInfo.ChannelId);
                    var pageUrl = PageUtilityWX.GetChannelUrl(PublishmentSystemInfo, NodeManager.GetNodeInfo(PublishmentSystemId, menuInfo.ChannelId));
                    return $"selectChannel('{nodeNames}', '{menuInfo.ChannelId}', '{pageUrl}');";
                }
            }
            
            return string.Empty;
        }

        public void ddlNavigationType_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            var isHideAll = false;
            if (_parentId == 0 && _menuId > 0)
            {
                var childrenCount = DataProviderWx.WebMenuDao.GetCount(PublishmentSystemId, _menuId);
                if (childrenCount > 0)
                {
                    isHideAll = true;
                }
            }

            if (isHideAll)
            {
                PhNavigationType.Visible = PhUrl.Visible = PhFunction.Visible = PhSite.Visible = false;
            }
            else
            {
                var navigationType = ENavigationTypeUtils.GetEnumType(DdlNavigationType.SelectedValue);

                PhUrl.Visible = PhFunction.Visible = PhSite.Visible = false;

                if (navigationType == ENavigationType.Url)
                {
                    PhUrl.Visible = true;
                }
                else if (navigationType == ENavigationType.Function)
                {
                    PhFunction.Visible = true;
                }
                else if (navigationType == ENavigationType.Site)
                {
                    PhSite.Visible = true;
                }
            }
        }

        public override void Submit_OnClick(object sender, EventArgs e)
		{
			if (Page.IsPostBack && Page.IsValid)
			{
				try
				{
                    var menuInfo = new WebMenuInfo();
                    if (_menuId > 0)
                    {
                        menuInfo = DataProviderWx.WebMenuDao.GetMenuInfo(_menuId);
                    }

                    menuInfo.MenuName = TbMenuName.Text;
                    menuInfo.ParentId = _parentId;
                    menuInfo.PublishmentSystemId = PublishmentSystemId;
                    menuInfo.NavigationType = DdlNavigationType.SelectedValue;

                    if (ENavigationTypeUtils.Equals(menuInfo.NavigationType, ENavigationType.Url))
                    {
                        menuInfo.Url = TbUrl.Text;
                    }
                    else if (ENavigationTypeUtils.Equals(menuInfo.NavigationType, ENavigationType.Function))
                    {
                        var keywordType = Request.Form["keywordType"];
                        var functionId = TranslateUtils.ToInt(Request.Form["functionID"]);
                        if (!string.IsNullOrEmpty(keywordType) && functionId > 0)
                        {
                            menuInfo.KeywordType = keywordType;
                            menuInfo.FunctionId = functionId;
                        }
                        else
                        {
                            FailMessage("菜单保存失败，必须选择微功能页面，请点击下方按钮进行选择");
                            return;
                        }
                    }
                    else if (ENavigationTypeUtils.Equals(menuInfo.NavigationType, ENavigationType.Site))
                    {
                        var idsCollection = Request.Form["idsCollection"];
                        if (!string.IsNullOrEmpty(idsCollection))
                        {
                            menuInfo.ChannelId = TranslateUtils.ToInt(idsCollection.Split('_')[0]);
                            menuInfo.ContentId = TranslateUtils.ToInt(idsCollection.Split('_')[1]);
                        }

                        if (menuInfo.ChannelId == 0 && menuInfo.ContentId == 0)
                        {
                            FailMessage("菜单保存失败，必须选择微网站页面，请点击下方按钮进行选择");
                            return;
                        }
                    }

                    if (_menuId > 0)
                    {
                        DataProviderWx.WebMenuDao.Update(menuInfo);
                        SuccessMessage("底部导航菜单修改成功！");
                    }
                    else
                    {
                        _menuId = DataProviderWx.WebMenuDao.Insert(menuInfo);
                        SuccessMessage("底部导航菜单新增成功！");
                    }

                    var redirectUrl = PageWebMenu.GetRedirectUrl(PublishmentSystemId, _parentId, _menuId);
                    LtlPageTitle.Text += $@"<script>parent.redirect('{redirectUrl}');</script>";
				}
				catch(Exception ex)
				{
                    FailMessage(ex, "底部导航菜单配置失败！");
                }
			}
		}
	}
}
