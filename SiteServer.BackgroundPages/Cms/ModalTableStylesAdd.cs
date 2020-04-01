using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web.UI;
using System.Web.UI.WebControls;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Model;
using SiteServer.Utils;
using SiteServer.Plugin;

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

                var styleInfo = TableStyleManager.GetTableStyleInfo(_tableName, string.Empty, _relatedIdentities);

                ControlUtils.SelectSingleItem(DdlInputType, styleInfo.InputType.Value);
                TbDefaultValue.Text = styleInfo.DefaultValue;
                DdlIsHorizontal.SelectedValue = styleInfo.IsHorizontal.ToString();
                TbColumns.Text = styleInfo.Additional.Columns.ToString();

                TbHeight.Text = styleInfo.Additional.Height == 0 ? string.Empty : styleInfo.Additional.Height.ToString();
                TbWidth.Text = styleInfo.Additional.Width;
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

            var isChanged = InsertTableStyleInfo(inputType);

            if (isChanged)
            {
                LayerUtils.CloseAndRedirect(Page, _redirectUrl);
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

                if (TableStyleManager.IsExists(relatedIdentity, _tableName, attributeName))
                {
                    FailMessage($@"显示样式添加失败：字段名""{attributeName}""已存在");
                    return false;
                }

                var styleInfo = new TableStyleInfo(0, relatedIdentity, _tableName, attributeName, 0, displayName, string.Empty, false, inputType, TbDefaultValue.Text, TranslateUtils.ToBool(DdlIsHorizontal.SelectedValue), string.Empty);
                styleInfo.Additional.Columns = TranslateUtils.ToInt(TbColumns.Text);
                styleInfo.Additional.Height = TranslateUtils.ToInt(TbHeight.Text);
                styleInfo.Additional.Width = TbWidth.Text;

                if (inputType == InputType.CheckBox || inputType == InputType.Radio || inputType == InputType.SelectMultiple || inputType == InputType.SelectOne)
                {
                    styleInfo.StyleItems = new List<TableStyleItemInfo>();

                    var rapidValues = TranslateUtils.StringCollectionToStringList(TbRapidValues.Text);
                    foreach (var rapidValue in rapidValues)
                    {
                        var itemInfo = new TableStyleItemInfo(0, styleInfo.Id, rapidValue, rapidValue, false);
                        styleInfo.StyleItems.Add(itemInfo);
                    }
                }

                styleInfoArrayList.Add(styleInfo);
            }

            try
            {
                var attributeNames = new ArrayList();
                foreach (TableStyleInfo styleInfo in styleInfoArrayList)
                {
                    attributeNames.Add(styleInfo.AttributeName);
                    DataProvider.TableStyleDao.Insert(styleInfo);
                }
                

                if (SiteId > 0)
                {
                    AuthRequest.AddSiteLog(SiteId, "批量添加表单显示样式", $"字段名: {TranslateUtils.ObjectCollectionToString(attributeNames)}");
                }
                else
                {
                    AuthRequest.AddAdminLog("批量添加表单显示样式", $"字段名: {TranslateUtils.ObjectCollectionToString(attributeNames)}");
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
