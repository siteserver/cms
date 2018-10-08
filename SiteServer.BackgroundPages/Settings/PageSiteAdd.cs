using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using SiteServer.Utils;
using SiteServer.BackgroundPages.Cms;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Model;
using SiteServer.CMS.Model.Enumerations;
using SiteServer.CMS.Provider;
using SiteServer.Utils.Enumerations;

namespace SiteServer.BackgroundPages.Settings
{
    public class PageSiteAdd : BasePageCms
    {
        protected override bool IsSinglePage => true;

        public PlaceHolder PhSource;
        public RadioButtonList RblSource;
        public HtmlInputHidden HihSiteTemplateDir;
        public HtmlInputHidden HihOnlineTemplateName;
        public PlaceHolder PhSiteTemplates;
        public Repeater RptSiteTemplates;
        public PlaceHolder PhOnlineTemplates;
        public Repeater RptOnlineTemplates;
        public PlaceHolder PhSubmit;
        public Literal LtlSource;
        public TextBox TbSiteName;
        public RadioButtonList RblIsRoot;
        public PlaceHolder PhIsNotRoot;
        public DropDownList DdlParentId;
        public TextBox TbSiteDir;
        public DropDownList DdlCharset;
        public PlaceHolder PhIsImportContents;
        public CheckBox CbIsImportContents;
        public PlaceHolder PhIsImportTableStyles;
        public CheckBox CbIsImportTableStyles;

        public RadioButtonList RblTableRule;
        public PlaceHolder PhTableChoose;
        public DropDownList DdlTableChoose;
        public PlaceHolder PhTableHandWrite;
        public TextBox TbTableHandWrite;

        public RadioButtonList RblIsCheckContentUseLevel;
        public PlaceHolder PhCheckContentLevel;
        public DropDownList DdlCheckContentLevel;

        public Button BtnPrevious;
        public Button BtnNext;
        public Button BtnSubmit;

        public static string GetRedirectUrl()
        {
            return PageUtils.GetSettingsUrl(nameof(PageSiteAdd), null);
        }

        public static string GetRedirectUrl(string siteTemplateDir, string onlineTemplateName)
        {
            return PageUtils.GetSettingsUrl(nameof(PageSiteAdd), new NameValueCollection
            {
                {"siteTemplateDir", siteTemplateDir},
                {"onlineTemplateName", onlineTemplateName}
            });
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            if (IsPostBack) return;

            VerifySystemPermissions(ConfigManager.SettingsPermissions.SiteAdd);

            //DataProvider.TableDao.CreateAllTableCollectionInfoIfNotExists();

            var hqSiteId = DataProvider.SiteDao.GetIdByIsRoot();
            if (hqSiteId == 0)
            {
                ControlUtils.SelectSingleItem(RblIsRoot, true.ToString());
                PhIsNotRoot.Visible = false;
            }
            else
            {
                RblIsRoot.Enabled = false;
            }

            DdlParentId.Items.Add(new ListItem("<无上级站点>", "0"));
            var siteIdArrayList = SiteManager.GetSiteIdList();
            var mySystemInfoArrayList = new ArrayList();
            var parentWithChildren = new Hashtable();
            foreach (var siteId in siteIdArrayList)
            {
                var siteInfo = SiteManager.GetSiteInfo(siteId);
                if (siteInfo.IsRoot == false)
                {
                    if (siteInfo.ParentId == 0)
                    {
                        mySystemInfoArrayList.Add(siteInfo);
                    }
                    else
                    {
                        var children = new ArrayList();
                        if (parentWithChildren.Contains(siteInfo.ParentId))
                        {
                            children = (ArrayList)parentWithChildren[siteInfo.ParentId];
                        }
                        children.Add(siteInfo);
                        parentWithChildren[siteInfo.ParentId] = children;
                    }
                }
            }
            foreach (SiteInfo siteInfo in mySystemInfoArrayList)
            {
                AddSite(DdlParentId, siteInfo, parentWithChildren, 0);
            }
            ControlUtils.SelectSingleItem(DdlParentId, "0");

            ECharsetUtils.AddListItems(DdlCharset);
            ControlUtils.SelectSingleItem(DdlCharset, ECharsetUtils.GetValue(ECharset.utf_8));

            var tableNameList = SiteManager.GetSiteTableNames();
            if (tableNameList.Count > 0)
            {
                RblTableRule.Items.Add(ETableRuleUtils.GetListItem(ETableRule.Choose, true));
                RblTableRule.Items.Add(ETableRuleUtils.GetListItem(ETableRule.Create, false));
                RblTableRule.Items.Add(ETableRuleUtils.GetListItem(ETableRule.HandWrite, false));

                PhTableChoose.Visible = true;
                PhTableHandWrite.Visible = false;

                foreach (var tableName in tableNameList)
                {
                    DdlTableChoose.Items.Add(new ListItem(tableName, tableName));
                }
            }
            else
            {
                RblTableRule.Items.Add(ETableRuleUtils.GetListItem(ETableRule.Create, true));
                RblTableRule.Items.Add(ETableRuleUtils.GetListItem(ETableRule.HandWrite, false));

                PhTableChoose.Visible = false;
                PhTableHandWrite.Visible = false;
            }

            RblIsCheckContentUseLevel.Items.Add(new ListItem("默认审核机制", false.ToString()));
            RblIsCheckContentUseLevel.Items.Add(new ListItem("多级审核机制", true.ToString()));
            ControlUtils.SelectSingleItem(RblIsCheckContentUseLevel, false.ToString());

            if (SiteTemplateManager.Instance.IsSiteTemplateExists)
            {
                RblSource.Items.Add(new ListItem("创建空站点（不使用站点模板）", ETriStateUtils.GetValue(ETriState.True)));
                RblSource.Items.Add(new ListItem("使用本地站点模板创建站点", ETriStateUtils.GetValue(ETriState.False)));
                RblSource.Items.Add(new ListItem("使用在线站点模板创建站点", ETriStateUtils.GetValue(ETriState.All)));
            }
            else
            {
                RblSource.Items.Add(new ListItem("创建空站点（不使用站点模板）", ETriStateUtils.GetValue(ETriState.True)));
                RblSource.Items.Add(new ListItem("使用在线站点模板创建站点", ETriStateUtils.GetValue(ETriState.All)));
            }
            ControlUtils.SelectSingleItem(RblSource, ETriStateUtils.GetValue(ETriState.True));

            var siteTemplateDir = AuthRequest.GetQueryString("siteTemplateDir");
            var onlineTemplateName = AuthRequest.GetQueryString("onlineTemplateName");

            if (!string.IsNullOrEmpty(siteTemplateDir))
            {
                HihSiteTemplateDir.Value = siteTemplateDir;
                ControlUtils.SelectSingleItem(RblSource, ETriStateUtils.GetValue(ETriState.False));
                BtnNext_Click(null, EventArgs.Empty);
            }
            else if (!string.IsNullOrEmpty(onlineTemplateName))
            {
                HihOnlineTemplateName.Value = onlineTemplateName;
                ControlUtils.SelectSingleItem(RblSource, ETriStateUtils.GetValue(ETriState.All));
                BtnNext_Click(null, EventArgs.Empty);
            }

            BtnSubmit.Attributes.Add("onclick", PageLoading());
            BtnSubmit.Attributes.Add("onclick", PageLoading());
        }

        private bool IsSiteTemplate => ETriStateUtils.GetEnumType(RblSource.SelectedValue) == ETriState.False;

        private bool IsOnlineTemplate => ETriStateUtils.GetEnumType(RblSource.SelectedValue) == ETriState.All;

        public void RblIsRoot_SelectedIndexChanged(object sender, EventArgs e)
        {
            PhIsNotRoot.Visible = !TranslateUtils.ToBool(RblIsRoot.SelectedValue);
        }

        public void RblIsCheckContentUseLevel_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            PhCheckContentLevel.Visible = EBooleanUtils.Equals(RblIsCheckContentUseLevel.SelectedValue, EBoolean.True);
        }

        public void RblTableRule_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            var tableRule = ETableRuleUtils.GetEnumType(RblTableRule.SelectedValue);
            PhTableChoose.Visible = PhTableHandWrite.Visible = false;
            if (tableRule == ETableRule.Choose)
            {
                PhTableChoose.Visible = true;
            }
            else if (tableRule == ETableRule.HandWrite)
            {
                PhTableHandWrite.Visible = true;
            }
        }

        public void BtnNext_Click(object sender, EventArgs e)
        {
            if (PhSource.Visible)
            {
                HideAll();

                if (IsSiteTemplate)
                {
                    var siteTemplates = SiteTemplateManager.Instance.GetSiteTemplateSortedList();

                    RptSiteTemplates.DataSource = siteTemplates.Values;
                    RptSiteTemplates.ItemDataBound += RptSiteTemplates_ItemDataBound;
                    RptSiteTemplates.DataBind();

                    ShowSiteTemplates();
                }
                else if (IsOnlineTemplate)
                {
                    List<Dictionary<string, string>> list;
                    if (OnlineTemplateManager.TryGetOnlineTemplates(out list))
                    {
                        RptOnlineTemplates.DataSource = list;
                        RptOnlineTemplates.ItemDataBound += RptOnlineTemplates_ItemDataBound;
                        RptOnlineTemplates.DataBind();

                        ShowOnlineTemplates();
                    }
                    else
                    {
                        FailMessage($"在线模板获取失败：页面地址{OnlineTemplateManager.UrlHome}无法访问！");

                        ShowSource();
                    }
                }
                else
                {
                    LtlSource.Text = "创建空站点（不使用站点模板）";

                    ShowSubmit();
                }
            }
            else if (PhSiteTemplates.Visible)
            {
                HideAll();

                var siteTemplateDir = HihSiteTemplateDir.Value;

                if (string.IsNullOrEmpty(siteTemplateDir))
                {
                    FailMessage("请选择需要使用的站点模板");
                    ShowSiteTemplates();
                    return;
                }

                LtlSource.Text = $"使用本地站点模板创建站点（{siteTemplateDir}）";

                ShowSubmit();
            }
            else if (PhOnlineTemplates.Visible)
            {
                HideAll();

                var onlineTemplateName = HihOnlineTemplateName.Value;

                if (string.IsNullOrEmpty(onlineTemplateName))
                {
                    FailMessage("请选择需要使用的在线站点模板");
                    ShowOnlineTemplates();
                    return;
                }

                LtlSource.Text = $@"使用在线站点模板创建站点（<a href=""{OnlineTemplateManager.GetTemplateUrl(onlineTemplateName)}"" target=""_blank"">{onlineTemplateName}</a>）";

                ShowSubmit();
            }
        }

        public void BtnSubmit_Click(object sender, EventArgs e)
        {
            HideAll();

            string errorMessage;
            var theSiteId = Validate_SiteInfo(out errorMessage);
            if (theSiteId > 0)
            {
                var siteTemplateDir = IsSiteTemplate ? HihSiteTemplateDir.Value : string.Empty;
                var onlineTemplateName = IsOnlineTemplate ? HihOnlineTemplateName.Value : string.Empty;
                PageUtils.Redirect(PageProgressBar.GetCreateSiteUrl(theSiteId,
                    CbIsImportContents.Checked, CbIsImportTableStyles.Checked, siteTemplateDir, onlineTemplateName, StringUtils.Guid()));
            }
            else
            {
                FailMessage(errorMessage);

                ShowSubmit();
            }
        }

        public void BtnPrevious_Click(object sender, EventArgs e)
        {
            if (PhSiteTemplates.Visible || PhOnlineTemplates.Visible)
            {
                HideAll();
                ShowSource();
            }
            else if (PhSubmit.Visible)
            {
                HideAll();

                if (IsSiteTemplate)
                {
                    ShowSiteTemplates();
                }
                else if (IsOnlineTemplate)
                {
                    ShowOnlineTemplates();
                }
                else
                {
                    ShowSource();
                }
            }
        }

        private void RptSiteTemplates_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.AlternatingItem && e.Item.ItemType != ListItemType.Item) return;

            var templateInfo = (SiteTemplateInfo)e.Item.DataItem;
            if (templateInfo == null) return;

            var directoryPath = PathUtility.GetSiteTemplatesPath(templateInfo.DirectoryName);
            var directoryInfo = new DirectoryInfo(directoryPath);

            var ltlChoose = (Literal)e.Item.FindControl("ltlChoose");
            var ltlTemplateName = (Literal)e.Item.FindControl("ltlTemplateName");
            var ltlName = (Literal)e.Item.FindControl("ltlName");
            var ltlDescription = (Literal)e.Item.FindControl("ltlDescription");
            var ltlSamplePic = (Literal)e.Item.FindControl("ltlSamplePic");

            ltlChoose.Text = $@"<input type=""radio"" name=""choose"" id=""choose_{directoryInfo.Name}"" onClick=""document.getElementById('{HihSiteTemplateDir.ClientID}').value=this.value;"" {(HihSiteTemplateDir.Value == directoryInfo.Name ? "checked" : string.Empty)} value=""{directoryInfo.Name}"" /><label for=""choose_{directoryInfo.Name}"">选中</label>";

            if (!string.IsNullOrEmpty(templateInfo.SiteTemplateName))
            {
                ltlTemplateName.Text = !string.IsNullOrEmpty(templateInfo.WebSiteUrl) ? $@"<a href=""{PageUtils.ParseConfigRootUrl(templateInfo.WebSiteUrl)}"" target=""_blank"">{templateInfo.SiteTemplateName}</a>" : templateInfo.SiteTemplateName;
            }

            ltlName.Text = directoryInfo.Name;

            if (!string.IsNullOrEmpty(templateInfo.Description))
            {
                ltlDescription.Text = templateInfo.Description;
            }

            if (!string.IsNullOrEmpty(templateInfo.PicFileName))
            {
                var siteTemplateUrl = PageUtils.GetSiteTemplatesUrl(directoryInfo.Name);
                var picFileName = PageUtils.GetSiteTemplateMetadataUrl(siteTemplateUrl, templateInfo.PicFileName);
                ltlSamplePic.Text = $@"<a href=""{picFileName}"" target=""_blank"">样图</a>";
            }
        }

        private void RptOnlineTemplates_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.AlternatingItem && e.Item.ItemType != ListItemType.Item) return;

            var dict = (Dictionary<string, string>)e.Item.DataItem;
            var title = dict["title"];
            var description = dict["description"];
            var author = dict["author"];
            var source = dict["source"];
            var lastEditDate = dict["lastEditDate"];

            var ltlChoose = (Literal)e.Item.FindControl("ltlChoose");
            var ltlTitle = (Literal)e.Item.FindControl("ltlTitle");
            var ltlDescription = (Literal)e.Item.FindControl("ltlDescription");
            var ltlAuthor = (Literal)e.Item.FindControl("ltlAuthor");
            var ltlLastEditDate = (Literal)e.Item.FindControl("ltlLastEditDate");
            var ltlPreviewUrl = (Literal)e.Item.FindControl("ltlPreviewUrl");

            ltlChoose.Text = $@"<input type=""radio"" name=""choose"" id=""choose_{title}"" onClick=""document.getElementById('{HihOnlineTemplateName.ClientID}').value=this.value;"" {(HihOnlineTemplateName.Value == title ? "checked" : string.Empty)} value=""{title}"" /><label for=""choose_{title}"" class=""m-l-10"">选中</label>";

            var templateUrl = OnlineTemplateManager.GetTemplateUrl(title);

            ltlTitle.Text = $@"<a href=""{templateUrl}"" target=""_blank"">{title}</a>";

            ltlDescription.Text = description;
            ltlAuthor.Text = author;
            if (!string.IsNullOrEmpty(source) && PageUtils.IsProtocolUrl(source))
            {
                ltlAuthor.Text = $@"<a href=""{source}"" target=""_blank"">{ltlAuthor.Text}</a>";
            }
            ltlLastEditDate.Text = lastEditDate;

            ltlPreviewUrl.Text = $@"<a href=""{templateUrl}"" target=""_blank"">模板详情</a>";
        }

        private int Validate_SiteInfo(out string errorMessage)
        {
            try
            {
                var isRoot = TranslateUtils.ToBool(RblIsRoot.SelectedValue); // 是否主站
                var parentSiteId = 0;
                var siteDir = string.Empty;

                if (isRoot == false)
                {
                    if (DirectoryUtils.IsSystemDirectory(TbSiteDir.Text))
                    {
                        errorMessage = "文件夹名称不能为系统文件夹名称！";
                        return 0;
                    }

                    parentSiteId = TranslateUtils.ToInt(DdlParentId.SelectedValue);
                    siteDir = TbSiteDir.Text;

                    var list = DataProvider.SiteDao.GetLowerSiteDirList(parentSiteId);
                    if (list.IndexOf(siteDir.ToLower()) != -1)
                    {
                        errorMessage = "已存在相同的发布路径！";
                        return 0;
                    }

                    if (!DirectoryUtils.IsDirectoryNameCompliant(siteDir))
                    {
                        errorMessage = "文件夹名称不符合系统要求！";
                        return 0;
                    }
                }

                var nodeInfo = new ChannelInfo();

                nodeInfo.ChannelName = nodeInfo.IndexName = "首页";
                nodeInfo.ParentId = 0;
                nodeInfo.ContentModelPluginId = string.Empty;

                var tableName = string.Empty;
                var tableRule = ETableRuleUtils.GetEnumType(RblTableRule.SelectedValue);
                if (tableRule == ETableRule.Choose)
                {
                    tableName = DdlTableChoose.SelectedValue;
                }
                else if (tableRule == ETableRule.HandWrite)
                {
                    tableName = TbTableHandWrite.Text;
                    if (!DataProvider.DatabaseDao.IsTableExists(tableName))
                    {
                        DataProvider.ContentDao.CreateContentTable(tableName, DataProvider.ContentDao.TableColumnsDefault);
                    }
                    else
                    {
                        DataProvider.DatabaseDao.AlterSystemTable(tableName, DataProvider.ContentDao.TableColumnsDefault);
                    }
                }

                var siteInfo = new SiteInfo
                {
                    SiteName = AttackUtils.FilterXss(TbSiteName.Text),
                    SiteDir = siteDir,
                    TableName = tableName,
                    ParentId = parentSiteId,
                    IsRoot = isRoot
                };

                siteInfo.Additional.IsCheckContentLevel = TranslateUtils.ToBool(RblIsCheckContentUseLevel.SelectedValue);

                if (siteInfo.Additional.IsCheckContentLevel)
                {
                    siteInfo.Additional.CheckContentLevel = TranslateUtils.ToInt(DdlCheckContentLevel.SelectedValue);
                }
                siteInfo.Additional.Charset = DdlCharset.SelectedValue;

                var siteId = DataProvider.ChannelDao.InsertSiteInfo(nodeInfo, siteInfo, AuthRequest.AdminName);

                if (string.IsNullOrEmpty(tableName))
                {
                    tableName = ContentDao.GetContentTableName(siteId);
                    DataProvider.ContentDao.CreateContentTable(tableName, DataProvider.ContentDao.TableColumnsDefault);
                    DataProvider.SiteDao.UpdateTableName(siteId, tableName);
                }

                if (AuthRequest.AdminPermissionsImpl.IsSystemAdministrator && !AuthRequest.AdminPermissionsImpl.IsConsoleAdministrator)
                {
                    var siteIdList = AuthRequest.AdminPermissionsImpl.GetSiteIdList() ?? new List<int>();
                    siteIdList.Add(siteId);
                    var adminInfo = AdminManager.GetAdminInfoByUserId(AuthRequest.AdminId);
                    DataProvider.AdministratorDao.UpdateSiteIdCollection(adminInfo, TranslateUtils.ObjectCollectionToString(siteIdList));
                }

                AuthRequest.AddAdminLog("创建新站点", $"站点名称：{AttackUtils.FilterXss(TbSiteName.Text)}");

                errorMessage = string.Empty;
                return siteId;
            }
            catch (Exception e)
            {
                errorMessage = e.Message;
                return 0;
            }
        }

        private static void AddSite(ListControl listControl, SiteInfo siteInfo, Hashtable parentWithChildren, int level)
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

            if (parentWithChildren[siteInfo.Id] != null)
            {
                var children = (ArrayList)parentWithChildren[siteInfo.Id];
                listControl.Items.Add(new ListItem(padding + siteInfo.SiteName + $"({children.Count})", siteInfo.Id.ToString()));
                level++;
                foreach (SiteInfo subSiteInfo in children)
                {
                    AddSite(listControl, subSiteInfo, parentWithChildren, level);
                }
            }
            else
            {
                listControl.Items.Add(new ListItem(padding + siteInfo.SiteName, siteInfo.Id.ToString()));
            }
        }

        private void HideAll()
        {
            PhSource.Visible =
                PhSiteTemplates.Visible =
                    PhOnlineTemplates.Visible =
                        PhSubmit.Visible = PhIsImportContents.Visible =
                            PhIsImportTableStyles.Visible =
                                BtnPrevious.Enabled = BtnNext.Visible = BtnSubmit.Visible = false;
        }

        private void ShowSource()
        {
            PhSource.Visible = BtnNext.Visible = true;
        }

        private void ShowSiteTemplates()
        {
            PhSiteTemplates.Visible = BtnPrevious.Enabled = BtnNext.Visible = true;
        }

        private void ShowOnlineTemplates()
        {
            PhOnlineTemplates.Visible = BtnPrevious.Enabled = BtnNext.Visible = true;
        }

        private void ShowSubmit()
        {
            if (IsSiteTemplate)
            {
                PhSubmit.Visible =
                    PhIsImportContents.Visible =
                        PhIsImportTableStyles.Visible =
                            BtnPrevious.Enabled = BtnSubmit.Visible = true;
            }
            else if (IsOnlineTemplate)
            {
                PhSubmit.Visible =
                    PhIsImportContents.Visible =
                        PhIsImportTableStyles.Visible =
                            BtnPrevious.Enabled = BtnSubmit.Visible = true;
            }
            else
            {
                PhSubmit.Visible = BtnPrevious.Enabled = BtnSubmit.Visible = true;
            }
        }
    }
}
