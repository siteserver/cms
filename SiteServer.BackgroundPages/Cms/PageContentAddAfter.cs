using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;
using SiteServer.BackgroundPages.Core;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;

namespace SiteServer.BackgroundPages.Cms
{
	public class PageContentAddAfter : BasePageCms
    {
        public RadioButtonList RblOperation;
        public PlaceHolder PhPublishmentSystemId;
        public DropDownList DdlPublishmentSystemId;
        public ListBox LbNodeId;
        public PlaceHolder PhSubmit;

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

            if (IsPostBack) return;

            RblOperation.Items.Add(new ListItem("继续添加内容", EContentAddAfter.ContinueAdd.ToString()));
            RblOperation.Items.Add(new ListItem("返回管理界面", EContentAddAfter.ManageContents.ToString()));

            var isCrossSiteTrans = CrossSiteTransUtility.IsCrossSiteTrans(PublishmentSystemInfo, _nodeInfo);
            var isAutomatic = CrossSiteTransUtility.IsAutomatic(_nodeInfo);

            var isTranslated = ContentUtility.AfterContentAdded(PublishmentSystemInfo, _nodeInfo, _contentId, isCrossSiteTrans, isAutomatic);
            if (isCrossSiteTrans && !isAutomatic)
            {
                RblOperation.Items.Add(new ListItem("转发到其他站点", EContentAddAfter.Contribute.ToString()));
            }

            SuccessMessage(isTranslated ? "内容添加成功并已转发到指定站点，请选择后续操作。" : "内容添加成功，请选择后续操作。");

            PhPublishmentSystemId.Visible = PhSubmit.Visible = false;
        }

        public void RblOperation_SelectedIndexChanged(object sender, EventArgs e)
		{
            var after = (EContentAddAfter)TranslateUtils.ToEnum(typeof(EContentAddAfter), RblOperation.SelectedValue, EContentAddAfter.ContinueAdd);
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
                CrossSiteTransUtility.LoadPublishmentSystemIdDropDownList(DdlPublishmentSystemId, PublishmentSystemInfo, _nodeInfo.NodeId);

                if (DdlPublishmentSystemId.Items.Count > 0)
                {
                    DdlPublishmentSystemId_SelectedIndexChanged(sender, e);
                }
                PhPublishmentSystemId.Visible = PhSubmit.Visible = true;
            }
		}

        public void DdlPublishmentSystemId_SelectedIndexChanged(object sender, EventArgs e)
        {
            var psId = int.Parse(DdlPublishmentSystemId.SelectedValue);
            CrossSiteTransUtility.LoadNodeIdListBox(LbNodeId, PublishmentSystemInfo, psId, _nodeInfo, Body.AdminName);
        }

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            if (!Page.IsPostBack || !Page.IsValid) return;

            var targetPublishmentSystemId = int.Parse(DdlPublishmentSystemId.SelectedValue);
            var targetPublishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(targetPublishmentSystemId);
            try
            {
                foreach (ListItem listItem in LbNodeId.Items)
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
                RblOperation.Items.Clear();
                RblOperation.Items.Add(new ListItem("继续添加内容", EContentAddAfter.ContinueAdd.ToString()));
                RblOperation.Items.Add(new ListItem("返回管理界面", EContentAddAfter.ManageContents.ToString()));
                RblOperation.Items.Add(new ListItem("转发到其他站点", EContentAddAfter.Contribute.ToString()));
                PhPublishmentSystemId.Visible = PhSubmit.Visible = false;
            }
            catch (Exception ex)
            {
                FailMessage(ex, "内容跨站转发失败！");
            }
        }
	}
}
