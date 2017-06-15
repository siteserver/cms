using System.Collections.Generic;
using System.Data;
using BaiRong.Core;
using BaiRong.Core.Data;
using SiteServer.CMS.WeiXin.Data;
using SiteServer.CMS.WeiXin.Model;

namespace SiteServer.CMS.WeiXin.Provider
{
    public class MessageDao : DataProviderBase
    {
        private const string TableName = "wx_Message";
         
        public int Insert(MessageInfo messageInfo)
        {
            var messageId = 0;

            IDataParameter[] parms = null;

            var sqlInsert = BaiRongDataProvider.TableStructureDao.GetInsertSqlString(messageInfo.ToNameValueCollection(), ConnectionString, TableName, out parms);

            using (var conn = GetConnection())
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        messageId = ExecuteNonQueryAndReturnId(trans, sqlInsert, parms);

                        trans.Commit();
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }

            return messageId;
        }

        public void Update(MessageInfo messageInfo)
        {
            IDataParameter[] parms = null;
            var sqlUpdate = BaiRongDataProvider.TableStructureDao.GetUpdateSqlString(messageInfo.ToNameValueCollection(), ConnectionString, TableName, out parms);

            ExecuteNonQuery(sqlUpdate, parms);
        }

        private List<int> GetMessageIdList(int publishmentSystemId)
        {
            var messageIdList = new List<int>();

            string sqlWhere = $"WHERE {MessageAttribute.PublishmentSystemId} = {publishmentSystemId}";
            var sqlSelect = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TableName, 0, MessageAttribute.Id, sqlWhere, null);

            using (var rdr = ExecuteReader(sqlSelect))
            {
                while (rdr.Read())
                {
                    messageIdList.Add(rdr.GetInt32(0));
                }
                rdr.Close();
            }

            return messageIdList;
        }

        public void UpdateUserCount(int publishmentSystemId, Dictionary<int, int> messageIdWithCount)
        {
            if (messageIdWithCount.Count == 0)
            {
                string sqlString =
                    $"UPDATE {TableName} SET {MessageAttribute.UserCount} = 0 WHERE {MessageAttribute.PublishmentSystemId} = {publishmentSystemId}";
                ExecuteNonQuery(sqlString);
            }
            else
            {
                var messageIdList = GetMessageIdList(publishmentSystemId);
                foreach (var messageId in messageIdList)
                {
                    if (messageIdWithCount.ContainsKey(messageId))
                    {
                        string sqlString =
                            $"UPDATE {TableName} SET {MessageAttribute.UserCount} = {messageIdWithCount[messageId]} WHERE ID = {messageId}";
                        ExecuteNonQuery(sqlString);
                    }
                    else
                    {
                        string sqlString =
                            $"UPDATE {TableName} SET {MessageAttribute.UserCount} = 0 WHERE ID = {messageId}";
                        ExecuteNonQuery(sqlString);
                    }
                }
            }
        }

        public void AddUserCount(int messageId)
        {
            if (messageId > 0)
            {
                string sqlString =
                    $"UPDATE {TableName} SET {MessageAttribute.UserCount} = {MessageAttribute.UserCount} + 1 WHERE ID = {messageId}";
                ExecuteNonQuery(sqlString);
            }
        }

        public void AddPvCount(int messageId)
        {
            if (messageId > 0)
            {
                string sqlString =
                    $"UPDATE {TableName} SET {MessageAttribute.PvCount} = {MessageAttribute.PvCount} + 1 WHERE ID = {messageId}";
                ExecuteNonQuery(sqlString);
            }
        }

        public void Delete(int messageId)
        {
            if (messageId > 0)
            {
                var messageIdList = new List<int>();
                messageIdList.Add(messageId);

                DataProviderWx.KeywordDao.Delete(GetKeywordIdList(messageIdList));

                DataProviderWx.MessageContentDao.DeleteAll(messageId);

                string sqlString = $"DELETE FROM {TableName} WHERE ID = {messageId}";
                ExecuteNonQuery(sqlString);
            }
        }

        public void Delete(List<int> messageIdList)
        {
            if (messageIdList != null  && messageIdList.Count > 0)
            {
                DataProviderWx.KeywordDao.Delete(GetKeywordIdList(messageIdList));

                foreach (var messageId in messageIdList)
                {
                    DataProviderWx.MessageContentDao.DeleteAll(messageId);
                }

                string sqlString =
                    $"DELETE FROM {TableName} WHERE ID IN ({TranslateUtils.ToSqlInStringWithoutQuote(messageIdList)})";
                ExecuteNonQuery(sqlString);
            }
        }

        private List<int> GetKeywordIdList(List<int> messageIdList)
        {
            var keywordIdList = new List<int>();

            if (messageIdList != null && messageIdList.Count > 0)
            {
                string sqlString =
                    $"SELECT {MessageAttribute.KeywordId} FROM {TableName} WHERE ID IN ({TranslateUtils.ToSqlInStringWithoutQuote(messageIdList)})";

                using (var rdr = ExecuteReader(sqlString))
                {
                    while (rdr.Read())
                    {
                        keywordIdList.Add(rdr.GetInt32(0));
                    }
                    rdr.Close();
                }
            }

            return keywordIdList;
        }

        public MessageInfo GetMessageInfo(int messageId)
        {
            MessageInfo messageInfo = null;

            string sqlWhere = $"WHERE ID = {messageId}";
            var sqlSelect = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TableName, 0, SqlUtils.Asterisk, sqlWhere, null);

            using (var rdr = ExecuteReader(sqlSelect))
            {
                if (rdr.Read())
                {
                    messageInfo = new MessageInfo(rdr);
                }
                rdr.Close();
            }

            return messageInfo;
        }

        public List<MessageInfo> GetMessageInfoListByKeywordId(int publishmentSystemId, int keywordId)
        {
            var messageInfoList = new List<MessageInfo>();

            string sqlWhere =
                $"WHERE {MessageAttribute.PublishmentSystemId} = {publishmentSystemId} AND {MessageAttribute.IsDisabled} <> '{true}'";
            if (keywordId > 0)
            {
                sqlWhere += $" AND {MessageAttribute.KeywordId} = {keywordId}";
            }

            var sqlSelect = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TableName, 0, SqlUtils.Asterisk, sqlWhere, null);

            using (var rdr = ExecuteReader(sqlSelect))
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

        public string GetTitle(int messageId)
        {
            var title = string.Empty;

            string sqlWhere = $"WHERE ID = {messageId}";
            var sqlSelect = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TableName, 0, MessageAttribute.Title, sqlWhere, null);

            using (var rdr = ExecuteReader(sqlSelect))
            {
                if (rdr.Read())
                {
                    title = rdr.GetValue(0).ToString();
                }
                rdr.Close();
            }

            return title;
        }

        public int GetFirstIdByKeywordId(int publishmentSystemId, int keywordId)
        {
            string sqlString =
                $"SELECT TOP 1 ID FROM {TableName} WHERE {MessageAttribute.PublishmentSystemId} = {publishmentSystemId} AND {MessageAttribute.IsDisabled} <> '{true}' AND {MessageAttribute.KeywordId} = {keywordId}";

            return BaiRongDataProvider.DatabaseDao.GetIntResult(sqlString);
        }

        public string GetSelectString(int publishmentSystemId)
        {
            string whereString = $"WHERE {MessageAttribute.PublishmentSystemId} = {publishmentSystemId}";
            return BaiRongDataProvider.TableStructureDao.GetSelectSqlString(TableName, SqlUtils.Asterisk, whereString);
        }

        public List<MessageInfo> GetMessageInfoList(int publishmentSystemId)
        {
            var messageInfoList = new List<MessageInfo>();

            string sqlWhere = $"WHERE {MessageAttribute.PublishmentSystemId} = {publishmentSystemId}";

            var sqlSelect = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TableName, 0, SqlUtils.Asterisk, sqlWhere, null);

            using (var rdr = ExecuteReader(sqlSelect))
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
