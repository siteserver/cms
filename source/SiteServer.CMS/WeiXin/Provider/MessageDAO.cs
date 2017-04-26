using System.Collections.Generic;
using System.Data;
using BaiRong.Core;
using BaiRong.Core.Data;
using SiteServer.CMS.WeiXin.Data;
using SiteServer.CMS.WeiXin.Model;

namespace SiteServer.CMS.WeiXin.Provider
{
    public class MessageDAO : DataProviderBase
    {
        private const string TABLE_NAME = "wx_Message";
         
        public int Insert(MessageInfo messageInfo)
        {
            var messageID = 0;

            IDataParameter[] parms = null;

            var SQL_INSERT = BaiRongDataProvider.TableStructureDao.GetInsertSqlString(messageInfo.ToNameValueCollection(), ConnectionString, TABLE_NAME, out parms);

            using (var conn = GetConnection())
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        ExecuteNonQuery(trans, SQL_INSERT, parms);

                        messageID = BaiRongDataProvider.DatabaseDao.GetSequence(trans, TABLE_NAME);

                        trans.Commit();
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }

            return messageID;
        }

        public void Update(MessageInfo messageInfo)
        {
            IDataParameter[] parms = null;
            var SQL_UPDATE = BaiRongDataProvider.TableStructureDao.GetUpdateSqlString(messageInfo.ToNameValueCollection(), ConnectionString, TABLE_NAME, out parms);

            ExecuteNonQuery(SQL_UPDATE, parms);
        }

        private List<int> GetMessageIDList(int publishmentSystemID)
        {
            var messageIDList = new List<int>();

            string SQL_WHERE = $"WHERE {MessageAttribute.PublishmentSystemID} = {publishmentSystemID}";
            var SQL_SELECT = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TABLE_NAME, 0, MessageAttribute.ID, SQL_WHERE, null);

            using (var rdr = ExecuteReader(SQL_SELECT))
            {
                while (rdr.Read())
                {
                    messageIDList.Add(rdr.GetInt32(0));
                }
                rdr.Close();
            }

            return messageIDList;
        }

        public void UpdateUserCount(int publishmentSystemID, Dictionary<int, int> messageIDWithCount)
        {
            if (messageIDWithCount.Count == 0)
            {
                string sqlString =
                    $"UPDATE {TABLE_NAME} SET {MessageAttribute.UserCount} = 0 WHERE {MessageAttribute.PublishmentSystemID} = {publishmentSystemID}";
                ExecuteNonQuery(sqlString);
            }
            else
            {
                var messageIDList = GetMessageIDList(publishmentSystemID);
                foreach (var messageID in messageIDList)
                {
                    if (messageIDWithCount.ContainsKey(messageID))
                    {
                        string sqlString =
                            $"UPDATE {TABLE_NAME} SET {MessageAttribute.UserCount} = {messageIDWithCount[messageID]} WHERE ID = {messageID}";
                        ExecuteNonQuery(sqlString);
                    }
                    else
                    {
                        string sqlString =
                            $"UPDATE {TABLE_NAME} SET {MessageAttribute.UserCount} = 0 WHERE ID = {messageID}";
                        ExecuteNonQuery(sqlString);
                    }
                }
            }
        }

        public void AddUserCount(int messageID)
        {
            if (messageID > 0)
            {
                string sqlString =
                    $"UPDATE {TABLE_NAME} SET {MessageAttribute.UserCount} = {MessageAttribute.UserCount} + 1 WHERE ID = {messageID}";
                ExecuteNonQuery(sqlString);
            }
        }

        public void AddPVCount(int messageID)
        {
            if (messageID > 0)
            {
                string sqlString =
                    $"UPDATE {TABLE_NAME} SET {MessageAttribute.PVCount} = {MessageAttribute.PVCount} + 1 WHERE ID = {messageID}";
                ExecuteNonQuery(sqlString);
            }
        }

        public void Delete(int messageID)
        {
            if (messageID > 0)
            {
                var messageIDList = new List<int>();
                messageIDList.Add(messageID);

                DataProviderWX.KeywordDAO.Delete(GetKeywordIDList(messageIDList));

                DataProviderWX.MessageContentDAO.DeleteAll(messageID);

                string sqlString = $"DELETE FROM {TABLE_NAME} WHERE ID = {messageID}";
                ExecuteNonQuery(sqlString);
            }
        }

        public void Delete(List<int> messageIDList)
        {
            if (messageIDList != null  && messageIDList.Count > 0)
            {
                DataProviderWX.KeywordDAO.Delete(GetKeywordIDList(messageIDList));

                foreach (var messageID in messageIDList)
                {
                    DataProviderWX.MessageContentDAO.DeleteAll(messageID);
                }

                string sqlString =
                    $"DELETE FROM {TABLE_NAME} WHERE ID IN ({TranslateUtils.ToSqlInStringWithoutQuote(messageIDList)})";
                ExecuteNonQuery(sqlString);
            }
        }

        private List<int> GetKeywordIDList(List<int> messageIDList)
        {
            var keywordIDList = new List<int>();

            if (messageIDList != null && messageIDList.Count > 0)
            {
                string sqlString =
                    $"SELECT {MessageAttribute.KeywordID} FROM {TABLE_NAME} WHERE ID IN ({TranslateUtils.ToSqlInStringWithoutQuote(messageIDList)})";

                using (var rdr = ExecuteReader(sqlString))
                {
                    while (rdr.Read())
                    {
                        keywordIDList.Add(rdr.GetInt32(0));
                    }
                    rdr.Close();
                }
            }

            return keywordIDList;
        }

        public MessageInfo GetMessageInfo(int messageID)
        {
            MessageInfo messageInfo = null;

            string SQL_WHERE = $"WHERE ID = {messageID}";
            var SQL_SELECT = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TABLE_NAME, 0, SqlUtils.Asterisk, SQL_WHERE, null);

            using (var rdr = ExecuteReader(SQL_SELECT))
            {
                if (rdr.Read())
                {
                    messageInfo = new MessageInfo(rdr);
                }
                rdr.Close();
            }

            return messageInfo;
        }

        public List<MessageInfo> GetMessageInfoListByKeywordID(int publishmentSystemID, int keywordID)
        {
            var messageInfoList = new List<MessageInfo>();

            string SQL_WHERE =
                $"WHERE {MessageAttribute.PublishmentSystemID} = {publishmentSystemID} AND {MessageAttribute.IsDisabled} <> '{true}'";
            if (keywordID > 0)
            {
                SQL_WHERE += $" AND {MessageAttribute.KeywordID} = {keywordID}";
            }

            var SQL_SELECT = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TABLE_NAME, 0, SqlUtils.Asterisk, SQL_WHERE, null);

            using (var rdr = ExecuteReader(SQL_SELECT))
            {
                while (rdr.Read())
                {
                    var messageInfo = new MessageInfo(rdr);
                    messageInfoList.Add(messageInfo);
                }
                rdr.Close();
            }

            return messageInfoList;
        }

        public string GetTitle(int messageID)
        {
            var title = string.Empty;

            string SQL_WHERE = $"WHERE ID = {messageID}";
            var SQL_SELECT = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TABLE_NAME, 0, MessageAttribute.Title, SQL_WHERE, null);

            using (var rdr = ExecuteReader(SQL_SELECT))
            {
                if (rdr.Read())
                {
                    title = rdr.GetValue(0).ToString();
                }
                rdr.Close();
            }

            return title;
        }

        public int GetFirstIDByKeywordID(int publishmentSystemID, int keywordID)
        {
            string sqlString =
                $"SELECT TOP 1 ID FROM {TABLE_NAME} WHERE {MessageAttribute.PublishmentSystemID} = {publishmentSystemID} AND {MessageAttribute.IsDisabled} <> '{true}' AND {MessageAttribute.KeywordID} = {keywordID}";

            return BaiRongDataProvider.DatabaseDao.GetIntResult(sqlString);
        }

        public string GetSelectString(int publishmentSystemID)
        {
            string whereString = $"WHERE {MessageAttribute.PublishmentSystemID} = {publishmentSystemID}";
            return BaiRongDataProvider.TableStructureDao.GetSelectSqlString(TABLE_NAME, SqlUtils.Asterisk, whereString);
        }

        public List<MessageInfo> GetMessageInfoList(int publishmentSystemID)
        {
            var messageInfoList = new List<MessageInfo>();

            string SQL_WHERE = $"WHERE {MessageAttribute.PublishmentSystemID} = {publishmentSystemID}";

            var SQL_SELECT = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TABLE_NAME, 0, SqlUtils.Asterisk, SQL_WHERE, null);

            using (var rdr = ExecuteReader(SQL_SELECT))
            {
                while (rdr.Read())
                {
                    var messageInfo = new MessageInfo(rdr);
                    messageInfoList.Add(messageInfo);
                }
                rdr.Close();
            }

            return messageInfoList;
        }
    }
}
