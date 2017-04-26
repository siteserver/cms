using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;
using SiteServer.BackgroundPages.Controls;
using SiteServer.CMS.Core;

namespace SiteServer.BackgroundPages.Cms
{
	public class ModalSeoMetaView : BasePageCms
    {
		protected NoTagText MetaName;
		protected TextBox MetaCode;

        public static string GetOpenWindowString(int publishmentSystemId, int seoMetaId)
        {
            return PageUtils.GetOpenWindowString("页面元数据源代码查看", PageUtils.GetCmsUrl(nameof(ModalSeoMetaView), new NameValueCollection
            {
                {"PublishmentSystemID", publishmentSystemId.ToString()},
                {"SeoMetaID", seoMetaId.ToString()}
            }), 480, 360, true);
        }

		public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            if (IsPostBack) return;

            if (!Body.IsQueryExists("SeoMetaID")) return;
            var seoMetaId = Body.GetQueryInt("SeoMetaID");
            var seoMetaInfo = DataProvider.SeoMetaDao.GetSeoMetaInfo(seoMetaId);
            if (seoMetaInfo == null) return;

            MetaName.Text = seoMetaInfo.SeoMetaName;
            MetaCode.Text = SeoManager.GetMetaContent(seoMetaInfo);
        }
	}
}
