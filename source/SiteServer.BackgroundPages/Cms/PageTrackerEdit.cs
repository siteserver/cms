using System;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Model.Enumerations;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model.Enumerations;

namespace SiteServer.BackgroundPages.Cms
{
	public class PageTrackerEdit : BasePageCms
    {
        public RadioButtonList IsTracker;

        public TextBox TrackerDays;
        public TextBox TrackerPageView;
        public TextBox TrackerUniqueVisitor;
        public TextBox TrackerCurrentMinute;
        public DropDownList TrackerStyle;

		public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("PublishmentSystemID");

            if (!IsPostBack)
            {
                BreadCrumb(AppManager.Cms.LeftMenu.IdFunction, AppManager.Cms.LeftMenu.Function.IdTracking, "修改统计设置", AppManager.Cms.Permission.WebSite.Tracking);

                EBooleanUtils.AddListItems(IsTracker, "统计", "不统计");
                ControlUtils.SelectListItemsIgnoreCase(IsTracker, PublishmentSystemInfo.Additional.IsTracker.ToString());

                TrackerDays.Text = PublishmentSystemInfo.Additional.TrackerDays.ToString();
                TrackerPageView.Text = PublishmentSystemInfo.Additional.TrackerPageView.ToString();
                TrackerUniqueVisitor.Text = PublishmentSystemInfo.Additional.TrackerUniqueVisitor.ToString();
                TrackerCurrentMinute.Text = PublishmentSystemInfo.Additional.TrackerCurrentMinute.ToString();
                ETrackerStyleUtils.AddListItems(TrackerStyle);
                ControlUtils.SelectListItems(TrackerStyle, ETrackerStyleUtils.GetValue(PublishmentSystemInfo.Additional.TrackerStyle));
            }
		}

        public override void Submit_OnClick(object sender, EventArgs e)
		{
			if (Page.IsPostBack && Page.IsValid)
			{
                PublishmentSystemInfo.Additional.IsTracker = TranslateUtils.ToBool(IsTracker.SelectedValue);

                PublishmentSystemInfo.Additional.TrackerDays = int.Parse(TrackerDays.Text);
                PublishmentSystemInfo.Additional.TrackerPageView = int.Parse(TrackerPageView.Text);
                PublishmentSystemInfo.Additional.TrackerUniqueVisitor = int.Parse(TrackerUniqueVisitor.Text);
                PublishmentSystemInfo.Additional.TrackerCurrentMinute = int.Parse(TrackerCurrentMinute.Text);
                PublishmentSystemInfo.Additional.TrackerStyle = ETrackerStyleUtils.GetEnumType(TrackerStyle.SelectedValue);

				try
				{
                    DataProvider.PublishmentSystemDao.Update(PublishmentSystemInfo);
                    Body.AddSiteLog(PublishmentSystemId, "修改统计设置");
					SuccessMessage("统计设置修改成功！");
				}
				catch(Exception ex)
				{
					FailMessage(ex, "统计设置修改失败！");
				}
			}
		}

	}
}
