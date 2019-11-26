using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Web.UI.WebControls;
using SiteServer.Utils;
using SiteServer.CMS.Core;
using SiteServer.BackgroundPages.Core;
using SiteServer.CMS.Context;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Plugin.Impl;
using SiteServer.Plugin;
using TableStyle = SiteServer.CMS.Model.TableStyle;
using WebUtils = SiteServer.BackgroundPages.Core.WebUtils;

namespace SiteServer.BackgroundPages.Cms
{
	public class PageConfigurationSiteAttributes : BasePageCms
    {
		public TextBox TbSiteName;
        public Literal LtlAttributes;
        public Button BtnSubmit;

        private List<TableStyle> _styleList;

        public static string GetRedirectUrl(int siteId)
        {
            return PageUtils.GetCmsUrl(siteId, nameof(PageConfigurationSiteAttributes), null);
        }

		public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("siteId");

            _styleList = TableStyleManager.GetSiteStyleListAsync(SiteId).GetAwaiter().GetResult();

            if (!IsPostBack)
			{
                VerifySitePermissions(Constants.WebSitePermissions.Configuration);

                TbSiteName.Text = Site.SiteName;

			    var nameValueCollection = TranslateUtils.DictionaryToNameValueCollection(Site.ToDictionary());

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

            if (_styleList == null) return string.Empty;

            var attributes = TranslateUtils.NameValueCollectionToDictionary(formCollection);

            var builder = new StringBuilder();
            foreach (var style in _styleList)
            {
                string extra;
                var value = BackgroundInputTypeParser.Parse(Site, 0, style, attributes, pageScripts, out extra);
                if (string.IsNullOrEmpty(value) && string.IsNullOrEmpty(extra)) continue;

                if (InputTypeUtils.Equals(style.InputType, InputType.TextEditor))
                {
                    var commands = WebUtils.GetTextEditorCommands(Site, style.AttributeName);
                    builder.Append($@"
<div class=""form-group"">
    <label class=""control-label"">{style.DisplayName}</label>
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
    <label class=""control-label"">{style.DisplayName}</label>
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

		    Site.SiteName = TbSiteName.Text;

            var dict = BackgroundInputTypeParser.SaveAttributesAsync(Site, _styleList, Page.Request.Form, null).GetAwaiter().GetResult();

            foreach (var o in dict)
            {
                Site.Set(o.Key, o.Value);
            }

            DataProvider.SiteDao.UpdateAsync(Site).GetAwaiter().GetResult();

            AuthRequest.AddSiteLogAsync(SiteId, "修改站点设置").GetAwaiter().GetResult();

            SuccessMessage("站点设置修改成功！");
        }
	}
}
