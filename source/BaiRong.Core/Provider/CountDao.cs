using System.Data;
using BaiRong.Core.Data;
using BaiRong.Core.Model.Enumerations;

namespace BaiRong.Core.Provider
{
    public class CountDao : DataProviderBase
    {
        // Static constants
        private const string SqlSelectCountNum = "SELECT CountNum FROM bairong_Count WHERE RelatedTableName = @RelatedTableName AND RelatedIdentity = @RelatedIdentity AND CountType = @CountType";

        private const string SqlDeleteByRelatedTableName = "DELETE FROM bairong_Count WHERE RelatedTableName = @RelatedTableName";

        private const string SqlDeleteByIdentity = "DELETE FROM bairong_Count WHERE RelatedTableName = @RelatedTableName AND RelatedIdentity = @RelatedIdentity";

        private const string ParmRelatedTableName = "@RelatedTableName";
        private const string ParmRelatedIdentity = "@RelatedIdentity";
        private const string ParmCountType = "@CountType";
        private const string ParmCountNum = "@CountNum";

        public void Insert(string relatedTableName, string relatedIdentity, ECountType countType, int countNum)
        {
            const string sqlString = "INSERT INTO bairong_Count (RelatedTableName, RelatedIdentity, CountType, CountNum) VALUES (@RelatedTableName, @RelatedIdentity, @CountType, @CountNum)";

            var insertParms = new IDataParameter[]
			{
				GetParameter(ParmRelatedTableName, EDataType.NVarChar, 255, relatedTableName),
				GetParameter(ParmRelatedIdentity, EDataType.NVarChar, 255, relatedIdentity),
				GetParameter(ParmCountType, EDataType.VarChar, 50, ECountTypeUtils.GetValue(countType)),
				GetParameter(ParmCountNum, EDataType.Integer, countNum)
			};

            ExecuteNonQuery(sqlString, insertParms);
        }

        public void AddCountNum(string relatedTableName, string relatedIdentity, ECountType countType)
        {
            var sqlString = $"UPDATE bairong_Count SET {SqlUtils.GetAddOne("CountNum")} WHERE RelatedTableName = @RelatedTableName AND RelatedIdentity = @RelatedIdentity AND CountType = @CountType";

            var insertParms = new IDataParameter[]
			{
				GetParameter(ParmRelatedTableName, EDataType.NVarChar, 255, relatedTableName),
				GetParameter(ParmRelatedIdentity, EDataType.NVarChar, 255, relatedIdentity),
				GetParameter(ParmCountType, EDataType.VarChar, 50, ECountTypeUtils.GetValue(countType)),
			};

            ExecuteNonQuery(sqlString, insertParms);
        }

        public void DeleteByRelatedTableName(string relatedTableName)
        {
            var parms = new IDataParameter[]
			{
				GetParameter(ParmRelatedTableName, EDataType.NVarChar, 255, relatedTableName)
			};

            ExecuteNonQuery(SqlDeleteByRelatedTableName, parms);
        }

        public void DeleteByIdentity(string relatedTableName, string relatedIdentity)
        {
            var parms = new IDataParameter[]
			{
				GetParameter(ParmRelatedTableName, EDataType.NVarChar, 255, relatedTableName),
				GetParameter(ParmRelatedIdentity, EDataType.NVarChar, 255, relatedIdentity),
			};

            ExecuteNonQuery(SqlDeleteByIdentity, parms);
        }

        public bool IsExists(string relatedTableName, string relatedIdentity, ECountType countType)
        {
            if (GetCountNum(relatedTableName, relatedIdentity, countType) == 0)
            {
                return false;
            }
            return true;
        }

        public int GetCountNum(string relatedTableName, string relatedIdentity, ECountType countType)
        {
            var countNum = 0;

            var parms = new IDataParameter[]
			{
				GetParameter(ParmRelatedTableName, EDataType.NVarChar, 255, relatedTableName),
				GetParameter(ParmRelatedIdentity, EDataType.NVarChar, 255, relatedIdentity),
				GetParameter(ParmCountType, EDataType.VarChar, 50, ECountTypeUtils.GetValue(countType))
			};

            using (var rdr = ExecuteReader(SqlSelectCountNum, parms))
            {
                if (rdr.Read())
                {
                    countNum = GetInt(rdr, 0);
                }
                rdr.Close();
            }

            return countNum;
        }

        /// <summary>
        /// 获取站点的统计数据
        /// </summary>
        /// <param name="relatedTableName"></param>
        /// <param name="publishmentSystemId"></param>
        /// <param name="countType"></param>
        /// <returns></returns>
        public int GetCountNum(string relatedTableName, int publishmentSystemId, ECountType countType)
        {
            var countNum = 0;

            string sqlString =
                $@"select sum(cou.CountNum) from bairong_Count cou left join {relatedTableName} con on cou.RelatedIdentity = con.ID
where cou.RelatedTableName = '{relatedTableName}'
and con.PublishmentSystemID = {publishmentSystemId}
and cou.CountType = '{ECountTypeUtils.GetValue(countType)}'";

            using (var rdr = ExecuteReader(sqlString))
            {
                if (rdr.Read())
                {
                    countNum = GetInt(rdr, 0);
                }
                rdr.Close();
            }
            return countNum;
        }
    }
}
