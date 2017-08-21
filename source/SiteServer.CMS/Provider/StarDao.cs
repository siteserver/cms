using System;
using System.Collections.Generic;
using System.Data;
using BaiRong.Core.Data;
using SiteServer.Plugin;
using SiteServer.Plugin.Models;

namespace SiteServer.CMS.Provider
{
    public class StarDao : DataProviderBase
	{
        private const string SqlSelectStar = "SELECT Point FROM siteserver_Star WHERE PublishmentSystemID = @PublishmentSystemID AND ContentID = @ContentID";

        private const string ParmPublishmentSystemId = "@PublishmentSystemID";
        private const string ParmChannelId = "@ChannelID";
        private const string ParmContentId = "@ContentID";
        private const string ParmUserName = "@UserName";
        private const string ParmPoint = "@Point";
        private const string ParmMessage = "@Message";
        private const string ParmAdddate = "@AddDate";

        public void AddCount(int publishmentSystemId, int channelId, int contentId, string userName, int point, string message, DateTime addDate)
		{
            var sqlString = "INSERT INTO siteserver_Star (PublishmentSystemID, ChannelID, ContentID, UserName, Point, Message, AddDate) VALUES (@PublishmentSystemID, @ChannelID, @ContentID, @UserName, @Point, @Message, @AddDate)";

			var parms = new IDataParameter[]
			{
				GetParameter(ParmPublishmentSystemId, DataType.Integer, publishmentSystemId),
				GetParameter(ParmChannelId, DataType.Integer, channelId),
                GetParameter(ParmContentId, DataType.Integer, contentId),
				GetParameter(ParmUserName, DataType.NVarChar, 255, userName),
				GetParameter(ParmPoint, DataType.Integer, point),
                GetParameter(ParmMessage, DataType.NVarChar, 255, message),
                GetParameter(ParmAdddate, DataType.DateTime, addDate)
			};

            ExecuteNonQuery(sqlString, parms);
		}

        public int[] GetCount(int publishmentSystemId, int channelId, int contentId)
        {
            var totalCount = 0;
            var totalPoint = 0;

            var parms = new IDataParameter[]
			{
				GetParameter(ParmPublishmentSystemId, DataType.Integer, publishmentSystemId),
                GetParameter(ParmContentId, DataType.Integer, contentId)
			};

            using (var rdr = ExecuteReader(SqlSelectStar, parms))
            {
                while (rdr.Read())
                {
                    totalCount++;
                    totalPoint += GetInt(rdr, 0);
                }
                rdr.Close();
            }
            return new[] { totalCount, totalPoint };
        }

        public List<int> GetContentIdListByPoint(int publishmentSystemId)
        {
            var list = new List<int>();

            string sqlString = $@"
SELECT ContentID, (SUM(Point) * 100)/Count(*) AS Num
FROM siteserver_Star
WHERE (PublishmentSystemID = {publishmentSystemId} AND ContentID > 0)
GROUP BY ContentID
ORDER BY Num DESC";

            using (var rdr = ExecuteReader(sqlString))
            {
                while (rdr.Read())
                {
                    var relatedIdentity = GetInt(rdr, 0);
                    list.Add(relatedIdentity);
                }
                rdr.Close();
            }

            return list;
        }
	}
}
