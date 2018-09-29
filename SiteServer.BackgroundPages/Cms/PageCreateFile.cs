using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using SiteServer.Utils;
using SiteServer.BackgroundPages.Settings;
using SiteServer.CMS.Core;
using SiteServer.CMS.Core.Create;
using SiteServer.CMS.DataCache;
using SiteServer.Plugin;

namespace SiteServer.BackgroundPages.Cms
{
    public class PageCreateFile : BasePageCms
    {
        public ListBox LbTemplateIdList;

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("siteId");

            if (IsPostBack) return;

            VerifySitePermissions(ConfigManager.WebSitePermissions.Create);

            var templateInfoList = DataProvider.TemplateDao.GetTemplateInfoListOfFile(SiteId);

            foreach (var templateInfo in templateInfoList)
            {
                var listitem = new ListItem(templateInfo.CreatedFileFullName, templateInfo.Id.ToString());
                LbTemplateIdList.Items.Add(listitem);
            }
        }

        public void Create_OnClick(object sender, EventArgs e)
        {
            if (!Page.IsPostBack || !Page.IsValid) return;

            var templateIdList = new List<int>();
            foreach (ListItem item in LbTemplateIdList.Items)
            {
                if (!item.Selected) continue;

                var templateId = int.Parse(item.Value);
                templateIdList.Add(templateId);
            }

            if (templateIdList.Count == 0)
            {
                FailMessage("请选择需要生成的文件页！");
                return;
            }

            foreach (var templateId in templateIdList)
            {
                CreateManager.CreateFile(SiteId, templateId);
            }

            PageUtils.Redirect(CmsPages.GetCreateStatusUrl(SiteId));
        }
    }
}
