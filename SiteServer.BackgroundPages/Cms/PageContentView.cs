using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Model;
using BaiRong.Core.Table;
using SiteServer.BackgroundPages.Core;
using SiteServer.CMS.Controllers.Preview;
using SiteServer.CMS.Core;
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

        private int _nodeId;
        private string _tableName;
        private List<int> _relatedIdentities;
        private int _contentId;
        private string _returnUrl;
        private ContentInfo _contentInfo;

        public static string GetContentViewUrl(int publishmentSystemId, int nodeId, int contentId, string returnUrl)
        {
            return PageUtils.GetCmsUrl(nameof(PageContentView), new NameValueCollection
            {
                {"PublishmentSystemID", publishmentSystemId.ToString()},
                {"NodeID", nodeId.ToString()},
                {"ID", contentId.ToString()},
                {"ReturnUrl", StringUtils.ValueToUrl(returnUrl)}
            });
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("PublishmentSystemID", "NodeID", "ID", "ReturnUrl");

            _nodeId = Body.GetQueryInt("NodeID");
            var nodeInfo = NodeManager.GetNodeInfo(PublishmentSystemId, _nodeId);
            _tableName = NodeManager.GetTableName(PublishmentSystemInfo, nodeInfo);
            _contentId = Body.GetQueryInt("ID");
            _returnUrl = StringUtils.ValueFromUrl(Body.GetQueryString("ReturnUrl"));

            _relatedIdentities = RelatedIdentities.GetChannelRelatedIdentities(PublishmentSystemId, _nodeId);

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

            LtlNodeName.Text = NodeManager.GetNodeName(PublishmentSystemId, _nodeId);

            LtlTags.Text = _contentInfo.Tags;
            if (string.IsNullOrEmpty(LtlTags.Text))
            {
                PhTags.Visible = false;
            }

            LtlContentGroup.Text = _contentInfo.ContentGroupNameCollection;
            if (string.IsNullOrEmpty(LtlContentGroup.Text))
            {
                PhContentGroup.Visible = false;
            }

            LtlLastEditDate.Text = DateUtils.GetDateAndTimeString(_contentInfo.LastEditDate);
            LtlAddUserName.Text = AdminManager.GetDisplayName(_contentInfo.AddUserName, true);
            LtlLastEditUserName.Text = AdminManager.GetDisplayName(_contentInfo.LastEditUserName, true);

            LtlContentLevel.Text = CheckManager.GetCheckState(PublishmentSystemInfo, _contentInfo.IsChecked, _contentInfo.CheckedLevel);

            if (_contentInfo.ReferenceId > 0 && _contentInfo.GetString(ContentAttribute.TranslateContentType) != ETranslateContentType.ReferenceContent.ToString())
            {
                var referencePublishmentSystemId = DataProvider.NodeDao.GetPublishmentSystemId(_contentInfo.SourceId);
                var referencePublishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(referencePublishmentSystemId);
                var referenceTableName = NodeManager.GetTableName(referencePublishmentSystemInfo, _contentInfo.SourceId);
                var referenceContentInfo = DataProvider.ContentDao.GetContentInfo(referenceTableName, _contentInfo.ReferenceId);

                if (referenceContentInfo != null)
                {
                    var pageUrl = PageUtility.GetContentUrl(referencePublishmentSystemInfo, referenceContentInfo, true);
                    var referenceNodeInfo = NodeManager.GetNodeInfo(referenceContentInfo.PublishmentSystemId, referenceContentInfo.NodeId);
                    var addEditUrl =
                        WebUtils.GetContentAddEditUrl(referencePublishmentSystemInfo.PublishmentSystemId,
                            referenceNodeInfo, _contentInfo.ReferenceId, Body.GetQueryString("ReturnUrl"));

                    LtlScripts.Text += $@"
<div class=""tips"">此内容为对内容 （站点：{referencePublishmentSystemInfo.PublishmentSystemName},栏目：{referenceNodeInfo.NodeName}）“<a href=""{pageUrl}"" target=""_blank"">{_contentInfo.Title}</a>”（<a href=""{addEditUrl}"">编辑</a>） 的引用，内容链接将和原始内容链接一致</div>";
                }
            }

            BtnSubmit.Attributes.Add("onclick", ModalContentCheck.GetOpenWindowString(PublishmentSystemInfo.PublishmentSystemId, _nodeId, _contentId, _returnUrl));
            HlPreview.NavigateUrl = PreviewApi.GetContentUrl(PublishmentSystemId, _contentInfo.NodeId, _contentInfo.Id);
        }

        private void RptContents_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem) return;

            var styleInfo = (TableStyleInfo)e.Item.DataItem;

            var inputHtml = InputParserUtility.GetContentByTableStyle(_contentInfo.GetString(styleInfo.AttributeName), PublishmentSystemInfo, styleInfo);

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
