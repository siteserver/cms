using System;
using System.Collections;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;
using SiteServer.BackgroundPages.Core;
using SiteServer.CMS.Core;
using SiteServer.CMS.Core.Security;
using SiteServer.CMS.Model;
using SiteServer.CMS.Model.Enumerations;

namespace SiteServer.BackgroundPages.Cms
{
	public class ModalChannelMultipleSelect : BasePageCms
    {
        public PlaceHolder PhPublishmentSystemId;
        public DropDownList DdlPublishmentSystemId;
        public Literal LtlChannelName;
        public Repeater RptChannel;

        private int _targetPublishmentSystemId;
        private bool _isPublishmentSystemSelect;
        private string _jsMethod;

        public static string GetOpenWindowString(int publishmentSystemId, bool isPublishmentSystemSelect,
            string jsMethod)
        {
            return PageUtils.GetOpenWindowString("选择目标栏目",
                PageUtils.GetCmsUrl(nameof(ModalChannelMultipleSelect), new NameValueCollection
                {
                    {"publishmentSystemID", publishmentSystemId.ToString()},
                    {"isPublishmentSystemSelect", isPublishmentSystemSelect.ToString()},
                    {"jsMethod", jsMethod}
                }), 600, 580, true);
        }

        public static string GetOpenWindowString(int publishmentSystemId, bool isPublishmentSystemSelect)
        {
            return GetOpenWindowString(publishmentSystemId, isPublishmentSystemSelect, "translateNodeAdd");
        }

        public string GetRedirectUrl(string targetPublishmentSystemId, string targetNodeId)
        {
            return PageUtils.GetCmsUrl(nameof(ModalChannelMultipleSelect), new NameValueCollection
            {
                {"publishmentSystemID", PublishmentSystemId.ToString()},
                {"isPublishmentSystemSelect", _isPublishmentSystemSelect.ToString()},
                {"jsMethod", _jsMethod},
                {"targetPublishmentSystemID", targetPublishmentSystemId},
                {"targetNodeID", targetNodeId}
            });
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            _isPublishmentSystemSelect = Body.GetQueryBool("isPublishmentSystemSelect");
            _jsMethod = Body.GetQueryString("jsMethod");

            _targetPublishmentSystemId = Body.GetQueryInt("TargetPublishmentSystemID");
            if (_targetPublishmentSystemId == 0)
            {
                _targetPublishmentSystemId = PublishmentSystemId;
            }

            if (!IsPostBack)
            {
                PhPublishmentSystemId.Visible = _isPublishmentSystemSelect;

                var publishmentSystemIdList = ProductPermissionsManager.Current.PublishmentSystemIdList;

                var mySystemInfoArrayList = new ArrayList();
                var parentWithChildren = new Hashtable();
                foreach (var publishmentSystemId in publishmentSystemIdList)
                {
                    var publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(publishmentSystemId);
                    if (publishmentSystemInfo.ParentPublishmentSystemId == 0)
                    {
                        mySystemInfoArrayList.Add(publishmentSystemInfo);
                    }
                    else
                    {
                        var children = new ArrayList();
                        if (parentWithChildren.Contains(publishmentSystemInfo.ParentPublishmentSystemId))
                        {
                            children = (ArrayList)parentWithChildren[publishmentSystemInfo.ParentPublishmentSystemId];
                        }
                        children.Add(publishmentSystemInfo);
                        parentWithChildren[publishmentSystemInfo.ParentPublishmentSystemId] = children;
                    }
                }
                foreach (PublishmentSystemInfo publishmentSystemInfo in mySystemInfoArrayList)
                {
                    AddSite(DdlPublishmentSystemId, publishmentSystemInfo, parentWithChildren, 0);
                }
                ControlUtils.SelectListItems(DdlPublishmentSystemId, _targetPublishmentSystemId.ToString());

                var targetNodeId = Body.GetQueryInt("TargetNodeID");
                if (targetNodeId > 0)
                {
                    var siteName = PublishmentSystemManager.GetPublishmentSystemInfo(_targetPublishmentSystemId).PublishmentSystemName;
                    var nodeNames = NodeManager.GetNodeNameNavigation(_targetPublishmentSystemId, targetNodeId);
                    if (_targetPublishmentSystemId != PublishmentSystemId)
                    {
                        nodeNames = siteName + "：" + nodeNames;
                    }
                    string value = $"{_targetPublishmentSystemId}_{targetNodeId}";
                    if (!_isPublishmentSystemSelect)
                    {
                        value = targetNodeId.ToString();
                    }
                    string scripts = $"window.parent.{_jsMethod}('{nodeNames}', '{value}');";
                    PageUtils.CloseModalPageWithoutRefresh(Page, scripts);
                }
                else
                {
                    var nodeInfo = NodeManager.GetNodeInfo(_targetPublishmentSystemId, _targetPublishmentSystemId);
                    var linkUrl = GetRedirectUrl(_targetPublishmentSystemId.ToString(), _targetPublishmentSystemId.ToString());
                    LtlChannelName.Text = $"<a href='{linkUrl}'>{nodeInfo.NodeName}</a>";

                    var additional = new NameValueCollection
                    {
                        ["linkUrl"] = GetRedirectUrl(_targetPublishmentSystemId.ToString(), string.Empty)
                    };
                    ClientScriptRegisterClientScriptBlock("NodeTreeScript", ChannelLoading.GetScript(PublishmentSystemManager.GetPublishmentSystemInfo(_targetPublishmentSystemId), ELoadingType.ChannelSelect, additional));

                    RptChannel.DataSource = DataProvider.NodeDao.GetNodeIdListByParentId(_targetPublishmentSystemId, _targetPublishmentSystemId);
                    RptChannel.ItemDataBound += RptChannel_ItemDataBound;
                    RptChannel.DataBind();
                }
            }
        }

        private void RptChannel_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            var nodeId = (int)e.Item.DataItem;
            var enabled = IsOwningNodeId(nodeId);
            if (!enabled)
            {
                if (!IsHasChildOwningNodeId(nodeId)) e.Item.Visible = false;
            }
            var nodeInfo = NodeManager.GetNodeInfo(_targetPublishmentSystemId, nodeId);

            var ltlHtml = (Literal)e.Item.FindControl("ltlHtml");

            var additional = new NameValueCollection
            {
                ["linkUrl"] = GetRedirectUrl(_targetPublishmentSystemId.ToString(), string.Empty)
            };

            ltlHtml.Text = ChannelLoading.GetChannelRowHtml(PublishmentSystemInfo, nodeInfo, enabled, ELoadingType.ChannelSelect, additional, Body.AdministratorName);
        }

        public void DdlPublishmentSystemId_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            var redirectUrl = GetRedirectUrl(DdlPublishmentSystemId.SelectedValue, string.Empty);
            PageUtils.Redirect(redirectUrl);
        }

        private void AddSite(ListControl listControl, PublishmentSystemInfo publishmentSystemInfo, Hashtable parentWithChildren, int level)
        {
            var padding = string.Empty;
            for (var i = 0; i < level; i++)
            {
                padding += "　";
            }
            if (level > 0)
            {
                padding += "└ ";
            }

            if (parentWithChildren[publishmentSystemInfo.PublishmentSystemId] != null)
            {
                var children = (ArrayList)parentWithChildren[publishmentSystemInfo.PublishmentSystemId];

                var listitem = new ListItem(padding + publishmentSystemInfo.PublishmentSystemName +
                                                 $"({children.Count})", publishmentSystemInfo.PublishmentSystemId.ToString());
                if (publishmentSystemInfo.PublishmentSystemId == PublishmentSystemId) listitem.Selected = true;

                listControl.Items.Add(listitem);
                level++;
                foreach (PublishmentSystemInfo subSiteInfo in children)
                {
                    AddSite(listControl, subSiteInfo, parentWithChildren, level);
                }
            }
            else
            {
                var listitem = new ListItem(padding + publishmentSystemInfo.PublishmentSystemName, publishmentSystemInfo.PublishmentSystemId.ToString());
                if (publishmentSystemInfo.PublishmentSystemId == PublishmentSystemId) listitem.Selected = true;

                listControl.Items.Add(listitem);
            }
        }
	}
}
