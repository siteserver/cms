using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Model.Enumerations;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;
using SiteServer.CMS.StlParser.StlElement;
using SiteServer.CMS.StlParser.Utility;

namespace SiteServer.BackgroundPages.Cms
{
	public class PageTagStyleTemplate : BasePageCms
    {
        public Literal ltlStyleName;
        public Literal ltlElement;
        public RadioButtonList rblIsTemplate;

        public PlaceHolder phTemplate;
        public CheckBox cbIsCreateTemplate;
		public TextBox tbContent;
        public PlaceHolder phSuccess;
        public TextBox tbSuccess;
        public PlaceHolder phFailure;
        public TextBox tbFailure;
        public PlaceHolder phStyle;
        public TextBox tbStyle;
        public PlaceHolder phScript;
        public TextBox tbScript;

        public Button Preview;
        public PlaceHolder phReturn;

        private TagStyleInfo _styleInfo;

        public static string GetRedirectUrl(int publishmentSystemId, int styleId, string returnUrl)
        {
            return PageUtils.GetCmsUrl(nameof(PageTagStyleTemplate), new NameValueCollection
            {
                {"PublishmentSystemID", publishmentSystemId.ToString()},
                {"StyleID", styleId.ToString()},
                {"ReturnUrl", StringUtils.ValueToUrl(returnUrl)}
            });
        }

		public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("StyleID");

            var styleID = Body.GetQueryInt("StyleID");
            _styleInfo = DataProvider.TagStyleDao.GetTagStyleInfo(styleID);

			if (!IsPostBack)
			{
                ltlStyleName.Text = _styleInfo.StyleName;
                ltlElement.Text = $@"
&lt;{_styleInfo.ElementName} styleName=&quot;{_styleInfo.StyleName}&quot;&gt;&lt;/{_styleInfo.ElementName}&gt;";
                if (StringUtils.EqualsIgnoreCase(_styleInfo.ElementName, StlGovInteractApply.ElementName))
                {
                    var nodeID = DataProvider.GovInteractChannelDao.GetNodeIdByApplyStyleId(_styleInfo.StyleID);
                    var nodeName = NodeManager.GetNodeName(PublishmentSystemId, nodeID);
                    ltlStyleName.Text = nodeName;
                    ltlElement.Text =
                        $@"&lt;{_styleInfo.ElementName} interactName=&quot;{nodeName}&quot;&gt;&lt;/{_styleInfo
                            .ElementName}&gt;";
                }
                else if (StringUtils.EqualsIgnoreCase(_styleInfo.ElementName, StlGovInteractQuery.ElementName))
                {
                    var nodeID = DataProvider.GovInteractChannelDao.GetNodeIdByQueryStyleId(_styleInfo.StyleID);
                    var nodeName = NodeManager.GetNodeName(PublishmentSystemId, nodeID);
                    ltlStyleName.Text = nodeName;
                    ltlElement.Text =
                        $@"&lt;{_styleInfo.ElementName} interactName=&quot;{nodeName}&quot;&gt;&lt;/{_styleInfo
                            .ElementName}&gt;";
                }

                EBooleanUtils.AddListItems(rblIsTemplate, "自定义模板", "默认模板");
                ControlUtils.SelectListItemsIgnoreCase(rblIsTemplate, _styleInfo.IsTemplate.ToString());
                phTemplate.Visible = _styleInfo.IsTemplate;

			    var previewUrl = PageTagStylePreview.GetRedirectUrl(PublishmentSystemId, _styleInfo.StyleID,
			        Body.GetQueryString("ReturnUrl"));
                Preview.Attributes.Add("onclick", $"location.href='{previewUrl}';return false;");

                phSuccess.Visible = TagStyleUtility.IsSuccessVisible(_styleInfo.ElementName);
                phFailure.Visible = TagStyleUtility.IsFailureVisible(_styleInfo.ElementName);
                phStyle.Visible = TagStyleUtility.IsStyleVisible(_styleInfo.ElementName);
                phScript.Visible = TagStyleUtility.IsScriptVisible(_styleInfo.ElementName);

                if (_styleInfo.IsTemplate)
                {
                    tbContent.Text = _styleInfo.ContentTemplate;
                    if (phSuccess.Visible)
                    {
                        tbSuccess.Text = _styleInfo.SuccessTemplate;
                    }
                    if (phFailure.Visible)
                    {
                        tbFailure.Text = _styleInfo.FailureTemplate;
                    }
                    if (phStyle.Visible)
                    {
                        tbStyle.Text = _styleInfo.StyleTemplate;
                    }
                    if (phScript.Visible)
                    {
                        tbScript.Text = _styleInfo.ScriptTemplate;
                    }
                }

                if (string.IsNullOrEmpty(Body.GetQueryString("ReturnUrl")))
                {
                    phReturn.Visible = false;
                }
			}
		}

        public void rblIsTemplate_SelectedIndexChanged(object sender, EventArgs e)
        {
            phTemplate.Visible = TranslateUtils.ToBool(rblIsTemplate.SelectedValue);
            if (phTemplate.Visible && string.IsNullOrEmpty(tbContent.Text))
            {
                cbIsCreateTemplate_CheckedChanged(sender, e);
            }
        }

        public void cbIsCreateTemplate_CheckedChanged(object sender, EventArgs e)
        {
            TagStyleUtility.IsCreateTemplate_CheckedChanged(_styleInfo, PublishmentSystemInfo, tbContent, tbSuccess, tbFailure, tbStyle, tbScript);
            cbIsCreateTemplate.Checked = false;
        }


        public override void Submit_OnClick(object sender, EventArgs e)
		{
			if (Page.IsPostBack && Page.IsValid)
			{
                _styleInfo.IsTemplate = TranslateUtils.ToBool(rblIsTemplate.SelectedValue);
                _styleInfo.StyleTemplate = tbStyle.Text;
                _styleInfo.ScriptTemplate = tbScript.Text;
                _styleInfo.ContentTemplate = tbContent.Text;
                _styleInfo.SuccessTemplate = tbSuccess.Text;
                _styleInfo.FailureTemplate = tbFailure.Text;
                
                try
                {
                    DataProvider.TagStyleDao.Update(_styleInfo);
                    SuccessMessage("模板修改成功！");
                }
                catch (Exception ex)
                {
                    FailMessage(ex, "模板修改失败," + ex.Message);
                }
			}
		}
	}
}
