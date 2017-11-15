using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;
using SiteServer.BackgroundPages.Cms;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;
using SiteServer.CMS.StlParser.StlElement;
using SiteServer.CMS.StlParser.Utility;

namespace SiteServer.BackgroundPages.Plugins
{
	public class PageInputPreview : BasePageCms
    {
        public Literal LtlInputName;
        public Literal LtlInputCode;
        public Literal LtlForm;

        private InputInfo _inputInfo;

        public static string GetRedirectUrl(int publishmentSystemId, int inputId, string returnUrl)
        {
            return PageUtils.GetPluginsUrl(nameof(PageInputPreview), new NameValueCollection
            {
                {"PublishmentSystemID", publishmentSystemId.ToString()},
                {"InputID", inputId.ToString()},
                {"ReturnUrl", StringUtils.ValueToUrl(returnUrl)}
            });
        }

        public string GetEditUrl()
        {
            return ModalInputAdd.GetOpenWindowStringToEdit(PublishmentSystemId, _inputInfo.InputId, true);
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("InputID");

            var inputId = Body.GetQueryInt("InputID"); 
            _inputInfo = DataProvider.InputDao.GetInputInfo(inputId);

			if (!IsPostBack)
			{
                LtlInputName.Text = _inputInfo.InputName;

			    var stlElement = StlInput.GetDefaultStlInputStlElement(PublishmentSystemInfo, _inputInfo);
                LtlInputCode.Text = StringUtils.ReplaceNewlineToBr(StringUtils.HtmlEncode(stlElement));

                LtlForm.Text = StlParserManager.ParsePreviewContent(PublishmentSystemInfo, stlElement);

                InfoMessage("预览提交表单无法提交信息，如需提交信息请到提交表单管理中进行操作");
                
                //if (string.IsNullOrEmpty(this.inputInfo.Template))
                //{
                //    InputTemplate inputTemplate = new InputTemplate(base.PublishmentSystemID, this.inputInfo);
                //    this.ltlForm.Text = inputTemplate.GetTemplate();
                //}
                //else
                //{
                //    this.ltlForm.Text = this.inputInfo.Template;
                //}
			}
		}
	}
}
