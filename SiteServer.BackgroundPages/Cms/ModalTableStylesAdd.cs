using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web.UI;
using System.Web.UI.WebControls;
using SiteServer.CMS.Context;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Model;
using SiteServer.Utils;
using SiteServer.Plugin;
using TableStyle = SiteServer.CMS.Model.TableStyle;

namespace SiteServer.BackgroundPages.Cms
{
	public class ModalTableStylesAdd : BasePageCms
    {
        public TextBox TbAttributeNames;
        public DropDownList DdlInputType;
        public Control SpDateTip;
        public PlaceHolder PhRepeat;
        public DropDownList DdlIsHorizontal;
        public TextBox TbColumns;
        public PlaceHolder PhWidth;
        public TextBox TbWidth;
        public PlaceHolder PhHeight;
        public TextBox TbHeight;
        public PlaceHolder PhDefaultValue;
        public TextBox TbDefaultValue;

        public PlaceHolder PhIsSelectField;
        public TextBox TbRapidValues;

        private List<int> _relatedIdentities;
        private string _tableName;
        private string _redirectUrl;

        public static string GetOpenWindowString(int siteId, List<int> relatedIdentities, string tableName, string redirectUrl)
        {
            return LayerUtils.GetOpenScript("批量添加显示样式", PageUtils.GetCmsUrl(siteId, nameof(ModalTableStylesAdd), new NameValueCollection
            {
                {"RelatedIdentities", TranslateUtils.ObjectCollectionToString(relatedIdentities)},
                {"TableName", tableName},
                {"RedirectUrl", StringUtils.ValueToUrl(redirectUrl)}
            }));
        }

		public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            _relatedIdentities = TranslateUtils.StringCollectionToIntList(AuthRequest.GetQueryString("RelatedIdentities"));
            if (_relatedIdentities.Count == 0)
            {
                _relatedIdentities.Add(0);
            }
            _tableName = AuthRequest.GetQueryString("TableName");
            _redirectUrl = StringUtils.ValueFromUrl(AuthRequest.GetQueryString("RedirectUrl"));

            if (!IsPostBack)
            {
                DdlIsHorizontal.Items[0].Value = true.ToString();
                DdlIsHorizontal.Items[1].Value = false.ToString();

                DdlInputType.Items.Add(InputTypeUtils.GetListItem(InputType.Text, false));
                DdlInputType.Items.Add(InputTypeUtils.GetListItem(InputType.TextArea, false));
                DdlInputType.Items.Add(InputTypeUtils.GetListItem(InputType.TextEditor, false));
                DdlInputType.Items.Add(InputTypeUtils.GetListItem(InputType.CheckBox, false));
                DdlInputType.Items.Add(InputTypeUtils.GetListItem(InputType.Radio, false));
                DdlInputType.Items.Add(InputTypeUtils.GetListItem(InputType.SelectOne, false));
                DdlInputType.Items.Add(InputTypeUtils.GetListItem(InputType.SelectMultiple, false));
                DdlInputType.Items.Add(InputTypeUtils.GetListItem(InputType.Date, false));
                DdlInputType.Items.Add(InputTypeUtils.GetListItem(InputType.DateTime, false));
                DdlInputType.Items.Add(InputTypeUtils.GetListItem(InputType.Image, false));
                DdlInputType.Items.Add(InputTypeUtils.GetListItem(InputType.Video, false));
                DdlInputType.Items.Add(InputTypeUtils.GetListItem(InputType.File, false));

                var style = TableStyleManager.GetTableStyleAsync(_tableName, string.Empty, _relatedIdentities).GetAwaiter().GetResult();

                ControlUtils.SelectSingleItem(DdlInputType, style.Type.Value);
                TbDefaultValue.Text = style.DefaultValue;
                DdlIsHorizontal.SelectedValue = style.Horizontal.ToString();
                TbColumns.Text = style.Columns.ToString();

                TbHeight.Text = style.Height == 0 ? string.Empty : style.Height.ToString();
                TbWidth.Text = style.Width;
            }

            ReFresh(null, EventArgs.Empty);
		}

        public void ReFresh(object sender, EventArgs e)
        {
            PhDefaultValue.Visible = PhWidth.Visible = PhHeight.Visible = SpDateTip.Visible = PhIsSelectField.Visible = PhRepeat.Visible = false;

            TbDefaultValue.TextMode = TextBoxMode.MultiLine;
            var inputType = InputTypeUtils.GetEnumType(DdlInputType.SelectedValue);
            if (inputType == InputType.CheckBox || inputType == InputType.Radio || inputType == InputType.SelectMultiple || inputType == InputType.SelectOne)
            {
                PhIsSelectField.Visible = true;
                if (inputType == InputType.CheckBox || inputType == InputType.Radio)
                {
                    PhRepeat.Visible = true;
                }
            }
            else if (inputType == InputType.TextEditor)
            {
                PhDefaultValue.Visible = PhWidth.Visible = PhHeight.Visible = true;
            }
            else if (inputType == InputType.TextArea)
            {
                PhDefaultValue.Visible = PhWidth.Visible = PhHeight.Visible = true;
            }
            else if (inputType == InputType.Text)
            {
                PhDefaultValue.Visible = PhWidth.Visible = true;
                TbDefaultValue.TextMode = TextBoxMode.SingleLine;
            }
            else if (inputType == InputType.Date || inputType == InputType.DateTime)
            {
                SpDateTip.Visible = PhDefaultValue.Visible = true;
                TbDefaultValue.TextMode = TextBoxMode.SingleLine;
            }
        }

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            var inputType = InputTypeUtils.GetEnumType(DdlInputType.SelectedValue);

            var isChanged = InsertTableStyle(inputType);

            if (isChanged)
            {
                LayerUtils.CloseAndRedirect(Page, _redirectUrl);
            }
		}

        private bool InsertTableStyle(InputType inputType)
        {
            var isChanged = false;

            var attributeNameArray = TbAttributeNames.Text.Split('\n');

            var relatedIdentity = _relatedIdentities[0];
            var styleArrayList = new ArrayList();

            foreach (var itemString in attributeNameArray)
            {
                if (string.IsNullOrEmpty(itemString)) continue;

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

                if (string.IsNullOrEmpty(attributeName))
                {
                    FailMessage("操作失败，字段名不能为空！");
                    return false;
                }

                if (TableStyleManager.IsExistsAsync(relatedIdentity, _tableName, attributeName).GetAwaiter().GetResult())
                {
                    FailMessage($@"显示样式添加失败：字段名""{attributeName}""已存在");
                    return false;
                }

                var style = new TableStyle
                {
                    Id = 0,
                    RelatedIdentity = relatedIdentity,
                    TableName = _tableName,
                    AttributeName = attributeName,
                    Taxis = 0,
                    DisplayName = displayName,
                    HelpText = string.Empty,
                    VisibleInList = false,
                    Type = inputType,
                    DefaultValue = TbDefaultValue.Text,
                    Horizontal = TranslateUtils.ToBool(DdlIsHorizontal.SelectedValue)
                };
                style.Columns = TranslateUtils.ToInt(TbColumns.Text);
                style.Height = TranslateUtils.ToInt(TbHeight.Text);
                style.Width = TbWidth.Text;

                if (inputType == InputType.CheckBox || inputType == InputType.Radio || inputType == InputType.SelectMultiple || inputType == InputType.SelectOne)
                {
                    style.StyleItems = new List<TableStyleItem>();

                    var rapidValues = TranslateUtils.StringCollectionToStringList(TbRapidValues.Text);
                    foreach (var rapidValue in rapidValues)
                    {
                        var itemInfo = new TableStyleItem
                        {
                            Id = 0,
                            TableStyleId = style.Id,
                            ItemTitle = rapidValue,
                            ItemValue = rapidValue,
                            Selected = false
                        };
                        style.StyleItems.Add(itemInfo);
                    }
                }

                styleArrayList.Add(style);
            }

            try
            {
                var attributeNames = new ArrayList();
                foreach (TableStyle style in styleArrayList)
                {
                    attributeNames.Add(style.AttributeName);
                    DataProvider.TableStyleDao.InsertAsync(style).GetAwaiter().GetResult();
                }
                

                if (SiteId > 0)
                {
                    AuthRequest.AddSiteLogAsync(SiteId, "批量添加表单显示样式", $"字段名: {TranslateUtils.ObjectCollectionToString(attributeNames)}").GetAwaiter().GetResult();
                }
                else
                {
                    AuthRequest.AddAdminLogAsync("批量添加表单显示样式", $"字段名: {TranslateUtils.ObjectCollectionToString(attributeNames)}").GetAwaiter().GetResult();
                }

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
