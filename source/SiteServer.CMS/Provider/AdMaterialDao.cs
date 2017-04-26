using System.Collections;
using System.Data;
using BaiRong.Core;
using BaiRong.Core.Data;
using BaiRong.Core.Model.Enumerations;
using SiteServer.CMS.Model;
using SiteServer.CMS.Model.Enumerations;

namespace SiteServer.CMS.Provider
{
    public class AdMaterialDao : DataProviderBase
	{
        private const string SqlInsertAdmaterial = "INSERT INTO siteserver_AdMaterial ( PublishmentSystemID,AdvID,AdMaterialName, AdMaterialType, Code, TextWord, TextLink, TextColor, TextFontSize, ImageUrl, ImageLink, ImageWidth, ImageHeight, ImageAlt,Weight ,IsEnabled) VALUES ( @PublishmentSystemID,@AdvID,@AdMaterialName, @AdMaterialType, @Code, @TextWord, @TextLink, @TextColor, @TextFontSize, @ImageUrl, @ImageLink, @ImageWidth, @ImageHeight, @ImageAlt,@Weight , @IsEnabled)";

        private const string SqlUpdateAdmaterial = "UPDATE siteserver_AdMaterial SET AdvID=@AdvID, AdMaterialName=@AdMaterialName, AdMaterialType = @AdMaterialType, Code = @Code, TextWord = @TextWord, TextLink = @TextLink, TextColor = @TextColor, TextFontSize = @TextFontSize, ImageUrl = @ImageUrl, ImageLink = @ImageLink, ImageWidth = @ImageWidth, ImageHeight = @ImageHeight, ImageAlt = @ImageAlt,Weight =@Weight , IsEnabled = @IsEnabled WHERE AdMaterialID = @AdMaterialID AND PublishmentSystemID = @PublishmentSystemID";

        private const string SqlDeleteAdmaterial = "DELETE FROM siteserver_AdMaterial WHERE AdMaterialID = @AdMaterialID AND PublishmentSystemID = @PublishmentSystemID";

        private const string SqlSelectAdmaterial = "SELECT AdMaterialID, PublishmentSystemID,AdvID,AdMaterialName, AdMaterialType, Code, TextWord, TextLink, TextColor, TextFontSize, ImageUrl, ImageLink, ImageWidth, ImageHeight, ImageAlt,Weight , IsEnabled  FROM siteserver_AdMaterial WHERE AdMaterialID = @AdMaterialID AND PublishmentSystemID = @PublishmentSystemID";

        private const string SqlSelectAdmaterialName = "SELECT AdMaterialName FROM siteserver_AdMaterial WHERE AdMaterialName = @AdMaterialName AND PublishmentSystemID = @PublishmentSystemID";

        private const string SqlSelectAllAdmaterial = "SELECT AdMaterialID, PublishmentSystemID,AdvID,AdMaterialName, AdMaterialType, Code, TextWord, TextLink, TextColor, TextFontSize, ImageUrl, ImageLink, ImageWidth, ImageHeight, ImageAlt,Weight , IsEnabled  FROM siteserver_AdMaterial WHERE AdvID=@AdvID AND PublishmentSystemID = @PublishmentSystemID ORDER BY AdMaterialID DESC";

        private const string SqlSelectAllAdmaterialByType = "SELECT AdMaterialID, PublishmentSystemID,AdvID,AdMaterialName, AdMaterialType, Code, TextWord, TextLink, TextColor, TextFontSize, ImageUrl, ImageLink, ImageWidth, ImageHeight, ImageAlt,Weight , IsEnabled  FROM siteserver_AdMaterial WHERE AdMaterialType = @AdMaterialType AND PublishmentSystemID = @PublishmentSystemID ORDER BY AdMaterialID DESC ";

        private const string SqlSelectAllAdmaterialByAdverid = "SELECT AdMaterialID, PublishmentSystemID,AdvID,AdMaterialName, AdMaterialType, Code, TextWord, TextLink, TextColor, TextFontSize, ImageUrl, ImageLink, ImageWidth, ImageHeight, ImageAlt,Weight , IsEnabled  FROM siteserver_AdMaterial WHERE AdvID = @AdvID AND PublishmentSystemID = @PublishmentSystemID ORDER BY AdMaterialID DESC ";	

		//Ad Attributes
        private const string ParmAdmaterialId = "@AdMaterialID";
        private const string ParmAdmaterialName = "@AdMaterialName";
		private const string ParmPublishmentsystemid = "@PublishmentSystemID";
        private const string ParmAdvertId = "@AdvID";
        private const string ParmAdmaterialType = "@AdMaterialType";
        private const string ParmCode = "@Code";
        private const string ParmTextWord = "@TextWord";
        private const string ParmTextLink = "@TextLink";
        private const string ParmTextColor = "@TextColor";
        private const string ParmTextFontSize = "@TextFontSize";
        private const string ParmImageUrl = "@ImageUrl";
        private const string ParmImageLink = "@ImageLink";
        private const string ParmImageWidth = "@ImageWidth";
        private const string ParmImageHeight = "@ImageHeight";
        private const string ParmImageAlt = "@ImageAlt";
        private const string ParmWeight  = "@Weight";
        private const string ParmIsEnabled = "@IsEnabled";

        public void Insert(AdMaterialInfo adMaterialInfo) 
		{
			var adParms = new IDataParameter[]
			{
				GetParameter(ParmAdmaterialName, EDataType.NVarChar, 50, adMaterialInfo.AdMaterialName),
				GetParameter(ParmPublishmentsystemid, EDataType.Integer, adMaterialInfo.PublishmentSystemID),
                GetParameter(ParmAdvertId,EDataType.Integer,adMaterialInfo.AdvID),
				GetParameter(ParmAdmaterialType, EDataType.VarChar, 50, EAdvTypeUtils.GetValue(adMaterialInfo.AdMaterialType)),
                GetParameter(ParmCode, EDataType.NText, adMaterialInfo.Code),
                GetParameter(ParmTextWord, EDataType.NVarChar, 255, adMaterialInfo.TextWord),
                GetParameter(ParmTextLink, EDataType.VarChar, 200, adMaterialInfo.TextLink),
                GetParameter(ParmTextColor, EDataType.VarChar, 10, adMaterialInfo.TextColor),
                GetParameter(ParmTextFontSize, EDataType.Integer, adMaterialInfo.TextFontSize),
                GetParameter(ParmImageUrl, EDataType.VarChar, 200, adMaterialInfo.ImageUrl),
                GetParameter(ParmImageLink, EDataType.VarChar, 200, adMaterialInfo.ImageLink),
                GetParameter(ParmImageWidth, EDataType.Integer, adMaterialInfo.ImageWidth),
                GetParameter(ParmImageHeight, EDataType.Integer, adMaterialInfo.ImageHeight),
                GetParameter(ParmImageAlt, EDataType.NVarChar, 50, adMaterialInfo.ImageAlt),
                GetParameter(ParmWeight , EDataType.Integer, adMaterialInfo.Weight ),
                GetParameter(ParmIsEnabled, EDataType.VarChar, 18, adMaterialInfo.IsEnabled.ToString())
			 
			};

            ExecuteNonQuery(SqlInsertAdmaterial, adParms);
		}

        public void Update(AdMaterialInfo adMaterialInfo)
		{
			var adParms = new IDataParameter[]
			{
                GetParameter(ParmAdvertId,EDataType.Integer,adMaterialInfo.AdvID),
				GetParameter(ParmAdmaterialName, EDataType.NVarChar, 50, adMaterialInfo.AdMaterialName),
				GetParameter(ParmAdmaterialType, EDataType.VarChar, 50, EAdvTypeUtils.GetValue(adMaterialInfo.AdMaterialType)),
                GetParameter(ParmCode, EDataType.NText, adMaterialInfo.Code),
                GetParameter(ParmTextWord, EDataType.NVarChar, 255, adMaterialInfo.TextWord),
                GetParameter(ParmTextLink, EDataType.VarChar, 200, adMaterialInfo.TextLink),
                GetParameter(ParmTextColor, EDataType.VarChar, 10, adMaterialInfo.TextColor),
                GetParameter(ParmTextFontSize, EDataType.Integer, adMaterialInfo.TextFontSize),
                GetParameter(ParmImageUrl, EDataType.VarChar, 200, adMaterialInfo.ImageUrl),
                GetParameter(ParmImageLink, EDataType.VarChar, 200, adMaterialInfo.ImageLink),
                GetParameter(ParmImageWidth, EDataType.Integer, adMaterialInfo.ImageWidth),
                GetParameter(ParmImageHeight, EDataType.Integer, adMaterialInfo.ImageHeight),
                GetParameter(ParmImageAlt, EDataType.NVarChar, 50, adMaterialInfo.ImageAlt),
                GetParameter(ParmWeight , EDataType.Integer, adMaterialInfo.Weight ),
                GetParameter(ParmIsEnabled, EDataType.VarChar, 18, adMaterialInfo.IsEnabled.ToString()),
                GetParameter(ParmAdmaterialId, EDataType.Integer, adMaterialInfo.AdMaterialID),
                GetParameter(ParmPublishmentsystemid, EDataType.Integer, adMaterialInfo.PublishmentSystemID),
			};

            ExecuteNonQuery(SqlUpdateAdmaterial, adParms);
		}

        public void Delete(int adMaterialId, int publishmentSystemId)
		{
			var parms = new IDataParameter[]
			{
				GetParameter(ParmAdmaterialId, EDataType.Integer, adMaterialId),
				GetParameter(ParmPublishmentsystemid, EDataType.Integer, publishmentSystemId)
			};

            ExecuteNonQuery(SqlDeleteAdmaterial, parms);
		}

        public void Delete(ArrayList adMaterialIdArrarList, int publishmentSystemId)
        {
            if (adMaterialIdArrarList.Count > 0)
            {
                string strSql =
                    $@"DELETE FROM siteserver_AdMaterial WHERE AdMaterialID IN ({TranslateUtils
                        .ToSqlInStringWithoutQuote(adMaterialIdArrarList)}) AND PublishmentSystemID={publishmentSystemId}";

                ExecuteNonQuery(strSql);
            }
        }

        public AdMaterialInfo GetAdMaterialInfo(int adMaterialD, int publishmentSystemId)
		{
            AdMaterialInfo adMaterialInfo = null;

			var parms = new IDataParameter[]
			{
				GetParameter(ParmAdmaterialId, EDataType.Integer, adMaterialD),
				GetParameter(ParmPublishmentsystemid, EDataType.Integer, publishmentSystemId)
			};

            using (var rdr = ExecuteReader(SqlSelectAdmaterial, parms)) 
			{
				if (rdr.Read())
				{
				    var i = 0;
                    adMaterialInfo = new AdMaterialInfo(GetInt(rdr, i++), GetInt(rdr, i++), GetInt(rdr, i++), GetString(rdr, i++), EAdvTypeUtils.GetEnumType(GetString(rdr, i++)), GetString(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetInt(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetInt(rdr, i++), GetInt(rdr, i++), GetString(rdr, i++), GetInt(rdr, i++), GetBool(rdr, i));
				}
				rdr.Close();
			}

            return adMaterialInfo;
		}

		public bool IsExists(string adMaterialName, int publishmentSystemId)
		{
			var exists = false;

			var parms = new IDataParameter[]
			{
				GetParameter(ParmAdmaterialName, EDataType.NVarChar, 50, adMaterialName),
				GetParameter(ParmPublishmentsystemid, EDataType.Integer, publishmentSystemId)
			};

            using (var rdr = ExecuteReader(SqlSelectAdmaterialName, parms)) 
			{
				if (rdr.Read()) 
				{					
					exists = true;
				}
				rdr.Close();
			}

			return exists;
		}

		public IEnumerable GetDataSource(int advertId, int publishmentSystemId)
		{
			var parms = new IDataParameter[]
			{
                 GetParameter(ParmAdvertId,EDataType.Integer,advertId),
				GetParameter(ParmPublishmentsystemid, EDataType.Integer, publishmentSystemId)
			};

            var enumerable = (IEnumerable)ExecuteReader(SqlSelectAllAdmaterial, parms);
			return enumerable;
		}

		public IEnumerable GetDataSourceByType(EAdvType adType, int publishmentSystemId)
		{
			var parms = new IDataParameter[]
			{
				GetParameter(ParmAdmaterialType, EDataType.VarChar, 50, EAdvTypeUtils.GetValue(adType)),
				GetParameter(ParmPublishmentsystemid, EDataType.Integer, publishmentSystemId)
			};

			var enumerable = (IEnumerable)ExecuteReader(SqlSelectAllAdmaterialByType, parms);
			return enumerable;
		}

		public ArrayList GetAdMaterialNameArrayList(int publishmentSystemId)
		{
			var arraylist = new ArrayList();
            string sqlString =
                $"SELECT AdMaterialName FROM siteserver_AdMaterial WHERE PublishmentSystemID = {publishmentSystemId}";

            using (var rdr = ExecuteReader(sqlString))
            {
                while (rdr.Read())
                {
                    var adMaterialName = GetString(rdr, 0);
                    arraylist.Add(adMaterialName);
                }
                rdr.Close();
            }

			return arraylist;
		}

        public ArrayList GetAdMaterialIdArrayList( int advertId,int publishmentSystemId)
        {
            var arraylist = new ArrayList();
            string sqlString =
                $"SELECT AdMaterialID FROM siteserver_AdMaterial WHERE PublishmentSystemID = {publishmentSystemId} AND AdvID={advertId}";

            using (var rdr = ExecuteReader(sqlString))
            {
                while (rdr.Read())
                {
                    var adMaterialId = GetInt(rdr, 0);
                    arraylist.Add(adMaterialId);
                }
                rdr.Close();
            }

            return arraylist;
        }

        public ArrayList GetAdMaterialInfoArrayList(int advertId, int publishmentSystemId)
        {
            var arraylist = new ArrayList();
            var parms = new IDataParameter[]
			{
                GetParameter(ParmAdvertId,EDataType.Integer,advertId),
				GetParameter(ParmPublishmentsystemid, EDataType.Integer, publishmentSystemId)
			};

            using (var rdr = ExecuteReader(SqlSelectAllAdmaterialByAdverid, parms))
            {
                while (rdr.Read())
                {
                    var i = 0;
                    var adMaterialInfo = new AdMaterialInfo(GetInt(rdr, i++), GetInt(rdr, i++), GetInt(rdr, i++), GetString(rdr, i++), EAdvTypeUtils.GetEnumType(GetString(rdr, i++)), GetString(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetInt(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetInt(rdr, i++), GetInt(rdr, i++), GetString(rdr, i++), GetInt(rdr, i++), GetBool(rdr, i));
                    arraylist.Add(adMaterialInfo);
                }
                rdr.Close();
            }

            return arraylist;
        }
	}
}
