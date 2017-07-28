using System;
using System.Web.UI.WebControls;
using BaiRong.Core;
using SiteServer.CMS.Core;
using SiteServer.WeiXin.Model;
using SiteServer.WeiXin.Core;

namespace SiteServer.WeiXin.BackgroundPages
{
	public class BackgroundMenuAdd : BackgroundBasePageWX
	{
        public Literal ltlPageTitle;
        public TextBox tbMenuName;

        public PlaceHolder phMenuType;
        public DropDownList ddlMenuType;

        public PlaceHolder phKeyword;
        public TextBox tbKeyword;
        public Button btnKeywordSelect;

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
                $"background_menuAdd.aspx?publishmentSystemID={publishmentSystemID}&parentID={parentID}&menuID={menuID}");
        }

		public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("PublishmentSystemID");
            menuID = TranslateUtils.ToInt(GetQueryString("menuID"));
            parentID = TranslateUtils.ToInt(GetQueryString("parentID"));

			if (!IsPostBack)
			{
                EMenuTypeUtils.AddListItems(ddlMenuType);

                var menuInfo = DataProviderWX.MenuDAO.GetMenuInfo(menuID);
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
                    ControlUtils.SelectListItems(ddlMenuType, EMenuTypeUtils.GetValue(menuInfo.MenuType));
                    tbKeyword.Text = menuInfo.Keyword;
                    tbUrl.Text = menuInfo.Url;
                    ltlScript.Text =
                        $"<script>{MPUtils.GetChannelOrContentSelectScript(PublishmentSystemInfo, menuInfo.ChannelID, menuInfo.ContentID)}</script>";
                }

                ddlMenuType_OnSelectedIndexChanged(null, EventArgs.Empty);

                btnKeywordSelect.Attributes.Add("onclick", "parent." + Modal.KeywordSelect.GetOpenWindowString(PublishmentSystemID, "selectKeyword"));

                btnContentSelect.Attributes.Add("onclick", "parent." + Modal.ContentSelect.GetOpenWindowString(PublishmentSystemID, false, "contentSelect"));
                btnChannelSelect.Attributes.Add("onclick", "parent." + CMS.BackgroundPages.Modal.ChannelSelect.GetOpenWindowString(PublishmentSystemID));
			}
		}

        public void ddlMenuType_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            var isHideAll = false;
            if (parentID == 0 && menuID > 0)
            {
                var childrenCount = DataProviderWX.MenuDAO.GetCount(menuID);
                if (childrenCount > 0)
                {
                    isHideAll = true;
                }
            }

            if (isHideAll)
            {
                phMenuType.Visible = phUrl.Visible = phKeyword.Visible = phSite.Visible = false;
            }
            else
            {
                var menuType = EMenuTypeUtils.GetEnumType(ddlMenuType.SelectedValue);

                phUrl.Visible = phKeyword.Visible = phSite.Visible = false;

                if (menuType == EMenuType.Url)
                {
                    phUrl.Visible = true;
                }
                else if (menuType == EMenuType.Keyword)
                {
                    phKeyword.Visible = true;
                }
                else if (menuType == EMenuType.Site)
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
                    var menuInfo = new MenuInfo();
                    if (menuID > 0)
                    {
                        menuInfo = DataProviderWX.MenuDAO.GetMenuInfo(menuID);
                    }

                    menuInfo.MenuName = tbMenuName.Text;
                    menuInfo.ParentID = parentID;
                    menuInfo.PublishmentSystemID = PublishmentSystemID;
                    menuInfo.MenuType = EMenuTypeUtils.GetEnumType(ddlMenuType.SelectedValue);

                    if (phMenuType.Visible)
                    {
                        if (menuInfo.MenuType == EMenuType.Keyword)
                        {
                            if (!DataProviderWX.KeywordMatchDAO.IsExists(PublishmentSystemID, tbKeyword.Text))
                            {
                                FailMessage("菜单保存失败，所填关键词不存在，请先在关键词回复中添加");
                                return;
                            }
                        }
                        else if (menuInfo.MenuType == EMenuType.Site)
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
                    }

                    if (parentID > 0)
                    {
                        if (StringUtils.GetByteCount(tbMenuName.Text) > 26)
                        {
                            FailMessage("菜单保存失败，子菜单菜名称不能超过26个字节（13个汉字）");
                            return;
                        }
                    }
                    else
                    {
                        if (StringUtils.GetByteCount(tbMenuName.Text) > 16)
                        {
                            FailMessage("菜单保存失败，子菜单菜名称不能超过16个字节（8个汉字）");
                            return;
                        }
                    }
                    if (menuInfo.MenuType == EMenuType.Url)
                    {
                        if (StringUtils.GetByteCount(tbUrl.Text) > 256)
                        {
                            FailMessage("菜单保存失败，菜单网址不能超过256个字节");
                            return;
                        }
                    }

                    if (menuInfo.MenuType == EMenuType.Url)
                    {
                        menuInfo.Url = tbUrl.Text;
                    }
                    else if (menuInfo.MenuType == EMenuType.Keyword)
                    {
                        menuInfo.Keyword = tbKeyword.Text;
                    }
                    else if (menuInfo.MenuType == EMenuType.Site)
                    {
                        var idsCollection = Request.Form["idsCollection"];
                        if (!string.IsNullOrEmpty(idsCollection))
                        {
                            menuInfo.ChannelID = TranslateUtils.ToInt(idsCollection.Split('_')[0]);
                            menuInfo.ContentID = TranslateUtils.ToInt(idsCollection.Split('_')[1]);
                        }
                    }

                    if (menuID > 0)
                    {
                        DataProviderWX.MenuDAO.Update(menuInfo);

                        StringUtility.AddLog(PublishmentSystemID, "修改自定义菜单");
                        SuccessMessage("自定义菜单修改成功！");
                    }
                    else
                    {
                        menuID = DataProviderWX.MenuDAO.Insert(menuInfo);

                        StringUtility.AddLog(PublishmentSystemID, "新增自定义菜单");
                        SuccessMessage("自定义菜单新增成功！");
                    }

                    var redirectUrl = BackgroundMenu.GetRedirectUrl(PublishmentSystemID, parentID, menuID);
                    ltlPageTitle.Text += $@"<script>parent.redirect('{redirectUrl}');</script>";
				}
				catch(Exception ex)
				{
                    FailMessage(ex, "自定义菜单配置失败！");
				}
			}
		}
	}
}
