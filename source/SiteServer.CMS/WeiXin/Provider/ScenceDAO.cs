using BaiRong.Core;
using BaiRong.Core.Data;
using SiteServer.CMS.WeiXin.Model;

namespace SiteServer.CMS.WeiXin.Provider
{
    public class ScenceDAO : DataProviderBase
    {
        private const string TABLE_NAME = "wx_Scence";

        public ScenceInfo GetScenceInfo(int scenceID)
        {
            ScenceInfo scenceInfo = null;

            string SQL_WHERE = $"WHERE ID = {scenceID}";
            var SQL_SELECT = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TABLE_NAME, 0, SqlUtils.Asterisk, SQL_WHERE, null);

            using (var rdr = ExecuteReader(SQL_SELECT))
            {
                if (rdr.Read())
                {
                    scenceInfo = new ScenceInfo(rdr);
                }
                rdr.Close();
            }

            return scenceInfo;
        }

        // Mr.wu begin
        public void UpdateClickNum(int scenceID, int publishmentSystemID)
        {
            if (scenceID > 0)
            {
                string sqlString =
                    $"UPDATE {TABLE_NAME} set ClickNum= ClickNum+1 WHERE ID = {scenceID} AND publishmentSystemID = {publishmentSystemID}";
                ExecuteNonQuery(sqlString);
            }
        }
        // Mr.wu end
    }
}
