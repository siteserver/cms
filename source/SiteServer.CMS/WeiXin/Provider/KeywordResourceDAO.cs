using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using BaiRong.Core;
using BaiRong.Core.Data;
using BaiRong.Core.Model.Enumerations;
using SiteServer.CMS.WeiXin.Model;
using SiteServer.CMS.WeiXin.Model.Enumerations;

namespace SiteServer.CMS.WeiXin.Provider
{
    public class KeywordResourceDAO : DataProviderBase
	{
        private const string SQL_UPDATE = "UPDATE wx_KeywordResource SET PublishmentSystemID = @PublishmentSystemID, KeywordID = @KeywordID, Title = @Title, ImageUrl = @ImageUrl, Summary = @Summary, ResourceType = @ResourceType, IsShowCoverPic = @IsShowCoverPic, Content = @Content, NavigationUrl = @NavigationUrl, ChannelID = @ChannelID, ContentID = @ContentID, Taxis = @Taxis WHERE ResourceID = @ResourceID";

        private const string SQL_DELETE = "DELETE FROM wx_KeywordResource WHERE ResourceID = @ResourceID";

        private const string SQL_SELECT = "SELECT ResourceID, PublishmentSystemID, KeywordID, Title, ImageUrl, Summary, ResourceType, IsShowCoverPic, Content, NavigationUrl, ChannelID, ContentID, Taxis FROM wx_KeywordResource WHERE ResourceID = @ResourceID";

        private const string SQL_SELECT_FIRST = "SELECT TOP 1 ResourceID, PublishmentSystemID, KeywordID, Title, ImageUrl, Summary, ResourceType, IsShowCoverPic, Content, NavigationUrl, ChannelID, ContentID, Taxis FROM wx_KeywordResource WHERE KeywordID = @KeywordID ORDER BY Taxis";

        private const string SQL_SELECT_ALL = "SELECT ResourceID, PublishmentSystemID, KeywordID, Title, ImageUrl, Summary, ResourceType, IsShowCoverPic, Content, NavigationUrl, ChannelID, ContentID, Taxis FROM wx_KeywordResource WHERE KeywordID = @KeywordID ORDER BY Taxis";

        private const string SQL_SELECT_ALL_ID = "SELECT ResourceID FROM wx_KeywordResource WHERE KeywordID = @KeywordID ORDER BY Taxis";

        private const string PARM_RESOURCE_ID = "@ResourceID";
        private const string PARM_PUBLISHMENT_SYSTEM_ID = "@PublishmentSystemID";
        private const string PARM_KEYWORD_ID = "@KeywordID";
        private const string PARM_TITLE = "@Title";
        private const string PARM_IMAGE_URL = "@ImageUrl";
        private const string PARM_SUMMARY = "@Summary";
        private const string PARM_RESOURCE_TYPE = "@ResourceType";
        private const string PARM_IS_SHOW_COVER_PIC = "@IsShowCoverPic";
        private const string PARM_CONTENT = "@Content";
        private const string PARM_NAVIGATION_URL = "@NavigationUrl";
        private const string PARM_CHANNEL_ID = "@ChannelID";
        private const string PARM_CONTENT_ID = "@ContentID";
        private const string PARM_TAXIS = "@Taxis";

        public int Insert(KeywordResourceInfo resourceInfo)
        {
            var resourceID = 0;

            var sqlString = "INSERT INTO wx_KeywordResource (PublishmentSystemID, KeywordID, Title, ImageUrl, Summary, ResourceType, IsShowCoverPic, Content, NavigationUrl, ChannelID, ContentID, Taxis) VALUES (@PublishmentSystemID, @KeywordID, @Title, @ImageUrl, @Summary, @ResourceType, @IsShowCoverPic, @Content, @NavigationUrl, @ChannelID, @ContentID, @Taxis)";

            var taxis = GetMaxTaxis(resourceInfo.KeywordID) + 1;
            var parms = new IDataParameter[]
			{
                GetParameter(PARM_PUBLISHMENT_SYSTEM_ID, EDataType.Integer, resourceInfo.PublishmentSystemID),
                GetParameter(PARM_KEYWORD_ID, EDataType.Integer, resourceInfo.KeywordID),
                GetParameter(PARM_TITLE, EDataType.NVarChar, 255, resourceInfo.Title),
                GetParameter(PARM_IMAGE_URL, EDataType.VarChar, 200, resourceInfo.ImageUrl),
                GetParameter(PARM_SUMMARY, EDataType.NVarChar, 255, resourceInfo.Summary),
                GetParameter(PARM_RESOURCE_TYPE, EDataType.VarChar, 50, EResourceTypeUtils.GetValue(resourceInfo.ResourceType)),
                GetParameter(PARM_IS_SHOW_COVER_PIC, EDataType.VarChar, 18, resourceInfo.IsShowCoverPic.ToString()),
                GetParameter(PARM_CONTENT, EDataType.NText, resourceInfo.Content),
                GetParameter(PARM_NAVIGATION_URL, EDataType.VarChar, 200, resourceInfo.NavigationUrl),
                GetParameter(PARM_CHANNEL_ID, EDataType.Integer, resourceInfo.ChannelID),
                GetParameter(PARM_CONTENT_ID, EDataType.Integer, resourceInfo.ContentID),
                GetParameter(PARM_TAXIS, EDataType.Integer, taxis)
			};

            using (var conn = GetConnection())
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        ExecuteNonQuery(trans, sqlString, parms);
                        resourceID = BaiRongDataProvider.DatabaseDao.GetSequence(trans, "wx_KeywordResource");
                        trans.Commit();
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }

            return resourceID;
        }

        public void Update(KeywordResourceInfo resourceInfo)
        {
            var parms = new IDataParameter[]
			{
                GetParameter(PARM_PUBLISHMENT_SYSTEM_ID, EDataType.Integer, resourceInfo.PublishmentSystemID),
                GetParameter(PARM_KEYWORD_ID, EDataType.Integer, resourceInfo.KeywordID),
                GetParameter(PARM_TITLE, EDataType.NVarChar, 255, resourceInfo.Title),
                GetParameter(PARM_IMAGE_URL, EDataType.VarChar, 200, resourceInfo.ImageUrl),
                GetParameter(PARM_SUMMARY, EDataType.NVarChar, 255, resourceInfo.Summary),
                GetParameter(PARM_RESOURCE_TYPE, EDataType.VarChar, 50, EResourceTypeUtils.GetValue(resourceInfo.ResourceType)),
                GetParameter(PARM_IS_SHOW_COVER_PIC, EDataType.VarChar, 18, resourceInfo.IsShowCoverPic.ToString()),
                GetParameter(PARM_CONTENT, EDataType.NText, resourceInfo.Content),
                GetParameter(PARM_NAVIGATION_URL, EDataType.VarChar, 200, resourceInfo.NavigationUrl),
                GetParameter(PARM_CHANNEL_ID, EDataType.Integer, resourceInfo.ChannelID),
                GetParameter(PARM_CONTENT_ID, EDataType.Integer, resourceInfo.ContentID),
                GetParameter(PARM_TAXIS, EDataType.Integer, resourceInfo.Taxis),
                GetParameter(PARM_RESOURCE_ID, EDataType.Integer, resourceInfo.ResourceID)
			};

            ExecuteNonQuery(SQL_UPDATE, parms);
        }

        public void Delete(int resourceID)
        {
            var parms = new IDataParameter[]
			{
				GetParameter(PARM_RESOURCE_ID, EDataType.Integer, resourceID)
			};

            ExecuteNonQuery(SQL_DELETE, parms);
        }

        public KeywordResourceInfo GetResourceInfo(int resourceID)
        {
            KeywordResourceInfo resourceInfo = null;

            var parms = new IDataParameter[]
			{
				GetParameter(PARM_RESOURCE_ID, EDataType.Integer, resourceID)
			};

            using (var rdr = ExecuteReader(SQL_SELECT, parms))
            {
                if (rdr.Read())
                {
                    resourceInfo = new KeywordResourceInfo(rdr.GetInt32(0), rdr.GetInt32(1), rdr.GetInt32(2), rdr.GetValue(3).ToString(), rdr.GetValue(4).ToString(), rdr.GetValue(5).ToString(), EResourceTypeUtils.GetEnumType(rdr.GetValue(6).ToString()), TranslateUtils.ToBool(rdr.GetValue(7).ToString()), rdr.GetValue(8).ToString(), rdr.GetValue(9).ToString(), rdr.GetInt32(10), rdr.GetInt32(11), rdr.GetInt32(12));
                }
                rdr.Close();
            }

            return resourceInfo;
        }

        public KeywordResourceInfo GetFirstResourceInfo(int keywordID)
        {
            KeywordResourceInfo resourceInfo = null;

            var parms = new IDataParameter[]
			{
				GetParameter(PARM_KEYWORD_ID, EDataType.Integer, keywordID)
			};

            using (var rdr = ExecuteReader(SQL_SELECT_FIRST, parms))
            {
                if (rdr.Read())
                {
                    resourceInfo = new KeywordResourceInfo(rdr.GetInt32(0), rdr.GetInt32(1), rdr.GetInt32(2), rdr.GetValue(3).ToString(), rdr.GetValue(4).ToString(), rdr.GetValue(5).ToString(), EResourceTypeUtils.GetEnumType(rdr.GetValue(6).ToString()), TranslateUtils.ToBool(rdr.GetValue(7).ToString()), rdr.GetValue(8).ToString(), rdr.GetValue(9).ToString(), rdr.GetInt32(10), rdr.GetInt32(11), rdr.GetInt32(12));
                }
                rdr.Close();
            }

            return resourceInfo;
        }

        public IEnumerable GetDataSource(int keywordID)
        {
            var parms = new IDataParameter[]
			{
				GetParameter(PARM_KEYWORD_ID, EDataType.Integer, keywordID)
			};

            var enumerable = (IEnumerable)ExecuteReader(SQL_SELECT_ALL, parms);
            return enumerable;
        }

        public int GetCount(int keywordID)
        {
            var sqlString = "SELECT COUNT(*) FROM wx_KeywordResource WHERE KeywordID = " + keywordID;
            return BaiRongDataProvider.DatabaseDao.GetIntResult(sqlString);
        }

        public List<KeywordResourceInfo> GetResourceInfoList(int keywordID)
        {
            var list = new List<KeywordResourceInfo>();

            var parms = new IDataParameter[]
			{
				GetParameter(PARM_KEYWORD_ID, EDataType.Integer, keywordID)
			};

            using (var rdr = ExecuteReader(SQL_SELECT_ALL, parms))
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

        public List<int> GetResourceIDList(int keywordID)
        {
            var list = new List<int>();

            var parms = new IDataParameter[]
			{
				GetParameter(PARM_KEYWORD_ID, EDataType.Integer, keywordID)
			};

            using (var rdr = ExecuteReader(SQL_SELECT_ALL_ID, parms))
            {
                while (rdr.Read())
                {
                    list.Add(rdr.GetInt32(0));
                }
                rdr.Close();
            }

            return list;
        }

        public bool UpdateTaxisToUp(int keywordID, int resourceID)
        {
            string sqlString =
                $"SELECT TOP 1 ResourceID, Taxis FROM wx_KeywordResource WHERE (Taxis > (SELECT Taxis FROM wx_KeywordResource WHERE ResourceID = {resourceID} AND KeywordID = {keywordID})) AND KeywordID = {keywordID} ORDER BY Taxis";
            var higherID = 0;
            var higherTaxis = 0;

            using (var rdr = ExecuteReader(sqlString))
            {
                if (rdr.Read())
                {
                    higherID = rdr.GetInt32(0);
                    higherTaxis = rdr.GetInt32(1);
                }
                rdr.Close();
            }

            var selectedTaxis = GetTaxis(resourceID);

            if (higherID > 0)
            {
                SetTaxis(resourceID, higherTaxis);
                SetTaxis(higherID, selectedTaxis);
                return true;
            }
            return false;
        }

        public bool UpdateTaxisToDown(int keywordID, int resourceID)
        {
            string sqlString =
                $"SELECT TOP 1 ResourceID, Taxis FROM wx_KeywordResource WHERE (Taxis < (SELECT Taxis FROM wx_KeywordResource WHERE ResourceID = {resourceID} AND KeywordID = {keywordID})) AND KeywordID = {keywordID} ORDER BY Taxis DESC";
            var lowerID = 0;
            var lowerTaxis = 0;

            using (var rdr = ExecuteReader(sqlString))
            {
                if (rdr.Read())
                {
                    lowerID = rdr.GetInt32(0);
                    lowerTaxis = rdr.GetInt32(1);
                }
                rdr.Close();
            }

            var selectedTaxis = GetTaxis(resourceID);

            if (lowerID > 0)
            {
                SetTaxis(resourceID, lowerTaxis);
                SetTaxis(lowerID, selectedTaxis);
                return true;
            }
            return false;
        }

        private int GetMaxTaxis(int keywordID)
        {
            string sqlString = $"SELECT MAX(Taxis) FROM wx_KeywordResource WHERE KeywordID = {keywordID}";
            return BaiRongDataProvider.DatabaseDao.GetIntResult(sqlString);
        }

        private int GetTaxis(int resourceID)
        {
            string sqlString = $"SELECT Taxis FROM wx_KeywordResource WHERE ResourceID = {resourceID}";
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

        private void SetTaxis(int resourceID, int taxis)
        {
            string sqlString = $"UPDATE wx_KeywordResource SET Taxis = {taxis} WHERE ResourceID = {resourceID}";
            ExecuteNonQuery(sqlString);
        }
        public List<KeywordResourceInfo> GetKeywordResourceInfoList(int publishmentSystemID,int keywordID)
        {
            var list = new List<KeywordResourceInfo>();

            var parms = new IDataParameter[]
			{
                GetParameter(PARM_PUBLISHMENT_SYSTEM_ID, EDataType.Integer, publishmentSystemID),
				GetParameter(PARM_KEYWORD_ID, EDataType.Integer, keywordID)
			};

            using (var rdr = ExecuteReader(SQL_SELECT_ALL, parms))
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