using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using BaiRong.Core;
using BaiRong.Core.Data;
using SiteServer.CMS.WeiXin.Model;
using SiteServer.CMS.WeiXin.Model.Enumerations;
using SiteServer.Plugin;
using SiteServer.Plugin.Models;

namespace SiteServer.CMS.WeiXin.Provider
{
    public class KeywordResourceDao : DataProviderBase
	{
        private const string SqlUpdate = "UPDATE wx_KeywordResource SET PublishmentSystemID = @PublishmentSystemID, KeywordID = @KeywordID, Title = @Title, ImageUrl = @ImageUrl, Summary = @Summary, ResourceType = @ResourceType, IsShowCoverPic = @IsShowCoverPic, Content = @Content, NavigationUrl = @NavigationUrl, ChannelID = @ChannelID, ContentID = @ContentID, Taxis = @Taxis WHERE ResourceID = @ResourceID";

        private const string SqlDelete = "DELETE FROM wx_KeywordResource WHERE ResourceID = @ResourceID";

        private const string SqlSelect = "SELECT ResourceID, PublishmentSystemID, KeywordID, Title, ImageUrl, Summary, ResourceType, IsShowCoverPic, Content, NavigationUrl, ChannelID, ContentID, Taxis FROM wx_KeywordResource WHERE ResourceID = @ResourceID";

        private const string SqlSelectFirst = "SELECT TOP 1 ResourceID, PublishmentSystemID, KeywordID, Title, ImageUrl, Summary, ResourceType, IsShowCoverPic, Content, NavigationUrl, ChannelID, ContentID, Taxis FROM wx_KeywordResource WHERE KeywordID = @KeywordID ORDER BY Taxis";

        private const string SqlSelectAll = "SELECT ResourceID, PublishmentSystemID, KeywordID, Title, ImageUrl, Summary, ResourceType, IsShowCoverPic, Content, NavigationUrl, ChannelID, ContentID, Taxis FROM wx_KeywordResource WHERE KeywordID = @KeywordID ORDER BY Taxis";

        private const string SqlSelectAllId = "SELECT ResourceID FROM wx_KeywordResource WHERE KeywordID = @KeywordID ORDER BY Taxis";

        private const string ParmResourceId = "@ResourceID";
        private const string ParmPublishmentSystemId = "@PublishmentSystemID";
        private const string ParmKeywordId = "@KeywordID";
        private const string ParmTitle = "@Title";
        private const string ParmImageUrl = "@ImageUrl";
        private const string ParmSummary = "@Summary";
        private const string ParmResourceType = "@ResourceType";
        private const string ParmIsShowCoverPic = "@IsShowCoverPic";
        private const string ParmContent = "@Content";
        private const string ParmNavigationUrl = "@NavigationUrl";
        private const string ParmChannelId = "@ChannelID";
        private const string ParmContentId = "@ContentID";
        private const string ParmTaxis = "@Taxis";

        public int Insert(KeywordResourceInfo resourceInfo)
        {
            var resourceId = 0;

            var sqlString = "INSERT INTO wx_KeywordResource (PublishmentSystemID, KeywordID, Title, ImageUrl, Summary, ResourceType, IsShowCoverPic, Content, NavigationUrl, ChannelID, ContentID, Taxis) VALUES (@PublishmentSystemID, @KeywordID, @Title, @ImageUrl, @Summary, @ResourceType, @IsShowCoverPic, @Content, @NavigationUrl, @ChannelID, @ContentID, @Taxis)";

            var taxis = GetMaxTaxis(resourceInfo.KeywordId) + 1;
            var parms = new IDataParameter[]
			{
                GetParameter(ParmPublishmentSystemId, DataType.Integer, resourceInfo.PublishmentSystemId),
                GetParameter(ParmKeywordId, DataType.Integer, resourceInfo.KeywordId),
                GetParameter(ParmTitle, DataType.NVarChar, 255, resourceInfo.Title),
                GetParameter(ParmImageUrl, DataType.VarChar, 200, resourceInfo.ImageUrl),
                GetParameter(ParmSummary, DataType.NVarChar, 255, resourceInfo.Summary),
                GetParameter(ParmResourceType, DataType.VarChar, 50, EResourceTypeUtils.GetValue(resourceInfo.ResourceType)),
                GetParameter(ParmIsShowCoverPic, DataType.VarChar, 18, resourceInfo.IsShowCoverPic.ToString()),
                GetParameter(ParmContent, DataType.NText, resourceInfo.Content),
                GetParameter(ParmNavigationUrl, DataType.VarChar, 200, resourceInfo.NavigationUrl),
                GetParameter(ParmChannelId, DataType.Integer, resourceInfo.ChannelId),
                GetParameter(ParmContentId, DataType.Integer, resourceInfo.ContentId),
                GetParameter(ParmTaxis, DataType.Integer, taxis)
			};

            using (var conn = GetConnection())
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        resourceId = ExecuteNonQueryAndReturnId(trans, sqlString, parms);
                        trans.Commit();
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }

            return resourceId;
        }

        public void Update(KeywordResourceInfo resourceInfo)
        {
            var parms = new IDataParameter[]
			{
                GetParameter(ParmPublishmentSystemId, DataType.Integer, resourceInfo.PublishmentSystemId),
                GetParameter(ParmKeywordId, DataType.Integer, resourceInfo.KeywordId),
                GetParameter(ParmTitle, DataType.NVarChar, 255, resourceInfo.Title),
                GetParameter(ParmImageUrl, DataType.VarChar, 200, resourceInfo.ImageUrl),
                GetParameter(ParmSummary, DataType.NVarChar, 255, resourceInfo.Summary),
                GetParameter(ParmResourceType, DataType.VarChar, 50, EResourceTypeUtils.GetValue(resourceInfo.ResourceType)),
                GetParameter(ParmIsShowCoverPic, DataType.VarChar, 18, resourceInfo.IsShowCoverPic.ToString()),
                GetParameter(ParmContent, DataType.NText, resourceInfo.Content),
                GetParameter(ParmNavigationUrl, DataType.VarChar, 200, resourceInfo.NavigationUrl),
                GetParameter(ParmChannelId, DataType.Integer, resourceInfo.ChannelId),
                GetParameter(ParmContentId, DataType.Integer, resourceInfo.ContentId),
                GetParameter(ParmTaxis, DataType.Integer, resourceInfo.Taxis),
                GetParameter(ParmResourceId, DataType.Integer, resourceInfo.ResourceId)
			};

            ExecuteNonQuery(SqlUpdate, parms);
        }

        public void Delete(int resourceId)
        {
            var parms = new IDataParameter[]
			{
				GetParameter(ParmResourceId, DataType.Integer, resourceId)
			};

            ExecuteNonQuery(SqlDelete, parms);
        }

        public KeywordResourceInfo GetResourceInfo(int resourceId)
        {
            KeywordResourceInfo resourceInfo = null;

            var parms = new IDataParameter[]
			{
				GetParameter(ParmResourceId, DataType.Integer, resourceId)
			};

            using (var rdr = ExecuteReader(SqlSelect, parms))
            {
                if (rdr.Read())
                {
                    resourceInfo = new KeywordResourceInfo(rdr.GetInt32(0), rdr.GetInt32(1), rdr.GetInt32(2), rdr.GetValue(3).ToString(), rdr.GetValue(4).ToString(), rdr.GetValue(5).ToString(), EResourceTypeUtils.GetEnumType(rdr.GetValue(6).ToString()), TranslateUtils.ToBool(rdr.GetValue(7).ToString()), rdr.GetValue(8).ToString(), rdr.GetValue(9).ToString(), rdr.GetInt32(10), rdr.GetInt32(11), rdr.GetInt32(12));
                }
                rdr.Close();
            }

            return resourceInfo;
        }

        public KeywordResourceInfo GetFirstResourceInfo(int keywordId)
        {
            KeywordResourceInfo resourceInfo = null;

            var parms = new IDataParameter[]
			{
				GetParameter(ParmKeywordId, DataType.Integer, keywordId)
			};

            using (var rdr = ExecuteReader(SqlSelectFirst, parms))
            {
                if (rdr.Read())
                {
                    resourceInfo = new KeywordResourceInfo(rdr.GetInt32(0), rdr.GetInt32(1), rdr.GetInt32(2), rdr.GetValue(3).ToString(), rdr.GetValue(4).ToString(), rdr.GetValue(5).ToString(), EResourceTypeUtils.GetEnumType(rdr.GetValue(6).ToString()), TranslateUtils.ToBool(rdr.GetValue(7).ToString()), rdr.GetValue(8).ToString(), rdr.GetValue(9).ToString(), rdr.GetInt32(10), rdr.GetInt32(11), rdr.GetInt32(12));
                }
                rdr.Close();
            }

            return resourceInfo;
        }

        public IEnumerable GetDataSource(int keywordId)
        {
            var parms = new IDataParameter[]
			{
				GetParameter(ParmKeywordId, DataType.Integer, keywordId)
			};

            var enumerable = (IEnumerable)ExecuteReader(SqlSelectAll, parms);
            return enumerable;
        }

        public int GetCount(int keywordId)
        {
            var sqlString = "SELECT COUNT(*) FROM wx_KeywordResource WHERE KeywordID = " + keywordId;
            return BaiRongDataProvider.DatabaseDao.GetIntResult(sqlString);
        }

        public List<KeywordResourceInfo> GetResourceInfoList(int keywordId)
        {
            var list = new List<KeywordResourceInfo>();

            var parms = new IDataParameter[]
			{
				GetParameter(ParmKeywordId, DataType.Integer, keywordId)
			};

            using (var rdr = ExecuteReader(SqlSelectAll, parms))
            {
                while (rdr.Read())
                {
                    var resourceInfo = new KeywordResourceInfo(rdr.GetInt32(0), rdr.GetInt32(1), rdr.GetInt32(2), rdr.GetValue(3).ToString(), rdr.GetValue(4).ToString(), rdr.GetValue(5).ToString(), EResourceTypeUtils.GetEnumType(rdr.GetValue(6).ToString()), TranslateUtils.ToBool(rdr.GetValue(7).ToString()), rdr.GetValue(8).ToString(), rdr.GetValue(9).ToString(), rdr.GetInt32(10), rdr.GetInt32(11), rdr.GetInt32(12));
                    list.Add(resourceInfo);
                }
                rdr.Close();
            }

            return list;
        }

        public List<int> GetResourceIdList(int keywordId)
        {
            var list = new List<int>();

            var parms = new IDataParameter[]
			{
				GetParameter(ParmKeywordId, DataType.Integer, keywordId)
			};

            using (var rdr = ExecuteReader(SqlSelectAllId, parms))
            {
                while (rdr.Read())
                {
                    list.Add(rdr.GetInt32(0));
                }
                rdr.Close();
            }

            return list;
        }

        public bool UpdateTaxisToUp(int keywordId, int resourceId)
        {
            string sqlString =
                $"SELECT TOP 1 ResourceID, Taxis FROM wx_KeywordResource WHERE (Taxis > (SELECT Taxis FROM wx_KeywordResource WHERE ResourceID = {resourceId} AND KeywordID = {keywordId})) AND KeywordID = {keywordId} ORDER BY Taxis";
            var higherId = 0;
            var higherTaxis = 0;

            using (var rdr = ExecuteReader(sqlString))
            {
                if (rdr.Read())
                {
                    higherId = rdr.GetInt32(0);
                    higherTaxis = rdr.GetInt32(1);
                }
                rdr.Close();
            }

            var selectedTaxis = GetTaxis(resourceId);

            if (higherId > 0)
            {
                SetTaxis(resourceId, higherTaxis);
                SetTaxis(higherId, selectedTaxis);
                return true;
            }
            return false;
        }

        public bool UpdateTaxisToDown(int keywordId, int resourceId)
        {
            string sqlString =
                $"SELECT TOP 1 ResourceID, Taxis FROM wx_KeywordResource WHERE (Taxis < (SELECT Taxis FROM wx_KeywordResource WHERE ResourceID = {resourceId} AND KeywordID = {keywordId})) AND KeywordID = {keywordId} ORDER BY Taxis DESC";
            var lowerId = 0;
            var lowerTaxis = 0;

            using (var rdr = ExecuteReader(sqlString))
            {
                if (rdr.Read())
                {
                    lowerId = rdr.GetInt32(0);
                    lowerTaxis = rdr.GetInt32(1);
                }
                rdr.Close();
            }

            var selectedTaxis = GetTaxis(resourceId);

            if (lowerId > 0)
            {
                SetTaxis(resourceId, lowerTaxis);
                SetTaxis(lowerId, selectedTaxis);
                return true;
            }
            return false;
        }

        private int GetMaxTaxis(int keywordId)
        {
            string sqlString = $"SELECT MAX(Taxis) FROM wx_KeywordResource WHERE KeywordID = {keywordId}";
            return BaiRongDataProvider.DatabaseDao.GetIntResult(sqlString);
        }

        private int GetTaxis(int resourceId)
        {
            string sqlString = $"SELECT Taxis FROM wx_KeywordResource WHERE ResourceID = {resourceId}";
            var taxis = 0;

            using (var rdr = ExecuteReader(sqlString))
            {
                if (rdr.Read())
                {
                    taxis = Convert.ToInt32(rdr[0]);
                }
                rdr.Close();
            }

            return taxis;
        }

        private void SetTaxis(int resourceId, int taxis)
        {
            string sqlString = $"UPDATE wx_KeywordResource SET Taxis = {taxis} WHERE ResourceID = {resourceId}";
            ExecuteNonQuery(sqlString);
        }
        public List<KeywordResourceInfo> GetKeywordResourceInfoList(int publishmentSystemId,int keywordId)
        {
            var list = new List<KeywordResourceInfo>();

            var parms = new IDataParameter[]
			{
                GetParameter(ParmPublishmentSystemId, DataType.Integer, publishmentSystemId),
				GetParameter(ParmKeywordId, DataType.Integer, keywordId)
			};

            using (var rdr = ExecuteReader(SqlSelectAll, parms))
            {
                while (rdr.Read())
                {
                    var resourceInfo = new KeywordResourceInfo(rdr.GetInt32(0), rdr.GetInt32(1), rdr.GetInt32(2), rdr.GetValue(3).ToString(), rdr.GetValue(4).ToString(), rdr.GetValue(5).ToString(), EResourceTypeUtils.GetEnumType(rdr.GetValue(6).ToString()), TranslateUtils.ToBool(rdr.GetValue(7).ToString()), rdr.GetValue(8).ToString(), rdr.GetValue(9).ToString(), rdr.GetInt32(10), rdr.GetInt32(11), rdr.GetInt32(12));
                    list.Add(resourceInfo);
                }
                rdr.Close();
            }

            return list;
        }
    
	}
}