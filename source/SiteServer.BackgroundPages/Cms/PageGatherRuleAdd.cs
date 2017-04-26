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
    public class PageGatherRuleAdd : BasePageCms
    {
        public Literal ltlPageTitle;
        public PlaceHolder GatherRuleBase;
        public TextBox GatherRuleName;
        public DropDownList NodeIDDropDownList;
        public DropDownList Charset;
        public TextBox GatherNum;
        public RadioButtonList IsSaveImage;
        public RadioButtonList IsSetFirstImageAsImageUrl;
        public RadioButtonList IsEmptyContentAllowed;
        public RadioButtonList IsSameTitleAllowed;
        public RadioButtonList IsChecked;
        public RadioButtonList IsAutoCreate;
        public RadioButtonList IsOrderByDesc;

        public PlaceHolder GatherRuleList;
        public CheckBox GatherUrlIsCollection;
        public Control GatherUrlCollectionRow;
        public TextBox GatherUrlCollection;
        public CheckBox GatherUrlIsSerialize;
        public Control GatherUrlSerializeRow;
        public TextBox GatherUrlSerialize;
        public TextBox SerializeFrom;
        public TextBox SerializeTo;
        public TextBox SerializeInterval;
        public CheckBox SerializeIsOrderByDesc;
        public CheckBox SerializeIsAddZero;
        public TextBox UrlInclude;

        public PlaceHolder GatherRuleContent;
        public TextBox ContentTitleStart;
        public TextBox ContentTitleEnd;
        public TextBox ContentContentStart;
        public TextBox ContentContentEnd;
        public TextBox ContentContentStart2;
        public TextBox ContentContentEnd2;
        public TextBox ContentContentStart3;
        public TextBox ContentContentEnd3;
        public TextBox ContentNextPageStart;
        public TextBox ContentNextPageEnd;

        public PlaceHolder GatherRuleOthers;
        public TextBox TitleInclude;
        public TextBox ListAreaStart;
        public TextBox ListAreaEnd;
        public TextBox CookieString;
        public TextBox ContentExclude;
        public CheckBoxList ContentHtmlClearCollection;
        public CheckBoxList ContentHtmlClearTagCollection;
        public TextBox ContentReplaceFrom;
        public TextBox ContentReplaceTo;
        public TextBox ContentChannelStart;
        public TextBox ContentChannelEnd;
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
            return PageUtils.GetCmsUrl(nameof(PageGatherRuleAdd), new NameValueCollection
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
                var pageTitle = _isEdit ? "编辑Web页面信息采集规则" : "添加Web页面信息采集规则";
                BreadCrumb(AppManager.Cms.LeftMenu.IdFunction, AppManager.Cms.LeftMenu.Function.IdGather, pageTitle, AppManager.Cms.Permission.WebSite.Gather);

                ltlPageTitle.Text = pageTitle;

                ECharsetUtils.AddListItems(Charset);
                ControlUtils.SelectListItemsIgnoreCase(Charset, ECharsetUtils.GetValue(ECharset.utf_8));
                NodeManager.AddListItemsForAddContent(NodeIDDropDownList.Items, PublishmentSystemInfo, true, Body.AdministratorName);

                SetActivePanel(WizardPanel.GatherRuleBase, GatherRuleBase);

                if (_isEdit)
                {
                    var gatherRuleInfo = DataProvider.GatherRuleDao.GetGatherRuleInfo(_theGatherRuleName, PublishmentSystemId);
                    GatherRuleName.Text = gatherRuleInfo.GatherRuleName;

                    ControlUtils.SelectListItemsIgnoreCase(Charset, ECharsetUtils.GetValue(gatherRuleInfo.Charset));
                    GatherNum.Text = gatherRuleInfo.Additional.GatherNum.ToString();
                    foreach (ListItem item in IsSaveImage.Items)
                    {
                        if (item.Value.Equals(gatherRuleInfo.Additional.IsSaveImage.ToString()))
                        {
                            item.Selected = true;
                        }
                        else
                        {
                            item.Selected = false;
                        }
                    }
                    foreach (ListItem item in IsSetFirstImageAsImageUrl.Items)
                    {
                        if (item.Value.Equals(gatherRuleInfo.Additional.IsSetFirstImageAsImageUrl.ToString()))
                        {
                            item.Selected = true;
                        }
                        else
                        {
                            item.Selected = false;
                        }
                    }
                    foreach (ListItem item in IsEmptyContentAllowed.Items)
                    {
                        if (item.Value.Equals(gatherRuleInfo.Additional.IsEmptyContentAllowed.ToString()))
                        {
                            item.Selected = true;
                        }
                        else
                        {
                            item.Selected = false;
                        }
                    }
                    foreach (ListItem item in IsSameTitleAllowed.Items)
                    {
                        if (item.Value.Equals(gatherRuleInfo.Additional.IsSameTitleAllowed.ToString()))
                        {
                            item.Selected = true;
                        }
                        else
                        {
                            item.Selected = false;
                        }
                    }
                    foreach (ListItem item in IsChecked.Items)
                    {
                        if (item.Value.Equals(gatherRuleInfo.Additional.IsChecked.ToString()))
                        {
                            item.Selected = true;
                        }
                        else
                        {
                            item.Selected = false;
                        }
                    }
                    foreach (ListItem item in IsAutoCreate.Items)
                    {
                        if (item.Value.Equals(gatherRuleInfo.Additional.IsAutoCreate.ToString()))
                        {
                            item.Selected = true;
                        }
                        else
                        {
                            item.Selected = false;
                        }
                    }
                    foreach (ListItem item in IsOrderByDesc.Items)
                    {
                        if (item.Value.Equals(gatherRuleInfo.Additional.IsOrderByDesc.ToString()))
                        {
                            item.Selected = true;
                        }
                        else
                        {
                            item.Selected = false;
                        }
                    }

                    GatherUrlIsCollection.Checked = gatherRuleInfo.GatherUrlIsCollection;
                    GatherUrlCollection.Text = gatherRuleInfo.GatherUrlCollection;
                    GatherUrlIsSerialize.Checked = gatherRuleInfo.GatherUrlIsSerialize;
                    GatherUrlSerialize.Text = gatherRuleInfo.GatherUrlSerialize;
                    SerializeFrom.Text = gatherRuleInfo.SerializeFrom.ToString();
                    SerializeTo.Text = gatherRuleInfo.SerializeTo.ToString();
                    SerializeInterval.Text = gatherRuleInfo.SerializeInterval.ToString();
                    SerializeIsOrderByDesc.Checked = gatherRuleInfo.SerializeIsOrderByDesc;
                    SerializeIsAddZero.Checked = gatherRuleInfo.SerializeIsAddZero;

                    foreach (ListItem item in NodeIDDropDownList.Items)
                    {
                        if (item.Value.Equals(gatherRuleInfo.NodeId.ToString()))
                        {
                            item.Selected = true;
                        }
                        else
                        {
                            item.Selected = false;
                        }
                    }
                    UrlInclude.Text = gatherRuleInfo.UrlInclude;
                    TitleInclude.Text = gatherRuleInfo.TitleInclude;
                    ContentExclude.Text = gatherRuleInfo.ContentExclude;
                    var htmlClearArrayList = TranslateUtils.StringCollectionToStringList(gatherRuleInfo.ContentHtmlClearCollection);
                    foreach (ListItem item in ContentHtmlClearCollection.Items)
                    {
                        item.Selected = htmlClearArrayList.Contains(item.Value);
                    }
                    var htmlClearTagArrayList = TranslateUtils.StringCollectionToStringList(gatherRuleInfo.ContentHtmlClearTagCollection);
                    foreach (ListItem item in ContentHtmlClearTagCollection.Items)
                    {
                        item.Selected = htmlClearTagArrayList.Contains(item.Value);
                    }
                    ListAreaStart.Text = gatherRuleInfo.ListAreaStart;
                    ListAreaEnd.Text = gatherRuleInfo.ListAreaEnd;
                    CookieString.Text = gatherRuleInfo.CookieString;
                    ContentTitleStart.Text = gatherRuleInfo.ContentTitleStart;
                    ContentTitleEnd.Text = gatherRuleInfo.ContentTitleEnd;
                    ContentContentStart.Text = gatherRuleInfo.ContentContentStart;
                    ContentContentEnd.Text = gatherRuleInfo.ContentContentEnd;
                    ContentContentStart2.Text = gatherRuleInfo.Additional.ContentContentStart2;
                    ContentContentEnd2.Text = gatherRuleInfo.Additional.ContentContentEnd2;
                    ContentContentStart3.Text = gatherRuleInfo.Additional.ContentContentStart3;
                    ContentContentEnd3.Text = gatherRuleInfo.Additional.ContentContentEnd3;
                    ContentReplaceFrom.Text = gatherRuleInfo.Additional.ContentReplaceFrom;
                    ContentReplaceTo.Text = gatherRuleInfo.Additional.ContentReplaceTo;
                    ContentChannelStart.Text = gatherRuleInfo.ContentChannelStart;
                    ContentChannelEnd.Text = gatherRuleInfo.ContentChannelEnd;
                    ContentNextPageStart.Text = gatherRuleInfo.ContentNextPageStart;
                    ContentNextPageEnd.Text = gatherRuleInfo.ContentNextPageEnd;

                    var contentAttributeArrayList = TranslateUtils.StringCollectionToStringList(gatherRuleInfo.ContentAttributes);
                    var relatedIdentities = RelatedIdentities.GetChannelRelatedIdentities(PublishmentSystemId, PublishmentSystemId);
                    var styleInfoList = TableStyleManager.GetTableStyleInfoList(ETableStyle.BackgroundContent, PublishmentSystemInfo.AuxiliaryTableForContent, relatedIdentities);
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

                    var listItem = new ListItem("点击量", ContentAttribute.Hits.ToLower());
                    if (contentAttributeArrayList.Contains(listItem.Value))
                    {
                        listItem.Selected = true;
                    }
                    ContentAttributes.Items.Add(listItem);

                    _contentAttributesXml = TranslateUtils.ToNameValueCollection(gatherRuleInfo.ContentAttributesXml);

                    ContentAttributes_SelectedIndexChanged(null, EventArgs.Empty);

                }
                else
                {
                    var relatedIdentities = RelatedIdentities.GetChannelRelatedIdentities(PublishmentSystemId, PublishmentSystemId);
                    var styleInfoList = TableStyleManager.GetTableStyleInfoList(ETableStyle.BackgroundContent, PublishmentSystemInfo.AuxiliaryTableForContent, relatedIdentities);
                    foreach (var styleInfo in styleInfoList)
                    {
                        if (StringUtils.EqualsIgnoreCase(styleInfo.AttributeName, ContentAttribute.Title) || StringUtils.EqualsIgnoreCase(styleInfo.AttributeName, BackgroundContentAttribute.Content)) continue;

                        var listitem = new ListItem(styleInfo.DisplayName, styleInfo.AttributeName.ToLower());
                        ContentAttributes.Items.Add(listitem);
                    }

                    var listItem = new ListItem("点击量", ContentAttribute.Hits.ToLower());
                    ContentAttributes.Items.Add(listItem);
                }

                GatherUrl_CheckedChanged(null, null);
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
            GatherRuleList,
            GatherRuleContent,
            GatherRuleOthers,
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
                    AddWaitAndRedirectScript(PageGatherRule.GetRedirectUrl(PublishmentSystemId));
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

            if (_isEdit == false)
            {
                var gatherRuleNameList = DataProvider.GatherRuleDao.GetGatherRuleNameArrayList(PublishmentSystemId);
                if (gatherRuleNameList.IndexOf(GatherRuleName.Text) != -1)
                {
                    errorMessage = "采集规则名称已存在！";
                    return false;
                }
            }

            errorMessage = string.Empty;
            return true;
        }

        public bool Validate_GatherList(out string errorMessage)
        {
            if (!GatherUrlIsCollection.Checked && !GatherUrlIsSerialize.Checked)
            {
                errorMessage = "必须填写起始网页地址！";
                return false;
            }

            if (GatherUrlIsCollection.Checked)
            {
                if (string.IsNullOrEmpty(GatherUrlCollection.Text))
                {
                    errorMessage = "必须填写起始网页地址！";
                    return false;
                }
            }

            if (GatherUrlIsSerialize.Checked)
            {
                if (string.IsNullOrEmpty(GatherUrlSerialize.Text))
                {
                    errorMessage = "必须填写起始网页地址！";
                    return false;
                }

                if (GatherUrlSerialize.Text.IndexOf("*") == -1)
                {
                    errorMessage = "起始网页地址必须带有 * 字符！";
                    return false;
                }

                if (string.IsNullOrEmpty(SerializeFrom.Text) || string.IsNullOrEmpty(SerializeTo.Text))
                {
                    errorMessage = "必须填写变动数字范围！";
                    return false;
                }
                else
                {
                    if (TranslateUtils.ToInt(SerializeFrom.Text) < 0 || TranslateUtils.ToInt(SerializeTo.Text) < 0)
                    {
                        errorMessage = "变动数字范围必须大于等于0！";
                        return false;
                    }
                    if (TranslateUtils.ToInt(SerializeTo.Text) <= TranslateUtils.ToInt(SerializeFrom.Text))
                    {
                        errorMessage = "变动数字范围结束必须大于开始！";
                        return false;
                    }
                }

                if (string.IsNullOrEmpty(SerializeInterval.Text))
                {
                    errorMessage = "必须填写数字变动倍数！";
                    return false;
                }
                else
                {
                    if (TranslateUtils.ToInt(SerializeInterval.Text) <= 0)
                    {
                        errorMessage = "数字变动倍数必须大于等于1！";
                        return false;
                    }
                }
            }

            if (string.IsNullOrEmpty(UrlInclude.Text))
            {
                errorMessage = "必须填写内容地址包含字符串！";
                return false;
            }

            errorMessage = string.Empty;
            return true;
        }

        public bool Validate_GatherContent(out string errorMessage)
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
                        DataProvider.GatherRuleDao.Delete(_theGatherRuleName, PublishmentSystemId);
                    }
                    else
                    {
                        var gatherRuleInfo = DataProvider.GatherRuleDao.GetGatherRuleInfo(_theGatherRuleName, PublishmentSystemId);
                        if (NodeIDDropDownList.SelectedValue != null)
                        {
                            gatherRuleInfo.NodeId = int.Parse(NodeIDDropDownList.SelectedValue);
                        }
                        gatherRuleInfo.Charset = ECharsetUtils.GetEnumType(Charset.SelectedValue);
                        gatherRuleInfo.Additional.GatherNum = int.Parse(GatherNum.Text);
                        gatherRuleInfo.Additional.IsSaveImage = TranslateUtils.ToBool(IsSaveImage.SelectedValue);
                        gatherRuleInfo.Additional.IsSetFirstImageAsImageUrl = TranslateUtils.ToBool(IsSetFirstImageAsImageUrl.SelectedValue);
                        gatherRuleInfo.Additional.IsEmptyContentAllowed = TranslateUtils.ToBool(IsEmptyContentAllowed.SelectedValue);
                        gatherRuleInfo.Additional.IsSameTitleAllowed = TranslateUtils.ToBool(IsSameTitleAllowed.SelectedValue);
                        gatherRuleInfo.Additional.IsChecked = TranslateUtils.ToBool(IsChecked.SelectedValue);
                        gatherRuleInfo.Additional.IsAutoCreate = TranslateUtils.ToBool(IsAutoCreate.SelectedValue);
                        gatherRuleInfo.Additional.IsOrderByDesc = TranslateUtils.ToBool(IsOrderByDesc.SelectedValue);

                        gatherRuleInfo.GatherUrlIsCollection = GatherUrlIsCollection.Checked;
                        gatherRuleInfo.GatherUrlCollection = GatherUrlCollection.Text;
                        gatherRuleInfo.GatherUrlIsSerialize = GatherUrlIsSerialize.Checked;
                        gatherRuleInfo.GatherUrlSerialize = GatherUrlSerialize.Text;
                        gatherRuleInfo.SerializeFrom = TranslateUtils.ToInt(SerializeFrom.Text);
                        gatherRuleInfo.SerializeTo = TranslateUtils.ToInt(SerializeTo.Text);
                        gatherRuleInfo.SerializeInterval = TranslateUtils.ToInt(SerializeInterval.Text);
                        gatherRuleInfo.SerializeIsOrderByDesc = SerializeIsOrderByDesc.Checked;
                        gatherRuleInfo.SerializeIsAddZero = SerializeIsAddZero.Checked;

                        gatherRuleInfo.UrlInclude = UrlInclude.Text;
                        gatherRuleInfo.TitleInclude = TitleInclude.Text;
                        gatherRuleInfo.ContentExclude = ContentExclude.Text;

                        var htmlClearArrayList = new ArrayList();
                        foreach (ListItem item in ContentHtmlClearCollection.Items)
                        {
                            if (item.Selected) htmlClearArrayList.Add(item.Value);
                        }
                        gatherRuleInfo.ContentHtmlClearCollection = TranslateUtils.ObjectCollectionToString(htmlClearArrayList);

                        var htmlClearTagArrayList = new ArrayList();
                        foreach (ListItem item in ContentHtmlClearTagCollection.Items)
                        {
                            if (item.Selected) htmlClearTagArrayList.Add(item.Value);
                        }
                        gatherRuleInfo.ContentHtmlClearTagCollection = TranslateUtils.ObjectCollectionToString(htmlClearTagArrayList);

                        gatherRuleInfo.ListAreaStart = ListAreaStart.Text;
                        gatherRuleInfo.ListAreaEnd = ListAreaEnd.Text;
                        gatherRuleInfo.CookieString = CookieString.Text;
                        gatherRuleInfo.ContentTitleStart = ContentTitleStart.Text;
                        gatherRuleInfo.ContentTitleEnd = ContentTitleEnd.Text;
                        gatherRuleInfo.ContentContentStart = ContentContentStart.Text;
                        gatherRuleInfo.ContentContentEnd = ContentContentEnd.Text;
                        gatherRuleInfo.Additional.ContentContentStart2 = ContentContentStart2.Text;
                        gatherRuleInfo.Additional.ContentContentEnd2 = ContentContentEnd2.Text;
                        gatherRuleInfo.Additional.ContentContentStart3 = ContentContentStart3.Text;
                        gatherRuleInfo.Additional.ContentContentEnd3 = ContentContentEnd3.Text;
                        gatherRuleInfo.Additional.ContentReplaceFrom = ContentReplaceFrom.Text;
                        gatherRuleInfo.Additional.ContentReplaceTo = ContentReplaceTo.Text;
                        gatherRuleInfo.ContentChannelStart = ContentChannelStart.Text;
                        gatherRuleInfo.ContentChannelEnd = ContentChannelEnd.Text;
                        gatherRuleInfo.ContentNextPageStart = ContentNextPageStart.Text;
                        gatherRuleInfo.ContentNextPageEnd = ContentNextPageEnd.Text;

                        var valueArrayList = ControlUtils.GetSelectedListControlValueArrayList(ContentAttributes);
                        gatherRuleInfo.ContentAttributes = TranslateUtils.ObjectCollectionToString(valueArrayList);
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

                                    //采集为空时的默认值
                                    var contentDefault = (TextBox)item.FindControl("ContentDefault");

                                    attributesXML[attributeName + "_ContentStart"] = StringUtils.ValueToUrl(contentStart.Text);
                                    attributesXML[attributeName + "_ContentEnd"] = StringUtils.ValueToUrl(contentEnd.Text);


                                    //采集为空时的默认值
                                    attributesXML[attributeName + "_ContentDefault"] = StringUtils.ValueToUrl(contentDefault.Text);
                                }
                            }
                        }
                        gatherRuleInfo.ContentAttributesXml = TranslateUtils.NameValueCollectionToString(attributesXML);

                        DataProvider.GatherRuleDao.Update(gatherRuleInfo);
                    }
                }
                else
                {
                    isNeedAdd = true;
                }

                if (isNeedAdd)
                {
                    var gatherRuleInfo = new GatherRuleInfo();
                    gatherRuleInfo.GatherRuleName = GatherRuleName.Text;
                    gatherRuleInfo.PublishmentSystemId = PublishmentSystemId;
                    if (NodeIDDropDownList.SelectedValue != null)
                    {
                        gatherRuleInfo.NodeId = int.Parse(NodeIDDropDownList.SelectedValue);
                    }
                    gatherRuleInfo.Charset = ECharsetUtils.GetEnumType(Charset.SelectedValue);
                    gatherRuleInfo.Additional.GatherNum = int.Parse(GatherNum.Text);
                    gatherRuleInfo.Additional.IsSaveImage = TranslateUtils.ToBool(IsSaveImage.SelectedValue);
                    gatherRuleInfo.Additional.IsSetFirstImageAsImageUrl = TranslateUtils.ToBool(IsSetFirstImageAsImageUrl.SelectedValue);
                    gatherRuleInfo.Additional.IsEmptyContentAllowed = TranslateUtils.ToBool(IsEmptyContentAllowed.SelectedValue);
                    gatherRuleInfo.Additional.IsSameTitleAllowed = TranslateUtils.ToBool(IsSameTitleAllowed.SelectedValue);
                    gatherRuleInfo.Additional.IsChecked = TranslateUtils.ToBool(IsChecked.SelectedValue);
                    gatherRuleInfo.Additional.IsAutoCreate = TranslateUtils.ToBool(IsAutoCreate.SelectedValue);
                    gatherRuleInfo.Additional.IsOrderByDesc = TranslateUtils.ToBool(IsOrderByDesc.SelectedValue);

                    gatherRuleInfo.GatherUrlIsCollection = GatherUrlIsCollection.Checked;
                    gatherRuleInfo.GatherUrlCollection = GatherUrlCollection.Text;
                    gatherRuleInfo.GatherUrlIsSerialize = GatherUrlIsSerialize.Checked;
                    gatherRuleInfo.GatherUrlSerialize = GatherUrlSerialize.Text;
                    gatherRuleInfo.SerializeFrom = TranslateUtils.ToInt(SerializeFrom.Text);
                    gatherRuleInfo.SerializeTo = TranslateUtils.ToInt(SerializeTo.Text);
                    gatherRuleInfo.SerializeInterval = TranslateUtils.ToInt(SerializeInterval.Text);
                    gatherRuleInfo.SerializeIsOrderByDesc = SerializeIsOrderByDesc.Checked;
                    gatherRuleInfo.SerializeIsAddZero = SerializeIsAddZero.Checked;

                    gatherRuleInfo.UrlInclude = UrlInclude.Text;
                    gatherRuleInfo.TitleInclude = TitleInclude.Text;
                    gatherRuleInfo.ContentExclude = ContentExclude.Text;

                    var htmlClearArrayList = new ArrayList();
                    foreach (ListItem item in ContentHtmlClearCollection.Items)
                    {
                        if (item.Selected) htmlClearArrayList.Add(item.Value);
                    }
                    gatherRuleInfo.ContentHtmlClearCollection = TranslateUtils.ObjectCollectionToString(htmlClearArrayList);

                    var htmlClearTagArrayList = new ArrayList();
                    foreach (ListItem item in ContentHtmlClearTagCollection.Items)
                    {
                        if (item.Selected) htmlClearTagArrayList.Add(item.Value);
                    }
                    gatherRuleInfo.ContentHtmlClearTagCollection = TranslateUtils.ObjectCollectionToString(htmlClearTagArrayList);

                    gatherRuleInfo.ListAreaStart = ListAreaStart.Text;
                    gatherRuleInfo.ListAreaEnd = ListAreaEnd.Text;
                    gatherRuleInfo.CookieString = CookieString.Text;
                    gatherRuleInfo.ContentTitleStart = ContentTitleStart.Text;
                    gatherRuleInfo.ContentTitleEnd = ContentTitleEnd.Text;
                    gatherRuleInfo.ContentContentStart = ContentContentStart.Text;
                    gatherRuleInfo.ContentContentEnd = ContentContentEnd.Text;
                    gatherRuleInfo.Additional.ContentContentStart2 = ContentContentStart2.Text;
                    gatherRuleInfo.Additional.ContentContentEnd2 = ContentContentEnd2.Text;
                    gatherRuleInfo.Additional.ContentContentStart3 = ContentContentStart3.Text;
                    gatherRuleInfo.Additional.ContentContentEnd3 = ContentContentEnd3.Text;
                    gatherRuleInfo.Additional.ContentReplaceFrom = ContentReplaceFrom.Text;
                    gatherRuleInfo.Additional.ContentReplaceTo = ContentReplaceTo.Text;
                    gatherRuleInfo.ContentChannelStart = ContentChannelStart.Text;
                    gatherRuleInfo.ContentChannelEnd = ContentChannelEnd.Text;
                    gatherRuleInfo.ContentNextPageStart = ContentNextPageStart.Text;
                    gatherRuleInfo.ContentNextPageEnd = ContentNextPageEnd.Text;
                    gatherRuleInfo.LastGatherDate = DateUtils.SqlMinValue;

                    var valueArrayList = ControlUtils.GetSelectedListControlValueArrayList(ContentAttributes);
                    gatherRuleInfo.ContentAttributes = TranslateUtils.ObjectCollectionToString(valueArrayList);
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

                                //采集为空时的默认值
                                var contentDefault = (TextBox)item.FindControl("ContentDefault");

                                attributesXML[attributeName + "_ContentStart"] = StringUtils.ValueToUrl(contentStart.Text);
                                attributesXML[attributeName + "_ContentEnd"] = StringUtils.ValueToUrl(contentEnd.Text);

                                //采集为空时的默认值
                                attributesXML[attributeName + "_ContentDefault"] = StringUtils.ValueToUrl(contentDefault.Text);
                            }
                        }
                    }
                    gatherRuleInfo.ContentAttributesXml = TranslateUtils.NameValueCollectionToString(attributesXML);

                    DataProvider.GatherRuleDao.Insert(gatherRuleInfo);
                }

                if (isNeedAdd)
                {
                    Body.AddSiteLog(PublishmentSystemId, "添加Web页面采集规则", $"采集规则:{GatherRuleName.Text}");
                }
                else
                {
                    Body.AddSiteLog(PublishmentSystemId, "编辑Web页面采集规则", $"采集规则:{GatherRuleName.Text}");
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
                        SetActivePanel(WizardPanel.GatherRuleList, GatherRuleList);
                    }
                    else
                    {
                        FailMessage(errorMessage);
                        SetActivePanel(WizardPanel.GatherRuleBase, GatherRuleBase);
                    }

                    break;

                case WizardPanel.GatherRuleList:

                    if (Validate_GatherList(out errorMessage))
                    {
                        SetActivePanel(WizardPanel.GatherRuleContent, GatherRuleContent);
                    }
                    else
                    {
                        FailMessage(errorMessage);
                        SetActivePanel(WizardPanel.GatherRuleList, GatherRuleList);
                    }

                    break;

                case WizardPanel.GatherRuleContent:

                    if (Validate_GatherContent(out errorMessage))
                    {
                        SetActivePanel(WizardPanel.GatherRuleOthers, GatherRuleOthers);
                    }
                    else
                    {
                        FailMessage(errorMessage);
                        SetActivePanel(WizardPanel.GatherRuleContent, GatherRuleContent);
                    }

                    break;

                case WizardPanel.GatherRuleOthers:

                    if (Validate_InsertGatherRule(out errorMessage))
                    {
                        SetActivePanel(WizardPanel.Done, Done);
                    }
                    else
                    {
                        ErrorLabel.Text = errorMessage;
                        SetActivePanel(WizardPanel.OperatingError, OperatingError);
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

                case WizardPanel.GatherRuleList:
                    SetActivePanel(WizardPanel.GatherRuleBase, GatherRuleBase);
                    break;

                case WizardPanel.GatherRuleContent:
                    SetActivePanel(WizardPanel.GatherRuleList, GatherRuleList);
                    break;

                case WizardPanel.GatherRuleOthers:
                    SetActivePanel(WizardPanel.GatherRuleContent, GatherRuleContent);
                    break;
            }
        }

        public void GatherUrl_CheckedChanged(object sender, EventArgs e)
        {
            GatherUrlCollectionRow.Visible = false;
            GatherUrlSerializeRow.Visible = false;

            if (GatherUrlIsCollection.Checked)
            {
                GatherUrlCollectionRow.Visible = true;
            }

            if (GatherUrlIsSerialize.Checked)
            {
                GatherUrlSerializeRow.Visible = true;
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
                        var ltlAttributeName = (Literal)item.FindControl("ltlAttributeName");
                        if (ltlAttributeName.Text == attributeName)
                        {
                            var contentStart = (TextBox)item.FindControl("ContentStart");
                            var contentEnd = (TextBox)item.FindControl("ContentEnd");

                            //采集为空时的默认值
                            var contentDefault = (TextBox)item.FindControl("ContentDefault");

                            _contentAttributesXml[attributeName + "_ContentStart"] = StringUtils.ValueToUrl(contentStart.Text);
                            _contentAttributesXml[attributeName + "_ContentEnd"] = StringUtils.ValueToUrl(contentEnd.Text);

                            //采集为空时的默认值
                            _contentAttributesXml[attributeName + "_ContentDefault"] = StringUtils.ValueToUrl(contentDefault.Text);
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

                var ltlAttributeName = (Literal)e.Item.FindControl("ltlAttributeName");
                var helpStart = (NoTagText)e.Item.FindControl("HelpStart");
                var helpEnd = (NoTagText)e.Item.FindControl("HelpEnd");
                var contentStart = (TextBox)e.Item.FindControl("ContentStart");
                var contentEnd = (TextBox)e.Item.FindControl("ContentEnd");

                //采集为空时的默认值
                var helpDefault = (NoTagText)e.Item.FindControl("HelpDefault");
                var contentDefault = (TextBox)e.Item.FindControl("ContentDefault");

                ltlAttributeName.Text = attributeName;

                helpStart.Text = displayName + "的开始字符串";
                helpEnd.Text = displayName + "的结束字符串";

                //采集为空时的默认值
                helpDefault.Text = displayName + "为空时的默认值";
                contentDefault.Text = StringUtils.ValueFromUrl(_contentAttributesXml[attributeName + "_ContentDefault"]);

                contentStart.Text = StringUtils.ValueFromUrl(_contentAttributesXml[attributeName + "_ContentStart"]);
                contentEnd.Text = StringUtils.ValueFromUrl(_contentAttributesXml[attributeName + "_ContentEnd"]);
            }
        }
    }
}
