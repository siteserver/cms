using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Web.UI.WebControls;
using SiteServer.Utils;
using SiteServer.CMS.Core;
using SiteServer.BackgroundPages.Core;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Model;
using SiteServer.CMS.Model.Attributes;
using SiteServer.CMS.Plugin.Impl;
using SiteServer.Plugin;

namespace SiteServer.BackgroundPages.Cms
{
	public class PageConfigurationSiteAttributes : BasePageCms
    {
		public TextBox TbSiteName;
        public Literal LtlAttributes;
        public Button BtnSubmit;

        private List<TableStyleInfo> _styleInfoList;

        public static string GetRedirectUrl(int siteId)
        {
            return PageUtils.GetCmsUrl(siteId, nameof(PageConfigurationSiteAttributes), null);
        }

		public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("siteId");

            _styleInfoList = TableStyleManager.GetSiteStyleInfoList(SiteId);

            if (!IsPostBack)
			{
                VerifySitePermissions(ConfigManager.WebSitePermissions.Configration);

                TbSiteName.Text = SiteInfo.SiteName;

			    var nameValueCollection = TranslateUtils.DictionaryToNameValueCollection(SiteInfo.Additional.ToDictionary());

                LtlAttributes.Text = GetAttributesHtml(nameValueCollection);

                BtnSubmit.Attributes.Add("onclick", InputParserUtils.GetValidateSubmitOnClickScript("myForm"));
            }
            else
            {
                LtlAttributes.Text = GetAttributesHtml(Request.Form);
            }
		}

        private string GetAttributesHtml(NameValueCollection formCollection)
        {
            if (formCollection == null)
            {
                formCollection = Request.Form.Count > 0 ? Request.Form : new NameValueCollection();
            }

            var pageScripts = new NameValueCollection();

            if (_styleInfoList == null) return string.Empty;

            var attributes = new AttributesImpl(formCollection);

            var builder = new StringBuilder();
            foreach (var styleInfo in _styleInfoList)
            {
                string extra;
                var value = BackgroundInputTypeParser.Parse(SiteInfo, 0, styleInfo, attributes, pageScripts, out extra);
                if (string.IsNullOrEmpty(value) && string.IsNullOrEmpty(extra)) continue;

                if (InputTypeUtils.Equals(styleInfo.InputType, InputType.TextEditor))
                {
                    var commands = WebUtils.GetTextEditorCommands(SiteInfo, styleInfo.AttributeName);
                    builder.Append($@"
<div class=""form-group"">
    <label class=""control-label"">{styleInfo.DisplayName}</label>
    {commands}
    <hr />
    {value}
    {extra}
</div>");
                }
                else
                {
                    builder.Append($@"
<div class=""form-group"">
    <label class=""control-label"">{styleInfo.DisplayName}</label>
    {value}
    {extra}
</div>");
                }
            }

            foreach (string key in pageScripts.Keys)
            {
                builder.Append(pageScripts[key]);
            }

            return builder.ToString();
        }

        public override void Submit_OnClick(object sender, EventArgs e)
		{
		    if (!Page.IsPostBack || !Page.IsValid) return;

		    SiteInfo.SiteName = TbSiteName.Text;

            var dict = BackgroundInputTypeParser.SaveAttributes(SiteInfo, _styleInfoList, Page.Request.Form, null);

		    SiteInfo.Additional.Load(dict);

            DataProvider.SiteDao.Update(SiteInfo);

            AuthRequest.AddSiteLog(SiteId, "修改站点设置");

            SuccessMessage("站点设置修改成功！");
        }
	}
}
