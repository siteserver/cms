using System;
using System.Collections.Specialized;
using System.Web.UI;
using System.Web.UI.WebControls;
using BaiRong.Core;
using SiteServer.BackgroundPages.Cms;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;
using SiteServer.CMS.Model.Enumerations;

namespace SiteServer.BackgroundPages.Wcm
{
    public class PageGovPublicContentAddAfter : BasePageGovPublic
    {
        public RadioButtonList Operation;
        public DropDownList PublishmentSystemIDDropDownList;
        public ListBox NodeIDListBox;
        public Control PublishmentSystemIDRow;
        public Control NodeIDDropDownListRow;
        public Button Submit;

        private NodeInfo nodeInfo;
        private int contentID;
        private string returnUrl;

        public enum EContentAddAfter
        {
            ContinueAdd,
            ManageContents,
            Contribute
        }

        public static string GetRedirectUrl(int publishmentSystemId, int nodeId, int contentId, string returnUrl)
        {
            return PageUtils.GetWcmUrl(nameof(PageGovPublicContentAddAfter), new NameValueCollection
            {
                {"PublishmentSystemID", publishmentSystemId.ToString()},
                {"NodeID", nodeId.ToString()},
                {"ContentID", contentId.ToString()},
                {"ReturnUrl", StringUtils.ValueToUrl(returnUrl)}
            });
        }

        public void Page_Load(object sender, EventArgs e)
		{
			PageUtils.CheckRequestParameter("PublishmentSystemID", "NodeID", "ContentID", "ReturnUrl");
			var nodeID = int.Parse(Request.QueryString["NodeID"]);
            contentID = int.Parse(Request.QueryString["ContentID"]);
            returnUrl = StringUtils.ValueFromUrl(Request.QueryString["ReturnUrl"]);
            if (string.IsNullOrEmpty(returnUrl))
            {
                returnUrl = PageContent.GetRedirectUrl(PublishmentSystemId, nodeID);
            }

            nodeInfo = NodeManager.GetNodeInfo(PublishmentSystemId, nodeID);

			if (!IsPostBack)
            {
                BreadCrumb(AppManager.Wcm.LeftMenu.IdGovPublic, AppManager.Wcm.LeftMenu.GovPublic.IdGovPublicContent, "后续操作", AppManager.Wcm.Permission.WebSite.GovPublicContent);

                Operation.Items.Add(new ListItem("继续添加内容", EContentAddAfter.ContinueAdd.ToString()));
                Operation.Items.Add(new ListItem("返回管理界面", EContentAddAfter.ManageContents.ToString()));
                var isContribute = CrossSiteTransUtility.IsTranslatable(PublishmentSystemInfo, nodeInfo);
                var isTransOk = false;
                if (isContribute)
                {
                    var isAutomatic = CrossSiteTransUtility.IsAutomatic(nodeInfo);
                    if (isAutomatic)
                    {
                        var targetPublishmentSystemID = 0;

                        if (nodeInfo.Additional.TransType == ECrossSiteTransType.SpecifiedSite)
                        {
                            targetPublishmentSystemID = nodeInfo.Additional.TransPublishmentSystemID;
                        }
                        else if (nodeInfo.Additional.TransType == ECrossSiteTransType.SelfSite)
                        {
                            targetPublishmentSystemID = PublishmentSystemId;
                        }
                        else if (nodeInfo.Additional.TransType == ECrossSiteTransType.ParentSite)
                        {
                            targetPublishmentSystemID = PublishmentSystemManager.GetParentPublishmentSystemId(PublishmentSystemId);
                        }

                        if (targetPublishmentSystemID > 0)
                        {
                            var targetPublishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(targetPublishmentSystemID);
                            if (targetPublishmentSystemInfo != null)
                            {
                                var targetNodeIDArrayList = TranslateUtils.StringCollectionToIntList(nodeInfo.Additional.TransNodeIDs);
                                if (targetNodeIDArrayList.Count > 0)
                                {
                                    foreach (int targetNodeID in targetNodeIDArrayList)
                                    {
                                        CrossSiteTransUtility.TransContentInfo(PublishmentSystemInfo, nodeInfo, contentID, targetPublishmentSystemInfo, targetNodeID);
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
                if (isTransOk)
                {
                    SuccessMessage("内容添加成功并已转发到指定站点，请选择后续操作。");
                }
                else
                {
                    SuccessMessage("内容添加成功，请选择后续操作。");
                }
                
                PublishmentSystemIDRow.Visible = NodeIDDropDownListRow.Visible = Submit.Visible = false;
			}
		}

        public void Operation_SelectedIndexChanged(object sender, EventArgs e)
		{
            var after = (EContentAddAfter)TranslateUtils.ToEnum(typeof(EContentAddAfter), Operation.SelectedValue, EContentAddAfter.ContinueAdd);
            if (after == EContentAddAfter.ContinueAdd)
            {
                PageUtils.Redirect(PageGovPublicContentAdd.GetRedirectUrlOfAdd(PublishmentSystemId, nodeInfo.NodeId, Request.QueryString["ReturnUrl"]));
            }
            else if (after == EContentAddAfter.ManageContents)
            {
                PageUtils.Redirect(returnUrl);
            }
            else if (after == EContentAddAfter.Contribute)
            {
                CrossSiteTransUtility.LoadPublishmentSystemIDDropDownList(PublishmentSystemIDDropDownList, PublishmentSystemInfo, nodeInfo.NodeId);

                if (PublishmentSystemIDDropDownList.Items.Count > 0)
                {
                    PublishmentSystemID_SelectedIndexChanged(sender, e);
                }
                PublishmentSystemIDRow.Visible = NodeIDDropDownListRow.Visible = Submit.Visible = true;
            }
		}

        public void PublishmentSystemID_SelectedIndexChanged(object sender, EventArgs e)
        {
            var psID = int.Parse(PublishmentSystemIDDropDownList.SelectedValue);
            CrossSiteTransUtility.LoadNodeIDListBox(NodeIDListBox, PublishmentSystemInfo, psID, nodeInfo, Body.AdministratorName);
        }

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            if (Page.IsPostBack && Page.IsValid)
            {
                var targetPublishmentSystemID = int.Parse(PublishmentSystemIDDropDownList.SelectedValue);
                var targetPublishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(targetPublishmentSystemID);
                try
                {
                    foreach (ListItem listItem in NodeIDListBox.Items)
                    {
                        if (listItem.Selected)
                        {
                            var targetNodeID = TranslateUtils.ToInt(listItem.Value);
                            if (targetNodeID != 0)
                            {
                                CrossSiteTransUtility.TransContentInfo(PublishmentSystemInfo, nodeInfo, contentID, targetPublishmentSystemInfo, targetNodeID);
                            }
                        }
                    }

                    Body.AddSiteLog(PublishmentSystemId, nodeInfo.NodeId, contentID, "内容跨站转发",
                        $"转发到站点:{targetPublishmentSystemInfo.PublishmentSystemName}");

                    SuccessMessage("内容跨站转发成功，请选择后续操作。");
                    Operation.Items.Clear();
                    Operation.Items.Add(new ListItem("继续添加内容", EContentAddAfter.ContinueAdd.ToString()));
                    Operation.Items.Add(new ListItem("返回管理界面", EContentAddAfter.ManageContents.ToString()));
                    Operation.Items.Add(new ListItem("转发到其他站点", EContentAddAfter.Contribute.ToString()));
                    PublishmentSystemIDRow.Visible = NodeIDDropDownListRow.Visible = Submit.Visible = false;
                }
                catch (Exception ex)
                {
                    FailMessage(ex, "内容跨站转发失败！");
                }
            }
        }
	}
}
