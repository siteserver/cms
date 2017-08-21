using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web.UI;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.AuxiliaryTable;
using BaiRong.Core.Data;
using BaiRong.Core.Model;
using BaiRong.Core.Model.Enumerations;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;
using SiteServer.CMS.Model.Enumerations;
using SiteServer.Plugin;
using SiteServer.Plugin.Models;

namespace SiteServer.BackgroundPages.Cms
{
    public class ModalTableStyleAdd : BasePageCms
    {
        public TextBox TbAttributeName;
        public TextBox TbDisplayName;
        public TextBox TbHelpText;
        public RadioButtonList RblIsVisible;
        public RadioButtonList RblIsSingleLine;
        public PlaceHolder PhIsFormatString;
        public DropDownList DdlInputType;
        public RadioButtonList RblIsFormatString;
        public TextBox TbDefaultValue;
        public Control SpanDateTip;
        public DropDownList DdlIsHorizontal;
        public TextBox TbColumns;
        public DropDownList DdlRelatedFieldId;
        public DropDownList DdlRelatedFieldStyle;
        public TextBox TbHeight;
        public TextBox TbWidth;

        public DropDownList DdlItemType;
        public PlaceHolder PhItemCount;
        public TextBox TbItemCount;
        public TextBox TbItemValues;
        public Repeater RptItems;

        public Control TrRepeat;
        public Control TrRelatedField;
        public Control TrHeightAndWidth;
        public Control TrItemsType;
        public Control TrItemsRapid;
        public Control TrItems;

        private int _tableStyleId;
        private List<int> _relatedIdentities;
        private string _tableName;
        private string _attributeName;
        private string _redirectUrl;
        private TableStyleInfo _styleInfo;
        private ETableStyle _tableStyle;

        public static string GetOpenWindowString(int publishmentSystemId, int tableStyleId, List<int> relatedIdentities, string tableName, string attributeName, ETableStyle tableStyle, string redirectUrl)
        {
            return PageUtils.GetOpenWindowString("修改显示样式", PageUtils.GetCmsUrl(nameof(ModalTableStyleAdd), new NameValueCollection
            {
                {"PublishmentSystemID", publishmentSystemId.ToString()},
                {"TableStyleID", tableStyleId.ToString()},
                {"RelatedIdentities", TranslateUtils.ObjectCollectionToString(relatedIdentities)},
                {"TableName", tableName},
                {"AttributeName", attributeName},
                {"TableStyle", ETableStyleUtils.GetValue(tableStyle)},
                {"RedirectUrl", StringUtils.ValueToUrl(redirectUrl)}
            }));
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            _tableStyleId = Body.GetQueryInt("TableStyleID");
            _relatedIdentities = TranslateUtils.StringCollectionToIntList(Body.GetQueryString("RelatedIdentities"));
            if (_relatedIdentities.Count == 0)
            {
                _relatedIdentities.Add(0);
            }
            _tableName = Body.GetQueryString("TableName");
            _attributeName = Body.GetQueryString("AttributeName");
            _tableStyle = ETableStyleUtils.GetEnumType(Body.GetQueryString("TableStyle"));
            _redirectUrl = StringUtils.ValueFromUrl(Body.GetQueryString("RedirectUrl"));

            _styleInfo = _tableStyleId != 0 ? BaiRongDataProvider.TableStyleDao.GetTableStyleInfo(_tableStyleId) : TableStyleManager.GetTableStyleInfo(_tableStyle, _tableName, _attributeName, _relatedIdentities);

            if (!IsPostBack)
            {
                RblIsVisible.Items[0].Value = true.ToString();
                RblIsVisible.Items[1].Value = false.ToString();

                RblIsSingleLine.Items[0].Value = true.ToString();
                RblIsSingleLine.Items[1].Value = false.ToString();

                RblIsFormatString.Items[0].Value = true.ToString();
                RblIsFormatString.Items[1].Value = false.ToString();

                DdlIsHorizontal.Items[0].Value = true.ToString();
                DdlIsHorizontal.Items[1].Value = false.ToString();

                InputTypeUtils.AddListItems(DdlInputType);

                var arraylist = DataProvider.RelatedFieldDao.GetRelatedFieldInfoArrayList(PublishmentSystemId);
                foreach (RelatedFieldInfo rfInfo in arraylist)
                {
                    var listItem = new ListItem(rfInfo.RelatedFieldName, rfInfo.RelatedFieldID.ToString());
                    DdlRelatedFieldId.Items.Add(listItem);
                }

                ERelatedFieldStyleUtils.AddListItems(DdlRelatedFieldStyle);

                if (_styleInfo.TableStyleId != 0 || _attributeName == "IsHot" || _attributeName == "IsRecommend" || _attributeName == "IsColor" || _attributeName == "IsTop")
                {
                    DdlItemType.SelectedValue = false.ToString();
                }
                else
                {
                    DdlItemType.SelectedValue = true.ToString();
                }

                TbAttributeName.Text = _styleInfo.AttributeName;
                TbDisplayName.Text = _styleInfo.DisplayName;
                TbHelpText.Text = _styleInfo.HelpText;
                ControlUtils.SelectListItems(DdlInputType, _styleInfo.InputType);
                ControlUtils.SelectListItems(RblIsVisible, _styleInfo.IsVisible.ToString());
                ControlUtils.SelectListItems(RblIsSingleLine, _styleInfo.IsSingleLine.ToString());
                ControlUtils.SelectListItems(RblIsFormatString, _styleInfo.Additional.IsFormatString.ToString());
                TbDefaultValue.Text = _styleInfo.DefaultValue;
                DdlIsHorizontal.SelectedValue = _styleInfo.IsHorizontal.ToString();
                TbColumns.Text = _styleInfo.Additional.Columns.ToString();

                ControlUtils.SelectListItems(DdlRelatedFieldId, _styleInfo.Additional.RelatedFieldId.ToString());
                ControlUtils.SelectListItems(DdlRelatedFieldStyle, _styleInfo.Additional.RelatedFieldStyle);

                TbHeight.Text = _styleInfo.Additional.Height.ToString();
                TbWidth.Text = _styleInfo.Additional.Width;

                var styleItems = _styleInfo.StyleItems ?? BaiRongDataProvider.TableStyleDao.GetStyleItemInfoList(_styleInfo.TableStyleId);
                TbItemCount.Text = styleItems.Count.ToString();
                RptItems.DataSource = TableStyleManager.GetStyleItemDataSet(styleItems.Count, styleItems);
                RptItems.ItemDataBound += MyRepeater_ItemDataBound;
                RptItems.DataBind();
                if (RptItems.Items.Count > 0)
                {
                    DdlItemType.SelectedValue = false.ToString();
                }
            }

            ReFresh(null, EventArgs.Empty);
        }

        private static void MyRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var isSelected = TranslateUtils.ToBool(SqlUtils.EvalString(e.Item.DataItem, "IsSelected"));

                var isSelectedControl = (CheckBox)e.Item.FindControl("IsSelected");

                isSelectedControl.Checked = isSelected;
            }
        }

        public void ReFresh(object sender, EventArgs e)
        {
            TrRelatedField.Visible = TrHeightAndWidth.Visible = SpanDateTip.Visible = TrItemsType.Visible = TrItemsRapid.Visible = TrItems.Visible = TrRepeat.Visible = PhItemCount.Visible = PhIsFormatString.Visible = false;

            if (!string.IsNullOrEmpty(_attributeName))
            {
                TbAttributeName.Enabled = false;
            }

            var inputType = InputTypeUtils.GetEnumType(DdlInputType.SelectedValue);
            if (inputType == InputType.CheckBox || inputType == InputType.Radio || inputType == InputType.SelectMultiple || inputType == InputType.SelectOne)
            {
                TrItemsType.Visible = true;
                var isRapid = TranslateUtils.ToBool(DdlItemType.SelectedValue);
                if (isRapid)
                {
                    TrItemsRapid.Visible = true;
                    PhItemCount.Visible = false;
                    TrItems.Visible = false;
                }
                else
                {
                    TrItemsRapid.Visible = false;
                    PhItemCount.Visible = true;
                    TrItems.Visible = true;
                }
                if (inputType == InputType.CheckBox || inputType == InputType.Radio)
                {
                    TrRepeat.Visible = true;
                }
            }
            else if (inputType == InputType.TextEditor)
            {
                TrHeightAndWidth.Visible = true;
            }
            else if (inputType == InputType.TextArea)
            {
                TrHeightAndWidth.Visible = true;
            }
            else if (inputType == InputType.Text)
            {
                PhIsFormatString.Visible = true;
                TrHeightAndWidth.Visible = true;
            }
            else if (inputType == InputType.Date || inputType == InputType.DateTime)
            {
                SpanDateTip.Visible = true;
            }
            else if (inputType == InputType.RelatedField)
            {
                TrRelatedField.Visible = true;
            }
        }

        public void SetCount_OnClick(object sender, EventArgs e)
        {
            if (Page.IsPostBack)
            {
                var count = TranslateUtils.ToInt(TbItemCount.Text);
                if (count != 0)
                {
                    List<TableStyleItemInfo> styleItems = null;
                    if (_styleInfo.TableStyleId != 0)
                    {
                        styleItems = BaiRongDataProvider.TableStyleDao.GetStyleItemInfoList(_styleInfo.TableStyleId);
                    }
                    RptItems.DataSource = TableStyleManager.GetStyleItemDataSet(count, styleItems);
                    RptItems.DataBind();
                }
                else
                {
                    FailMessage("选项数目必须为大于0的数字！");
                }
            }
        }


        public override void Submit_OnClick(object sender, EventArgs e)
        {
            bool isChanged;

            var inputType = InputTypeUtils.GetEnumType(DdlInputType.SelectedValue);

            if (inputType == InputType.Radio || inputType == InputType.SelectMultiple || inputType == InputType.SelectOne)
            {
                var isRapid = TranslateUtils.ToBool(DdlItemType.SelectedValue);
                if (!isRapid)
                {
                    var itemCount = TranslateUtils.ToInt(TbItemCount.Text);
                    if (itemCount == 0)
                    {
                        FailMessage("操作失败，选项数目不能为0！");
                        return;
                    }
                }
            }  

            if (_styleInfo.TableStyleId == 0 && _styleInfo.RelatedIdentity == 0)//数据库中没有此项及父项的表样式
            {
                isChanged = InsertTableStyleInfo(inputType);
            }
            else if (_styleInfo.RelatedIdentity != _relatedIdentities[0])//数据库中没有此项的表样式，但是有父项的表样式
            {
                isChanged = InsertTableStyleInfo(inputType);
            }
            else//数据库中有此项的表样式
            {
                isChanged = UpdateTableStyleInfo(inputType);
            }

            if (isChanged)
            {
                PageUtils.CloseModalPageAndRedirect(Page, _redirectUrl);
            }
        }

        private bool UpdateTableStyleInfo(InputType inputType)
        {
            var isChanged = false;
            _styleInfo.AttributeName =TbAttributeName.Text;
            _styleInfo.DisplayName = PageUtils.FilterXss(TbDisplayName.Text);
            _styleInfo.HelpText = TbHelpText.Text;
            _styleInfo.IsVisible = TranslateUtils.ToBool(RblIsVisible.SelectedValue);
            _styleInfo.IsSingleLine = TranslateUtils.ToBool(RblIsSingleLine.SelectedValue);
            _styleInfo.InputType = InputTypeUtils.GetValue(inputType);
            _styleInfo.DefaultValue = TbDefaultValue.Text;
            _styleInfo.IsHorizontal = TranslateUtils.ToBool(DdlIsHorizontal.SelectedValue);

            _styleInfo.Additional.Columns = TranslateUtils.ToInt(TbColumns.Text);
            _styleInfo.Additional.Height = TranslateUtils.ToInt(TbHeight.Text);
            _styleInfo.Additional.Width = TbWidth.Text;
            _styleInfo.Additional.IsFormatString = TranslateUtils.ToBool(RblIsFormatString.SelectedValue);
            _styleInfo.Additional.RelatedFieldId = TranslateUtils.ToInt(DdlRelatedFieldId.SelectedValue);
            _styleInfo.Additional.RelatedFieldStyle = DdlRelatedFieldStyle.SelectedValue;

            List<TableStyleItemInfo> styleItems = null;

            if (inputType == InputType.CheckBox || inputType == InputType.Radio || inputType == InputType.SelectMultiple || inputType == InputType.SelectOne)
            {
                styleItems = new List<TableStyleItemInfo>();

                var isRapid = TranslateUtils.ToBool(DdlItemType.SelectedValue);
                if (isRapid)
                {
                    var itemArrayList = TranslateUtils.StringCollectionToStringList(TbItemValues.Text);
                    foreach (string itemValue in itemArrayList)
                    {
                        var itemInfo = new TableStyleItemInfo(0, _styleInfo.TableStyleId, itemValue, itemValue, false);
                        styleItems.Add(itemInfo);
                    }
                }
                else
                {
                    var isHasSelected = false;
                    foreach (RepeaterItem item in RptItems.Items)
                    {
                        var itemTitle = (TextBox)item.FindControl("ItemTitle");
                        var itemValue = (TextBox)item.FindControl("ItemValue");
                        var isSelected = (CheckBox)item.FindControl("IsSelected");

                        if ((inputType != InputType.SelectMultiple && inputType != InputType.CheckBox) && isHasSelected && isSelected.Checked)
                        {
                            FailMessage("操作失败，只能有一个初始化时选定项！");
                            return false;
                        }
                        if (isSelected.Checked) isHasSelected = true;

                        var itemInfo = new TableStyleItemInfo(0, _styleInfo.TableStyleId, itemTitle.Text, itemValue.Text, isSelected.Checked);
                        styleItems.Add(itemInfo);
                    }
                }
            }

            try
            {
                TableStyleManager.Update(_styleInfo);
                TableStyleManager.DeleteAndInsertStyleItems(_styleInfo.TableStyleId, styleItems);
                Body.AddSiteLog(PublishmentSystemId, "修改表单显示样式", $"类型:{ETableStyleUtils.GetText(_tableStyle)},字段名:{_styleInfo.AttributeName}");
                isChanged = true;
            }
            catch (Exception ex)
            {
                FailMessage(ex, "显示样式修改失败：" + ex.Message);
            }
            return isChanged;
        }

        private bool InsertTableStyleInfo(InputType inputType)
        {
            var isChanged = false;

            var relatedIdentity = _relatedIdentities[0];

            if (_tableStyle == ETableStyle.Site)
            {
                if (string.IsNullOrEmpty(TbAttributeName.Text))
                {
                    FailMessage("操作失败，字段名不能为空！");
                    return false;
                }
                else if (StringUtils.StartsWithIgnoreCase(TbAttributeName.Text, "Site"))
                {
                    FailMessage("操作失败，字段名不能以site开始！");
                    return false;
                }
            }

            if (TableStyleManager.IsExists(relatedIdentity, _tableName, TbAttributeName.Text))
            //|| TableStyleManager.IsExistsInParents(this.relatedIdentities, this.tableName, this.tbAttributeName.Text)      
            {
                FailMessage($@"显示样式添加失败：字段名""{TbAttributeName.Text}""已存在");
                return false;
            }

            _styleInfo = TableStyleManager.IsMetadata(_tableStyle, TbAttributeName.Text) ? TableStyleManager.GetTableStyleInfo(_tableStyle, _tableName, TbAttributeName.Text, _relatedIdentities) : new TableStyleInfo();

            _styleInfo.RelatedIdentity = relatedIdentity;
            _styleInfo.TableName = _tableName;
            _styleInfo.AttributeName = TbAttributeName.Text;
            _styleInfo.DisplayName =PageUtils.FilterXss(TbDisplayName.Text);
            _styleInfo.HelpText = TbHelpText.Text;
            _styleInfo.IsVisible = TranslateUtils.ToBool(RblIsVisible.SelectedValue);
            _styleInfo.IsSingleLine = TranslateUtils.ToBool(RblIsSingleLine.SelectedValue);
            _styleInfo.InputType = InputTypeUtils.GetValue(inputType);
            _styleInfo.DefaultValue = TbDefaultValue.Text;
            _styleInfo.IsHorizontal = TranslateUtils.ToBool(DdlIsHorizontal.SelectedValue);

            _styleInfo.Additional.Columns = TranslateUtils.ToInt(TbColumns.Text);
            _styleInfo.Additional.Height = TranslateUtils.ToInt(TbHeight.Text);
            _styleInfo.Additional.Width = TbWidth.Text;
            _styleInfo.Additional.IsFormatString = TranslateUtils.ToBool(RblIsFormatString.SelectedValue);
            _styleInfo.Additional.RelatedFieldId = TranslateUtils.ToInt(DdlRelatedFieldId.SelectedValue);
            _styleInfo.Additional.RelatedFieldStyle = DdlRelatedFieldStyle.SelectedValue;

            if (inputType == InputType.CheckBox || inputType == InputType.Radio || inputType == InputType.SelectMultiple || inputType == InputType.SelectOne)
            {
                _styleInfo.StyleItems = new List<TableStyleItemInfo>();

                var isRapid = TranslateUtils.ToBool(DdlItemType.SelectedValue);
                if (isRapid)
                {
                    var itemArrayList = TranslateUtils.StringCollectionToStringList(TbItemValues.Text);
                    foreach (string itemValue in itemArrayList)
                    {
                        var itemInfo = new TableStyleItemInfo(0, _styleInfo.TableStyleId, itemValue, itemValue, false);
                        _styleInfo.StyleItems.Add(itemInfo);
                    }
                }
                else
                {
                    var isHasSelected = false;
                    foreach (RepeaterItem item in RptItems.Items)
                    {
                        var itemTitle = (TextBox)item.FindControl("ItemTitle");
                        var itemValue = (TextBox)item.FindControl("ItemValue");
                        var isSelected = (CheckBox)item.FindControl("IsSelected");

                        if (inputType != InputType.SelectMultiple && inputType != InputType.CheckBox && isHasSelected && isSelected.Checked)
                        {
                            FailMessage("操作失败，只能有一个初始化时选定项！");
                            return false;
                        }
                        if (isSelected.Checked) isHasSelected = true;

                        var itemInfo = new TableStyleItemInfo(0, 0, itemTitle.Text, itemValue.Text, isSelected.Checked);
                        _styleInfo.StyleItems.Add(itemInfo);
                    }
                }
            }

            try
            {
                TableStyleManager.Insert(_styleInfo, _tableStyle);
                Body.AddSiteLog(PublishmentSystemId, "添加表单显示样式", $"类型:{ETableStyleUtils.GetText(_tableStyle)},字段名:{_styleInfo.AttributeName}");
                isChanged = true;
            }
            catch (Exception ex)
            {
                FailMessage(ex, "显示样式添加失败：" + ex.Message);
            }
            return isChanged;
        }
    }
}
