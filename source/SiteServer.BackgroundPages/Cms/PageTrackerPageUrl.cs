using System;
using System.Collections;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using BaiRong.Core;
using SiteServer.CMS.Core;

namespace SiteServer.BackgroundPages.Cms
{
	public class PageTrackerPageUrl : BasePageCms
    {
		public DataGrid dgContents;
		public LinkButton pageFirst;
		public LinkButton pageLast;
		public LinkButton pageNext;
		public LinkButton pagePrevious;
		public Label currentPage;

        private List<KeyValuePair<string, int>> _accessNumPairList = new List<KeyValuePair<string, int>>();
		private Hashtable uniqueAccessNumHashtable = new Hashtable();
		private Hashtable todayAccessNumHashtable = new Hashtable();
		private Hashtable todayUniqueAccessNumHashtable = new Hashtable();
		private int totalAccessNum = 0;

        //public int GetAccessNum(string pageUrl)
        //{
        //    int accessNum = 0;
        //    if (this.accessNumHashtable[pageUrl] != null)
        //    {
        //        accessNum = Convert.ToInt32(this.accessNumHashtable[pageUrl]);
        //    }
        //    return accessNum;
        //}

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

		public int GetUniqueAccessNum(string pageUrl)
		{
			var uniqueAccessNum = 0;
			if (uniqueAccessNumHashtable[pageUrl] != null)
			{
				uniqueAccessNum = Convert.ToInt32(uniqueAccessNumHashtable[pageUrl]);
			}
			return uniqueAccessNum;
		}

		public int GetTodayAccessNum(string pageUrl)
		{
			var todayAccessNum = 0;
			if (todayAccessNumHashtable[pageUrl] != null)
			{
				todayAccessNum = Convert.ToInt32(todayAccessNumHashtable[pageUrl]);
			}
			return todayAccessNum;
		}

		public int GetTodayUniqueAccessNum(string pageUrl)
		{
			var todayUniqueAccessNum = 0;
			if (todayUniqueAccessNumHashtable[pageUrl] != null)
			{
				todayUniqueAccessNum = Convert.ToInt32(todayUniqueAccessNumHashtable[pageUrl]);
			}
			return todayUniqueAccessNum;
		}

		public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("PublishmentSystemID");

            _accessNumPairList = DataProvider.TrackingDao.GetPageUrlAccessPairList(PublishmentSystemId);
            uniqueAccessNumHashtable = DataProvider.TrackingDao.GetPageUrlUniqueAccessNumHashtable(PublishmentSystemId);
            todayAccessNumHashtable = DataProvider.TrackingDao.GetPageUrlTodayAccessNumHashtable(PublishmentSystemId);
            todayUniqueAccessNumHashtable = DataProvider.TrackingDao.GetPageUrlTodayUniqueAccessNumHashtable(PublishmentSystemId);

            foreach (var pair in _accessNumPairList)
            {
                var accessNum = pair.Value;
                totalAccessNum += accessNum;
            }
            if (!IsPostBack)
            {
                BreadCrumb(AppManager.Cms.LeftMenu.IdFunction, AppManager.Cms.LeftMenu.Function.IdTracking, "受访页面统计", AppManager.Cms.Permission.WebSite.Tracking);

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
				foreach (var pair in _accessNumPairList)
				{
                    var pageUrl = pair.Key;
                    var accessNum = pair.Value;
					//int accessNum = this.GetAccessNum(pageUrl);
					var pageUrlWithAccessNum = new PageUrlWithAccessNum(pageUrl, accessNum);
					arraylist.Add(pageUrlWithAccessNum);
				}

                dgContents.DataSource = arraylist;
                dgContents.PageSize = PublishmentSystemManager.GetPublishmentSystemInfo(PublishmentSystemId).Additional.PageSize;
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

		public class PageUrlWithAccessNum
		{
			protected string m_strPageUrl;
			protected int m_intAccessNum;

			public PageUrlWithAccessNum(string pageUrl, int accessNum)
			{
				m_strPageUrl = pageUrl;
				m_intAccessNum = accessNum;
			}

			public string PageUrl
			{
				get
				{
					return m_strPageUrl;
				}
				set
				{
					m_strPageUrl =  value;
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
