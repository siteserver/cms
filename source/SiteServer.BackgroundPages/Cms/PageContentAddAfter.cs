using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;
using SiteServer.BackgroundPages.Core;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;
using SiteServer.CMS.Model.Enumerations;

namespace SiteServer.BackgroundPages.Cms
{
	public class PageContentAddAfter : BasePageCms
    {
        public RadioButtonList Operation;
        public DropDownList PublishmentSystemIDDropDownList;
        public ListBox NodeIDListBox;
        public PlaceHolder phPublishmentSystemID;
        public PlaceHolder phSubmit;

        private NodeInfo _nodeInfo;
        private int _contentId;
        private string _returnUrl;

        public enum EContentAddAfter
        {
            ContinueAdd,
            ManageContents,
            Contribute
        }

        public static string GetRedirectUrl(int publishmentSystemId, int nodeId, int contentId, string returnUrl)
        {
            return PageUtils.GetCmsUrl(nameof(PageContentAddAfter), new NameValueCollection
            {
                {"PublishmentSystemID", publishmentSystemId.ToString()},
                {"NodeID", nodeId.ToString()},
                {"ContentID", contentId.ToString()},
                {"ReturnUrl", StringUtils.ValueToUrl(returnUrl)}
            });
        }

		public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

			PageUtils.CheckRequestParameter("PublishmentSystemID", "NodeID", "ContentID", "ReturnUrl");
			var nodeId = Body.GetQueryInt("NodeID");
            _contentId = Body.GetQueryInt("ContentID");
            _returnUrl = StringUtils.ValueFromUrl(Body.GetQueryString("ReturnUrl"));

            _nodeInfo = NodeManager.GetNodeInfo(PublishmentSystemId, nodeId);

			if (!IsPostBack)
			{
                BreadCrumb(AppManager.Cms.LeftMenu.IdContent, "内容管理", string.Empty);

                Operation.Items.Add(new ListItem("继续添加内容", EContentAddAfter.ContinueAdd.ToString()));
                Operation.Items.Add(new ListItem("返回管理界面", EContentAddAfter.ManageContents.ToString()));
                var isContribute = CrossSiteTransUtility.IsTranslatable(PublishmentSystemInfo, _nodeInfo);
                var isTransOk = false;
                if (isContribute)
                {
                    var isAutomatic = CrossSiteTransUtility.IsAutomatic(_nodeInfo);
                    if (isAutomatic)
                    {
                        var targetPublishmentSystemId = 0;

                        if (_nodeInfo.Additional.TransType == ECrossSiteTransType.SpecifiedSite)
                        {
                            targetPublishmentSystemId = _nodeInfo.Additional.TransPublishmentSystemID;
                        }
                        else if (_nodeInfo.Additional.TransType == ECrossSiteTransType.SelfSite)
                        {
                            targetPublishmentSystemId = PublishmentSystemId;
                        }
                        else if (_nodeInfo.Additional.TransType == ECrossSiteTransType.ParentSite)
                        {
                            targetPublishmentSystemId = PublishmentSystemManager.GetParentPublishmentSystemId(PublishmentSystemId);
                        }

                        if (targetPublishmentSystemId > 0)
                        {
                            var targetPublishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(targetPublishmentSystemId);
                            if (targetPublishmentSystemInfo != null)
                            {
                                var targetNodeIdArrayList = TranslateUtils.StringCollectionToIntList(_nodeInfo.Additional.TransNodeIDs);
                                if (targetNodeIdArrayList.Count > 0)
                                {
                                    foreach (int targetNodeId in targetNodeIdArrayList)
                                    {
                                        CrossSiteTransUtility.TransContentInfo(PublishmentSystemInfo, _nodeInfo, _contentId, targetPublishmentSystemInfo, targetNodeId);
                                        isTransOk = true;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        Operation.Items.Add(new ListItem("转发到其他站点", EContentAddAfter.Contribute.ToString()));
                    }
                }
			    SuccessMessage(isTransOk ? "内容添加成功并已转发到指定站点，请选择后续操作。" : "内容添加成功，请选择后续操作。");

			    phPublishmentSystemID.Visible = phSubmit.Visible = false;
			}

        }

        public void Operation_SelectedIndexChanged(object sender, EventArgs e)
		{
            var after = (EContentAddAfter)TranslateUtils.ToEnum(typeof(EContentAddAfter), Operation.SelectedValue, EContentAddAfter.ContinueAdd);
            if (after == EContentAddAfter.ContinueAdd)
            {
                PageUtils.Redirect(WebUtils.GetContentAddAddUrl(PublishmentSystemId, _nodeInfo, Body.GetQueryString("ReturnUrl")));
            }
            else if (after == EContentAddAfter.ManageContents)
            {
                PageUtils.Redirect(_returnUrl);
            }
            else if (after == EContentAddAfter.Contribute)
            {
                CrossSiteTransUtility.LoadPublishmentSystemIDDropDownList(PublishmentSystemIDDropDownList, PublishmentSystemInfo, _nodeInfo.NodeId);

                if (PublishmentSystemIDDropDownList.Items.Count > 0)
                {
                    PublishmentSystemID_SelectedIndexChanged(sender, e);
                }
                phPublishmentSystemID.Visible = phSubmit.Visible = true;
            }
		}

        public void PublishmentSystemID_SelectedIndexChanged(object sender, EventArgs e)
        {
            var psId = int.Parse(PublishmentSystemIDDropDownList.SelectedValue);
            CrossSiteTransUtility.LoadNodeIDListBox(NodeIDListBox, PublishmentSystemInfo, psId, _nodeInfo, Body.AdministratorName);
        }

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            if (!Page.IsPostBack || !Page.IsValid) return;

            var targetPublishmentSystemId = int.Parse(PublishmentSystemIDDropDownList.SelectedValue);
            var targetPublishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(targetPublishmentSystemId);
            try
            {
                foreach (ListItem listItem in NodeIDListBox.Items)
                {
                    if (!listItem.Selected) continue;
                    var targetNodeId = TranslateUtils.ToInt(listItem.Value);
                    if (targetNodeId != 0)
                    {
                        CrossSiteTransUtility.TransContentInfo(PublishmentSystemInfo, _nodeInfo, _contentId, targetPublishmentSystemInfo, targetNodeId);
                    }
                }

                Body.AddSiteLog(PublishmentSystemId, _nodeInfo.NodeId, _contentId, "内容跨站转发", $"转发到站点:{targetPublishmentSystemInfo.PublishmentSystemName}");

                SuccessMessage("内容跨站转发成功，请选择后续操作。");
                Operation.Items.Clear();
                Operation.Items.Add(new ListItem("继续添加内容", EContentAddAfter.ContinueAdd.ToString()));
                Operation.Items.Add(new ListItem("返回管理界面", EContentAddAfter.ManageContents.ToString()));
                Operation.Items.Add(new ListItem("转发到其他站点", EContentAddAfter.Contribute.ToString()));
                phPublishmentSystemID.Visible = phSubmit.Visible = false;
            }
            catch (Exception ex)
            {
                FailMessage(ex, "内容跨站转发失败！");
            }
        }
	}
}
