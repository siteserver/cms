using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.AuxiliaryTable;
using BaiRong.Core.Data;
using BaiRong.Core.Model;
using BaiRong.Core.Model.Enumerations;
using SiteServer.Plugin.Models;

namespace SiteServer.BackgroundPages.Settings
{
	public class ModalTableMetadataAdd : BasePageCms
    {
		public TextBox TbAttributeName;
		public DropDownList DdlDataType;
        public PlaceHolder PhDataLength;
        public TextBox TbDataLength;

        private string _tableName;
        private EAuxiliaryTableType _tableType;

        public static string GetOpenWindowStringToAdd(string tableName, EAuxiliaryTableType tableType)
        {
            return PageUtils.GetOpenLayerString("添加辅助表字段", PageUtils.GetSettingsUrl(nameof(ModalTableMetadataAdd), new NameValueCollection
            {
                {"TableName", tableName},
                {"TableType", EAuxiliaryTableTypeUtils.GetValue(tableType)}
            }), 600, 320);
        }

        public static string GetOpenWindowStringToEdit(string tableName, EAuxiliaryTableType tableType, int tableMetadataId)
        {
            return PageUtils.GetOpenLayerString("修改辅助表字段", PageUtils.GetSettingsUrl(nameof(ModalTableMetadataAdd), new NameValueCollection
            {
                {"TableName", tableName},
                {"TableType", EAuxiliaryTableTypeUtils.GetValue(tableType)},
                {"TableMetadataID", tableMetadataId.ToString()}
            }), 600, 320);
        }

		public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("TableName", "TableType");

            _tableName = Body.GetQueryString("TableName");
            _tableType = EAuxiliaryTableTypeUtils.GetEnumType(Body.GetQueryString("TableType"));

            if (IsPostBack) return;

            DataTypeUtils.AddListItems(DdlDataType);

            if (Body.IsQueryExists("TableMetadataID"))
            {
                var tableMetadataId = Body.GetQueryInt("TableMetadataID");
                var info = BaiRongDataProvider.TableMetadataDao.GetTableMetadataInfo(tableMetadataId);
                if (info != null)
                {
                    TbAttributeName.Text = info.AttributeName;
                    TbAttributeName.Enabled = false;
                    ControlUtils.SelectListItemsIgnoreCase(DdlDataType, info.DataType.ToString());
                    TbDataLength.Text = info.DataLength.ToString();
                }
            }
        }

        public void DdlDataType_SelectedIndexChanged(object sender, EventArgs e)
        {
            PhDataLength.Visible = DataTypeUtils.GetEnumType(DdlDataType.SelectedValue) == DataType.VarChar;
        }

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            var isChanged = false;

            if (Body.IsQueryExists("TableMetadataID"))
            {
                var tableMetadataId = Body.GetQueryInt("TableMetadataID");

                var info = BaiRongDataProvider.TableMetadataDao.GetTableMetadataInfo(tableMetadataId);
                info.AuxiliaryTableEnName = _tableName;
                info.AttributeName = TbAttributeName.Text;
                info.DataType = DataTypeUtils.GetEnumType(DdlDataType.SelectedValue);

                if (info.DataType == DataType.VarChar)
                {
                    info.DataLength = TranslateUtils.ToInt(TbDataLength.Text);
                    var maxLength = SqlUtils.GetMaxLengthForNVarChar();
                    if (info.DataLength <= 0 || info.DataLength > maxLength)
                    {
                        FailMessage($"字段修改失败，数据长度的值必须位于 1 和 {maxLength} 之间");
                        return;
                    }
                }

                try
                {
                    BaiRongDataProvider.TableMetadataDao.Update(info);

                    Body.AddAdminLog("修改辅助表字段",
                        $"辅助表:{_tableName},字段名:{info.AttributeName}");

                    isChanged = true;
                }
                catch (Exception ex)
                {
                    FailMessage(ex, ex.Message);
                }
            }
            else
            {
                var tableStyle = EAuxiliaryTableTypeUtils.GetTableStyle(_tableType);
                var attributeNameList = TableManager.GetAttributeNameList(tableStyle, _tableName, true);
                attributeNameList.AddRange(TableManager.GetHiddenAttributeNameList(tableStyle));
                if (attributeNameList.IndexOf(TbAttributeName.Text.Trim().ToLower()) != -1)
                {
                    FailMessage("字段添加失败，字段名已存在！");
                }
                else if (!SqlUtils.IsAttributeNameCompliant(TbAttributeName.Text))
                {
                    FailMessage("字段名不符合系统要求！");
                }
                else
                {
                    var info = new TableMetadataInfo
                    {
                        AuxiliaryTableEnName = _tableName,
                        AttributeName = TbAttributeName.Text,
                        DataType = DataTypeUtils.GetEnumType(DdlDataType.SelectedValue)
                    };

                    if (info.DataType == DataType.VarChar)
                    {
                        info.DataLength = TranslateUtils.ToInt(TbDataLength.Text);
                        var maxLength = SqlUtils.GetMaxLengthForNVarChar();
                        if (info.DataLength <= 0 || info.DataLength > maxLength)
                        {
                            FailMessage($"字段修改失败，数据长度的值必须位于 1 和 {maxLength} 之间");
                            return;
                        }
                    }
                    info.IsSystem = false;

                    try
                    {
                        BaiRongDataProvider.TableMetadataDao.Insert(info);

                        Body.AddAdminLog("添加辅助表字段",
                            $"辅助表:{_tableName},字段名:{info.AttributeName}");

                        isChanged = true;
                    }
                    catch (Exception ex)
                    {
                        FailMessage(ex, ex.Message);
                    }
                }
            }

            if (isChanged)
            {
                PageUtils.CloseModalPage(Page);
            }
		}

	}
}
