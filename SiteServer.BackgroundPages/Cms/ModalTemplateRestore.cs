using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;
using SiteServer.CMS.Core;

namespace SiteServer.BackgroundPages.Cms
{
    public class ModalTemplateRestore : BasePageCms
    {
        public PlaceHolder PhContent;
        public DropDownList DdlLogId;
        public TextBox TbContent;

        private int _templateId;
        private string _includeUrl;
        private int _logId;

        protected override bool IsSinglePage => true;

	    public static string GetOpenWindowString(int publishmentSystemId, int templateId, string includeUrl)
        {
            return LayerUtils.GetOpenScript("还原历史版本", PageUtils.GetCmsUrl(nameof(ModalTemplateRestore), new NameValueCollection
            {
                {"PublishmentSystemID", publishmentSystemId.ToString()},
                {"templateID", templateId.ToString()},
                {"includeUrl", includeUrl}
            }));
        }

		public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("PublishmentSystemID");

            _templateId = Body.GetQueryInt("templateID");
            _includeUrl = Body.GetQueryString("includeUrl");
            _logId = Body.GetQueryInt("logID");

            if (IsPostBack) return;

            var logDictionary = DataProvider.TemplateLogDao.GetLogIdWithNameDictionary(PublishmentSystemId, _templateId);
            if (logDictionary.Count > 0)
            {
                PhContent.Visible = true;
                foreach (var value in logDictionary)
                {
                    var listItem = new ListItem(value.Value, value.Key.ToString());
                    DdlLogId.Items.Add(listItem);
                }
                if (_logId > 0)
                {
                    ControlUtils.SelectSingleItem(DdlLogId, _logId.ToString());
                }

                if (_logId == 0)
                {
                    _logId = TranslateUtils.ToInt(DdlLogId.Items[0].Value);
                }
                TbContent.Text = DataProvider.TemplateLogDao.GetTemplateContent(_logId);
            }
            else
            {
                PhContent.Visible = false;
                InfoMessage("当前模板不存在历史版本，无法进行还原");
            }
        }

        public void DdlLogId_SelectedIndexChanged(object sender, EventArgs e)
        {
            PageUtils.Redirect(PageUtils.GetCmsUrl(nameof(ModalTemplateRestore), new NameValueCollection
            {
                {"PublishmentSystemID", PublishmentSystemId.ToString()},
                {"templateID", _templateId.ToString()},
                {"includeUrl", _includeUrl},
                {"logID", DdlLogId.SelectedValue}
            }));
        }

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            var templateLogId = TranslateUtils.ToInt(DdlLogId.SelectedValue);
            if (templateLogId == 0)
            {
                FailMessage("当前模板不存在历史版本，无法进行还原");
            }
            else
            {
                LayerUtils.CloseAndRedirect(Page, PageTemplateAdd.GetRedirectUrlToRestore(PublishmentSystemId, _templateId, templateLogId));
            }
        }
	}
}
