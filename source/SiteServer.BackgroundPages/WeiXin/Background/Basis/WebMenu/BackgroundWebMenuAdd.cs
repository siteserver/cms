using System;
using System.Web.UI.WebControls;
using BaiRong.Core;
using SiteServer.CMS.Core;
using SiteServer.CMS.BackgroundPages;
using SiteServer.WeiXin.Model;
using SiteServer.WeiXin.Core;

namespace SiteServer.WeiXin.BackgroundPages
{
	public class BackgroundWebMenuAdd : BackgroundBasePage
	{
        public Literal ltlPageTitle;
        public TextBox tbMenuName;

        public PlaceHolder phNavigationType;
        public DropDownList ddlNavigationType;

        public PlaceHolder phFunction;
        public Button btnFunctionSelect;

        public PlaceHolder phUrl;
        public TextBox tbUrl;

        public PlaceHolder phSite;
        public Button btnContentSelect;
        public Button btnChannelSelect;

        public Literal ltlScript;

        private int parentID;
        private int menuID;

        public static string GetRedirectUrl(int publishmentSystemID, int parentID, int menuID)
        {
            return PageUtils.GetWXUrl(
                $"background_webMenuAdd.aspx?publishmentSystemID={publishmentSystemID}&parentID={parentID}&menuID={menuID}");
        }

		public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("PublishmentSystemID");
            menuID = TranslateUtils.ToInt(GetQueryString("menuID"));
            parentID = TranslateUtils.ToInt(GetQueryString("parentID"));

			if (!IsPostBack)
			{
                ENavigationTypeUtils.AddListItems(ddlNavigationType);

                var menuInfo = DataProviderWX.WebMenuDAO.GetMenuInfo(menuID);
                if (menuInfo == null)
                {
                    menuID = 0;
                }

                if (menuID == 0)
                {
                    ltlPageTitle.Text = $"添加{(parentID == 0 ? "主" : "子")}菜单";
                }
                else
                {
                    ltlPageTitle.Text = $"修改{(parentID == 0 ? "主" : "子")}菜单（{menuInfo.MenuName}）";

                    tbMenuName.Text = menuInfo.MenuName;
                    ControlUtils.SelectListItems(ddlNavigationType, menuInfo.NavigationType);
                    tbUrl.Text = menuInfo.Url;
                    ltlScript.Text = $"<script>{GetFunctionOrChannelOrContentSelectScript(menuInfo)}</script>";
                }

                ddlNavigationType_OnSelectedIndexChanged(null, EventArgs.Empty);

                btnFunctionSelect.Attributes.Add("onclick", "parent." + Modal.FunctionSelect.GetOpenWindowString(PublishmentSystemID, "selectKeyword"));

                btnContentSelect.Attributes.Add("onclick", "parent." + Modal.ContentSelect.GetOpenWindowString(PublishmentSystemID, false, "contentSelect"));
                btnChannelSelect.Attributes.Add("onclick", "parent." + CMS.BackgroundPages.Modal.ChannelSelect.GetOpenWindowString(PublishmentSystemID));
			}
		}

        public string GetFunctionOrChannelOrContentSelectScript(WebMenuInfo menuInfo)
        {
            if (ENavigationTypeUtils.Equals(menuInfo.NavigationType, ENavigationType.Function))
            {
                if (menuInfo.FunctionID > 0)
                {
                    var functionName = KeywordManager.GetFunctionName(EKeywordTypeUtils.GetEnumType(menuInfo.KeywordType), menuInfo.FunctionID);
                    return $@"selectKeyword(""{menuInfo.KeywordType},{menuInfo.FunctionID},{functionName}"")";
                }
            }
            else if (ENavigationTypeUtils.Equals(menuInfo.NavigationType, ENavigationType.Site))
            {
                if (menuInfo.ContentID > 0)
                {
                    var tableStyle = NodeManager.GetTableStyle(PublishmentSystemInfo, menuInfo.ChannelID);
                    var tableName = NodeManager.GetTableName(PublishmentSystemInfo, menuInfo.ChannelID);

                    var contentInfo = DataProvider.ContentDAO.GetContentInfo(tableStyle, tableName, menuInfo.ContentID);

                    var pageUrl = PageUtilityWX.GetContentUrl(PublishmentSystemInfo, contentInfo);

                    return
                        $@"contentSelect(""{contentInfo.Title}"", ""{menuInfo.ChannelID}"", ""{menuInfo.ContentID}"", ""{pageUrl}"")";
                }
                else if (menuInfo.ChannelID > 0)
                {
                    var nodeNames = NodeManager.GetNodeNameNavigation(PublishmentSystemID, menuInfo.ChannelID);
                    var pageUrl = PageUtilityWX.GetChannelUrl(PublishmentSystemInfo, NodeManager.GetNodeInfo(PublishmentSystemID, menuInfo.ChannelID));
                    return $"selectChannel('{nodeNames}', '{menuInfo.ChannelID}', '{pageUrl}');";
                }
            }
            
            return string.Empty;
        }

        public void ddlNavigationType_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            var isHideAll = false;
            if (parentID == 0 && menuID > 0)
            {
                var childrenCount = DataProviderWX.WebMenuDAO.GetCount(PublishmentSystemID, menuID);
                if (childrenCount > 0)
                {
                    isHideAll = true;
                }
            }

            if (isHideAll)
            {
                phNavigationType.Visible = phUrl.Visible = phFunction.Visible = phSite.Visible = false;
            }
            else
            {
                var navigationType = ENavigationTypeUtils.GetEnumType(ddlNavigationType.SelectedValue);

                phUrl.Visible = phFunction.Visible = phSite.Visible = false;

                if (navigationType == ENavigationType.Url)
                {
                    phUrl.Visible = true;
                }
                else if (navigationType == ENavigationType.Function)
                {
                    phFunction.Visible = true;
                }
                else if (navigationType == ENavigationType.Site)
                {
                    phSite.Visible = true;
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
                    if (menuID > 0)
                    {
                        menuInfo = DataProviderWX.WebMenuDAO.GetMenuInfo(menuID);
                    }

                    menuInfo.MenuName = tbMenuName.Text;
                    menuInfo.ParentID = parentID;
                    menuInfo.PublishmentSystemID = PublishmentSystemID;
                    menuInfo.NavigationType = ddlNavigationType.SelectedValue;

                    if (ENavigationTypeUtils.Equals(menuInfo.NavigationType, ENavigationType.Url))
                    {
                        menuInfo.Url = tbUrl.Text;
                    }
                    else if (ENavigationTypeUtils.Equals(menuInfo.NavigationType, ENavigationType.Function))
                    {
                        var keywordType = Request.Form["keywordType"];
                        var functionID = TranslateUtils.ToInt(Request.Form["functionID"]);
                        if (!string.IsNullOrEmpty(keywordType) && functionID > 0)
                        {
                            menuInfo.KeywordType = keywordType;
                            menuInfo.FunctionID = functionID;
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
                            menuInfo.ChannelID = TranslateUtils.ToInt(idsCollection.Split('_')[0]);
                            menuInfo.ContentID = TranslateUtils.ToInt(idsCollection.Split('_')[1]);
                        }

                        if (menuInfo.ChannelID == 0 && menuInfo.ContentID == 0)
                        {
                            FailMessage("菜单保存失败，必须选择微网站页面，请点击下方按钮进行选择");
                            return;
                        }
                    }

                    if (menuID > 0)
                    {
                        DataProviderWX.WebMenuDAO.Update(menuInfo);
                        SuccessMessage("底部导航菜单修改成功！");
                    }
                    else
                    {
                        menuID = DataProviderWX.WebMenuDAO.Insert(menuInfo);
                        SuccessMessage("底部导航菜单新增成功！");
                    }

                    var redirectUrl = BackgroundWebMenu.GetRedirectUrl(PublishmentSystemID, parentID, menuID);
                    ltlPageTitle.Text += $@"<script>parent.redirect('{redirectUrl}');</script>";
				}
				catch(Exception ex)
				{
                    FailMessage(ex, "底部导航菜单配置失败！");
				}
			}
		}
	}
}
