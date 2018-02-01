using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using SiteServer.Utils;
using SiteServer.BackgroundPages.Settings;
using SiteServer.CMS.Core;
using SiteServer.CMS.Core.Create;
using SiteServer.Plugin;

namespace SiteServer.BackgroundPages.Cms
{
    public class PageCreateFile : BasePageCms
    {
        public ListBox LbTemplateIdList;
        public Button BtnDeleteAll;

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("siteId");

            if (IsPostBack) return;

            VerifySitePermissions(ConfigManager.Permissions.WebSite.Create);

            var templateInfoList = DataProvider.TemplateDao.GetTemplateInfoListOfFile(SiteId);

            foreach (var templateInfo in templateInfoList)
            {
                var listitem = new ListItem(templateInfo.CreatedFileFullName, templateInfo.Id.ToString());
                LbTemplateIdList.Items.Add(listitem);
            }

            BtnDeleteAll.Attributes.Add("onclick", "return confirm(\"此操作将删除所有已生成的文件页面，确定吗？\");");
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

            PageCreateStatus.Redirect(SiteId);
        }

        public void BtnDeleteAll_OnClick(object sender, EventArgs e)
        {
            if (!Page.IsPostBack || !Page.IsValid) return;

            var url = PageProgressBar.GetDeleteAllPageUrl(SiteId, TemplateType.FileTemplate);
            PageUtils.RedirectToLoadingPage(url);
        }
    }
}
