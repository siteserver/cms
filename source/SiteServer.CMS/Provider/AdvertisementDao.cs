using System.Collections;
using System.Data;
using BaiRong.Core;
using BaiRong.Core.Data;
using BaiRong.Core.Model.Enumerations;
using SiteServer.CMS.Core.Advertisement;
using SiteServer.CMS.Model;
using SiteServer.CMS.Model.Enumerations;

namespace SiteServer.CMS.Provider
{
	public class AdvertisementDao : DataProviderBase
	{
		// Static constants
        private const string SqlInsertAd = "INSERT INTO siteserver_Advertisement (AdvertisementName, PublishmentSystemID, AdvertisementType, IsDateLimited, StartDate, EndDate, AddDate, NodeIDCollectionToChannel, NodeIDCollectionToContent, FileTemplateIDCollection, Settings) VALUES (@AdvertisementName, @PublishmentSystemID, @AdvertisementType, @IsDateLimited, @StartDate, @EndDate, @AddDate, @NodeIDCollectionToChannel, @NodeIDCollectionToContent, @FileTemplateIDCollection, @Settings)";

        private const string SqlUpdateAd = "UPDATE siteserver_Advertisement SET AdvertisementType = @AdvertisementType, IsDateLimited = @IsDateLimited, StartDate = @StartDate, EndDate = @EndDate, NodeIDCollectionToChannel = @NodeIDCollectionToChannel, NodeIDCollectionToContent = @NodeIDCollectionToContent, FileTemplateIDCollection = @FileTemplateIDCollection, Settings = @Settings WHERE AdvertisementName = @AdvertisementName AND PublishmentSystemID = @PublishmentSystemID";

		private const string SqlDeleteAd = "DELETE FROM siteserver_Advertisement WHERE AdvertisementName = @AdvertisementName AND PublishmentSystemID = @PublishmentSystemID";

        private const string SqlSelectAd = "SELECT AdvertisementName, PublishmentSystemID, AdvertisementType, IsDateLimited, StartDate, EndDate, AddDate, NodeIDCollectionToChannel, NodeIDCollectionToContent, FileTemplateIDCollection, Settings FROM siteserver_Advertisement WHERE AdvertisementName = @AdvertisementName AND PublishmentSystemID = @PublishmentSystemID";

		private const string SqlSelectAdName = "SELECT AdvertisementName FROM siteserver_Advertisement WHERE AdvertisementName = @AdvertisementName AND PublishmentSystemID = @PublishmentSystemID";

		private const string SqlSelectAdType = "SELECT AdvertisementType FROM siteserver_Advertisement WHERE AdvertisementName = @AdvertisementName AND PublishmentSystemID = @PublishmentSystemID";

        private const string SqlSelectAllAd = "SELECT AdvertisementName, PublishmentSystemID, AdvertisementType, IsDateLimited, StartDate, EndDate, AddDate, NodeIDCollectionToChannel, NodeIDCollectionToContent, FileTemplateIDCollection, Settings FROM siteserver_Advertisement WHERE PublishmentSystemID = @PublishmentSystemID ORDER BY AddDate DESC";

        private const string SqlSelectAllAdByType = "SELECT AdvertisementName, PublishmentSystemID, AdvertisementType, IsDateLimited, StartDate, EndDate, AddDate, NodeIDCollectionToChannel, NodeIDCollectionToContent, FileTemplateIDCollection, Settings FROM siteserver_Advertisement WHERE AdvertisementType = @AdvertisementType AND PublishmentSystemID = @PublishmentSystemID ORDER BY AddDate DESC";	

		//Advertisement Attributes
		private const string ParmAdName = "@AdvertisementName";
		private const string ParmPublishmentsystemid = "@PublishmentSystemID";
		private const string ParmAdType = "@AdvertisementType";
		private const string ParmIsDateLimited = "@IsDateLimited";
		private const string ParmStartDate = "@StartDate";
		private const string ParmEndDate = "@EndDate";
		private const string ParmAddDate = "@AddDate";
        private const string ParmNodeIdCollectionToChannel = "@NodeIDCollectionToChannel";
        private const string ParmNodeIdCollectionToContent = "@NodeIDCollectionToContent";
        private const string ParmFileTemplateIdCollection = "@FileTemplateIDCollection";
        private const string ParmSettings = "@Settings";

		public void Insert(AdvertisementInfo adInfo) 
		{
			var adParms = new IDataParameter[]
			{
				GetParameter(ParmAdName, EDataType.VarChar, 50, adInfo.AdvertisementName),
				GetParameter(ParmPublishmentsystemid, EDataType.Integer, adInfo.PublishmentSystemID),
				GetParameter(ParmAdType, EDataType.VarChar, 50, EAdvertisementTypeUtils.GetValue(adInfo.AdvertisementType)),
				GetParameter(ParmIsDateLimited, EDataType.VarChar, 18, adInfo.IsDateLimited.ToString()),
				GetParameter(ParmStartDate, EDataType.DateTime, adInfo.StartDate),
				GetParameter(ParmEndDate, EDataType.DateTime, adInfo.EndDate),
				GetParameter(ParmAddDate, EDataType.DateTime, adInfo.AddDate),
				GetParameter(ParmNodeIdCollectionToChannel, EDataType.NVarChar, 255, adInfo.NodeIDCollectionToChannel),
                GetParameter(ParmNodeIdCollectionToContent, EDataType.NVarChar, 255, adInfo.NodeIDCollectionToContent),
                GetParameter(ParmFileTemplateIdCollection, EDataType.NVarChar, 255, adInfo.FileTemplateIDCollection),
                GetParameter(ParmSettings, EDataType.NText, adInfo.Settings)
			};

            ExecuteNonQuery(SqlInsertAd, adParms);
            AdvertisementManager.RemoveCache(adInfo.PublishmentSystemID);
		}

		public void Update(AdvertisementInfo adInfo)
		{
			var adParms = new IDataParameter[]
			{
				GetParameter(ParmAdType, EDataType.VarChar, 50, EAdvertisementTypeUtils.GetValue(adInfo.AdvertisementType)),
				GetParameter(ParmIsDateLimited, EDataType.VarChar, 18, adInfo.IsDateLimited.ToString()),
				GetParameter(ParmStartDate, EDataType.DateTime, adInfo.StartDate),
				GetParameter(ParmEndDate, EDataType.DateTime, adInfo.EndDate),
				GetParameter(ParmNodeIdCollectionToChannel, EDataType.NVarChar, 255, adInfo.NodeIDCollectionToChannel),
                GetParameter(ParmNodeIdCollectionToContent, EDataType.NVarChar, 255, adInfo.NodeIDCollectionToContent),
                GetParameter(ParmFileTemplateIdCollection, EDataType.NVarChar, 255, adInfo.FileTemplateIDCollection),
                GetParameter(ParmSettings, EDataType.NText, adInfo.Settings),
				GetParameter(ParmAdName, EDataType.VarChar, 50, adInfo.AdvertisementName),
				GetParameter(ParmPublishmentsystemid, EDataType.Integer, adInfo.PublishmentSystemID)
			};

            ExecuteNonQuery(SqlUpdateAd, adParms);

            AdvertisementManager.RemoveCache(adInfo.PublishmentSystemID);
		}

		public void Delete(string advertisementName, int publishmentSystemId)
		{
			var parms = new IDataParameter[]
			{
				GetParameter(ParmAdName, EDataType.VarChar, 50, advertisementName),
				GetParameter(ParmPublishmentsystemid, EDataType.Integer, publishmentSystemId)
			};

            ExecuteNonQuery(SqlDeleteAd, parms);
            AdvertisementManager.RemoveCache(publishmentSystemId);
		}

		public AdvertisementInfo GetAdvertisementInfo(string advertisementName, int publishmentSystemId)
		{
			AdvertisementInfo adInfo = null;

			var parms = new IDataParameter[]
			{
				GetParameter(ParmAdName, EDataType.VarChar, 50, advertisementName),
				GetParameter(ParmPublishmentsystemid, EDataType.Integer, publishmentSystemId)
			};
			
			using (var rdr = ExecuteReader(SqlSelectAd, parms)) 
			{
				if (rdr.Read())
				{
				    var i = 0;
                    adInfo = new AdvertisementInfo(GetString(rdr, i++), GetInt(rdr, i++), EAdvertisementTypeUtils.GetEnumType(GetString(rdr, i++)), GetBool(rdr, i++), GetDateTime(rdr, i++), GetDateTime(rdr, i++), GetDateTime(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetString(rdr, i));
				}
				rdr.Close();
			}

			return adInfo;
		}

		public EAdvertisementType GetAdvertisementType(string advertisementName, int publishmentSystemId)
		{
			var adType = EAdvertisementType.FloatImage;

			var parms = new IDataParameter[]
			{
				GetParameter(ParmAdName, EDataType.VarChar, 50, advertisementName),
				GetParameter(ParmPublishmentsystemid, EDataType.Integer, publishmentSystemId)
			};

			using (var rdr = ExecuteReader(SqlSelectAdType, parms)) 
			{
				if (rdr.Read()) 
				{
                    adType = EAdvertisementTypeUtils.GetEnumType(GetString(rdr, 0));
				}
				rdr.Close();
			}

			return adType;
		}

		public bool IsExists(string advertisementName, int publishmentSystemId)
		{
			var exists = false;

			var parms = new IDataParameter[]
			{
				GetParameter(ParmAdName, EDataType.VarChar, 50, advertisementName),
				GetParameter(ParmPublishmentsystemid, EDataType.Integer, publishmentSystemId)
			};
			
			using (var rdr = ExecuteReader(SqlSelectAdName, parms)) 
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

			var enumerable = (IEnumerable)ExecuteReader(SqlSelectAllAd, parms);
			return enumerable;
		}

		public IEnumerable GetDataSourceByType(EAdvertisementType advertisementType, int publishmentSystemId)
		{
			var parms = new IDataParameter[]
			{
				GetParameter(ParmAdType, EDataType.VarChar, 50, EAdvertisementTypeUtils.GetValue(advertisementType)),
				GetParameter(ParmPublishmentsystemid, EDataType.Integer, publishmentSystemId)
			};

			var enumerable = (IEnumerable)ExecuteReader(SqlSelectAllAdByType, parms);
			return enumerable;
		}

		public ArrayList GetAdvertisementNameArrayList(int publishmentSystemId)
		{
			var arraylist = new ArrayList();
            string sqlString =
                $"SELECT AdvertisementName FROM siteserver_Advertisement WHERE PublishmentSystemID = {publishmentSystemId}";

            using (var rdr = ExecuteReader(sqlString))
            {
                while (rdr.Read())
                {
                    var advertisementName = GetString(rdr, 0);
                    arraylist.Add(advertisementName);
                }
                rdr.Close();
            }

			return arraylist;
		}

        public ArrayList[] GetAdvertisementArrayLists(int publishmentSystemId)
        {
            string sqlString =
                $"SELECT NodeIDCollectionToChannel, NodeIDCollectionToContent, FileTemplateIDCollection FROM siteserver_Advertisement WHERE PublishmentSystemID = {publishmentSystemId}";

            var arraylist1 = new ArrayList();
            var arraylist2 = new ArrayList();
            var arraylist3 = new ArrayList();

            using (var rdr = ExecuteReader(sqlString))
            {
                while (rdr.Read())
                {
                    var collection1 = GetString(rdr, 0);
                    var collection2 = GetString(rdr, 1);
                    var collection3 = GetString(rdr, 2);

                    if (!string.IsNullOrEmpty(collection1))
                    {
                        var list = TranslateUtils.StringCollectionToIntList(collection1);
                        foreach (int id in list)
                        {
                            if (!arraylist1.Contains(id))
                            {
                                arraylist1.Add(id);
                            }
                        }
                    }

                    if (!string.IsNullOrEmpty(collection2))
                    {
                        var list = TranslateUtils.StringCollectionToIntList(collection2);
                        foreach (int id in list)
                        {
                            if (!arraylist2.Contains(id))
                            {
                                arraylist2.Add(id);
                            }
                        }
                    }

                    if (!string.IsNullOrEmpty(collection3))
                    {
                        var list = TranslateUtils.StringCollectionToIntList(collection3);
                        foreach (int id in list)
                        {
                            if (!arraylist3.Contains(id))
                            {
                                arraylist3.Add(id);
                            }
                        }
                    }
                }
                rdr.Close();
            }

            return new[] { arraylist1, arraylist2, arraylist3 };
        }
	}
}
