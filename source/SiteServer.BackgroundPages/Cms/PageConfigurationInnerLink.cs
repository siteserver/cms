using System;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Model.Enumerations;
using SiteServer.CMS.Core;

namespace SiteServer.BackgroundPages.Cms
{
	public class PageConfigurationInnerLink : BasePageCms
    {
		public RadioButtonList IsInnerLink;

        public PlaceHolder phInnerLink;

        public DropDownList IsInnerLinkByChannelName;
        public TextBox InnerLinkFormatString;
        public TextBox InnerLinkMaxNum;

		public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("PublishmentSystemID");

			if (!IsPostBack)
			{
                BreadCrumb(AppManager.Cms.LeftMenu.IdFunction, AppManager.Cms.LeftMenu.Function.IdInnerLink, "站内链接设置", AppManager.Cms.Permission.WebSite.InnerLink);

                EBooleanUtils.AddListItems(IsInnerLink, "启用", "禁用");
                ControlUtils.SelectListItemsIgnoreCase(IsInnerLink, PublishmentSystemInfo.Additional.IsInnerLink.ToString());

                EBooleanUtils.AddListItems(IsInnerLinkByChannelName);
                ControlUtils.SelectListItemsIgnoreCase(IsInnerLinkByChannelName, PublishmentSystemInfo.Additional.IsInnerLinkByChannelName.ToString());

                InnerLinkFormatString.Text = PublishmentSystemInfo.Additional.InnerLinkFormatString;

                InnerLinkMaxNum.Text = PublishmentSystemInfo.Additional.InnerLinkMaxNum.ToString();

				IsInnerLink_SelectedIndexChanged(null, null);
			}
		}

        public override void Submit_OnClick(object sender, EventArgs e)
		{
			if (Page.IsPostBack && Page.IsValid)
			{
                //if (!StringUtils.Contains(this.InnerLinkFormatString.Text, "{0}") || !StringUtils.Contains(this.InnerLinkFormatString.Text, "{1}"))
                //{
                //    base.FailMessage("站内链接显示代码必须包含{0}及{1}！");
                //    return;
                //}
                PublishmentSystemInfo.Additional.IsInnerLink = TranslateUtils.ToBool(IsInnerLink.SelectedValue);
                PublishmentSystemInfo.Additional.IsInnerLinkByChannelName = TranslateUtils.ToBool(IsInnerLinkByChannelName.SelectedValue);
                PublishmentSystemInfo.Additional.InnerLinkFormatString = InnerLinkFormatString.Text;
                PublishmentSystemInfo.Additional.InnerLinkMaxNum = TranslateUtils.ToInt(InnerLinkMaxNum.Text, PublishmentSystemInfo.Additional.InnerLinkMaxNum);
				
				try
				{
                    DataProvider.PublishmentSystemDao.Update(PublishmentSystemInfo);

                    Body.AddSiteLog(PublishmentSystemId, "修改站内链接设置");

					SuccessMessage("站内链接设置修改成功！");
				}
				catch(Exception ex)
				{
                    FailMessage(ex, "站内链接设置修改失败！");
				}
			}
		}

        public void IsInnerLink_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (EBooleanUtils.Equals(IsInnerLink.SelectedValue, EBoolean.True))
			{
                phInnerLink.Visible = true;
			}
			else
			{
                phInnerLink.Visible = false;
			}
		}
	}
}
