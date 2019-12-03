using System;
using System.Web.UI.WebControls;
using SiteServer.CMS.Context;
using SiteServer.Abstractions;
using SiteServer.CMS.Context.Enumerations;
using SiteServer.CMS.Core.Create;
using SiteServer.CMS.DataCache;

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

            foreach (var channelId in StringUtils.GetIntList(_channelIdCollection))
            {
                CreateManager.CreateChannelAsync(SiteId, channelId).GetAwaiter().GetResult();
                if (isCreateContents)
                {
                    CreateManager.CreateAllContentAsync(SiteId, channelId).GetAwaiter().GetResult();
                }
                if (isIncludeChildren)
                {
                    foreach (var childChannelId in ChannelManager.GetChannelIdListAsync(ChannelManager.GetChannelAsync(SiteId, channelId).GetAwaiter().GetResult(), EScopeType.Descendant, string.Empty, string.Empty, string.Empty).GetAwaiter().GetResult())
                    {
                        CreateManager.CreateChannelAsync(SiteId, childChannelId).GetAwaiter().GetResult();
                        if (isCreateContents)
                        {
                            CreateManager.CreateAllContentAsync(SiteId, channelId).GetAwaiter().GetResult();
                        }
                    }
                }
            }

            LayerUtils.CloseAndOpenPageCreateStatus(Page);
		}
	}
}
