using System;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Model.Enumerations;
using SiteServer.CMS.Core;

namespace SiteServer.BackgroundPages.Plugins
{
	public class PageTrackerEdit : BasePageCms
    {
        public RadioButtonList RblIsCountHits;
        public PlaceHolder PhIsCountHitsByDay;
        public RadioButtonList RblIsCountHitsByDay;
        public RadioButtonList RblIsCountDownload;
        public RadioButtonList IsTracker;

        public TextBox TrackerDays;
        public TextBox TrackerPageView;
        public TextBox TrackerUniqueVisitor;
        public TextBox TrackerCurrentMinute;

		public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("PublishmentSystemID");

            if (!IsPostBack)
            {
                EBooleanUtils.AddListItems(RblIsCountHits, "统计", "不统计");
                ControlUtils.SelectListItemsIgnoreCase(RblIsCountHits, PublishmentSystemInfo.Additional.IsCountHits.ToString());

                EBooleanUtils.AddListItems(RblIsCountHitsByDay, "统计", "不统计");
                ControlUtils.SelectListItemsIgnoreCase(RblIsCountHitsByDay, PublishmentSystemInfo.Additional.IsCountHitsByDay.ToString());

                EBooleanUtils.AddListItems(RblIsCountDownload, "统计", "不统计");
                ControlUtils.SelectListItemsIgnoreCase(RblIsCountDownload, PublishmentSystemInfo.Additional.IsCountDownload.ToString());

                RblIsCountHits_SelectedIndexChanged(null, EventArgs.Empty);

                EBooleanUtils.AddListItems(IsTracker, "统计", "不统计");
                ControlUtils.SelectListItemsIgnoreCase(IsTracker, PublishmentSystemInfo.Additional.IsTracker.ToString());

                TrackerDays.Text = PublishmentSystemInfo.Additional.TrackerDays.ToString();
                TrackerPageView.Text = PublishmentSystemInfo.Additional.TrackerPageView.ToString();
                TrackerUniqueVisitor.Text = PublishmentSystemInfo.Additional.TrackerUniqueVisitor.ToString();
                TrackerCurrentMinute.Text = PublishmentSystemInfo.Additional.TrackerCurrentMinute.ToString();
            }
		}

        public void RblIsCountHits_SelectedIndexChanged(object sender, EventArgs e)
        {
            PhIsCountHitsByDay.Visible = TranslateUtils.ToBool(RblIsCountHits.SelectedValue);
        }

        public override void Submit_OnClick(object sender, EventArgs e)
		{
			if (Page.IsPostBack && Page.IsValid)
			{
                PublishmentSystemInfo.Additional.IsCountHits = TranslateUtils.ToBool(RblIsCountHits.SelectedValue);
                PublishmentSystemInfo.Additional.IsCountHitsByDay = TranslateUtils.ToBool(RblIsCountHitsByDay.SelectedValue);
                PublishmentSystemInfo.Additional.IsCountDownload = TranslateUtils.ToBool(RblIsCountDownload.SelectedValue);

                PublishmentSystemInfo.Additional.IsTracker = TranslateUtils.ToBool(IsTracker.SelectedValue);

                PublishmentSystemInfo.Additional.TrackerDays = int.Parse(TrackerDays.Text);
                PublishmentSystemInfo.Additional.TrackerPageView = int.Parse(TrackerPageView.Text);
                PublishmentSystemInfo.Additional.TrackerUniqueVisitor = int.Parse(TrackerUniqueVisitor.Text);
                PublishmentSystemInfo.Additional.TrackerCurrentMinute = int.Parse(TrackerCurrentMinute.Text);

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
