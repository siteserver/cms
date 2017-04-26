using System.Data;
using BaiRong.Core.Data;
using BaiRong.Core.Model.Enumerations;

namespace SiteServer.CMS.Provider
{
    public class StarSettingDao : DataProviderBase
	{
        private const string SqlSelectStarSetting = "SELECT TotalCount, PointAverage FROM siteserver_StarSetting WHERE PublishmentSystemID = @PublishmentSystemID AND ContentID = @ContentID";

        private const string SqlSelectStarSettingId = "SELECT StarSettingID FROM siteserver_StarSetting WHERE PublishmentSystemID = @PublishmentSystemID AND ContentID = @ContentID";

        private const string SqlUpdateStarSetting = "UPDATE siteserver_StarSetting SET TotalCount = @TotalCount, PointAverage = @PointAverage WHERE StarSettingID = @StarSettingID";

        private const string ParmStarSettingId = "@StarSettingID";
        private const string ParmPublishmentSystemId = "@PublishmentSystemID";
        private const string ParmChannelId = "@ChannelID";
        private const string ParmContentId = "@ContentID";
        private const string ParmTotalCount = "@TotalCount";
        private const string ParmPointAverage = "@PointAverage";

        public void SetStarSetting(int publishmentSystemId, int channelId, int contentId, int totalCount, decimal pointAverage)
		{
            var starSettingId = 0;

            var parms = new IDataParameter[]
			{
				GetParameter(ParmPublishmentSystemId, EDataType.Integer, publishmentSystemId),
                GetParameter(ParmChannelId, EDataType.Integer, channelId),
                GetParameter(ParmContentId, EDataType.Integer, contentId)
			};

            using (var rdr = ExecuteReader(SqlSelectStarSettingId, parms))
            {
                if (rdr.Read())
                {
                    starSettingId = GetInt(rdr, 0);
                }
            }

            if (starSettingId > 0)
            {
                parms = new IDataParameter[]
			    {
				    GetParameter(ParmTotalCount, EDataType.Integer, totalCount),
				    GetParameter(ParmPointAverage, EDataType.Decimal, 18, pointAverage),
                    GetParameter(ParmStarSettingId, EDataType.Integer, starSettingId)
			    };

                ExecuteNonQuery(SqlUpdateStarSetting, parms);
            }
            else
            {
                var sqlInsertStarSetting = "INSERT INTO siteserver_StarSetting (PublishmentSystemID, ChannelID, ContentID, TotalCount, PointAverage) VALUES (@PublishmentSystemID, @ChannelID, @ContentID, @TotalCount, @PointAverage)";

                parms = new IDataParameter[]
			    {
				    GetParameter(ParmPublishmentSystemId, EDataType.Integer, publishmentSystemId),
				    GetParameter(ParmChannelId, EDataType.Integer, channelId),
                    GetParameter(ParmContentId, EDataType.Integer, contentId),
				    GetParameter(ParmTotalCount, EDataType.Integer, totalCount),
				    GetParameter(ParmPointAverage, EDataType.Decimal, 18, pointAverage)
			    };

                ExecuteNonQuery(sqlInsertStarSetting, parms);
            }
		}

        public object[] GetTotalCountAndPointAverage(int publishmentSystemId, int contentId)
        {
            var totalCount = 0;
            decimal pointAverage = 0;

            var parms = new IDataParameter[]
			{
				GetParameter(ParmPublishmentSystemId, EDataType.Integer, publishmentSystemId),
                GetParameter(ParmContentId, EDataType.Integer, contentId)
			};

            using (var rdr = ExecuteReader(SqlSelectStarSetting, parms))
            {
                if (rdr.Read())
                {
                    totalCount = GetInt(rdr, 0);
                    pointAverage = GetDecimal(rdr, 1);
                }
                rdr.Close();
            }
            return new object[] { totalCount, pointAverage };
        }
	}
}
