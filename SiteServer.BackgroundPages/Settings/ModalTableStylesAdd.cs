using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web.UI;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.AuxiliaryTable;
using BaiRong.Core.Model;
using BaiRong.Core.Model.Enumerations;
using SiteServer.Plugin;
using SiteServer.Plugin.Models;

namespace SiteServer.BackgroundPages.Settings
{
	public class ModalTableStylesAdd : BasePage
    {
        public TextBox AttributeNames;
        public RadioButtonList IsVisible;
        public RadioButtonList IsSingleLine;
        public DropDownList DdlInputType;
        public TextBox DefaultValue;
        public Control DateTip;
        public DropDownList IsHorizontal;
        public TextBox Columns;
        public TextBox Height;
        public TextBox Width;

        public TextBox ItemCount;
        public Repeater MyRepeater;

        public Control RowDefaultValue;
        public Control RowRepeat;
        public Control RowHeightAndWidth;
        public Control RowItemCount;
        public Control RowSetItems;

        private string _tableName;
        private string _redirectUrl;
        private ETableStyle _tableStyle;

        public static string GetOpenWindowString(string tableName, ETableStyle tableStyle, string redirectUrl)
        {
            return PageUtils.GetOpenWindowString("批量添加显示样式", PageUtils.GetSettingsUrl(nameof(ModalTableStylesAdd), new NameValueCollection
            {
                {"TableName", tableName},
                {"TableStyle", ETableStyleUtils.GetValue(tableStyle)},
                {"RedirectUrl", StringUtils.ValueToUrl(redirectUrl)}
            }));
        }

		public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            _tableName = Body.GetQueryString("TableName");
            _tableStyle = ETableStyleUtils.GetEnumType(Body.GetQueryString("TableStyle"));
            _redirectUrl = StringUtils.ValueFromUrl(Body.GetQueryString("RedirectUrl"));

            if (!IsPostBack)
            {
                IsVisible.Items[0].Value = true.ToString();
                IsVisible.Items[1].Value = false.ToString();

                IsSingleLine.Items[0].Value = true.ToString();
                IsSingleLine.Items[1].Value = false.ToString();

                IsHorizontal.Items[0].Value = true.ToString();
                IsHorizontal.Items[1].Value = false.ToString();

                InputTypeUtils.AddListItems(DdlInputType);

                var styleInfo = TableStyleManager.GetTableStyleInfo(_tableStyle, _tableName, string.Empty, new List<int>{0});

                ControlUtils.SelectListItems(DdlInputType, InputTypeUtils.GetValue(InputTypeUtils.GetEnumType(styleInfo.InputType)));
                ControlUtils.SelectListItems(IsVisible, styleInfo.IsVisible.ToString());
                ControlUtils.SelectListItems(IsSingleLine, styleInfo.IsSingleLine.ToString());
                DefaultValue.Text = styleInfo.DefaultValue;
                IsHorizontal.SelectedValue = styleInfo.IsHorizontal.ToString();
                Columns.Text = styleInfo.Additional.Columns.ToString();

                Height.Text = styleInfo.Additional.Height.ToString();
                Width.Text = styleInfo.Additional.Width;

                ItemCount.Text = "0";

                
            }

            ReFresh(null, EventArgs.Empty);
		}

        public void ReFresh(object sender, EventArgs e)
        {
            RowDefaultValue.Visible = RowHeightAndWidth.Visible = DateTip.Visible = RowItemCount.Visible = RowSetItems.Visible = RowRepeat.Visible = false;
            Height.Enabled = true;

            DefaultValue.TextMode = TextBoxMode.MultiLine;
            var inputType = InputTypeUtils.GetEnumType(DdlInputType.SelectedValue);
            if (inputType == InputType.CheckBox || inputType == InputType.Radio || inputType == InputType.SelectMultiple || inputType == InputType.SelectOne)
            {
                RowItemCount.Visible = RowSetItems.Visible = true;
                if (inputType == InputType.CheckBox || inputType == InputType.Radio)
                {
                    RowRepeat.Visible = true;
                }
            }
            else if (inputType == InputType.TextEditor)
            {
                RowDefaultValue.Visible = RowHeightAndWidth.Visible = true;
            }
            else if (inputType == InputType.TextArea)
            {
                RowDefaultValue.Visible = RowHeightAndWidth.Visible = true;
            }
            else if (inputType == InputType.Text)
            {
                RowDefaultValue.Visible = RowHeightAndWidth.Visible = true;
                Height.Enabled = false;
                DefaultValue.TextMode = TextBoxMode.SingleLine;
            }
            else if (inputType == InputType.Date || inputType == InputType.DateTime)
            {
                DateTip.Visible = RowDefaultValue.Visible = true;
                DefaultValue.TextMode = TextBoxMode.SingleLine;
            }
        }

        public void SetCount_OnClick(object sender, EventArgs e)
        {
            if (Page.IsPostBack)
            {
                var count = TranslateUtils.ToInt(ItemCount.Text);
                if (count != 0)
                {
                    MyRepeater.DataSource = TableStyleManager.GetStyleItemDataSet(count, null);
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
            var inputType = InputTypeUtils.GetEnumType(DdlInputType.SelectedValue);

            if (inputType == InputType.CheckBox || inputType == InputType.Radio || inputType == InputType.SelectMultiple || inputType == InputType.SelectOne)
            {
                var itemCount = TranslateUtils.ToInt(ItemCount.Text);
                if (itemCount == 0)
                {
                    FailMessage("操作失败，选项数目不能为0！");
                    return;
                }
            }

            var isChanged = InsertTableStyleInfo(inputType);

            if (isChanged)
            {
                PageUtils.CloseModalPageAndRedirect(Page, _redirectUrl);
            }
		}

        private bool InsertTableStyleInfo(InputType inputType)
        {
            var isChanged = false;

            var attributeNameArray = AttributeNames.Text.Split('\n');

            var relatedIdentity = 0;
            var styleInfoArrayList = new ArrayList();

            foreach (var itemString in attributeNameArray)
            {
                if (!string.IsNullOrEmpty(itemString))
                {
                    var attributeName = itemString;
                    var displayName = string.Empty;

                    if (StringUtils.Contains(itemString, "(") && StringUtils.Contains(itemString, ")"))
                    {
                        var length = itemString.IndexOf(')') - itemString.IndexOf('(');
                        if (length > 0)
                        {
                            displayName = itemString.Substring(itemString.IndexOf('(') + 1, length);
                            attributeName = itemString.Substring(0, itemString.IndexOf('('));
                        }
                    }
                    attributeName = attributeName.Trim();
                    displayName = displayName.Trim(' ', '(', ')');
                    if (string.IsNullOrEmpty(displayName))
                    {
                        displayName = attributeName;
                    }

                    if (TableStyleManager.IsExists(relatedIdentity, _tableName, attributeName))
                    {
                        FailMessage($@"显示样式添加失败：字段名""{attributeName}""已存在");
                        return false;
                    }

                    var styleInfo = new TableStyleInfo(0, relatedIdentity, _tableName, attributeName, 0, displayName, string.Empty, TranslateUtils.ToBool(IsVisible.SelectedValue), false, TranslateUtils.ToBool(IsSingleLine.SelectedValue), InputTypeUtils.GetValue(inputType), DefaultValue.Text, TranslateUtils.ToBool(IsHorizontal.SelectedValue), string.Empty);
                    styleInfo.Additional.Columns = TranslateUtils.ToInt(Columns.Text);
                    styleInfo.Additional.Height = TranslateUtils.ToInt(Height.Text);
                    styleInfo.Additional.Width = Width.Text;

                    if (inputType == InputType.CheckBox || inputType == InputType.Radio || inputType == InputType.SelectMultiple || inputType == InputType.SelectOne)
                    {
                        styleInfo.StyleItems = new List<TableStyleItemInfo>();

                        var isHasSelected = false;
                        foreach (RepeaterItem item in MyRepeater.Items)
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

                            var itemInfo = new TableStyleItemInfo(0, 0, itemTitle.Text, itemValue.Text, isSelected.Checked);
                            styleInfo.StyleItems.Add(itemInfo);
                        }
                    }

                    styleInfoArrayList.Add(styleInfo);
                }
            }

            try
            {
                var attributeNames = new ArrayList();
                foreach (TableStyleInfo styleInfo in styleInfoArrayList)
                {
                    attributeNames.Add(styleInfo.AttributeName);
                    TableStyleManager.Insert(styleInfo, _tableStyle);
                }
                Body.AddAdminLog("批量添加表单显示样式", $"类型:{ETableStyleUtils.GetText(_tableStyle)},字段名: {TranslateUtils.ObjectCollectionToString(attributeNames)}");
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
