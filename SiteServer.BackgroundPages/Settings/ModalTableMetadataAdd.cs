using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;
using SiteServer.CMS.Model.Attributes;
using SiteServer.Utils;
using SiteServer.Plugin;

namespace SiteServer.BackgroundPages.Settings
{
	public class ModalTableMetadataAdd : BasePageCms
    {
		public TextBox TbAttributeName;
		public DropDownList DdlDataType;
        public PlaceHolder PhDataLength;
        public TextBox TbDataLength;

        private string _tableName;

        public static string GetOpenWindowStringToAdd(string tableName)
        {
            return LayerUtils.GetOpenScript("添加内容表字段", PageUtils.GetSettingsUrl(nameof(ModalTableMetadataAdd), new NameValueCollection
            {
                {"TableName", tableName}
            }), 600, 320);
        }

        public static string GetOpenWindowStringToEdit(string tableName, int tableMetadataId)
        {
            return LayerUtils.GetOpenScript("修改内容表字段", PageUtils.GetSettingsUrl(nameof(ModalTableMetadataAdd), new NameValueCollection
            {
                {"TableName", tableName},
                {"TableMetadataID", tableMetadataId.ToString()}
            }), 600, 320);
        }

		public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("TableName");

            _tableName = AuthRequest.GetQueryString("TableName");

            if (IsPostBack) return;

            DataTypeUtils.AddListItems(DdlDataType);

            if (AuthRequest.IsQueryExists("TableMetadataID"))
            {
                var tableMetadataId = AuthRequest.GetQueryInt("TableMetadataID");
                var info = DataProvider.TableMetadataDao.GetTableMetadataInfo(tableMetadataId);
                if (info != null)
                {
                    TbAttributeName.Text = info.AttributeName;
                    TbAttributeName.Enabled = false;
                    ControlUtils.SelectSingleItemIgnoreCase(DdlDataType, info.DataType.ToString());
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

            if (AuthRequest.IsQueryExists("TableMetadataID"))
            {
                var tableMetadataId = AuthRequest.GetQueryInt("TableMetadataID");

                var info = DataProvider.TableMetadataDao.GetTableMetadataInfo(tableMetadataId);
                info.TableName = _tableName;
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
                    DataProvider.TableMetadataDao.Update(info);

                    AuthRequest.AddAdminLog("修改内容表字段",
                        $"内容表:{_tableName},字段名:{info.AttributeName}");

                    isChanged = true;
                }
                catch (Exception ex)
                {
                    FailMessage(ex, ex.Message);
                }
            }
            else
            {
                var attributeNameList = TableMetadataManager.GetAttributeNameList(_tableName, true);

                var attributeNameLowercase = TbAttributeName.Text.Trim().ToLower();
                if (attributeNameList.Contains(attributeNameLowercase) || ContentAttribute.AllAttributesLowercase.Contains(attributeNameLowercase))
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
                        TableName = _tableName,
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
                        DataProvider.TableMetadataDao.Insert(info);

                        AuthRequest.AddAdminLog("添加内容表字段",
                            $"内容表:{_tableName},字段名:{info.AttributeName}");

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
                LayerUtils.Close(Page);
            }
		}

	}
}
