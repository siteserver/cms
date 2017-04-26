using System;
using System.Collections;
using System.Web.UI.WebControls;
using BaiRong.Core;
using SiteServer.CMS.Core;

namespace SiteServer.BackgroundPages.Cms
{
	public class PageTrackerOS : BasePageCms
    {
		public DataGrid dgContents;
		private Hashtable accessNumHashtable = new Hashtable();
		private Hashtable uniqueAccessNumHashtable = new Hashtable();
		private Hashtable todayAccessNumHashtable = new Hashtable();
		private Hashtable todayUniqueAccessNumHashtable = new Hashtable();
		private int totalAccessNum = 0;

		public int GetAccessNum(string os)
		{
			var accessNum = 0;
			if (accessNumHashtable[os] != null)
			{
				accessNum = Convert.ToInt32(accessNumHashtable[os]);
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

		public int GetUniqueAccessNum(string os)
		{
			var uniqueAccessNum = 0;
			if (uniqueAccessNumHashtable[os] != null)
			{
				uniqueAccessNum = Convert.ToInt32(uniqueAccessNumHashtable[os]);
			}
			return uniqueAccessNum;
		}

		public int GetTodayAccessNum(string os)
		{
			var todayAccessNum = 0;
			if (todayAccessNumHashtable[os] != null)
			{
				todayAccessNum = Convert.ToInt32(todayAccessNumHashtable[os]);
			}
			return todayAccessNum;
		}

		public int GetTodayUniqueAccessNum(string os)
		{
			var todayUniqueAccessNum = 0;
			if (todayUniqueAccessNumHashtable[os] != null)
			{
				todayUniqueAccessNum = Convert.ToInt32(todayUniqueAccessNumHashtable[os]);
			}
			return todayUniqueAccessNum;
		}

		public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("PublishmentSystemID");

            if (!IsPostBack)
            {
                BreadCrumb(AppManager.Cms.LeftMenu.IdFunction, AppManager.Cms.LeftMenu.Function.IdTracking, "操作系统统计", AppManager.Cms.Permission.WebSite.Tracking);

                accessNumHashtable = DataProvider.TrackingDao.GetOsAccessNumHashtable(PublishmentSystemId);
                uniqueAccessNumHashtable = DataProvider.TrackingDao.GetOsUniqueAccessNumHashtable(PublishmentSystemId);
                todayAccessNumHashtable = DataProvider.TrackingDao.GetOsTodayAccessNumHashtable(PublishmentSystemId);
                todayUniqueAccessNumHashtable = DataProvider.TrackingDao.GetOsTodayUniqueAccessNumHashtable(PublishmentSystemId);

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
				foreach (string OS in accessNumHashtable.Keys)
				{
					var accessNum = GetAccessNum(OS);
					var osWithAccessNum = new OSWithAccessNum(OS, accessNum);
					arraylist.Add(osWithAccessNum);
				}

                dgContents.DataSource = arraylist;
                dgContents.DataBind();
			}
			catch(Exception ex)
			{
                PageUtils.RedirectToErrorPage(ex.Message);
			}
		}

		public class OSWithAccessNum
		{
			protected string m_strOS;
			protected int m_intAccessNum;

			public OSWithAccessNum(string os, int accessNum)
			{
				m_strOS = os;
				m_intAccessNum = accessNum;
			}

			public string OS
			{
				get
				{
					return m_strOS;
				}
				set
				{
					m_strOS =  value;
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
