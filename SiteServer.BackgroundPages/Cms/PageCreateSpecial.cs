using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using SiteServer.CMS.Caches;
using SiteServer.Utils;
using SiteServer.CMS.Core.Create;
using SiteServer.CMS.Database.Core;

namespace SiteServer.BackgroundPages.Cms
{
    public class PageCreateSpecial : BasePageCms
    {
        public ListBox LbSpecialIdList;

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("siteId");

            if (IsPostBack) return;

            VerifySitePermissions(ConfigManager.WebSitePermissions.Create);

            var specialInfoList = DataProvider.Special.GetSpecialInfoList(SiteId);

            foreach (var specialInfo in specialInfoList)
            {
                var listitem = new ListItem($"{specialInfo.Title}({specialInfo.Url})", specialInfo.Id.ToString());
                LbSpecialIdList.Items.Add(listitem);
            }
        }

        public void Create_OnClick(object sender, EventArgs e)
        {
            if (!Page.IsPostBack || !Page.IsValid) return;

            var specialIdList = new List<int>();
            foreach (ListItem item in LbSpecialIdList.Items)
            {
                if (!item.Selected) continue;

                var specialId = TranslateUtils.ToInt(item.Value);
                specialIdList.Add(specialId);
            }

            if (specialIdList.Count == 0)
            {
                FailMessage("请选择需要生成的专题！");
                return;
            }

            foreach (var specialId in specialIdList)
            {
                CreateManager.CreateSpecial(SiteId, specialId);
            }

            PageUtils.Redirect(AdminPagesUtils.Cms.GetCreateStatusUrl(SiteId));
        }
    }
}
