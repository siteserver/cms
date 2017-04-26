using System;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Model.Enumerations;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;

namespace SiteServer.BackgroundPages.Cms
{
    public class PageTagStyleMenuAdd : BasePageCms
    {
        public Literal ltlPageTitle;
        public TextBox MenuDisplayName;
        public DropDownList Vertical;
        public DropDownList FontFamily;
        public TextBox FontSize;
        public DropDownList FontWeight;
        public DropDownList FontStyle;
        public DropDownList MenuItemHAlign;
        public DropDownList MenuItemVAlign;
        public TextBox FontColor;
        public TextBox MenuItemBgColor;
        public TextBox FontColorHilite;
        public TextBox MenuHiliteBgColor;
        public TextBox XPosition;
        public TextBox YPosition;
        public DropDownList HideOnMouseOut;
        public TextBox MenuWidth;
        public TextBox MenuItemHeight;
        public TextBox MenuItemPadding;
        public TextBox MenuItemSpacing;
        public TextBox MenuItemIndent;
        public TextBox HideTimeout;
        public DropDownList MenuBgOpaque;
        public TextBox MenuBorder;
        public TextBox BGColor;
        public TextBox MenuBorderBgColor;
        public TextBox MenuLiteBgColor;
        public TextBox ChildMenuIcon;
        public TextBox Description;
        public RadioButtonList IsDefault;
        public HyperLink SelectImage;

        private int menuDisplayID;

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("PublishmentSystemID");

            menuDisplayID = Body.GetQueryInt("MenuDisplayID");

            if (!IsPostBack)
            {
                var pageTitle = (menuDisplayID == 0) ? "添加菜单样式" : "编辑菜单样式";
                BreadCrumb(AppManager.Cms.LeftMenu.IdTemplate, AppManager.Cms.LeftMenu.Template.IdTagStyle, pageTitle, AppManager.Cms.Permission.WebSite.Template);

                ltlPageTitle.Text = pageTitle;

                IsDefault.Items[0].Value = true.ToString();
                IsDefault.Items[1].Value = false.ToString();

                Vertical.Items.Add(new ListItem("垂直", "true"));
                Vertical.Items.Add(new ListItem("水平", "false"));

                FontFamily.Items.Add(new ListItem("<未设置>", ""));
                FontFamily.Items.Add(new ListItem("Arial, Helvetica, sans-serif", "Arial, Helvetica, sans-serif"));
                FontFamily.Items.Add(new ListItem("Times New Roman, Times, serif", "Times New Roman, Times, serif"));
                FontFamily.Items.Add(new ListItem("Courier New, Courier, mono", "Courier New, Courier, mono"));
                FontFamily.Items.Add(new ListItem("Georgia, Times New Roman, Times, serif", "Georgia, Times New Roman, Times, serif"));
                FontFamily.Items.Add(new ListItem("Verdana, Arial, Helvetica, sans-serif", "Verdana, Arial, Helvetica, sans-serif"));
                FontFamily.Items.Add(new ListItem("Geneva, Arial, Helvetica, sans-serif", "Geneva, Arial, Helvetica, sans-serif"));
                FontFamily.Items.Add(new ListItem("宋体", "宋体"));
                FontFamily.Items.Add(new ListItem("仿宋", "仿宋"));
                FontFamily.Items.Add(new ListItem("黑体", "黑体"));
                FontFamily.Items.Add(new ListItem("楷体", "楷体"));
                FontFamily.Items.Add(new ListItem("新宋体", "新宋体"));
                FontFamily.Items.Add(new ListItem("幼圆", "幼圆"));

                FontWeight.Items.Add(new ListItem("普通", "plain"));
                FontWeight.Items.Add(new ListItem("粗体", "bold"));

                FontStyle.Items.Add(new ListItem("普通", "normal"));
                FontStyle.Items.Add(new ListItem("斜体", "italic"));

                MenuItemHAlign.Items.Add(new ListItem("居中", "center"));
                MenuItemHAlign.Items.Add(new ListItem("对齐", "justify"));
                MenuItemHAlign.Items.Add(new ListItem("左", "left"));
                MenuItemHAlign.Items.Add(new ListItem("右", "right"));

                MenuItemVAlign.Items.Add(new ListItem("居中", "middle"));
                MenuItemVAlign.Items.Add(new ListItem("底端", "bottom"));
                MenuItemVAlign.Items.Add(new ListItem("顶端", "top"));

                HideOnMouseOut.Items.Add(new ListItem("隐藏", "true"));
                HideOnMouseOut.Items.Add(new ListItem("不隐藏", "false"));

                MenuBgOpaque.Items.Add(new ListItem("显示边框", "true"));
                MenuBgOpaque.Items.Add(new ListItem("不显示边框", "false"));
                var showPopWinString = ModalSelectImage.GetOpenWindowString(PublishmentSystemInfo, ChildMenuIcon.ClientID);
                SelectImage.Attributes.Add("onclick", showPopWinString);
                if (menuDisplayID > 0)
                {
                    var menuDisplayInfo = DataProvider.MenuDisplayDao.GetMenuDisplayInfo(menuDisplayID);
                    if (menuDisplayInfo != null)
                    {
                        MenuDisplayName.Text = menuDisplayInfo.MenuDisplayName;
                        MenuDisplayName.Enabled = false;
                        foreach (ListItem item in Vertical.Items)
                        {
                            item.Selected = ((menuDisplayInfo.Vertical.Equals(item.Value))) ? true : false;
                        }
                        foreach (ListItem item in FontFamily.Items)
                        {
                            item.Selected = ((menuDisplayInfo.FontFamily.Equals(item.Value))) ? true : false;
                        }
                        FontSize.Text = menuDisplayInfo.FontSize.ToString();
                        foreach (ListItem item in FontWeight.Items)
                        {
                            item.Selected = ((menuDisplayInfo.FontWeight.Equals(item.Value))) ? true : false;
                        }
                        foreach (ListItem item in FontStyle.Items)
                        {
                            item.Selected = ((menuDisplayInfo.FontStyle.Equals(item.Value))) ? true : false;
                        }
                        foreach (ListItem item in MenuItemHAlign.Items)
                        {
                            item.Selected = ((menuDisplayInfo.MenuItemHAlign.Equals(item.Value))) ? true : false;
                        }
                        foreach (ListItem item in MenuItemVAlign.Items)
                        {
                            item.Selected = ((menuDisplayInfo.MenuItemVAlign.Equals(item.Value))) ? true : false;
                        }
                        FontColor.Text = menuDisplayInfo.FontColor;
                        MenuItemBgColor.Text = menuDisplayInfo.MenuItemBgColor;
                        FontColorHilite.Text = menuDisplayInfo.FontColorHilite;
                        MenuHiliteBgColor.Text = menuDisplayInfo.MenuHiliteBgColor;
                        XPosition.Text = menuDisplayInfo.XPosition;
                        YPosition.Text = menuDisplayInfo.YPosition;
                        foreach (ListItem item in HideOnMouseOut.Items)
                        {
                            item.Selected = ((menuDisplayInfo.HideOnMouseOut.Equals(item.Value))) ? true : false;
                        }
                        MenuWidth.Text = menuDisplayInfo.MenuWidth.ToString();
                        MenuItemHeight.Text = menuDisplayInfo.MenuItemHeight.ToString();
                        MenuItemPadding.Text = menuDisplayInfo.MenuItemPadding.ToString();
                        MenuItemSpacing.Text = menuDisplayInfo.MenuItemSpacing.ToString();
                        MenuItemIndent.Text = menuDisplayInfo.MenuItemIndent.ToString();
                        HideTimeout.Text = menuDisplayInfo.HideTimeout.ToString();
                        foreach (ListItem item in MenuBgOpaque.Items)
                        {
                            item.Selected = ((menuDisplayInfo.MenuBgOpaque.Equals(item.Value))) ? true : false;
                        }
                        MenuBorder.Text = menuDisplayInfo.MenuBorder.ToString();
                        BGColor.Text = menuDisplayInfo.BgColor;
                        MenuBorderBgColor.Text = menuDisplayInfo.MenuBorderBgColor;
                        MenuLiteBgColor.Text = menuDisplayInfo.MenuLiteBgColor;
                        ChildMenuIcon.Text = menuDisplayInfo.ChildMenuIcon;

                        Description.Text = menuDisplayInfo.Description;
                        ControlUtils.SelectListItems(IsDefault, menuDisplayInfo.IsDefault.ToString());
                    }
                }
            }
        }

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            if (Page.IsPostBack && Page.IsValid)
            {
                if (menuDisplayID > 0)
                {
                    var menuDisplayInfo = DataProvider.MenuDisplayDao.GetMenuDisplayInfo(menuDisplayID);

                    menuDisplayInfo.Vertical = Vertical.SelectedValue;
                    menuDisplayInfo.FontFamily = FontFamily.SelectedValue;
                    menuDisplayInfo.FontSize = int.Parse(FontSize.Text);
                    menuDisplayInfo.FontWeight = FontWeight.SelectedValue;
                    menuDisplayInfo.FontStyle = FontStyle.SelectedValue;
                    menuDisplayInfo.MenuItemHAlign = MenuItemHAlign.SelectedValue;
                    menuDisplayInfo.MenuItemVAlign = MenuItemVAlign.SelectedValue;
                    menuDisplayInfo.FontColor = FontColor.Text;
                    menuDisplayInfo.MenuItemBgColor = MenuItemBgColor.Text;
                    menuDisplayInfo.FontColorHilite = FontColorHilite.Text;
                    menuDisplayInfo.MenuHiliteBgColor = MenuHiliteBgColor.Text;
                    menuDisplayInfo.XPosition = XPosition.Text;
                    menuDisplayInfo.YPosition = YPosition.Text;
                    menuDisplayInfo.HideOnMouseOut = HideOnMouseOut.SelectedValue;
                    menuDisplayInfo.MenuWidth = int.Parse(MenuWidth.Text);
                    menuDisplayInfo.MenuItemHeight = int.Parse(MenuItemHeight.Text);
                    menuDisplayInfo.MenuItemPadding = int.Parse(MenuItemPadding.Text);
                    menuDisplayInfo.MenuItemSpacing = int.Parse(MenuItemSpacing.Text);
                    menuDisplayInfo.MenuItemIndent = int.Parse(MenuItemIndent.Text);
                    menuDisplayInfo.HideTimeout = int.Parse(HideTimeout.Text);
                    menuDisplayInfo.MenuBgOpaque = MenuBgOpaque.SelectedValue;
                    menuDisplayInfo.MenuBorder = int.Parse(MenuBorder.Text);
                    menuDisplayInfo.BgColor = BGColor.Text;
                    menuDisplayInfo.MenuBorderBgColor = MenuBorderBgColor.Text;
                    menuDisplayInfo.MenuLiteBgColor = MenuLiteBgColor.Text;
                    menuDisplayInfo.ChildMenuIcon = ChildMenuIcon.Text;
                    menuDisplayInfo.Description = Description.Text;
                    menuDisplayInfo.IsDefault = TranslateUtils.ToBool(IsDefault.SelectedValue);
                    try
                    {
                        DataProvider.MenuDisplayDao.Update(menuDisplayInfo);
                        Body.AddSiteLog(PublishmentSystemId, "修改下拉菜单样式", $"下拉菜单样式:{menuDisplayInfo.MenuDisplayName}");
                        SuccessMessage("下拉菜单样式修改成功！");
                    }
                    catch (Exception ex)
                    {
                        FailMessage(ex, "下拉菜单样式修改失败！");
                    }
                }
                else
                {
                    var MenuDisplayNameList = DataProvider.MenuDisplayDao.GetMenuDisplayNameCollection(PublishmentSystemId);
                    if (MenuDisplayNameList.IndexOf(MenuDisplayName.Text) != -1)
                    {
                        FailMessage("下拉菜单样式添加失败，下拉菜单样式已存在！");
                    }
                    else
                    {
                        var menuDisplayInfo = new MenuDisplayInfo();
                        menuDisplayInfo.PublishmentSystemId = PublishmentSystemId;
                        menuDisplayInfo.MenuDisplayName = MenuDisplayName.Text;
                        menuDisplayInfo.Vertical = Vertical.SelectedValue;
                        menuDisplayInfo.FontFamily = FontFamily.SelectedValue;
                        menuDisplayInfo.FontSize = int.Parse(FontSize.Text);
                        menuDisplayInfo.FontWeight = FontWeight.SelectedValue;
                        menuDisplayInfo.FontStyle = FontStyle.SelectedValue;
                        menuDisplayInfo.MenuItemHAlign = MenuItemHAlign.SelectedValue;
                        menuDisplayInfo.MenuItemVAlign = MenuItemVAlign.SelectedValue;
                        menuDisplayInfo.FontColor = FontColor.Text;
                        menuDisplayInfo.MenuItemBgColor = MenuItemBgColor.Text;
                        menuDisplayInfo.FontColorHilite = FontColorHilite.Text;
                        menuDisplayInfo.MenuHiliteBgColor = MenuHiliteBgColor.Text;
                        menuDisplayInfo.XPosition = XPosition.Text;
                        menuDisplayInfo.YPosition = YPosition.Text;
                        menuDisplayInfo.HideOnMouseOut = HideOnMouseOut.SelectedValue;
                        menuDisplayInfo.MenuWidth = int.Parse(MenuWidth.Text);
                        menuDisplayInfo.MenuItemHeight = int.Parse(MenuItemHeight.Text);
                        menuDisplayInfo.MenuItemPadding = int.Parse(MenuItemPadding.Text);
                        menuDisplayInfo.MenuItemSpacing = int.Parse(MenuItemSpacing.Text);
                        menuDisplayInfo.MenuItemIndent = int.Parse(MenuItemIndent.Text);
                        menuDisplayInfo.HideTimeout = int.Parse(HideTimeout.Text);
                        menuDisplayInfo.MenuBgOpaque = MenuBgOpaque.SelectedValue;
                        menuDisplayInfo.MenuBorder = int.Parse(MenuBorder.Text);
                        menuDisplayInfo.BgColor = BGColor.Text;
                        menuDisplayInfo.MenuBorderBgColor = MenuBorderBgColor.Text;
                        menuDisplayInfo.MenuLiteBgColor = MenuLiteBgColor.Text;
                        menuDisplayInfo.ChildMenuIcon = ChildMenuIcon.Text;
                        menuDisplayInfo.Description = Description.Text;
                        menuDisplayInfo.IsDefault = TranslateUtils.ToBool(IsDefault.SelectedValue);
                        menuDisplayInfo.AddDate = DateTime.Now;
                        try
                        {
                            DataProvider.MenuDisplayDao.Insert(menuDisplayInfo);
                            Body.AddSiteLog(PublishmentSystemId, "添加下拉菜单样式", $"下拉菜单样式:{menuDisplayInfo.MenuDisplayName}");
                            SuccessMessage("下拉菜单样式添加成功！");
                        }
                        catch (Exception ex)
                        {
                            FailMessage(ex, "下拉菜单样式添加失败！");
                        }
                    }
                }

            }
        }

        public string GetPreviewImageSrc(string adType)
        {
           
            var imageUrl = ChildMenuIcon.Text;
         
            if (!string.IsNullOrEmpty(imageUrl))
            {
                var extension = PathUtils.GetExtension(imageUrl);
                if (EFileSystemTypeUtils.IsImage(extension))
                {
                    return PageUtility.ParseNavigationUrl(PublishmentSystemInfo, imageUrl);
                }
                else if (EFileSystemTypeUtils.IsFlash(extension))
                {
                    return SiteServerAssets.GetIconUrl("flash.jpg");
                }
                else if (EFileSystemTypeUtils.IsPlayer(extension))
                {
                    return SiteServerAssets.GetIconUrl("player.gif");
                }
            }
            return SiteServerAssets.GetIconUrl("empty.gif");
        }
    }
}
