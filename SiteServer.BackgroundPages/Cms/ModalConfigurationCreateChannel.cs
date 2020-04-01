using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using SiteServer.Utils;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.Utils.Enumerations;

namespace SiteServer.BackgroundPages.Cms
{
    public class ModalConfigurationCreateChannel : BasePageCms
    {
        public DropDownList DdlIsCreateChannelIfContentChanged;

        protected ListBox LbChannelId;

		private int _channelId;

        public static string GetOpenWindowString(int siteId, int channelId)
        {
            return LayerUtils.GetOpenScript("栏目生成设置",
                PageUtils.GetCmsUrl(siteId, nameof(ModalConfigurationCreateChannel), new NameValueCollection
                {
                    {"channelId", channelId.ToString()}
                }), 550, 500);
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("siteId", "channelId");
            _channelId = AuthRequest.GetQueryInt("channelId");

			if (!IsPostBack)
			{
                var nodeInfo = ChannelManager.GetChannelInfo(SiteId, _channelId);

                EBooleanUtils.AddListItems(DdlIsCreateChannelIfContentChanged, "生成", "不生成");
                ControlUtils.SelectSingleItemIgnoreCase(DdlIsCreateChannelIfContentChanged, nodeInfo.Additional.IsCreateChannelIfContentChanged.ToString());

                //NodeManager.AddListItemsForAddContent(this.channelIdCollection.Items, base.SiteInfo, false);
                ChannelManager.AddListItemsForCreateChannel(LbChannelId.Items, SiteInfo, false, AuthRequest.AdminPermissionsImpl);
                ControlUtils.SelectMultiItems(LbChannelId, TranslateUtils.StringCollectionToStringList(nodeInfo.Additional.CreateChannelIdsIfContentChanged));
			}
		}

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            var isSuccess = false;

            try
            {
                var nodeInfo = ChannelManager.GetChannelInfo(SiteId, _channelId);

                nodeInfo.Additional.IsCreateChannelIfContentChanged = TranslateUtils.ToBool(DdlIsCreateChannelIfContentChanged.SelectedValue);
                nodeInfo.Additional.CreateChannelIdsIfContentChanged = ControlUtils.GetSelectedListControlValueCollection(LbChannelId);

                DataProvider.ChannelDao.Update(nodeInfo);

                AuthRequest.AddSiteLog(SiteId, _channelId, 0, "设置栏目变动生成页面", $"栏目:{nodeInfo.ChannelName}");
                isSuccess = true;
            }
            catch (Exception ex)
            {
                FailMessage(ex, ex.Message);
            }

            if (isSuccess)
            {
                LayerUtils.CloseAndRedirect(Page, PageConfigurationCreateTrigger.GetRedirectUrl(SiteId, _channelId));
            }
        }
	}
}
