using System;
using System.Collections;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using BaiRong.Core;
using SiteServer.BackgroundPages.Settings;
using SiteServer.CMS.Core;
using SiteServer.CMS.Core.Create;
using SiteServer.CMS.Model.Enumerations;

namespace SiteServer.BackgroundPages.Cms
{
    public class PageCreateChannel : BasePageCms
    {
        public ListBox NodeIDCollectionToCreate;
        public DropDownList ChooseScope;
        public Button DeleteAllNodeButton;
        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("PublishmentSystemID");

            if (!IsPostBack)
            {
                BreadCrumb(AppManager.Cms.LeftMenu.IdCreate, "生成栏目页", AppManager.Permissions.WebSite.Create);

                var listitem = new ListItem("所有选中的栏目", "All");
                ChooseScope.Items.Add(listitem);
                listitem = new ListItem("一个月内更新的栏目", "Month");
                ChooseScope.Items.Add(listitem);
                listitem = new ListItem("一天内更新的栏目", "Day");
                ChooseScope.Items.Add(listitem);
                listitem = new ListItem("2小时内更新的栏目", "2Hour");
                ChooseScope.Items.Add(listitem);

                NodeManager.AddListItems(NodeIDCollectionToCreate.Items, PublishmentSystemInfo, false, true, Body.AdministratorName);
                DeleteAllNodeButton.Attributes.Add("onclick", "return confirm(\"此操作将删除所有已生成的栏目页面，确定吗？\");");
            }
        }


        public void CreateNodeButton_OnClick(object sender, EventArgs e)
        {
            if (!Page.IsPostBack || !Page.IsValid) return;

            var nodeIdList = new List<int>();
            var selectedNodeIdArrayList = ControlUtils.GetSelectedListControlValueArrayList(NodeIDCollectionToCreate);

            var tableName = PublishmentSystemInfo.AuxiliaryTableForContent;

            if (ChooseScope.SelectedValue == "Month")
            {
                var lastEditList = BaiRongDataProvider.ContentDao.GetNodeIdListCheckedByLastEditDateHour(tableName, PublishmentSystemId, 720);
                foreach (var nodeId in lastEditList)
                {
                    if (selectedNodeIdArrayList.Contains(nodeId.ToString()))
                    {
                        nodeIdList.Add(nodeId);
                    }
                }
            }
            else if (ChooseScope.SelectedValue == "Day")
            {
                var lastEditList = BaiRongDataProvider.ContentDao.GetNodeIdListCheckedByLastEditDateHour(tableName, PublishmentSystemId, 24);
                foreach (var nodeId in lastEditList)
                {
                    if (selectedNodeIdArrayList.Contains(nodeId.ToString()))
                    {
                        nodeIdList.Add(nodeId);
                    }
                }
            }
            else if (ChooseScope.SelectedValue == "2Hour")
            {
                var lastEditList = BaiRongDataProvider.ContentDao.GetNodeIdListCheckedByLastEditDateHour(tableName, PublishmentSystemId, 2);
                foreach (var nodeId in lastEditList)
                {
                    if (selectedNodeIdArrayList.Contains(nodeId.ToString()))
                    {
                        nodeIdList.Add(nodeId);
                    }
                }
            }
            else
            {
                nodeIdList = TranslateUtils.StringCollectionToIntList(TranslateUtils.ObjectCollectionToString(selectedNodeIdArrayList));
            }

            if (nodeIdList.Count == 0)
            {
                FailMessage("请首先选中希望生成页面的栏目！");
                return;
            }

            foreach (var nodeId in nodeIdList)
            {
                CreateManager.CreateChannel(PublishmentSystemId, nodeId);
            }

            PageCreateStatus.Redirect(PublishmentSystemId);
        }

        public void DeleteAllNodeButton_OnClick(object sender, EventArgs e)
        {
            if (!Page.IsPostBack || !Page.IsValid) return;

            var url = PageProgressBar.GetDeleteAllPageUrl(PublishmentSystemId, ETemplateType.ChannelTemplate);
            PageUtils.RedirectToLoadingPage(url);
        }
    }
}
