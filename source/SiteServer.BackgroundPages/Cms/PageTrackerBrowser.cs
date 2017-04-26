using System;
using System.Collections;
using System.Web.UI.WebControls;
using BaiRong.Core;
using SiteServer.CMS.Core;

namespace SiteServer.BackgroundPages.Cms
{
	public class PageTrackerBrowser : BasePageCms
    {
		public DataGrid dgContents;
		private Hashtable accessNumHashtable = new Hashtable();
		private Hashtable uniqueAccessNumHashtable = new Hashtable();
		private Hashtable todayAccessNumHashtable = new Hashtable();
		private Hashtable todayUniqueAccessNumHashtable = new Hashtable();
		private int totalAccessNum = 0;

		public int GetAccessNum(string browser)
		{
			var accessNum = 0;
			if (accessNumHashtable[browser] != null)
			{
				accessNum = Convert.ToInt32(accessNumHashtable[browser]);
			}
			return accessNum;
		}

		public double GetAccessNumBarWidth(int accessNum)
		{
			double width = 0;
            if (totalAccessNum > 0)
			{
                width = Convert.ToDouble(accessNum) / Convert.ToDouble(totalAccessNum);
				width = Math.Round(width, 2) * 100;
			}
			return width;
		}

		public int GetUniqueAccessNum(string browser)
		{
			var uniqueAccessNum = 0;
			if (uniqueAccessNumHashtable[browser] != null)
			{
				uniqueAccessNum = Convert.ToInt32(uniqueAccessNumHashtable[browser]);
			}
			return uniqueAccessNum;
		}

		public int GetTodayAccessNum(string browser)
		{
			var todayAccessNum = 0;
			if (todayAccessNumHashtable[browser] != null)
			{
				todayAccessNum = Convert.ToInt32(todayAccessNumHashtable[browser]);
			}
			return todayAccessNum;
		}

		public int GetTodayUniqueAccessNum(string browser)
		{
			var todayUniqueAccessNum = 0;
			if (todayUniqueAccessNumHashtable[browser] != null)
			{
				todayUniqueAccessNum = Convert.ToInt32(todayUniqueAccessNumHashtable[browser]);
			}
			return todayUniqueAccessNum;
		}

		public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("PublishmentSystemID");

            if (!IsPostBack)
            {
                BreadCrumb(AppManager.Cms.LeftMenu.IdFunction, AppManager.Cms.LeftMenu.Function.IdTracking, "浏览器统计", AppManager.Cms.Permission.WebSite.Tracking);

                accessNumHashtable = DataProvider.TrackingDao.GetBrowserAccessNumHashtable(PublishmentSystemId);
                uniqueAccessNumHashtable = DataProvider.TrackingDao.GetBrowserUniqueAccessNumHashtable(PublishmentSystemId);
                todayAccessNumHashtable = DataProvider.TrackingDao.GetBrowserTodayAccessNumHashtable(PublishmentSystemId);
                todayUniqueAccessNumHashtable = DataProvider.TrackingDao.GetBrowserTodayUniqueAccessNumHashtable(PublishmentSystemId);

                foreach (int accessNum in accessNumHashtable.Values)
                {
                    totalAccessNum += accessNum;
                }

                BindGrid();
            }
		}

		public void BindGrid()
		{
			try
			{
				var arraylist = new ArrayList();
				foreach (string browser in accessNumHashtable.Keys)
				{
					var accessNum = GetAccessNum(browser);
					var browserWithAccessNum = new BrowserWithAccessNum(browser, accessNum);
					arraylist.Add(browserWithAccessNum);
				}

                dgContents.DataSource = arraylist;
                dgContents.DataBind();
			}
			catch(Exception ex)
			{
                PageUtils.RedirectToErrorPage(ex.Message);
			}
		}

		public class BrowserWithAccessNum
		{
			protected string m_strBrowser;
			protected int m_intAccessNum;

			public BrowserWithAccessNum(string browser, int accessNum)
			{
				m_strBrowser = browser;
				m_intAccessNum = accessNum;
			}

			public string Browser
			{
				get
				{
					return m_strBrowser;
				}
				set
				{
					m_strBrowser =  value;
				}
			}

			public int AccessNum
			{
				get
				{
					return m_intAccessNum;
				}
				set
				{
					m_intAccessNum =  value;
				}
			}
		}
	}
}
