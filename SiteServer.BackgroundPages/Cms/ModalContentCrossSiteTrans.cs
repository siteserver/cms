using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using SiteServer.Utils;
using SiteServer.CMS.Core;

namespace SiteServer.BackgroundPages.Cms
{
	public class ModalContentCrossSiteTrans : BasePageCms
    {
	    protected DropDownList DdlSiteId;
        protected ListBox LbNodeId;

        private int _nodeId;
	    private List<int> _contentIdArrayList;

        public static string GetOpenWindowString(int siteId, int nodeId)
        {
            return LayerUtils.GetOpenScriptWithCheckBoxValue("跨站转发", PageUtils.GetCmsUrl(siteId, nameof(ModalContentCrossSiteTrans), new NameValueCollection
            {
                {"NodeID", nodeId.ToString()}
            }), "ContentIDCollection", "请选择需要转发的内容！", 400, 410);
        }

		public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("siteId", "NodeID", "ContentIDCollection");

            _nodeId = Body.GetQueryInt("NodeID");
            _contentIdArrayList = TranslateUtils.StringCollectionToIntList(Body.GetQueryString("ContentIDCollection"));

			if (!IsPostBack)
			{
                CrossSiteTransUtility.LoadSiteIdDropDownList(DdlSiteId, SiteInfo, _nodeId);

                if (DdlSiteId.Items.Count > 0)
                {
                    DdlSiteId_SelectedIndexChanged(null, EventArgs.Empty);
                }
			}
		}

        public void DdlSiteId_SelectedIndexChanged(object sender, EventArgs e)
        {
            var psId = int.Parse(DdlSiteId.SelectedValue);
            CrossSiteTransUtility.LoadNodeIdListBox(LbNodeId, SiteInfo, psId, ChannelManager.GetChannelInfo(SiteId, _nodeId), Body.AdminName);
        }

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            var targetSiteId = int.Parse(DdlSiteId.SelectedValue);
            var targetSiteInfo = SiteManager.GetSiteInfo(targetSiteId);
            try
            {
                foreach (ListItem listItem in LbNodeId.Items)
                {
                    if (listItem.Selected)
                    {
                        var targetNodeId = TranslateUtils.ToInt(listItem.Value);
                        if (targetNodeId != 0)
                        {
                            var targetTableName = ChannelManager.GetTableName(targetSiteInfo, targetNodeId);
                            var tableName = ChannelManager.GetTableName(SiteInfo, _nodeId);
                            foreach (var contentId in _contentIdArrayList)
                            {
                                var contentInfo = DataProvider.ContentDao.GetContentInfo(tableName, contentId);
                                FileUtility.MoveFileByContentInfo(SiteInfo, targetSiteInfo, contentInfo);
                                contentInfo.SiteId = targetSiteId;
                                contentInfo.SourceId = contentInfo.ChannelId;
                                contentInfo.ChannelId = targetNodeId;
                                
                                contentInfo.IsChecked = targetSiteInfo.Additional.IsCrossSiteTransChecked;
                                contentInfo.CheckedLevel = 0;

                                DataProvider.ContentDao.Insert(targetTableName, targetSiteInfo, contentInfo);
                            }
                        }
                    }
                }

                Body.AddSiteLog(SiteId, _nodeId, 0, "跨站转发", string.Empty);

                SuccessMessage("内容转发成功，请选择后续操作。");
            }
            catch (Exception ex)
            {
                FailMessage(ex, "内容转发失败！");
            }

            LayerUtils.Close(Page);
		}

	}
}
