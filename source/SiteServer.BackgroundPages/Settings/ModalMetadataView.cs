using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web.UI;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.AuxiliaryTable;
using BaiRong.Core.Data;
using BaiRong.Core.Model.Enumerations;
using SiteServer.Plugin;
using SiteServer.Plugin.Models;

namespace SiteServer.BackgroundPages.Settings
{
	public class ModalTableMetadataView : BasePageCms
    {
        public PlaceHolder phAttribute;

        public Label lblAttributeName;
        public Label AuxiliaryTableENName;
        public Label DataType;
        public Label DataLength;
        public Label DataScale;
        public Label DataDigit;

        public Label DisplayName;
        public Label HelpText;
        public Label IsVisible;
        public Label IsValidate;
        public Label LbInputType;
        public Label DefaultValue;
        public Label IsHorizontal;
        public Repeater MyRepeater;

        public Control RowDefaultValue;
        public Control RowIsHorizontal;
        public Control RowSetItems;

        private EAuxiliaryTableType _tableType = EAuxiliaryTableType.BackgroundContent;
        private string _tableName;
        private string _attributeName;

        public static string GetOpenWindowString(EAuxiliaryTableType tableType, string tableName, string attributeName)
        {
            return PageUtils.GetOpenWindowString("辅助表字段查看", PageUtils.GetSettingsUrl(nameof(ModalTableMetadataView), new NameValueCollection
            {
                {"TableType", EAuxiliaryTableTypeUtils.GetValue(tableType)},
                {"TableName", tableName},
                {"AttributeName", attributeName}
            }), 480, 510, true);
        }

		public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            _tableType = EAuxiliaryTableTypeUtils.GetEnumType(Body.GetQueryString("TableType"));
            _tableName = Body.GetQueryString("TableName");
            _attributeName = Body.GetQueryString("AttributeName");

			if (!IsPostBack)
			{
                var metadataInfo = TableManager.GetTableMetadataInfo(_tableName, _attributeName);

                if (metadataInfo != null)
                {
                    lblAttributeName.Text = metadataInfo.AttributeName;
                    AuxiliaryTableENName.Text = metadataInfo.AuxiliaryTableEnName;
                    DataType.Text = metadataInfo.DataType.ToString();
                    DataLength.Text = metadataInfo.DataLength.ToString();

                    var styleInfo = TableStyleManager.GetTableStyleInfo(EAuxiliaryTableTypeUtils.GetTableStyle(_tableType), metadataInfo.AuxiliaryTableEnName, metadataInfo.AttributeName, new List<int>{0});

                    if (InputTypeUtils.EqualsAny(styleInfo.InputType, InputType.CheckBox, InputType.Radio, InputType.SelectMultiple, InputType.SelectOne))
                    {
                        RowDefaultValue.Visible = RowIsHorizontal.Visible = false;
                        RowSetItems.Visible = true;
                        if (InputTypeUtils.EqualsAny(styleInfo.InputType, InputType.CheckBox, InputType.Radio))
                        {
                            RowIsHorizontal.Visible = true;
                        }
                    }
                    else if (InputTypeUtils.EqualsAny(styleInfo.InputType, InputType.Text, InputType.TextArea, InputType.TextEditor))
                    {
                        RowDefaultValue.Visible = true;
                        RowSetItems.Visible = RowIsHorizontal.Visible = false;
                    }
                    else
                    {
                        RowDefaultValue.Visible = RowIsHorizontal.Visible = RowSetItems.Visible = false;
                    }

                    if (metadataInfo.IsSystem)
                    {
                        RowDefaultValue.Visible = RowIsHorizontal.Visible = RowSetItems.Visible = false;
                    }

                    DisplayName.Text = styleInfo.DisplayName;
                    HelpText.Text = styleInfo.HelpText;
                    IsVisible.Text = StringUtils.GetTrueOrFalseImageHtml(styleInfo.IsVisible.ToString());
                    IsValidate.Text = StringUtils.GetTrueImageHtml(styleInfo.Additional.IsValidate);
                    LbInputType.Text = InputTypeUtils.GetText(InputTypeUtils.GetEnumType(styleInfo.InputType));

                    DefaultValue.Text = styleInfo.DefaultValue;
                    IsHorizontal.Text = StringUtils.GetBoolText(styleInfo.IsHorizontal);

                    var styleItems = BaiRongDataProvider.TableStyleDao.GetStyleItemInfoList(styleInfo.TableStyleId);
                    MyRepeater.DataSource = TableStyleManager.GetStyleItemDataSet(styleItems.Count, styleItems);
                    MyRepeater.ItemDataBound += MyRepeater_ItemDataBound;
                    MyRepeater.DataBind();
                }
                else
                {
                    FailMessage("此字段为虚拟字段，在数据库中不存在！");
                    phAttribute.Visible = false;
                }
			}
		}

	    static void MyRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var itemTitle = SqlUtils.EvalString(e.Item.DataItem, "ItemTitle");
                var itemValue = SqlUtils.EvalString(e.Item.DataItem, "ItemValue");
                var isSelected = TranslateUtils.ToBool(SqlUtils.EvalString(e.Item.DataItem, "IsSelected"));

                var itemTitleControl = (Label)e.Item.FindControl("ItemTitle");
                var itemValueControl = (Label)e.Item.FindControl("ItemValue");
                var isSelectedControl = (Label)e.Item.FindControl("IsSelected");

                itemTitleControl.Text = itemTitle;
                itemValueControl.Text = itemValue;
                isSelectedControl.Text = StringUtils.GetTrueImageHtml(isSelected.ToString());
            }
        }
	}
}
