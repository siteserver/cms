using System;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Model;
using BaiRong.Core.Model.Enumerations;
using BaiRong.Core.Text;

namespace SiteServer.BackgroundPages.Sys
{
    public class PageAuxiliaryTableAdd : BasePageCms
    {
        public Literal ltlPageTitle;

        public TextBox TableENName;
        public TextBox TableCNName;
        public TextBox Description;
        public RadioButtonList AuxiliaryTableType;

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            var enName = Body.GetQueryString("ENName");

            if (!IsPostBack)
            {
                var pageTitle = !string.IsNullOrEmpty(enName) ? "编辑辅助表" : "添加辅助表";
                BreadCrumbSys(AppManager.Sys.LeftMenu.Auxiliary, pageTitle, AppManager.Sys.Permission.SysAuxiliary);

                ltlPageTitle.Text = pageTitle;

                //cms
                AuxiliaryTableType.Items.Add(EAuxiliaryTableTypeUtils.GetListItem(EAuxiliaryTableType.BackgroundContent, true));

                if (AppManager.IsWcm())
                {
                    //wcm
                    AuxiliaryTableType.Items.Add(EAuxiliaryTableTypeUtils.GetListItem(EAuxiliaryTableType.GovPublicContent, false));
                    AuxiliaryTableType.Items.Add(EAuxiliaryTableTypeUtils.GetListItem(EAuxiliaryTableType.GovInteractContent, false));
                }
                //others
                AuxiliaryTableType.Items.Add(EAuxiliaryTableTypeUtils.GetListItem(EAuxiliaryTableType.VoteContent, false));
                AuxiliaryTableType.Items.Add(EAuxiliaryTableTypeUtils.GetListItem(EAuxiliaryTableType.JobContent, false));
                AuxiliaryTableType.Items.Add(EAuxiliaryTableTypeUtils.GetListItem(EAuxiliaryTableType.UserDefined, false));

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
                    var TableENNameList = BaiRongDataProvider.TableCollectionDao.GetTableEnNameList();
                    if (TableENNameList.IndexOf(TableENName.Text) != -1)
                    {
                        FailMessage("辅助表添加失败，辅助表标识已存在！");
                    }
                    else if (BaiRongDataProvider.TableStructureDao.IsTableExists(TableENName.Text))
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
