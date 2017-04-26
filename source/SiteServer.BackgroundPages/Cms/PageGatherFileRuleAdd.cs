using System;
using System.Collections;
using System.Collections.Specialized;
using System.Web.UI;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.AuxiliaryTable;
using BaiRong.Core.Model;
using BaiRong.Core.Model.Attributes;
using BaiRong.Core.Model.Enumerations;
using SiteServer.BackgroundPages.Controls;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;

namespace SiteServer.BackgroundPages.Cms
{
	public class PageGatherFileRuleAdd : BasePageCms
    {
        public Literal ltlPageTitle;
		public PlaceHolder GatherRuleBase;
		public TextBox GatherRuleName;
		public TextBox GatherUrl;
        public DropDownList Charset;
        
        public DropDownList IsToFile;

        public PlaceHolder PlaceHolder_File;
        public TextBox FilePath;
        public RadioButtonList IsSaveRelatedFiles;
        public RadioButtonList IsRemoveScripts;
        public PlaceHolder PlaceHolder_File_Directory;
        public TextBox StyleDirectoryPath;
        public TextBox ScriptDirectoryPath;
        public TextBox ImageDirectoryPath;

        public PlaceHolder PlaceHolder_Content;
		public DropDownList NodeIDDropDownList;
        public RadioButtonList IsSaveImage;
        public RadioButtonList IsChecked;
        public RadioButtonList IsAutoCreate;


        public PlaceHolder GatherRuleContent;
		public TextBox ContentTitleStart;
		public TextBox ContentTitleEnd;
        public TextBox ContentExclude;
		public CheckBoxList ContentHtmlClearCollection;
        public CheckBoxList ContentHtmlClearTagCollection;
		public TextBox ContentContentStart;
		public TextBox ContentContentEnd;
        public CheckBoxList ContentAttributes;
        public Repeater ContentAttributesRepeater;

		public PlaceHolder Done;

		public PlaceHolder OperatingError;
		public Label ErrorLabel;

		public Button Previous;
		public Button Next;

		private bool _isEdit;
		private string _theGatherRuleName;
        private NameValueCollection _contentAttributesXml;

        public static string GetRedirectUrl(int publishmentSystemId, string gatherRuleName)
        {
            return PageUtils.GetCmsUrl(nameof(PageGatherFileRuleAdd), new NameValueCollection
            {
                {"PublishmentSystemID", publishmentSystemId.ToString()},
                {"GatherRuleName", gatherRuleName}
            });
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("PublishmentSystemID");

			if (Body.IsQueryExists("GatherRuleName"))
			{
				_isEdit = true;
                _theGatherRuleName = Body.GetQueryString("GatherRuleName");
			}

			if (!Page.IsPostBack)
            {
                var pageTitle = _isEdit ? "编辑单文件页采集规则" : "添加单文件页采集规则";
                BreadCrumb(AppManager.Cms.LeftMenu.IdFunction, AppManager.Cms.LeftMenu.Function.IdGather, pageTitle, AppManager.Cms.Permission.WebSite.Gather);

                ltlPageTitle.Text = pageTitle;

                ECharsetUtils.AddListItems(Charset);
                ControlUtils.SelectListItemsIgnoreCase(Charset, ECharsetUtils.GetValue(ECharset.utf_8));
                NodeManager.AddListItemsForAddContent(NodeIDDropDownList.Items, PublishmentSystemInfo, true, Body.AdministratorName);

				SetActivePanel(WizardPanel.GatherRuleBase, GatherRuleBase);

				if (_isEdit)
				{
                    var gatherFileRuleInfo = DataProvider.GatherFileRuleDao.GetGatherFileRuleInfo(_theGatherRuleName, PublishmentSystemId);
					GatherRuleName.Text = gatherFileRuleInfo.GatherRuleName;
                    GatherUrl.Text = gatherFileRuleInfo.GatherUrl;
                    ControlUtils.SelectListItemsIgnoreCase(Charset, ECharsetUtils.GetValue(gatherFileRuleInfo.Charset));

                    ControlUtils.SelectListItems(IsToFile, gatherFileRuleInfo.IsToFile.ToString());
                    FilePath.Text = gatherFileRuleInfo.FilePath;
                    ControlUtils.SelectListItems(IsSaveRelatedFiles, gatherFileRuleInfo.IsSaveRelatedFiles.ToString());
                    ControlUtils.SelectListItems(IsRemoveScripts, gatherFileRuleInfo.IsRemoveScripts.ToString());
                    StyleDirectoryPath.Text = gatherFileRuleInfo.StyleDirectoryPath;
                    ScriptDirectoryPath.Text = gatherFileRuleInfo.ScriptDirectoryPath;
                    ImageDirectoryPath.Text = gatherFileRuleInfo.ImageDirectoryPath;

                    ControlUtils.SelectListItems(NodeIDDropDownList, gatherFileRuleInfo.NodeId.ToString());
                    ControlUtils.SelectListItems(IsSaveImage, gatherFileRuleInfo.IsSaveImage.ToString());
                    ControlUtils.SelectListItems(IsChecked, gatherFileRuleInfo.IsChecked.ToString());
                    ControlUtils.SelectListItems(IsAutoCreate, gatherFileRuleInfo.IsAutoCreate.ToString());

                    ContentExclude.Text = gatherFileRuleInfo.ContentExclude;
					var htmlClearArrayList = TranslateUtils.StringCollectionToStringList(gatherFileRuleInfo.ContentHtmlClearCollection);
					foreach (ListItem item in ContentHtmlClearCollection.Items)
					{
						if (htmlClearArrayList.Contains(item.Value)) item.Selected = true;
					}
                    var htmlClearTagArrayList = TranslateUtils.StringCollectionToStringList(gatherFileRuleInfo.ContentHtmlClearTagCollection);
                    foreach (ListItem item in ContentHtmlClearTagCollection.Items)
                    {
                        if (htmlClearTagArrayList.Contains(item.Value)) item.Selected = true;
                    }
					ContentTitleStart.Text = gatherFileRuleInfo.ContentTitleStart;
					ContentTitleEnd.Text = gatherFileRuleInfo.ContentTitleEnd;
					ContentContentStart.Text = gatherFileRuleInfo.ContentContentStart;
					ContentContentEnd.Text = gatherFileRuleInfo.ContentContentEnd;

                    var contentAttributeArrayList = TranslateUtils.StringCollectionToStringList(gatherFileRuleInfo.ContentAttributes);
                    var styleInfoList = TableStyleManager.GetTableStyleInfoList(ETableStyle.BackgroundContent, PublishmentSystemInfo.AuxiliaryTableForContent, null);
                    foreach (var styleInfo in styleInfoList)
                    {
                        if (StringUtils.EqualsIgnoreCase(styleInfo.AttributeName, ContentAttribute.Title) || StringUtils.EqualsIgnoreCase(styleInfo.AttributeName, BackgroundContentAttribute.Content)) continue;

                        var listitem = new ListItem(styleInfo.DisplayName, styleInfo.AttributeName.ToLower());
                        if (contentAttributeArrayList.Contains(listitem.Value))
                        {
                            listitem.Selected = true;
                        }
                        ContentAttributes.Items.Add(listitem);
                    }

                    _contentAttributesXml = TranslateUtils.ToNameValueCollection(gatherFileRuleInfo.ContentAttributesXml);
                    
                    ContentAttributes_SelectedIndexChanged(null, EventArgs.Empty);
                    
				}

                DropDownList_SelectedIndexChanged(null, EventArgs.Empty);
			}			

			SuccessMessage(string.Empty);
		}

        

        private WizardPanel CurrentWizardPanel
		{
			get
			{
				if (ViewState["WizardPanel"] != null)
					return (WizardPanel)ViewState["WizardPanel"];

				return WizardPanel.GatherRuleBase;
			}
			set
			{
				ViewState["WizardPanel"] = value;
			}
		}


		private enum WizardPanel
		{
			GatherRuleBase,
			GatherRuleContent,
			OperatingError,
			Done
		}

		void SetActivePanel(WizardPanel panel, Control controlToShow)
		{
			var currentPanel = FindControl(CurrentWizardPanel.ToString()) as PlaceHolder;
			if (currentPanel != null)
				currentPanel.Visible = false;

			switch (panel)
			{
				case WizardPanel.GatherRuleBase:
					Previous.CssClass = "btn disabled";
                    Previous.Enabled = false;
					break;
				case WizardPanel.Done:
					Previous.CssClass = "btn disabled";
                    Previous.Enabled = false;
                    Next.CssClass = "btn btn-primary disabled";
                    Next.Enabled = false;
                    AddWaitAndRedirectScript(PageGatherFileRule.GetRedirectUrl(PublishmentSystemId));
					break;
				case WizardPanel.OperatingError:
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
			CurrentWizardPanel = panel;
		}

		public bool Validate_GatherRuleBase(out string errorMessage)
		{
			if (string.IsNullOrEmpty(GatherRuleName.Text))
			{
				errorMessage = "必须填写采集规则名称！";
				return false;
			}

			if (string.IsNullOrEmpty(GatherUrl.Text))
			{
                errorMessage = "必须填写采集网页地址！";
				return false;
			}

			if (_isEdit == false)
			{
                var gatherRuleNameList = DataProvider.GatherFileRuleDao.GetGatherRuleNameArrayList(PublishmentSystemId);
				if (gatherRuleNameList.IndexOf(GatherRuleName.Text) != -1)
				{
					errorMessage = "采集规则名称已存在！";
					return false;
				}
			}

            if (TranslateUtils.ToBool(IsToFile.SelectedValue))
            {
                if (string.IsNullOrEmpty(FilePath.Text))
                {
                    errorMessage = "必须填写采集到的文件地址！";
                    return false;
                }
                else
                {
                    var isOk = false;
                    if (StringUtils.StringStartsWith(FilePath.Text, '~') || StringUtils.StringStartsWith(FilePath.Text, '@'))
                    {
                        if (!PathUtils.IsDirectoryPath(FilePath.Text))
                        {
                            isOk = true;
                        }
                    }
                    if (isOk == false)
                    {
                        errorMessage = "采集到的文件地址不正确,必须填写有效的文件地址！";
                        return false;
                    }
                }

                if (TranslateUtils.ToBool(IsSaveRelatedFiles.SelectedValue))
                {
                    var isOk = false;
                    if (StringUtils.StringStartsWith(StyleDirectoryPath.Text, '~') || StringUtils.StringStartsWith(StyleDirectoryPath.Text, '@'))
                    {
                        if (PathUtils.IsDirectoryPath(StyleDirectoryPath.Text))
                        {
                            isOk = true;
                        }
                    }
                    if (isOk == false)
                    {
                        errorMessage = "CSS样式保存地址不正确,必须填写有效的文件夹地址！";
                        return false;
                    }

                    isOk = false;
                    if (StringUtils.StringStartsWith(ScriptDirectoryPath.Text, '~') || StringUtils.StringStartsWith(ScriptDirectoryPath.Text, '@'))
                    {
                        if (PathUtils.IsDirectoryPath(ScriptDirectoryPath.Text))
                        {
                            isOk = true;
                        }
                    }
                    if (isOk == false)
                    {
                        errorMessage = "Js脚本保存地址不正确,必须填写有效的文件夹地址！";
                        return false;
                    }
                    
                    isOk = false;
                    if (StringUtils.StringStartsWith(ImageDirectoryPath.Text, '~') || StringUtils.StringStartsWith(ImageDirectoryPath.Text, '@'))
                    {
                        if (PathUtils.IsDirectoryPath(ImageDirectoryPath.Text))
                        {
                            isOk = true;
                        }
                    }
                    if (isOk == false)
                    {
                        errorMessage = "图片保存地址不正确,必须填写有效的文件夹地址！";
                        return false;
                    }
                }
            }

			errorMessage = string.Empty;
			return true;
		}

		public bool Validate_GatherContent(out string errorMessage)
		{
            if (!TranslateUtils.ToBool(IsToFile.SelectedValue))
            {
                if (string.IsNullOrEmpty(ContentTitleStart.Text) || string.IsNullOrEmpty(ContentTitleEnd.Text))
                {
                    errorMessage = "必须填写内容标题规则！";
                    return false;
                }
                else if (string.IsNullOrEmpty(ContentContentStart.Text) || string.IsNullOrEmpty(ContentContentEnd.Text))
                {
                    errorMessage = "必须填写内容正文规则！";
                    return false;
                }
            }
			errorMessage = string.Empty;
			return true;
		}

		public bool Validate_InsertGatherRule(out string errorMessage)
		{
			try
			{
                var isNeedAdd = false;
				if (_isEdit)
				{
                    if (_theGatherRuleName != GatherRuleName.Text)
                    {
                        isNeedAdd = true;
                        DataProvider.GatherDatabaseRuleDao.Delete(_theGatherRuleName, PublishmentSystemId);
                    }
                    else
                    {
                        var gatherFileRuleInfo =
                            DataProvider.GatherFileRuleDao.GetGatherFileRuleInfo(_theGatherRuleName,
                                                                                 PublishmentSystemId);
                        gatherFileRuleInfo.GatherUrl = GatherUrl.Text;
                        gatherFileRuleInfo.Charset = ECharsetUtils.GetEnumType(Charset.SelectedValue);

                        gatherFileRuleInfo.IsToFile = TranslateUtils.ToBool(IsToFile.SelectedValue);
                        gatherFileRuleInfo.FilePath = FilePath.Text;
                        gatherFileRuleInfo.IsSaveRelatedFiles =
                            TranslateUtils.ToBool(IsSaveRelatedFiles.SelectedValue);
                        gatherFileRuleInfo.IsRemoveScripts =
                            TranslateUtils.ToBool(IsRemoveScripts.SelectedValue);
                        gatherFileRuleInfo.StyleDirectoryPath = StyleDirectoryPath.Text;
                        gatherFileRuleInfo.ScriptDirectoryPath = ScriptDirectoryPath.Text;
                        gatherFileRuleInfo.ImageDirectoryPath = ImageDirectoryPath.Text;

                        if (NodeIDDropDownList.SelectedValue != null)
                        {
                            gatherFileRuleInfo.NodeId = int.Parse(NodeIDDropDownList.SelectedValue);
                        }
                        gatherFileRuleInfo.IsSaveImage = TranslateUtils.ToBool(IsSaveImage.SelectedValue);
                        gatherFileRuleInfo.IsChecked = TranslateUtils.ToBool(IsChecked.SelectedValue);
                        gatherFileRuleInfo.IsAutoCreate = TranslateUtils.ToBool(IsAutoCreate.SelectedValue);

                        gatherFileRuleInfo.ContentExclude = ContentExclude.Text;
                        var htmlClearArrayList = new ArrayList();
                        foreach (ListItem item in ContentHtmlClearCollection.Items)
                        {
                            if (item.Selected) htmlClearArrayList.Add(item.Value);
                        }
                        gatherFileRuleInfo.ContentHtmlClearCollection = TranslateUtils.ObjectCollectionToString(htmlClearArrayList);
                        var htmlClearTagArrayList = new ArrayList();
                        foreach (ListItem item in ContentHtmlClearTagCollection.Items)
                        {
                            if (item.Selected) htmlClearTagArrayList.Add(item.Value);
                        }
                        gatherFileRuleInfo.ContentHtmlClearTagCollection = TranslateUtils.ObjectCollectionToString(htmlClearTagArrayList);
                        gatherFileRuleInfo.ContentTitleStart = ContentTitleStart.Text;
                        gatherFileRuleInfo.ContentTitleEnd = ContentTitleEnd.Text;
                        gatherFileRuleInfo.ContentContentStart = ContentContentStart.Text;
                        gatherFileRuleInfo.ContentContentEnd = ContentContentEnd.Text;

                        var valueArrayList =
                            ControlUtils.GetSelectedListControlValueArrayList(ContentAttributes);
                        gatherFileRuleInfo.ContentAttributes = TranslateUtils.ObjectCollectionToString(valueArrayList);
                        var attributesXML = new NameValueCollection();

                        for (var i = 0; i < valueArrayList.Count; i++)
                        {
                            var attributeName = valueArrayList[i] as string;

                            foreach (RepeaterItem item in ContentAttributesRepeater.Items)
                            {
                                if (item.ItemIndex == i)
                                {
                                    var contentStart = (TextBox) item.FindControl("ContentStart");
                                    var contentEnd = (TextBox) item.FindControl("ContentEnd");

                                    attributesXML[attributeName + "_ContentStart"] =
                                        StringUtils.ValueToUrl(contentStart.Text);
                                    attributesXML[attributeName + "_ContentEnd"] =
                                        StringUtils.ValueToUrl(contentEnd.Text);
                                }
                            }
                        }
                        gatherFileRuleInfo.ContentAttributesXml =
                            TranslateUtils.NameValueCollectionToString(attributesXML);

                        DataProvider.GatherFileRuleDao.Update(gatherFileRuleInfo);
                    }
				}
				else
				{
				    isNeedAdd = true;
				}

                if (isNeedAdd)
                {
                    var gatherFileRuleInfo = new GatherFileRuleInfo();
                    gatherFileRuleInfo.GatherRuleName = GatherRuleName.Text;
                    gatherFileRuleInfo.PublishmentSystemId = PublishmentSystemId;
                    if (NodeIDDropDownList.SelectedValue != null)
                    {
                        gatherFileRuleInfo.NodeId = int.Parse(NodeIDDropDownList.SelectedValue);
                    }
                    gatherFileRuleInfo.GatherUrl = GatherUrl.Text;
                    gatherFileRuleInfo.Charset = ECharsetUtils.GetEnumType(Charset.SelectedValue);

                    gatherFileRuleInfo.IsToFile = TranslateUtils.ToBool(IsToFile.SelectedValue);
                    gatherFileRuleInfo.FilePath = FilePath.Text;
                    gatherFileRuleInfo.IsSaveRelatedFiles = TranslateUtils.ToBool(IsSaveRelatedFiles.SelectedValue);
                    gatherFileRuleInfo.IsRemoveScripts = TranslateUtils.ToBool(IsRemoveScripts.SelectedValue);
                    gatherFileRuleInfo.StyleDirectoryPath = StyleDirectoryPath.Text;
                    gatherFileRuleInfo.ScriptDirectoryPath = ScriptDirectoryPath.Text;
                    gatherFileRuleInfo.ImageDirectoryPath = ImageDirectoryPath.Text;

                    if (NodeIDDropDownList.SelectedValue != null)
                    {
                        gatherFileRuleInfo.NodeId = int.Parse(NodeIDDropDownList.SelectedValue);
                    }
                    gatherFileRuleInfo.IsSaveImage = TranslateUtils.ToBool(IsSaveImage.SelectedValue);
                    gatherFileRuleInfo.IsChecked = TranslateUtils.ToBool(IsChecked.SelectedValue);
                    gatherFileRuleInfo.IsAutoCreate = TranslateUtils.ToBool(IsAutoCreate.SelectedValue);

                    gatherFileRuleInfo.ContentExclude = ContentExclude.Text;
                    var htmlClearArrayList = new ArrayList();
                    foreach (ListItem item in ContentHtmlClearCollection.Items)
                    {
                        if (item.Selected) htmlClearArrayList.Add(item.Value);
                    }
                    gatherFileRuleInfo.ContentHtmlClearCollection = TranslateUtils.ObjectCollectionToString(htmlClearArrayList);
                    var htmlClearTagArrayList = new ArrayList();
                    foreach (ListItem item in ContentHtmlClearTagCollection.Items)
                    {
                        if (item.Selected) htmlClearTagArrayList.Add(item.Value);
                    }
                    gatherFileRuleInfo.ContentHtmlClearTagCollection = TranslateUtils.ObjectCollectionToString(htmlClearTagArrayList);
                    gatherFileRuleInfo.LastGatherDate = DateUtils.SqlMinValue;
                    gatherFileRuleInfo.ContentTitleStart = ContentTitleStart.Text;
                    gatherFileRuleInfo.ContentTitleEnd = ContentTitleEnd.Text;
                    gatherFileRuleInfo.ContentContentStart = ContentContentStart.Text;
                    gatherFileRuleInfo.ContentContentEnd = ContentContentEnd.Text;

                    var valueArrayList = ControlUtils.GetSelectedListControlValueArrayList(ContentAttributes);
                    gatherFileRuleInfo.ContentAttributes = TranslateUtils.ObjectCollectionToString(valueArrayList);
                    var attributesXML = new NameValueCollection();

                    for (var i = 0; i < valueArrayList.Count; i++)
                    {
                        var attributeName = valueArrayList[i] as string;

                        foreach (RepeaterItem item in ContentAttributesRepeater.Items)
                        {
                            if (item.ItemIndex == i)
                            {
                                var contentStart = (TextBox)item.FindControl("ContentStart");
                                var contentEnd = (TextBox)item.FindControl("ContentEnd");

                                attributesXML[attributeName + "_ContentStart"] = StringUtils.ValueToUrl(contentStart.Text);
                                attributesXML[attributeName + "_ContentEnd"] = StringUtils.ValueToUrl(contentEnd.Text);
                            }
                        }
                    }
                    gatherFileRuleInfo.ContentAttributesXml = TranslateUtils.NameValueCollectionToString(attributesXML);

                    DataProvider.GatherFileRuleDao.Insert(gatherFileRuleInfo);
                }

                if (isNeedAdd)
                {
                    Body.AddSiteLog(PublishmentSystemId, "添加单文件页采集规则", $"采集规则:{GatherRuleName.Text}");
                }
                else
                {
                    Body.AddSiteLog(PublishmentSystemId, "编辑单文件页采集规则", $"采集规则:{GatherRuleName.Text}");
                }

				errorMessage = string.Empty;
				return true;
			}
			catch
			{
				errorMessage = "操作失败！";
				return false;
			}
		}


		public void NextPanel(object sender, EventArgs e)
		{
			string errorMessage;
			switch (CurrentWizardPanel)
			{
				case WizardPanel.GatherRuleBase:

					if (Validate_GatherRuleBase(out errorMessage))
					{
                        if (TranslateUtils.ToBool(IsToFile.SelectedValue))
                        {
                            if (Validate_InsertGatherRule(out errorMessage))
                            {
                                SetActivePanel(WizardPanel.Done, Done);
                            }
                            else
                            {
                                ErrorLabel.Text = errorMessage;
                                SetActivePanel(WizardPanel.OperatingError, OperatingError);
                            }
                        }
                        else
                        {
                            SetActivePanel(WizardPanel.GatherRuleContent, GatherRuleContent);
                        }
					}
					else
					{
                        FailMessage(errorMessage);
						SetActivePanel(WizardPanel.GatherRuleBase, GatherRuleBase);
					}

					break;

				case WizardPanel.GatherRuleContent:

					if (Validate_GatherContent(out errorMessage))
					{
						if (Validate_InsertGatherRule(out errorMessage))
						{
							SetActivePanel(WizardPanel.Done, Done);
						}
						else
						{
							ErrorLabel.Text = errorMessage;
							SetActivePanel(WizardPanel.OperatingError, OperatingError);
						}
					}
					else
					{
                        FailMessage(errorMessage);
						SetActivePanel(WizardPanel.GatherRuleContent, GatherRuleContent);
					}

					break;

				case WizardPanel.Done:
					break;
			}
		}

		public void PreviousPanel(object sender, EventArgs e)
		{
			switch (CurrentWizardPanel)
			{
				case WizardPanel.GatherRuleBase:
					break;

				case WizardPanel.GatherRuleContent:
                    SetActivePanel(WizardPanel.GatherRuleBase, GatherRuleBase);
					break;
			}
		}

        public void DropDownList_SelectedIndexChanged(object sender, EventArgs e)
		{
            if (TranslateUtils.ToBool(IsToFile.SelectedValue))
            {
                PlaceHolder_File.Visible = true;
                PlaceHolder_Content.Visible = false;
                PlaceHolder_File_Directory.Visible = TranslateUtils.ToBool(IsSaveRelatedFiles.SelectedValue);
            }
            else
            {
                PlaceHolder_File.Visible = false;
                PlaceHolder_Content.Visible = true;
            }
		}

        public void ContentAttributes_SelectedIndexChanged(object sender, EventArgs e)
        {
            var valueArrayList = ControlUtils.GetSelectedListControlValueArrayList(ContentAttributes);
            if (Page.IsPostBack)
            {
                _contentAttributesXml = new NameValueCollection();

                for (var i = 0; i < valueArrayList.Count; i++)
                {
                    var attributeName = valueArrayList[i] as string;

                    foreach (RepeaterItem item in ContentAttributesRepeater.Items)
                    {
                        if (item.ItemIndex == i)
                        {
                            var contentStart = (TextBox) item.FindControl("ContentStart");
                            var contentEnd = (TextBox) item.FindControl("ContentEnd");

                            _contentAttributesXml[attributeName + "_ContentStart"] = StringUtils.ValueToUrl(contentStart.Text);
                            _contentAttributesXml[attributeName + "_ContentEnd"] = StringUtils.ValueToUrl(contentEnd.Text);
                        }
                    }
                }
            }

            ContentAttributesRepeater.DataSource = valueArrayList;
            ContentAttributesRepeater.ItemDataBound += ContentAttributesRepeater_ItemDataBound;
            ContentAttributesRepeater.DataBind();
        }

        void ContentAttributesRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var attributeName = e.Item.DataItem as string;

                var displayName = ContentAttributes.Items.FindByValue(attributeName).Text;

                var helpStart = (NoTagText)e.Item.FindControl("HelpStart") ;
                var helpEnd = (NoTagText)e.Item.FindControl("HelpEnd");
                var contentStart = (TextBox) e.Item.FindControl("ContentStart");
                var contentEnd = (TextBox) e.Item.FindControl("ContentEnd");

                helpStart.Text = displayName + "的开始字符串";
                helpEnd.Text = displayName + "的结束字符串";

                contentStart.Text = StringUtils.ValueFromUrl(_contentAttributesXml[attributeName + "_ContentStart"]);
                contentEnd.Text = StringUtils.ValueFromUrl(_contentAttributesXml[attributeName + "_ContentEnd"]);
            }
        }
	}
}
