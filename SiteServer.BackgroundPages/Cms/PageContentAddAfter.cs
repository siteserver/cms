using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using SiteServer.Abstractions;
using SiteServer.CMS.Context;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Repositories;
using WebUtils = SiteServer.BackgroundPages.Core.WebUtils;

namespace SiteServer.BackgroundPages.Cms
{
	public class PageContentAddAfter : BasePageCms
    {
        public RadioButtonList RblOperation;
        public PlaceHolder PhSiteId;
        public DropDownList DdlSiteId;
        public ListBox LbChannelId;
        public PlaceHolder PhSubmit;

        private Channel _channel;
        private int _contentId;
        private string _returnUrl;

        public enum EContentAddAfter
        {
            ContinueAdd,
            ManageContents,
            Contribute
        }

        public static string GetRedirectUrl(int siteId, int channelId, int contentId, string returnUrl)
        {
            return PageUtils.GetCmsUrl(siteId, nameof(PageContentAddAfter), new NameValueCollection
            {
                {"channelId", channelId.ToString()},
                {"ContentID", contentId.ToString()},
                {"ReturnUrl", StringUtils.ValueToUrl(returnUrl)}
            });
        }

		public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

			PageUtils.CheckRequestParameter("siteId", "channelId", "ContentID", "ReturnUrl");
			var channelId = AuthRequest.GetQueryInt("channelId");
            _contentId = AuthRequest.GetQueryInt("ContentID");
            _returnUrl = StringUtils.ValueFromUrl(AuthRequest.GetQueryString("ReturnUrl"));

            _channel = DataProvider.ChannelRepository.GetAsync(channelId).GetAwaiter().GetResult();

            if (IsPostBack) return;

            RblOperation.Items.Add(new ListItem("继续添加内容", EContentAddAfter.ContinueAdd.ToString()));
            RblOperation.Items.Add(new ListItem("返回管理界面", EContentAddAfter.ManageContents.ToString()));

            var isCrossSiteTrans = CrossSiteTransUtility.IsCrossSiteTransAsync(Site, _channel).GetAwaiter().GetResult();
            var isAutomatic = CrossSiteTransUtility.IsAutomatic(_channel);

            var isTranslated = ContentUtility.AfterContentAddedAsync(Site, _channel, _contentId, isCrossSiteTrans, isAutomatic).GetAwaiter().GetResult();
            if (isCrossSiteTrans && !isAutomatic)
            {
                RblOperation.Items.Add(new ListItem("转发到其他站点", EContentAddAfter.Contribute.ToString()));
            }

            SuccessMessage(isTranslated ? "内容添加成功并已转发到指定站点，请选择后续操作。" : "内容添加成功，请选择后续操作。");

            PhSiteId.Visible = PhSubmit.Visible = false;
        }

        public void RblOperation_SelectedIndexChanged(object sender, EventArgs e)
		{
            var after = TranslateUtils.ToEnum(RblOperation.SelectedValue, EContentAddAfter.ContinueAdd);
            if (after == EContentAddAfter.ContinueAdd)
            {
                PageUtils.Redirect(WebUtils.GetContentAddAddUrl(SiteId, _channel.Id, AuthRequest.GetQueryString("ReturnUrl")));
                return;
            }

		    if (after == EContentAddAfter.ManageContents)
		    {
		        PageUtils.Redirect(_returnUrl);
		        return;
		    }

		    if (after == EContentAddAfter.Contribute)
		    {
		        CrossSiteTransUtility.LoadSiteIdDropDownListAsync(DdlSiteId, Site, _channel.Id).GetAwaiter().GetResult();

		        if (DdlSiteId.Items.Count > 0)
		        {
		            DdlSiteId_SelectedIndexChanged(sender, e);
		        }
		        PhSiteId.Visible = PhSubmit.Visible = true;
		    }
		}

        public void DdlSiteId_SelectedIndexChanged(object sender, EventArgs e)
        {
            var psId = int.Parse(DdlSiteId.SelectedValue);
            CrossSiteTransUtility.LoadChannelIdListBoxAsync(LbChannelId, Site, psId, _channel, AuthRequest.AdminPermissionsImpl).GetAwaiter().GetResult();
        }

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            if (!Page.IsPostBack || !Page.IsValid) return;

            var targetSiteId = int.Parse(DdlSiteId.SelectedValue);
            var targetSite = DataProvider.SiteRepository.GetAsync(targetSiteId).GetAwaiter().GetResult();
            try
            {
                foreach (ListItem listItem in LbChannelId.Items)
                {
                    if (!listItem.Selected) continue;
                    var targetChannelId = TranslateUtils.ToInt(listItem.Value);
                    if (targetChannelId != 0)
                    {
                        CrossSiteTransUtility.TransContentInfoAsync(Site, _channel, _contentId, targetSite, targetChannelId).GetAwaiter().GetResult();
                    }
                }

                AuthRequest.AddSiteLogAsync(SiteId, _channel.Id, _contentId, "内容跨站转发", $"转发到站点:{targetSite.SiteName}").GetAwaiter().GetResult();

                SuccessMessage("内容跨站转发成功，请选择后续操作。");
                RblOperation.Items.Clear();
                RblOperation.Items.Add(new ListItem("继续添加内容", EContentAddAfter.ContinueAdd.ToString()));
                RblOperation.Items.Add(new ListItem("返回管理界面", EContentAddAfter.ManageContents.ToString()));
                RblOperation.Items.Add(new ListItem("转发到其他站点", EContentAddAfter.Contribute.ToString()));
                PhSiteId.Visible = PhSubmit.Visible = false;
            }
            catch (Exception ex)
            {
                FailMessage(ex, "内容跨站转发失败！");
            }
        }
	}
}
