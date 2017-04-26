using System;
using System.Collections;
using System.Data;
using BaiRong.Core.Data;
using BaiRong.Core.Model.Enumerations;

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
				GetParameter(ParmPublishmentSystemId, EDataType.Integer, publishmentSystemId),
				GetParameter(ParmChannelId, EDataType.Integer, channelId),
                GetParameter(ParmContentId, EDataType.Integer, contentId),
				GetParameter(ParmUserName, EDataType.NVarChar, 255, userName),
				GetParameter(ParmPoint, EDataType.Integer, point),
                GetParameter(ParmMessage, EDataType.NVarChar, 255, message),
                GetParameter(ParmAdddate, EDataType.DateTime, addDate)
			};

            ExecuteNonQuery(sqlString, parms);
		}

        public int[] GetCount(int publishmentSystemId, int channelId, int contentId)
        {
            var totalCount = 0;
            var totalPoint = 0;

            var parms = new IDataParameter[]
			{
				GetParameter(ParmPublishmentSystemId, EDataType.Integer, publishmentSystemId),
                GetParameter(ParmContentId, EDataType.Integer, contentId)
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

        public ArrayList GetContentIdArrayListByPoint(int publishmentSystemId)
        {
            var arraylist = new ArrayList();

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
                    arraylist.Add(relatedIdentity);
                }
                rdr.Close();
            }

            return arraylist;
        }
	}
}
