using System;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Model;
using BaiRong.Core.Model.Enumerations;

namespace SiteServer.BackgroundPages.Settings
{
    public class PageAuxiliaryTableAdd : BasePageCms
    {
        public Literal ltlPageTitle;

        public TextBox TableENName;
        public TextBox TableCNName;
        public TextBox Description;
        public RadioButtonList AuxiliaryTableType;

        public static string GetRedirectUrl()
        {
            return PageUtils.GetSettingsUrl(nameof(PageAuxiliaryTableAdd), null);
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            var enName = Body.GetQueryString("ENName");

            if (!IsPostBack)
            {
                var pageTitle = !string.IsNullOrEmpty(enName) ? "编辑辅助表" : "添加辅助表";
                BreadCrumbSettings(pageTitle, AppManager.Permissions.Settings.SiteManagement);

                ltlPageTitle.Text = pageTitle;

                //cms
                AuxiliaryTableType.Items.Add(EAuxiliaryTableTypeUtils.GetListItem(EAuxiliaryTableType.BackgroundContent, true));

                //others
                AuxiliaryTableType.Items.Add(EAuxiliaryTableTypeUtils.GetListItem(EAuxiliaryTableType.Custom, false));

                if (!string.IsNullOrEmpty(enName))
                {
                    var info = BaiRongDataProvider.TableCollectionDao.GetAuxiliaryTableInfo(enName);
                    if (info != null)
                    {
                        TableENName.Text = info.TableEnName;
                        TableENName.Enabled = false;
                        TableCNName.Text = info.TableCnName;
                        Description.Text = info.Description;

                        ControlUtils.SelectListItems(AuxiliaryTableType, EAuxiliaryTableTypeUtils.GetValue(info.AuxiliaryTableType));

                        AuxiliaryTableType.Enabled = false;
                    }
                }
            }
        }

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            if (Page.IsPostBack && Page.IsValid)
            {
                if (Body.IsQueryExists("ENName"))
                {
                    var tableEnName = Body.GetQueryString("ENName");
                    var info = BaiRongDataProvider.TableCollectionDao.GetAuxiliaryTableInfo(tableEnName);
                    info.TableCnName = TableCNName.Text;
                    info.Description = Description.Text;
                    try
                    {
                        BaiRongDataProvider.TableCollectionDao.Update(info);

                        Body.AddAdminLog("修改辅助表", $"辅助表:{tableEnName}");

                        SuccessMessage("辅助表修改成功！");
                    }
                    catch (Exception ex)
                    {
                        FailMessage(ex, "辅助表修改失败！");
                    }
                }
                else
                {
                    var tableEnNameList = BaiRongDataProvider.TableCollectionDao.GetTableEnNameList();
                    if (tableEnNameList.IndexOf(TableENName.Text) != -1)
                    {
                        FailMessage("辅助表添加失败，辅助表标识已存在！");
                    }
                    else if (BaiRongDataProvider.DatabaseDao.IsTableExists(TableENName.Text))
                    {
                        FailMessage("辅助表添加失败，数据库中已存在此表！");
                    }
                    else
                    {
                        var info = new AuxiliaryTableInfo();
                        info.TableEnName = TableENName.Text;
                        info.TableCnName = TableCNName.Text;
                        info.Description = Description.Text;
                        info.AuxiliaryTableType = EAuxiliaryTableTypeUtils.GetEnumType(AuxiliaryTableType.SelectedValue);
                        try
                        {
                            BaiRongDataProvider.TableCollectionDao.Insert(info);

                            Body.AddAdminLog("添加辅助表",
                                $"辅助表:{TableENName.Text}");

                            SuccessMessage("辅助表添加成功！");
                        }
                        catch (Exception ex)
                        {
                            FailMessage(ex, "辅助表添加失败！");
                        }
                    }
                }

            }
        }

    }
}
