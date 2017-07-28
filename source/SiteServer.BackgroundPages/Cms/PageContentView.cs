using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web.UI;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.AuxiliaryTable;
using BaiRong.Core.Model;
using BaiRong.Core.Model.Attributes;
using BaiRong.Core.Model.Enumerations;
using BaiRong.Core.Permissions;
using SiteServer.BackgroundPages.Core;
using SiteServer.CMS.Core;
using SiteServer.CMS.Core.User;
using SiteServer.CMS.Model.Enumerations;

namespace SiteServer.BackgroundPages.Cms
{
    public class PageContentView : BasePageCms
    {
        public Literal ltlScripts;
        public Literal ltlNodeName;

        public Literal ltlContentGroup;
        public Literal ltlTags;

        public Literal ltlLastEditDate;
        public Literal ltlAddUserName;
        public Literal ltlLastEditUserName;
        public Literal ltlContentLevel;

        public Repeater MyRepeater;

        public Control RowTags;
        public Control RowContentGroup;

        public Button Submit;

        private int _nodeId;
        private ETableStyle _tableStyle;
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
            _tableStyle = NodeManager.GetTableStyle(PublishmentSystemInfo, nodeInfo);
            _tableName = NodeManager.GetTableName(PublishmentSystemInfo, nodeInfo);
            _contentId = Body.GetQueryInt("ID");
            _returnUrl = StringUtils.ValueFromUrl(Body.GetQueryString("ReturnUrl"));

            _relatedIdentities = RelatedIdentities.GetChannelRelatedIdentities(PublishmentSystemId, _nodeId);

            _contentInfo = DataProvider.ContentDao.GetContentInfo(_tableStyle, _tableName, _contentId);

            if (!IsPostBack)
            {
                BreadCrumb(AppManager.Cms.LeftMenu.IdContent, "查看内容", string.Empty);

                var styleInfoList = TableStyleManager.GetTableStyleInfoList(_tableStyle, _tableName, _relatedIdentities);
                var myStyleInfoArrayList = new ArrayList();
                if (styleInfoList != null)
                {
                    foreach (var styleInfo in styleInfoList)
                    {
                        if (styleInfo.IsVisible)
                        {
                            myStyleInfoArrayList.Add(styleInfo);
                        }
                    }
                }

                MyRepeater.DataSource = myStyleInfoArrayList;
                MyRepeater.ItemDataBound += MyRepeater_ItemDataBound;
                MyRepeater.DataBind();

                ltlNodeName.Text = NodeManager.GetNodeName(PublishmentSystemId, _nodeId);
                ltlNodeName.Text += $@"
<script>
function submitPreview(){{
    window.open(""{PagePreview.GetRedirectUrl(PublishmentSystemId, _nodeId, _contentId, 0, 0)}"");
}}
</script>
";

                if (PublishmentSystemInfo.Additional.IsRelatedByTags)
                {
                    ltlTags.Text = _contentInfo.Tags;
                }
                if (string.IsNullOrEmpty(ltlTags.Text))
                {
                    RowTags.Visible = false;
                }

                ltlContentGroup.Text = _contentInfo.ContentGroupNameCollection;
                if (string.IsNullOrEmpty(ltlContentGroup.Text))
                {
                    RowContentGroup.Visible = false;
                }

                ltlLastEditDate.Text = DateUtils.GetDateAndTimeString(_contentInfo.LastEditDate);
                ltlAddUserName.Text = AdminManager.GetDisplayName(_contentInfo.AddUserName, true);
                ltlLastEditUserName.Text = AdminManager.GetDisplayName(_contentInfo.LastEditUserName, true);

                ltlContentLevel.Text = LevelManager.GetCheckState(PublishmentSystemInfo, _contentInfo.IsChecked, _contentInfo.CheckedLevel);

                if (_contentInfo.ReferenceId > 0 && _contentInfo.GetExtendedAttribute(ContentAttribute.TranslateContentType) != ETranslateContentType.ReferenceContent.ToString())
                {
                    var referencePublishmentSystemID = DataProvider.NodeDao.GetPublishmentSystemId(_contentInfo.SourceId);
                    var referencePublishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(referencePublishmentSystemID);
                    var referenceTableStyle = NodeManager.GetTableStyle(referencePublishmentSystemInfo, _contentInfo.SourceId);
                    var referenceTableName = NodeManager.GetTableName(referencePublishmentSystemInfo, _contentInfo.SourceId);
                    var referenceContentInfo = DataProvider.ContentDao.GetContentInfo(referenceTableStyle, referenceTableName, _contentInfo.ReferenceId);

                    if (referenceContentInfo != null)
                    {
                        var pageUrl = PageUtility.GetContentUrl(referencePublishmentSystemInfo, referenceContentInfo);
                        var referenceNodeInfo = NodeManager.GetNodeInfo(referenceContentInfo.PublishmentSystemId, referenceContentInfo.NodeId);
                        var addEditUrl =
                            WebUtils.GetContentAddEditUrl(referencePublishmentSystemInfo.PublishmentSystemId,
                                referenceNodeInfo, _contentInfo.ReferenceId, Body.GetQueryString("ReturnUrl"));

                        ltlScripts.Text += $@"
<div class=""tips"">此内容为对内容 （站点：{referencePublishmentSystemInfo.PublishmentSystemName},栏目：{referenceNodeInfo.NodeName}）“<a href=""{pageUrl}"" target=""_blank"">{_contentInfo.Title}</a>”（<a href=""{addEditUrl}"">编辑</a>） 的引用，内容链接将和原始内容链接一致</div>";
                    }
                }

                Submit.Attributes.Add("onclick", ModalContentCheck.GetOpenWindowString(PublishmentSystemInfo.PublishmentSystemId, _nodeId, _contentId, _returnUrl));
            }
        }

        void MyRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var styleInfo = (TableStyleInfo)e.Item.DataItem;

                var helpHtml = $"{styleInfo.DisplayName}：";

                var inputHtml = InputParserUtility.GetContentByTableStyle(_contentInfo.GetExtendedAttribute(styleInfo.AttributeName), PublishmentSystemInfo, _tableStyle, styleInfo);

                var ltlHtml = e.Item.FindControl("ltlHtml") as Literal;

                ltlHtml.Text = $@"
<tr>
  <td>{helpHtml}</td>
  <td colspan=""2"">{inputHtml}</td>
</tr>
";
            }
        }

        public string ReturnUrl => _returnUrl;
    }
}
