using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;
using SiteServer.CMS.Core;

namespace SiteServer.BackgroundPages.Cms
{
    public class ModalTemplateRestore : BasePageCms
    {
        public DropDownList DdlLogId;
        public TextBox TbContent;

        private int _templateId;
        private string _includeUrl;
        private int _logId;

        protected override bool IsSinglePage => true;

	    public static string GetOpenLayerString(int publishmentSystemId, int templateId, string includeUrl)
        {
            return PageUtils.GetOpenLayerString("还原历史版本", PageUtils.GetCmsUrl(nameof(ModalTemplateRestore), new NameValueCollection
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
           
			if (!IsPostBack)
			{
                var logDictionary = DataProvider.TemplateLogDao.GetLogIdWithNameDictionary(PublishmentSystemId, _templateId);
                foreach (var value in logDictionary)
                {
                    var listItem = new ListItem(value.Value, value.Key.ToString());
                    DdlLogId.Items.Add(listItem);
                }
                if (_logId > 0)
                {
                    ControlUtils.SelectListItems(DdlLogId, _logId.ToString());
                }

                if (DdlLogId.Items.Count > 0)
                {
                    if (_logId == 0)
                    {
                        _logId = TranslateUtils.ToInt(DdlLogId.Items[0].Value);
                    }
                    TbContent.Text = DataProvider.TemplateLogDao.GetTemplateContent(_logId);
                }
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
                PageUtils.CloseModalPageAndRedirect(Page, PageTemplateAdd.GetRedirectUrlToRestore(PublishmentSystemId, _templateId, templateLogId));
            }
        }
	}
}
