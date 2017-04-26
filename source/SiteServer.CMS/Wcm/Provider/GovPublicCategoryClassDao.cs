using System.Collections;
using System.Data;
using BaiRong.Core;
using BaiRong.Core.Data;
using BaiRong.Core.Model.Attributes;
using BaiRong.Core.Model.Enumerations;
using SiteServer.CMS.Model.Enumerations;
using SiteServer.CMS.Wcm.Model;

namespace SiteServer.CMS.Wcm.Provider
{
    public class GovPublicCategoryClassDao : DataProviderBase
	{
        private const string SqlInsert = "INSERT INTO wcm_GovPublicCategoryClass (ClassCode, PublishmentSystemID, ClassName, IsSystem, IsEnabled, ContentAttributeName, Taxis, Description) VALUES (@ClassCode, @PublishmentSystemID, @ClassName, @IsSystem, @IsEnabled, @ContentAttributeName, @Taxis, @Description)";

        private const string SqlUpdate = "UPDATE wcm_GovPublicCategoryClass SET ClassName = @ClassName, IsEnabled = @IsEnabled, Description = @Description WHERE ClassCode = @ClassCode AND PublishmentSystemID = @PublishmentSystemID";

        private const string SqlDelete = "DELETE FROM wcm_GovPublicCategoryClass WHERE ClassCode = @ClassCode AND PublishmentSystemID = @PublishmentSystemID";

        private const string SqlSelect = "SELECT ClassCode, PublishmentSystemID, ClassName, IsSystem, IsEnabled, ContentAttributeName, Taxis, Description FROM wcm_GovPublicCategoryClass WHERE ClassCode = @ClassCode AND PublishmentSystemID = @PublishmentSystemID";

        private const string SqlSelectAll = "SELECT ClassCode, PublishmentSystemID, ClassName, IsSystem, IsEnabled, ContentAttributeName, Taxis, Description FROM wcm_GovPublicCategoryClass WHERE PublishmentSystemID = @PublishmentSystemID ORDER BY Taxis";

        private const string SqlSelectClassCode = "SELECT ClassCode FROM wcm_GovPublicCategoryClass WHERE PublishmentSystemID = @PublishmentSystemID ORDER BY Taxis";

        private const string SqlSelectClassName = "SELECT ClassName FROM wcm_GovPublicCategoryClass WHERE PublishmentSystemID = @PublishmentSystemID ORDER BY Taxis";

        private const string ParmClassCode = "@ClassCode";
        private const string ParmPublishmentsystemid = "@PublishmentSystemID";
        private const string ParmClassName = "@ClassName";
        private const string ParmIsSystem = "@IsSystem";
        private const string ParmIsEnabled = "@IsEnabled";
        private const string ParmContentAttributeName = "@ContentAttributeName";
        private const string ParmTaxis = "@Taxis";
        private const string ParmDescription = "@Description";

        private string GetContentAttributeNameNotUsed(int publishmentSystemId)
        {
            var contentAttributeName = string.Empty;

            for (var i = 1; i <= 6; i++)
            {
                string sqlString =
                    $"SELECT ContentAttributeName FROM wcm_GovPublicCategoryClass WHERE PublishmentSystemID = {publishmentSystemId} AND  ContentAttributeName = 'Category{i}ID'";

                using (var rdr = ExecuteReader(sqlString))
                {
                    if (!rdr.Read())
                    {
                        contentAttributeName = $"Category{i}ID";
                    }
                    rdr.Close();
                }
            }

            return contentAttributeName;
        }

		public void Insert(GovPublicCategoryClassInfo categoryClassInfo) 
		{
            if (categoryClassInfo.IsSystem)
            {
                if (EGovPublicCategoryClassTypeUtils.Equals(EGovPublicCategoryClassType.Channel, categoryClassInfo.ClassCode))
                {
                    categoryClassInfo.ContentAttributeName = ContentAttribute.NodeId;
                }
                else if (EGovPublicCategoryClassTypeUtils.Equals(EGovPublicCategoryClassType.Department, categoryClassInfo.ClassCode))
                {
                    categoryClassInfo.ContentAttributeName = GovPublicContentAttribute.DepartmentId;
                }
            }
            else
            {
                categoryClassInfo.ContentAttributeName = GetContentAttributeNameNotUsed(categoryClassInfo.PublishmentSystemID);
            }
            var taxis = GetMaxTaxis(categoryClassInfo.PublishmentSystemID) + 1;
			var parms = new IDataParameter[]
			{
				GetParameter(ParmClassCode, EDataType.NVarChar, 50, categoryClassInfo.ClassCode),
				GetParameter(ParmPublishmentsystemid, EDataType.Integer, categoryClassInfo.PublishmentSystemID),
                GetParameter(ParmClassName, EDataType.NVarChar, 255, categoryClassInfo.ClassName),
                GetParameter(ParmIsSystem, EDataType.VarChar, 18, categoryClassInfo.IsSystem.ToString()),
                GetParameter(ParmIsEnabled, EDataType.VarChar, 18, categoryClassInfo.IsEnabled.ToString()),
                GetParameter(ParmContentAttributeName, EDataType.VarChar, 50, categoryClassInfo.ContentAttributeName),
                GetParameter(ParmTaxis, EDataType.Integer, taxis),
                GetParameter(ParmDescription, EDataType.NVarChar, 255, categoryClassInfo.Description)
			};

            ExecuteNonQuery(SqlInsert, parms);
		}

        public void Update(GovPublicCategoryClassInfo categoryClassInfo) 
		{
			var parms = new IDataParameter[]
			{
                GetParameter(ParmClassName, EDataType.NVarChar, 255, categoryClassInfo.ClassName),
                GetParameter(ParmIsEnabled, EDataType.VarChar, 18, categoryClassInfo.IsEnabled.ToString()),
                GetParameter(ParmDescription, EDataType.NVarChar, 255, categoryClassInfo.Description),
                GetParameter(ParmClassCode, EDataType.NVarChar, 50, categoryClassInfo.ClassCode),
				GetParameter(ParmPublishmentsystemid, EDataType.Integer, categoryClassInfo.PublishmentSystemID),
			};

            ExecuteNonQuery(SqlUpdate, parms);
		}

		public void Delete(string classCode, int publishmentSystemId)
		{
            var parms = new IDataParameter[]
			{
                GetParameter(ParmClassCode, EDataType.NVarChar, 50, classCode),
				GetParameter(ParmPublishmentsystemid, EDataType.Integer, publishmentSystemId)
			};

            ExecuteNonQuery(SqlDelete, parms);
		}

        public GovPublicCategoryClassInfo GetCategoryClassInfo(string classCode, int publishmentSystemId)
		{
            GovPublicCategoryClassInfo categoryClassInfo = null;

            var parms = new IDataParameter[]
			{
				GetParameter(ParmClassCode, EDataType.NVarChar, 50, classCode),
				GetParameter(ParmPublishmentsystemid, EDataType.Integer, publishmentSystemId)
			};

            using (var rdr = ExecuteReader(SqlSelect, parms))
            {
                if (rdr.Read())
                {
                    var i = 0;
                    categoryClassInfo = new GovPublicCategoryClassInfo(GetString(rdr, i++), GetInt(rdr, i++), GetString(rdr, i++), GetBool(rdr, i++), GetBool(rdr, i++), GetString(rdr, i++), GetInt(rdr, i++), GetString(rdr, i));
                }
                rdr.Close();
            }

            return categoryClassInfo;
		}

        public string GetContentAttributeName(string classCode, int publishmentSystemId)
        {
            var contentAttributeName = string.Empty;

            string sqlString =
                $"SELECT ContentAttributeName FROM wcm_GovPublicCategoryClass WHERE PublishmentSystemID = {publishmentSystemId} AND  ClassCode = '{classCode}'";

            using (var rdr = ExecuteReader(sqlString))
            {
                if (rdr.Read())
                {
                    contentAttributeName = GetString(rdr, 0);
                }
                rdr.Close();
            }

            return contentAttributeName;
        }

        public bool IsExists(string classCode, int publishmentSystemId)
		{
			var exists = false;

            var parms = new IDataParameter[]
			{
				GetParameter(ParmClassCode, EDataType.NVarChar, 50, classCode),
				GetParameter(ParmPublishmentsystemid, EDataType.Integer, publishmentSystemId)
			};

            using (var rdr = ExecuteReader(SqlSelect, parms))
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
            var enumerable = (IEnumerable)ExecuteReader(SqlSelectAll, parms);
			return enumerable;
		}

        public int GetCount(int publishmentSystemId)
        {
            string sqlString =
                $"SELECT COUNT(*) FROM wcm_GovPublicCategoryClass WHERE PublishmentSystemID = {publishmentSystemId}";

            return BaiRongDataProvider.DatabaseDao.GetIntResult(sqlString);
        }

        public ArrayList GetCategoryClassInfoArrayList(int publishmentSystemId, ETriState isSystem, ETriState isEnabled)
        {
            var arraylist = new ArrayList();

            var selectParms = new IDataParameter[]
			{
				GetParameter(ParmPublishmentsystemid, EDataType.Integer, publishmentSystemId)
			};

            using (var rdr = ExecuteReader(SqlSelectAll, selectParms))
            {
                while (rdr.Read())
                {
                    var i = 0;
                    var categoryClassInfo = new GovPublicCategoryClassInfo(GetString(rdr, i++), GetInt(rdr, i++), GetString(rdr, i++), GetBool(rdr, i++), GetBool(rdr, i++), GetString(rdr, i++), GetInt(rdr, i++), GetString(rdr, i));
                    if (isSystem == ETriState.False)
                    {
                        if (categoryClassInfo.IsSystem) continue;
                    }
                    else if (isSystem == ETriState.True)
                    {
                        if (!categoryClassInfo.IsSystem) continue;
                    }
                    if (isEnabled == ETriState.False)
                    {
                        if (categoryClassInfo.IsEnabled) continue;
                    }
                    else if (isEnabled == ETriState.True)
                    {
                        if (!categoryClassInfo.IsEnabled) continue;
                    }
                    arraylist.Add(categoryClassInfo);
                }
                rdr.Close();
            }

            return arraylist;
        }

        public ArrayList GetClassCodeArrayList(int publishmentSystemId)
        {
            var arraylist = new ArrayList();

            var selectParms = new IDataParameter[]
			{
				GetParameter(ParmPublishmentsystemid, EDataType.Integer, publishmentSystemId)
			};

            using (var rdr = ExecuteReader(SqlSelectClassCode, selectParms))
            {
                while (rdr.Read())
                {
                    arraylist.Add(GetString(rdr, 0));
                }
                rdr.Close();
            }

            return arraylist;
        }

        public ArrayList GetClassNameArrayList(int publishmentSystemId)
        {
            var arraylist = new ArrayList();

            var selectParms = new IDataParameter[]
			{
				GetParameter(ParmPublishmentsystemid, EDataType.Integer, publishmentSystemId)
			};

            using (var rdr = ExecuteReader(SqlSelectClassName, selectParms))
            {
                while (rdr.Read())
                {
                    arraylist.Add(GetString(rdr, 0));
                }
                rdr.Close();
            }

            return arraylist;
        }

        public bool UpdateTaxisToUp(string classCode, int publishmentSystemId)
        {
            //string sqlString =
            //    $"SELECT TOP 1 ClassCode, Taxis FROM wcm_GovPublicCategoryClass WHERE ((Taxis > (SELECT Taxis FROM wcm_GovPublicCategoryClass WHERE ClassCode = '{classCode}' AND PublishmentSystemID = {publishmentSystemId})) AND PublishmentSystemID ={publishmentSystemId}) ORDER BY Taxis";
            var sqlString = SqlUtils.GetTopSqlString("wcm_GovPublicCategoryClass", "ClassCode, Taxis", $"WHERE ((Taxis > (SELECT Taxis FROM wcm_GovPublicCategoryClass WHERE ClassCode = '{classCode}' AND PublishmentSystemID = {publishmentSystemId})) AND PublishmentSystemID ={publishmentSystemId}) ORDER BY Taxis", 1);

            var higherClassCode = string.Empty;
            var higherTaxis = 0;

            using (var rdr = ExecuteReader(sqlString))
            {
                if (rdr.Read())
                {
                    higherClassCode = GetString(rdr, 0);
                    higherTaxis = GetInt(rdr, 1);
                }
                rdr.Close();
            }

            var selectedTaxis = GetTaxis(classCode, publishmentSystemId);

            if (!string.IsNullOrEmpty(higherClassCode))
            {
                SetTaxis(classCode, publishmentSystemId, higherTaxis);
                SetTaxis(higherClassCode, publishmentSystemId, selectedTaxis);
                return true;
            }
            return false;
        }

        public bool UpdateTaxisToDown(string classCode, int publishmentSystemId)
        {
            //string sqlString =
            //    $"SELECT TOP 1 ClassCode, Taxis FROM wcm_GovPublicCategoryClass WHERE ((Taxis < (SELECT Taxis FROM wcm_GovPublicCategoryClass WHERE ClassCode = '{classCode}' AND PublishmentSystemID = {publishmentSystemId})) AND PublishmentSystemID = {publishmentSystemId}) ORDER BY Taxis DESC";
            var sqlString = SqlUtils.GetTopSqlString("wcm_GovPublicCategoryClass", "ClassCode, Taxis", $"WHERE ((Taxis < (SELECT Taxis FROM wcm_GovPublicCategoryClass WHERE ClassCode = '{classCode}' AND PublishmentSystemID = {publishmentSystemId})) AND PublishmentSystemID = {publishmentSystemId}) ORDER BY Taxis DESC", 1);

            var lowerClassCode = string.Empty;
            var lowerTaxis = 0;

            using (var rdr = ExecuteReader(sqlString))
            {
                if (rdr.Read())
                {
                    lowerClassCode = GetString(rdr, 0);
                    lowerTaxis = GetInt(rdr, 1);
                }
                rdr.Close();
            }

            var selectedTaxis = GetTaxis(classCode, publishmentSystemId);

            if (!string.IsNullOrEmpty(lowerClassCode))
            {
                SetTaxis(classCode, publishmentSystemId, lowerTaxis);
                SetTaxis(lowerClassCode, publishmentSystemId, selectedTaxis);
                return true;
            }
            return false;
        }

        private int GetMaxTaxis(int publishmentSystemId)
        {
            string sqlString =
                $"SELECT MAX(Taxis) FROM wcm_GovPublicCategoryClass WHERE PublishmentSystemID = {publishmentSystemId}";
            return BaiRongDataProvider.DatabaseDao.GetIntResult(sqlString);
        }

        private int GetTaxis(string classCode, int publishmentSystemId)
        {
            string sqlString =
                $"SELECT Taxis FROM wcm_GovPublicCategoryClass WHERE ClassCode = '{classCode}' AND PublishmentSystemID = {publishmentSystemId}";
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

        private void SetTaxis(string classCode, int publishmentSystemId, int taxis)
        {
            string sqlString =
                $"UPDATE wcm_GovPublicCategoryClass SET Taxis = {taxis} WHERE ClassCode = '{classCode}' AND PublishmentSystemID = {publishmentSystemId}";
            ExecuteNonQuery(sqlString);
        }
	}
}