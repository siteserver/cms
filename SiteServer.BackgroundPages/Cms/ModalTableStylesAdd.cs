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
using SiteServer.Plugin.Models;

namespace SiteServer.BackgroundPages.Cms
{
	public class ModalTableStylesAdd : BasePageCms
    {
        public TextBox TbAttributeNames;
        public DropDownList DdlIsVisible;
        public DropDownList DdlInputType;
        public TextBox TbDefaultValue;
        public Control SpDateTip;
        public DropDownList DdlIsHorizontal;
        public TextBox TbColumns;
        public TextBox TbHeight;
        public TextBox TbWidth;

        public TextBox TbItemCount;
        public Repeater RptContents;

        public PlaceHolder PhDefaultValue;
        public PlaceHolder PhRepeat;
        public PlaceHolder PhHeightAndWidth;
        public PlaceHolder PhItemCount;
        public PlaceHolder PhSetItems;

        private List<int> _relatedIdentities;
        private string _tableName;
        private string _redirectUrl;
        private ETableStyle _tableStyle;

        public static string GetOpenWindowString(int publishmentSystemId, List<int> relatedIdentities, string tableName, ETableStyle tableStyle, string redirectUrl)
        {
            return PageUtils.GetOpenLayerString("批量添加显示样式", PageUtils.GetCmsUrl(nameof(ModalTableStylesAdd), new NameValueCollection
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
                DdlIsVisible.Items[0].Value = true.ToString();
                DdlIsVisible.Items[1].Value = false.ToString();

                DdlIsHorizontal.Items[0].Value = true.ToString();
                DdlIsHorizontal.Items[1].Value = false.ToString();

                InputTypeUtils.AddListItems(DdlInputType);

                var styleInfo = TableStyleManager.GetTableStyleInfo(_tableStyle, _tableName, string.Empty, _relatedIdentities);

                ControlUtils.SelectListItems(DdlInputType, InputTypeUtils.GetValue(InputTypeUtils.GetEnumType(styleInfo.InputType)));
                ControlUtils.SelectListItems(DdlIsVisible, styleInfo.IsVisible.ToString());
                TbDefaultValue.Text = styleInfo.DefaultValue;
                DdlIsHorizontal.SelectedValue = styleInfo.IsHorizontal.ToString();
                TbColumns.Text = styleInfo.Additional.Columns.ToString();

                TbHeight.Text = styleInfo.Additional.Height.ToString();
                TbWidth.Text = styleInfo.Additional.Width;

                TbItemCount.Text = "0";
            }

            ReFresh(null, EventArgs.Empty);
		}

        public void ReFresh(object sender, EventArgs e)
        {
            PhDefaultValue.Visible = PhHeightAndWidth.Visible = SpDateTip.Visible = PhItemCount.Visible = PhSetItems.Visible = PhRepeat.Visible = false;
            TbHeight.Enabled = true;

            TbDefaultValue.TextMode = TextBoxMode.MultiLine;
            var inputType = InputTypeUtils.GetEnumType(DdlInputType.SelectedValue);
            if (inputType == InputType.CheckBox || inputType == InputType.Radio || inputType == InputType.SelectMultiple || inputType == InputType.SelectOne)
            {
                PhItemCount.Visible = PhSetItems.Visible = true;
                if (inputType == InputType.CheckBox || inputType == InputType.Radio)
                {
                    PhRepeat.Visible = true;
                }
            }
            else if (inputType == InputType.TextEditor)
            {
                PhDefaultValue.Visible = PhHeightAndWidth.Visible = true;
            }
            else if (inputType == InputType.TextArea)
            {
                PhDefaultValue.Visible = PhHeightAndWidth.Visible = true;
            }
            else if (inputType == InputType.Text)
            {
                PhDefaultValue.Visible = PhHeightAndWidth.Visible = true;
                TbHeight.Enabled = false;
                TbDefaultValue.TextMode = TextBoxMode.SingleLine;
            }
            else if (inputType == InputType.Date || inputType == InputType.DateTime)
            {
                SpDateTip.Visible = PhDefaultValue.Visible = true;
                TbDefaultValue.TextMode = TextBoxMode.SingleLine;
            }
        }

        public void SetCount_OnClick(object sender, EventArgs e)
        {
            if (Page.IsPostBack)
            {
                var count = TranslateUtils.ToInt(TbItemCount.Text);
                if (count != 0)
                {
                    RptContents.DataSource = TableStyleManager.GetStyleItemDataSet(count, null);
                    RptContents.DataBind();
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
                var itemCount = TranslateUtils.ToInt(TbItemCount.Text);
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

            var attributeNameArray = TbAttributeNames.Text.Split('\n');

            var relatedIdentity = _relatedIdentities[0];
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

                    if (_tableStyle == ETableStyle.Site)
                    {
                        if (string.IsNullOrEmpty(attributeName))
                        {
                            FailMessage("操作失败，字段名不能为空！");
                            return false;
                        }
                        else if (StringUtils.StartsWithIgnoreCase(attributeName, "Site"))
                        {
                            FailMessage("操作失败，字段名不能以site开始！");
                            return false;
                        }
                    }

                    if (TableStyleManager.IsExists(relatedIdentity, _tableName, attributeName) || TableStyleManager.IsExistsInParents(_relatedIdentities, _tableName, attributeName))
                    {
                        FailMessage($@"显示样式添加失败：字段名""{attributeName}""已存在");
                        return false;
                    }

                    var styleInfo = new TableStyleInfo(0, relatedIdentity, _tableName, attributeName, 0, displayName, string.Empty, TranslateUtils.ToBool(DdlIsVisible.SelectedValue), false, InputTypeUtils.GetValue(inputType), TbDefaultValue.Text, TranslateUtils.ToBool(DdlIsHorizontal.SelectedValue), string.Empty);
                    styleInfo.Additional.Columns = TranslateUtils.ToInt(TbColumns.Text);
                    styleInfo.Additional.Height = TranslateUtils.ToInt(TbHeight.Text);
                    styleInfo.Additional.Width = TbWidth.Text;

                    if (inputType == InputType.CheckBox || inputType == InputType.Radio || inputType == InputType.SelectMultiple || inputType == InputType.SelectOne)
                    {
                        styleInfo.StyleItems = new List<TableStyleItemInfo>();

                        var isHasSelected = false;
                        foreach (RepeaterItem item in RptContents.Items)
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
