using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Model.Enumerations;
using BaiRong.Core.Permissions;
using BaiRong.Core.Text;
using SiteServer.BackgroundPages.Cms;
using SiteServer.CMS.Core;
using SiteServer.CMS.Core.Security;
using SiteServer.CMS.Core.SystemData;
using SiteServer.CMS.ImportExport;
using SiteServer.CMS.ImportExport.Components;
using SiteServer.CMS.Model;
using SiteServer.CMS.Model.Enumerations;

namespace SiteServer.BackgroundPages.Sys
{
    public class PagePublishmentSystemAdd : BasePageCms
    {
        protected override bool IsSinglePage => true;

        public Literal ltlPageTitle;
        public PlaceHolder ChooseSiteTemplate;
        public CheckBox UseSiteTemplate;
        public DataList dlContents;
        public HtmlInputHidden SiteTemplateDir;

        public PlaceHolder CreateSiteParameters;
        public Control RowSiteTemplateName;
        public Label SiteTemplateName;
        public TextBox PublishmentSystemName;
        public Literal ltlPublishmentSystemType;
        public RadioButtonList IsHeadquarters;
        public PlaceHolder phNotIsHeadquarters;
        public DropDownList ParentPublishmentSystemID;
        public TextBox PublishmentSystemDir;

        public PlaceHolder phNodeRelated;
        public DropDownList Charset;
        public Control RowIsImportContents;
        public CheckBox IsImportContents;
        public Control RowIsImportTableStyles;
        public CheckBox IsImportTableStyles;
        public Control RowIsUserSiteTemplateAuxiliaryTables;
        public RadioButtonList IsUserSiteTemplateAuxiliaryTables;
        public PlaceHolder phAuxiliaryTable;
        public DropDownList AuxiliaryTableForContent;
        public PlaceHolder phWCMTables;
        public DropDownList AuxiliaryTableForGovPublic;
        public DropDownList AuxiliaryTableForGovInteract;
        public DropDownList AuxiliaryTableForVote;
        public DropDownList AuxiliaryTableForJob;
        public RadioButtonList IsCheckContentUseLevel;
        public Control CheckContentLevelRow;
        public DropDownList CheckContentLevel;

        public PlaceHolder OperatingError;
        public Literal ltlErrorMessage;

        public Button Previous;
        public Button Next;

        private EPublishmentSystemType _publishmentSystemType = EPublishmentSystemType.CMS;
        private SortedList _sortedlist = new SortedList();
        private AdministratorWithPermissions _permissions;

        public static string GetRedirectUrl(EPublishmentSystemType publishmentSystemType)
        {
            return PageUtils.GetSysUrl(nameof(PagePublishmentSystemAdd), new NameValueCollection
            {
                {"publishmentSystemType", EPublishmentSystemTypeUtils.GetValue(publishmentSystemType)}
            });
        }

        public static string GetRedirectUrl(string siteTemplate)
        {
            return PageUtils.GetSysUrl(nameof(PagePublishmentSystemAdd), new NameValueCollection
            {
                {"siteTemplate", siteTemplate}
            });
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            _publishmentSystemType = EPublishmentSystemTypeUtils.GetEnumType(Body.GetQueryString("publishmentSystemType"));
            _sortedlist = SiteTemplateManager.Instance.GetSiteTemplateSortedList();
            _permissions = PermissionsManager.GetPermissions(Body.AdministratorName);

            if (!IsPostBack)
            {
                BaiRongDataProvider.TableCollectionDao.CreateAllAuxiliaryTableIfNotExists();

                SiteTemplateDir.Value = Body.GetQueryString("siteTemplate");

                string pageTitle = $"创建{EPublishmentSystemTypeUtils.GetText(_publishmentSystemType)}";
                ltlPageTitle.Text = pageTitle;
                BreadCrumbSys(AppManager.Sys.LeftMenu.Site, pageTitle, AppManager.Sys.Permission.SysSite);

                var hqSiteId = DataProvider.PublishmentSystemDao.GetPublishmentSystemIdByIsHeadquarters();
                if (hqSiteId == 0)
                {
                    IsHeadquarters.SelectedValue = "True";
                    phNotIsHeadquarters.Visible = false;
                }
                else
                {
                    IsHeadquarters.Enabled = false;
                }

                ltlPublishmentSystemType.Text = EPublishmentSystemTypeUtils.GetHtml(_publishmentSystemType);

                phWCMTables.Visible = _publishmentSystemType == EPublishmentSystemType.WCM;

                ParentPublishmentSystemID.Items.Add(new ListItem("<无上级站点>", "0"));
                var publishmentSystemIdArrayList = PublishmentSystemManager.GetPublishmentSystemIdList();
                var mySystemInfoArrayList = new ArrayList();
                var parentWithChildren = new Hashtable();
                foreach (int publishmentSystemId in publishmentSystemIdArrayList)
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
                    AddSite(ParentPublishmentSystemID, publishmentSystemInfo, parentWithChildren, 0);
                }
                ControlUtils.SelectListItems(ParentPublishmentSystemID, "0");

                phNodeRelated.Visible = EPublishmentSystemTypeUtils.IsNodeRelated(_publishmentSystemType);

                ECharsetUtils.AddListItems(Charset);
                ControlUtils.SelectListItems(Charset, ECharsetUtils.GetValue(ECharset.utf_8));

                var tableList = BaiRongDataProvider.TableCollectionDao.GetAuxiliaryTableListCreatedInDbByAuxiliaryTableType(EAuxiliaryTableType.BackgroundContent);
                foreach (var tableInfo in tableList)
                {
                    var li = new ListItem($"{tableInfo.TableCnName}({tableInfo.TableEnName})", tableInfo.TableEnName);
                    AuxiliaryTableForContent.Items.Add(li);
                }

                tableList = BaiRongDataProvider.TableCollectionDao.GetAuxiliaryTableListCreatedInDbByAuxiliaryTableType(EAuxiliaryTableType.GovPublicContent);
                foreach (var tableInfo in tableList)
                {
                    var li = new ListItem($"{tableInfo.TableCnName}({tableInfo.TableEnName})", tableInfo.TableEnName);
                    AuxiliaryTableForGovPublic.Items.Add(li);
                }

                tableList = BaiRongDataProvider.TableCollectionDao.GetAuxiliaryTableListCreatedInDbByAuxiliaryTableType(EAuxiliaryTableType.GovInteractContent);
                foreach (var tableInfo in tableList)
                {
                    var li = new ListItem($"{tableInfo.TableCnName}({tableInfo.TableEnName})", tableInfo.TableEnName);
                    AuxiliaryTableForGovInteract.Items.Add(li);
                }

                tableList = BaiRongDataProvider.TableCollectionDao.GetAuxiliaryTableListCreatedInDbByAuxiliaryTableType(EAuxiliaryTableType.VoteContent);
                foreach (var tableInfo in tableList)
                {
                    var li = new ListItem($"{tableInfo.TableCnName}({tableInfo.TableEnName})", tableInfo.TableEnName);
                    AuxiliaryTableForVote.Items.Add(li);
                }

                tableList = BaiRongDataProvider.TableCollectionDao.GetAuxiliaryTableListCreatedInDbByAuxiliaryTableType(EAuxiliaryTableType.JobContent);
                foreach (var tableInfo in tableList)
                {
                    var li = new ListItem($"{tableInfo.TableCnName}({tableInfo.TableEnName})", tableInfo.TableEnName);
                    AuxiliaryTableForJob.Items.Add(li);
                }

                IsCheckContentUseLevel.Items.Add(new ListItem("默认审核机制", false.ToString()));
                IsCheckContentUseLevel.Items.Add(new ListItem("多级审核机制", true.ToString()));
                ControlUtils.SelectListItems(IsCheckContentUseLevel, false.ToString());

                UseSiteTemplate.Attributes.Add("onclick", "displaySiteTemplateDiv(this)");

                BindGrid();

                if (_sortedlist.Count > 0)
                {
                    SetActivePlaceHolder(WizardPlaceHolder.ChooseSiteTemplate, ChooseSiteTemplate);
                }
                else
                {
                    ChooseSiteTemplate.Visible = false;
                    UseSiteTemplate.Checked = false;
                    SetActivePlaceHolder(WizardPlaceHolder.CreateSiteParameters, CreateSiteParameters);
                    RowSiteTemplateName.Visible = RowIsImportContents.Visible = RowIsImportTableStyles.Visible = RowIsUserSiteTemplateAuxiliaryTables.Visible = false;
                    phAuxiliaryTable.Visible = true;
                }
            }
        }

        public string GetSiteTemplateName(string siteTemplateDir)
        {
            var siteTemplateInfo = _sortedlist[siteTemplateDir] as SiteTemplateInfo;
            if (string.IsNullOrEmpty(siteTemplateInfo?.SiteTemplateName)) return string.Empty;

            return !string.IsNullOrEmpty(siteTemplateInfo.WebSiteUrl) ? $"<a href=\"{PageUtils.ParseNavigationUrl(siteTemplateInfo.WebSiteUrl)}\" target=_blank>{siteTemplateInfo.SiteTemplateName}</a>" : siteTemplateInfo.SiteTemplateName;
        }

        private static void AddSite(ListControl listControl, PublishmentSystemInfo publishmentSystemInfo, Hashtable parentWithChildren, int level)
        {
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

        public void IsHeadquarters_SelectedIndexChanged(object sender, EventArgs e)
        {
            phNotIsHeadquarters.Visible = !TranslateUtils.ToBool(IsHeadquarters.SelectedValue);
        }

        public void IsUserSiteTemplateAuxiliaryTables_SelectedIndexChanged(object sender, EventArgs e)
        {
            phAuxiliaryTable.Visible = !TranslateUtils.ToBool(IsUserSiteTemplateAuxiliaryTables.SelectedValue);
        }

        public void IsCheckContentUseLevel_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            CheckContentLevelRow.Visible = EBooleanUtils.Equals(IsCheckContentUseLevel.SelectedValue, EBoolean.True);
        }

        public void BindGrid()
        {
            try
            {
                var directoryArrayList = new ArrayList();
                foreach (string directoryName in _sortedlist.Keys)
                {
                    var directoryPath = PathUtility.GetSiteTemplatesPath(directoryName);
                    var dirInfo = new DirectoryInfo(directoryPath);
                    directoryArrayList.Add(dirInfo);
                }

                dlContents.DataSource = directoryArrayList;
                dlContents.ItemDataBound += dlContents_ItemDataBound;
                dlContents.DataBind();
            }
            catch (Exception ex)
            {
                PageUtils.RedirectToErrorPage(ex.Message);
            }
        }

        void dlContents_ItemDataBound(object sender, DataListItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.Item)
            {
                var dirInfo = (DirectoryInfo)e.Item.DataItem;

                var ltlImageUrl = (Literal) e.Item.FindControl("ltlImageUrl");
                var ltlDescription = (Literal) e.Item.FindControl("ltlDescription");
                var ltlRadio = (Literal) e.Item.FindControl("ltlRadio");

                var siteTemplateInfo = _sortedlist[dirInfo.Name] as SiteTemplateInfo;
                if (!string.IsNullOrEmpty(siteTemplateInfo?.SiteTemplateName))
                {
                    var checkedStr = string.Empty;
                    if (StringUtils.EqualsIgnoreCase(Body.GetQueryString("siteTemplate"), dirInfo.Name))
                    {
                        checkedStr = "checked";
                    }

                    var templateSn = dirInfo.Name.ToUpper().Substring(2);
                    if (!string.IsNullOrEmpty(siteTemplateInfo.WebSiteUrl))
                    {
                        templateSn =
                            $@"<a href=""{PageUtils.ParseConfigRootUrl(siteTemplateInfo.WebSiteUrl)}"" target=""_blank"">{templateSn}</a>";
                    }

                    if (e.Item.ItemIndex == 0)
                    {
                        ltlRadio.Text = $@"
<label class=""radio lead"" onClick=""$('#SiteTemplateDir').val($(this).find('input').val());"">
  <input type=""radio"" name=""choose"" id=""choose{e.Item.ItemIndex + 1}"" value=""{dirInfo.Name}"" {checkedStr} checked=""checked"">
  {templateSn}
</label>";
                        if (string.IsNullOrEmpty(SiteTemplateDir.Value))
                        {
                            SiteTemplateDir.Value = dirInfo.Name;
                        }
                    }
                    else
                    {
                        ltlRadio.Text = $@"
<label class=""radio lead"" onClick=""$('#SiteTemplateDir').val($(this).find('input').val());"">
  <input type=""radio"" name=""choose"" id=""choose{e.Item.ItemIndex + 1}"" value=""{dirInfo.Name}"" {checkedStr}>
  {templateSn}
</label>";
                    }

                    if (!string.IsNullOrEmpty(siteTemplateInfo.PicFileName))
                    {
                        var siteTemplateUrl = PageUtility.GetSiteTemplatesUrl(dirInfo.Name);
                        ltlImageUrl.Text =
                            $@"<img class=""cover"" src=""{PageUtility.GetSiteTemplateMetadataUrl(siteTemplateUrl,
                                siteTemplateInfo.PicFileName)}"" width=""180""><p></p>";
                    }

                    if (!string.IsNullOrEmpty(siteTemplateInfo.Description))
                    {
                        ltlDescription.Text = siteTemplateInfo.Description + "<p></p>";
                    }
                }
            }
        }

        public WizardPlaceHolder CurrentWizardPlaceHolder
        {
            get
            {
                if (ViewState["WizardPlaceHolder"] != null)
                    return (WizardPlaceHolder)ViewState["WizardPlaceHolder"];

                return _sortedlist.Count > 0 ? WizardPlaceHolder.ChooseSiteTemplate : WizardPlaceHolder.CreateSiteParameters;
            }
            set
            {
                ViewState["WizardPlaceHolder"] = value;
            }
        }

        public enum WizardPlaceHolder
        {
            ChooseSiteTemplate,
            CreateSiteParameters,
            OperatingError,
        }

        void SetActivePlaceHolder(WizardPlaceHolder panel, Control controlToShow)
        {
            var currentPlaceHolder = FindControl(CurrentWizardPlaceHolder.ToString()) as PlaceHolder;
            if (currentPlaceHolder != null)
                currentPlaceHolder.Visible = false;

            switch (panel)
            {
                case WizardPlaceHolder.ChooseSiteTemplate:
                    Previous.CssClass = "btn disabled";
                    Previous.Enabled = false;
                    Next.CssClass = "btn btn-primary";
                    Next.Enabled = true;
                    break;
                case WizardPlaceHolder.CreateSiteParameters:
                    Previous.CssClass = "btn";
                    Previous.Enabled = true;
                    Next.CssClass = "btn btn-primary";
                    Next.Enabled = true;
                    break;
                case WizardPlaceHolder.OperatingError:
                    Previous.CssClass = "btn disabled";
                    Previous.Enabled = false;
                    Next.CssClass = "btn btn-primary disabled";
                    Next.Enabled = false;
                    break;
                default:
                    Previous.CssClass = "btn";
                    Previous.Enabled = true;
                    Next.CssClass = "btn btn-primary";
                    Next.Enabled = true;
                    break;
            }

            controlToShow.Visible = true;
            CurrentWizardPlaceHolder = panel;
        }

        private int Validate_PublishmentSystemInfo(out string errorMessage)
        {
            try
            {
                var isHq = TranslateUtils.ToBool(IsHeadquarters.SelectedValue); // 是否主站
                var parentPublishmentSystemId = 0;
                var publishmentSystemDir = string.Empty;

                if (isHq == false)
                {
                    if (DirectoryUtils.IsSystemDirectory(PublishmentSystemDir.Text))
                    {
                        errorMessage = "文件夹名称不能为系统文件夹名称！";
                        return 0;
                    }

                    parentPublishmentSystemId = TranslateUtils.ToInt(ParentPublishmentSystemID.SelectedValue);
                    publishmentSystemDir = PublishmentSystemDir.Text;

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
                nodeInfo.NodeType = ENodeType.BackgroundPublishNode;
                nodeInfo.ContentModelId = EContentModelTypeUtils.GetValue(EContentModelTypeUtils.GetEnumTypeByPublishmentSystemType(_publishmentSystemType));

                var publishmentSystemUrl = PageUtils.Combine(WebConfigUtils.ApplicationPath, publishmentSystemDir);

                var psInfo = BaseTable.GetDefaultPublishmentSystemInfo(PageUtils.FilterXss(PublishmentSystemName.Text), _publishmentSystemType, AuxiliaryTableForContent.SelectedValue, AuxiliaryTableForGovPublic.SelectedValue, AuxiliaryTableForGovInteract.SelectedValue, AuxiliaryTableForVote.SelectedValue, AuxiliaryTableForJob.SelectedValue, publishmentSystemDir, publishmentSystemUrl, parentPublishmentSystemId);

                if (psInfo.ParentPublishmentSystemId > 0)
                {
                    var parentPublishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(psInfo.ParentPublishmentSystemId);
                    psInfo.PublishmentSystemUrl = PageUtils.Combine(parentPublishmentSystemInfo.PublishmentSystemUrl, psInfo.PublishmentSystemDir);
                }

                psInfo.IsHeadquarters = isHq;

                psInfo.Additional.Charset = Charset.SelectedValue;
                psInfo.IsCheckContentUseLevel = TranslateUtils.ToBool(IsCheckContentUseLevel.SelectedValue);
                if (psInfo.IsCheckContentUseLevel)
                {
                    psInfo.CheckContentLevel = TranslateUtils.ToInt(CheckContentLevel.SelectedValue);
                }

                var thePublishmentSystemId = DataProvider.NodeDao.InsertPublishmentSystemInfo(nodeInfo, psInfo, Body.AdministratorName);

                if (_permissions.IsSystemAdministrator && !_permissions.IsConsoleAdministrator)
                {
                    var publishmentSystemIdList = ProductPermissionsManager.Current.PublishmentSystemIdList ?? new List<int>();
                    publishmentSystemIdList.Add(thePublishmentSystemId);
                    BaiRongDataProvider.AdministratorDao.UpdatePublishmentSystemIdCollection(Body.AdministratorName, TranslateUtils.ObjectCollectionToString(publishmentSystemIdList));
                }

                Body.AddAdminLog($"新建{EPublishmentSystemTypeUtils.GetText(_publishmentSystemType)}站点", $"站点名称:{PageUtils.FilterXss(PublishmentSystemName.Text)}");

                //if (isHQ == EBoolean.False)
                //{
                //    string configFilePath = PathUtility.MapPath(psInfo, "@/web.config");
                //    if (FileUtils.IsFileExists(configFilePath))
                //    {
                //        FileUtility.UpdateWebConfig(configFilePath, psInfo.Additional.Charset);
                //    }
                //    else
                //    {
                //        FileUtility.CreateWebConfig(configFilePath, psInfo.Additional.Charset);
                //    }
                //}
                errorMessage = string.Empty;
                return thePublishmentSystemId;
            }
            catch (Exception e)
            {
                errorMessage = e.Message;
                return 0;
            }
        }

        public void NextPlaceHolder(object sender, EventArgs e)
        {
            switch (CurrentWizardPlaceHolder)
            {
                case WizardPlaceHolder.ChooseSiteTemplate:
                    if (UseSiteTemplate.Checked)
                    {
                        RowSiteTemplateName.Visible = RowIsImportContents.Visible = RowIsImportTableStyles.Visible = RowIsUserSiteTemplateAuxiliaryTables.Visible = true;
                        phAuxiliaryTable.Visible = true;
                        SiteTemplateName.Text =
                            $"{GetSiteTemplateName(SiteTemplateDir.Value)}（{SiteTemplateDir.Value}）";

                        var siteTemplatePath = PathUtility.GetSiteTemplatesPath(SiteTemplateDir.Value);
                        var filePath = PathUtility.GetSiteTemplateMetadataPath(siteTemplatePath, DirectoryUtils.SiteTemplates.FileConfiguration);
                        var publishmentSystemInfo = ConfigurationIe.GetPublishmentSytemInfo(filePath);

                        PublishmentSystemName.Text = publishmentSystemInfo.PublishmentSystemName;
                        PublishmentSystemDir.Text = publishmentSystemInfo.PublishmentSystemDir;
                        var extend = new PublishmentSystemInfoExtend(publishmentSystemInfo.SettingsXml);
                        if (!string.IsNullOrEmpty(extend.Charset))
                        {
                            Charset.SelectedValue = extend.Charset;
                        }
                    }
                    else
                    {
                        RowSiteTemplateName.Visible = RowIsImportContents.Visible = RowIsImportTableStyles.Visible = RowIsUserSiteTemplateAuxiliaryTables.Visible = false;
                        phAuxiliaryTable.Visible = true;
                    }
                    SetActivePlaceHolder(WizardPlaceHolder.CreateSiteParameters, CreateSiteParameters);
                    break;

                case WizardPlaceHolder.CreateSiteParameters:
                    string errorMessage;
                    var thePublishmentSystemId = Validate_PublishmentSystemInfo(out errorMessage);
                    if (thePublishmentSystemId > 0)
                    {
                        var url = PageProgressBar.GetCreatePublishmentSystemUrl(thePublishmentSystemId, UseSiteTemplate.Checked, IsImportContents.Checked, IsImportTableStyles.Checked, SiteTemplateDir.Value, bool.Parse(IsUserSiteTemplateAuxiliaryTables.SelectedValue), StringUtils.Guid(), string.Empty);
                        PageUtils.Redirect(url);
                    }
                    else
                    {
                        ltlErrorMessage.Text = errorMessage;
                        SetActivePlaceHolder(WizardPlaceHolder.OperatingError, OperatingError);
                    }
                    break;
            }
        }

        public void PreviousPlaceHolder(object sender, EventArgs e)
        {
            switch (CurrentWizardPlaceHolder)
            {
                case WizardPlaceHolder.ChooseSiteTemplate:
                    break;

                case WizardPlaceHolder.CreateSiteParameters:
                    AddScript("displaySiteTemplateDiv(document.all.UseSiteTemplate);");
                    SetActivePlaceHolder(WizardPlaceHolder.ChooseSiteTemplate, ChooseSiteTemplate);
                    break;
            }
        }

    }
}
