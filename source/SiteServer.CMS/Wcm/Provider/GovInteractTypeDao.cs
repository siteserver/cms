using System;
using System.Collections;
using System.Data;
using BaiRong.Core;
using BaiRong.Core.Data;
using BaiRong.Core.Model.Enumerations;
using SiteServer.CMS.Wcm.Model;

namespace SiteServer.CMS.Wcm.Provider
{
    public class GovInteractTypeDao : DataProviderBase 
	{
        private const string SqlUpdate = "UPDATE wcm_GovInteractType SET TypeName = @TypeName WHERE TypeID = @TypeID";

        private const string SqlDelete = "DELETE FROM wcm_GovInteractType WHERE TypeID = @TypeID";

        private const string SqlSelect = "SELECT TypeID, TypeName, NodeID, PublishmentSystemID, Taxis FROM wcm_GovInteractType WHERE TypeID = @TypeID";

        private const string SqlSelectName = "SELECT TypeName FROM wcm_GovInteractType WHERE TypeID = @TypeID";

        private const string SqlSelectAll = "SELECT TypeID, TypeName, NodeID, PublishmentSystemID, Taxis FROM wcm_GovInteractType WHERE NodeID = @NodeID ORDER BY Taxis";

        private const string SqlSelectInteractName = "SELECT TypeName FROM wcm_GovInteractType WHERE NodeID = @NodeID ORDER BY Taxis";

        private const string ParmTypeId = "@TypeID";
        private const string ParmInteractName = "@TypeName";
        private const string ParmNodeId = "@NodeID";
        private const string ParmPublishmentsystemid = "@PublishmentSystemID";
        private const string ParmTaxis = "@Taxis";

		public void Insert(GovInteractTypeInfo typeInfo) 
		{
            var sqlString = "INSERT INTO wcm_GovInteractType (TypeName, NodeID, PublishmentSystemID, Taxis) VALUES (@TypeName, @NodeID, @PublishmentSystemID, @Taxis)";

            var taxis = GetMaxTaxis(typeInfo.NodeID) + 1;
			var parms = new IDataParameter[]
			{
				GetParameter(ParmInteractName, EDataType.NVarChar, 50, typeInfo.TypeName),
				GetParameter(ParmNodeId, EDataType.Integer, typeInfo.NodeID),
                GetParameter(ParmPublishmentsystemid, EDataType.Integer, typeInfo.PublishmentSystemID),
                GetParameter(ParmTaxis, EDataType.Integer, taxis)
			};

            ExecuteNonQuery(sqlString, parms);
		}

        public void Update(GovInteractTypeInfo typeInfo) 
		{
			var parms = new IDataParameter[]
			{
                GetParameter(ParmInteractName, EDataType.NVarChar, 50, typeInfo.TypeName),
				GetParameter(ParmTypeId, EDataType.Integer, typeInfo.TypeID),
			};

            ExecuteNonQuery(SqlUpdate, parms);
		}

		public void Delete(int typeId)
		{
            var parms = new IDataParameter[]
			{
				GetParameter(ParmTypeId, EDataType.Integer, typeId)
			};

            ExecuteNonQuery(SqlDelete, parms);
		}

        public GovInteractTypeInfo GetTypeInfo(int typeId)
		{
            GovInteractTypeInfo typeInfo = null;

            var parms = new IDataParameter[]
			{
				GetParameter(ParmTypeId, EDataType.Integer, typeId)
			};

            using (var rdr = ExecuteReader(SqlSelect, parms))
            {
                if (rdr.Read())
                {
                    var i = 0;
                    typeInfo = new GovInteractTypeInfo(GetInt(rdr, i++), GetString(rdr, i++), GetInt(rdr, i++), GetInt(rdr, i++), GetInt(rdr, i));
                }
                rdr.Close();
            }

            return typeInfo;
		}

        public string GetTypeName(int typeId)
        {
            var typeName = string.Empty;

            var parms = new IDataParameter[]
			{
				GetParameter(ParmTypeId, EDataType.Integer, typeId)
			};

            using (var rdr = ExecuteReader(SqlSelectName, parms))
            {
                if (rdr.Read())
                {
                    typeName = GetString(rdr, 0);
                }
                rdr.Close();
            }

            return typeName;
        }

        public IEnumerable GetDataSource(int nodeId)
		{
            var parms = new IDataParameter[]
			{
				GetParameter(ParmNodeId, EDataType.Integer, nodeId)
			};
            var enumerable = (IEnumerable)ExecuteReader(SqlSelectAll, parms);
			return enumerable;
		}

        public ArrayList GetTypeInfoArrayList(int nodeId)
        {
            var arraylist = new ArrayList();

            var parms = new IDataParameter[]
			{
				GetParameter(ParmNodeId, EDataType.Integer, nodeId)
			};

            using (var rdr = ExecuteReader(SqlSelectAll, parms))
            {
                while (rdr.Read())
                {
                    var i = 0;
                    var typeInfo = new GovInteractTypeInfo(GetInt(rdr, i++), GetString(rdr, i++), GetInt(rdr, i++), GetInt(rdr, i++), GetInt(rdr, i));
                    arraylist.Add(typeInfo);
                }
                rdr.Close();
            }

            return arraylist;
        }

        public ArrayList GetTypeNameArrayList(int nodeId)
        {
            var arraylist = new ArrayList();

            var selectParms = new IDataParameter[]
			{
				GetParameter(ParmNodeId, EDataType.Integer, nodeId)
			};

            using (var rdr = ExecuteReader(SqlSelectInteractName, selectParms))
            {
                while (rdr.Read())
                {
                    arraylist.Add(GetString(rdr, 0));
                }
                rdr.Close();
            }

            return arraylist;
        }

        public bool UpdateTaxisToUp(int typeId, int nodeId)
        {
            //string sqlString =
            //    $"SELECT TOP 1 TypeID, Taxis FROM wcm_GovInteractType WHERE ((Taxis > (SELECT Taxis FROM wcm_GovInteractType WHERE TypeID = {typeId})) AND NodeID ={nodeId}) ORDER BY Taxis";
            var sqlString = SqlUtils.GetTopSqlString("wcm_GovInteractType", "TypeID, Taxis", $"WHERE ((Taxis > (SELECT Taxis FROM wcm_GovInteractType WHERE TypeID = {typeId})) AND NodeID ={nodeId}) ORDER BY Taxis", 1);

            var higherId = 0;
            var higherTaxis = 0;

            using (var rdr = ExecuteReader(sqlString))
            {
                if (rdr.Read())
                {
                    higherId = GetInt(rdr, 0);
                    higherTaxis = GetInt(rdr, 1);
                }
                rdr.Close();
            }

            var selectedTaxis = GetTaxis(typeId);

            if (higherId > 0)
            {
                SetTaxis(typeId, nodeId, higherTaxis);
                SetTaxis(higherId, nodeId, selectedTaxis);
                return true;
            }
            return false;
        }

        public bool UpdateTaxisToDown(int typeId, int nodeId)
        {
            //string sqlString =
            //    $"SELECT TOP 1 TypeID, Taxis FROM wcm_GovInteractType WHERE ((Taxis < (SELECT Taxis FROM wcm_GovInteractType WHERE TypeID = {typeId})) AND NodeID = {nodeId}) ORDER BY Taxis DESC";
            var sqlString = SqlUtils.GetTopSqlString("wcm_GovInteractType", "TypeID, Taxis", $"WHERE ((Taxis < (SELECT Taxis FROM wcm_GovInteractType WHERE TypeID = {typeId})) AND NodeID = {nodeId}) ORDER BY Taxis DESC", 1);

            var lowerId = 0;
            var lowerTaxis = 0;

            using (var rdr = ExecuteReader(sqlString))
            {
                if (rdr.Read())
                {
                    lowerId = GetInt(rdr, 0);
                    lowerTaxis = GetInt(rdr, 1);
                }
                rdr.Close();
            }

            var selectedTaxis = GetTaxis(typeId);

            if (lowerId > 0)
            {
                SetTaxis(typeId, nodeId, lowerTaxis);
                SetTaxis(lowerId, nodeId, selectedTaxis);
                return true;
            }
            return false;
        }

        private int GetMaxTaxis(int nodeId)
        {
            string sqlString = $"SELECT MAX(Taxis) FROM wcm_GovInteractType WHERE NodeID = {nodeId}";
            return BaiRongDataProvider.DatabaseDao.GetIntResult(sqlString);
        }

        private int GetTaxis(int typeId)
        {
            string sqlString = $"SELECT Taxis FROM wcm_GovInteractType WHERE TypeID = {typeId}";
            var taxis = 0;

            using (var rdr = ExecuteReader(sqlString))
            {
                if (rdr.Read())
                {
                    taxis = GetInt(rdr, 0);
                }
                rdr.Close();
            }

            return taxis;
        }

        private void SetTaxis(int typeId, int nodeId, int taxis)
        {
            string sqlString =
                $"UPDATE wcm_GovInteractType SET Taxis = {taxis} WHERE TypeID = {typeId} AND NodeID = {nodeId}";
            ExecuteNonQuery(sqlString);
        }
	}
}