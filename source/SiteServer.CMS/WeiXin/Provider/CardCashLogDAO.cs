using System.Collections.Generic;
using System.Data;
using BaiRong.Core;
using BaiRong.Core.Data;
using SiteServer.CMS.WeiXin.Model;
using SiteServer.CMS.WeiXin.Model.Enumerations;

namespace SiteServer.CMS.WeiXin.Provider
{
    public class CardCashLogDAO : DataProviderBase
    {
        private const string TABLE_NAME = "wx_CardCashLog";
         
        public int Insert(CardCashLogInfo cardCashLogInfo)
        {
            var cardCashLogID = 0;

            IDataParameter[] parms = null;

            var SQL_INSERT = BaiRongDataProvider.TableStructureDao.GetInsertSqlString(cardCashLogInfo.ToNameValueCollection(), ConnectionString, TABLE_NAME, out parms);
             
            using (var conn = GetConnection())
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        ExecuteNonQuery(trans, SQL_INSERT, parms);

                         cardCashLogID = BaiRongDataProvider.DatabaseDao.GetSequence(trans, TABLE_NAME);

                        trans.Commit();
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }

            return cardCashLogID;
        }

        public void Update(CardCashLogInfo cardCashLogInfo)
        {
            IDataParameter[] parms = null;
            var SQL_UPDATE = BaiRongDataProvider.TableStructureDao.GetUpdateSqlString(cardCashLogInfo.ToNameValueCollection(), ConnectionString, TABLE_NAME, out parms);

            ExecuteNonQuery(SQL_UPDATE, parms);
        }
   
        public void Delete(int publishmentSystemID, int cardCashLogID)
        {
            if (cardCashLogID > 0)
            { 
                string sqlString = $"DELETE FROM {TABLE_NAME} WHERE ID = {cardCashLogID}";
                ExecuteNonQuery(sqlString);
            }
        }

        public void Delete(int publishmentSystemID, List<int> cardCashLogIDList)
        {
            if (cardCashLogIDList != null && cardCashLogIDList.Count > 0)
            {
                string sqlString =
                    $"DELETE FROM {TABLE_NAME} WHERE ID IN ({TranslateUtils.ToSqlInStringWithoutQuote(cardCashLogIDList)})";
                ExecuteNonQuery(sqlString);
            }
        }
         
        public CardCashLogInfo GetCardCashLogInfo(int  cardCashLogID)
        {
            CardCashLogInfo cardCashLogInfo = null;

            string SQL_WHERE = $"WHERE ID = {cardCashLogID}";
            var SQL_SELECT = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TABLE_NAME, 0, SqlUtils.Asterisk, SQL_WHERE, null);

            using (var rdr = ExecuteReader(SQL_SELECT))
            {
                if (rdr.Read())
                {
                    cardCashLogInfo = new CardCashLogInfo(rdr);
                }
                rdr.Close();
            }

            return cardCashLogInfo;
        }

        public List<CardCashLogInfo> GetCardCashLogInfoList(int cardID,int cardSNID,string userName,string startDate,string endDate)
        {
            var cardCashLogInfoList = new List<CardCashLogInfo>();

            string SQL_WHERE =
                $"WHERE CardID = {cardID} AND CardSNID={cardSNID} AND UserName='{PageUtils.FilterSql(userName)}'";
            if (!string.IsNullOrEmpty(startDate))
            {
                SQL_WHERE += $" AND AddDate >='{startDate}' AND AddDate < '{endDate}'";
            }
            string SQL_ORDER = $" ORDER BY {CardCashLogAttribute.AddDate} DESC";
            var SQL_SELECT = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TABLE_NAME, 0, SqlUtils.Asterisk, SQL_WHERE, SQL_ORDER);

            using (var rdr = ExecuteReader(SQL_SELECT))
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

        public List<CardCashYearCountInfo> GetCardCashYearCountInfoList(int cardID, int cardSNID,string userName)
        {
            var cardCashYearCountInfoList = new List<CardCashYearCountInfo>();

            string sqlString =
                $"select datepart(year,addDate)as 年份,sum( case CashType when 'Consume' then Amount else 0 end)as '消费',sum(case CashType when 'Recharge' then Amount else 0 end )as '充值' ,sum(case CashType when 'Exchange' then Amount else 0 end )as '积分兑换' from wx_CardCashLog  where CardID= {cardID} and CardSNID={cardSNID} and UserName = '{PageUtils.FilterSql(userName)}'  group by  datepart(year,addDate) order by datepart(year,addDate) desc ";

            using (var rdr = ExecuteReader(sqlString))
            {
                while (rdr.Read())
                {
                    var cardCashYearCountInfo = new CardCashYearCountInfo();
                    cardCashYearCountInfo.Year =ConvertHelper.GetString( rdr.GetValue(0));
                    cardCashYearCountInfo.TotalConsume = ConvertHelper.GetDecimal(rdr.GetValue(1));
                    cardCashYearCountInfo.TotalRecharge = ConvertHelper.GetDecimal(rdr.GetValue(2));
                    cardCashYearCountInfo.TotalExchange = ConvertHelper.GetDecimal(rdr.GetValue(3));
                    cardCashYearCountInfoList.Add(cardCashYearCountInfo);
                }
                rdr.Close();
            }

            return cardCashYearCountInfoList;
        }

        public List<CardCashMonthCountInfo> GetCardCashMonthCountInfoList(int cardID, int cardSNID, string userName,string year)
        {
            var cardCashMonthCountInfoList = new List<CardCashMonthCountInfo>();

            string sqlString =
                $"select datepart(year,addDate)as 年份, datepart(month,addDate)as 月份,sum( case CashType when 'Consume' then Amount else 0 end)as '消费',sum(case CashType when 'Recharge' then Amount else 0 end )as '充值' ,sum(case CashType when 'Exchange' then Amount else 0 end )as '积分兑换' from wx_CardCashLog where CardID= {cardID} and  CardSNID= {cardSNID} and  UserName='{PageUtils.FilterSql(userName)}' and AddDate like '%{year}%'  group by datepart(month,addDate), datepart(year,addDate) order by datepart(year,addDate) desc,datepart(month,addDate) desc ";

            using (var rdr = ExecuteReader(sqlString))
            {
                while (rdr.Read())
                {
                    var cardCashMonthCountInfo = new CardCashMonthCountInfo();
                    cardCashMonthCountInfo.Year = ConvertHelper.GetString(rdr.GetValue(0));
                    cardCashMonthCountInfo.Month = ConvertHelper.GetString(rdr.GetValue(1));
                    cardCashMonthCountInfo.TotalConsume = ConvertHelper.GetDecimal(rdr.GetValue(2));
                    cardCashMonthCountInfo.TotalRecharge = ConvertHelper.GetDecimal(rdr.GetValue(3));
                    cardCashMonthCountInfo.TotalExchange = ConvertHelper.GetDecimal(rdr.GetValue(4));
                    cardCashMonthCountInfoList.Add(cardCashMonthCountInfo);
                }
                rdr.Close();
            }

            return cardCashMonthCountInfoList;
        }

        public string GetSelectString(int publishmentSystemID, ECashType cashType, int cardID, string cardSN, string userName, string mobile)
        {
            string whereString =
                $"WHERE {CardCashLogAttribute.PublishmentSystemID} = {publishmentSystemID} AND {CardCashLogAttribute.CashType}='{cashType}'";
            if (cardID > 0)
            {
                whereString += $" AND {CardCashLogAttribute.CardID}={cardID}";
            }
            if (!string.IsNullOrEmpty(cardSN))
            {
                whereString += $" AND {CardCashLogAttribute.CardSNID} IN (SELECT ID FROM wx_CardSN WHERE SN='{cardSN}')";
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
            return BaiRongDataProvider.TableStructureDao.GetSelectSqlString(TABLE_NAME, SqlUtils.Asterisk, whereString);
        }

        public List<CardCashLogInfo> GetCardCashLogInfoList(int publishmentSystemID, int cardID, int cardSNID)
        {
            var cardCashLogInfoList = new List<CardCashLogInfo>();

            string SQL_WHERE =
                $"WHERE PublishmentSystemID={publishmentSystemID} AND CardID = {cardID} AND CardSNID = {cardSNID}";
            
            string SQL_ORDER = $" ORDER BY {CardCashLogAttribute.AddDate} DESC";
            var SQL_SELECT = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(ConnectionString, TABLE_NAME, 0, SqlUtils.Asterisk, SQL_WHERE, SQL_ORDER);

            using (var rdr = ExecuteReader(SQL_SELECT))
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
