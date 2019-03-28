using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using SiteServer.CMS.Caches;
using SiteServer.Utils;
using SiteServer.CMS.Database.Core;
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
                var channelInfo = ChannelManager.GetChannelInfo(SiteId, _channelId);

                EBooleanUtils.AddListItems(DdlIsCreateChannelIfContentChanged, "生成", "不生成");
                ControlUtils.SelectSingleItemIgnoreCase(DdlIsCreateChannelIfContentChanged, channelInfo.IsCreateChannelIfContentChanged.ToString());

                //NodeManager.AddListItemsForAddContent(this.channelIdCollection.Items, base.SiteInfo, false);
                ChannelManager.AddListItemsForCreateChannel(LbChannelId.Items, SiteInfo, false, AuthRequest.AdminPermissionsImpl);
                ControlUtils.SelectMultiItems(LbChannelId, TranslateUtils.StringCollectionToStringList(channelInfo.CreateChannelIdsIfContentChanged));
			}
		}

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            var isSuccess = false;

            try
            {
                var channelInfo = ChannelManager.GetChannelInfo(SiteId, _channelId);

                channelInfo.IsCreateChannelIfContentChanged = TranslateUtils.ToBool(DdlIsCreateChannelIfContentChanged.SelectedValue);
                channelInfo.CreateChannelIdsIfContentChanged = ControlUtils.GetSelectedListControlValueCollection(LbChannelId);

                DataProvider.Channel.Update(channelInfo);

                AuthRequest.AddSiteLog(SiteId, _channelId, 0, "设置栏目变动生成页面", $"栏目:{channelInfo.ChannelName}");
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
