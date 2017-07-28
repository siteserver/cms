using System;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Model;
using SiteServer.BackgroundPages.Controls;
using SiteServer.BackgroundPages.Core;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model.Enumerations;
using SiteServer.CMS.Wcm.GovPublic;

namespace SiteServer.BackgroundPages.Wcm
{
    public class PageGovPublicApplyAnalysis : BasePageGovPublic
	{
        public DateTimeTextBox StartDate;
        public DateTimeTextBox EndDate;
		public Repeater rptContents;

		public void Page_Load(object sender, EventArgs e)
		{
            PageUtils.CheckRequestParameter("PublishmentSystemID");

			if(!IsPostBack)
            {
                BreadCrumb(AppManager.Wcm.LeftMenu.IdGovPublic, AppManager.Wcm.LeftMenu.GovPublic.IdGovPublicAnalysis, "依申请公开统计", AppManager.Wcm.Permission.WebSite.GovPublicAnalysis);

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
                if (departmentIdList.Count == 0)
                {
                    departmentIdList = DepartmentManager.GetDepartmentIdList();
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
            var departmentId = (int)e.Item.DataItem;

            var departmentInfo = DepartmentManager.GetDepartmentInfo(departmentId);

            var ltlTrHtml = e.Item.FindControl("ltlTrHtml") as Literal;
            var ltlTarget = e.Item.FindControl("ltlTarget") as Literal;
            var ltlTotalCount = e.Item.FindControl("ltlTotalCount") as Literal;
            var ltlDoCount = e.Item.FindControl("ltlDoCount") as Literal;
            var ltlUndoCount = e.Item.FindControl("ltlUndoCount") as Literal;
            var ltlBar = e.Item.FindControl("ltlBar") as Literal;

            ltlTrHtml.Text =
                $@"<tr treeItemLevel=""{departmentInfo.ParentsCount + 1}"" style=""{StringUtils.Constants.ShowElementStyle}"">";
            ltlTarget.Text = GetTitle(departmentInfo);

            var totalCount = DataProvider.GovPublicApplyDao.GetCountByDepartmentId(PublishmentSystemId, departmentId, StartDate.DateTime, EndDate.DateTime);
            var doCount = DataProvider.GovPublicApplyDao.GetCountByDepartmentIdAndState(PublishmentSystemId, departmentId, EGovPublicApplyState.Checked, StartDate.DateTime, EndDate.DateTime);
            var unDoCount = totalCount - doCount;

            ltlTotalCount.Text = totalCount.ToString();
            ltlDoCount.Text = doCount.ToString();
            ltlUndoCount.Text = unDoCount.ToString();

            ltlBar.Text = $@"<div class=""progress progress-success progress-striped"">
            <div class=""bar"" style=""width: {GetBarWidth(doCount, totalCount)}%""></div>
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
