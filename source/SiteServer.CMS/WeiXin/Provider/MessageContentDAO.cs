using System.Collections.Generic;
using System.Data;
using BaiRong.Core;
using BaiRong.Core.Data;
using SiteServer.CMS.WeiXin.Data;
using SiteServer.CMS.WeiXin.Model;

namespace SiteServer.CMS.WeiXin.Provider
{
    public class MessageContentDAO : DataProviderBase
    {
        private const string TABLE_NAME = "wx_MessageContent";
        
        public int Insert(MessageContentInfo contentInfo)
        {
            var messageContentID = 0;

            IDataParameter[] parms = null;

            var SQL_INSERT = BaiRongDataProvider.TableStructureDao.GetInsertSqlString(contentInfo.ToNameValueCollection(), ConnectionString, TABLE_NAME, out parms);

            using (var conn = GetConnection())
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        ExecuteNonQuery(trans, SQL_INSERT, parms);

                        messageContentID = BaiRongDataProvider.DatabaseDao.GetSequence(trans, TABLE_NAME);

                        trans.Commit();
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }

            return messageContentID;
        }

        public void DeleteAll(int messageID)
        {
            if (messageID > 0)
            {
                string sqlString =
                    $"DELETE FROM {TABLE_NAME} WHERE {MessageContentAttribute.MessageID} = {messageID}";
                ExecuteNonQuery(sqlString);
            }
        }

        private void UpdateUserCount(int publishmentSystemID)
        {
            var messageIDWithCount = new Dictionary<int, int>();

            string sqlString =
                $"SELECT {MessageContentAttribute.MessageID}, COUNT(*) FROM {TABLE_NAME} WHERE {MessageContentAttribute.PublishmentSystemID} = {publishmentSystemID} GROUP BY {MessageContentAttribute.MessageID}";

            using (var rdr = ExecuteReader(sqlString))
            {
                while (rdr.Read())
                {
                    messageIDWithCount.Add(rdr.GetInt32(0), rdr.GetInt32(1));
                }
                rdr.Close();
            }

            DataProviderWX.MessageDAO.UpdateUserCount(publishmentSystemID, messageIDWithCount);

        }

        public void Delete(int publishmentSystemID, List<int> contentIDList)
        {
            if (contentIDList != null && contentIDList.Count > 0)
            {
                string sqlString =
                    $"DELETE FROM {TABLE_NAME} WHERE ID IN ({TranslateUtils.ToSqlInStringWithoutQuote(contentIDList)})";
                ExecuteNonQuery(sqlString);

                UpdateUserCount(publishmentSystemID);
            }
        }

        public bool AddLikeCount(int contentID, string cookieSN, string wxOpenID)
        {
            var isAdd = false;
            if (contentID > 0)
            {
                string sqlString =
                    $"SELECT {MessageContentAttribute.LikeCookieSNCollection} FROM {TABLE_NAME} WHERE ID = {contentID}";

                var cookieSNList = TranslateUtils.StringCollectionToStringList(BaiRongDataProvider.DatabaseDao.GetString(sqlString));
                if (!cookieSNList.Contains(cookieSN))
                {
                    cookieSNList.Add(cookieSN);

                    sqlString =
                        $"UPDATE {TABLE_NAME} SET {MessageContentAttribute.LikeCount} = {MessageContentAttribute.LikeCount} + 1, {MessageContentAttribute.LikeCookieSNCollection} = '{TranslateUtils.ToSqlInStringWithoutQuote(cookieSNList)}' WHERE ID = {contentID}";
                    ExecuteNonQuery(sqlString);

                    isAdd = true;
                }
            }
            return isAdd;
        }

        public void AddReplyCount(int contentID)
        {
            if (contentID > 0)
            {
                string sqlString =
                    $"UPDATE {TABLE_NAME} SET {MessageContentAttribute.ReplyCount} = {MessageContentAttribute.ReplyCount} + 1 WHERE ID = {contentID}";
                ExecuteNonQuery(sqlString);
            }
        }

        public int GetCount(int messageID, bool isReply)
        {
            string sqlString =
                $"SELECT COUNT(*) FROM {TABLE_NAME} WHERE {MessageContentAttribute.MessageID} = {messageID} AND {MessageContentAttribute.IsReply} = '{isReply}'";

            return BaiRongDataProvider.DatabaseDao.GetIntResult(sqlString);
        }

        public string GetSelectString(int publishmentSystemID, int messageID)
        {
            string whereString = $"WHERE {MessageContentAttribute.PublishmentSystemID} = {publishmentSystemID}";
            if (messageID > 0)
            {
                whereString += $" AND {MessageContentAttribute.MessageID} = {messageID}";
            }
            return BaiRongDataProvider.TableStructureDao.GetSelectSqlString(TABLE_NAME, SqlUtils.Asterisk, whereString);
        }

        public List<MessageContentInfo> GetContentInfoList(int messageID, int startNum, int totalNum)
        {
            var list = new List<MessageContentInfo>();

            string SQL_WHERE =
                $"WHERE {MessageContentAttribute.IsReply} = '{false}' AND {MessageContentAttribute.MessageID} = {messageID}";
            var SQL_SELECT = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TABLE_NAME, startNum, totalNum, SqlUtils.Asterisk, SQL_WHERE, "ORDER BY ID DESC");

            using (var rdr = ExecuteReader(SQL_SELECT))
            {
                while (rdr.Read())
                {
                    var itemInfo = new MessageContentInfo(rdr);
                    list.Add(itemInfo);
                }
                rdr.Close();
            }

            return list;
        }

        public List<MessageContentInfo> GetReplyContentInfoList(int messageID, int replyID)
        {
            var list = new List<MessageContentInfo>();

            string SQL_WHERE =
                $"WHERE {MessageContentAttribute.IsReply} = '{true}' AND {MessageContentAttribute.MessageID} = {messageID} AND {MessageContentAttribute.ReplyID} = {replyID}";
            var SQL_SELECT = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TABLE_NAME, 0, SqlUtils.Asterisk, SQL_WHERE, "ORDER BY ID DESC");

            using (var rdr = ExecuteReader(SQL_SELECT))
            {
                while (rdr.Read())
                {
                    var itemInfo = new MessageContentInfo(rdr);
                    list.Add(itemInfo);
                }
                rdr.Close();
            }

            return list;
        }
        public List<MessageContentInfo> GetMessageContentInfoList(int publishmentSystemID, int messageID)
        {
            var messageContentInfoList = new List<MessageContentInfo>();

            string SQL_WHERE =
                $"WHERE {MessageContentAttribute.PublishmentSystemID} = {publishmentSystemID} AND {MessageContentAttribute.MessageID} = {messageID} AND ReplyID = 0";

            var SQL_SELECT = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TABLE_NAME, 0, SqlUtils.Asterisk, SQL_WHERE, null);

            using (var rdr = ExecuteReader(SQL_SELECT))
            {
                while (rdr.Read())
                {
                    var messageContentInfo = new MessageContentInfo(rdr);
                    messageContentInfoList.Add(messageContentInfo);
                }
                rdr.Close();
            }

            return messageContentInfoList;
        }
    }
}
