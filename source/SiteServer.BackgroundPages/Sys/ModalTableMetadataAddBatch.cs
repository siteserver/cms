using System;
using System.Collections;
using System.Collections.Specialized;
using BaiRong.Core;
using BaiRong.Core.AuxiliaryTable;
using BaiRong.Core.Data;
using BaiRong.Core.Model;
using BaiRong.Core.Model.Enumerations;
using BaiRong.Core.Text;

namespace SiteServer.BackgroundPages.Sys
{
    public class ModalTableMetadataAddBatch : BasePageCms
    {
        private string _tableName;
        private EAuxiliaryTableType _tableType;

        public static string GetOpenWindowStringToAdd(string tableName, EAuxiliaryTableType tableType)
        {
            return PageUtils.GetOpenWindowString("批量添加辅助表字段",
                PageUtils.GetSysUrl(nameof(ModalTableMetadataAddBatch), new NameValueCollection
                {
                    {"TableName", tableName},
                    {"TableType", EAuxiliaryTableTypeUtils.GetValue(tableType)}
                }));
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("TableName", "TableType");

            _tableName = Body.GetQueryString("TableName");
            _tableType = EAuxiliaryTableTypeUtils.GetEnumType(Body.GetQueryString("TableType"));

            if (!IsPostBack)
            {

            }
        }


        public override void Submit_OnClick(object sender, EventArgs e)
        {
            var isChanged = false;

            var attributeNameList = TranslateUtils.StringCollectionToStringList(Request.Form["attributeName"]);
            var dataTypeList = TranslateUtils.StringCollectionToStringList(Request.Form["dataType"]);
            var dataLengthList = TranslateUtils.StringCollectionToStringList(Request.Form["dataLength"]);

            for (var i = 0; i < attributeNameList.Count; i++)
            {
                if (dataTypeList.Count < attributeNameList.Count)
                    dataTypeList.Add(string.Empty);
                if (dataLengthList.Count < attributeNameList.Count)
                    dataLengthList.Add(string.Empty);
            }

            var tableStyle = EAuxiliaryTableTypeUtils.GetTableStyle(_tableType);
            var attributeNameArrayList = TableManager.GetAttributeNameList(tableStyle, _tableName, true);
            attributeNameArrayList.AddRange(TableManager.GetHiddenAttributeNameList(tableStyle));

            for (var i = 0; i < attributeNameList.Count; i++)
            {
                var attributeName = attributeNameList[i];
                var dataType = dataTypeList[i];
                var dataLength = dataLengthList[i];

                if (attributeNameArrayList.IndexOf(attributeName.Trim().ToLower()) != -1)
                {
                    FailMessage("字段添加失败，字段名已存在！");
                }
                else if (!SqlUtils.IsAttributeNameCompliant(attributeName))
                {
                    FailMessage("字段名不符合系统要求！");
                }
                else
                {
                    var info = new TableMetadataInfo
                    {
                        AuxiliaryTableEnName = _tableName,
                        AttributeName = attributeName,
                        DataType = EDataTypeUtils.GetEnumType(dataType)
                    };

                    var hashtable = new Hashtable
                    {
                        [EDataType.DateTime] = new[] {"8", "false"},
                        [EDataType.Integer] = new[] {"4", "false"},
                        [EDataType.NChar] = new[] {"50", "true"},
                        [EDataType.NText] = new[] {"16", "false"},
                        [EDataType.NVarChar] = new[] {"255", "true"}
                    };

                    var strArr = (string[])hashtable[EDataTypeUtils.GetEnumType(dataType)];
                    if (strArr[1].Equals("false"))
                    {
                        dataLength = strArr[0];
                    }

                    if (string.IsNullOrEmpty(dataLength))
                    {
                        dataLength = strArr[0];
                    }

                    info.DataLength = int.Parse(dataLength);
                    if (info.DataType == EDataType.NVarChar || info.DataType == EDataType.NChar)
                    {
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
