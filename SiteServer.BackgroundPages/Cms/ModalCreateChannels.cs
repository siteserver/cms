using System;
using System.Web.UI.WebControls;
using SiteServer.Utils;
using SiteServer.CMS.Core;
using SiteServer.CMS.Core.Create;
using SiteServer.CMS.DataCache;
using SiteServer.Utils.Enumerations;

namespace SiteServer.BackgroundPages.Cms
{
    public class ModalCreateChannels : BasePageCms
    {
		protected DropDownList DdlIsIncludeChildren;
        protected DropDownList DdlIsCreateContents;

        private string _channelIdCollection;

        public static string GetOpenWindowString(int siteId)
        {
            return LayerUtils.GetOpenScriptWithCheckBoxValue("生成栏目页", PageUtils.GetCmsUrl(siteId, nameof(ModalCreateChannels), null), "ChannelIDCollection", "请选择需要生成页面的栏目!", 550, 300);
        }

		public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("siteId", "ChannelIDCollection");

            _channelIdCollection = AuthRequest.GetQueryString("ChannelIDCollection");
		}

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            var isIncludeChildren = TranslateUtils.ToBool(DdlIsIncludeChildren.SelectedValue);
            var isCreateContents = TranslateUtils.ToBool(DdlIsCreateContents.SelectedValue);

            foreach (var channelId in TranslateUtils.StringCollectionToIntList(_channelIdCollection))
            {
                CreateManager.CreateChannel(SiteId, channelId);
                if (isCreateContents)
                {
                    CreateManager.CreateAllContent(SiteId, channelId);
                }
                if (isIncludeChildren)
                {
                    foreach (var childChannelId in ChannelManager.GetChannelIdList(ChannelManager.GetChannelInfo(SiteId, channelId), EScopeType.Descendant, string.Empty, string.Empty, string.Empty))
                    {
                        CreateManager.CreateChannel(SiteId, childChannelId);
                        if (isCreateContents)
                        {
                            CreateManager.CreateAllContent(SiteId, channelId);
                        }
                    }
                }
            }

            LayerUtils.CloseAndOpenPageCreateStatus(Page);
		}
	}
}
