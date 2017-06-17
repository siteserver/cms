using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;
using SiteServer.CMS.StlParser.StlElement;
using SiteServer.CMS.StlParser.Utility;

namespace SiteServer.BackgroundPages.Cms
{
	public class PageTagStylePreview : BasePageCms
    {
        public Literal ltlStyleName;
        public Literal ltlElement;
        public Literal ltlForm;

        private TagStyleInfo _styleInfo;

        public static string GetRedirectUrl(int publishmentSystemId, int styleId, string returnUrl)
        {
            return PageUtils.GetCmsUrl(nameof(PageTagStylePreview), new NameValueCollection
            {
                {"PublishmentSystemID", publishmentSystemId.ToString()},
                {"StyleID", styleId.ToString()},
                {"ReturnUrl", StringUtils.ValueToUrl(returnUrl)}
            });
        }

        public string GetTemplateUrl()
        {
            var urlTemplate = PageTagStyleTemplate.GetRedirectUrl(PublishmentSystemId, _styleInfo.StyleID,
                Body.GetQueryString("ReturnUrl"));
            return $"location.href='{urlTemplate}';return false;";
        }

		public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("StyleID");

            var styleId = Body.GetQueryInt("StyleID");
            _styleInfo = DataProvider.TagStyleDao.GetTagStyleInfo(styleId);

			if (!IsPostBack)
			{
                ltlStyleName.Text = _styleInfo.StyleName;

                string stlElement =
                    $@"<{_styleInfo.ElementName} styleName=""{_styleInfo.StyleName}""></{_styleInfo
                        .ElementName}>";
                if (StringUtils.EqualsIgnoreCase(_styleInfo.ElementName, StlGovInteractApply.ElementName))
                {
                    var nodeId = DataProvider.GovInteractChannelDao.GetNodeIdByApplyStyleId(_styleInfo.StyleID);
                    var nodeName = NodeManager.GetNodeName(PublishmentSystemId, nodeId);
                    ltlStyleName.Text = nodeName;
                    stlElement =
                        $@"<{_styleInfo.ElementName} interactName=""{nodeName}""></{_styleInfo.ElementName}>";
                }
                else if (StringUtils.EqualsIgnoreCase(_styleInfo.ElementName, StlGovInteractQuery.ElementName))
                {
                    var nodeId = DataProvider.GovInteractChannelDao.GetNodeIdByQueryStyleId(_styleInfo.StyleID);
                    var nodeName = NodeManager.GetNodeName(PublishmentSystemId, nodeId);
                    ltlStyleName.Text = nodeName;
                    stlElement =
                        $@"<{_styleInfo.ElementName} interactName=""{nodeName}""></{_styleInfo.ElementName}>";
                }

                ltlElement.Text = StringUtils.HtmlEncode(stlElement);

                ltlForm.Text = StlParserManager.ParsePreviewContent(PublishmentSystemInfo, stlElement);
			}
		}
	}
}
