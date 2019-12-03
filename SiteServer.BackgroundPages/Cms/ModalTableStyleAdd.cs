using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web.UI;
using System.Web.UI.WebControls;
using SiteServer.CMS.Context;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Context.Enumerations;
using SiteServer.CMS.Repositories;
using TableStyle = SiteServer.Abstractions.TableStyle;
using SiteServer.Abstractions;

namespace SiteServer.BackgroundPages.Cms
{
    public class ModalTableStyleAdd : BasePageCms
    {
        public TextBox TbAttributeName;
        public TextBox TbDisplayName;
        public TextBox TbHelpText;
        public TextBox TbTaxis;
        public PlaceHolder PhIsFormatString;
        public DropDownList DdlInputType;
        public DropDownList DdlIsFormatString;
        public PlaceHolder PhDefaultValue;
        public TextBox TbDefaultValue;
        public Control SpanDateTip;
        public DropDownList DdlIsHorizontal;
        public TextBox TbColumns;
        public PlaceHolder PhRelatedField;
        public DropDownList DdlRelatedFieldId;
        public DropDownList DdlRelatedFieldStyle;
        public PlaceHolder PhWidth;
        public TextBox TbWidth;
        public PlaceHolder PhHeight;
        public TextBox TbHeight;

        public PlaceHolder PhIsSelectField;
        public DropDownList DdlIsRapid;
        public PlaceHolder PhRapid;
        public TextBox TbRapidValues;
        public PlaceHolder PhNotRapid;
        public TextBox TbItemCount;
        public Repeater RptItems;
        public PlaceHolder PhRepeat;

        public PlaceHolder PhCustomize;
        public TextBox TbCustomizeLeft;
        public TextBox TbCustomizeRight;

        private int _tableStyleId;
        private List<int> _relatedIdentities;
        private string _tableName;
        private string _attributeName;
        private string _redirectUrl;
        private TableStyle _style;

        public static string GetOpenWindowString(int siteId, int tableStyleId, List<int> relatedIdentities, string tableName, string attributeName, string redirectUrl)
        {
            return LayerUtils.GetOpenScript(string.IsNullOrEmpty(attributeName) ? "新增字段" : "修改字段", PageUtils.GetCmsUrl(siteId, nameof(ModalTableStyleAdd), new NameValueCollection
            {
                {"TableStyleID", tableStyleId.ToString()},
                {"RelatedIdentities", TranslateUtils.ObjectCollectionToString(relatedIdentities)},
                {"TableName", tableName},
                {"AttributeName", attributeName},
                {"RedirectUrl", StringUtils.ValueToUrl(redirectUrl)}
            }));
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            _tableStyleId = AuthRequest.GetQueryInt("TableStyleID");
            _relatedIdentities = StringUtils.GetIntList(AuthRequest.GetQueryString("RelatedIdentities"));
            if (_relatedIdentities.Count == 0)
            {
                _relatedIdentities.Add(0);
            }
            _tableName = AuthRequest.GetQueryString("TableName");
            _attributeName = AuthRequest.GetQueryString("AttributeName");
            _redirectUrl = StringUtils.ValueFromUrl(AuthRequest.GetQueryString("RedirectUrl"));

            _style = _tableStyleId != 0 ? TableStyleManager.GetTableStyleAsync(_tableStyleId).GetAwaiter().GetResult() : TableStyleManager.GetTableStyleAsync(_tableName, _attributeName, _relatedIdentities).GetAwaiter().GetResult();

            if (IsPostBack) return;

            InputTypeUtils.AddListItems(DdlInputType);

            var relatedFieldInfoList = DataProvider.RelatedFieldRepository.GetRelatedFieldListAsync(SiteId).GetAwaiter().GetResult();
            foreach (var rfInfo in relatedFieldInfoList)
            {
                var listItem = new ListItem(rfInfo.Title, rfInfo.Id.ToString());
                DdlRelatedFieldId.Items.Add(listItem);
            }

            ERelatedFieldStyleUtilsExtensions.AddListItems(DdlRelatedFieldStyle);

            ControlUtils.SelectSingleItem(DdlIsRapid, _style.Id != 0 ? false.ToString() : true.ToString());

            TbAttributeName.Text = _style.AttributeName;
            TbDisplayName.Text = _style.DisplayName;
            TbHelpText.Text = _style.HelpText;
            ControlUtils.SelectSingleItem(DdlInputType, _style.Type.Value);
            TbTaxis.Text = _style.Taxis.ToString();
            ControlUtils.SelectSingleItem(DdlIsFormatString, _style.IsFormatString.ToString());
            TbDefaultValue.Text = _style.DefaultValue;
            DdlIsHorizontal.SelectedValue = _style.Horizontal.ToString();
            TbColumns.Text = _style.Columns.ToString();

            ControlUtils.SelectSingleItem(DdlRelatedFieldId, _style.RelatedFieldId.ToString());
            ControlUtils.SelectSingleItem(DdlRelatedFieldStyle, _style.RelatedFieldStyle);

            TbHeight.Text = _style.Height == 0 ? string.Empty : _style.Height.ToString();
            TbWidth.Text = _style.Width;

            var styleItems = _style.StyleItems ?? new List<TableStyleItem>();
            TbItemCount.Text = styleItems.Count.ToString();
            RptItems.DataSource = GetDataSource(styleItems.Count, styleItems);
            RptItems.ItemDataBound += RptItems_ItemDataBound;
            RptItems.DataBind();

            var isSelected = false;
            var isNotEquals = false;
            var list = new List<string>();
            foreach (var item in styleItems)
            {
                list.Add(item.ItemValue);
                if (item.Selected)
                {
                    isSelected = true;
                }
                if (item.ItemValue != item.ItemTitle)
                {
                    isNotEquals = true;
                }
            }

            DdlIsRapid.SelectedValue = (!isSelected && !isNotEquals).ToString();
            TbRapidValues.Text = StringUtils.Join(list);

            TbCustomizeLeft.Text = _style.CustomizeLeft;
            TbCustomizeRight.Text = _style.CustomizeRight;

            ReFresh(null, EventArgs.Empty);
        }

        private List<TableStyleItem> GetDataSource(int count, List<TableStyleItem> styleItems)
        {
            var items = new List<TableStyleItem>();
            for (var i = 0; i < count; i++)
            {
                var itemInfo = styleItems != null && styleItems.Count > i ? styleItems[i] : new TableStyleItem();
                items.Add(itemInfo);
            }
            return items;
        }

        private static void RptItems_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem) return;

            var itemInfo = (TableStyleItem) e.Item.DataItem;

            var ltlSeq = (Literal) e.Item.FindControl("ltlSeq");
            var tbTitle = (TextBox) e.Item.FindControl("tbTitle");
            var tbValue = (TextBox) e.Item.FindControl("tbValue");
            var cbIsSelected = (CheckBox) e.Item.FindControl("cbIsSelected");

            ltlSeq.Text = (e.Item.ItemIndex + 1).ToString();
            tbTitle.Text = itemInfo.ItemTitle;
            tbValue.Text = itemInfo.ItemValue;
            cbIsSelected.Checked = itemInfo.Selected;
        }

        public void ReFresh(object sender, EventArgs e)
        {
            PhRelatedField.Visible = PhWidth.Visible = PhHeight.Visible = SpanDateTip.Visible = PhIsSelectField.Visible = PhRapid.Visible = PhNotRapid.Visible = PhRepeat.Visible = PhIsFormatString.Visible = PhDefaultValue.Visible = PhCustomize.Visible = false;

            if (!string.IsNullOrEmpty(_attributeName))
            {
                TbAttributeName.Enabled = false;
            }

            var inputType = InputTypeUtils.GetEnumType(DdlInputType.SelectedValue);
            if (inputType == InputType.CheckBox || inputType == InputType.Radio || inputType == InputType.SelectMultiple || inputType == InputType.SelectOne)
            {
                PhDefaultValue.Visible = PhIsSelectField.Visible = true;
                var isRapid = TranslateUtils.ToBool(DdlIsRapid.SelectedValue);
                if (isRapid)
                {
                    PhRapid.Visible = true;
                    PhNotRapid.Visible = false;
                }
                else
                {
                    PhRapid.Visible = false;
                    PhNotRapid.Visible = true;
                }
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
                PhDefaultValue.Visible = PhIsFormatString.Visible = PhWidth.Visible = true;
            }
            else if (inputType == InputType.Date || inputType == InputType.DateTime)
            {
                PhDefaultValue.Visible = SpanDateTip.Visible = true;
            }
            else if (inputType == InputType.SelectCascading)
            {
                PhDefaultValue.Visible = PhRelatedField.Visible = true;
            }
            else if (inputType == InputType.Image || inputType == InputType.Video || inputType == InputType.File)
            {
                PhDefaultValue.Visible = true;
            }
            else if (inputType == InputType.Customize)
            {
                PhCustomize.Visible = true;
            }
        }

        public void SetCount_OnClick(object sender, EventArgs e)
        {
            if (!Page.IsPostBack) return;

            var count = TranslateUtils.ToInt(TbItemCount.Text);
            if (count != 0)
            {
                List<TableStyleItem> styleItems = null;
                if (_style.Id != 0)
                {
                    styleItems = _style.StyleItems;
                }
                RptItems.DataSource = GetDataSource(count, styleItems);
                RptItems.DataBind();
            }
            else
            {
                FailMessage("选项数目必须为大于0的数字！");
            }
        }

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            bool isChanged;

            var inputType = InputTypeUtils.GetEnumType(DdlInputType.SelectedValue);

            if (inputType == InputType.Radio || inputType == InputType.SelectMultiple || inputType == InputType.SelectOne)
            {
                var isRapid = TranslateUtils.ToBool(DdlIsRapid.SelectedValue);
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

            if (_style.Id == 0 && _style.RelatedIdentity == 0)//数据库中没有此项及父项的表样式
            {
                isChanged = InsertTableStyle(inputType);
            }
            else if (_style.RelatedIdentity != _relatedIdentities[0])//数据库中没有此项的表样式，但是有父项的表样式
            {
                isChanged = InsertTableStyle(inputType);
            }
            else//数据库中有此项的表样式
            {
                isChanged = UpdateTableStyle(inputType);
            }

            if (isChanged)
            {
                LayerUtils.CloseAndRedirect(Page, _redirectUrl);
            }
        }

        private bool UpdateTableStyle(InputType inputType)
        {
            var isChanged = false;
            _style.AttributeName =TbAttributeName.Text;
            _style.DisplayName = AttackUtils.FilterXss(TbDisplayName.Text);
            _style.HelpText = TbHelpText.Text;
            _style.Taxis = TranslateUtils.ToInt(TbTaxis.Text);
            _style.Type = inputType;
            _style.DefaultValue = TbDefaultValue.Text;
            _style.Horizontal = TranslateUtils.ToBool(DdlIsHorizontal.SelectedValue);

            _style.Columns = TranslateUtils.ToInt(TbColumns.Text);
            _style.Height = TranslateUtils.ToInt(TbHeight.Text);
            _style.Width = TbWidth.Text;
            _style.IsFormatString = TranslateUtils.ToBool(DdlIsFormatString.SelectedValue);
            _style.RelatedFieldId = TranslateUtils.ToInt(DdlRelatedFieldId.SelectedValue);
            _style.RelatedFieldStyle = DdlRelatedFieldStyle.SelectedValue;
            _style.CustomizeLeft = TbCustomizeLeft.Text;
            _style.CustomizeRight = TbCustomizeRight.Text;

            _style.StyleItems = new List<TableStyleItem>();

            if (inputType == InputType.CheckBox || inputType == InputType.Radio || inputType == InputType.SelectMultiple || inputType == InputType.SelectOne)
            {
                

                var isRapid = TranslateUtils.ToBool(DdlIsRapid.SelectedValue);
                if (isRapid)
                {
                    var rapidValues = StringUtils.GetStringList(TbRapidValues.Text);
                    foreach (var rapidValue in rapidValues)
                    {
                        var itemInfo = new TableStyleItem
                        {
                            Id = 0,
                            TableStyleId = _style.Id,
                            ItemTitle = rapidValue,
                            ItemValue = rapidValue,
                            Selected = false
                        };
                        _style.StyleItems.Add(itemInfo);
                    }
                }
                else
                {
                    var isHasSelected = false;
                    foreach (RepeaterItem item in RptItems.Items)
                    {
                        var tbTitle = (TextBox)item.FindControl("tbTitle");
                        var tbValue = (TextBox)item.FindControl("tbValue");
                        var cbIsSelected = (CheckBox)item.FindControl("cbIsSelected");

                        if (inputType != InputType.SelectMultiple && inputType != InputType.CheckBox && isHasSelected && cbIsSelected.Checked)
                        {
                            FailMessage("操作失败，只能有一个初始化时选定项！");
                            return false;
                        }
                        if (cbIsSelected.Checked) isHasSelected = true;

                        var itemInfo = new TableStyleItem
                        {
                            Id = 0,
                            TableStyleId = _style.Id,
                            ItemTitle = tbTitle.Text,
                            ItemValue = tbValue.Text,
                            Selected = cbIsSelected.Checked
                        };
                        _style.StyleItems.Add(itemInfo);
                    }
                }
            }

            try
            {
                DataProvider.TableStyleRepository.UpdateAsync(_style).GetAwaiter().GetResult();

                if (SiteId > 0)
                {
                    AuthRequest.AddSiteLogAsync(SiteId, "修改表单显示样式", $"字段名:{_style.AttributeName}").GetAwaiter().GetResult();
                }
                else
                {
                    AuthRequest.AddAdminLogAsync("修改表单显示样式", $"字段名:{_style.AttributeName}").GetAwaiter().GetResult();
                }
                isChanged = true;
            }
            catch (Exception ex)
            {
                FailMessage(ex, "显示样式修改失败：" + ex.Message);
            }
            return isChanged;
        }

        private bool InsertTableStyle(InputType inputType)
        {
            var isChanged = false;

            var relatedIdentity = _relatedIdentities[0];

            if (string.IsNullOrEmpty(TbAttributeName.Text))
            {
                FailMessage("操作失败，字段名不能为空！");
                return false;
            }

            if (TableStyleManager.IsExistsAsync(relatedIdentity, _tableName, TbAttributeName.Text).GetAwaiter().GetResult())   
            {
                FailMessage($@"显示样式添加失败：字段名""{TbAttributeName.Text}""已存在");
                return false;
            }

            _style = TableColumnManager.IsAttributeNameExists(_tableName, TbAttributeName.Text) ? TableStyleManager.GetTableStyleAsync(_tableName, TbAttributeName.Text, _relatedIdentities).GetAwaiter().GetResult() : new TableStyle();

            _style.RelatedIdentity = relatedIdentity;
            _style.TableName = _tableName;
            _style.AttributeName = TbAttributeName.Text;
            _style.DisplayName = AttackUtils.FilterXss(TbDisplayName.Text);
            _style.HelpText = TbHelpText.Text;
            _style.Taxis = TranslateUtils.ToInt(TbTaxis.Text);
            _style.Type = inputType;
            _style.DefaultValue = TbDefaultValue.Text;
            _style.Horizontal = TranslateUtils.ToBool(DdlIsHorizontal.SelectedValue);

            _style.Columns = TranslateUtils.ToInt(TbColumns.Text);
            _style.Height = TranslateUtils.ToInt(TbHeight.Text);
            _style.Width = TbWidth.Text;
            _style.IsFormatString = TranslateUtils.ToBool(DdlIsFormatString.SelectedValue);
            _style.RelatedFieldId = TranslateUtils.ToInt(DdlRelatedFieldId.SelectedValue);
            _style.RelatedFieldStyle = DdlRelatedFieldStyle.SelectedValue;
            _style.CustomizeLeft = TbCustomizeLeft.Text;
            _style.CustomizeRight = TbCustomizeRight.Text;

            if (inputType == InputType.CheckBox || inputType == InputType.Radio || inputType == InputType.SelectMultiple || inputType == InputType.SelectOne)
            {
                _style.StyleItems = new List<TableStyleItem>();

                var isRapid = TranslateUtils.ToBool(DdlIsRapid.SelectedValue);
                if (isRapid)
                {
                    var rapidValues = StringUtils.GetStringList(TbRapidValues.Text);
                    foreach (var rapidValue in rapidValues)
                    {
                        var itemInfo = new TableStyleItem
                        {
                            Id = 0,
                            TableStyleId = _style.Id,
                            ItemTitle = rapidValue,
                            ItemValue = rapidValue,
                            Selected = false
                        };
                        _style.StyleItems.Add(itemInfo);
                    }
                }
                else
                {
                    var isHasSelected = false;
                    foreach (RepeaterItem item in RptItems.Items)
                    {
                        var tbTitle = (TextBox)item.FindControl("tbTitle");
                        var tbValue = (TextBox)item.FindControl("tbValue");
                        var cbIsSelected = (CheckBox)item.FindControl("cbIsSelected");

                        if (inputType != InputType.SelectMultiple && inputType != InputType.CheckBox && isHasSelected && cbIsSelected.Checked)
                        {
                            FailMessage("操作失败，只能有一个初始化时选定项！");
                            return false;
                        }
                        if (cbIsSelected.Checked) isHasSelected = true;

                        var itemInfo = new TableStyleItem
                        {
                            Id = 0,
                            TableStyleId = 0,
                            ItemTitle = tbTitle.Text,
                            ItemValue = tbValue.Text,
                            Selected = cbIsSelected.Checked
                        };
                        _style.StyleItems.Add(itemInfo);
                    }
                }
            }

            try
            {
                DataProvider.TableStyleRepository.InsertAsync(_style).GetAwaiter().GetResult();

                if (SiteId > 0)
                {
                    AuthRequest.AddSiteLogAsync(SiteId, "添加表单显示样式", $"字段名:{_style.AttributeName}").GetAwaiter().GetResult();
                }
                else
                {
                    AuthRequest.AddAdminLogAsync("添加表单显示样式", $"字段名:{_style.AttributeName}").GetAwaiter().GetResult();
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
