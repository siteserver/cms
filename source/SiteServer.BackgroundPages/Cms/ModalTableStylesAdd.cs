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

namespace SiteServer.BackgroundPages.Cms
{
	public class ModalTableStylesAdd : BasePageCms
    {
        public TextBox AttributeNames;
        public RadioButtonList IsVisible;
        public RadioButtonList IsSingleLine;
        public DropDownList InputType;
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

        private List<int> _relatedIdentities;
        private string _tableName;
        private string _redirectUrl;
        private ETableStyle _tableStyle;

        public static string GetOpenWindowString(int publishmentSystemId, List<int> relatedIdentities, string tableName, ETableStyle tableStyle, string redirectUrl)
        {
            return PageUtils.GetOpenWindowString("批量添加显示样式", PageUtils.GetCmsUrl(nameof(ModalTableStylesAdd), new NameValueCollection
            {
                {"PublishmentSystemID", publishmentSystemId.ToString()},
                {"RelatedIdentities", TranslateUtils.ObjectCollectionToString(relatedIdentities)},
                {"TableName", tableName},
                {"TableStyle", ETableStyleUtils.GetValue(tableStyle)},
                {"RedirectUrl", StringUtils.ValueToUrl(redirectUrl)}
            }));
        }

		public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            _relatedIdentities = TranslateUtils.StringCollectionToIntList(Body.GetQueryString("RelatedIdentities"));
            if (_relatedIdentities.Count == 0)
            {
                _relatedIdentities.Add(0);
            }
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

                EInputTypeUtils.AddListItems(InputType);

                var styleInfo = TableStyleManager.GetTableStyleInfo(_tableStyle, _tableName, string.Empty, _relatedIdentities);

                ControlUtils.SelectListItems(InputType, EInputTypeUtils.GetValue(EInputTypeUtils.GetEnumType(styleInfo.InputType)));
                ControlUtils.SelectListItems(IsVisible, styleInfo.IsVisible.ToString());
                ControlUtils.SelectListItems(IsSingleLine, styleInfo.IsSingleLine.ToString());
                DefaultValue.Text = styleInfo.DefaultValue;
                IsHorizontal.SelectedValue = styleInfo.IsHorizontal.ToString();
                Columns.Text = styleInfo.Additional.Columns.ToString();

                Height.Text = styleInfo.Additional.Height.ToString();
                Width.Text = styleInfo.Additional.Width.ToString();

                ItemCount.Text = "0";

                
            }

            ReFresh(null, EventArgs.Empty);
		}

        public void ReFresh(object sender, EventArgs e)
        {
            RowDefaultValue.Visible = RowHeightAndWidth.Visible = DateTip.Visible = RowItemCount.Visible = RowSetItems.Visible = RowRepeat.Visible = false;
            Height.Enabled = true;

            DefaultValue.TextMode = TextBoxMode.MultiLine;
            var inputType = EInputTypeUtils.GetEnumType(InputType.SelectedValue);
            if (inputType == EInputType.CheckBox || inputType == EInputType.Radio || inputType == EInputType.SelectMultiple || inputType == EInputType.SelectOne)
            {
                RowItemCount.Visible = RowSetItems.Visible = true;
                if (inputType == EInputType.CheckBox || inputType == EInputType.Radio)
                {
                    RowRepeat.Visible = true;
                }
            }
            else if (inputType == EInputType.TextEditor)
            {
                RowDefaultValue.Visible = RowHeightAndWidth.Visible = true;
            }
            else if (inputType == EInputType.TextArea)
            {
                RowDefaultValue.Visible = RowHeightAndWidth.Visible = true;
            }
            else if (inputType == EInputType.Text)
            {
                RowDefaultValue.Visible = RowHeightAndWidth.Visible = true;
                Height.Enabled = false;
                DefaultValue.TextMode = TextBoxMode.SingleLine;
            }
            else if (inputType == EInputType.Date || inputType == EInputType.DateTime)
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
            var isChanged = false;

            var inputType = EInputTypeUtils.GetEnumType(InputType.SelectedValue);

            if (inputType == EInputType.CheckBox || inputType == EInputType.Radio || inputType == EInputType.SelectMultiple || inputType == EInputType.SelectOne)
            {
                var itemCount = TranslateUtils.ToInt(ItemCount.Text);
                if (itemCount == 0)
                {
                    FailMessage("操作失败，选项数目不能为0！");
                    return;
                }
            }

            isChanged = InsertTableStyleInfo(inputType);

            if (isChanged)
            {
                PageUtils.CloseModalPageAndRedirect(Page, _redirectUrl);
            }
		}

        private bool InsertTableStyleInfo(EInputType inputType)
        {
            var isChanged = false;

            var attributeNameArray = AttributeNames.Text.Split('\n');

            var relatedIdentity = (int)_relatedIdentities[0];
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

                    if (TableStyleManager.IsExists(relatedIdentity, _tableName, attributeName) || TableStyleManager.IsExistsInParents(_relatedIdentities, _tableName, attributeName))
                    {
                        FailMessage($@"显示样式添加失败：字段名""{attributeName}""已存在");
                        return false;
                    }

                    var styleInfo = new TableStyleInfo(0, relatedIdentity, _tableName, attributeName, 0, displayName, string.Empty, TranslateUtils.ToBool(IsVisible.SelectedValue), false, TranslateUtils.ToBool(IsSingleLine.SelectedValue), EInputTypeUtils.GetValue(inputType), DefaultValue.Text, TranslateUtils.ToBool(IsHorizontal.SelectedValue), string.Empty);
                    styleInfo.Additional.Columns = TranslateUtils.ToInt(Columns.Text);
                    styleInfo.Additional.Height = TranslateUtils.ToInt(Height.Text);
                    styleInfo.Additional.Width = Width.Text;

                    if (inputType == EInputType.CheckBox || inputType == EInputType.Radio || inputType == EInputType.SelectMultiple || inputType == EInputType.SelectOne)
                    {
                        styleInfo.StyleItems = new List<TableStyleItemInfo>();

                        var isHasSelected = false;
                        foreach (RepeaterItem item in MyRepeater.Items)
                        {
                            var ItemTitle = (TextBox)item.FindControl("ItemTitle");
                            var ItemValue = (TextBox)item.FindControl("ItemValue");
                            var IsSelected = (CheckBox)item.FindControl("IsSelected");

                            if ((inputType != EInputType.SelectMultiple && inputType != EInputType.CheckBox) && isHasSelected && IsSelected.Checked)
                            {
                                FailMessage("操作失败，只能有一个初始化时选定项！");
                                return false;
                            }
                            if (IsSelected.Checked) isHasSelected = true;

                            var itemInfo = new TableStyleItemInfo(0, 0, ItemTitle.Text, ItemValue.Text, IsSelected.Checked);
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
                Body.AddSiteLog(PublishmentSystemId, "批量添加表单显示样式", $"类型:{ETableStyleUtils.GetText(_tableStyle)},字段名: {TranslateUtils.ObjectCollectionToString(attributeNames)}");
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
