using System;
using System.Collections;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Model.Enumerations;
using SiteServer.CMS.Core;

namespace SiteServer.BackgroundPages.Cms
{
	public class PageTrackerDay : BasePageCms
    {
        public Button ExportTracking;
	    readonly Hashtable accessNumHashtable = new Hashtable();
		int maxAccessNum = 0;
	    readonly Hashtable uniqueAccessNumHashtable = new Hashtable();
		int uniqueMaxAccessNum = 0;

        public int count = 30;
        public EStatictisXType xType = EStatictisXType.Day;

        public string GetGraphicX(int index)
        {
            var xNum = 0;
            var datetime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, 0, 0);
            if (Equals(xType, EStatictisXType.Day))
            {
                datetime = datetime.AddDays(-(count - index));
                xNum = datetime.Day;
            }
            else if (Equals(xType, EStatictisXType.Month))
            {
                datetime = datetime.AddMonths(-(count - index));
                xNum = datetime.Month;
            }
            else if (Equals(xType, EStatictisXType.Year))
            {
                datetime = datetime.AddYears(-(count - index));
                xNum = datetime.Year;
            }
            else if (Equals(xType, EStatictisXType.Hour))
            {
                datetime = datetime.AddHours(-(count - index));
                xNum = datetime.Hour;
            }
            return xNum.ToString();
        }

        public string GetGraphicY(int index)
        {
            if (index <= 0 || index > count) return string.Empty;
            var accessNum = (int)accessNumHashtable[index];
            return accessNum.ToString();
        }

        public string GetUniqueGraphicY(int index)
        {
            if (index <= 0 || index > count) return string.Empty;
            var accessNum = (int)uniqueAccessNumHashtable[index];
            return accessNum.ToString();
        }

		public double GetAccessNum(int index)
		{
			double accessNum = 0;
			if (maxAccessNum > 0)
			{
				accessNum = (Convert.ToDouble(maxAccessNum) * Convert.ToDouble(index)) / 8;
				accessNum = Math.Round(accessNum, 2);
			}
			return accessNum;
		}

		public string GetGraphicHtml(int index)
		{
			if (index <= 0 || index > 30) return string.Empty;
			var accessNum = (int)accessNumHashtable[index];
			var datetime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);
			datetime = datetime.AddDays(-(30 - index));
			double height = 0;
			if (maxAccessNum >0)
			{
				height = (Convert.ToDouble(accessNum) / Convert.ToDouble(maxAccessNum)) * 200.0;
			}
            string html =
                $"<IMG title=访问量：{accessNum} height={height} style=height:{height}px src=../pic/tracker_bar.gif width=16><BR>{datetime.Day}";
			return html;
		}

		public double GetUniqueAccessNum(int index)
		{
			double uniqueAccessNum = 0;
			if (uniqueMaxAccessNum > 0)
			{
				uniqueAccessNum = (Convert.ToDouble(uniqueMaxAccessNum) * Convert.ToDouble(index)) / 8;
				uniqueAccessNum = Math.Round(uniqueAccessNum, 2);
			}
			return uniqueAccessNum;
		}

		public string GetUniqueGraphicHtml(int index)
		{
			if (index <= 0 || index > 30) return string.Empty;
			var uniqueAccessNum = (int)uniqueAccessNumHashtable[index];
			var datetime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);
			datetime = datetime.AddDays(-(30 - index));
			double height = 0;
			if (uniqueMaxAccessNum >0)
			{
				height = (Convert.ToDouble(uniqueAccessNum) / Convert.ToDouble(uniqueMaxAccessNum)) * 200.0;
			}
            string html =
                $"<IMG title=访客数：{uniqueAccessNum} height={height} style=height:{height}px src=../pic/tracker_bar.gif width=16><BR>{datetime.Day}";
			return html;
		}

		public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("PublishmentSystemID");

            if (!IsPostBack)
            {
                BreadCrumb(AppManager.Cms.LeftMenu.IdFunction, AppManager.Cms.LeftMenu.Function.IdTracking, "天统计", AppManager.Cms.Permission.WebSite.Tracking);

                var trackingDayHashtable = DataProvider.TrackingDao.GetTrackingDayHashtable(PublishmentSystemId);
                var uniqueTrackingDayHashtable = DataProvider.TrackingDao.GetUniqueTrackingDayHashtable(PublishmentSystemId);
                var now = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);
                for (var i = 0; i < 30; i++)
                {
                    var datetime = now.AddDays(-i);
                    var accessNum = 0;
                    if (trackingDayHashtable[datetime] != null)
                    {
                        accessNum = (int)trackingDayHashtable[datetime];
                    }
                    accessNumHashtable.Add(30 - i, accessNum);
                    if (accessNum > maxAccessNum)
                    {
                        maxAccessNum = accessNum;
                    }

                    var uniqueAccessNum = 0;
                    if (uniqueTrackingDayHashtable[datetime] != null)
                    {
                        uniqueAccessNum = (int)uniqueTrackingDayHashtable[datetime];
                    }
                    uniqueAccessNumHashtable.Add(30 - i, uniqueAccessNum);
                    if (uniqueAccessNum > uniqueMaxAccessNum)
                    {
                        uniqueMaxAccessNum = uniqueAccessNum;
                    }
                }

                ExportTracking.Attributes.Add("onclick", ModalExportMessage.GetOpenWindowStringToExport(PublishmentSystemId, ModalExportMessage.ExportTypeTrackerDay));
            }
		}

	}
}
