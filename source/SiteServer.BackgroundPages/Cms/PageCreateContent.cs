using System;
using System.Collections;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using BaiRong.Core;
using SiteServer.BackgroundPages.Service;
using SiteServer.CMS.Core;
using SiteServer.CMS.Core.Create;
using SiteServer.CMS.Model.Enumerations;

namespace SiteServer.BackgroundPages.Cms
{
    public class PageCreateContent : BasePageCms
    {
        public ListBox NodeIDList;
        public DropDownList ChooseScope;
        public Button DeleteAllContentButton;

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("PublishmentSystemID");

            if (!IsPostBack)
            {
                BreadCrumb(AppManager.Cms.LeftMenu.IdCreate, "生成内容页", AppManager.Cms.Permission.WebSite.Create);

                var listitem = new ListItem("所有选中的栏目", "All");
                ChooseScope.Items.Add(listitem);
                listitem = new ListItem("一个月内更新的内容", "Month");
                ChooseScope.Items.Add(listitem);
                listitem = new ListItem("一天内更新的内容", "Day");
                ChooseScope.Items.Add(listitem);
                listitem = new ListItem("2小时内更新的内容", "2Hour");
                ChooseScope.Items.Add(listitem);

                NodeManager.AddListItems(NodeIDList.Items, PublishmentSystemInfo, false, true, Body.AdministratorName);
                DeleteAllContentButton.Attributes.Add("onclick", "return confirm(\"此操作将删除所有已生成的内容页面，确定吗？\");");
            }
        }


        public void CreateContentButton_OnClick(object sender, EventArgs e)
        {
            if (Page.IsPostBack && Page.IsValid)
            {
                var nodeIdArrayList = new List<int>();
                var selectedNodeIdArrayList = ControlUtils.GetSelectedListControlValueArrayList(NodeIDList);

                var tableName = PublishmentSystemInfo.AuxiliaryTableForContent;

                if (ChooseScope.SelectedValue == "Month")
                {
                    var lastEditList = BaiRongDataProvider.ContentDao.GetNodeIdListCheckedByLastEditDateHour(tableName, PublishmentSystemId, 720);
                    foreach (var nodeId in lastEditList)
                    {
                        if (selectedNodeIdArrayList.Contains(nodeId.ToString()))
                        {
                            nodeIdArrayList.Add(nodeId);
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
                            nodeIdArrayList.Add(nodeId);
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
                            nodeIdArrayList.Add(nodeId);
                        }
                    }
                }
                else
                {
                    nodeIdArrayList = TranslateUtils.StringCollectionToIntList(TranslateUtils.ObjectCollectionToString(selectedNodeIdArrayList));
                }
                ProcessCreateContent(nodeIdArrayList);
            }
        }

        public void DeleteAllContentButton_OnClick(object sender, EventArgs e)
        {
            if (Page.IsPostBack && Page.IsValid)
            {
                var url = PageProgressBar.GetDeleteAllPageUrl(PublishmentSystemId, ETemplateType.ContentTemplate);
                PageUtils.RedirectToLoadingPage(url);
            }
        }

        private void ProcessCreateContent(ICollection nodeIdArrayList)
        {
            if (nodeIdArrayList.Count == 0)
            {
                FailMessage("请首先选中希望生成内容页面的栏目！");
                return;
            }

            foreach (int nodeId in nodeIdArrayList)
            {
                CreateManager.CreateAllContent(PublishmentSystemId, nodeId);
            }

            PageCreateStatus.Redirect(PublishmentSystemId);
        }
    }
}
