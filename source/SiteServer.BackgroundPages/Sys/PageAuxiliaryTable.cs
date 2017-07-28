using System;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Model.Enumerations;
using BaiRong.Core.Text;

namespace SiteServer.BackgroundPages.Sys
{
	public class PageAuxiliaryTable : BasePageCms
    {
		public DataGrid dgContents;

	    public static string GetRedirectUrl()
	    {
	        return PageUtils.GetSysUrl(nameof(PageAuxiliaryTable), null);
	    }

		public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

			if (Body.IsQueryExists("Delete"))
			{
                var enName = Body.GetQueryString("ENName");//辅助表
                var enNameArchive = enName + "_Archive";//辅助表归档
			
				try
				{
                    BaiRongDataProvider.TableCollectionDao.Delete(enName);//删除辅助表
                    BaiRongDataProvider.TableCollectionDao.Delete(enNameArchive);//删除辅助表归档

                    Body.AddAdminLog("删除辅助表", $"辅助表:{enName}");

					SuccessDeleteMessage();
				}
				catch(Exception ex)
				{
                    FailDeleteMessage(ex);
				}
			}
			if (!IsPostBack)
            {
                BreadCrumbSys(AppManager.Sys.LeftMenu.Auxiliary, "辅助表管理", AppManager.Sys.Permission.SysAuxiliary);

                try
                {
                    dgContents.DataSource = BaiRongDataProvider.TableCollectionDao.GetDataSourceByAuxiliaryTableType();
                    dgContents.DataBind();
                }
                catch (Exception ex)
                {
                    PageUtils.RedirectToErrorPage(ex.Message);
                }
			}
		}

        public string GetYesOrNo(string isDefaultStr)
        {
            return StringUtils.GetBoolText(TranslateUtils.ToBool(isDefaultStr));
        }

        public int GetTableUsedNum(string tableENName, string auxiliaryTableType)
        {
            var tableType = EAuxiliaryTableTypeUtils.GetEnumType(auxiliaryTableType);
            var usedNum = BaiRongDataProvider.TableCollectionDao.GetTableUsedNum(tableENName, tableType);
            return usedNum;
        }

        public string GetAuxiliatyTableType(string auxiliaryTableType)
        {
            return EAuxiliaryTableTypeUtils.GetText(EAuxiliaryTableTypeUtils.GetEnumType(auxiliaryTableType));
        }

        public string GetIsChangedAfterCreatedInDB(string isCreatedInDB, string isChangedAfterCreatedInDB)
        {
            if (TranslateUtils.ToBool(isCreatedInDB) == false)
            {
                return "----";
            }
            else
            {
                return StringUtils.GetBoolText(TranslateUtils.ToBool(isChangedAfterCreatedInDB));
            }
        }

        public string GetFontColor(string isCreatedInDB, string isChangedAfterCreatedInDB)
        {
            if (EBooleanUtils.Equals(EBoolean.False, isCreatedInDB))
            {
                return string.Empty;
            }
            else
            {
                if (EBooleanUtils.Equals(EBoolean.False, isChangedAfterCreatedInDB))
                {
                    return string.Empty;
                }
                else
                {
                    return "red";
                }
            }
        }
	}
}
