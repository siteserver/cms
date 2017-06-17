using System.Collections.Generic;
using System.Data;
using BaiRong.Core;
using BaiRong.Core.Data;
using SiteServer.CMS.WeiXin.Model;
using SiteServer.CMS.WeiXin.Model.Enumerations;

namespace SiteServer.CMS.WeiXin.Provider
{
    public class CardCashLogDao : DataProviderBase
    {
        private const string TableName = "wx_CardCashLog";
         
        public int Insert(CardCashLogInfo cardCashLogInfo)
        {
            int cardCashLogId;

            IDataParameter[] parms;
            var sqlInsert = BaiRongDataProvider.TableStructureDao.GetInsertSqlString(cardCashLogInfo.ToNameValueCollection(), ConnectionString, TableName, out parms);
             
            using (var conn = GetConnection())
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                         cardCashLogId = ExecuteNonQueryAndReturnId(trans, sqlInsert, parms);

                        trans.Commit();
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }

            return cardCashLogId;
        }

        public void Update(CardCashLogInfo cardCashLogInfo)
        {
            IDataParameter[] parms;
            var sqlUpdate = BaiRongDataProvider.TableStructureDao.GetUpdateSqlString(cardCashLogInfo.ToNameValueCollection(), ConnectionString, TableName, out parms);

            ExecuteNonQuery(sqlUpdate, parms);
        }
   
        public void Delete(int publishmentSystemId, int cardCashLogId)
        {
            if (cardCashLogId > 0)
            { 
                string sqlString = $"DELETE FROM {TableName} WHERE ID = {cardCashLogId}";
                ExecuteNonQuery(sqlString);
            }
        }

        public void Delete(int publishmentSystemId, List<int> cardCashLogIdList)
        {
            if (cardCashLogIdList != null && cardCashLogIdList.Count > 0)
            {
                string sqlString =
                    $"DELETE FROM {TableName} WHERE ID IN ({TranslateUtils.ToSqlInStringWithoutQuote(cardCashLogIdList)})";
                ExecuteNonQuery(sqlString);
            }
        }
         
        public CardCashLogInfo GetCardCashLogInfo(int  cardCashLogId)
        {
            CardCashLogInfo cardCashLogInfo = null;

            string sqlWhere = $"WHERE ID = {cardCashLogId}";
            var sqlSelect = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TableName, 0, SqlUtils.Asterisk, sqlWhere, null);

            using (var rdr = ExecuteReader(sqlSelect))
            {
                if (rdr.Read())
                {
                    cardCashLogInfo = new CardCashLogInfo(rdr);
                }
                rdr.Close();
            }

            return cardCashLogInfo;
        }

        public List<CardCashLogInfo> GetCardCashLogInfoList(int cardId,int cardSnid,string userName,string startDate,string endDate)
        {
            var cardCashLogInfoList = new List<CardCashLogInfo>();

            string sqlWhere =
                $"WHERE CardID = {cardId} AND CardSNID={cardSnid} AND UserName='{PageUtils.FilterSql(userName)}'";
            if (!string.IsNullOrEmpty(startDate))
            {
                sqlWhere += $" AND AddDate >='{startDate}' AND AddDate < '{endDate}'";
            }
            string sqlOrder = $" ORDER BY {CardCashLogAttribute.AddDate} DESC";
            var sqlSelect = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TableName, 0, SqlUtils.Asterisk, sqlWhere, sqlOrder);

            using (var rdr = ExecuteReader(sqlSelect))
            {
                while (rdr.Read())
                {
                    var cardCashLogInfo = new CardCashLogInfo(rdr);
                    cardCashLogInfoList.Add(cardCashLogInfo);
                }
                rdr.Close();
            }

            return cardCashLogInfoList;
        }

        public List<CardCashYearCountInfo> GetCardCashYearCountInfoList(int cardId, int cardSnid,string userName)
        {
            var cardCashYearCountInfoList = new List<CardCashYearCountInfo>();

            string sqlString =
                $"select datepart(year,addDate)as 年份,sum( case CashType when 'Consume' then Amount else 0 end)as '消费',sum(case CashType when 'Recharge' then Amount else 0 end )as '充值' ,sum(case CashType when 'Exchange' then Amount else 0 end )as '积分兑换' from wx_CardCashLog  where CardID= {cardId} and CardSNID={cardSnid} and UserName = '{PageUtils.FilterSql(userName)}'  group by  datepart(year,addDate) order by datepart(year,addDate) desc ";

            using (var rdr = ExecuteReader(sqlString))
            {
                while (rdr.Read())
                {
                    var cardCashYearCountInfo = new CardCashYearCountInfo
                    {
                        Year = ConvertHelper.GetString(rdr.GetValue(0)),
                        TotalConsume = ConvertHelper.GetDecimal(rdr.GetValue(1)),
                        TotalRecharge = ConvertHelper.GetDecimal(rdr.GetValue(2)),
                        TotalExchange = ConvertHelper.GetDecimal(rdr.GetValue(3))
                    };
                    cardCashYearCountInfoList.Add(cardCashYearCountInfo);
                }
                rdr.Close();
            }

            return cardCashYearCountInfoList;
        }

        public List<CardCashMonthCountInfo> GetCardCashMonthCountInfoList(int cardId, int cardSnid, string userName,string year)
        {
            var cardCashMonthCountInfoList = new List<CardCashMonthCountInfo>();

            string sqlString =
                $"select datepart(year,addDate)as 年份, datepart(month,addDate)as 月份,sum( case CashType when 'Consume' then Amount else 0 end)as '消费',sum(case CashType when 'Recharge' then Amount else 0 end )as '充值' ,sum(case CashType when 'Exchange' then Amount else 0 end )as '积分兑换' from wx_CardCashLog where CardID= {cardId} and  CardSNID= {cardSnid} and  UserName='{PageUtils.FilterSql(userName)}' and AddDate like '%{year}%'  group by datepart(month,addDate), datepart(year,addDate) order by datepart(year,addDate) desc,datepart(month,addDate) desc ";

            using (var rdr = ExecuteReader(sqlString))
            {
                while (rdr.Read())
                {
                    var cardCashMonthCountInfo = new CardCashMonthCountInfo
                    {
                        Year = ConvertHelper.GetString(rdr.GetValue(0)),
                        Month = ConvertHelper.GetString(rdr.GetValue(1)),
                        TotalConsume = ConvertHelper.GetDecimal(rdr.GetValue(2)),
                        TotalRecharge = ConvertHelper.GetDecimal(rdr.GetValue(3)),
                        TotalExchange = ConvertHelper.GetDecimal(rdr.GetValue(4))
                    };
                    cardCashMonthCountInfoList.Add(cardCashMonthCountInfo);
                }
                rdr.Close();
            }

            return cardCashMonthCountInfoList;
        }

        public string GetSelectString(int publishmentSystemId, ECashType cashType, int cardId, string cardSn, string userName, string mobile)
        {
            string whereString =
                $"WHERE {CardCashLogAttribute.PublishmentSystemId} = {publishmentSystemId} AND {CardCashLogAttribute.CashType}='{cashType}'";
            if (cardId > 0)
            {
                whereString += $" AND {CardCashLogAttribute.CardId}={cardId}";
            }
            if (!string.IsNullOrEmpty(cardSn))
            {
                whereString += $" AND {CardCashLogAttribute.CardSnId} IN (SELECT ID FROM wx_CardSN WHERE SN='{cardSn}')";
            }
            if (!string.IsNullOrEmpty(userName))
            {
                whereString += $" AND {CardCashLogAttribute.UserName}='{PageUtils.FilterSql(userName)}'";
            }
            if (!string.IsNullOrEmpty(mobile))
            {
                whereString +=
                    $" AND {CardCashLogAttribute.UserName} IN (SELECT UserName FROM bairong_Users WHERE Mobile='{PageUtils.FilterSql(mobile)}')";
            }
            return BaiRongDataProvider.TableStructureDao.GetSelectSqlString(TableName, SqlUtils.Asterisk, whereString);
        }

        public List<CardCashLogInfo> GetCardCashLogInfoList(int publishmentSystemId, int cardId, int cardSnid)
        {
            var cardCashLogInfoList = new List<CardCashLogInfo>();

            string sqlWhere =
                $"WHERE PublishmentSystemID={publishmentSystemId} AND CardID = {cardId} AND CardSNID = {cardSnid}";
            
            string sqlOrder = $" ORDER BY {CardCashLogAttribute.AddDate} DESC";
            var sqlSelect = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TableName, 0, SqlUtils.Asterisk, sqlWhere, sqlOrder);

            using (var rdr = ExecuteReader(sqlSelect))
            {
                while (rdr.Read())
                {
                    var cardCashLogInfo = new CardCashLogInfo(rdr);
                    cardCashLogInfoList.Add(cardCashLogInfo);
                }
                rdr.Close();
            }

            return cardCashLogInfoList;
        }
    }
}
