using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web.UI;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.AuxiliaryTable;
using BaiRong.Core.Data;
using BaiRong.Core.Model;
using BaiRong.Core.Model.Enumerations;

namespace SiteServer.BackgroundPages.Sys
{
    public class ModalTableStyleAdd : BasePage
    {
        public TextBox tbAttributeName;
        public TextBox tbDisplayName;
        public TextBox tbHelpText;
        public RadioButtonList rblIsVisible;
        public RadioButtonList rblIsSingleLine;
        public PlaceHolder phIsFormatString;
        public DropDownList ddlInputType;
        public RadioButtonList rblIsFormatString;
        public TextBox tbDefaultValue;
        public Control DateTip;
        public DropDownList ddlIsHorizontal;
        public TextBox tbColumns;
        public TextBox tbHeight;
        public TextBox tbWidth;

        public DropDownList ddlItemType;
        public PlaceHolder phItemCount;
        public TextBox tbItemCount;
        public TextBox tbItemValues;
        public Repeater MyRepeater;

        public Control rowRepeat;
        public Control rowRelatedField;
        public Control rowHeightAndWidth;
        public Control rowItemsType;
        public Control rowItemsRapid;
        public Control rowItems;

        private int _tableStyleId;
        private string _tableName;
        private string _attributeName;
        private string _redirectUrl;
        private TableStyleInfo _styleInfo;
        private ETableStyle _tableStyle;

        public static string GetOpenWindowString(int tableStyleId, string tableName, string attributeName, ETableStyle tableStyle, string redirectUrl)
        {
            return PageUtils.GetOpenWindowString("修改显示样式", PageUtils.GetSysUrl(nameof(ModalTableStyleAdd), new NameValueCollection
            {
                {"TableStyleID", tableStyleId.ToString()},
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
            _tableName = Body.GetQueryString("TableName");
            _attributeName = Body.GetQueryString("AttributeName");
            _tableStyle = ETableStyleUtils.GetEnumType(Body.GetQueryString("TableStyle"));
            _redirectUrl = StringUtils.ValueFromUrl(Body.GetQueryString("RedirectUrl"));

            _styleInfo = _tableStyleId != 0 ? BaiRongDataProvider.TableStyleDao.GetTableStyleInfo(_tableStyleId) : TableStyleManager.GetTableStyleInfo(_tableStyle, _tableName, _attributeName, new List<int>{0});

            if (!IsPostBack)
            {
                rblIsVisible.Items[0].Value = true.ToString();
                rblIsVisible.Items[1].Value = false.ToString();

                rblIsSingleLine.Items[0].Value = true.ToString();
                rblIsSingleLine.Items[1].Value = false.ToString();

                rblIsFormatString.Items[0].Value = true.ToString();
                rblIsFormatString.Items[1].Value = false.ToString();

                ddlIsHorizontal.Items[0].Value = true.ToString();
                ddlIsHorizontal.Items[1].Value = false.ToString();

                EInputTypeUtils.AddListItems(ddlInputType);

                if (_styleInfo.TableStyleId != 0 || _attributeName == "IsHot" || _attributeName == "IsRecommend" || _attributeName == "IsColor" || _attributeName == "IsTop")
                {
                    ddlItemType.SelectedValue = false.ToString();
                }
                else
                {
                    ddlItemType.SelectedValue = true.ToString();
                }

                tbAttributeName.Text = _styleInfo.AttributeName;
                tbDisplayName.Text = _styleInfo.DisplayName;
                tbHelpText.Text = _styleInfo.HelpText;
                ControlUtils.SelectListItems(ddlInputType, _styleInfo.InputType);
                ControlUtils.SelectListItems(rblIsVisible, _styleInfo.IsVisible.ToString());
                ControlUtils.SelectListItems(rblIsSingleLine, _styleInfo.IsSingleLine.ToString());
                ControlUtils.SelectListItems(rblIsFormatString, _styleInfo.Additional.IsFormatString.ToString());
                tbDefaultValue.Text = _styleInfo.DefaultValue;
                ddlIsHorizontal.SelectedValue = _styleInfo.IsHorizontal.ToString();
                tbColumns.Text = _styleInfo.Additional.Columns.ToString();

                tbHeight.Text = _styleInfo.Additional.Height.ToString();
                tbWidth.Text = _styleInfo.Additional.Width;

                var styleItems = _styleInfo.StyleItems ?? BaiRongDataProvider.TableStyleDao.GetStyleItemInfoList(_styleInfo.TableStyleId);
                tbItemCount.Text = styleItems.Count.ToString();
                MyRepeater.DataSource = TableStyleManager.GetStyleItemDataSet(styleItems.Count, styleItems);
                MyRepeater.ItemDataBound += MyRepeater_ItemDataBound;
                MyRepeater.DataBind();
                if (MyRepeater.Items.Count > 0)
                {
                    ddlItemType.SelectedValue = false.ToString();
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
            rowRelatedField.Visible = rowHeightAndWidth.Visible = DateTip.Visible = rowItemsType.Visible = rowItemsRapid.Visible = rowItems.Visible = rowRepeat.Visible = phItemCount.Visible = phIsFormatString.Visible = false;

            if (!string.IsNullOrEmpty(_attributeName))
            {
                tbAttributeName.Enabled = false;
            }

            var inputType = EInputTypeUtils.GetEnumType(ddlInputType.SelectedValue);
            if (inputType == EInputType.CheckBox || inputType == EInputType.Radio || inputType == EInputType.SelectMultiple || inputType == EInputType.SelectOne)
            {
                rowItemsType.Visible = true;
                var isRapid = TranslateUtils.ToBool(ddlItemType.SelectedValue);
                if (isRapid)
                {
                    rowItemsRapid.Visible = true;
                    phItemCount.Visible = false;
                    rowItems.Visible = false;
                }
                else
                {
                    rowItemsRapid.Visible = false;
                    phItemCount.Visible = true;
                    rowItems.Visible = true;
                }
                if (inputType == EInputType.CheckBox || inputType == EInputType.Radio)
                {
                    rowRepeat.Visible = true;
                }
            }
            else if (inputType == EInputType.TextEditor)
            {
                rowHeightAndWidth.Visible = true;
            }
            else if (inputType == EInputType.TextArea)
            {
                rowHeightAndWidth.Visible = true;
            }
            else if (inputType == EInputType.Text)
            {
                phIsFormatString.Visible = true;
                rowHeightAndWidth.Visible = true;
            }
            else if (inputType == EInputType.Date || inputType == EInputType.DateTime)
            {
                DateTip.Visible = true;
            }
            else if (inputType == EInputType.RelatedField)
            {
                rowRelatedField.Visible = true;
            }
        }

        public void SetCount_OnClick(object sender, EventArgs e)
        {
            if (Page.IsPostBack)
            {
                var count = TranslateUtils.ToInt(tbItemCount.Text);
                if (count != 0)
                {
                    List<TableStyleItemInfo> styleItems = null;
                    if (_styleInfo.TableStyleId != 0)
                    {
                        styleItems = BaiRongDataProvider.TableStyleDao.GetStyleItemInfoList(_styleInfo.TableStyleId);
                    }
                    MyRepeater.DataSource = TableStyleManager.GetStyleItemDataSet(count, styleItems);
                    MyRepeater.DataBind();
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

            var inputType = EInputTypeUtils.GetEnumType(ddlInputType.SelectedValue);

            if (inputType == EInputType.Radio || inputType == EInputType.SelectMultiple || inputType == EInputType.SelectOne)
            {
                var isRapid = TranslateUtils.ToBool(ddlItemType.SelectedValue);
                if (!isRapid)
                {
                    var itemCount = TranslateUtils.ToInt(tbItemCount.Text);
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
            else//数据库中有此项的表样式
            {
                isChanged = UpdateTableStyleInfo(inputType);
            }

            if (isChanged)
            {
                PageUtils.CloseModalPageAndRedirect(Page, _redirectUrl);
            }
        }

        private bool UpdateTableStyleInfo(EInputType inputType)
        {
            var isChanged = false;
            _styleInfo.AttributeName =tbAttributeName.Text;
            _styleInfo.DisplayName = PageUtils.FilterXss(tbDisplayName.Text);
            _styleInfo.HelpText = tbHelpText.Text;
            _styleInfo.IsVisible = TranslateUtils.ToBool(rblIsVisible.SelectedValue);
            _styleInfo.IsSingleLine = TranslateUtils.ToBool(rblIsSingleLine.SelectedValue);
            _styleInfo.InputType = EInputTypeUtils.GetValue(inputType);
            _styleInfo.DefaultValue = tbDefaultValue.Text;
            _styleInfo.IsHorizontal = TranslateUtils.ToBool(ddlIsHorizontal.SelectedValue);

            _styleInfo.Additional.Columns = TranslateUtils.ToInt(tbColumns.Text);
            _styleInfo.Additional.Height = TranslateUtils.ToInt(tbHeight.Text);
            _styleInfo.Additional.Width = tbWidth.Text;
            _styleInfo.Additional.IsFormatString = TranslateUtils.ToBool(rblIsFormatString.SelectedValue);

            ArrayList styleItems = null;

            if (inputType == EInputType.CheckBox || inputType == EInputType.Radio || inputType == EInputType.SelectMultiple || inputType == EInputType.SelectOne)
            {
                styleItems = new ArrayList();

                var isRapid = TranslateUtils.ToBool(ddlItemType.SelectedValue);
                if (isRapid)
                {
                    var itemArrayList = TranslateUtils.StringCollectionToStringList(tbItemValues.Text);
                    foreach (string itemValue in itemArrayList)
                    {
                        var itemInfo = new TableStyleItemInfo(0, _styleInfo.TableStyleId, itemValue, itemValue, false);
                        styleItems.Add(itemInfo);
                    }
                }
                else
                {
                    var isHasSelected = false;
                    foreach (RepeaterItem item in MyRepeater.Items)
                    {
                        var itemTitle = (TextBox)item.FindControl("ItemTitle");
                        var itemValue = (TextBox)item.FindControl("ItemValue");
                        var isSelected = (CheckBox)item.FindControl("IsSelected");

                        if ((inputType != EInputType.SelectMultiple && inputType != EInputType.CheckBox) && isHasSelected && isSelected.Checked)
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
                Body.AddAdminLog("修改表单显示样式", $"类型:{ETableStyleUtils.GetText(_tableStyle)},字段名:{_styleInfo.AttributeName}");
                isChanged = true;
            }
            catch (Exception ex)
            {
                FailMessage(ex, "显示样式修改失败：" + ex.Message);
            }
            return isChanged;
        }

        private bool InsertTableStyleInfo(EInputType inputType)
        {
            var isChanged = false;

            var relatedIdentity = 0;

            if (TableStyleManager.IsExists(relatedIdentity, _tableName, tbAttributeName.Text))
            //|| TableStyleManager.IsExistsInParents(this.relatedIdentities, this.tableName, this.tbAttributeName.Text)      
            {
                FailMessage($@"显示样式添加失败：字段名""{tbAttributeName.Text}""已存在");
                return false;
            }

            _styleInfo = TableStyleManager.IsMetadata(_tableStyle, tbAttributeName.Text) ? TableStyleManager.GetTableStyleInfo(_tableStyle, _tableName, tbAttributeName.Text, new List<int>{0}) : new TableStyleInfo();

            _styleInfo.RelatedIdentity = relatedIdentity;
            _styleInfo.TableName = _tableName;
            _styleInfo.AttributeName = tbAttributeName.Text;
            _styleInfo.DisplayName =PageUtils.FilterXss(tbDisplayName.Text);
            _styleInfo.HelpText = tbHelpText.Text;
            _styleInfo.IsVisible = TranslateUtils.ToBool(rblIsVisible.SelectedValue);
            _styleInfo.IsSingleLine = TranslateUtils.ToBool(rblIsSingleLine.SelectedValue);
            _styleInfo.InputType = EInputTypeUtils.GetValue(inputType);
            _styleInfo.DefaultValue = tbDefaultValue.Text;
            _styleInfo.IsHorizontal = TranslateUtils.ToBool(ddlIsHorizontal.SelectedValue);

            _styleInfo.Additional.Columns = TranslateUtils.ToInt(tbColumns.Text);
            _styleInfo.Additional.Height = TranslateUtils.ToInt(tbHeight.Text);
            _styleInfo.Additional.Width = tbWidth.Text;
            _styleInfo.Additional.IsFormatString = TranslateUtils.ToBool(rblIsFormatString.SelectedValue);

            if (inputType == EInputType.CheckBox || inputType == EInputType.Radio || inputType == EInputType.SelectMultiple || inputType == EInputType.SelectOne)
            {
                _styleInfo.StyleItems = new List<TableStyleItemInfo>();

                var isRapid = TranslateUtils.ToBool(ddlItemType.SelectedValue);
                if (isRapid)
                {
                    var itemArrayList = TranslateUtils.StringCollectionToStringList(tbItemValues.Text);
                    foreach (string itemValue in itemArrayList)
                    {
                        var itemInfo = new TableStyleItemInfo(0, _styleInfo.TableStyleId, itemValue, itemValue, false);
                        _styleInfo.StyleItems.Add(itemInfo);
                    }
                }
                else
                {
                    var isHasSelected = false;
                    foreach (RepeaterItem item in MyRepeater.Items)
                    {
                        var itemTitle = (TextBox)item.FindControl("ItemTitle");
                        var itemValue = (TextBox)item.FindControl("ItemValue");
                        var isSelected = (CheckBox)item.FindControl("IsSelected");

                        if (inputType != EInputType.SelectMultiple && inputType != EInputType.CheckBox && isHasSelected && isSelected.Checked)
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
                Body.AddAdminLog("添加表单显示样式", $"类型:{ETableStyleUtils.GetText(_tableStyle)},字段名:{_styleInfo.AttributeName}");
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
