using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using Datory.Utils;
using SiteServer.Abstractions;
using SiteServer.CMS.Context;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Repositories;
using Content = SiteServer.Abstractions.Content;
using TableStyle = SiteServer.Abstractions.TableStyle;
using WebUtils = SiteServer.BackgroundPages.Core.WebUtils;

namespace SiteServer.BackgroundPages.Cms
{
    public class ModalContentView : BasePageCms
    {
        public Literal LtlScripts;
        public Literal LtlTitle;
        public Literal LtlChannelName;
        public Literal LtlContentGroup;
        public Literal LtlTags;
        public Literal LtlLastEditDate;
        public Literal LtlAddUserName;
        public Literal LtlLastEditUserName;
        public Literal LtlContentLevel;
        public Repeater RptContents;
        public PlaceHolder PhTags;
        public PlaceHolder PhContentGroup;

        private int _channelId;
        private int _contentId;
        private string _returnUrl;
        private Content _content;

        public static string GetOpenWindowString(int siteId, int channelId, int contentId, string returnUrl)
        {
            return LayerUtils.GetOpenScript("查看内容", PageUtils.GetCmsUrl(siteId, nameof(ModalContentView), new NameValueCollection
            {
                {"channelId", channelId.ToString()},
                {"id", contentId.ToString()},
                {"returnUrl", StringUtils.ValueToUrl(returnUrl)}
            }));
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("siteId", "channelId", "id", "ReturnUrl");

            _channelId = AuthRequest.GetQueryInt("channelId");
            if (_channelId < 0) _channelId = -_channelId;

            var channelInfo = DataProvider.ChannelRepository.GetAsync(_channelId).GetAwaiter().GetResult();
            _contentId = AuthRequest.GetQueryInt("id");
            _returnUrl = StringUtils.ValueFromUrl(AuthRequest.GetQueryString("returnUrl"));

            _content = DataProvider.ContentRepository.GetAsync(Site, channelInfo, _contentId).GetAwaiter().GetResult();

            if (IsPostBack) return;

            var styleList = DataProvider.TableStyleRepository.GetContentStyleListAsync(Site, channelInfo).GetAwaiter().GetResult();

            RptContents.DataSource = styleList;
            RptContents.ItemDataBound += RptContents_ItemDataBound;
            RptContents.DataBind();

            LtlTitle.Text = _content.Title;
            LtlChannelName.Text = channelInfo.ChannelName;

            LtlTags.Text = Utilities.ToString(_content.TagNames);
            if (string.IsNullOrEmpty(LtlTags.Text))
            {
                PhTags.Visible = false;
            }

            LtlContentGroup.Text = Utilities.ToString(_content.GroupNames);
            if (string.IsNullOrEmpty(LtlContentGroup.Text))
            {
                PhContentGroup.Visible = false;
            }

            LtlLastEditDate.Text = DateUtils.GetDateAndTimeString(_content.LastEditDate);
            //LtlAddUserName.Text = DataProvider.AdministratorRepository.GetDisplayNameAsync(_content.AddUserName).GetAwaiter().GetResult();
            //LtlLastEditUserName.Text = DataProvider.AdministratorRepository.GetDisplayNameAsync(_content.LastEditUserName).GetAwaiter().GetResult();

            LtlContentLevel.Text = CheckManager.GetCheckState(Site, _content);

            if (_content.ReferenceId > 0 && TranslateContentType.ReferenceContent ==  _content.TranslateContentType)
            {
                var referenceSiteId = DataProvider.ChannelRepository.GetSiteIdAsync(_content.SourceId).GetAwaiter().GetResult();
                var referenceSite = DataProvider.SiteRepository.GetAsync(referenceSiteId).GetAwaiter().GetResult();
                var referenceContentInfo = DataProvider.ContentRepository.GetAsync(referenceSite, _content.SourceId, _content.ReferenceId).GetAwaiter().GetResult();

                if (referenceContentInfo != null)
                {
                    var pageUrl = PageUtility.GetContentUrlAsync(referenceSite, referenceContentInfo, true).GetAwaiter().GetResult();
                    var referenceNodeInfo = DataProvider.ChannelRepository.GetAsync(referenceContentInfo.ChannelId).GetAwaiter().GetResult();
                    var addEditUrl =
                        WebUtils.GetContentAddEditUrl(referenceSite.Id,
                            referenceNodeInfo.Id, _content.ReferenceId, AuthRequest.GetQueryString("ReturnUrl"));

                    LtlScripts.Text += $@"
<div class=""tips"">此内容为对内容 （站点：{referenceSite.SiteName},栏目：{referenceNodeInfo.ChannelName}）“<a href=""{pageUrl}"" target=""_blank"">{_content.Title}</a>”（<a href=""{addEditUrl}"">编辑</a>） 的引用，内容链接将和原始内容链接一致</div>";
                }
            }
        }

        private void RptContents_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem) return;

            var style = (TableStyle)e.Item.DataItem;

            var inputHtml = InputParserUtility.GetContentByTableStyleAsync(_content.Get<string>(style.AttributeName), Site, style).GetAwaiter().GetResult();

            var ltlHtml = (Literal)e.Item.FindControl("ltlHtml");

            ltlHtml.Text = $@"
<div class=""form-group form-row"">
    <label class=""col-sm-2 col-form-label"">{style.DisplayName}</label>
    <div class=""col-sm-9 form-control-plaintext"">
        {inputHtml}
    </div>
    <div class=""col-sm-1""></div>
</div>
";
        }

        public string ReturnUrl => _returnUrl;
    }
}
