using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Model.Enumerations;
using SiteServer.BackgroundPages.Cms;
using SiteServer.CMS.Core;
using SiteServer.CMS.Core.Security;
using SiteServer.CMS.ImportExport;
using SiteServer.CMS.ImportExport.Components;
using SiteServer.CMS.Model;

namespace SiteServer.BackgroundPages.Settings
{
    public class PageSiteAdd : BasePageCms
    {
        protected override bool IsSinglePage => true;

        public PlaceHolder PhSiteTemplate;
        public HtmlInputHidden HihSiteTemplateDir;
        public CheckBox CbIsSiteTemplate;
        public PlaceHolder PhIsSiteTemplate;
        public Repeater RptSiteTemplates;

        public PlaceHolder PhParameters;
        public PlaceHolder PhSiteTemplateName;
        public Literal LtlSiteTemplateName;
        public TextBox TbPublishmentSystemName;
        public RadioButtonList RblIsHeadquarters;
        public PlaceHolder PhIsNotHeadquarters;
        public DropDownList DdlParentPublishmentSystemId;
        public TextBox TbPublishmentSystemDir;
        public DropDownList DdlCharset;
        public PlaceHolder PhIsImportContents;
        public CheckBox CbIsImportContents;
        public PlaceHolder PhIsImportTableStyles;
        public CheckBox CbIsImportTableStyles;
        public PlaceHolder PhIsUserSiteTemplateAuxiliaryTables;
        public RadioButtonList RblIsUserSiteTemplateAuxiliaryTables;
        public PlaceHolder PhAuxiliaryTable;
        public DropDownList DdlAuxiliaryTableForContent;
        public RadioButtonList RblIsCheckContentUseLevel;
        public PlaceHolder PhCheckContentLevel;
        public DropDownList DdlCheckContentLevel;

        public Button BtnPrevious;
        public Button BtnSiteTemplateNext;
        public Button BtnParameters;

        private SortedList _sortedlist = new SortedList();
        private AdministratorWithPermissions _permissions;

        public static string GetRedirectUrl()
        {
            return PageUtils.GetSettingsUrl(nameof(PageSiteAdd), null);
        }

        public static string GetRedirectUrl(string siteTemplate)
        {
            return PageUtils.GetSettingsUrl(nameof(PageSiteAdd), new NameValueCollection
            {
                {"siteTemplate", siteTemplate}
            });
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            _sortedlist = SiteTemplateManager.Instance.GetSiteTemplateSortedList();
            _permissions = PermissionsManager.GetPermissions(Body.AdminName);

            if (IsPostBack) return;

            BaiRongDataProvider.TableCollectionDao.CreateAllTableCollectionInfoIfNotExists();

            HihSiteTemplateDir.Value = Body.GetQueryString("siteTemplate");

            VerifyAdministratorPermissions(AppManager.Permissions.Settings.SiteAdd);

            var hqSiteId = DataProvider.PublishmentSystemDao.GetPublishmentSystemIdByIsHeadquarters();
            if (hqSiteId == 0)
            {
                ControlUtils.SelectSingleItem(RblIsHeadquarters, true.ToString());
                PhIsNotHeadquarters.Visible = false;
            }
            else
            {
                RblIsHeadquarters.Enabled = false;
            }

            DdlParentPublishmentSystemId.Items.Add(new ListItem("<无上级站点>", "0"));
            var publishmentSystemIdArrayList = PublishmentSystemManager.GetPublishmentSystemIdList();
            var mySystemInfoArrayList = new ArrayList();
            var parentWithChildren = new Hashtable();
            foreach (var publishmentSystemId in publishmentSystemIdArrayList)
            {
                var publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(publishmentSystemId);
                if (publishmentSystemInfo.IsHeadquarters == false)
                {
                    if (publishmentSystemInfo.ParentPublishmentSystemId == 0)
                    {
                        mySystemInfoArrayList.Add(publishmentSystemInfo);
                    }
                    else
                    {
                        var children = new ArrayList();
                        if (parentWithChildren.Contains(publishmentSystemInfo.ParentPublishmentSystemId))
                        {
                            children = (ArrayList)parentWithChildren[publishmentSystemInfo.ParentPublishmentSystemId];
                        }
                        children.Add(publishmentSystemInfo);
                        parentWithChildren[publishmentSystemInfo.ParentPublishmentSystemId] = children;
                    }
                }
            }
            foreach (PublishmentSystemInfo publishmentSystemInfo in mySystemInfoArrayList)
            {
                AddSite(DdlParentPublishmentSystemId, publishmentSystemInfo, parentWithChildren, 0);
            }
            ControlUtils.SelectSingleItem(DdlParentPublishmentSystemId, "0");

            ECharsetUtils.AddListItems(DdlCharset);
            ControlUtils.SelectSingleItem(DdlCharset, ECharsetUtils.GetValue(ECharset.utf_8));

            var tableList = BaiRongDataProvider.TableCollectionDao.GetTableCollectionInfoListCreatedInDb();
            foreach (var tableInfo in tableList)
            {
                var li = new ListItem($"{tableInfo.TableCnName}({tableInfo.TableEnName})", tableInfo.TableEnName);
                DdlAuxiliaryTableForContent.Items.Add(li);
            }

            RblIsCheckContentUseLevel.Items.Add(new ListItem("默认审核机制", false.ToString()));
            RblIsCheckContentUseLevel.Items.Add(new ListItem("多级审核机制", true.ToString()));
            ControlUtils.SelectSingleItem(RblIsCheckContentUseLevel, false.ToString());

            var directoryList = new List<DirectoryInfo>();
            foreach (string directoryName in _sortedlist.Keys)
            {
                var directoryPath = PathUtility.GetSiteTemplatesPath(directoryName);
                var dirInfo = new DirectoryInfo(directoryPath);
                directoryList.Add(dirInfo);
            }

            RptSiteTemplates.DataSource = directoryList;
            RptSiteTemplates.ItemDataBound += RptSiteTemplates_ItemDataBound;
            RptSiteTemplates.DataBind();

            PhSiteTemplate.Visible = PhParameters.Visible = BtnSiteTemplateNext.Visible = BtnParameters.Visible = false;

            if (_sortedlist.Count > 0)
            {
                PhSiteTemplate.Visible = BtnSiteTemplateNext.Visible = true;
            }
            else
            {
                PhParameters.Visible = BtnParameters.Visible = true;
                CbIsSiteTemplate.Checked = false;
                PhSiteTemplateName.Visible = PhIsImportContents.Visible = PhIsImportTableStyles.Visible = PhIsUserSiteTemplateAuxiliaryTables.Visible = false;
                PhAuxiliaryTable.Visible = true;
            }
        }

        public void CbIsSiteTemplate_CheckedChanged(object sender, EventArgs e)
        {
            PhIsSiteTemplate.Visible = CbIsSiteTemplate.Checked;
        }

        public void RblIsHeadquarters_SelectedIndexChanged(object sender, EventArgs e)
        {
            PhIsNotHeadquarters.Visible = !TranslateUtils.ToBool(RblIsHeadquarters.SelectedValue);
        }

        public void RblIsUserSiteTemplateAuxiliaryTables_SelectedIndexChanged(object sender, EventArgs e)
        {
            PhAuxiliaryTable.Visible = !TranslateUtils.ToBool(RblIsUserSiteTemplateAuxiliaryTables.SelectedValue);
        }

        public void RblIsCheckContentUseLevel_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            PhCheckContentLevel.Visible = EBooleanUtils.Equals(RblIsCheckContentUseLevel.SelectedValue, EBoolean.True);
        }

        public void BtnPrevious_Click(object sender, EventArgs e)
        {
            PhSiteTemplate.Visible = BtnSiteTemplateNext.Visible = true;
            PhParameters.Visible = BtnParameters.Visible = BtnPrevious.Enabled = false;
        }

        public void BtnSiteTemplateNext_Click(object sender, EventArgs e)
        {
            PhSiteTemplate.Visible = PhParameters.Visible = BtnSiteTemplateNext.Visible = BtnParameters.Visible = false;

            if (CbIsSiteTemplate.Checked)
            {
                var siteTemplateDir = HihSiteTemplateDir.Value;

                if (string.IsNullOrEmpty(siteTemplateDir))
                {
                    FailMessage("请选择需要使用的站点模板");
                    PhSiteTemplate.Visible = BtnSiteTemplateNext.Visible = true;
                    BtnPrevious.Enabled = false;
                    return;
                }

                PhSiteTemplateName.Visible = PhIsImportContents.Visible = PhIsImportTableStyles.Visible = PhIsUserSiteTemplateAuxiliaryTables.Visible = true;
                PhAuxiliaryTable.Visible = true;

                
                LtlSiteTemplateName.Text = $"{GetSiteTemplateName(siteTemplateDir)}（{siteTemplateDir}）";

                var siteTemplatePath = PathUtility.GetSiteTemplatesPath(siteTemplateDir);
                var filePath = PathUtility.GetSiteTemplateMetadataPath(siteTemplatePath, DirectoryUtils.SiteTemplates.FileConfiguration);
                var publishmentSystemInfo = ConfigurationIe.GetPublishmentSytemInfo(filePath);

                TbPublishmentSystemName.Text = publishmentSystemInfo.PublishmentSystemName;
                TbPublishmentSystemDir.Text = publishmentSystemInfo.PublishmentSystemDir;
                var extend = new PublishmentSystemInfoExtend(publishmentSystemInfo.PublishmentSystemDir, publishmentSystemInfo.SettingsXml);
                if (!string.IsNullOrEmpty(extend.Charset))
                {
                    DdlCharset.SelectedValue = extend.Charset;
                }
            }
            else
            {
                PhSiteTemplateName.Visible = PhIsImportContents.Visible = PhIsImportTableStyles.Visible = PhIsUserSiteTemplateAuxiliaryTables.Visible = false;
                PhAuxiliaryTable.Visible = true;
            }

            PhParameters.Visible = BtnParameters.Visible = BtnPrevious.Enabled = true;
        }

        public void BtnParameters_Click(object sender, EventArgs e)
        {
            string errorMessage;
            var thePublishmentSystemId = Validate_PublishmentSystemInfo(out errorMessage);
            if (thePublishmentSystemId > 0)
            {
                var url = PageProgressBar.GetCreatePublishmentSystemUrl(thePublishmentSystemId, CbIsSiteTemplate.Checked, CbIsImportContents.Checked, CbIsImportTableStyles.Checked, HihSiteTemplateDir.Value, TranslateUtils.ToBool(RblIsUserSiteTemplateAuxiliaryTables.SelectedValue), StringUtils.Guid(), string.Empty);
                PageUtils.Redirect(url);
            }
            else
            {
                FailMessage(errorMessage);
            }
        }

        private void RptSiteTemplates_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.AlternatingItem && e.Item.ItemType != ListItemType.Item) return;

            var directoryInfo = (DirectoryInfo)e.Item.DataItem;
            var siteTemplateInfo = _sortedlist[directoryInfo.Name] as SiteTemplateInfo;

            if (siteTemplateInfo == null) return;

            var ltlChoose = (Literal)e.Item.FindControl("ltlChoose");
            var ltlTemplateName = (Literal)e.Item.FindControl("ltlTemplateName");
            var ltlName = (Literal)e.Item.FindControl("ltlName");
            var ltlDescription = (Literal)e.Item.FindControl("ltlDescription");
            var ltlSamplePic = (Literal)e.Item.FindControl("ltlSamplePic");

            ltlChoose.Text = $@"<input type=""radio"" name=""choose"" id=""choose_{directoryInfo.Name}"" onClick=""document.getElementById('{HihSiteTemplateDir.ClientID}').value=this.value;"" value=""{directoryInfo.Name}"" /><label for=""choose_{directoryInfo.Name}"">选中</label>";

            if (!string.IsNullOrEmpty(siteTemplateInfo.SiteTemplateName))
            {
                ltlTemplateName.Text = !string.IsNullOrEmpty(siteTemplateInfo.WebSiteUrl) ? $"<a href=\"{PageUtils.ParseConfigRootUrl(siteTemplateInfo.WebSiteUrl)}\" target=_blank>{siteTemplateInfo.SiteTemplateName}</a>" : siteTemplateInfo.SiteTemplateName;
            }

            ltlName.Text = directoryInfo.Name;

            if (!string.IsNullOrEmpty(siteTemplateInfo.Description))
            {
                ltlDescription.Text = siteTemplateInfo.Description;
            }

            if (!string.IsNullOrEmpty(siteTemplateInfo.PicFileName))
            {
                var siteTemplateUrl = PageUtils.GetSiteTemplatesUrl(directoryInfo.Name);
                var picFileName = PageUtils.GetSiteTemplateMetadataUrl(siteTemplateUrl, siteTemplateInfo.PicFileName);
                ltlSamplePic.Text = $@"<a href=""{picFileName}"" target=""_blank"">样图</a>";
            }
        }

        private int Validate_PublishmentSystemInfo(out string errorMessage)
        {
            try
            {
                var isHq = TranslateUtils.ToBool(RblIsHeadquarters.SelectedValue); // 是否主站
                var parentPublishmentSystemId = 0;
                var publishmentSystemDir = string.Empty;

                if (isHq == false)
                {
                    if (DirectoryUtils.IsSystemDirectory(TbPublishmentSystemDir.Text))
                    {
                        errorMessage = "文件夹名称不能为系统文件夹名称！";
                        return 0;
                    }

                    parentPublishmentSystemId = TranslateUtils.ToInt(DdlParentPublishmentSystemId.SelectedValue);
                    publishmentSystemDir = TbPublishmentSystemDir.Text;

                    var list = DataProvider.NodeDao.GetLowerSystemDirList(parentPublishmentSystemId);
                    if (list.IndexOf(publishmentSystemDir.ToLower()) != -1)
                    {
                        errorMessage = "已存在相同的发布路径！";
                        return 0;
                    }

                    if (!DirectoryUtils.IsDirectoryNameCompliant(publishmentSystemDir))
                    {
                        errorMessage = "文件夹名称不符合系统要求！";
                        return 0;
                    }
                }

                var nodeInfo = new NodeInfo();

                nodeInfo.NodeName = nodeInfo.NodeIndexName = "首页";
                nodeInfo.ParentId = 0;
                nodeInfo.ContentModelPluginId = string.Empty;

                var psInfo = new PublishmentSystemInfo
                {
                    PublishmentSystemName = PageUtils.FilterXss(TbPublishmentSystemName.Text),
                    AuxiliaryTableForContent = DdlAuxiliaryTableForContent.SelectedValue,
                    PublishmentSystemDir = publishmentSystemDir,
                    ParentPublishmentSystemId = parentPublishmentSystemId,
                    IsHeadquarters = isHq,
                    IsCheckContentUseLevel = TranslateUtils.ToBool(RblIsCheckContentUseLevel.SelectedValue)
                };

                if (psInfo.IsCheckContentUseLevel)
                {
                    psInfo.CheckContentLevel = TranslateUtils.ToInt(DdlCheckContentLevel.SelectedValue);
                }
                psInfo.Additional.Charset = DdlCharset.SelectedValue;

                var thePublishmentSystemId = DataProvider.NodeDao.InsertPublishmentSystemInfo(nodeInfo, psInfo, Body.AdminName);

                if (_permissions.IsSystemAdministrator && !_permissions.IsConsoleAdministrator)
                {
                    var publishmentSystemIdList = ProductPermissionsManager.Current.PublishmentSystemIdList ?? new List<int>();
                    publishmentSystemIdList.Add(thePublishmentSystemId);
                    BaiRongDataProvider.AdministratorDao.UpdatePublishmentSystemIdCollection(Body.AdminName, TranslateUtils.ObjectCollectionToString(publishmentSystemIdList));
                }

                Body.AddAdminLog("新建站点", $"站点名称:{PageUtils.FilterXss(TbPublishmentSystemName.Text)}");

                errorMessage = string.Empty;
                return thePublishmentSystemId;
            }
            catch (Exception e)
            {
                errorMessage = e.Message;
                return 0;
            }
        }

        private string GetSiteTemplateName(string siteTemplateDir)
        {
            var siteTemplateInfo = _sortedlist[siteTemplateDir] as SiteTemplateInfo;
            if (string.IsNullOrEmpty(siteTemplateInfo?.SiteTemplateName)) return string.Empty;

            return !string.IsNullOrEmpty(siteTemplateInfo.WebSiteUrl) ? $"<a href=\"{PageUtils.ParseNavigationUrl(siteTemplateInfo.WebSiteUrl)}\" target=_blank>{siteTemplateInfo.SiteTemplateName}</a>" : siteTemplateInfo.SiteTemplateName;
        }

        private static void AddSite(ListControl listControl, PublishmentSystemInfo publishmentSystemInfo, Hashtable parentWithChildren, int level)
        {
            if (level > 1) return;
            var padding = string.Empty;
            for (var i = 0; i < level; i++)
            {
                padding += "　";
            }
            if (level > 0)
            {
                padding += "└ ";
            }

            if (parentWithChildren[publishmentSystemInfo.PublishmentSystemId] != null)
            {
                var children = (ArrayList)parentWithChildren[publishmentSystemInfo.PublishmentSystemId];
                listControl.Items.Add(new ListItem(padding + publishmentSystemInfo.PublishmentSystemName + $"({children.Count})", publishmentSystemInfo.PublishmentSystemId.ToString()));
                level++;
                foreach (PublishmentSystemInfo subSiteInfo in children)
                {
                    AddSite(listControl, subSiteInfo, parentWithChildren, level);
                }
            }
            else
            {
                listControl.Items.Add(new ListItem(padding + publishmentSystemInfo.PublishmentSystemName, publishmentSystemInfo.PublishmentSystemId.ToString()));
            }
        }
    }
}
