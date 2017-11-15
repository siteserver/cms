using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using BaiRong.Core;

namespace SiteServer.BackgroundPages
{
    public class PageTest : Page
    {
        public Literal LtlContent;

        public void Page_Load(object sender, EventArgs e)
        {
            LtlContent.Text = string.Empty;

            var sqlCreate = BaiRongDataProvider.DatabaseDao.GetCreateSystemTableSqlString(BaiRongDataProvider.ContentCheckDao.TableName, BaiRongDataProvider.ContentCheckDao.TableColumns);

            var sqlAlert = BaiRongDataProvider.DatabaseDao.GetAlterSystemTableSqlString(BaiRongDataProvider.ContentCheckDao.TableName, BaiRongDataProvider.ContentCheckDao.TableColumns);

            LtlContent.Text = sqlCreate + "<br /><hr /></br />" + string.Join("<br />", sqlAlert);
        }
    }
}
