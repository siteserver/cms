using System;
using System.Collections;
using System.Web.UI.WebControls;
using BaiRong.Core;
using SiteServer.CMS.Core;

namespace SiteServer.BackgroundPages.Cms
{
	public class PageTrackerReferrer : BasePageCms
    {
		public DataGrid dgContents;
		public LinkButton pageFirst;
		public LinkButton pageLast;
		public LinkButton pageNext;
		public LinkButton pagePrevious;
		public Label currentPage;

		private Hashtable accessNumHashtable = new Hashtable();
		private Hashtable uniqueAccessNumHashtable = new Hashtable();
		private Hashtable todayAccessNumHashtable = new Hashtable();
		private Hashtable todayUniqueAccessNumHashtable = new Hashtable();
		private int totalAccessNum = 0;

		public string GetReferrerUrl(string referrer)
		{
			if (referrer != null && referrer.Trim().Length == 0)
			{
				return "直接输入网址";
			}
			else
			{
				return $"<a HREF=\"{referrer}\" TARGET=\"_BLANK\">{referrer}</a>";
			}
		}

		public int GetAccessNum(string referrer)
		{
			var accessNum = 0;
			if (accessNumHashtable[referrer] != null)
			{
				accessNum = Convert.ToInt32(accessNumHashtable[referrer]);
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

		public int GetUniqueAccessNum(string referrer)
		{
			var uniqueAccessNum = 0;
			if (uniqueAccessNumHashtable[referrer] != null)
			{
				uniqueAccessNum = Convert.ToInt32(uniqueAccessNumHashtable[referrer]);
			}
			return uniqueAccessNum;
		}

		public int GetTodayAccessNum(string referrer)
		{
			var todayAccessNum = 0;
			if (todayAccessNumHashtable[referrer] != null)
			{
				todayAccessNum = Convert.ToInt32(todayAccessNumHashtable[referrer]);
			}
			return todayAccessNum;
		}

		public int GetTodayUniqueAccessNum(string referrer)
		{
			var todayUniqueAccessNum = 0;
			if (todayUniqueAccessNumHashtable[referrer] != null)
			{
				todayUniqueAccessNum = Convert.ToInt32(todayUniqueAccessNumHashtable[referrer]);
			}
			return todayUniqueAccessNum;
		}

		public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("PublishmentSystemID");

            accessNumHashtable = DataProvider.TrackingDao.GetReferrerAccessNumHashtable(PublishmentSystemId);
            uniqueAccessNumHashtable = DataProvider.TrackingDao.GetReferrerUniqueAccessNumHashtable(PublishmentSystemId);
            todayAccessNumHashtable = DataProvider.TrackingDao.GetReferrerTodayAccessNumHashtable(PublishmentSystemId);
            todayUniqueAccessNumHashtable = DataProvider.TrackingDao.GetReferrerTodayUniqueAccessNumHashtable(PublishmentSystemId);

            foreach (int accessNum in accessNumHashtable.Values)
            {
                totalAccessNum += accessNum;
            }

            if (!IsPostBack)
            {
                BreadCrumb(AppManager.Cms.LeftMenu.IdFunction, AppManager.Cms.LeftMenu.Function.IdTracking, "来访网址统计", AppManager.Cms.Permission.WebSite.Tracking);

                BindGrid();
            }
		}

		public void MyDataGrid_Page(object sender, DataGridPageChangedEventArgs e)
		{
            dgContents.CurrentPageIndex = e.NewPageIndex;
			BindGrid();
		}

		public void BindGrid()
		{
			try
			{
				var arraylist = new ArrayList();
				foreach (string referrer in accessNumHashtable.Keys)
				{
					var accessNum = GetAccessNum(referrer);
					var referrerWithAccessNum = new ReferrerWithAccessNum(referrer, accessNum);
					arraylist.Add(referrerWithAccessNum);
				}

                dgContents.DataSource = arraylist;
                dgContents.PageSize = PublishmentSystemInfo.Additional.PageSize;
                dgContents.DataBind();


                if (dgContents.CurrentPageIndex > 0)
				{
					pageFirst.Enabled = true;
					pagePrevious.Enabled = true;
				}
				else
				{
					pageFirst.Enabled = false;
					pagePrevious.Enabled = false;
				}

                if (dgContents.CurrentPageIndex + 1 == dgContents.PageCount)
				{
					pageLast.Enabled = false;
					pageNext.Enabled = false;
				}
				else
				{
					pageLast.Enabled = true;
					pageNext.Enabled = true;
				}

                currentPage.Text = $"{dgContents.CurrentPageIndex + 1}/{dgContents.PageCount}";
			}
			catch (Exception ex)
			{
                PageUtils.RedirectToErrorPage(ex.Message);
			}

		}


		protected void NavigationButtonClick(object sender, EventArgs e)
		{
			var button = (LinkButton)sender;
			var direction = button.CommandName;

			switch (direction.ToUpper())
			{
				case "FIRST":
                    dgContents.CurrentPageIndex = 0;
					break;
				case "PREVIOUS":
                    dgContents.CurrentPageIndex =
                        Math.Max(dgContents.CurrentPageIndex - 1, 0);
					break;
				case "NEXT":
                    dgContents.CurrentPageIndex =
                        Math.Min(dgContents.CurrentPageIndex + 1,
                        dgContents.PageCount - 1);
					break;
				case "LAST":
                    dgContents.CurrentPageIndex = dgContents.PageCount - 1;
					break;
				default:
					break;
			}
			BindGrid();
		}

		public class ReferrerWithAccessNum
		{
			protected string m_strreferrer;
			protected int m_intAccessNum;

			public ReferrerWithAccessNum(string referrer, int accessNum)
			{
				m_strreferrer = referrer;
				m_intAccessNum = accessNum;
			}

			public string referrer
			{
				get
				{
					return m_strreferrer;
				}
				set
				{
					m_strreferrer =  value;
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
