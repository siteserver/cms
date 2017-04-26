using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Model.Enumerations;
using SiteServer.BackgroundPages.Core;
using SiteServer.CMS.Core;

namespace SiteServer.BackgroundPages.Cms
{
    public class PageTrackerContentRank : BasePageCms
    {
        public DataGrid dgContents;
        public LinkButton pageFirst;
        public LinkButton pageLast;
        public LinkButton pageNext;
        public LinkButton pagePrevious;
        public Label currentPage;

        private List<KeyValuePair<int, int>> _accessNumPairList = new List<KeyValuePair<int, int>>();
        private Hashtable todayAccessNumHashtable = new Hashtable();
        private int totalAccessNum = 0;

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

        public int GetTodayAccessNum(int pageContentID)
        {
            var todayAccessNum = 0;
            if (todayAccessNumHashtable[pageContentID] != null)
            {
                todayAccessNum = Convert.ToInt32(todayAccessNumHashtable[pageContentID]);
            }
            return todayAccessNum;
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("PublishmentSystemID");

            _accessNumPairList = DataProvider.TrackingDao.GetContentAccessNumPairList(PublishmentSystemId);
            todayAccessNumHashtable = DataProvider.TrackingDao.GetTodayContentAccessNumHashtable(PublishmentSystemId);

            foreach (var pair in _accessNumPairList)
            {
                var accessNum = pair.Value;
                totalAccessNum += accessNum;
            }
            if (!IsPostBack)
            {
                BreadCrumb(AppManager.Cms.LeftMenu.IdFunction, AppManager.Cms.LeftMenu.Function.IdTracking, "内容流量排名", AppManager.Cms.Permission.WebSite.Tracking);

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
            var arraylist = new ArrayList();
            foreach (var pair in _accessNumPairList)
            {
                var pageContentId = pair.Key;
                var accessNum = pair.Value;
                var pageContentIdWithAccessNum = new PageContentIDWithAccessNum(pageContentId, accessNum);
                arraylist.Add(pageContentIdWithAccessNum);
            }

            dgContents.DataSource = arraylist;
            dgContents.ItemDataBound += MyDataGrid_ItemDataBound;
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

        void MyDataGrid_ItemDataBound(object sender, DataGridItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var ltlTitle = (Literal)e.Item.FindControl("ltlTitle");

                var pageContentIdWithAccessNum = e.Item.DataItem as PageContentIDWithAccessNum;

                if (pageContentIdWithAccessNum != null && pageContentIdWithAccessNum.PageContentID > 0)
                {
                    var contentInfo = DataProvider.ContentDao.GetContentInfo(ETableStyle.BackgroundContent, PublishmentSystemInfo.AuxiliaryTableForContent, pageContentIdWithAccessNum.PageContentID);
                    if (contentInfo != null && !string.IsNullOrEmpty(contentInfo.Title))
                    {
                        ltlTitle.Text = WebUtils.GetContentTitle(PublishmentSystemInfo, contentInfo,
                            PageUtils.GetCmsUrl(nameof(PageTrackerContentRank), new NameValueCollection
                            {
                                {"PublishmentSystemID", PublishmentSystemId.ToString()}
                            }));
                    }
                    else
                    {
                        e.Item.Visible = false;
                    }
                }
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
            }
            BindGrid();
        }

        public class PageContentIDWithAccessNum
        {
            protected int m_intPageContentID;
            protected int m_intAccessNum;

            public PageContentIDWithAccessNum(int pageContentID, int accessNum)
            {
                m_intPageContentID = pageContentID;
                m_intAccessNum = accessNum;
            }

            public int PageContentID
            {
                get
                {
                    return m_intPageContentID;
                }
                set
                {
                    m_intPageContentID = value;
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
                    m_intAccessNum = value;
                }
            }
        }
    }
}
