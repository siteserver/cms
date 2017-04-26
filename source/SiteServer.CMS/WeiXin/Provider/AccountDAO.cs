using System.Collections.Generic;
using System.Data;
using BaiRong.Core;
using BaiRong.Core.Data;
using SiteServer.CMS.WeiXin.Model;

namespace SiteServer.CMS.WeiXin.Provider
{
    public class AccountDAO : DataProviderBase
    {
        private const string TABLE_NAME = "wx_Account";

        public int Insert(AccountInfo accountInfo)
        {
            var accountID = 0;

            IDataParameter[] parms = null;

            var SQL_INSERT = BaiRongDataProvider.TableStructureDao.GetInsertSqlString(accountInfo.ToNameValueCollection(), ConnectionString, TABLE_NAME, out parms);

            using (var conn = GetConnection())
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        ExecuteNonQuery(trans, SQL_INSERT, parms);

                        accountID = BaiRongDataProvider.DatabaseDao.GetSequence(trans, TABLE_NAME);

                        trans.Commit();
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }

            return accountID;
        }

        public void Update(AccountInfo accountInfo)
        {
            IDataParameter[] parms = null;
            var SQL_UPDATE = BaiRongDataProvider.TableStructureDao.GetUpdateSqlString(accountInfo.ToNameValueCollection(), ConnectionString, TABLE_NAME, out parms);

            ExecuteNonQuery(SQL_UPDATE, parms);
        }

        public AccountInfo GetAccountInfo(int publishmentSystemID)
        {
            AccountInfo accountInfo = null;

            string SQL_WHERE = $"WHERE {AccountAttribute.PublishmentSystemID} = {publishmentSystemID}";
            var SQL_SELECT = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TABLE_NAME, 0, SqlUtils.Asterisk, SQL_WHERE, null);

            using (var rdr = ExecuteReader(SQL_SELECT))
            {
                if (rdr.Read())
                {
                    accountInfo = new AccountInfo(rdr);
                }
                rdr.Close();
            }

            if (accountInfo == null)
            {
                accountInfo = new AccountInfo();
                accountInfo.PublishmentSystemID = publishmentSystemID;
                accountInfo.Token = StringUtils.GetShortGuid();
                accountInfo.ID = Insert(accountInfo);
            }

            return accountInfo;
        }

        public List<AccountInfo> GetAccountInfoList(int publishmentSystemID)
        {
            var accountInfoList = new List<AccountInfo>();

            string SQL_WHERE = $"WHERE {AccountAttribute.PublishmentSystemID} = {publishmentSystemID}";

            var SQL_SELECT = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TABLE_NAME, 0, SqlUtils.Asterisk, SQL_WHERE, null);

            using (var rdr = ExecuteReader(SQL_SELECT))
            {
                while (rdr.Read())
                {
                    var accountInfo = new AccountInfo(rdr);
                    accountInfoList.Add(accountInfo);
                }
                rdr.Close();
            }

            return accountInfoList;
        }
    }
}
