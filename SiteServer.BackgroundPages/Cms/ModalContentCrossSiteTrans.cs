using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using Datory.Utils;
using SiteServer.Abstractions;
using SiteServer.CMS.Context;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Repositories;

namespace SiteServer.BackgroundPages.Cms
{
	public class ModalContentCrossSiteTrans : BasePageCms
    {
	    protected DropDownList DdlSiteId;
        protected ListBox LbChannelId;

        private int _channelId;
	    private List<int> _contentIdList;

        public static string GetOpenWindowString(int siteId, int channelId)
        {
            return LayerUtils.GetOpenScriptWithCheckBoxValue("跨站转发", PageUtils.GetCmsUrl(siteId, nameof(ModalContentCrossSiteTrans), new NameValueCollection
            {
                {"channelId", channelId.ToString()}
            }), "contentIdCollection", "请选择需要转发的内容！", 400, 410);
        }

		public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("siteId", "channelId", "contentIdCollection");

            _channelId = AuthRequest.GetQueryInt("channelId");
            _contentIdList = Utilities.GetIntList(AuthRequest.GetQueryString("contentIdCollection"));

            if (IsPostBack) return;

            CrossSiteTransUtility.LoadSiteIdDropDownListAsync(DdlSiteId, Site, _channelId).GetAwaiter().GetResult();

            if (DdlSiteId.Items.Count > 0)
            {
                DdlSiteId_SelectedIndexChanged(null, EventArgs.Empty);
            }
        }

        public void DdlSiteId_SelectedIndexChanged(object sender, EventArgs e)
        {
            var psId = int.Parse(DdlSiteId.SelectedValue);
            CrossSiteTransUtility.LoadChannelIdListBoxAsync(LbChannelId, Site, psId, DataProvider.ChannelRepository.GetAsync(_channelId).GetAwaiter().GetResult(), AuthRequest.AdminPermissionsImpl).GetAwaiter().GetResult();
        }

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            var targetSiteId = int.Parse(DdlSiteId.SelectedValue);
            var targetSite = DataProvider.SiteRepository.GetAsync(targetSiteId).GetAwaiter().GetResult();
            try
            {
                foreach (ListItem listItem in LbChannelId.Items)
                {
                    if (listItem.Selected)
                    {
                        var targetChannelId = TranslateUtils.ToInt(listItem.Value);
                        if (targetChannelId != 0)
                        {
                            var targetChannelInfo = DataProvider.ChannelRepository.GetAsync(targetChannelId).GetAwaiter().GetResult();
                            foreach (var contentId in _contentIdList)
                            {
                                var contentInfo = DataProvider.ContentRepository.GetAsync(Site, _channelId, contentId).GetAwaiter().GetResult();
                                FileUtility.MoveFileByContentAsync(Site, targetSite, contentInfo).GetAwaiter().GetResult();
                                contentInfo.SiteId = targetSiteId;
                                contentInfo.SourceId = contentInfo.ChannelId;
                                contentInfo.ChannelId = targetChannelId;
                                
                                contentInfo.Checked = targetSite.IsCrossSiteTransChecked;
                                contentInfo.CheckedLevel = 0;

                                DataProvider.ContentRepository.InsertAsync(targetSite, targetChannelInfo, contentInfo).GetAwaiter().GetResult();
                            }
                        }
                    }
                }

                AuthRequest.AddSiteLogAsync(SiteId, _channelId, 0, "跨站转发", string.Empty).GetAwaiter().GetResult();

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
