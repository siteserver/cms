using System.Collections.Generic;
using System.Data;
using BaiRong.Core;
using BaiRong.Core.Data;
using SiteServer.CMS.WeiXin.Data;
using SiteServer.CMS.WeiXin.Model;

namespace SiteServer.CMS.WeiXin.Provider
{
    public class MessageContentDao : DataProviderBase
    {
        private const string TableName = "wx_MessageContent";
        
        public int Insert(MessageContentInfo contentInfo)
        {
            var messageContentId = 0;

            IDataParameter[] parms = null;

            var sqlInsert = BaiRongDataProvider.TableStructureDao.GetInsertSqlString(contentInfo.ToNameValueCollection(), ConnectionString, TableName, out parms);

            using (var conn = GetConnection())
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        messageContentId = ExecuteNonQueryAndReturnId(trans, sqlInsert, parms);

                        trans.Commit();
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }

            return messageContentId;
        }

        public void DeleteAll(int messageId)
        {
            if (messageId > 0)
            {
                string sqlString =
                    $"DELETE FROM {TableName} WHERE {MessageContentAttribute.MessageId} = {messageId}";
                ExecuteNonQuery(sqlString);
            }
        }

        private void UpdateUserCount(int publishmentSystemId)
        {
            var messageIdWithCount = new Dictionary<int, int>();

            string sqlString =
                $"SELECT {MessageContentAttribute.MessageId}, COUNT(*) FROM {TableName} WHERE {MessageContentAttribute.PublishmentSystemId} = {publishmentSystemId} GROUP BY {MessageContentAttribute.MessageId}";

            using (var rdr = ExecuteReader(sqlString))
            {
                while (rdr.Read())
                {
                    messageIdWithCount.Add(rdr.GetInt32(0), rdr.GetInt32(1));
                }
                rdr.Close();
            }

            DataProviderWx.MessageDao.UpdateUserCount(publishmentSystemId, messageIdWithCount);

        }

        public void Delete(int publishmentSystemId, List<int> contentIdList)
        {
            if (contentIdList != null && contentIdList.Count > 0)
            {
                string sqlString =
                    $"DELETE FROM {TableName} WHERE ID IN ({TranslateUtils.ToSqlInStringWithoutQuote(contentIdList)})";
                ExecuteNonQuery(sqlString);

                UpdateUserCount(publishmentSystemId);
            }
        }

        public bool AddLikeCount(int contentId, string cookieSn, string wxOpenId)
        {
            var isAdd = false;
            if (contentId > 0)
            {
                string sqlString =
                    $"SELECT {MessageContentAttribute.LikeCookieSnCollection} FROM {TableName} WHERE ID = {contentId}";

                var cookieSnList = TranslateUtils.StringCollectionToStringList(BaiRongDataProvider.DatabaseDao.GetString(sqlString));
                if (!cookieSnList.Contains(cookieSn))
                {
                    cookieSnList.Add(cookieSn);

                    sqlString =
                        $"UPDATE {TableName} SET {MessageContentAttribute.LikeCount} = {MessageContentAttribute.LikeCount} + 1, {MessageContentAttribute.LikeCookieSnCollection} = '{TranslateUtils.ToSqlInStringWithoutQuote(cookieSnList)}' WHERE ID = {contentId}";
                    ExecuteNonQuery(sqlString);

                    isAdd = true;
                }
            }
            return isAdd;
        }

        public void AddReplyCount(int contentId)
        {
            if (contentId > 0)
            {
                string sqlString =
                    $"UPDATE {TableName} SET {MessageContentAttribute.ReplyCount} = {MessageContentAttribute.ReplyCount} + 1 WHERE ID = {contentId}";
                ExecuteNonQuery(sqlString);
            }
        }

        public int GetCount(int messageId, bool isReply)
        {
            string sqlString =
                $"SELECT COUNT(*) FROM {TableName} WHERE {MessageContentAttribute.MessageId} = {messageId} AND {MessageContentAttribute.IsReply} = '{isReply}'";

            return BaiRongDataProvider.DatabaseDao.GetIntResult(sqlString);
        }

        public string GetSelectString(int publishmentSystemId, int messageId)
        {
            string whereString = $"WHERE {MessageContentAttribute.PublishmentSystemId} = {publishmentSystemId}";
            if (messageId > 0)
            {
                whereString += $" AND {MessageContentAttribute.MessageId} = {messageId}";
            }
            return BaiRongDataProvider.TableStructureDao.GetSelectSqlString(TableName, SqlUtils.Asterisk, whereString);
        }

        public List<MessageContentInfo> GetContentInfoList(int messageId, int startNum, int totalNum)
        {
            var list = new List<MessageContentInfo>();

            string sqlWhere =
                $"WHERE {MessageContentAttribute.IsReply} = '{false}' AND {MessageContentAttribute.MessageId} = {messageId}";
            var sqlSelect = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TableName, startNum, totalNum, SqlUtils.Asterisk, sqlWhere, "ORDER BY ID DESC");

            using (var rdr = ExecuteReader(sqlSelect))
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

        public List<MessageContentInfo> GetReplyContentInfoList(int messageId, int replyId)
        {
            var list = new List<MessageContentInfo>();

            string sqlWhere =
                $"WHERE {MessageContentAttribute.IsReply} = '{true}' AND {MessageContentAttribute.MessageId} = {messageId} AND {MessageContentAttribute.ReplyId} = {replyId}";
            var sqlSelect = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TableName, 0, SqlUtils.Asterisk, sqlWhere, "ORDER BY ID DESC");

            using (var rdr = ExecuteReader(sqlSelect))
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
        public List<MessageContentInfo> GetMessageContentInfoList(int publishmentSystemId, int messageId)
        {
            var messageContentInfoList = new List<MessageContentInfo>();

            string sqlWhere =
                $"WHERE {MessageContentAttribute.PublishmentSystemId} = {publishmentSystemId} AND {MessageContentAttribute.MessageId} = {messageId} AND ReplyID = 0";

            var sqlSelect = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TableName, 0, SqlUtils.Asterisk, sqlWhere, null);

            using (var rdr = ExecuteReader(sqlSelect))
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
