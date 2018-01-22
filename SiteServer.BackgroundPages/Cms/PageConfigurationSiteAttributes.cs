using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Web.UI.WebControls;
using SiteServer.Utils;
using SiteServer.Utils.Model;
using SiteServer.Utils.Table;
using SiteServer.CMS.Core;
using SiteServer.BackgroundPages.Core;
using SiteServer.Plugin;

namespace SiteServer.BackgroundPages.Cms
{
	public class PageConfigurationSiteAttributes : BasePageCms
    {
		public TextBox TbPublishmentSystemName;
        public Literal LtlAttributes;
        public Button BtnSubmit;

        private List<TableStyleInfo> _styleInfoList;

        public static string GetRedirectUrl(int publishmentSystemId)
        {
            return PageUtils.GetCmsUrl(nameof(PageConfigurationSiteAttributes), new NameValueCollection
            {
                {"PublishmentSystemID", publishmentSystemId.ToString()}
            });
        }

		public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("PublishmentSystemID");

            var relatedIdentities = RelatedIdentities.GetRelatedIdentities(PublishmentSystemId, PublishmentSystemId);
            _styleInfoList = TableStyleManager.GetTableStyleInfoList(DataProvider.PublishmentSystemDao.TableName, relatedIdentities);

            if (!IsPostBack)
			{
                VerifySitePermissions(AppManager.Permissions.WebSite.Configration);

                TbPublishmentSystemName.Text = PublishmentSystemInfo.PublishmentSystemName;

                LtlAttributes.Text = GetAttributesHtml(PublishmentSystemInfo.Additional.ToNameValueCollection());

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

            var attributes = new ExtendedAttributes(formCollection);

            var builder = new StringBuilder();
            foreach (var styleInfo in _styleInfoList)
            {
                string extra;
                var value = BackgroundInputTypeParser.Parse(PublishmentSystemInfo, 0, styleInfo, attributes, pageScripts, out extra);
                if (string.IsNullOrEmpty(value) && string.IsNullOrEmpty(extra)) continue;

                if (InputTypeUtils.Equals(styleInfo.InputType, InputType.TextEditor))
                {
                    var commands = WebUtils.GetTextEditorCommands(PublishmentSystemInfo, styleInfo.AttributeName);
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
			if (Page.IsPostBack && Page.IsValid)
			{
				PublishmentSystemInfo.PublishmentSystemName = TbPublishmentSystemName.Text;
                
				try
				{
                    BackgroundInputTypeParser.SaveAttributes(PublishmentSystemInfo.Additional, PublishmentSystemInfo, _styleInfoList, Page.Request.Form, null);

                    DataProvider.PublishmentSystemDao.Update(PublishmentSystemInfo);

                    Body.AddSiteLog(PublishmentSystemId, "修改站点设置");

					SuccessMessage("站点设置修改成功！");
				}
				catch(Exception ex)
				{
                    FailMessage(ex, "站点设置修改失败！");
				}
			}
		}
	}
}
