using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web.UI;
using System.Web.UI.WebControls;
using SiteServer.Utils;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Model;
using SiteServer.CMS.Model.Enumerations;
using SiteServer.Plugin;

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
        private TableStyleInfo _styleInfo;

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
            _relatedIdentities = TranslateUtils.StringCollectionToIntList(AuthRequest.GetQueryString("RelatedIdentities"));
            if (_relatedIdentities.Count == 0)
            {
                _relatedIdentities.Add(0);
            }
            _tableName = AuthRequest.GetQueryString("TableName");
            _attributeName = AuthRequest.GetQueryString("AttributeName");
            _redirectUrl = StringUtils.ValueFromUrl(AuthRequest.GetQueryString("RedirectUrl"));

            _styleInfo = _tableStyleId != 0 ? TableStyleManager.GetTableStyleInfo(_tableStyleId) : TableStyleManager.GetTableStyleInfo(_tableName, _attributeName, _relatedIdentities);

            if (IsPostBack) return;

            InputTypeUtils.AddListItems(DdlInputType);

            var relatedFieldInfoList = DataProvider.RelatedFieldDao.GetRelatedFieldInfoList(SiteId);
            foreach (var rfInfo in relatedFieldInfoList)
            {
                var listItem = new ListItem(rfInfo.Title, rfInfo.Id.ToString());
                DdlRelatedFieldId.Items.Add(listItem);
            }

            ERelatedFieldStyleUtils.AddListItems(DdlRelatedFieldStyle);

            ControlUtils.SelectSingleItem(DdlIsRapid, _styleInfo.Id != 0 ? false.ToString() : true.ToString());

            TbAttributeName.Text = _styleInfo.AttributeName;
            TbDisplayName.Text = _styleInfo.DisplayName;
            TbHelpText.Text = _styleInfo.HelpText;
            ControlUtils.SelectSingleItem(DdlInputType, _styleInfo.InputType.Value);
            TbTaxis.Text = _styleInfo.Taxis.ToString();
            ControlUtils.SelectSingleItem(DdlIsFormatString, _styleInfo.Additional.IsFormatString.ToString());
            TbDefaultValue.Text = _styleInfo.DefaultValue;
            DdlIsHorizontal.SelectedValue = _styleInfo.IsHorizontal.ToString();
            TbColumns.Text = _styleInfo.Additional.Columns.ToString();

            ControlUtils.SelectSingleItem(DdlRelatedFieldId, _styleInfo.Additional.RelatedFieldId.ToString());
            ControlUtils.SelectSingleItem(DdlRelatedFieldStyle, _styleInfo.Additional.RelatedFieldStyle);

            TbHeight.Text = _styleInfo.Additional.Height == 0 ? string.Empty : _styleInfo.Additional.Height.ToString();
            TbWidth.Text = _styleInfo.Additional.Width;

            var styleItems = _styleInfo.StyleItems ?? new List<TableStyleItemInfo>();
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
                if (item.IsSelected)
                {
                    isSelected = true;
                }
                if (item.ItemValue != item.ItemTitle)
                {
                    isNotEquals = true;
                }
            }

            DdlIsRapid.SelectedValue = (!isSelected && !isNotEquals).ToString();
            TbRapidValues.Text = string.Join(",", list);

            TbCustomizeLeft.Text = _styleInfo.Additional.CustomizeLeft;
            TbCustomizeRight.Text = _styleInfo.Additional.CustomizeRight;

            ReFresh(null, EventArgs.Empty);
        }

        private List<TableStyleItemInfo> GetDataSource(int count, List<TableStyleItemInfo> styleItems)
        {
            var items = new List<TableStyleItemInfo>();
            for (var i = 0; i < count; i++)
            {
                var itemInfo = styleItems != null && styleItems.Count > i ? styleItems[i] : new TableStyleItemInfo();
                items.Add(itemInfo);
            }
            return items;
        }

        private static void RptItems_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem) return;

            var itemInfo = (TableStyleItemInfo) e.Item.DataItem;

            var ltlSeq = (Literal) e.Item.FindControl("ltlSeq");
            var tbTitle = (TextBox) e.Item.FindControl("tbTitle");
            var tbValue = (TextBox) e.Item.FindControl("tbValue");
            var cbIsSelected = (CheckBox) e.Item.FindControl("cbIsSelected");

            ltlSeq.Text = (e.Item.ItemIndex + 1).ToString();
            tbTitle.Text = itemInfo.ItemTitle;
            tbValue.Text = itemInfo.ItemValue;
            cbIsSelected.Checked = itemInfo.IsSelected;
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
                List<TableStyleItemInfo> styleItems = null;
                if (_styleInfo.Id != 0)
                {
                    styleItems = _styleInfo.StyleItems;
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

            if (_styleInfo.Id == 0 && _styleInfo.RelatedIdentity == 0)//数据库中没有此项及父项的表样式
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
                LayerUtils.CloseAndRedirect(Page, _redirectUrl);
            }
        }

        private bool UpdateTableStyleInfo(InputType inputType)
        {
            var isChanged = false;
            _styleInfo.AttributeName =TbAttributeName.Text;
            _styleInfo.DisplayName = AttackUtils.FilterXss(TbDisplayName.Text);
            _styleInfo.HelpText = TbHelpText.Text;
            _styleInfo.Taxis = TranslateUtils.ToInt(TbTaxis.Text);
            _styleInfo.InputType = inputType;
            _styleInfo.DefaultValue = TbDefaultValue.Text;
            _styleInfo.IsHorizontal = TranslateUtils.ToBool(DdlIsHorizontal.SelectedValue);

            _styleInfo.Additional.Columns = TranslateUtils.ToInt(TbColumns.Text);
            _styleInfo.Additional.Height = TranslateUtils.ToInt(TbHeight.Text);
            _styleInfo.Additional.Width = TbWidth.Text;
            _styleInfo.Additional.IsFormatString = TranslateUtils.ToBool(DdlIsFormatString.SelectedValue);
            _styleInfo.Additional.RelatedFieldId = TranslateUtils.ToInt(DdlRelatedFieldId.SelectedValue);
            _styleInfo.Additional.RelatedFieldStyle = DdlRelatedFieldStyle.SelectedValue;
            _styleInfo.Additional.CustomizeLeft = TbCustomizeLeft.Text;
            _styleInfo.Additional.CustomizeRight = TbCustomizeRight.Text;

            _styleInfo.StyleItems = new List<TableStyleItemInfo>();

            if (inputType == InputType.CheckBox || inputType == InputType.Radio || inputType == InputType.SelectMultiple || inputType == InputType.SelectOne)
            {
                

                var isRapid = TranslateUtils.ToBool(DdlIsRapid.SelectedValue);
                if (isRapid)
                {
                    var rapidValues = TranslateUtils.StringCollectionToStringList(TbRapidValues.Text);
                    foreach (var rapidValue in rapidValues)
                    {
                        var itemInfo = new TableStyleItemInfo(0, _styleInfo.Id, rapidValue, rapidValue, false);
                        _styleInfo.StyleItems.Add(itemInfo);
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

                        var itemInfo = new TableStyleItemInfo(0, _styleInfo.Id, tbTitle.Text, tbValue.Text, cbIsSelected.Checked);
                        _styleInfo.StyleItems.Add(itemInfo);
                    }
                }
            }

            try
            {
                DataProvider.TableStyleDao.Update(_styleInfo);

                if (SiteId > 0)
                {
                    AuthRequest.AddSiteLog(SiteId, "修改表单显示样式", $"字段名:{_styleInfo.AttributeName}");
                }
                else
                {
                    AuthRequest.AddAdminLog("修改表单显示样式", $"字段名:{_styleInfo.AttributeName}");
                }
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

            if (string.IsNullOrEmpty(TbAttributeName.Text))
            {
                FailMessage("操作失败，字段名不能为空！");
                return false;
            }

            if (TableStyleManager.IsExists(relatedIdentity, _tableName, TbAttributeName.Text))   
            {
                FailMessage($@"显示样式添加失败：字段名""{TbAttributeName.Text}""已存在");
                return false;
            }

            _styleInfo = TableColumnManager.IsAttributeNameExists(_tableName, TbAttributeName.Text) ? TableStyleManager.GetTableStyleInfo(_tableName, TbAttributeName.Text, _relatedIdentities) : new TableStyleInfo();

            _styleInfo.RelatedIdentity = relatedIdentity;
            _styleInfo.TableName = _tableName;
            _styleInfo.AttributeName = TbAttributeName.Text;
            _styleInfo.DisplayName = AttackUtils.FilterXss(TbDisplayName.Text);
            _styleInfo.HelpText = TbHelpText.Text;
            _styleInfo.Taxis = TranslateUtils.ToInt(TbTaxis.Text);
            _styleInfo.InputType = inputType;
            _styleInfo.DefaultValue = TbDefaultValue.Text;
            _styleInfo.IsHorizontal = TranslateUtils.ToBool(DdlIsHorizontal.SelectedValue);

            _styleInfo.Additional.Columns = TranslateUtils.ToInt(TbColumns.Text);
            _styleInfo.Additional.Height = TranslateUtils.ToInt(TbHeight.Text);
            _styleInfo.Additional.Width = TbWidth.Text;
            _styleInfo.Additional.IsFormatString = TranslateUtils.ToBool(DdlIsFormatString.SelectedValue);
            _styleInfo.Additional.RelatedFieldId = TranslateUtils.ToInt(DdlRelatedFieldId.SelectedValue);
            _styleInfo.Additional.RelatedFieldStyle = DdlRelatedFieldStyle.SelectedValue;
            _styleInfo.Additional.CustomizeLeft = TbCustomizeLeft.Text;
            _styleInfo.Additional.CustomizeRight = TbCustomizeRight.Text;

            if (inputType == InputType.CheckBox || inputType == InputType.Radio || inputType == InputType.SelectMultiple || inputType == InputType.SelectOne)
            {
                _styleInfo.StyleItems = new List<TableStyleItemInfo>();

                var isRapid = TranslateUtils.ToBool(DdlIsRapid.SelectedValue);
                if (isRapid)
                {
                    var rapidValues = TranslateUtils.StringCollectionToStringList(TbRapidValues.Text);
                    foreach (var rapidValue in rapidValues)
                    {
                        var itemInfo = new TableStyleItemInfo(0, _styleInfo.Id, rapidValue, rapidValue, false);
                        _styleInfo.StyleItems.Add(itemInfo);
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

                        var itemInfo = new TableStyleItemInfo(0, 0, tbTitle.Text, tbValue.Text, cbIsSelected.Checked);
                        _styleInfo.StyleItems.Add(itemInfo);
                    }
                }
            }

            try
            {
                DataProvider.TableStyleDao.Insert(_styleInfo);

                if (SiteId > 0)
                {
                    AuthRequest.AddSiteLog(SiteId, "添加表单显示样式", $"字段名:{_styleInfo.AttributeName}");
                }
                else
                {
                    AuthRequest.AddAdminLog("添加表单显示样式", $"字段名:{_styleInfo.AttributeName}");
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
