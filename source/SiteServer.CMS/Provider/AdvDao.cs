using System.Collections;
using System.Data;
using System.Text;
using BaiRong.Core.Data;
using SiteServer.CMS.Model;
using SiteServer.CMS.Model.Enumerations;
using SiteServer.Plugin;
using SiteServer.Plugin.Models;

namespace SiteServer.CMS.Provider
{
    public class AdvDao : DataProviderBase
    {
        private const string SqlInsertAdv = "INSERT INTO siteserver_Adv (PublishmentSystemID,AdAreaID,AdvName,Summary, IsEnabled, IsDateLimited, StartDate, EndDate,LevelType,Level,IsWeight,Weight,RotateType,RotateInterval,NodeIDCollectionToChannel,NodeIDCollectionToContent,FileTemplateIDCollection) VALUES (@PublishmentSystemID,@AdAreaID,@AdvName, @Summary, @IsEnabled, @IsDateLimited, @StartDate, @EndDate,@LevelType,@Level,@IsWeight,@Weight,@RotateType,@RotateInterval,@NodeIDCollectionToChannel,@NodeIDCollectionToContent,@FileTemplateIDCollection)";

        private const string SqlUpdateAdv = "UPDATE siteserver_Adv SET AdAreaID=@AdAreaID,AdvName=@AdvName, Summary = @Summary,IsEnabled = @IsEnabled, IsDateLimited = @IsDateLimited, StartDate = @StartDate, EndDate = @EndDate,LevelType=@LevelType,Level=@Level,IsWeight=@IsWeight,Weight=@Weight,RotateType=@RotateType,RotateInterval=@RotateInterval,NodeIDCollectionToChannel=@NodeIDCollectionToChannel,NodeIDCollectionToContent=@NodeIDCollectionToContent,FileTemplateIDCollection=@FileTemplateIDCollection WHERE AdvID = @AdvID AND PublishmentSystemID = @PublishmentSystemID";

        private const string SqlDeleteAdv = "DELETE FROM siteserver_Adv WHERE AdvID = @AdvID AND PublishmentSystemID = @PublishmentSystemID";

        private const string SqlSelectAdv = "SELECT AdvID ,PublishmentSystemID,AdAreaID,AdvName,Summary, IsEnabled, IsDateLimited, StartDate, EndDate,LevelType,Level,IsWeight,Weight ,RotateType,RotateInterval,NodeIDCollectionToChannel,NodeIDCollectionToContent,FileTemplateIDCollection  FROM siteserver_Adv WHERE AdvID = @AdvID AND PublishmentSystemID = @PublishmentSystemID";

        private const string SqlSelectAdvName = "SELECT AdvName FROM siteserver_Adv WHERE AdvName = @AdvName AND PublishmentSystemID = @PublishmentSystemID";

        private const string SqlSelectAllAdv = "SELECT AdvID , PublishmentSystemID,AdAreaID,AdvName,Summary, IsEnabled, IsDateLimited, StartDate, EndDate,LevelType,Level,IsWeight,Weight ,RotateType,RotateInterval,NodeIDCollectionToChannel,NodeIDCollectionToContent,FileTemplateIDCollection  FROM siteserver_Adv WHERE PublishmentSystemID = @PublishmentSystemID ORDER BY StartDate DESC";

        private const string SqlSelectAllAdvByAdareaid = "SELECT AdvID , PublishmentSystemID,AdAreaID,AdvName,Summary, IsEnabled, IsDateLimited, StartDate, EndDate,LevelType,Level,IsWeight,Weight ,RotateType,RotateInterval,NodeIDCollectionToChannel,NodeIDCollectionToContent,FileTemplateIDCollection  FROM siteserver_Adv WHERE AdAreaID=@AdAreaID AND PublishmentSystemID = @PublishmentSystemID ORDER BY StartDate DESC";

        //Ad Attributes
        private const string ParmAdvId = "AdvID";
        private const string ParmPublishmentsystemid = "@PublishmentSystemID";
        private const string ParmAdvAreaid = "@AdAreaID";
        private const string ParmAdvName = "@AdvName";
        private const string ParmSummary = "@Summary";
        private const string ParmIsEnabled = "@IsEnabled";
        private const string ParmIsDateLimited = "@IsDateLimited";
        private const string ParmStartDate = "@StartDate";
        private const string ParmEndDate = "@EndDate";
        private const string ParmLevelType = "@LevelType";
        private const string ParmLevel = "@Level";
        private const string ParmIsWeight = "@IsWeight";
        private const string ParmWeight = "@Weight";
        private const string ParmRotateType = "@RotateType";
        private const string ParmRotateInterval = "@RotateInterval";
        private const string ParmNodeIdCollectionToChannel = "@NodeIDCollectionToChannel";
        private const string ParmNodeIdCollectionToContent = "@NodeIDCollectionToContent";
        private const string ParmFiletemplateIdCollection = "@FileTemplateIDCollection";

        public void Insert(AdvInfo advInfo)
        {
            var adParms = new IDataParameter[]
            {
                GetParameter(ParmPublishmentsystemid, DataType.Integer, advInfo.PublishmentSystemID),
                GetParameter(ParmAdvAreaid, DataType.Integer, advInfo.AdAreaID),
                GetParameter(ParmAdvName, DataType.NVarChar, 50, advInfo.AdvName),
                GetParameter(ParmSummary, DataType.Text, advInfo.Summary),
                GetParameter(ParmIsEnabled, DataType.VarChar, 18, advInfo.IsEnabled.ToString()),
                GetParameter(ParmIsDateLimited, DataType.VarChar, 18, advInfo.IsDateLimited.ToString()),
                GetParameter(ParmStartDate, DataType.DateTime, advInfo.StartDate),
                GetParameter(ParmEndDate, DataType.DateTime, advInfo.EndDate),
                GetParameter(ParmLevelType, DataType.NVarChar, 50,EAdvLevelTypeUtils.GetValue(advInfo.LevelType)),
                GetParameter(ParmLevel , DataType.Integer, advInfo.Level),
                GetParameter(ParmIsWeight, DataType.VarChar, 18, advInfo.IsWeight.ToString()),
                GetParameter(ParmWeight , DataType.Integer,advInfo.Weight ),
                GetParameter(ParmRotateType, DataType.NVarChar,50,EAdvRotateTypeUtils.GetValue(advInfo.RotateType)),
                GetParameter(ParmRotateInterval, DataType.Integer, advInfo.RotateInterval),
                GetParameter(ParmNodeIdCollectionToChannel, DataType.NVarChar, 4000, advInfo.NodeIDCollectionToChannel),
                GetParameter(ParmNodeIdCollectionToContent, DataType.NVarChar, 4000, advInfo.NodeIDCollectionToContent),
                GetParameter(ParmFiletemplateIdCollection, DataType.NVarChar,4000, advInfo.FileTemplateIDCollection)

            };

            ExecuteNonQuery(SqlInsertAdv, adParms);
        }

        public void Update(AdvInfo advInfo)
        {
            var adParms = new IDataParameter[]
            {
                GetParameter(ParmPublishmentsystemid, DataType.Integer, advInfo.PublishmentSystemID),
                GetParameter(ParmAdvAreaid, DataType.Integer, advInfo.AdAreaID),
                GetParameter(ParmAdvName, DataType.NVarChar, 50, advInfo.AdvName),
                GetParameter(ParmSummary, DataType.Text, advInfo.Summary),
                GetParameter(ParmIsEnabled, DataType.VarChar, 18, advInfo.IsEnabled.ToString()),
                GetParameter(ParmIsDateLimited, DataType.VarChar, 18, advInfo.IsDateLimited.ToString()),
                GetParameter(ParmStartDate, DataType.DateTime, advInfo.StartDate),
                GetParameter(ParmEndDate, DataType.DateTime, advInfo.EndDate),
                GetParameter(ParmLevelType, DataType.NVarChar, 50,EAdvLevelTypeUtils.GetValue(advInfo.LevelType)),
                GetParameter(ParmLevel , DataType.Integer, advInfo.Level),
                GetParameter(ParmIsWeight, DataType.VarChar, 18, advInfo.IsWeight.ToString()),
                GetParameter(ParmWeight , DataType.Integer,advInfo.Weight ),
                GetParameter(ParmRotateType, DataType.NVarChar,50,EAdvRotateTypeUtils.GetValue(advInfo.RotateType)),
                GetParameter(ParmRotateInterval, DataType.Integer, advInfo.RotateInterval),
                GetParameter(ParmNodeIdCollectionToChannel, DataType.NVarChar, 4000, advInfo.NodeIDCollectionToChannel),
                GetParameter(ParmNodeIdCollectionToContent, DataType.NVarChar, 4000, advInfo.NodeIDCollectionToContent),
                GetParameter(ParmFiletemplateIdCollection, DataType.NVarChar,4000, advInfo.FileTemplateIDCollection),
                GetParameter(ParmAdvId, DataType.Integer, advInfo.AdvID)

            };

            ExecuteNonQuery(SqlUpdateAdv, adParms);
        }

        public void Delete(int advId, int publishmentSystemId)
        {
            var parms = new IDataParameter[]
            {
                GetParameter(ParmAdvId, DataType.Integer,advId),
                GetParameter(ParmPublishmentsystemid, DataType.Integer, publishmentSystemId)
            };
            ExecuteNonQuery(SqlDeleteAdv, parms);
        }

        public void Delete(ArrayList advIdArrayList, int publishmentSystemId)
        {
            if (advIdArrayList.Count > 0)
            {
                foreach (int advId in advIdArrayList)
                {
                    Delete(advId, publishmentSystemId);
                }
            }
        }

        public AdvInfo GetAdvInfo(int advId, int publishmentSystemId)
        {
            AdvInfo advInfo = null;

            var parms = new IDataParameter[]
            {
                GetParameter(ParmAdvId, DataType.Integer,advId),
                GetParameter(ParmPublishmentsystemid, DataType.Integer, publishmentSystemId)
            };

            using (var rdr = ExecuteReader(SqlSelectAdv, parms))
            {
                if (rdr.Read())
                {
                    var i = 0;
                    advInfo = new AdvInfo(GetInt(rdr, i++), GetInt(rdr, i++), GetInt(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetBool(rdr, i++), GetBool(rdr, i++), GetDateTime(rdr, i++), GetDateTime(rdr, i++), EAdvLevelTypeUtils.GetEnumType(GetString(rdr, i++)), GetInt(rdr, i++), GetBool(rdr, i++), GetInt(rdr, i++), EAdvRotateTypeUtils.GetEnumType(GetString(rdr, i++)), GetInt(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetString(rdr, i));
                }
                rdr.Close();
            }

            return advInfo;
        }

        public bool IsExists(string adertName, int publishmentSystemId)
        {
            var exists = false;

            var parms = new IDataParameter[]
            {
                GetParameter(ParmAdvName, DataType.NVarChar, 50, adertName),
                GetParameter(ParmPublishmentsystemid, DataType.Integer, publishmentSystemId)
            };

            using (var rdr = ExecuteReader(SqlSelectAdvName, parms))
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
                GetParameter(ParmPublishmentsystemid, DataType.Integer, publishmentSystemId)
            };

            var enumerable = (IEnumerable)ExecuteReader(SqlSelectAllAdv, parms);
            return enumerable;
        }

        public IEnumerable GetDataSourceByAdAreaId(int adAreaId, int publishmentSystemId)
        {
            var parms = new IDataParameter[]
            {
                GetParameter(ParmAdvAreaid, DataType.Integer, adAreaId),
                GetParameter(ParmPublishmentsystemid, DataType.Integer, publishmentSystemId)
            };

            var enumerable = (IEnumerable)ExecuteReader(SqlSelectAllAdvByAdareaid, parms);
            return enumerable;
        }

        public ArrayList GetAdvNameArrayList(int publishmentSystemId)
        {
            var arraylist = new ArrayList();
            string sqlString = $"SELECT AdvName FROM siteserver_Adv WHERE PublishmentSystemID = {publishmentSystemId}";

            using (var rdr = ExecuteReader(sqlString))
            {
                while (rdr.Read())
                {
                    var advName = GetString(rdr, 0);
                    arraylist.Add(advName);
                }
                rdr.Close();
            }

            return arraylist;
        }

        public ArrayList GetAdvIdArrayList(int adAreaId, int publishmentSystemId)
        {
            var arraylist = new ArrayList();
            string sqlString =
                $"SELECT AdvID FROM siteserver_Adv WHERE PublishmentSystemID = {publishmentSystemId} AND AdAreaID={adAreaId}";

            using (var rdr = ExecuteReader(sqlString))
            {
                while (rdr.Read())
                {
                    var advId = GetInt(rdr, 0);
                    arraylist.Add(advId);
                }
                rdr.Close();
            }

            return arraylist;
        }

        public ArrayList GetAdvInfoArrayList(ETemplateType templateType, int adAreaId, int publishmentSystemId, int nodeId, int fileTemplateId)
        {
            var arraylist = new ArrayList();
            var strSql = new StringBuilder();
            strSql.AppendFormat(@"SELECT AdvID,PublishmentSystemID,AdAreaID,AdvName,Summary, IsEnabled, IsDateLimited, StartDate, EndDate,LevelType,Level,IsWeight,Weight ,RotateType,RotateInterval,NodeIDCollectionToChannel,NodeIDCollectionToContent,FileTemplateIDCollection FROM (SELECT * FROM siteserver_Adv WHERE AdAreaID ={0} AND PublishmentSystemID ={1}) AS ADV", adAreaId, publishmentSystemId);
            if (templateType == ETemplateType.IndexPageTemplate || templateType == ETemplateType.ChannelTemplate)
            {
                strSql.AppendFormat(" WHERE NodeIDCollectionToChannel='{0}' OR NodeIDCollectionToChannel LIKE '{0},%' OR NodeIDCollectionToChannel LIKE '%,{0}' OR NodeIDCollectionToChannel LIKE '%,{0},%'", nodeId.ToString());
            }
            else if (templateType == ETemplateType.ContentTemplate)
            {
                strSql.AppendFormat(" WHERE NodeIDCollectionToContent='{0}' OR NodeIDCollectionToContent LIKE '{0},%' OR NodeIDCollectionToContent LIKE '%,{0}' OR NodeIDCollectionToContent LIKE '%,{0},%'", nodeId.ToString());
            }
            else if (templateType == ETemplateType.FileTemplate)
            {
                strSql.AppendFormat(" WHERE FileTemplateIDCollection='{0}' OR FileTemplateIDCollection LIKE '{0},%' OR FileTemplateIDCollection LIKE '%,{0}' OR FileTemplateIDCollection LIKE '%,{0},%'", fileTemplateId);
            }

            strSql.AppendFormat(@" ORDER BY StartDate ASC");
            using (var rdr = ExecuteReader(strSql.ToString()))
            {
                while (rdr.Read())
                {
                    var i = 0;
                    var advInfo = new AdvInfo(GetInt(rdr, i++), GetInt(rdr, i++), GetInt(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetBool(rdr, i++), GetBool(rdr, i++), GetDateTime(rdr, i++), GetDateTime(rdr, i++), EAdvLevelTypeUtils.GetEnumType(GetString(rdr, i++)), GetInt(rdr, i++), GetBool(rdr, i++), GetInt(rdr, i++), EAdvRotateTypeUtils.GetEnumType(GetString(rdr, i++)), GetInt(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetString(rdr, i));
                    arraylist.Add(advInfo);
                }
                rdr.Close();
            }

            return arraylist;
        }

    }
}
