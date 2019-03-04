using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using SiteServer.Utils;
using SiteServer.BackgroundPages.Core;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.DataCache.Content;
using SiteServer.CMS.Model;
using SiteServer.CMS.Model.Attributes;
using SiteServer.CMS.Model.Enumerations;

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
        private ContentInfo _contentInfo;

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

            var channelInfo = ChannelManager.GetChannelInfo(SiteId, _channelId);
            _contentId = AuthRequest.GetQueryInt("id");
            _returnUrl = StringUtils.ValueFromUrl(AuthRequest.GetQueryString("returnUrl"));

            _contentInfo = ContentManager.GetContentInfo(SiteInfo, channelInfo, _contentId);

            if (IsPostBack) return;

            var styleInfoList = TableStyleManager.GetContentStyleInfoList(SiteInfo, channelInfo);

            RptContents.DataSource = styleInfoList;
            RptContents.ItemDataBound += RptContents_ItemDataBound;
            RptContents.DataBind();

            LtlTitle.Text = _contentInfo.Title;
            LtlChannelName.Text = channelInfo.ChannelName;

            LtlTags.Text = _contentInfo.Tags;
            if (string.IsNullOrEmpty(LtlTags.Text))
            {
                PhTags.Visible = false;
            }

            LtlContentGroup.Text = _contentInfo.GroupNameCollection;
            if (string.IsNullOrEmpty(LtlContentGroup.Text))
            {
                PhContentGroup.Visible = false;
            }

            LtlLastEditDate.Text = DateUtils.GetDateAndTimeString(_contentInfo.LastEditDate);
            LtlAddUserName.Text = AdminManager.GetDisplayName(_contentInfo.AddUserName, true);
            LtlLastEditUserName.Text = AdminManager.GetDisplayName(_contentInfo.LastEditUserName, true);

            LtlContentLevel.Text = CheckManager.GetCheckState(SiteInfo, _contentInfo);

            if (_contentInfo.ReferenceId > 0 && _contentInfo.GetString(ContentAttribute.TranslateContentType) != ETranslateContentType.ReferenceContent.ToString())
            {
                var referenceSiteId = DataProvider.ChannelDao.GetSiteId(_contentInfo.SourceId);
                var referenceSiteInfo = SiteManager.GetSiteInfo(referenceSiteId);
                var referenceContentInfo = ContentManager.GetContentInfo(referenceSiteInfo, _contentInfo.SourceId, _contentInfo.ReferenceId);

                if (referenceContentInfo != null)
                {
                    var pageUrl = PageUtility.GetContentUrl(referenceSiteInfo, referenceContentInfo, true);
                    var referenceNodeInfo = ChannelManager.GetChannelInfo(referenceContentInfo.SiteId, referenceContentInfo.ChannelId);
                    var addEditUrl =
                        WebUtils.GetContentAddEditUrl(referenceSiteInfo.Id,
                            referenceNodeInfo, _contentInfo.ReferenceId, AuthRequest.GetQueryString("ReturnUrl"));

                    LtlScripts.Text += $@"
<div class=""tips"">此内容为对内容 （站点：{referenceSiteInfo.SiteName},栏目：{referenceNodeInfo.ChannelName}）“<a href=""{pageUrl}"" target=""_blank"">{_contentInfo.Title}</a>”（<a href=""{addEditUrl}"">编辑</a>） 的引用，内容链接将和原始内容链接一致</div>";
                }
            }
        }

        private void RptContents_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem) return;

            var styleInfo = (TableStyleInfo)e.Item.DataItem;

            var inputHtml = InputParserUtility.GetContentByTableStyle(_contentInfo.GetString(styleInfo.AttributeName), SiteInfo, styleInfo);

            var ltlHtml = (Literal)e.Item.FindControl("ltlHtml");

            ltlHtml.Text = $@"
<div class=""form-group form-row"">
    <label class=""col-sm-2 col-form-label"">{styleInfo.DisplayName}</label>
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
