using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web.UI.WebControls;
using SiteServer.Utils;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.Plugin;
using SiteServer.Utils.IO;

namespace SiteServer.BackgroundPages.Settings
{
    public class ModalChangeSiteType : BasePageCms
    {
        protected PlaceHolder PhChangeToSite;
        protected TextBox TbSiteDir;
        protected CheckBoxList CblFilesToSite;

        protected PlaceHolder PhChangeToHeadquarters;
        protected DropDownList DdlIsMoveFiles;

        public Button BtnSubmit;

        private bool _isHeadquarters;

        public static string GetOpenWindowString(int siteId)
        {
            var site = SiteManager.GetSiteAsync(siteId).GetAwaiter().GetResult();
            var title = site.Root ? "转移到子目录" : "转移到根目录";
            return LayerUtils.GetOpenScript(title,
                PageUtils.GetSettingsUrl(nameof(ModalChangeSiteType),
                    new NameValueCollection
                    {
                        {"SiteId", siteId.ToString()}
                    }));
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("siteId");

            _isHeadquarters = Site.Root;

            var selectedList = new List<string>();

            if (Page.IsPostBack) return;

            if (_isHeadquarters)
            {
                InfoMessage($"将站点{Site.SiteName}转移到子目录");

                PhChangeToSite.Visible = true;
                PhChangeToHeadquarters.Visible = false;
                var fileSystems = FileManager.GetFileSystemInfoExtendCollection(WebConfigUtils.PhysicalApplicationPath, true);
                var siteDirList = DataProvider.SiteDao.GetLowerSiteDirListThatNotIsRootAsync().GetAwaiter().GetResult();
                foreach (FileSystemInfoExtend fileSystem in fileSystems)
                {
                    if (fileSystem.IsDirectory)
                    {
                        if (!DirectoryUtils.IsSystemDirectory(fileSystem.Name) && !siteDirList.Contains(fileSystem.Name.ToLower()))
                        {
                            CblFilesToSite.Items.Add(new ListItem(fileSystem.Name, fileSystem.Name));
                        }
                    }
                    else
                    {
                        if (!PathUtility.IsSystemFileForChangeSiteType(fileSystem.Name))
                        {
                            CblFilesToSite.Items.Add(new ListItem(fileSystem.Name, fileSystem.Name));
                        }
                    }

                    if (PathUtility.IsWebSiteFile(fileSystem.Name) || DirectoryUtils.IsWebSiteDirectory(fileSystem.Name))
                    {
                        selectedList.Add(fileSystem.Name);
                    }
                }

                //主站下的单页模板
                var fileTemplateInfoList = DataProvider.TemplateDao.GetTemplateInfoListByType(SiteId, TemplateType.FileTemplate);
                foreach (var fileT in fileTemplateInfoList)
                {
                    if (fileT.CreatedFileFullName.StartsWith("@/") || fileT.CreatedFileFullName.StartsWith("~/"))
                    {
                        var arr = fileT.CreatedFileFullName.Substring(2).Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
                        if (arr.Length > 0)
                            selectedList.Add(arr[0]);
                    }
                }
            }
            else
            {
                InfoMessage($"将站点{Site.SiteName}转移到根目录");

                var headquartersExists = false;
                var siteIdList = SiteManager.GetSiteIdListAsync().GetAwaiter().GetResult();
                foreach (var psId in siteIdList)
                {
                    var psInfo = SiteManager.GetSiteAsync(psId).GetAwaiter().GetResult();
                    if (psInfo.Root)
                    {
                        headquartersExists = true;
                        break;
                    }
                }
                if (headquartersExists)
                {
                    FailMessage($"根目录站点已经存在，站点{Site.SiteName}不能转移到根目录");
                    BtnSubmit.Visible = false;
                    PhChangeToSite.Visible = false;
                    PhChangeToHeadquarters.Visible = false;
                }
                else
                {
                    PhChangeToSite.Visible = false;
                    PhChangeToHeadquarters.Visible = true;
                }
            }

            //设置选中的文件以及文件夹
            ControlUtils.SelectMultiItems(CblFilesToSite, selectedList);
        }

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            if (_isHeadquarters)
            {
                var list = DataProvider.SiteDao.GetLowerSiteDirListAsync(Site.ParentId).GetAwaiter().GetResult();
                if (list.Contains(TbSiteDir.Text.Trim().ToLower()))
                {
                    FailMessage("操作失败，已存在相同的发布路径");
                    return;
                }
                if (!DirectoryUtils.IsDirectoryNameCompliant(TbSiteDir.Text))
                {
                    FailMessage("操作失败，文件夹名称不符合要求");
                    return;
                }
                var filesToSite = new ArrayList();
                foreach (ListItem item in CblFilesToSite.Items)
                {
                    if (item.Selected)
                    {
                        filesToSite.Add(item.Value);
                    }
                }
                DirectoryUtility.ChangeToSubSiteAsync(Site, TbSiteDir.Text, filesToSite).GetAwaiter().GetResult();
            }
            else
            {
                DirectoryUtility.ChangeToHeadquartersAsync(Site, TranslateUtils.ToBool(DdlIsMoveFiles.SelectedValue)).GetAwaiter().GetResult();
            }

            AuthRequest.AddAdminLogAsync(_isHeadquarters ? "转移到子目录" : "转移到根目录",
                $"站点:{Site.SiteName}").GetAwaiter().GetResult();
            LayerUtils.Close(Page);
        }
    }
}
