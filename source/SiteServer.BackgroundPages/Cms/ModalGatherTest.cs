using System;
using System.Collections.Specialized;
using System.Text;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.AuxiliaryTable;
using BaiRong.Core.Model;
using BaiRong.Core.Model.Attributes;
using BaiRong.Core.Model.Enumerations;
using BaiRong.Core.Net;
using SiteServer.BackgroundPages.Controls;
using SiteServer.CMS.Core;

namespace SiteServer.BackgroundPages.Cms
{
	public class ModalGatherTest : BasePageCms
    {
		protected DropDownList GatherUrls;
		protected Repeater ContentUrlRepeater;

		protected Button GetContentUrls;
		protected NoTagText Content;

		private string _gatherRuleName;
        private bool _isFileRule;

        public static string GetOpenWindowString(int publishmentSystemId, string gatherRuleName, bool isFileRule)
        {
            return PageUtils.GetOpenWindowString("信息采集测试", PageUtils.GetCmsUrl(nameof(ModalGatherTest), new NameValueCollection
            {
                {"PublishmentSystemID", publishmentSystemId.ToString()},
                {"GatherRuleName", gatherRuleName},
                {"IsFileRule", isFileRule.ToString()}
            }), 700, 650, true);
        }

		public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("PublishmentSystemID", "GatherRuleName", "IsFileRule");

			_gatherRuleName = Body.GetQueryString("GatherRuleName");
            _isFileRule = Body.GetQueryBool("IsFileRule");

			if (!IsPostBack)
			{
                InfoMessage("采集名称：" + _gatherRuleName);

                if (_isFileRule)
                {
                    var gatherFileRuleInfo = DataProvider.GatherFileRuleDao.GetGatherFileRuleInfo(_gatherRuleName, PublishmentSystemId);

                    GatherUrls.Items.Add(new ListItem(gatherFileRuleInfo.GatherUrl, gatherFileRuleInfo.GatherUrl));

                    GetContentUrls.Text = "获取内容";
                }
                else
                {
                    var gatherRuleInfo = DataProvider.GatherRuleDao.GetGatherRuleInfo(_gatherRuleName, PublishmentSystemId);

                    var gatherUrlArrayList = GatherUtility.GetGatherUrlArrayList(gatherRuleInfo);
                    foreach (string gatherUrl in gatherUrlArrayList)
                    {
                        GatherUrls.Items.Add(new ListItem(gatherUrl, gatherUrl));
                    }
                }				
			}
		}

		public void GatherUrls_Click(object sender, EventArgs e)
		{
            if (_isFileRule)
            {
                var gatherFileRuleInfo = DataProvider.GatherFileRuleDao.GetGatherFileRuleInfo(_gatherRuleName, PublishmentSystemId);

                var builder = new StringBuilder();
                if (gatherFileRuleInfo.IsToFile == false)
                {
                    var regexTitle = GatherUtility.GetRegexTitle(gatherFileRuleInfo.ContentTitleStart, gatherFileRuleInfo.ContentTitleEnd);
                    var regexContentExclude = GatherUtility.GetRegexString(gatherFileRuleInfo.ContentExclude);
                    var regexContent = GatherUtility.GetRegexContent(gatherFileRuleInfo.ContentContentStart, gatherFileRuleInfo.ContentContentEnd);

                    var contentAttributes = TranslateUtils.StringCollectionToStringList(gatherFileRuleInfo.ContentAttributes);
                    var contentAttributesXML = TranslateUtils.ToNameValueCollection(gatherFileRuleInfo.ContentAttributesXml);

                    var attributes = GatherUtility.GetContentNameValueCollection(gatherFileRuleInfo.Charset, gatherFileRuleInfo.GatherUrl, string.Empty, regexContentExclude, gatherFileRuleInfo.ContentHtmlClearCollection, gatherFileRuleInfo.ContentHtmlClearTagCollection, regexTitle, regexContent, string.Empty, string.Empty, string.Empty, string.Empty, contentAttributes, contentAttributesXML);

                    var styleInfoList = TableStyleManager.GetTableStyleInfoList(ETableStyle.BackgroundContent, PublishmentSystemInfo.AuxiliaryTableForContent, null);
                    foreach (var styleInfo in styleInfoList)
                    {
                        if (string.IsNullOrEmpty(attributes[styleInfo.AttributeName.ToLower()])) continue;
                        builder.Append($"{styleInfo.DisplayName}： {attributes[styleInfo.AttributeName.ToLower()]}<br><br>");
                    }
                }
                else
                {
                    try
                    {
                        var fileContent = WebClientUtils.GetRemoteFileSource(gatherFileRuleInfo.GatherUrl, gatherFileRuleInfo.Charset, string.Empty);

                        builder.Append(StringUtils.HtmlEncode(fileContent));
                    }
                    catch (Exception ex)
                    {
                        FailMessage(ex, ex.Message);
                    }
                }

                Content.Text = builder.ToString();
            }
            else
            {
                var gatherUrl = GatherUrls.SelectedValue;
                var errorBuilder = new StringBuilder();
                var gatherRuleInfo = DataProvider.GatherRuleDao.GetGatherRuleInfo(_gatherRuleName, PublishmentSystemId);

                var regexUrlInclude = GatherUtility.GetRegexString(gatherRuleInfo.UrlInclude);
                var regexListArea = GatherUtility.GetRegexArea(gatherRuleInfo.ListAreaStart, gatherRuleInfo.ListAreaEnd);

                var contentUrlArrayList = GatherUtility.GetContentUrls(gatherUrl, gatherRuleInfo.Charset, gatherRuleInfo.CookieString, regexListArea, regexUrlInclude, errorBuilder);

                ContentUrlRepeater.DataSource = contentUrlArrayList;
                ContentUrlRepeater.ItemDataBound += ContentUrlRepeater_ItemDataBound;
                ContentUrlRepeater.DataBind();

                InfoMessage($"采集名称：{_gatherRuleName}&nbsp;&nbsp;内容数：{contentUrlArrayList.Count}");

                Content.Text = string.Empty;
            }
		}

		public void GetContent_Click(object sender, EventArgs e)
		{
			var getContent = (Button)sender;
			var contentUrl = getContent.CommandArgument;

            var gatherRuleInfo = DataProvider.GatherRuleDao.GetGatherRuleInfo(_gatherRuleName, PublishmentSystemId);

			var regexContentExclude = GatherUtility.GetRegexString(gatherRuleInfo.ContentExclude);
			var regexChannel = GatherUtility.GetRegexChannel(gatherRuleInfo.ContentChannelStart, gatherRuleInfo.ContentChannelEnd);
			var regexContent = GatherUtility.GetRegexContent(gatherRuleInfo.ContentContentStart, gatherRuleInfo.ContentContentEnd);
            var regexContent2 = string.Empty;
            if (!string.IsNullOrEmpty(gatherRuleInfo.Additional.ContentContentStart2) && !string.IsNullOrEmpty(gatherRuleInfo.Additional.ContentContentEnd2))
            {
                regexContent2 = GatherUtility.GetRegexContent(gatherRuleInfo.Additional.ContentContentStart2, gatherRuleInfo.Additional.ContentContentEnd2);
            }
            var regexContent3 = string.Empty;
            if (!string.IsNullOrEmpty(gatherRuleInfo.Additional.ContentContentStart3) && !string.IsNullOrEmpty(gatherRuleInfo.Additional.ContentContentEnd3))
            {
                regexContent3 = GatherUtility.GetRegexContent(gatherRuleInfo.Additional.ContentContentStart3, gatherRuleInfo.Additional.ContentContentEnd3);
            }
			var regexNextPage = GatherUtility.GetRegexUrl(gatherRuleInfo.ContentNextPageStart, gatherRuleInfo.ContentNextPageEnd);
			var regexTitle = GatherUtility.GetRegexTitle(gatherRuleInfo.ContentTitleStart, gatherRuleInfo.ContentTitleEnd);
            var contentAttributes = TranslateUtils.StringCollectionToStringList(gatherRuleInfo.ContentAttributes);
            var contentAttributesXML = TranslateUtils.ToNameValueCollection(gatherRuleInfo.ContentAttributesXml);

            var attributes = GatherUtility.GetContentNameValueCollection(gatherRuleInfo.Charset, contentUrl, gatherRuleInfo.CookieString, regexContentExclude, gatherRuleInfo.ContentHtmlClearCollection, gatherRuleInfo.ContentHtmlClearTagCollection, regexTitle, regexContent, regexContent2, regexContent3, regexNextPage, regexChannel, contentAttributes, contentAttributesXML);

            var builder = new StringBuilder();

            var relatedIdentities = RelatedIdentities.GetChannelRelatedIdentities(PublishmentSystemId, PublishmentSystemId);

            var styleInfoList = TableStyleManager.GetTableStyleInfoList(ETableStyle.BackgroundContent, PublishmentSystemInfo.AuxiliaryTableForContent, relatedIdentities);
            foreach (var styleInfo in styleInfoList)
            {
                if (string.IsNullOrEmpty(attributes[styleInfo.AttributeName.ToLower()])) continue;
                if (StringUtils.EqualsIgnoreCase(ContentAttribute.Title, styleInfo.AttributeName))
                {
                    builder.Append(
                        $@"<a href=""{contentUrl}"" target=""_blank"">{styleInfo.DisplayName}： {attributes[
                            styleInfo.AttributeName.ToLower()]}</a><br><br>");
                }
                else if (StringUtils.EqualsIgnoreCase(BackgroundContentAttribute.ImageUrl, styleInfo.AttributeName) || EInputTypeUtils.Equals(styleInfo.InputType, EInputType.Image))
                {
                    var imageUrl = PageUtils.GetUrlByBaseUrl(attributes[styleInfo.AttributeName.ToLower()], contentUrl);
                    builder.Append($"{styleInfo.DisplayName}： <img src='{imageUrl}' /><br><br>");
                }
                else
                {
                    builder.Append($"{styleInfo.DisplayName}： {attributes[styleInfo.AttributeName.ToLower()]}<br><br>");
                }
            }

			Content.Text = builder.ToString();
		}

		private static void ContentUrlRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
		{
			var contentUrl = e.Item.DataItem as string;
            var contentItem = (NoTagText)e.Item.FindControl("ContentItem");
            var getContent = (Button)e.Item.FindControl("GetContent");
			getContent.CommandArgument = contentUrl;

			contentItem.Text = $@"<a href=""{PageUtils.AddProtocolToUrl(contentUrl)}"" target=""_blank"">{contentUrl}</a>";
		}
	}
}
