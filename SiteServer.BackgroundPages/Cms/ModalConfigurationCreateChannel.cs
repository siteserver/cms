using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using SiteServer.CMS.Context;
using SiteServer.Abstractions;
using SiteServer.CMS.Context.Enumerations;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Repositories;

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
                var nodeInfo = ChannelManager.GetChannelAsync(SiteId, _channelId).GetAwaiter().GetResult();

                EBooleanUtils.AddListItems(DdlIsCreateChannelIfContentChanged, "生成", "不生成");
                ControlUtils.SelectSingleItemIgnoreCase(DdlIsCreateChannelIfContentChanged, nodeInfo.IsCreateChannelIfContentChanged.ToString());

                //NodeManager.AddListItemsForAddContent(this.channelIdCollection.Items, base.Site, false);
                ChannelManager.AddListItemsForCreateChannelAsync(LbChannelId.Items, Site, false, AuthRequest.AdminPermissionsImpl).GetAwaiter().GetResult();
                ControlUtils.SelectMultiItems(LbChannelId, StringUtils.GetStringList(nodeInfo.CreateChannelIdsIfContentChanged));
			}
		}

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            var isSuccess = false;

            try
            {
                var nodeInfo = ChannelManager.GetChannelAsync(SiteId, _channelId).GetAwaiter().GetResult();

                nodeInfo.IsCreateChannelIfContentChanged = TranslateUtils.ToBool(DdlIsCreateChannelIfContentChanged.SelectedValue);
                nodeInfo.CreateChannelIdsIfContentChanged = ControlUtils.GetSelectedListControlValueCollection(LbChannelId);

                DataProvider.ChannelRepository.UpdateAsync(nodeInfo).GetAwaiter().GetResult();

                AuthRequest.AddSiteLogAsync(SiteId, _channelId, 0, "设置栏目变动生成页面", $"栏目:{nodeInfo.ChannelName}").GetAwaiter().GetResult();
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
