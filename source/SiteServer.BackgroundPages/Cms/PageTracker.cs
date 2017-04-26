using System;
using System.Web.UI.WebControls;
using BaiRong.Core;
using SiteServer.CMS.Core;

namespace SiteServer.BackgroundPages.Cms
{
	public class PageTracker : BasePageCms
    {
		public Label StartDateTime;
		public Label TrackerPageView;
        public Label TrackerUniqueVisitor;
		public Label CurrentVisitorNum;
		public Label TrackingDayNum;
		public Label TotalAccessNum;
		public Label TotalUniqueAccessNum;
		public Label AverageDayAccessNum;
		public Label AverageDayUniqueAccessNum;
		public Label MaxAccessNumOfDay;
		public Label MaxAccessNumOfMonth;
		public Label MaxAccessDay;
		public Label MaxUniqueAccessNumOfDay;
		public Label MaxUniqueAccessNumOfMonth;

		public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("PublishmentSystemID");

            if (!IsPostBack)
            {
                BreadCrumb(AppManager.Cms.LeftMenu.IdFunction, AppManager.Cms.LeftMenu.Function.IdTracking, "网站统计摘要", AppManager.Cms.Permission.WebSite.Tracking);

                var nodeInfo = NodeManager.GetNodeInfo(PublishmentSystemId, PublishmentSystemId);
                var timeSpan = new TimeSpan(DateTime.Now.Ticks - nodeInfo.AddDate.Ticks);
                var trackerPageView = PublishmentSystemInfo.Additional.TrackerPageView;//原有访问量
                var trackerUniqueVisitor = PublishmentSystemInfo.Additional.TrackerUniqueVisitor;//原有访客数
                var trackingdayNumInt = (timeSpan.Days == 0) ? 1 : timeSpan.Days;//总统计天数
                trackingdayNumInt = trackingdayNumInt + PublishmentSystemInfo.Additional.TrackerDays;
                var currentVisitorNumInt = DataProvider.TrackingDao.GetCurrentVisitorNum(PublishmentSystemId, PublishmentSystemInfo.Additional.TrackerCurrentMinute);//当前在线人数
                var totalAccessNumInt = DataProvider.TrackingDao.GetTotalAccessNum(PublishmentSystemId, DateUtils.SqlMinValue);//总访问量
                var totalUniqueAccessNumInt = DataProvider.TrackingDao.GetTotalUniqueAccessNum(PublishmentSystemId, DateUtils.SqlMinValue);//总唯一访客
                var maxAccessDay = string.Empty;//访问量最大的日期
                var maxAccessNumOfDayInt = DataProvider.TrackingDao.GetMaxAccessNumOfDay(PublishmentSystemId, out maxAccessDay);//最大访问量（日）
                var maxAccessNumOfMonthInt = DataProvider.TrackingDao.GetMaxAccessNumOfMonth(PublishmentSystemId);//最大访问量（月）
                var maxUniqueAccessNumOfDayInt = DataProvider.TrackingDao.GetMaxUniqueAccessNumOfDay(PublishmentSystemId);//最大访问量（日）
                var maxUniqueAccessNumOfMonthInt = DataProvider.TrackingDao.GetMaxUniqueAccessNumOfMonth(PublishmentSystemId);//最大访问量（月）


                StartDateTime.Text = DateUtils.GetDateAndTimeString(nodeInfo.AddDate);
                TrackerPageView.Text = trackerPageView.ToString();
                TrackerUniqueVisitor.Text = trackerUniqueVisitor.ToString();
                CurrentVisitorNum.Text = currentVisitorNumInt.ToString();

                TrackingDayNum.Text = trackingdayNumInt.ToString();
                TotalAccessNum.Text = totalAccessNumInt.ToString();
                TotalUniqueAccessNum.Text = totalUniqueAccessNumInt.ToString();
                AverageDayAccessNum.Text = Convert.ToString(Math.Round(Convert.ToDouble(totalAccessNumInt / trackingdayNumInt)));
                AverageDayUniqueAccessNum.Text = Convert.ToString(Math.Round(Convert.ToDouble(totalUniqueAccessNumInt / trackingdayNumInt)));
                MaxAccessDay.Text = maxAccessDay;
                MaxAccessNumOfDay.Text = maxAccessNumOfDayInt.ToString();
                MaxAccessNumOfMonth.Text = maxAccessNumOfMonthInt.ToString();
                MaxUniqueAccessNumOfDay.Text = maxUniqueAccessNumOfDayInt.ToString();
                MaxUniqueAccessNumOfMonth.Text = maxUniqueAccessNumOfMonthInt.ToString();
            }		
		}

	}
}
