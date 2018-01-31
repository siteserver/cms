using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;
using SiteServer.Utils;

namespace SiteServer.BackgroundPages.Settings
{
    public class ModalAuxiliaryTableAdd : BasePageCms
    {
        public TextBox TbTableName;
        public TextBox TbDisplayName;
        public TextBox TbDescription;

        private string _tableName;

        public static string GetOpenWindowString()
        {
            return LayerUtils.GetOpenScript("添加辅助表", PageUtils.GetSettingsUrl(nameof(ModalAuxiliaryTableAdd), null), 580, 450);
        }

        public static string GetOpenWindowString(string tableName)
        {
            return LayerUtils.GetOpenScript("编辑辅助表", PageUtils.GetSettingsUrl(nameof(ModalAuxiliaryTableAdd), new NameValueCollection
            {
                {"tableName", tableName}
            }), 580, 450);
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            _tableName = Body.GetQueryString("tableName");

            if (IsPostBack) return;

            VerifyAdministratorPermissions(ConfigManager.Permissions.Settings.Site);

            if (!string.IsNullOrEmpty(_tableName))
            {
                var info = DataProvider.TableDao.GetTableCollectionInfo(_tableName);
                if (info != null)
                {
                    TbTableName.Text = info.TableName;
                    TbTableName.Enabled = false;
                    TbDisplayName.Text = info.DisplayName;
                    TbDescription.Text = info.Description;
                }
            }
        }

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            if (!Page.IsPostBack || !Page.IsValid) return;

            if (!string.IsNullOrEmpty(_tableName))
            {
                var info = DataProvider.TableDao.GetTableCollectionInfo(_tableName);
                info.DisplayName = TbDisplayName.Text;
                info.Description = TbDescription.Text;

                DataProvider.TableDao.Update(info);

                Body.AddAdminLog("修改辅助表", $"辅助表:{_tableName}");

                SuccessMessage("辅助表修改成功！");
                LayerUtils.Close(Page);
            }
            else
            {
                var tableNameList = DataProvider.TableDao.GetTableNameList();
                if (tableNameList.IndexOf(TbTableName.Text) != -1)
                {
                    FailMessage("辅助表添加失败，辅助表标识已存在！");
                }
                else if (DataProvider.DatabaseDao.IsTableExists(TbTableName.Text))
                {
                    FailMessage("辅助表添加失败，数据库中已存在此表！");
                }
                else
                {
                    var info = new TableInfo
                    {
                        TableName = TbTableName.Text,
                        DisplayName = TbDisplayName.Text,
                        Description = TbDescription.Text
                    };

                    DataProvider.TableDao.Insert(info, DataProvider.TableMetadataDao.GetDefaultTableMetadataInfoList(info.TableName));

                    Body.AddAdminLog("添加辅助表", $"辅助表:{TbTableName.Text}");

                    SuccessMessage("辅助表添加成功！");
                    LayerUtils.Close(Page);
                }
            }
        }

    }
}
