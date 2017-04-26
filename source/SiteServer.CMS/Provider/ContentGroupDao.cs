using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using BaiRong.Core;
using BaiRong.Core.Data;
using BaiRong.Core.Model.Enumerations;
using SiteServer.CMS.Model;

namespace SiteServer.CMS.Provider
{
    public class ContentGroupDao : DataProviderBase
    {
        private const string SqlInsertContentgroup = "INSERT INTO siteserver_ContentGroup (ContentGroupName, PublishmentSystemID, Taxis, Description) VALUES (@ContentGroupName, @PublishmentSystemID, @Taxis, @Description)";
        private const string SqlUpdateContentgroup = "UPDATE siteserver_ContentGroup SET Description = @Description WHERE ContentGroupName = @ContentGroupName AND PublishmentSystemID = @PublishmentSystemID";
        private const string SqlDeleteContentgroup = "DELETE FROM siteserver_ContentGroup WHERE ContentGroupName = @ContentGroupName AND PublishmentSystemID = @PublishmentSystemID";

        private const string ParmGroupName = "@ContentGroupName";
        private const string ParmPublishmentsystemid = "@PublishmentSystemID";
        private const string ParmTaxis = "@Taxis";
        private const string ParmDescription = "@Description";

        public void Insert(ContentGroupInfo contentGroup)
        {
            var maxTaxis = GetMaxTaxis(contentGroup.PublishmentSystemId);
            contentGroup.Taxis = maxTaxis + 1;

            var insertParms = new IDataParameter[]
			{
				GetParameter(ParmGroupName, EDataType.NVarChar, 255, contentGroup.ContentGroupName),
				GetParameter(ParmPublishmentsystemid, EDataType.Integer, contentGroup.PublishmentSystemId),
                GetParameter(ParmTaxis, EDataType.Integer, contentGroup.Taxis),
				GetParameter(ParmDescription, EDataType.NText, contentGroup.Description)
			};

            ExecuteNonQuery(SqlInsertContentgroup, insertParms);
        }

        public void Update(ContentGroupInfo contentGroup)
        {
            var updateParms = new IDataParameter[]
			{
				GetParameter(ParmDescription, EDataType.NText, contentGroup.Description),
				GetParameter(ParmGroupName, EDataType.NVarChar, 255, contentGroup.ContentGroupName),
				GetParameter(ParmPublishmentsystemid, EDataType.Integer, contentGroup.PublishmentSystemId)
			};

            ExecuteNonQuery(SqlUpdateContentgroup, updateParms);
        }

        public void Delete(string groupName, int publishmentSystemId)
        {
            var contentGroupParms = new IDataParameter[]
			{
				GetParameter(ParmGroupName, EDataType.NVarChar, 255, groupName),
				GetParameter(ParmPublishmentsystemid, EDataType.Integer, publishmentSystemId)
			};

            ExecuteNonQuery(SqlDeleteContentgroup, contentGroupParms);
        }

        public ContentGroupInfo GetContentGroupInfo(string groupName, int publishmentSystemId)
        {
            ContentGroupInfo contentGroup = null;

            string sqlString =
                $"SELECT ContentGroupName, PublishmentSystemID, Taxis, Description FROM siteserver_ContentGroup WHERE ContentGroupName = @ContentGroupName AND PublishmentSystemID = {publishmentSystemId}";

            var selectParms = new IDataParameter[]
			{
				GetParameter(ParmGroupName, EDataType.NVarChar, 255, groupName)			 
			};

            using (var rdr = ExecuteReader(sqlString, selectParms))
            {
                if (rdr.Read())
                {
                    var i = 0;
                    contentGroup = new ContentGroupInfo(GetString(rdr, i++), GetInt(rdr, i++), GetInt(rdr, i++), GetString(rdr, i));
                }
                rdr.Close();
            }

            return contentGroup;
        }

        public bool IsExists(string groupName, int publishmentSystemId)
        {
            var exists = false;

            string sqlString =
                $"SELECT ContentGroupName FROM siteserver_ContentGroup WHERE ContentGroupName = @ContentGroupName AND PublishmentSystemID = {publishmentSystemId}";

            var selectParms = new IDataParameter[]
			{
				GetParameter(ParmGroupName, EDataType.NVarChar, 255, groupName)			 
			};

            using (var rdr = ExecuteReader(sqlString, selectParms))
            {
                if (rdr.Read())
                {
                    exists = true;
                }
                rdr.Close();
            }

            return exists;
        }

        public IEnumerable GetDataSource(int publishmentSystemId)
        {
            string sqlString =
                $"SELECT ContentGroupName, PublishmentSystemID, Taxis, Description FROM siteserver_ContentGroup WHERE PublishmentSystemID = {publishmentSystemId} ORDER BY Taxis DESC, ContentGroupName";
            var enumerable = (IEnumerable)ExecuteReader(sqlString);
            return enumerable;
        }

        public List<ContentGroupInfo> GetContentGroupInfoList(int publishmentSystemId)
        {
            var list = new List<ContentGroupInfo>();
            string sqlString =
                $"SELECT ContentGroupName, PublishmentSystemID, Taxis, Description FROM siteserver_ContentGroup WHERE PublishmentSystemID = {publishmentSystemId} ORDER BY Taxis DESC, ContentGroupName";

            using (var rdr = ExecuteReader(sqlString))
            {
                while (rdr.Read())
                {
                    var i = 0;
                    list.Add(new ContentGroupInfo(GetString(rdr, i++), GetInt(rdr, i++), GetInt(rdr, i++), GetString(rdr, i)));
                }
                rdr.Close();
            }

            return list;
        }

        public List<string> GetContentGroupNameList(int publishmentSystemId)
        {
            var list = new List<string>();
            string sqlString =
                $"SELECT ContentGroupName FROM siteserver_ContentGroup WHERE PublishmentSystemID = {publishmentSystemId} ORDER BY Taxis DESC, ContentGroupName";

            using (var rdr = ExecuteReader(sqlString))
            {
                while (rdr.Read())
                {
                    list.Add(GetString(rdr, 0));
                }
                rdr.Close();
            }

            return list;
        }

        private int GetTaxis(int publishmentSystemId, string groupName)
        {
            string sqlString =
                $"SELECT Taxis FROM siteserver_ContentGroup WHERE (ContentGroupName = @ContentGroupName AND PublishmentSystemID = {publishmentSystemId})";

            var selectParms = new IDataParameter[]
			{
				GetParameter(ParmGroupName, EDataType.NVarChar, 255, groupName)			 
			};

            return BaiRongDataProvider.DatabaseDao.GetIntResult(sqlString, selectParms);
        }

        private void SetTaxis(int publishmentSystemId, string groupName, int taxis)
        {
            string sqlString =
                $"UPDATE siteserver_ContentGroup SET Taxis = {taxis} WHERE (ContentGroupName = @ContentGroupName AND PublishmentSystemID = {publishmentSystemId})";
            var updateParms = new IDataParameter[]
			{
				GetParameter(ParmGroupName, EDataType.NVarChar, 255, groupName)			 
			};
            ExecuteNonQuery(sqlString, updateParms);
        }

        private int GetMaxTaxis(int publishmentSystemId)
        {
            string sqlString =
                $"SELECT MAX(Taxis) FROM siteserver_ContentGroup WHERE (PublishmentSystemID = {publishmentSystemId})";
            var maxTaxis = 0;

            using (var rdr = ExecuteReader(sqlString))
            {
                if (rdr.Read())
                {
                    maxTaxis = GetInt(rdr, 0);
                }
                rdr.Close();
            }
            return maxTaxis;
        }

        public bool UpdateTaxisToUp(int publishmentSystemId, string groupName)
        {
            //Get Higher Taxis and ID
            //string sqlString =
            //    $"SELECT TOP 1 ContentGroupName, Taxis FROM siteserver_ContentGroup WHERE (Taxis > (SELECT Taxis FROM siteserver_ContentGroup WHERE ContentGroupName = @ContentGroupName AND PublishmentSystemID = {publishmentSystemId}) AND PublishmentSystemID = {publishmentSystemId}) ORDER BY Taxis";
            string sqlString = SqlUtils.GetTopSqlString("siteserver_ContentGroup", "ContentGroupName, Taxis", $"WHERE (Taxis > (SELECT Taxis FROM siteserver_ContentGroup WHERE ContentGroupName = @ContentGroupName AND PublishmentSystemID = {publishmentSystemId}) AND PublishmentSystemID = {publishmentSystemId}) ORDER BY Taxis", 1);

            var selectParms = new IDataParameter[]
			{
				GetParameter(ParmGroupName, EDataType.NVarChar, 255, groupName)			 
			};
            var higherGroupName = string.Empty;
            var higherTaxis = 0;

            using (var rdr = ExecuteReader(sqlString, selectParms))
            {
                if (rdr.Read())
                {
                    higherGroupName = GetString(rdr, 0);
                    higherTaxis = GetInt(rdr, 1);
                }
                rdr.Close();
            }

            if (!string.IsNullOrEmpty(higherGroupName))
            {
                //Get Taxis Of Selected ID
                var selectedTaxis = GetTaxis(publishmentSystemId, groupName);

                //Set The Selected Class Taxis To Higher Level
                SetTaxis(publishmentSystemId, groupName, higherTaxis);
                //Set The Higher Class Taxis To Lower Level
                SetTaxis(publishmentSystemId, higherGroupName, selectedTaxis);
                return true;
            }
            return false;
        }

        public bool UpdateTaxisToDown(int publishmentSystemId, string groupName)
        {
            //Get Lower Taxis and ID
            //string sqlString =
            //    $"SELECT TOP 1 ContentGroupName, Taxis FROM siteserver_ContentGroup WHERE (Taxis < (SELECT Taxis FROM siteserver_ContentGroup WHERE ContentGroupName = @ContentGroupName AND PublishmentSystemID = {publishmentSystemId}) AND PublishmentSystemID = {publishmentSystemId}) ORDER BY Taxis DESC";
            string sqlString = SqlUtils.GetTopSqlString("siteserver_ContentGroup", "ContentGroupName, Taxis", $"WHERE (Taxis < (SELECT Taxis FROM siteserver_ContentGroup WHERE ContentGroupName = @ContentGroupName AND PublishmentSystemID = {publishmentSystemId}) AND PublishmentSystemID = {publishmentSystemId}) ORDER BY Taxis DESC", 1);

            var selectParms = new IDataParameter[]
			{
				GetParameter(ParmGroupName, EDataType.NVarChar, 255, groupName)			 
			};
            var lowerGroupName = string.Empty;
            var lowerTaxis = 0;

            using (var rdr = ExecuteReader(sqlString, selectParms))
            {
                if (rdr.Read())
                {
                    lowerGroupName = GetString(rdr, 0);
                    lowerTaxis = GetInt(rdr, 1);
                }
                rdr.Close();
            }

            if (!string.IsNullOrEmpty(lowerGroupName))
            {
                //Get Taxis Of Selected Class
                var selectedTaxis = GetTaxis(publishmentSystemId, groupName);

                //Set The Selected Class Taxis To Lower Level
                SetTaxis(publishmentSystemId, groupName, lowerTaxis);
                //Set The Lower Class Taxis To Higher Level
                SetTaxis(publishmentSystemId, lowerGroupName, selectedTaxis);
                return true;
            }
            return false;
        }

    }
}