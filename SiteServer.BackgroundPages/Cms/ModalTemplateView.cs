using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using SiteServer.BackgroundPages.Core;
using SiteServer.Utils;
using SiteServer.CMS.Database.Core;
using SiteServer.CMS.Fx;

namespace SiteServer.BackgroundPages.Cms
{
	public class ModalTemplateView : BasePageCms
    {
        public TextBox TbContent;

        public static string GetOpenWindowString(int siteId, int templateLogId)
        {
            return LayerUtils.GetOpenScript("查看修订内容", FxUtils.GetCmsUrl(siteId, nameof(ModalTemplateView), new NameValueCollection
            {
                {"templateLogID", templateLogId.ToString()}
            }));
        }

		public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            FxUtils.CheckRequestParameter("siteId");
           
			if (!IsPostBack)
			{
                var templateLogId = AuthRequest.GetQueryInt("templateLogID");
                TbContent.Text = DataProvider.TemplateLog.GetTemplateContent(templateLogId);
			}
		}
	}
}
