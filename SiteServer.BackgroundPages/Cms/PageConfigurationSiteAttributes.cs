using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Model;
using BaiRong.Core.Table;
using SiteServer.CMS.Core;
using SiteServer.BackgroundPages.Core;
using SiteServer.Plugin.Models;

namespace SiteServer.BackgroundPages.Cms
{
	public class PageConfigurationSiteAttributes : BasePageCms
    {
		public TextBox TbPublishmentSystemName;
        public Literal LtlAttributes;
        public Literal LtlSettings;
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

                LtlSettings.Text =
                    $@"<a class=""btn btn-success"" href=""{PageTableStyle.GetRedirectUrl(PublishmentSystemId, DataProvider.PublishmentSystemDao.TableName, PublishmentSystemId, GetRedirectUrl(PublishmentSystemId))}"">设置站点属性</a>";

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
    <label class=""col-sm-3 control-label"">{styleInfo.DisplayName}</label>
    <div class=""col-sm-8"">
        {commands}
        <hr />
        {value}
    </div>
    <div class=""col-sm-1"">
        {extra}
    </div>
</div>");
                }
                else
                {
                    builder.Append($@"
<div class=""form-group"">
    <label class=""col-sm-3 control-label"">{styleInfo.DisplayName}</label>
    <div class=""col-sm-3"">
        {value}
    </div>
    <div class=""col-sm-6"">
        {extra}
    </div>
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
