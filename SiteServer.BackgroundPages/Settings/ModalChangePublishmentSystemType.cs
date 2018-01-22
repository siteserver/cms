using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using SiteServer.Utils;
using SiteServer.Utils.IO.FileManagement;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model.Enumerations;

namespace SiteServer.BackgroundPages.Settings
{
    public class ModalChangePublishmentSystemType : BasePageCms
    {
        protected PlaceHolder PhChangeToSite;
        protected TextBox TbPublishmentSystemDir;
        protected CheckBoxList CblFilesToSite;

        protected PlaceHolder PhChangeToHeadquarters;
        protected DropDownList DdlIsMoveFiles;

        public Button BtnSubmit;

        private bool _isHeadquarters;

        public static string GetOpenWindowString(int publishmentSystemId)
        {
            var publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(publishmentSystemId);
            var title = publishmentSystemInfo.IsHeadquarters ? "转移到子目录" : "转移到根目录";
            return LayerUtils.GetOpenScript(title,
                PageUtils.GetSettingsUrl(nameof(ModalChangePublishmentSystemType),
                    new NameValueCollection
                    {
                        {"PublishmentSystemID", publishmentSystemId.ToString()}
                    }));
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("PublishmentSystemID");

            _isHeadquarters = PublishmentSystemInfo.IsHeadquarters;

            var selectedList = new List<string>();

            if (Page.IsPostBack) return;

            if (_isHeadquarters)
            {
                InfoMessage($"将站点{PublishmentSystemInfo.PublishmentSystemName}转移到子目录");

                PhChangeToSite.Visible = true;
                PhChangeToHeadquarters.Visible = false;
                var fileSystems = FileManager.GetFileSystemInfoExtendCollection(WebConfigUtils.PhysicalApplicationPath, true);
                var publishmentSystemDirList = DataProvider.PublishmentSystemDao.GetLowerPublishmentSystemDirListThatNotIsHeadquarters();
                foreach (FileSystemInfoExtend fileSystem in fileSystems)
                {
                    if (fileSystem.IsDirectory)
                    {
                        if (!DirectoryUtils.IsSystemDirectory(fileSystem.Name) && !publishmentSystemDirList.Contains(fileSystem.Name.ToLower()))
                        {
                            CblFilesToSite.Items.Add(new ListItem(fileSystem.Name, fileSystem.Name));
                        }
                    }
                    else
                    {
                        if (!PathUtility.IsSystemFileForChangePublishmentSystemType(fileSystem.Name))
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
                var fileTemplateInfoList = DataProvider.TemplateDao.GetTemplateInfoListByType(PublishmentSystemId, ETemplateType.FileTemplate);
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
                InfoMessage($"将站点{PublishmentSystemInfo.PublishmentSystemName}转移到根目录");

                var headquartersExists = false;
                var publishmentSystemIdList = PublishmentSystemManager.GetPublishmentSystemIdList();
                foreach (var psId in publishmentSystemIdList)
                {
                    var psInfo = PublishmentSystemManager.GetPublishmentSystemInfo(psId);
                    if (psInfo.IsHeadquarters)
                    {
                        headquartersExists = true;
                        break;
                    }
                }
                if (headquartersExists)
                {
                    FailMessage($"根目录站点已经存在，站点{PublishmentSystemInfo.PublishmentSystemName}不能转移到根目录");
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
                var list = DataProvider.NodeDao.GetLowerSystemDirList(PublishmentSystemInfo.ParentPublishmentSystemId);
                if (list.IndexOf(TbPublishmentSystemDir.Text.Trim().ToLower()) != -1)
                {
                    FailMessage("操作失败，已存在相同的发布路径");
                    return;
                }
                if (!DirectoryUtils.IsDirectoryNameCompliant(TbPublishmentSystemDir.Text))
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
                DirectoryUtility.ChangeToSubSite(PublishmentSystemInfo, TbPublishmentSystemDir.Text, filesToSite);
            }
            else
            {
                DirectoryUtility.ChangeToHeadquarters(PublishmentSystemInfo, TranslateUtils.ToBool(DdlIsMoveFiles.SelectedValue));
            }

            Body.AddAdminLog(_isHeadquarters ? "转移到子目录" : "转移到根目录",
                $"站点:{PublishmentSystemInfo.PublishmentSystemName}");
            LayerUtils.Close(Page);
        }
    }
}
