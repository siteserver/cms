using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using BaiRong.Core;
using BaiRong.Core.Data;
using BaiRong.Core.Model.Enumerations;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;
using SiteServer.CMS.Model.Enumerations;

namespace SiteServer.CMS.Provider
{
    public class TrackingDao : DataProviderBase
    {
        protected const string ParmTrackingId = "@TrackingID";
        protected const string ParmPublishmentSystemId = "@PublishmentSystemID";
        protected const string ParmTrackerType = "@TrackerType";
        protected const string ParmLastAccessDateTime = "@LastAccessDateTime";
        protected const string ParmPageUrl = "@PageUrl";
        protected const string ParmPageNodeId = "@PageNodeID";
        protected const string ParmPageContentId = "@PageContentID";
        protected const string ParmReferrer = "@Referrer";
        protected const string ParmIpAddress = "@IPAddress";
        protected const string ParmOperatingSystem = "@OperatingSystem";
        protected const string ParmBrowser = "@Browser";
        protected const string ParmAccessDateTime = "@AccessDateTime";

        //与数据库字段名无关
        protected const string ParmConstTrackingCurrentMinute = "@TrackingCurrentMinute";

        public void Insert(TrackingInfo trackingInfo)
        {
            var sqlString = "INSERT INTO siteserver_Tracking (PublishmentSystemID, TrackerType, LastAccessDateTime, PageUrl, PageNodeID, PageContentID, Referrer, IPAddress, OperatingSystem, Browser, AccessDateTime) VALUES (@PublishmentSystemID, @TrackerType, @LastAccessDateTime, @PageUrl, @PageNodeID, @PageContentID, @Referrer, @IPAddress, @OperatingSystem, @Browser, @AccessDateTime)";

            var insertParms = new IDataParameter[]
			{
				GetParameter(ParmPublishmentSystemId, EDataType.Integer, trackingInfo.PublishmentSystemId),
				GetParameter(ParmTrackerType, EDataType.VarChar, 50, ETrackerTypeUtils.GetValue(trackingInfo.TrackerType)),
				GetParameter(ParmLastAccessDateTime, EDataType.DateTime, trackingInfo.LastAccessDateTime),
				GetParameter(ParmPageUrl, EDataType.VarChar, 200, trackingInfo.PageUrl),
                GetParameter(ParmPageNodeId, EDataType.Integer, trackingInfo.PageNodeId),
                GetParameter(ParmPageContentId, EDataType.Integer, trackingInfo.PageContentId),
				GetParameter(ParmReferrer, EDataType.VarChar, 200, trackingInfo.Referrer),
				GetParameter(ParmIpAddress, EDataType.VarChar, 200, trackingInfo.IpAddress),
				GetParameter(ParmOperatingSystem, EDataType.VarChar, 200, trackingInfo.OperatingSystem),
				GetParameter(ParmBrowser, EDataType.VarChar, 200, trackingInfo.Browser),
				GetParameter(ParmAccessDateTime, EDataType.DateTime, trackingInfo.AccessDateTime)
			};

            ExecuteNonQuery(sqlString, insertParms);
        }

        public virtual DataSet GetDataSource(int publishmentSystemId, int trackingCurrentMinute)
        {
            string sqlSelectTrackerAnalysis = $@"
SELECT TrackingID, PublishmentSystemID, TrackerType, LastAccessDateTime, PageUrl, PageNodeID, PageContentID, Referrer, IPAddress, OperatingSystem, Browser, AccessDateTime
FROM siteserver_Tracking
WHERE (PublishmentSystemID = @PublishmentSystemID) AND (AccessDateTime BETWEEN '{DateUtils.GetDateAndTimeString(DateTime.Now.AddMinutes(-trackingCurrentMinute))}' AND {SqlUtils.GetDefaultDateString()}) ORDER BY AccessDateTime DESC";

            var parms = new IDataParameter[]
			{
				GetParameter(ParmPublishmentSystemId, EDataType.Integer, publishmentSystemId)
			};

            return ExecuteDataset(sqlSelectTrackerAnalysis, parms);
        }

        public virtual int GetCurrentVisitorNum(int publishmentSystemId, int trackingCurrentMinute)
        {
            var currentVisitorNum = 0;

            string sqlSelectCurrentVisitorNum = $@"
SELECT COUNT(*) AS visitorNum
FROM siteserver_Tracking
WHERE (TrackerType = @TrackerType) AND
(PublishmentSystemID = @PublishmentSystemID) AND ({SqlUtils.GetDateDiffGreatThanMinutes("AccessDateTime", "@TrackingCurrentMinute")})";//当前在线人数

            var parms = new IDataParameter[]
			{
                GetParameter(ParmTrackerType, EDataType.VarChar, 50, ETrackerTypeUtils.GetValue(ETrackerType.Site)),
				GetParameter(ParmPublishmentSystemId, EDataType.Integer, publishmentSystemId),
				GetParameter(ParmConstTrackingCurrentMinute, EDataType.Integer, trackingCurrentMinute)
			};

            using (var rdr = ExecuteReader(sqlSelectCurrentVisitorNum, parms))
            {
                if (rdr.Read())
                {
                    currentVisitorNum = GetInt(rdr, 0);
                }
                rdr.Close();
            }
            return currentVisitorNum;
        }

        //总访问量
        public virtual int GetTotalAccessNum(int publishmentSystemId, DateTime sinceDate)
        {
            var totalAccessNum = 0;

            string sqlString;
            if (sinceDate == DateUtils.SqlMinValue)
            {
                sqlString =
                    $"SELECT COUNT(*) AS Num FROM siteserver_Tracking WHERE (PublishmentSystemID = {publishmentSystemId})";
            }
            else
            {
                sqlString =
                    $"SELECT COUNT(*) AS Num FROM siteserver_Tracking WHERE (PublishmentSystemID = {publishmentSystemId}) AND (AccessDateTime BETWEEN '{sinceDate.ToShortDateString()}' AND '{DateTime.Now.ToShortDateString()}')";
            }

            using (var rdr = ExecuteReader(sqlString))
            {
                if (rdr.Read())
                {
                    totalAccessNum = GetInt(rdr, 0);
                }
                rdr.Close();
            }
            return totalAccessNum;
        }

        //获取特定页面的总访问量
        public virtual int GetTotalUniqueAccessNumByPageInfo(int publishmentSystemId, int nodeId, int contentId, DateTime sinceDate)
        {
            var totalUniqueAccessNum = 0;

            string sqlString;
            if (sinceDate == DateUtils.SqlMinValue)
            {
                sqlString =
                    $"SELECT COUNT(*) AS Num FROM siteserver_Tracking WHERE (PublishmentSystemID = {publishmentSystemId}) AND (TrackerType = '{ETrackerTypeUtils.GetValue(ETrackerType.Site)}') AND (PageNodeID = {nodeId}) AND (PageContentID = {contentId})";
            }
            else
            {
                sqlString =
                    $"SELECT COUNT(*) AS Num FROM siteserver_Tracking WHERE (PublishmentSystemID = {publishmentSystemId}) AND (TrackerType = '{ETrackerTypeUtils.GetValue(ETrackerType.Site)}') AND (PageNodeID = {nodeId}) AND (PageContentID = {contentId}) AND (AccessDateTime BETWEEN '{sinceDate.ToShortDateString()}' AND '{DateTime.Now.ToShortDateString()}')";
            }

            using (var rdr = ExecuteReader(sqlString))
            {
                if (rdr.Read())
                {
                    totalUniqueAccessNum = GetInt(rdr, 0);
                }
                rdr.Close();
            }
            return totalUniqueAccessNum;
        }

        //获取特定页面的总访问量
        public virtual int GetTotalUniqueAccessNumByPageUrl(int publishmentSystemId, string pageUrl, DateTime sinceDate)
        {
            var totalUniqueAccessNum = 0;

            string sqlString;
            if (sinceDate == DateUtils.SqlMinValue)
            {
                sqlString =
                    $"SELECT COUNT(*) AS Num FROM siteserver_Tracking WHERE (PublishmentSystemID = {publishmentSystemId}) AND (TrackerType = '{ETrackerTypeUtils.GetValue(ETrackerType.Site)}') AND (PageUrl = '{pageUrl}')";
            }
            else
            {
                sqlString =
                    $"SELECT COUNT(*) AS Num FROM siteserver_Tracking WHERE (PublishmentSystemID = {publishmentSystemId}) AND (TrackerType = '{ETrackerTypeUtils.GetValue(ETrackerType.Site)}') AND (PageUrl = '{pageUrl}') AND (AccessDateTime BETWEEN '{sinceDate.ToShortDateString()}' AND '{DateTime.Now.ToShortDateString()}')";
            }

            using (var rdr = ExecuteReader(sqlString))
            {
                if (rdr.Read())
                {
                    totalUniqueAccessNum = GetInt(rdr, 0);
                }
                rdr.Close();
            }
            return totalUniqueAccessNum;
        }

        //总唯一访问量
        public virtual int GetTotalUniqueAccessNum(int publishmentSystemId, DateTime sinceDate)
        {
            var totalUniqueAccessNum = 0;

            string sqlString;
            if (sinceDate == DateUtils.SqlMinValue)
            {
                sqlString =
                    $"SELECT COUNT(*) AS Num FROM siteserver_Tracking WHERE (PublishmentSystemID = {publishmentSystemId}) AND (TrackerType = '{ETrackerTypeUtils.GetValue(ETrackerType.Site)}')";
            }
            else
            {
                sqlString =
                    $"SELECT COUNT(*) AS Num FROM siteserver_Tracking WHERE (PublishmentSystemID = {publishmentSystemId}) AND (TrackerType = '{ETrackerTypeUtils.GetValue(ETrackerType.Site)}') AND (AccessDateTime BETWEEN '{sinceDate.ToShortDateString()}' AND '{DateTime.Now.ToShortDateString()}')";
            }

            using (var rdr = ExecuteReader(sqlString))
            {
                if (rdr.Read())
                {
                    totalUniqueAccessNum = GetInt(rdr, 0);
                }
                rdr.Close();
            }
            return totalUniqueAccessNum;
        }

        //获取特定页面的总访问量
        public virtual int GetTotalAccessNumByPageUrl(int publishmentSystemId, string pageUrl, DateTime sinceDate)
        {
            var totalAccessNum = 0;

            string sqlString;
            if (sinceDate == DateUtils.SqlMinValue)
            {
                sqlString =
                    $"SELECT COUNT(*) AS Num FROM siteserver_Tracking WHERE (PublishmentSystemID = {publishmentSystemId}) AND (PageUrl = '{pageUrl}')";
            }
            else
            {
                sqlString =
                    $"SELECT COUNT(*) AS Num FROM siteserver_Tracking WHERE (PublishmentSystemID = {publishmentSystemId}) AND (PageUrl = '{pageUrl}') AND (AccessDateTime BETWEEN '{sinceDate.ToShortDateString()}' AND '{DateTime.Now.ToShortDateString()}')";
            }

            using (var rdr = ExecuteReader(sqlString))
            {
                if (rdr.Read())
                {
                    totalAccessNum = GetInt(rdr, 0);
                }
                rdr.Close();
            }
            return totalAccessNum;
        }

        //获取特定页面的总访问量
        public virtual int GetTotalAccessNumByPageInfo(int publishmentSystemId, int nodeId, int contentId, DateTime sinceDate)
        {
            var totalAccessNum = 0;

            string sqlString;
            if (sinceDate == DateUtils.SqlMinValue)
            {
                sqlString =
                    $"SELECT COUNT(*) AS Num FROM siteserver_Tracking WHERE PublishmentSystemID = {publishmentSystemId} AND PageNodeID = {nodeId} AND PageContentID = {contentId} AND TrackerType = '{ETrackerTypeUtils.GetValue(ETrackerType.Page)}'";
            }
            else
            {
                sqlString =
                    $"SELECT COUNT(*) AS Num FROM siteserver_Tracking WHERE PublishmentSystemID = {publishmentSystemId} AND PageNodeID = {nodeId} AND PageContentID = {contentId} AND TrackerType = '{ETrackerTypeUtils.GetValue(ETrackerType.Page)}' AND (AccessDateTime BETWEEN '{sinceDate.ToShortDateString()}' AND '{DateTime.Now.ToShortDateString()}')";
            }

            using (var rdr = ExecuteReader(sqlString))
            {
                if (rdr.Read())
                {
                    totalAccessNum = GetInt(rdr, 0);
                }
                rdr.Close();
            }
            return totalAccessNum;
        }

        public virtual int GetMaxAccessNumOfDay(int publishmentSystemId, out string maxAccessDay)
        {
            var maxAccessNumOfDay = 0;
            maxAccessDay = string.Empty;

            var sqlSelectMaxAccessNumOfDay = $@"
SELECT MAX(AccessNum) AS MaxAccessNum, AccessYear, AccessMonth, AccessDay FROM (
    SELECT COUNT(*) AS AccessNum, AccessYear, AccessMonth, AccessDay FROM (
        SELECT {SqlUtils.GetDatePartDay("AccessDateTime")} AS AccessDay, {SqlUtils.GetDatePartMonth("AccessDateTime")} AS AccessMonth, {SqlUtils.GetDatePartYear("AccessDateTime")} AS AccessYear
        FROM siteserver_Tracking
        WHERE PublishmentSystemID = @PublishmentSystemID
    ) DERIVEDTBL GROUP BY AccessYear, AccessMonth, AccessDay
) DERIVEDTBL GROUP BY AccessYear, AccessMonth, AccessDay";//最大访问量（日）

            var parms = new IDataParameter[]
			{
				GetParameter(ParmPublishmentSystemId, EDataType.Integer, publishmentSystemId)
			};

            using (var rdr = ExecuteReader(sqlSelectMaxAccessNumOfDay, parms))
            {
                if (rdr.Read())
                {
                    maxAccessNumOfDay = GetInt(rdr, 0);
                    var accessYear = GetInt(rdr, 1);
                    var accessMonth = GetInt(rdr, 2);
                    var accessDay = GetInt(rdr, 3);
                    maxAccessDay = $"{accessYear}-{accessMonth}-{accessDay}";
                }
                rdr.Close();
            }
            return maxAccessNumOfDay;
        }

        public virtual int GetMaxAccessNumOfMonth(int publishmentSystemId)
        {
            var maxAccessNumOfMonth = 0;

            var sqlSelectMaxAccessNumOfMonth = $@"
SELECT MAX(Expr1) AS Expr1 FROM (
    SELECT COUNT(*) AS Expr1 FROM (
        SELECT {SqlUtils.GetDatePartMonth("AccessDateTime")} AS Expr1, {SqlUtils.GetDatePartYear("AccessDateTime")} AS Expr2 
        FROM siteserver_Tracking 
        WHERE PublishmentSystemID = @PublishmentSystemID
    ) DERIVEDTBL GROUP BY Expr2, Expr1
) DERIVEDTBL";//最大访问量（月）

            var parms = new IDataParameter[]
			{
				GetParameter(ParmPublishmentSystemId, EDataType.Integer, publishmentSystemId)
			};

            using (var rdr = ExecuteReader(sqlSelectMaxAccessNumOfMonth, parms))
            {
                if (rdr.Read())
                {
                    maxAccessNumOfMonth = GetInt(rdr, 0);
                }
                rdr.Close();
            }
            return maxAccessNumOfMonth;
        }

        public virtual int GetMaxUniqueAccessNumOfDay(int publishmentSystemId)
        {
            var maxUniqueAccessNumOfDay = 0;

            var sqlSelectMaxUniqueAccessNumOfDay = $@"
SELECT MAX(Expr1) AS Expr1 FROM (
    SELECT COUNT(*) AS Expr1 FROM (
        SELECT {SqlUtils.GetDatePartDayOfYear("AccessDateTime")} AS Expr1, {SqlUtils.GetDatePartYear("AccessDateTime")} AS Expr2
        FROM siteserver_Tracking
        WHERE PublishmentSystemID = @PublishmentSystemID AND TrackerType = @TrackerType
    ) DERIVEDTBL GROUP BY Expr2, Expr1
) DERIVEDTBL";//最大唯一访客（日）

            var parms = new IDataParameter[]
			{
				GetParameter(ParmPublishmentSystemId, EDataType.Integer, publishmentSystemId),
				GetParameter(ParmTrackerType, EDataType.VarChar, 50, ETrackerTypeUtils.GetValue(ETrackerType.Site))
			};

            using (var rdr = ExecuteReader(sqlSelectMaxUniqueAccessNumOfDay, parms))
            {
                if (rdr.Read())
                {
                    maxUniqueAccessNumOfDay = TranslateUtils.ToInt(GetString(rdr, 0));
                }
                rdr.Close();
            }
            return maxUniqueAccessNumOfDay;
        }

        public virtual int GetMaxUniqueAccessNumOfMonth(int publishmentSystemId)
        {
            var maxUniqueAccessNumOfMonth = 0;

            var sqlSelectMaxUniqueAccessNumOfMonth = $@"
SELECT MAX(Expr1) AS Expr1 FROM (
    SELECT COUNT(*) AS Expr1 FROM (
        SELECT {SqlUtils.GetDatePartMonth("AccessDateTime")} AS Expr1, {SqlUtils.GetDatePartYear("AccessDateTime")} AS Expr2
        FROM siteserver_Tracking
        WHERE PublishmentSystemID = @PublishmentSystemID AND TrackerType = @TrackerType
    ) DERIVEDTBL GROUP BY Expr2, Expr1
) DERIVEDTBL";//最大唯一访客（月）

            var parms = new IDataParameter[]
			{
				GetParameter(ParmPublishmentSystemId, EDataType.Integer, publishmentSystemId),
				GetParameter(ParmTrackerType, EDataType.VarChar, 50, ETrackerTypeUtils.GetValue(ETrackerType.Site))
			};

            using (var rdr = ExecuteReader(sqlSelectMaxUniqueAccessNumOfMonth, parms))
            {
                if (rdr.Read())
                {
                    maxUniqueAccessNumOfMonth = GetInt(rdr, 0);
                }
                rdr.Close();
            }
            return maxUniqueAccessNumOfMonth;
        }

        public virtual ArrayList GetContentIpAddressArrayList(int publishmentSystemId, int nodeId, int contentId, DateTime begin, DateTime end)
        {
            var arraylist = new ArrayList();

            string sqlString;
            if (contentId != 0)
            {
                sqlString = $@"
SELECT IPAddress FROM siteserver_Tracking
WHERE (PublishmentSystemID = {publishmentSystemId} AND PageNodeID = {nodeId} AND PageContentID = {contentId} AND (AccessDateTime BETWEEN '{begin
                    .ToShortDateString()}' AND '{end.AddDays(1).ToShortDateString()}'))
";
            }
            else
            {
                sqlString = $@"
SELECT IPAddress FROM siteserver_Tracking
WHERE (PublishmentSystemID = {publishmentSystemId} AND PageNodeID = {nodeId} AND (AccessDateTime BETWEEN '{begin
                    .ToShortDateString()}' AND '{end.AddDays(1).ToShortDateString()}'))
";
            }

            using (var rdr = ExecuteReader(sqlString))
            {
                while (rdr.Read())
                {
                    arraylist.Add(GetString(rdr, 0));
                }
                rdr.Close();
            }

            return arraylist;
        }

        public virtual ArrayList GetTrackingInfoArrayList(int publishmentSystemId, int nodeId, int contentId, DateTime begin, DateTime end)
        {
            var arraylist = new ArrayList();

            string sqlString;
            if (contentId != 0)
            {
                sqlString = $@"
SELECT TrackingID, PublishmentSystemID, TrackerType, LastAccessDateTime, PageUrl, PageNodeID, PageContentID, Referrer, IPAddress, OperatingSystem, Browser, AccessDateTime FROM siteserver_Tracking
WHERE (PublishmentSystemID = {publishmentSystemId} AND PageNodeID = {nodeId} AND PageContentID = {contentId} AND (AccessDateTime BETWEEN '{begin
                    .ToShortDateString()}' AND '{end.AddDays(1).ToShortDateString()}'))
";
            }
            else
            {
                var nodeIdList = DataProvider.NodeDao.GetNodeIdListByScopeType(nodeId, EScopeType.All, string.Empty, string.Empty);
                nodeIdList.Add(nodeId);
                sqlString = $@"
SELECT TrackingID, PublishmentSystemID, TrackerType, LastAccessDateTime, PageUrl, PageNodeID, PageContentID, Referrer, IPAddress, OperatingSystem, Browser, AccessDateTime FROM siteserver_Tracking
WHERE (PublishmentSystemID = {publishmentSystemId} AND PageNodeID IN ({TranslateUtils
                    .ToSqlInStringWithoutQuote(nodeIdList)}) AND (AccessDateTime BETWEEN '{begin
                    .ToShortDateString()}' AND '{end.AddDays(1).ToShortDateString()}'))
";
            }

            using (var rdr = ExecuteReader(sqlString))
            {
                while (rdr.Read())
                {
                    var i = 0;
                    var trackingInfo = new TrackingInfo(GetInt(rdr, i++), GetInt(rdr, i++), ETrackerTypeUtils.GetEnumType(GetString(rdr, i++)), GetDateTime(rdr, i++), GetString(rdr, i++), GetInt(rdr, i++), GetInt(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetDateTime(rdr, i));
                    arraylist.Add(trackingInfo);
                }
                rdr.Close();
            }

            return arraylist;
        }

        public virtual Hashtable GetTrackingHourHashtable(int publishmentSystemId)
        {
            var hashtable = new Hashtable();

            string sqlSelectTrackingHour = $@"
SELECT COUNT(*) AS AccessNum, AccessYear, AccessMonth, AccessDay, AccessHour FROM (
    SELECT {SqlUtils.GetDatePartYear("AccessDateTime")} AS AccessYear, {SqlUtils.GetDatePartMonth("AccessDateTime")} AS AccessMonth, {SqlUtils.GetDatePartDay("AccessDateTime")} AS AccessDay, {SqlUtils.GetDatePartHour("AccessDateTime")} AS AccessHour
    FROM siteserver_Tracking 
    WHERE ({SqlUtils.GetDateDiffLessThanHours("AccessDateTime", 24.ToString())}) AND PublishmentSystemID = @PublishmentSystemID
) DERIVEDTBL GROUP BY AccessYear, AccessMonth, AccessDay, AccessHour ORDER BY AccessYear, AccessMonth, AccessDay, AccessHour";//访问量24小时统计

            var parms = new IDataParameter[]
			{
				GetParameter(ParmPublishmentSystemId, EDataType.Integer, publishmentSystemId)
			};

            using (var rdr = ExecuteReader(sqlSelectTrackingHour, parms))
            {
                while (rdr.Read())
                {
                    var accessNum = GetInt(rdr, 0);
                    var year = GetInt(rdr, 1);
                    var month = GetInt(rdr, 2);
                    var day = GetInt(rdr, 3);
                    var hour = GetInt(rdr, 4);

                    var dateTime = new DateTime(year, month, day, hour, 0, 0);
                    hashtable.Add(dateTime, accessNum);
                }
                rdr.Close();
            }
            return hashtable;
        }

        public virtual Hashtable GetUniqueTrackingHourHashtable(int publishmentSystemId)
        {
            var hashtable = new Hashtable();

            string sqlSelectUniqueTrackingHour = $@"
SELECT COUNT(*) AS AccessNum, AccessYear, AccessMonth, AccessDay, AccessHour FROM (
    SELECT {SqlUtils.GetDatePartYear("AccessDateTime")} AS AccessYear, {SqlUtils.GetDatePartMonth("AccessDateTime")} AS AccessMonth, {SqlUtils.GetDatePartDay("AccessDateTime")} AS AccessDay, {SqlUtils.GetDatePartHour("AccessDateTime")} AS AccessHour
    FROM siteserver_Tracking
    WHERE ({SqlUtils.GetDateDiffLessThanHours("AccessDateTime", 24.ToString())}) AND PublishmentSystemID = @PublishmentSystemID AND TrackerType = @TrackerType
) DERIVEDTBL GROUP BY AccessYear, AccessMonth, AccessDay, AccessHour ORDER BY AccessYear, AccessMonth, AccessDay, AccessHour";//访客24小时统计

            var parms = new IDataParameter[]
			{
				GetParameter(ParmPublishmentSystemId, EDataType.Integer, publishmentSystemId),
				GetParameter(ParmTrackerType, EDataType.VarChar, 50, ETrackerTypeUtils.GetValue(ETrackerType.Site))
			};

            using (var rdr = ExecuteReader(sqlSelectUniqueTrackingHour, parms))
            {
                while (rdr.Read())
                {
                    var accessNum = GetInt(rdr, 0);
                    var year = GetInt(rdr, 1);
                    var month = GetInt(rdr, 2);
                    var day = GetInt(rdr, 3);
                    var hour = GetInt(rdr, 4);

                    var dateTime = new DateTime(year, month, day, hour, 0, 0);
                    hashtable.Add(dateTime, accessNum);
                }
                rdr.Close();
            }
            return hashtable;
        }

        public virtual Hashtable GetTrackingDayHashtable(int publishmentSystemId)
        {
            var hashtable = new Hashtable();

            string sqlSelectTrackingDay = $@"
SELECT COUNT(*) AS AccessNum, AccessYear, AccessMonth, AccessDay FROM (
    SELECT {SqlUtils.GetDatePartYear("AccessDateTime")} AS AccessYear, {SqlUtils.GetDatePartMonth("AccessDateTime")} AS AccessMonth, {SqlUtils.GetDatePartDay("AccessDateTime")} AS AccessDay
    FROM siteserver_Tracking
    WHERE ({SqlUtils.GetDateDiffLessThanHours("AccessDateTime", 720.ToString())}) AND PublishmentSystemID = @PublishmentSystemID
) DERIVEDTBL GROUP BY AccessYear, AccessMonth, AccessDay ORDER BY AccessYear, AccessMonth, AccessDay";//访问量日统计

            var parms = new IDataParameter[]
			{
				GetParameter(ParmPublishmentSystemId, EDataType.Integer, publishmentSystemId)
			};

            using (var rdr = ExecuteReader(sqlSelectTrackingDay, parms))
            {
                while (rdr.Read())
                {
                    var accessNum = GetInt(rdr, 0);
                    var year = GetInt(rdr, 1);
                    var month = GetInt(rdr, 2);
                    var day = GetInt(rdr, 3);

                    var dateTime = new DateTime(year, month, day, 0, 0, 0);
                    hashtable.Add(dateTime, accessNum);
                }
                rdr.Close();
            }
            return hashtable;
        }

        public virtual Hashtable GetUniqueTrackingDayHashtable(int publishmentSystemId)
        {
            var hashtable = new Hashtable();

            string sqlSelectUniqueTrackingDay = $@"
SELECT COUNT(*) AS AccessNum, AccessYear, AccessMonth, AccessDay FROM (
    SELECT {SqlUtils.GetDatePartYear("AccessDateTime")} AS AccessYear, {SqlUtils.GetDatePartMonth("AccessDateTime")} AS AccessMonth, {SqlUtils.GetDatePartDay("AccessDateTime")} AS AccessDay
    FROM siteserver_Tracking
    WHERE ({SqlUtils.GetDateDiffLessThanHours("AccessDateTime", 720.ToString())}) AND PublishmentSystemID = @PublishmentSystemID AND TrackerType = @TrackerType
) DERIVEDTBL GROUP BY AccessYear, AccessMonth, AccessDay ORDER BY AccessYear, AccessMonth, AccessDay";//访客日统计

            var parms = new IDataParameter[]
			{
				GetParameter(ParmPublishmentSystemId, EDataType.Integer, publishmentSystemId),
				GetParameter(ParmTrackerType, EDataType.VarChar, 50, ETrackerTypeUtils.GetValue(ETrackerType.Site))
			};

            using (var rdr = ExecuteReader(sqlSelectUniqueTrackingDay, parms))
            {
                while (rdr.Read())
                {
                    var accessNum = GetInt(rdr, 0);
                    var year = GetInt(rdr, 1);
                    var month = GetInt(rdr, 2);
                    var day = GetInt(rdr, 3);

                    var dateTime = new DateTime(year, month, day, 0, 0, 0);
                    hashtable.Add(dateTime, accessNum);
                }
                rdr.Close();
            }
            return hashtable;
        }

        public virtual Hashtable GetTrackingMonthHashtable(int publishmentSystemId)
        {
            var hashtable = new Hashtable();

            string sqlSelectTrackingMonth = $@"
SELECT COUNT(*) AS AccessNum, AccessYear, AccessMonth FROM (
    SELECT {SqlUtils.GetDatePartYear("AccessDateTime")} AS AccessYear, {SqlUtils.GetDatePartMonth("AccessDateTime")} AS AccessMonth
    FROM siteserver_Tracking
    WHERE ({SqlUtils.GetDateDiffLessThanMonths("AccessDateTime", 12.ToString())}) AND PublishmentSystemID = @PublishmentSystemID
) DERIVEDTBL GROUP BY AccessYear, AccessMonth ORDER BY AccessYear, AccessMonth";//访问量月统计

            var parms = new IDataParameter[]
			{
				GetParameter(ParmPublishmentSystemId, EDataType.Integer, publishmentSystemId)
			};

            using (var rdr = ExecuteReader(sqlSelectTrackingMonth, parms))
            {
                while (rdr.Read())
                {
                    var accessNum = GetInt(rdr, 0);
                    var year = GetInt(rdr, 1);
                    var month = GetInt(rdr, 2);

                    var dateTime = new DateTime(year, month, 1, 0, 0, 0);
                    hashtable.Add(dateTime, accessNum);
                }
                rdr.Close();
            }
            return hashtable;
        }

        public virtual Hashtable GetUniqueTrackingMonthHashtable(int publishmentSystemId)
        {
            var hashtable = new Hashtable();

            string sqlSelectUniqueTrackingMonth = $@"
SELECT COUNT(*) AS AccessNum, AccessYear, AccessMonth FROM (
    SELECT {SqlUtils.GetDatePartYear("AccessDateTime")} AS AccessYear, {SqlUtils.GetDatePartMonth("AccessDateTime")} AS AccessMonth
    FROM siteserver_Tracking
    WHERE ({SqlUtils.GetDateDiffLessThanMonths("AccessDateTime", 12.ToString())}) AND PublishmentSystemID = @PublishmentSystemID AND TrackerType = @TrackerType
) DERIVEDTBL GROUP BY AccessYear, AccessMonth ORDER BY AccessYear, AccessMonth";//访客月统计

            var parms = new IDataParameter[]
			{
				GetParameter(ParmPublishmentSystemId, EDataType.Integer, publishmentSystemId),
				GetParameter(ParmTrackerType, EDataType.VarChar, 50, ETrackerTypeUtils.GetValue(ETrackerType.Site))
			};

            using (var rdr = ExecuteReader(sqlSelectUniqueTrackingMonth, parms))
            {
                while (rdr.Read())
                {
                    var accessNum = GetInt(rdr, 0);
                    var year = GetInt(rdr, 1);
                    var month = GetInt(rdr, 2);

                    var dateTime = new DateTime(year, month, 1, 0, 0, 0);
                    hashtable.Add(dateTime, accessNum);
                }
                rdr.Close();
            }
            return hashtable;
        }

        public virtual Hashtable GetTrackingYearHashtable(int publishmentSystemId)
        {
            var hashtable = new Hashtable();

            string sqlSelectTrackingYear = $@"
SELECT COUNT(*) AS AccessNum, AccessYear FROM (
    SELECT {SqlUtils.GetDatePartYear("AccessDateTime")} AS AccessYear
    FROM siteserver_Tracking
    WHERE ({SqlUtils.GetDateDiffLessThanYears("AccessDateTime", 10.ToString())}) AND PublishmentSystemID = @PublishmentSystemID
) DERIVEDTBL GROUP BY AccessYear ORDER BY AccessYear";//访问量年统计

            var parms = new IDataParameter[]
			{
				GetParameter(ParmPublishmentSystemId, EDataType.Integer, publishmentSystemId)
			};

            using (var rdr = ExecuteReader(sqlSelectTrackingYear, parms))
            {
                while (rdr.Read())
                {
                    var accessNum = GetInt(rdr, 0);
                    var year = GetInt(rdr, 1);

                    var dateTime = new DateTime(year, 1, 1, 0, 0, 0);
                    hashtable.Add(dateTime, accessNum);
                }
                rdr.Close();
            }
            return hashtable;
        }

        public virtual Hashtable GetUniqueTrackingYearHashtable(int publishmentSystemId)
        {
            var hashtable = new Hashtable();

            string sqlSelectUniqueTrackingYear = $@"
SELECT COUNT(*) AS AccessNum, AccessYear FROM (
    SELECT {SqlUtils.GetDatePartYear("AccessDateTime")} AS AccessYear
    FROM siteserver_Tracking
    WHERE ({SqlUtils.GetDateDiffLessThanYears("AccessDateTime", 10.ToString())}) AND PublishmentSystemID = @PublishmentSystemID AND TrackerType = @TrackerType
) DERIVEDTBL GROUP BY AccessYear ORDER BY AccessYear";//访客年统计

            var parms = new IDataParameter[]
			{
				GetParameter(ParmPublishmentSystemId, EDataType.Integer, publishmentSystemId),
				GetParameter(ParmTrackerType, EDataType.VarChar, 50, ETrackerTypeUtils.GetValue(ETrackerType.Site))
			};

            using (var rdr = ExecuteReader(sqlSelectUniqueTrackingYear, parms))
            {
                while (rdr.Read())
                {
                    var accessNum = GetInt(rdr, 0);
                    var year = GetInt(rdr, 1);

                    var dateTime = new DateTime(year, 1, 1, 0, 0, 0);
                    hashtable.Add(dateTime, accessNum);
                }
                rdr.Close();
            }
            return hashtable;
        }

        public virtual List<KeyValuePair<string, int>> GetPageUrlAccessPairList(int publishmentSystemId)
        {
            var pairList = new List<KeyValuePair<string, int>>();

            const string sqlSelectPageUrlAccessNum = @"
SELECT PageUrl, COUNT(*) AS AccessNum 
FROM siteserver_Tracking
WHERE (PublishmentSystemID = @PublishmentSystemID)
GROUP BY PageUrl ORDER BY AccessNum DESC
"; //访问页面，总访问量

            var parms = new IDataParameter[]
			{
				GetParameter(ParmPublishmentSystemId, EDataType.Integer, publishmentSystemId)
			};

            using (var rdr = ExecuteReader(sqlSelectPageUrlAccessNum, parms))
            {
                while (rdr.Read())
                {
                    var pageUrl = GetString(rdr, 0);
                    var accessNum = GetInt(rdr, 1);

                    var pair = new KeyValuePair<string, int>(pageUrl, accessNum);

                    pairList.Add(pair);
                }
                rdr.Close();
            }

            return pairList;
        }

        public Hashtable GetPageUrlUniqueAccessNumHashtable(int publishmentSystemId)
        {
            var hashtable = new Hashtable();

            var sqlSelectPageUrlUniqueAccessNum = @"
SELECT PageUrl, COUNT(*) AS UniqueAccessNum
FROM siteserver_Tracking
WHERE (PublishmentSystemID = @PublishmentSystemID) AND (TrackerType = @TrackerType)
GROUP BY PageUrl
";//访问页面，总唯一访客

            var parms = new IDataParameter[]
			{
				GetParameter(ParmPublishmentSystemId, EDataType.Integer, publishmentSystemId),
				GetParameter(ParmTrackerType, EDataType.VarChar, 50, ETrackerTypeUtils.GetValue(ETrackerType.Site))
			};

            using (var rdr = ExecuteReader(sqlSelectPageUrlUniqueAccessNum, parms))
            {
                while (rdr.Read())
                {
                    var pageUrl = GetString(rdr, 0);
                    var uniqueAccessNum = GetInt(rdr, 1);

                    hashtable.Add(pageUrl, uniqueAccessNum);
                }
                rdr.Close();
            }
            return hashtable;
        }

        public virtual Hashtable GetPageUrlTodayAccessNumHashtable(int publishmentSystemId)
        {
            var hashtable = new Hashtable();

            var defaultDateString = SqlUtils.GetDefaultDateString();
            string sqlSelectPageUrlTodayAccessNum = $@"
SELECT PageUrl, COUNT(*) AS TodayAccessNum
FROM siteserver_Tracking
WHERE (PublishmentSystemID = @PublishmentSystemID) AND 
    ({SqlUtils.GetDatePartYear("AccessDateTime")} = {SqlUtils.GetDatePartYear(defaultDateString)}) AND 
    ({SqlUtils.GetDatePartMonth("AccessDateTime")} = {SqlUtils.GetDatePartMonth(defaultDateString)}) AND 
    ({SqlUtils.GetDatePartDay("AccessDateTime")} = {SqlUtils.GetDatePartDay(defaultDateString)})
GROUP BY PageUrl
";//访问页面，当天总访问量

            var parms = new IDataParameter[]
			{
				GetParameter(ParmPublishmentSystemId, EDataType.Integer, publishmentSystemId)
			};

            using (var rdr = ExecuteReader(sqlSelectPageUrlTodayAccessNum, parms))
            {
                while (rdr.Read())
                {
                    var pageUrl = GetString(rdr, 0);
                    var accessNum = GetInt(rdr, 1);

                    hashtable.Add(pageUrl, accessNum);
                }
                rdr.Close();
            }
            return hashtable;
        }

        public virtual Hashtable GetPageUrlTodayUniqueAccessNumHashtable(int publishmentSystemId)
        {
            var hashtable = new Hashtable();

            var defaultDateString = SqlUtils.GetDefaultDateString();
            string sqlSelectPageUrlTodayUniqueAccessNum = $@"
SELECT PageUrl, COUNT(*) AS TodayUniqueAccessNum
FROM siteserver_Tracking
WHERE (PublishmentSystemID = @PublishmentSystemID) AND (TrackerType = @TrackerType) AND 
    ({SqlUtils.GetDatePartYear("AccessDateTime")} = {SqlUtils.GetDatePartYear(defaultDateString)}) AND 
    ({SqlUtils.GetDatePartMonth("AccessDateTime")} = {SqlUtils.GetDatePartMonth(defaultDateString)}) AND 
    ({SqlUtils.GetDatePartDay("AccessDateTime")} = {SqlUtils.GetDatePartDay(defaultDateString)})
GROUP BY PageUrl
";//访问页面，当天总唯一访客

            var parms = new IDataParameter[]
			{
				GetParameter(ParmPublishmentSystemId, EDataType.Integer, publishmentSystemId),
				GetParameter(ParmTrackerType, EDataType.VarChar, 50, ETrackerTypeUtils.GetValue(ETrackerType.Site))
			};

            using (var rdr = ExecuteReader(sqlSelectPageUrlTodayUniqueAccessNum, parms))
            {
                while (rdr.Read())
                {
                    var pageUrl = GetString(rdr, 0);
                    var uniqueAccessNum = GetInt(rdr, 1);

                    hashtable.Add(pageUrl, uniqueAccessNum);
                }
                rdr.Close();
            }
            return hashtable;
        }

        public virtual Hashtable GetReferrerAccessNumHashtable(int publishmentSystemId)
        {
            var hashtable = new Hashtable();

            var sqlSelectReferrerAccessNum = @"
SELECT Referrer, COUNT(*) AS AccessNum
FROM siteserver_Tracking
WHERE (PublishmentSystemID = @PublishmentSystemID)
GROUP BY Referrer ORDER BY AccessNum DESC
";//来路页面，总访问量

            var parms = new IDataParameter[]
			{
				GetParameter(ParmPublishmentSystemId, EDataType.Integer, publishmentSystemId)
			};

            using (var rdr = ExecuteReader(sqlSelectReferrerAccessNum, parms))
            {
                while (rdr.Read())
                {
                    var referrer = PageUtils.GetUrlWithoutPathInfo(GetString(rdr, 0));
                    var accessNum = GetInt(rdr, 1);

                    if (hashtable[referrer] != null)
                    {
                        var value = (int)hashtable[referrer];
                        accessNum += value;
                    }
                    hashtable[referrer] = accessNum;
                }
                rdr.Close();
            }

            return hashtable;
        }

        public Hashtable GetReferrerUniqueAccessNumHashtable(int publishmentSystemId)
        {
            var hashtable = new Hashtable();

            var sqlSelectReferrerUniqueAccessNum = @"
SELECT Referrer, COUNT(*) AS UniqueAccessNum
FROM siteserver_Tracking
WHERE (PublishmentSystemID = @PublishmentSystemID) AND (TrackerType = @TrackerType)
GROUP BY Referrer
";//来路页面，总唯一访客

            var parms = new IDataParameter[]
			{
				GetParameter(ParmPublishmentSystemId, EDataType.Integer, publishmentSystemId),
				GetParameter(ParmTrackerType, EDataType.VarChar, 50, ETrackerTypeUtils.GetValue(ETrackerType.Site))
			};

            using (var rdr = ExecuteReader(sqlSelectReferrerUniqueAccessNum, parms))
            {
                while (rdr.Read())
                {
                    var referrer = PageUtils.GetUrlWithoutPathInfo(GetString(rdr, 0));
                    var uniqueAccessNum = GetInt(rdr, 1);

                    if (hashtable[referrer] != null)
                    {
                        var value = (int)hashtable[referrer];
                        uniqueAccessNum += value;
                    }
                    hashtable[referrer] = uniqueAccessNum;
                }
                rdr.Close();
            }
            return hashtable;
        }

        public virtual Hashtable GetReferrerTodayAccessNumHashtable(int publishmentSystemId)
        {
            var hashtable = new Hashtable();

            var defaultDateString = SqlUtils.GetDefaultDateString();
            string sqlSelectReferrerTodayAccessNum = $@"
SELECT Referrer, COUNT(*) AS TodayAccessNum
FROM siteserver_Tracking
WHERE (PublishmentSystemID = @PublishmentSystemID) AND 
    ({SqlUtils.GetDatePartYear("AccessDateTime")} = {SqlUtils.GetDatePartYear(defaultDateString)}) AND 
    ({SqlUtils.GetDatePartMonth("AccessDateTime")} = {SqlUtils.GetDatePartMonth(defaultDateString)}) AND 
    ({SqlUtils.GetDatePartDay("AccessDateTime")} = {SqlUtils.GetDatePartDay(defaultDateString)})
GROUP BY Referrer
";//来路页面，当天总访问量

            var parms = new IDataParameter[]
			{
				GetParameter(ParmPublishmentSystemId, EDataType.Integer, publishmentSystemId)
			};

            using (var rdr = ExecuteReader(sqlSelectReferrerTodayAccessNum, parms))
            {
                while (rdr.Read())
                {
                    var referrer = PageUtils.GetUrlWithoutPathInfo(GetString(rdr, 0));
                    var accessNum = GetInt(rdr, 1);

                    if (hashtable[referrer] != null)
                    {
                        var value = (int)hashtable[referrer];
                        accessNum += value;
                    }
                    hashtable[referrer] = accessNum;
                }
                rdr.Close();
            }
            return hashtable;
        }

        public virtual Hashtable GetReferrerTodayUniqueAccessNumHashtable(int publishmentSystemId)
        {
            var hashtable = new Hashtable();

            var defaultDateString = SqlUtils.GetDefaultDateString();
            string sqlSelectReferrerTodayUniqueAccessNum = $@"
SELECT Referrer, COUNT(*) AS TodayUniqueAccessNum
FROM siteserver_Tracking
WHERE (PublishmentSystemID = @PublishmentSystemID) AND (TrackerType = @TrackerType) AND 
    ({SqlUtils.GetDatePartYear("AccessDateTime")} = {SqlUtils.GetDatePartYear(defaultDateString)}) AND 
    ({SqlUtils.GetDatePartMonth("AccessDateTime")} = {SqlUtils.GetDatePartMonth(defaultDateString)}) AND 
    ({SqlUtils.GetDatePartDay("AccessDateTime")} = {SqlUtils.GetDatePartDay(defaultDateString)})
GROUP BY Referrer
";//来路页面，当天总唯一访客

            var parms = new IDataParameter[]
			{
				GetParameter(ParmPublishmentSystemId, EDataType.Integer, publishmentSystemId),
				GetParameter(ParmTrackerType, EDataType.VarChar, 50, ETrackerTypeUtils.GetValue(ETrackerType.Site))
			};

            using (var rdr = ExecuteReader(sqlSelectReferrerTodayUniqueAccessNum, parms))
            {
                while (rdr.Read())
                {
                    var referrer = PageUtils.GetUrlWithoutPathInfo(GetString(rdr, 0));
                    var uniqueAccessNum = GetInt(rdr, 1);

                    if (hashtable[referrer] != null)
                    {
                        var value = (int)hashtable[referrer];
                        uniqueAccessNum += value;
                    }
                    hashtable[referrer] = uniqueAccessNum;
                }
                rdr.Close();
            }
            return hashtable;
        }

        public Hashtable GetOsAccessNumHashtable(int publishmentSystemId)
        {
            var hashtable = new Hashtable();

            //";//访问操作系统，总访问量
            var sqlSelectOsAccessNum = @"
SELECT OperatingSystem, COUNT(*) AS AccessNum
FROM siteserver_Tracking
WHERE (PublishmentSystemID = @PublishmentSystemID)
GROUP BY OperatingSystem
";//访问操作系统，总访问量

            var parms = new IDataParameter[]
			{
				GetParameter(ParmPublishmentSystemId, EDataType.Integer, publishmentSystemId)
			};

            using (var rdr = ExecuteReader(sqlSelectOsAccessNum, parms))
            {
                while (rdr.Read())
                {
                    var os = GetString(rdr, 0);
                    var accessNum = GetInt(rdr, 1);

                    hashtable.Add(os, accessNum);
                }
                rdr.Close();
            }

            return hashtable;
        }

        public Hashtable GetOsUniqueAccessNumHashtable(int publishmentSystemId)
        {
            var hashtable = new Hashtable();

            var sqlSelectOsUniqueAccessNum = @"
SELECT OperatingSystem, COUNT(*) AS UniqueAccessNum
FROM siteserver_Tracking
WHERE (PublishmentSystemID = @PublishmentSystemID) AND (TrackerType = @TrackerType)
GROUP BY OperatingSystem
";//访问操作系统，总唯一访客

            var parms = new IDataParameter[]
			{
				GetParameter(ParmPublishmentSystemId, EDataType.Integer, publishmentSystemId),
				GetParameter(ParmTrackerType, EDataType.VarChar, 50, ETrackerTypeUtils.GetValue(ETrackerType.Site))
			};

            using (var rdr = ExecuteReader(sqlSelectOsUniqueAccessNum, parms))
            {
                while (rdr.Read())
                {
                    var os = GetString(rdr, 0);
                    var uniqueAccessNum = GetInt(rdr, 1);

                    hashtable.Add(os, uniqueAccessNum);
                }
                rdr.Close();
            }
            return hashtable;
        }

        public virtual Hashtable GetOsTodayAccessNumHashtable(int publishmentSystemId)
        {
            var hashtable = new Hashtable();

            var defaultDateString = SqlUtils.GetDefaultDateString();
            string sqlSelectOsTodayAccessNum = $@"
SELECT OperatingSystem, COUNT(*) AS TodayAccessNum
FROM siteserver_Tracking
WHERE (PublishmentSystemID = @PublishmentSystemID) AND 
    ({SqlUtils.GetDatePartYear("AccessDateTime")} = {SqlUtils.GetDatePartYear(defaultDateString)}) AND 
    ({SqlUtils.GetDatePartMonth("AccessDateTime")} = {SqlUtils.GetDatePartMonth(defaultDateString)}) AND 
    ({SqlUtils.GetDatePartDay("AccessDateTime")} = {SqlUtils.GetDatePartDay(defaultDateString)})
GROUP BY OperatingSystem
";//访问操作系统，当天总访问量

            var parms = new IDataParameter[]
			{
				GetParameter(ParmPublishmentSystemId, EDataType.Integer, publishmentSystemId)
			};

            using (var rdr = ExecuteReader(sqlSelectOsTodayAccessNum, parms))
            {
                while (rdr.Read())
                {
                    var os = GetString(rdr, 0);
                    var accessNum = GetInt(rdr, 1);

                    hashtable.Add(os, accessNum);
                }
                rdr.Close();
            }
            return hashtable;
        }

        public virtual Hashtable GetOsTodayUniqueAccessNumHashtable(int publishmentSystemId)
        {
            var hashtable = new Hashtable();

            var defaultDateString = SqlUtils.GetDefaultDateString();
            string sqlSelectOsTodayUniqueAccessNum = $@"
SELECT OperatingSystem, COUNT(*) AS TodayUniqueAccessNum
FROM siteserver_Tracking
WHERE (PublishmentSystemID = @PublishmentSystemID) AND (TrackerType = @TrackerType) AND 
    ({SqlUtils.GetDatePartYear("AccessDateTime")} = {SqlUtils.GetDatePartYear(defaultDateString)}) AND 
    ({SqlUtils.GetDatePartMonth("AccessDateTime")} = {SqlUtils.GetDatePartMonth(defaultDateString)}) AND 
    ({SqlUtils.GetDatePartDay("AccessDateTime")} = {SqlUtils.GetDatePartDay(defaultDateString)})
GROUP BY OperatingSystem
";//访问操作系统，当天总唯一访客

            var parms = new IDataParameter[]
			{
				GetParameter(ParmPublishmentSystemId, EDataType.Integer, publishmentSystemId),
				GetParameter(ParmTrackerType, EDataType.VarChar, 50, ETrackerTypeUtils.GetValue(ETrackerType.Site))
			};

            using (var rdr = ExecuteReader(sqlSelectOsTodayUniqueAccessNum, parms))
            {
                while (rdr.Read())
                {
                    var os = GetString(rdr, 0);
                    var uniqueAccessNum = GetInt(rdr, 1);

                    hashtable.Add(os, uniqueAccessNum);
                }
                rdr.Close();
            }
            return hashtable;
        }

        public Hashtable GetBrowserAccessNumHashtable(int publishmentSystemId)
        {
            var hashtable = new Hashtable();

            var sqlSelectBrowserAccessNum = @"
SELECT Browser, COUNT(*) AS AccessNum
FROM siteserver_Tracking
WHERE (PublishmentSystemID = @PublishmentSystemID)
GROUP BY Browser
";//访问浏览器，总访问量

            var parms = new IDataParameter[]
			{
				GetParameter(ParmPublishmentSystemId, EDataType.Integer, publishmentSystemId)
			};

            using (var rdr = ExecuteReader(sqlSelectBrowserAccessNum, parms))
            {
                while (rdr.Read())
                {
                    var browser = GetString(rdr, 0);
                    var accessNum = GetInt(rdr, 1);

                    hashtable.Add(browser, accessNum);
                }
                rdr.Close();
            }

            return hashtable;
        }

        public Hashtable GetBrowserUniqueAccessNumHashtable(int publishmentSystemId)
        {
            var hashtable = new Hashtable();

            var sqlSelectBrowserUniqueAccessNum = @"
SELECT Browser, COUNT(*) AS UniqueAccessNum
FROM siteserver_Tracking
WHERE (PublishmentSystemID = @PublishmentSystemID) AND (TrackerType = @TrackerType)
GROUP BY Browser
";//访问浏览器，总唯一访客

            var parms = new IDataParameter[]
			{
				GetParameter(ParmPublishmentSystemId, EDataType.Integer, publishmentSystemId),
				GetParameter(ParmTrackerType, EDataType.VarChar, 50, ETrackerTypeUtils.GetValue(ETrackerType.Site))
			};

            using (var rdr = ExecuteReader(sqlSelectBrowserUniqueAccessNum, parms))
            {
                while (rdr.Read())
                {
                    var browser = GetString(rdr, 0);
                    var uniqueAccessNum = GetInt(rdr, 1);

                    hashtable.Add(browser, uniqueAccessNum);
                }
                rdr.Close();
            }
            return hashtable;
        }

        public virtual Hashtable GetBrowserTodayAccessNumHashtable(int publishmentSystemId)
        {
            var hashtable = new Hashtable();

            var defaultDateString = SqlUtils.GetDefaultDateString();
            string sqlSelectBrowserTodayAccessNum = $@"
SELECT Browser, COUNT(*) AS TodayAccessNum
FROM siteserver_Tracking
WHERE (PublishmentSystemID = @PublishmentSystemID) AND 
    ({SqlUtils.GetDatePartYear("AccessDateTime")} = {SqlUtils.GetDatePartYear(defaultDateString)}) AND 
    ({SqlUtils.GetDatePartMonth("AccessDateTime")} = {SqlUtils.GetDatePartMonth(defaultDateString)}) AND 
    ({SqlUtils.GetDatePartDay("AccessDateTime")} = {SqlUtils.GetDatePartDay(defaultDateString)})
GROUP BY Browser
";//访问浏览器，当天总访问量

            var parms = new IDataParameter[]
			{
				GetParameter(ParmPublishmentSystemId, EDataType.Integer, publishmentSystemId)
			};

            using (var rdr = ExecuteReader(sqlSelectBrowserTodayAccessNum, parms))
            {
                while (rdr.Read())
                {
                    var browser = GetString(rdr, 0);
                    var accessNum = GetInt(rdr, 1);

                    hashtable.Add(browser, accessNum);
                }
                rdr.Close();
            }
            return hashtable;
        }

        public virtual Hashtable GetBrowserTodayUniqueAccessNumHashtable(int publishmentSystemId)
        {
            var hashtable = new Hashtable();

            var defaultDateString = SqlUtils.GetDefaultDateString();
            string sqlSelectBrowserTodayUniqueAccessNum = $@"
SELECT Browser, COUNT(*) AS TodayUniqueAccessNum
FROM siteserver_Tracking
WHERE (PublishmentSystemID = @PublishmentSystemID) AND (TrackerType = @TrackerType) AND 
    ({SqlUtils.GetDatePartYear("AccessDateTime")} = {SqlUtils.GetDatePartYear(defaultDateString)}) AND 
    ({SqlUtils.GetDatePartMonth("AccessDateTime")} = {SqlUtils.GetDatePartMonth(defaultDateString)}) AND 
    ({SqlUtils.GetDatePartDay("AccessDateTime")} = {SqlUtils.GetDatePartDay(defaultDateString)})
GROUP BY Browser
";//访问浏览器，当天总唯一访客

            var parms = new IDataParameter[]
			{
				GetParameter(ParmPublishmentSystemId, EDataType.Integer, publishmentSystemId),
				GetParameter(ParmTrackerType, EDataType.VarChar, 50, ETrackerTypeUtils.GetValue(ETrackerType.Site))
			};

            using (var rdr = ExecuteReader(sqlSelectBrowserTodayUniqueAccessNum, parms))
            {
                while (rdr.Read())
                {
                    var browser = GetString(rdr, 0);
                    var uniqueAccessNum = GetInt(rdr, 1);

                    hashtable.Add(browser, uniqueAccessNum);
                }
                rdr.Close();
            }
            return hashtable;
        }


        public virtual Hashtable GetChannelAccessNumHashtable(int publishmentSystemId, DateTime begin, DateTime end)
        {
            var hashtable = new Hashtable();

            //访问栏目，总访问量
            string sqlSelectBrowserAccessNum = $@"
SELECT PageNodeID, COUNT(*) AS AccessNum
FROM siteserver_Tracking
WHERE (PublishmentSystemID = @PublishmentSystemID AND PageNodeID <> 0 AND PageContentID = 0 AND (AccessDateTime BETWEEN '{begin.ToShortDateString()}' AND '{end.AddDays(1).ToShortDateString()}'))
GROUP BY PageNodeID
";

            var parms = new IDataParameter[]
			{
				GetParameter(ParmPublishmentSystemId, EDataType.Integer, publishmentSystemId)
			};

            using (var rdr = ExecuteReader(sqlSelectBrowserAccessNum, parms))
            {
                while (rdr.Read())
                {
                    var nodeId = GetInt(rdr, 0);
                    var accessNum = GetInt(rdr, 1);

                    hashtable.Add(nodeId, accessNum);
                }
                rdr.Close();
            }

            return hashtable;
        }

        public virtual Hashtable GetChannelContentAccessNumHashtable(int publishmentSystemId, DateTime begin, DateTime end)
        {
            var hashtable = new Hashtable();

            //访问栏目，总访问量

            string sqlSelectBrowserAccessNum = $@"
SELECT PageNodeID, COUNT(*) AS AccessNum
FROM siteserver_Tracking
WHERE (PublishmentSystemID = @PublishmentSystemID AND PageNodeID <> 0 AND PageContentID <> 0 AND (AccessDateTime BETWEEN '{begin.ToShortDateString()}' AND '{end.AddDays(1).ToShortDateString()}'))
GROUP BY PageNodeID
";

            var parms = new IDataParameter[]
			{
				GetParameter(ParmPublishmentSystemId, EDataType.Integer, publishmentSystemId)
			};

            using (var rdr = ExecuteReader(sqlSelectBrowserAccessNum, parms))
            {
                while (rdr.Read())
                {
                    var nodeId = GetInt(rdr, 0);
                    var accessNum = GetInt(rdr, 1);

                    hashtable.Add(nodeId, accessNum);
                }
                rdr.Close();
            }

            return hashtable;
        }

        public virtual Hashtable GetContentAccessNumHashtable(int publishmentSystemId, int nodeId, DateTime begin, DateTime end)
        {
            var hashtable = new Hashtable();

            //访问栏目，总访问量
            string sqlSelectBrowserAccessNum = $@"
SELECT PageContentID, COUNT(*) AS AccessNum
FROM siteserver_Tracking
WHERE (PublishmentSystemID = @PublishmentSystemID AND PageNodeID = @PageNodeID AND PageContentID <> 0 AND (AccessDateTime BETWEEN '{begin.ToShortDateString()}' AND '{end.AddDays(1).ToShortDateString()}'))
GROUP BY PageContentID
";

            var parms = new IDataParameter[]
			{
				GetParameter(ParmPublishmentSystemId, EDataType.Integer, publishmentSystemId),
                GetParameter(ParmPageNodeId, EDataType.Integer, nodeId)
			};

            using (var rdr = ExecuteReader(sqlSelectBrowserAccessNum, parms))
            {
                while (rdr.Read())
                {
                    var contentId = GetInt(rdr, 0);
                    var accessNum = GetInt(rdr, 1);

                    hashtable.Add(contentId, accessNum);
                }
                rdr.Close();
            }

            return hashtable;
        }

        public virtual List<KeyValuePair<int, int>> GetContentAccessNumPairList(int publishmentSystemId)
        {
            var pairList = new List<KeyValuePair<int, int>>();

            const string sqlSelectPageUrlAccessNum = @"
SELECT PageContentID, COUNT(*) AS AccessNum
FROM siteserver_Tracking
WHERE (PublishmentSystemID = @PublishmentSystemID AND PageContentID <> 0 AND TrackerType = @TrackerType)
GROUP BY PageContentID ORDER BY AccessNum DESC
"; //访问页面，总访问量

            var parms = new IDataParameter[]
			{
				GetParameter(ParmPublishmentSystemId, EDataType.Integer, publishmentSystemId),
                GetParameter(ParmTrackerType, EDataType.VarChar, 50, ETrackerTypeUtils.GetValue(ETrackerType.Page))
			};

            using (var rdr = ExecuteReader(sqlSelectPageUrlAccessNum, parms))
            {
                while (rdr.Read())
                {
                    var pageContentId = GetInt(rdr, 0);
                    var accessNum = GetInt(rdr, 1);

                    var pair = new KeyValuePair<int, int>(pageContentId, accessNum);
                    pairList.Add(pair);
                }
                rdr.Close();
            }

            return pairList;
        }

        public virtual Hashtable GetTodayContentAccessNumHashtable(int publishmentSystemId)
        {
            var hashtable = new Hashtable();

            var defaultDateString = SqlUtils.GetDefaultDateString();
            string sqlSelectPageUrlTodayAccessNum = $@"
SELECT PageContentID, COUNT(*) AS TodayAccessNum
FROM siteserver_Tracking
WHERE (PublishmentSystemID = @PublishmentSystemID AND PageContentID <> 0 AND TrackerType = @TrackerType) AND 
    ({SqlUtils.GetDatePartYear("AccessDateTime")} = {SqlUtils.GetDatePartYear(defaultDateString)}) AND 
    ({SqlUtils.GetDatePartMonth("AccessDateTime")} = {SqlUtils.GetDatePartMonth(defaultDateString)}) AND 
    ({SqlUtils.GetDatePartDay("AccessDateTime")} = {SqlUtils.GetDatePartDay(defaultDateString)})
GROUP BY PageContentID
";//访问页面，当天总访问量

            var parms = new IDataParameter[]
			{
				GetParameter(ParmPublishmentSystemId, EDataType.Integer, publishmentSystemId),
                GetParameter(ParmTrackerType, EDataType.VarChar, 50, ETrackerTypeUtils.GetValue(ETrackerType.Page))
			};

            using (var rdr = ExecuteReader(sqlSelectPageUrlTodayAccessNum, parms))
            {
                while (rdr.Read())
                {
                    var pageContentId = GetInt(rdr, 0);
                    var accessNum = GetInt(rdr, 1);

                    hashtable.Add(pageContentId, accessNum);
                }
                rdr.Close();
            }
            return hashtable;
        }

        public virtual ArrayList GetPageNodeIdArrayListByAccessNum(int publishmentSystemId)
        {
            var arraylist = new ArrayList();

            var sqlSelectPageUrlAccessNum = @"
SELECT PageNodeID, COUNT(*) AS AccessNum
FROM siteserver_Tracking
WHERE (PublishmentSystemID = @PublishmentSystemID AND PageNodeID <> 0 AND TrackerType = @TrackerType)
GROUP BY PageNodeID ORDER BY AccessNum DESC
";//访问页面，总访问量

            var parms = new IDataParameter[]
			{
				GetParameter(ParmPublishmentSystemId, EDataType.Integer, publishmentSystemId),
                GetParameter(ParmTrackerType, EDataType.VarChar, 50, ETrackerTypeUtils.GetValue(ETrackerType.Page))
			};

            using (var rdr = ExecuteReader(sqlSelectPageUrlAccessNum, parms))
            {
                while (rdr.Read())
                {
                    var pageNodeId = GetInt(rdr, 0);
                    arraylist.Add(pageNodeId);
                }
                rdr.Close();
            }

            return arraylist;
        }

        public void DeleteAll(int publishmentSystemId)
        {
            string sqlString = $"DELETE FROM siteserver_Tracking WHERE PublishmentSystemID = {publishmentSystemId}";
            ExecuteNonQuery(sqlString);
        }

        /// <summary>
        /// 统计站点访问量
        /// add by sessionlliang at 201225
        /// </summary>
        /// <param name="publishmentSystemId"></param>
        /// <param name="dateFrom"></param>
        /// <param name="dateTo"></param>
        /// <returns></returns>
        public int GetHitsCountOfPublishmentSystem(int publishmentSystemId, DateTime dateFrom, DateTime dateTo)
        {
            string sqlString =
                $@" SELECT COUNT(*) FROM siteserver_Tracking WHERE PublishmentSystemId = {publishmentSystemId} ";
            if (dateFrom > DateUtils.SqlMinValue)
            {
                sqlString += $@" AND AccessDateTime >= '{dateFrom}' ";
            }
            if (dateTo > DateUtils.SqlMinValue)
            {
                sqlString += $@" AND AccessDateTime <= '{dateTo}' ";
            }
            return TranslateUtils.ToInt(ExecuteScalar(sqlString).ToString());
        }
    }
}
