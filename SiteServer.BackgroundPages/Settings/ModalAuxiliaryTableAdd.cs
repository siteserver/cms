using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Model;

namespace SiteServer.BackgroundPages.Settings
{
    public class ModalAuxiliaryTableAdd : BasePageCms
    {
        public TextBox TbTableEnName;
        public TextBox TbTableCnName;
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

            VerifyAdministratorPermissions(AppManager.Permissions.Settings.Site);

            if (!string.IsNullOrEmpty(_tableName))
            {
                var info = BaiRongDataProvider.TableCollectionDao.GetTableCollectionInfo(_tableName);
                if (info != null)
                {
                    TbTableEnName.Text = info.TableEnName;
                    TbTableEnName.Enabled = false;
                    TbTableCnName.Text = info.TableCnName;
                    TbDescription.Text = info.Description;
                }
            }
        }

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            if (!Page.IsPostBack || !Page.IsValid) return;

            if (!string.IsNullOrEmpty(_tableName))
            {
                var info = BaiRongDataProvider.TableCollectionDao.GetTableCollectionInfo(_tableName);
                info.TableCnName = TbTableCnName.Text;
                info.Description = TbDescription.Text;

                BaiRongDataProvider.TableCollectionDao.Update(info);

                Body.AddAdminLog("修改辅助表", $"辅助表:{_tableName}");

                SuccessMessage("辅助表修改成功！");
                LayerUtils.Close(Page);
            }
            else
            {
                var tableEnNameList = BaiRongDataProvider.TableCollectionDao.GetTableEnNameList();
                if (tableEnNameList.IndexOf(TbTableEnName.Text) != -1)
                {
                    FailMessage("辅助表添加失败，辅助表标识已存在！");
                }
                else if (BaiRongDataProvider.DatabaseDao.IsTableExists(TbTableEnName.Text))
                {
                    FailMessage("辅助表添加失败，数据库中已存在此表！");
                }
                else
                {
                    var info = new TableCollectionInfo
                    {
                        TableEnName = TbTableEnName.Text,
                        TableCnName = TbTableCnName.Text,
                        Description = TbDescription.Text
                    };

                    BaiRongDataProvider.TableCollectionDao.Insert(info, BaiRongDataProvider.TableMetadataDao.GetDefaultTableMetadataInfoList(info.TableEnName));

                    Body.AddAdminLog("添加辅助表", $"辅助表:{TbTableEnName.Text}");

                    SuccessMessage("辅助表添加成功！");
                    LayerUtils.Close(Page);
                }
            }
        }

    }
}
