using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;
using SiteServer.CMS.Core;
using SiteServer.CMS.WeiXin.Model.Enumerations;

namespace SiteServer.BackgroundPages.WeiXin
{
	public class PageWebMenuType : BasePageCms
    {
        //public TextBox tbWebMenuColor;
        //public RadioButtonList rblIsWebMenuLeft;

		public DataList DlContents;

        private EWebMenuType _webMenuType;

        public static string GetRedirectUrl(int publishmentSystemId)
        {
            return PageUtils.GetWeiXinUrl(nameof(PageWebMenuType), new NameValueCollection
            {
                {"publishmentSystemId", publishmentSystemId.ToString()}
            });
        }

		public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            _webMenuType = EWebMenuTypeUtils.GetEnumType(PublishmentSystemInfo.Additional.WxWebMenuType);

			if (!IsPostBack)
            {
                InfoMessage("当前底部导航菜单风格：" + EWebMenuTypeUtils.GetText(_webMenuType));

                //this.tbWebMenuColor.Text = base.PublishmentSystemInfo.Additional.WX_WebMenuColor;

                //EBooleanUtils.AddListItems(this.rblIsWebMenuLeft, "左对齐", "右对齐");
                //ControlUtils.SelectListItems(this.rblIsWebMenuLeft, base.PublishmentSystemInfo.Additional.WX_IsWebMenuLeft.ToString());

                DlContents.DataSource = EWebMenuTypeUtils.GetList();
                DlContents.ItemDataBound += dlContents_ItemDataBound;
                DlContents.DataBind();
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
                if (menuType == _webMenuType)
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
                    //base.PublishmentSystemInfo.Additional.WxWebMenuColor = this.tbWebMenuColor.Text;
                    //base.PublishmentSystemInfo.Additional.WxIsWebMenuLeft = TranslateUtils.ToBool(this.rblIsWebMenuLeft.SelectedValue);

                    var menuType = EWebMenuTypeUtils.GetEnumType(Request.Form["choose"]);
                    PublishmentSystemInfo.Additional.WxWebMenuType = EWebMenuTypeUtils.GetValue(menuType);

                    DataProvider.PublishmentSystemDao.Update(PublishmentSystemInfo);

                    SuccessMessage("底部导航菜单风格配置成功！");
                    AddWaitAndRedirectScript(GetRedirectUrl(PublishmentSystemId));
                }
                catch (Exception ex)
                {
                    SuccessMessage("底部导航菜单风格配置成功！");
                }
            }
        }
	}
}
