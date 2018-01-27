using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using SiteServer.Utils;
using SiteServer.BackgroundPages.Core;
using SiteServer.CMS.Controllers.Preview;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;
using SiteServer.CMS.Model.Enumerations;

namespace SiteServer.BackgroundPages.Cms
{
    public class PageContentView : BasePageCms
    {
        public Literal LtlScripts;
        public Literal LtlNodeName;
        public Literal LtlContentGroup;
        public Literal LtlTags;
        public Literal LtlLastEditDate;
        public Literal LtlAddUserName;
        public Literal LtlLastEditUserName;
        public Literal LtlContentLevel;
        public Repeater RptContents;
        public PlaceHolder PhTags;
        public PlaceHolder PhContentGroup;
        public Button BtnSubmit;
        public HyperLink HlPreview;

        private int _channelId;
        private string _tableName;
        private List<int> _relatedIdentities;
        private int _contentId;
        private string _returnUrl;
        private ContentInfo _contentInfo;

        public static string GetContentViewUrl(int siteId, int channelId, int contentId, string returnUrl)
        {
            return PageUtils.GetCmsUrl(siteId, nameof(PageContentView), new NameValueCollection
            {
                {"channelId", channelId.ToString()},
                {"ID", contentId.ToString()},
                {"ReturnUrl", StringUtils.ValueToUrl(returnUrl)}
            });
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("siteId", "channelId", "ID", "ReturnUrl");

            _channelId = Body.GetQueryInt("channelId");
            var channelInfo = ChannelManager.GetChannelInfo(SiteId, _channelId);
            _tableName = ChannelManager.GetTableName(SiteInfo, channelInfo);
            _contentId = Body.GetQueryInt("ID");
            _returnUrl = StringUtils.ValueFromUrl(Body.GetQueryString("ReturnUrl"));

            _relatedIdentities = RelatedIdentities.GetChannelRelatedIdentities(SiteId, _channelId);

            _contentInfo = DataProvider.ContentDao.GetContentInfo(_tableName, _contentId);

            if (IsPostBack) return;

            var styleInfoList = TableStyleManager.GetTableStyleInfoList(_tableName, _relatedIdentities);
            var myStyleInfoArrayList = new ArrayList();
            if (styleInfoList != null)
            {
                foreach (var styleInfo in styleInfoList)
                {
                    myStyleInfoArrayList.Add(styleInfo);
                }
            }

            RptContents.DataSource = myStyleInfoArrayList;
            RptContents.ItemDataBound += RptContents_ItemDataBound;
            RptContents.DataBind();

            LtlNodeName.Text = ChannelManager.GetChannelName(SiteId, _channelId);

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

            LtlContentLevel.Text = CheckManager.GetCheckState(SiteInfo, _contentInfo.IsChecked, _contentInfo.CheckedLevel);

            if (_contentInfo.ReferenceId > 0 && _contentInfo.GetString(ContentAttribute.TranslateContentType) != ETranslateContentType.ReferenceContent.ToString())
            {
                var referenceSiteId = DataProvider.ChannelDao.GetSiteId(_contentInfo.SourceId);
                var referenceSiteInfo = SiteManager.GetSiteInfo(referenceSiteId);
                var referenceTableName = ChannelManager.GetTableName(referenceSiteInfo, _contentInfo.SourceId);
                var referenceContentInfo = DataProvider.ContentDao.GetContentInfo(referenceTableName, _contentInfo.ReferenceId);

                if (referenceContentInfo != null)
                {
                    var pageUrl = PageUtility.GetContentUrl(referenceSiteInfo, referenceContentInfo, true);
                    var referenceNodeInfo = ChannelManager.GetChannelInfo(referenceContentInfo.SiteId, referenceContentInfo.ChannelId);
                    var addEditUrl =
                        WebUtils.GetContentAddEditUrl(referenceSiteInfo.Id,
                            referenceNodeInfo, _contentInfo.ReferenceId, Body.GetQueryString("ReturnUrl"));

                    LtlScripts.Text += $@"
<div class=""tips"">此内容为对内容 （站点：{referenceSiteInfo.SiteName},栏目：{referenceNodeInfo.ChannelName}）“<a href=""{pageUrl}"" target=""_blank"">{_contentInfo.Title}</a>”（<a href=""{addEditUrl}"">编辑</a>） 的引用，内容链接将和原始内容链接一致</div>";
                }
            }

            BtnSubmit.Attributes.Add("onclick", ModalContentCheck.GetOpenWindowString(SiteInfo.Id, _channelId, _contentId, _returnUrl));
            HlPreview.NavigateUrl = ApiRoutePreview.GetContentUrl(SiteId, _contentInfo.ChannelId, _contentInfo.Id);
        }

        private void RptContents_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem) return;

            var styleInfo = (TableStyleInfo)e.Item.DataItem;

            var inputHtml = InputParserUtility.GetContentByTableStyle(_contentInfo.GetString(styleInfo.AttributeName), SiteInfo, styleInfo);

            var ltlHtml = (Literal)e.Item.FindControl("ltlHtml");

            ltlHtml.Text = $@"
<div class=""form-group form-row"">
    <label class=""col-sm-1 col-form-label"">{styleInfo.DisplayName}</label>
    <div class=""col-sm-10 form-control-plaintext"">
        {inputHtml}
    </div>
    <div class=""col-sm-1""></div>
</div>
";
        }

        public string ReturnUrl => _returnUrl;
    }
}
