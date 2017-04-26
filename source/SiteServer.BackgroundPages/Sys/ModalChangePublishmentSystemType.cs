using System;
using System.Collections;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.IO.FileManagement;
using BaiRong.Core.Text;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;
using SiteServer.CMS.Model.Enumerations;

namespace SiteServer.BackgroundPages.Sys
{
    public class ModalChangePublishmentSystemType : BasePageCms
    {
        protected PlaceHolder HeadquartersExists;

        protected PlaceHolder ChangeToSite;
        protected TextBox PublishmentSystemDir;
        protected RegularExpressionValidator PublishmentSystemDirValidator;
        protected CheckBoxList FilesToSite;

        protected PlaceHolder ChangeToHeadquarters;
        protected RadioButtonList IsMoveFiles;

        private bool _isHeadquarters;

        public string SiteName;

        public static string GetOpenWindowString(int publishmentSystemId)
        {
            var publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(publishmentSystemId);
            var title = publishmentSystemInfo.IsHeadquarters ? "转移到子目录" : "转移到根目录";
            return PageUtils.GetOpenWindowString(title,
                PageUtils.GetSysUrl(nameof(ModalChangePublishmentSystemType),
                    new NameValueCollection
                    {
                        {"PublishmentSystemID", publishmentSystemId.ToString()}
                    }), 550, 500);
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("PublishmentSystemID");

            _isHeadquarters = PublishmentSystemInfo.IsHeadquarters;

            var selectedList = new ArrayList();

            if (!Page.IsPostBack)
            {
                if (_isHeadquarters)
                {
                    HeadquartersExists.Visible = false;
                    ChangeToSite.Visible = true;
                    ChangeToHeadquarters.Visible = false;
                    var fileSystems = FileManager.GetFileSystemInfoExtendCollection(WebConfigUtils.PhysicalApplicationPath, true);
                    var publishmentSystemDirList = DataProvider.PublishmentSystemDao.GetLowerPublishmentSystemDirListThatNotIsHeadquarters();
                    foreach (FileSystemInfoExtend fileSystem in fileSystems)
                    {
                        if (fileSystem.IsDirectory)
                        {
                            if (!DirectoryUtils.IsSystemDirectory(fileSystem.Name) && !publishmentSystemDirList.Contains(fileSystem.Name.ToLower()))
                            {
                                FilesToSite.Items.Add(new ListItem(fileSystem.Name, fileSystem.Name));
                            }
                        }
                        else
                        {
                            if (!PathUtility.IsSystemFileForChangePublishmentSystemType(fileSystem.Name))
                            {
                                FilesToSite.Items.Add(new ListItem(fileSystem.Name, fileSystem.Name));
                            }
                        }

                        if (PathUtility.IsWebSiteFile(fileSystem.Name) || DirectoryUtils.IsWebSiteDirectory(fileSystem.Name))
                        {
                            selectedList.Add(fileSystem.Name);
                        }

                    }

                    //主站下的单页模板
                    var fileTemplateInfoList = DataProvider.TemplateDao.GetTemplateInfoArrayListByType(PublishmentSystemId, ETemplateType.FileTemplate);
                    foreach (TemplateInfo fileT in fileTemplateInfoList)
                    {
                        if (fileT.CreatedFileFullName.StartsWith("@/") || fileT.CreatedFileFullName.StartsWith("~/"))
                        {
                            var arr = fileT.CreatedFileFullName.Substring(2).Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
                            if (arr.Length > 0)
                                selectedList.Add(arr[0]);
                        }
                    }
                }
                else
                {
                    var headquartersExists = false;
                    var publishmentSystemIDArrayList = PublishmentSystemManager.GetPublishmentSystemIdList();
                    foreach (int psID in publishmentSystemIDArrayList)
                    {
                        var psInfo = PublishmentSystemManager.GetPublishmentSystemInfo(psID);
                        if (psInfo.IsHeadquarters)
                        {
                            headquartersExists = true;
                            break;
                        }
                    }
                    if (headquartersExists)
                    {
                        HeadquartersExists.Visible = true;
                        ChangeToSite.Visible = false;
                        ChangeToHeadquarters.Visible = false;
                    }
                    else
                    {
                        HeadquartersExists.Visible = false;
                        ChangeToSite.Visible = false;
                        ChangeToHeadquarters.Visible = true;
                    }
                }

                //设置选中的文件以及文件夹
                ControlUtils.SelectListItems(FilesToSite, selectedList);
            }
        }

        public string GetSiteName()
        {
            return PublishmentSystemInfo.PublishmentSystemName;
        }

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            var isChanged = false;
            if (HeadquartersExists.Visible)
            {
                Page.Response.Clear();
                Page.Response.Write("<script language=\"javascript\">" + PageUtils.HidePopWin + "</script>");
                Page.Response.End();
            }
            else
            {
                try
                {
                    if (_isHeadquarters)
                    {
                        var list = DataProvider.NodeDao.GetLowerSystemDirList(PublishmentSystemInfo.ParentPublishmentSystemId);
                        if (list.IndexOf(PublishmentSystemDir.Text.Trim().ToLower()) != -1)
                        {
                            PublishmentSystemDirValidator.IsValid = false;
                            PublishmentSystemDirValidator.ErrorMessage = "已存在相同的发布路径";
                        }
                        else
                        {
                            if (!DirectoryUtils.IsDirectoryNameCompliant(PublishmentSystemDir.Text))
                            {
                                PublishmentSystemDirValidator.IsValid = false;
                                PublishmentSystemDirValidator.ErrorMessage = "文件夹名称不符合要求";
                            }
                            else
                            {
                                var filesToSite = new ArrayList();
                                foreach (ListItem item in FilesToSite.Items)
                                {
                                    if (item.Selected)
                                    {
                                        filesToSite.Add(item.Value);
                                    }
                                }
                                DirectoryUtility.ChangeToSubSite(PublishmentSystemInfo, PublishmentSystemDir.Text, filesToSite);
                                isChanged = true;
                            }
                        }
                    }
                    else
                    {
                        DirectoryUtility.ChangeToHeadquarters(PublishmentSystemInfo, TranslateUtils.ToBool(IsMoveFiles.SelectedValue));

                        isChanged = true;
                    }
                }
                catch (Exception ex)
                {
                    PageUtils.RedirectToErrorPage($"修改失败：{ex.Message}");
                    return;
                }
            }

            if (isChanged)
            {
                if (_isHeadquarters)
                {
                    Body.AddAdminLog("转移到子目录",
                        $"站点:{PublishmentSystemInfo.PublishmentSystemName}");
                }
                else
                {
                    Body.AddAdminLog("转移到根目录",
                        $"站点:{PublishmentSystemInfo.PublishmentSystemName}");
                }
                PageUtils.CloseModalPage(Page);
            }
        }
    }
}
