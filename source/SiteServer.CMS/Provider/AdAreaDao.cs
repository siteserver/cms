using System.Collections;
using System.Data;
using System.Text;
using BaiRong.Core;
using BaiRong.Core.Data;
using BaiRong.Core.Model.Enumerations;
using SiteServer.CMS.Model;

namespace SiteServer.CMS.Provider
{
    public class AdAreaDao : DataProviderBase
    {
        private const string SqlInsertAdarea = "INSERT INTO siteserver_AdArea ( PublishmentSystemID,AdAreaName, Width, Height, Summary, IsEnabled, AddDate) VALUES (@PublishmentSystemID, @AdAreaName, @Width, @Height, @Summary, @IsEnabled,@AddDate)";

        private const string SqlUpdateAdarea = "UPDATE siteserver_AdArea SET AdAreaName = @AdAreaName, Width = @Width,Height=@Height, Summary = @Summary, IsEnabled = @IsEnabled, AddDate = @AddDate WHERE AdAreaID = @AdAreaID AND PublishmentSystemID = @PublishmentSystemID";

        private const string SqlDeleteAdarea = "DELETE FROM siteserver_AdArea WHERE AdAreaName = @AdAreaName AND PublishmentSystemID = @PublishmentSystemID";

        private const string SqlSelectAdareaByname = "SELECT AdAreaID, PublishmentSystemID,AdAreaName, Width, Height, Summary, IsEnabled, AddDate FROM siteserver_AdArea WHERE AdAreaName = @AdAreaName AND PublishmentSystemID = @PublishmentSystemID";

        private const string SqlSelectAdareaByid = "SELECT AdAreaID, PublishmentSystemID,AdAreaName, Width, Height, Summary, IsEnabled, AddDate FROM siteserver_AdArea WHERE AdAreaID = @AdAreaID AND PublishmentSystemID = @PublishmentSystemID";

        private const string SqlSelectAdareaName = "SELECT AdAreaName FROM siteserver_AdArea WHERE AdAreaName = @AdAreaName AND PublishmentSystemID = @PublishmentSystemID";

        private const string SqlSelectAllAdarea = "SELECT AdAreaID, PublishmentSystemID,AdAreaName, Width, Height, Summary, IsEnabled,AddDate FROM siteserver_AdArea WHERE PublishmentSystemID = @PublishmentSystemID ORDER BY AddDate DESC";

        private const string ParmAdareaId = "@AdAreaID";
        private const string ParmPublishmentsystemid = "@PublishmentSystemID";
        private const string ParmAdareaName = "@AdAreaName";
        private const string ParmWidth = "@Width";
        private const string ParmHight = "@Height";
        private const string ParmSummary = "@Summary";
        private const string ParmIsEnabled = "@IsEnabled";
        private const string ParmAddDate = "@AddDate";

        public void Insert(AdAreaInfo adAreaInfo)
        {
            var adParms = new IDataParameter[]
			{ 
				GetParameter(ParmPublishmentsystemid, EDataType.Integer, adAreaInfo.PublishmentSystemID),
			    GetParameter(ParmAdareaName, EDataType.NVarChar,255, adAreaInfo.AdAreaName),
                GetParameter(ParmWidth, EDataType.Integer, adAreaInfo.Width),
                GetParameter(ParmHight, EDataType.Integer, adAreaInfo.Height),
                GetParameter(ParmSummary, EDataType.NVarChar, 255, adAreaInfo.Summary),
                GetParameter(ParmIsEnabled, EDataType.VarChar,18, adAreaInfo.IsEnabled.ToString()),
                GetParameter(ParmAddDate, EDataType.DateTime, adAreaInfo.AddDate)
              
			};

            ExecuteNonQuery(SqlInsertAdarea, adParms);
        }

        public void Update(AdAreaInfo adAreaInfo)
        {
            var adParms = new IDataParameter[]
			{
			    GetParameter(ParmAdareaName, EDataType.NVarChar,255, adAreaInfo.AdAreaName),
                GetParameter(ParmWidth, EDataType.Integer, adAreaInfo.Width),
                GetParameter(ParmHight, EDataType.Integer, adAreaInfo.Height),
                GetParameter(ParmSummary, EDataType.Text, 255, adAreaInfo.Summary),
                GetParameter(ParmIsEnabled, EDataType.VarChar,18, adAreaInfo.IsEnabled.ToString()),
                GetParameter(ParmAddDate, EDataType.DateTime, adAreaInfo.AddDate),
                GetParameter(ParmAdareaId, EDataType.Integer, adAreaInfo.AdAreaID),
				GetParameter(ParmPublishmentsystemid, EDataType.Integer, adAreaInfo.PublishmentSystemID)
			};

            ExecuteNonQuery(SqlUpdateAdarea, adParms);
        }

        public void Delete(string adAreaName, int publishmentSystemId)
        {
            var parms = new IDataParameter[]
			{
			    GetParameter(ParmAdareaName, EDataType.NVarChar,255, adAreaName),
				GetParameter(ParmPublishmentsystemid, EDataType.Integer, publishmentSystemId)
			};
            ExecuteNonQuery(SqlDeleteAdarea, parms);

        }

        public AdAreaInfo GetAdAreaInfo(string adAreaName, int publishmentSystemId)
        {
            AdAreaInfo adAreaInfo = null;

            var parms = new IDataParameter[]
			{
				GetParameter(ParmAdareaName, EDataType.NVarChar,255, adAreaName),
				GetParameter(ParmPublishmentsystemid, EDataType.Integer, publishmentSystemId)
			};

            using (var rdr = ExecuteReader(SqlSelectAdareaByname, parms))
            {
                if (rdr.Read())
                {
                    var i = 0;
                    adAreaInfo = new AdAreaInfo(GetInt(rdr, i++), GetInt(rdr, i++), GetString(rdr, i++), GetInt(rdr, i++), GetInt(rdr, i++), GetString(rdr, i++), GetBool(rdr, i++), GetDateTime(rdr, i));
                }
                rdr.Close();
            }

            return adAreaInfo;
        }

        public AdAreaInfo GetAdAreaInfo(int adAreaId, int publishmentSystemId)
        {
            AdAreaInfo adAreaInfo = null;

            var parms = new IDataParameter[]
			{
				GetParameter(ParmAdareaId, EDataType.Integer, adAreaId),
				GetParameter(ParmPublishmentsystemid, EDataType.Integer, publishmentSystemId)
			};

            using (var rdr = ExecuteReader(SqlSelectAdareaByid, parms))
            {
                if (rdr.Read())
                {
                    var i = 0;
                    adAreaInfo = new AdAreaInfo(GetInt(rdr, i++), GetInt(rdr, i++), GetString(rdr, i++), GetInt(rdr, i++), GetInt(rdr, i++), GetString(rdr, i++), GetBool(rdr, i++), GetDateTime(rdr, i));
                }
                rdr.Close();
            }

            return adAreaInfo;
        }

        public bool IsExists(string adAreaName, int publishmentSystemId)
        {
            var exists = false;

            var parms = new IDataParameter[]
			{
				GetParameter(ParmAdareaName, EDataType.NVarChar,255, adAreaName),
				GetParameter(ParmPublishmentsystemid, EDataType.Integer, publishmentSystemId)
			};

            using (var rdr = ExecuteReader(SqlSelectAdareaName, parms))
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
            var parms = new IDataParameter[]
			{
				GetParameter(ParmPublishmentsystemid, EDataType.Integer, publishmentSystemId)
			};

            var enumerable = (IEnumerable)ExecuteReader(SqlSelectAllAdarea, parms);
            return enumerable;
        }

        public IEnumerable GetDataSourceByName(string adAreaName, int publishmentSystemId)
        {
            var strSql = new StringBuilder();
            strSql.AppendFormat("SELECT AdAreaID, PublishmentSystemID,AdAreaName, Width, Height, Summary, IsEnabled,AddDate FROM siteserver_AdArea WHERE PublishmentSystemID ={0}", publishmentSystemId);
            if (!string.IsNullOrEmpty(adAreaName))
            {
                strSql.AppendFormat(" AND AdAreaName LIKE '%{0}%'", PageUtils.FilterSql(adAreaName));
            }
            strSql.Append("ORDER BY AddDate DESC");

            var enumerable = (IEnumerable)ExecuteReader(strSql.ToString());

            return enumerable;
        }

        public ArrayList GetAdAreaNameArrayList(int publishmentSystemId)
        {
            var arraylist = new ArrayList();
            string sqlString =
                $"SELECT AdAreaName FROM siteserver_AdArea WHERE PublishmentSystemID = {publishmentSystemId}";

            using (var rdr = ExecuteReader(sqlString))
            {
                while (rdr.Read())
                {
                    var adAreaName = GetString(rdr, 0);
                    arraylist.Add(adAreaName);
                }
                rdr.Close();
            }

            return arraylist;
        }

        public ArrayList GetAdAreaInfoArrayList(int publishmentSystemId)
        {
            var arraylist = new ArrayList();
            var parms = new IDataParameter[]
			{
				GetParameter(ParmPublishmentsystemid, EDataType.Integer, publishmentSystemId)
			};

            using (var rdr = ExecuteReader(SqlSelectAllAdarea, parms))
            {
                while (rdr.Read())
                {
                    var i = 0;
                    var adAreaInfo = new AdAreaInfo(GetInt(rdr, i++), GetInt(rdr, i++), GetString(rdr, i++), GetInt(rdr, i++), GetInt(rdr, i++), GetString(rdr, i++), GetBool(rdr, i++), GetDateTime(rdr, i));
                    arraylist.Add(adAreaInfo);
                }
                rdr.Close();
            }

            return arraylist;
        }
    }
}
