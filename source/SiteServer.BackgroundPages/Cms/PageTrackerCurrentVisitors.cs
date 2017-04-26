using System;
using System.Web.UI.WebControls;
using BaiRong.Core;
using SiteServer.CMS.Core;

namespace SiteServer.BackgroundPages.Cms
{
	public class PageTrackerCurrentVisitors : BasePageCms
    {
		public DataGrid dgContents;
		public LinkButton pageFirst;
		public LinkButton pageLast;
		public LinkButton pageNext;
		public LinkButton pagePrevious;
		public Label currentPage;

		public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("PublishmentSystemID");

            if (!IsPostBack)
            {
                BreadCrumb(AppManager.Cms.LeftMenu.IdFunction, AppManager.Cms.LeftMenu.Function.IdTracking, "最近访问统计", AppManager.Cms.Permission.WebSite.Tracking);

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
                dgContents.DataSource = DataProvider.TrackingDao.GetDataSource(PublishmentSystemId, PublishmentSystemInfo.Additional.TrackerCurrentMinute);
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
	}
}
