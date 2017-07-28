using System;
using System.Collections;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Model;
using SiteServer.BackgroundPages.Controls;
using SiteServer.BackgroundPages.Core;
using SiteServer.CMS.Core;
using SiteServer.CMS.Wcm.GovPublic;

namespace SiteServer.BackgroundPages.Wcm
{
    public class PageGovPublicAnalysisDepartment : BasePageGovPublic
	{
        public DateTimeTextBox StartDate;
        public DateTimeTextBox EndDate;
		public Repeater rptContents;

        private int totalCount = 0;
        private Hashtable countHashtable = new Hashtable();

		public void Page_Load(object sender, EventArgs e)
		{
            PageUtils.CheckRequestParameter("PublishmentSystemID");

			if(!IsPostBack)
            {
                BreadCrumb(AppManager.Wcm.LeftMenu.IdGovPublic, AppManager.Wcm.LeftMenu.GovPublic.IdGovPublicAnalysis, "主动公开统计", AppManager.Wcm.Permission.WebSite.GovPublicAnalysis);

                StartDate.Text = string.Empty;
                EndDate.Now = true;

                ClientScriptRegisterClientScriptBlock("TreeScript", DepartmentTreeItem.GetScript(EDepartmentLoadingType.List, null));
                BindGrid();
			}
		}

        public void BindGrid()
        {
            try
            {
                var departmentIdList = BaiRongDataProvider.DepartmentDao.GetDepartmentIdListByFirstDepartmentIdList(GovPublicManager.GetFirstDepartmentIdList(PublishmentSystemInfo));

                foreach (var departmentId in departmentIdList)
                {
                    var count = DataProvider.GovPublicContentDao.GetCountByDepartmentId(PublishmentSystemInfo, departmentId, StartDate.DateTime, EndDate.DateTime);
                    totalCount += count;
                    countHashtable[departmentId] = count;
                }

                rptContents.DataSource = departmentIdList;
                rptContents.ItemDataBound += rptContents_ItemDataBound;
                rptContents.DataBind();
            }
            catch (Exception ex)
            {
                PageUtils.RedirectToErrorPage(ex.Message);
            }
        }

        void rptContents_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            var departmentID = (int)e.Item.DataItem;

            var departmentInfo = DepartmentManager.GetDepartmentInfo(departmentID);

            var ltlTrHtml = e.Item.FindControl("ltlTrHtml") as Literal;
            var ltlTarget = e.Item.FindControl("ltlTarget") as Literal;
            var ltlCount = e.Item.FindControl("ltlCount") as Literal;
            var ltlBar = e.Item.FindControl("ltlBar") as Literal;

            ltlTrHtml.Text =
                $@"<tr treeItemLevel=""{departmentInfo.ParentsCount + 1}"" style=""{StringUtils.Constants.ShowElementStyle}"">";
            ltlTarget.Text = GetTitle(departmentInfo);

            var count = (int)countHashtable[departmentID];

            ltlCount.Text = count.ToString();

            ltlBar.Text = $@"<div class=""progress progress-success progress-striped"">
            <div class=""bar"" style=""width: {GetBarWidth(count, totalCount)}%""></div>
          </div>";
        }

        private double GetBarWidth(int doCount, int totalCount)
        {
            double width = 0;
            if (totalCount > 0)
            {
                width = Convert.ToDouble(doCount) / Convert.ToDouble(totalCount);
                width = Math.Round(width, 2) * 100;
            }
            return width;
        }

		public void Analysis_OnClick(object sender, EventArgs e)
		{
			BindGrid();
		}

        private string GetTitle(DepartmentInfo departmentInfo)
        {
            var treeItem = DepartmentTreeItem.CreateInstance(departmentInfo);
            return treeItem.GetItemHtml(EDepartmentLoadingType.List, null, true);
        }
	}
}
