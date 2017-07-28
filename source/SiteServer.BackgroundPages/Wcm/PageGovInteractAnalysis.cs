using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Model;
using SiteServer.BackgroundPages.Controls;
using SiteServer.BackgroundPages.Core;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;
using SiteServer.CMS.Model.Enumerations;
using SiteServer.CMS.Wcm.GovInteract;

namespace SiteServer.BackgroundPages.Wcm
{
    public class PageGovInteractAnalysis : BasePageGovInteract
    {
        public DateTimeTextBox StartDate;
        public DateTimeTextBox EndDate;
        public DropDownList ddlNodeID;
		public Repeater rptContents;

        private int _nodeId;

		public void Page_Load(object sender, EventArgs e)
		{
            PageUtils.CheckRequestParameter("PublishmentSystemID");

			if(!IsPostBack)
            {
                BreadCrumb(AppManager.Wcm.LeftMenu.IdGovInteract, AppManager.Wcm.LeftMenu.GovInteract.IdGovInteractAnalysis, "互动交流统计", AppManager.Wcm.Permission.WebSite.GovInteractAnalysis);

                StartDate.Text = string.Empty;
                EndDate.Now = true;

                var nodeInfoList = GovInteractManager.GetNodeInfoList(PublishmentSystemInfo);

                var listItem = new ListItem("<<全部>>", "0");
                ddlNodeID.Items.Add(listItem);
                foreach (var nodeInfo in nodeInfoList)
                {
                    listItem = new ListItem(nodeInfo.NodeName, nodeInfo.NodeId.ToString());
                    ddlNodeID.Items.Add(listItem);
                }

                ClientScriptRegisterClientScriptBlock("TreeScript", DepartmentTreeItem.GetScript(EDepartmentLoadingType.List, null));
                BindGrid();
			}
		}

        public void BindGrid()
        {
            try
            {
                _nodeId = TranslateUtils.ToInt(ddlNodeID.SelectedValue);

                var departmentIdList = new List<int>();

                if (_nodeId > 0)
                {
                    var channelInfo = DataProvider.GovInteractChannelDao.GetChannelInfo(PublishmentSystemId, _nodeId);

                    departmentIdList = BaiRongDataProvider.DepartmentDao.GetDepartmentIdListByFirstDepartmentIdList(GovInteractManager.GetFirstDepartmentIdList(channelInfo));
                }

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

            var ltlTrHtml = (Literal)e.Item.FindControl("ltlTrHtml");
            var ltlTarget = (Literal)e.Item.FindControl("ltlTarget");
            var ltlTotalCount = (Literal)e.Item.FindControl("ltlTotalCount");
            var ltlDoCount = (Literal)e.Item.FindControl("ltlDoCount");
            var ltlUndoCount = (Literal)e.Item.FindControl("ltlUndoCount");
            var ltlBar = (Literal)e.Item.FindControl("ltlBar");

            ltlTrHtml.Text =
                $@"<tr treeItemLevel=""{departmentInfo.ParentsCount + 1}"" style=""{StringUtils.Constants.ShowElementStyle}"">";
            ltlTarget.Text = GetTitle(departmentInfo);

            int totalCount;
            int doCount;
            if (_nodeId == 0)
            {
                totalCount = DataProvider.GovInteractContentDao.GetCountByDepartmentId(PublishmentSystemInfo, departmentId, StartDate.DateTime, EndDate.DateTime);
                doCount = DataProvider.GovInteractContentDao.GetCountByDepartmentIdAndState(PublishmentSystemInfo, departmentId, EGovInteractState.Checked, StartDate.DateTime, EndDate.DateTime);
            }
            else
            {
                totalCount = DataProvider.GovInteractContentDao.GetCountByDepartmentId(PublishmentSystemInfo, departmentId, _nodeId, StartDate.DateTime, EndDate.DateTime);
                doCount = DataProvider.GovInteractContentDao.GetCountByDepartmentIdAndState(PublishmentSystemInfo, departmentId, _nodeId, EGovInteractState.Checked, StartDate.DateTime, EndDate.DateTime);
            
            }
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
