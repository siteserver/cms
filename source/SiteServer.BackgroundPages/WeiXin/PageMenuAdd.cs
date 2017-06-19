using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;
using SiteServer.BackgroundPages.Cms;
using SiteServer.CMS.WeiXin.Data;
using SiteServer.CMS.WeiXin.Model;
using SiteServer.CMS.WeiXin.Model.Enumerations;
using SiteServer.CMS.WeiXin.MP;

namespace SiteServer.BackgroundPages.WeiXin
{
	public class PageMenuAdd : BasePageCms
    {
        public Literal LtlPageTitle;
        public TextBox TbMenuName;

        public PlaceHolder PhMenuType;
        public DropDownList DdlMenuType;

        public PlaceHolder PhKeyword;
        public TextBox TbKeyword;
        public Button BtnKeywordSelect;

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
            return PageUtils.GetWeiXinUrl(nameof(PageMenuAdd), new NameValueCollection
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
                EMenuTypeUtils.AddListItems(DdlMenuType);

                var menuInfo = DataProviderWx.MenuDao.GetMenuInfo(_menuId);
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
                    ControlUtils.SelectListItems(DdlMenuType, EMenuTypeUtils.GetValue(menuInfo.MenuType));
                    TbKeyword.Text = menuInfo.Keyword;
                    TbUrl.Text = menuInfo.Url;
                    LtlScript.Text =
                        $"<script>{MPUtils.GetChannelOrContentSelectScript(PublishmentSystemInfo, menuInfo.ChannelId, menuInfo.ContentId)}</script>";
                }

                DdlMenuType_OnSelectedIndexChanged(null, EventArgs.Empty);

                BtnKeywordSelect.Attributes.Add("onclick", "parent." + ModalKeywordSelect.GetOpenWindowString(PublishmentSystemId, "selectKeyword"));

                BtnContentSelect.Attributes.Add("onclick", "parent." + ModalContentSelect.GetOpenWindowString(PublishmentSystemId, false, "contentSelect"));
                BtnChannelSelect.Attributes.Add("onclick", "parent." + ModalChannelSelect.GetOpenWindowString(PublishmentSystemId));
			}
		}

        public void DdlMenuType_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            var isHideAll = false;
            if (_parentId == 0 && _menuId > 0)
            {
                var childrenCount = DataProviderWx.MenuDao.GetCount(_menuId);
                if (childrenCount > 0)
                {
                    isHideAll = true;
                }
            }

            if (isHideAll)
            {
                PhMenuType.Visible = PhUrl.Visible = PhKeyword.Visible = PhSite.Visible = false;
            }
            else
            {
                var menuType = EMenuTypeUtils.GetEnumType(DdlMenuType.SelectedValue);

                PhUrl.Visible = PhKeyword.Visible = PhSite.Visible = false;

                if (menuType == EMenuType.Url)
                {
                    PhUrl.Visible = true;
                }
                else if (menuType == EMenuType.Keyword)
                {
                    PhKeyword.Visible = true;
                }
                else if (menuType == EMenuType.Site)
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
                    var menuInfo = new MenuInfo();
                    if (_menuId > 0)
                    {
                        menuInfo = DataProviderWx.MenuDao.GetMenuInfo(_menuId);
                    }

                    menuInfo.MenuName = TbMenuName.Text;
                    menuInfo.ParentId = _parentId;
                    menuInfo.PublishmentSystemId = PublishmentSystemId;
                    menuInfo.MenuType = EMenuTypeUtils.GetEnumType(DdlMenuType.SelectedValue);

                    if (PhMenuType.Visible)
                    {
                        if (menuInfo.MenuType == EMenuType.Keyword)
                        {
                            if (!DataProviderWx.KeywordMatchDao.IsExists(PublishmentSystemId, TbKeyword.Text))
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
                                menuInfo.ChannelId = TranslateUtils.ToInt(idsCollection.Split('_')[0]);
                                menuInfo.ContentId = TranslateUtils.ToInt(idsCollection.Split('_')[1]);
                            }

                            if (menuInfo.ChannelId == 0 && menuInfo.ContentId == 0)
                            {
                                FailMessage("菜单保存失败，必须选择微网站页面，请点击下方按钮进行选择");
                                return;
                            }
                        }
                    }

                    if (_parentId > 0)
                    {
                        if (StringUtils.GetByteCount(TbMenuName.Text) > 26)
                        {
                            FailMessage("菜单保存失败，子菜单菜名称不能超过26个字节（13个汉字）");
                            return;
                        }
                    }
                    else
                    {
                        if (StringUtils.GetByteCount(TbMenuName.Text) > 16)
                        {
                            FailMessage("菜单保存失败，子菜单菜名称不能超过16个字节（8个汉字）");
                            return;
                        }
                    }
                    if (menuInfo.MenuType == EMenuType.Url)
                    {
                        if (StringUtils.GetByteCount(TbUrl.Text) > 256)
                        {
                            FailMessage("菜单保存失败，菜单网址不能超过256个字节");
                            return;
                        }
                    }

                    if (menuInfo.MenuType == EMenuType.Url)
                    {
                        menuInfo.Url = TbUrl.Text;
                    }
                    else if (menuInfo.MenuType == EMenuType.Keyword)
                    {
                        menuInfo.Keyword = TbKeyword.Text;
                    }
                    else if (menuInfo.MenuType == EMenuType.Site)
                    {
                        var idsCollection = Request.Form["idsCollection"];
                        if (!string.IsNullOrEmpty(idsCollection))
                        {
                            menuInfo.ChannelId = TranslateUtils.ToInt(idsCollection.Split('_')[0]);
                            menuInfo.ContentId = TranslateUtils.ToInt(idsCollection.Split('_')[1]);
                        }
                    }

                    if (_menuId > 0)
                    {
                        DataProviderWx.MenuDao.Update(menuInfo);

                        Body.AddSiteLog(PublishmentSystemId, "修改自定义菜单");
                        SuccessMessage("自定义菜单修改成功！");
                    }
                    else
                    {
                        _menuId = DataProviderWx.MenuDao.Insert(menuInfo);

                        Body.AddSiteLog(PublishmentSystemId, "新增自定义菜单");
                        SuccessMessage("自定义菜单新增成功！");
                    }

                    var redirectUrl = PageMenu.GetRedirectUrl(PublishmentSystemId, _parentId, _menuId);
                    LtlPageTitle.Text += $@"<script>parent.redirect('{redirectUrl}');</script>";
				}
				catch(Exception ex)
				{
                    FailMessage(ex, "自定义菜单配置失败！");
                }
			}
		}
	}
}
