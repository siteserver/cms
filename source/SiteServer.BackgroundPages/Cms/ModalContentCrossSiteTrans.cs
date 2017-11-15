using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;

namespace SiteServer.BackgroundPages.Cms
{
	public class ModalContentCrossSiteTrans : BasePageCms
    {
	    protected DropDownList DdlPublishmentSystemId;
        protected ListBox LbNodeId;

        private int _nodeId;
	    private List<int> _contentIdArrayList;

        public static string GetOpenWindowString(int publishmentSystemId, int nodeId)
        {
            return PageUtils.GetOpenLayerStringWithCheckBoxValue("转发所选内容", PageUtils.GetCmsUrl(nameof(ModalContentCrossSiteTrans), new NameValueCollection
            {
                {"PublishmentSystemID", publishmentSystemId.ToString()},
                {"NodeID", nodeId.ToString()}
            }), "ContentIDCollection", "请选择需要转发的内容！", 400, 410);
        }

		public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("PublishmentSystemID", "NodeID", "ContentIDCollection");

            _nodeId = Body.GetQueryInt("NodeID");
            _contentIdArrayList = TranslateUtils.StringCollectionToIntList(Body.GetQueryString("ContentIDCollection"));

			if (!IsPostBack)
			{
                CrossSiteTransUtility.LoadPublishmentSystemIdDropDownList(DdlPublishmentSystemId, PublishmentSystemInfo, _nodeId);

                if (DdlPublishmentSystemId.Items.Count > 0)
                {
                    DdlPublishmentSystemId_SelectedIndexChanged(null, EventArgs.Empty);
                }
			}
		}

        public void DdlPublishmentSystemId_SelectedIndexChanged(object sender, EventArgs e)
        {
            var psId = int.Parse(DdlPublishmentSystemId.SelectedValue);
            CrossSiteTransUtility.LoadNodeIdListBox(LbNodeId, PublishmentSystemInfo, psId, NodeManager.GetNodeInfo(PublishmentSystemId, _nodeId), Body.AdminName);
        }

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            var targetPublishmentSystemId = int.Parse(DdlPublishmentSystemId.SelectedValue);
            var targetPublishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(targetPublishmentSystemId);
            try
            {
                foreach (ListItem listItem in LbNodeId.Items)
                {
                    if (listItem.Selected)
                    {
                        var targetNodeId = TranslateUtils.ToInt(listItem.Value);
                        if (targetNodeId != 0)
                        {
                            var targetTableName = NodeManager.GetTableName(targetPublishmentSystemInfo, targetNodeId);
                            var tableStyle = NodeManager.GetTableStyle(PublishmentSystemInfo, _nodeId);
                            var tableName = NodeManager.GetTableName(PublishmentSystemInfo, _nodeId);
                            foreach (var contentId in _contentIdArrayList)
                            {
                                var contentInfo = DataProvider.ContentDao.GetContentInfo(tableStyle, tableName, contentId);
                                FileUtility.MoveFileByContentInfo(PublishmentSystemInfo, targetPublishmentSystemInfo, contentInfo as BackgroundContentInfo);
                                contentInfo.PublishmentSystemId = targetPublishmentSystemId;
                                contentInfo.SourceId = contentInfo.NodeId;
                                contentInfo.NodeId = targetNodeId;
                                
                                contentInfo.IsChecked = targetPublishmentSystemInfo.Additional.IsCrossSiteTransChecked;
                                contentInfo.CheckedLevel = 0;

                                DataProvider.ContentDao.Insert(targetTableName, targetPublishmentSystemInfo, contentInfo);
                            }
                        }
                    }
                }

                Body.AddSiteLog(PublishmentSystemId, _nodeId, 0, "跨站转发内容", string.Empty);

                SuccessMessage("内容转发成功，请选择后续操作。");
            }
            catch (Exception ex)
            {
                FailMessage(ex, "内容转发失败！");
            }
            
            PageUtils.CloseModalPage(Page);
		}

	}
}
