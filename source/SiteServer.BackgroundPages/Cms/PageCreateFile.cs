using System;
using System.Collections;
using System.Web.UI.WebControls;
using BaiRong.Core;
using SiteServer.BackgroundPages.Service;
using SiteServer.CMS.Core;
using SiteServer.CMS.Core.Create;
using SiteServer.CMS.Model.Enumerations;

namespace SiteServer.BackgroundPages.Cms
{
    public class PageCreateFile : BasePageCms
    {
        public ListBox FileCollectionToCreate;
        public Button CreateFileButton;
        public Button DeleteAllFileButton;

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("PublishmentSystemID");

            if (!IsPostBack)
            {
                BreadCrumb(AppManager.Cms.LeftMenu.IdCreate, "生成文件页", AppManager.Cms.Permission.WebSite.Create);

                var templateInfoList = DataProvider.TemplateDao.GetTemplateInfoListOfFile(PublishmentSystemId);

                foreach (var templateInfo in templateInfoList)
                {
                    var listitem = new ListItem(templateInfo.CreatedFileFullName, templateInfo.TemplateId.ToString());
                    FileCollectionToCreate.Items.Add(listitem);
                }

                DeleteAllFileButton.Attributes.Add("onclick", "return confirm(\"此操作将删除所有已生成的文件页面，确定吗？\");");
            }
        }

        public void CreateFileButton_OnClick(object sender, EventArgs e)
        {
            if (Page.IsPostBack && Page.IsValid)
            {
                var templateIdArrayList = new ArrayList();
                foreach (ListItem item in FileCollectionToCreate.Items)
                {
                    if (item.Selected)
                    {
                        var templateId = int.Parse(item.Value);
                        templateIdArrayList.Add(templateId);
                    }
                }
                ProcessCreateFile(templateIdArrayList);
            }
        }

        public void DeleteAllFileButton_OnClick(object sender, EventArgs e)
        {
            if (Page.IsPostBack && Page.IsValid)
            {
                var url = PageProgressBar.GetDeleteAllPageUrl(PublishmentSystemId, ETemplateType.FileTemplate);
                PageUtils.RedirectToLoadingPage(url);
            }
        }

        private void ProcessCreateFile(ICollection templateIdArrayList)
        {
            if (templateIdArrayList.Count == 0)
            {
                FailMessage("请选择需要生成的文件页！");
                return;
            }

            foreach (int templateId in templateIdArrayList)
            {
                CreateManager.CreateFile(PublishmentSystemId, templateId);
            }

            PageCreateStatus.Redirect(PublishmentSystemId);
        }

    }
}
