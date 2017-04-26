using System;
using System.Web.UI.WebControls;
using BaiRong.Core;
using SiteServer.CMS.Core;
using SiteServer.CMS.BackgroundPages;
using SiteServer.WeiXin.Model;

namespace SiteServer.WeiXin.BackgroundPages
{
	public class BackgroundWebMenuType : BackgroundBasePage
	{
        //public TextBox tbWebMenuColor;
        //public RadioButtonList rblIsWebMenuLeft;

		public DataList dlContents;

        private EWebMenuType webMenuType;

        public static string GetRedirectUrl(int publishmentSystemID)
        {
            return PageUtils.GetWXUrl($"background_webMenuType.aspx?PublishmentSystemID={publishmentSystemID}");
        }

		public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            webMenuType = EWebMenuTypeUtils.GetEnumType(PublishmentSystemInfo.Additional.WX_WebMenuType);

			if (!IsPostBack)
            {
                InfoMessage("当前底部导航菜单风格：" + EWebMenuTypeUtils.GetText(webMenuType));

                //this.tbWebMenuColor.Text = base.PublishmentSystemInfo.Additional.WX_WebMenuColor;

                //EBooleanUtils.AddListItems(this.rblIsWebMenuLeft, "左对齐", "右对齐");
                //ControlUtils.SelectListItems(this.rblIsWebMenuLeft, base.PublishmentSystemInfo.Additional.WX_IsWebMenuLeft.ToString());

                dlContents.DataSource = EWebMenuTypeUtils.GetList();
                dlContents.ItemDataBound += dlContents_ItemDataBound;
                dlContents.DataBind();
			}
		}

        void dlContents_ItemDataBound(object sender, DataListItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.Item)
            {
                var menuType = (EWebMenuType)e.Item.DataItem;

                var ltlImageUrl = e.Item.FindControl("ltlImageUrl") as Literal;
                var ltlDescription = e.Item.FindControl("ltlDescription") as Literal;
                var ltlRadio = e.Item.FindControl("ltlRadio") as Literal;

                var checkedStr = string.Empty;
                if (menuType == webMenuType)
                {
                    checkedStr = "checked";
                }

                ltlRadio.Text = $@"
<label class=""radio lead"">
  <input type=""radio"" name=""choose"" id=""choose{e.Item.ItemIndex + 1}"" value=""{EWebMenuTypeUtils.GetValue(menuType)}"" {checkedStr}>
  {EWebMenuTypeUtils.GetText(menuType)}
</label>";

                ltlImageUrl.Text =
                    $@"<img class=""cover"" src=""images/webMenu/{EWebMenuTypeUtils.GetValue(menuType)}.png"" class=""img-polaroid""><p></p>";
            }
        }

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            if (Page.IsPostBack && Page.IsValid)
            {
                try
                {
                    //base.PublishmentSystemInfo.Additional.WX_WebMenuColor = this.tbWebMenuColor.Text;
                    //base.PublishmentSystemInfo.Additional.WX_IsWebMenuLeft = TranslateUtils.ToBool(this.rblIsWebMenuLeft.SelectedValue);

                    var menuType = EWebMenuTypeUtils.GetEnumType(Request.Form["choose"]);
                    PublishmentSystemInfo.Additional.WX_WebMenuType = EWebMenuTypeUtils.GetValue(menuType);

                    DataProvider.PublishmentSystemDAO.Update(PublishmentSystemInfo);

                    SuccessMessage("底部导航菜单风格配置成功！");
                    AddWaitAndRedirectScript(GetRedirectUrl(PublishmentSystemID));
                }
                catch (Exception ex)
                {
                    FailMessage(ex, "底部导航菜单风格配置失败！");
                }
            }
        }
	}
}
