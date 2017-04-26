using System.Collections;
using System.Data;
using BaiRong.Core.Data;
using BaiRong.Core.Model.Enumerations;

namespace BaiRong.Core.Provider
{
    public class DiggDao : DataProviderBase
	{
        private const string SqlSelectDigg = "SELECT Good, Bad FROM bairong_Digg WHERE PublishmentSystemID = @PublishmentSystemID AND RelatedIdentity = @RelatedIdentity";

        private const string SqlSelectDiggId = "SELECT DiggID FROM bairong_Digg WHERE PublishmentSystemID = @PublishmentSystemID AND RelatedIdentity = @RelatedIdentity";

        private const string SqlUpdateDigg = "UPDATE bairong_Digg SET Good = @Good, Bad = @Bad WHERE PublishmentSystemID = @PublishmentSystemID AND RelatedIdentity = @RelatedIdentity";

        private const string SqlDeleteDigg = "DELETE FROM bairong_Digg WHERE PublishmentSystemID = @PublishmentSystemID AND RelatedIdentity = @RelatedIdentity";

        private const string ParmPublishmentSystemId = "@PublishmentSystemID";
		private const string ParmRelatedIdentity = "@RelatedIdentity";
        private const string ParmGood = "@Good";
        private const string ParmBad = "@Bad";		

		private void Insert(int publishmentSystemId, int relatedIdentity, bool isGood)
		{
            var good = (isGood) ? 1 : 0;
            var bad = (isGood) ? 0 : 1;

            var sqlString = "INSERT INTO bairong_Digg (PublishmentSystemID, RelatedIdentity, Good, Bad) VALUES (@PublishmentSystemID, @RelatedIdentity, @Good, @Bad)";

			var parms = new IDataParameter[]
			{
				GetParameter(ParmPublishmentSystemId, EDataType.Integer, publishmentSystemId),
				GetParameter(ParmRelatedIdentity, EDataType.Integer, relatedIdentity),
				GetParameter(ParmGood, EDataType.Integer, good),
				GetParameter(ParmBad, EDataType.Integer, bad)
			};

            ExecuteNonQuery(sqlString, parms);
		}

        private void Insert(int publishmentSystemId, int relatedIdentity, int goodNum, int badNum)
        {
            var sqlString = "INSERT INTO bairong_Digg (PublishmentSystemID, RelatedIdentity, Good, Bad) VALUES (@PublishmentSystemID, @RelatedIdentity, @Good, @Bad)";

            var parms = new IDataParameter[]
			{
				GetParameter(ParmPublishmentSystemId, EDataType.Integer, publishmentSystemId),
				GetParameter(ParmRelatedIdentity, EDataType.Integer, relatedIdentity),
				GetParameter(ParmGood, EDataType.Integer, goodNum),
				GetParameter(ParmBad, EDataType.Integer, badNum)
			};

            ExecuteNonQuery(sqlString, parms);
        }

        private void Update(int publishmentSystemId, int relatedIdentity, bool isGood)
		{
            var sqlString = isGood ? $"UPDATE bairong_Digg SET {SqlUtils.GetAddOne("Good")} WHERE PublishmentSystemID = @PublishmentSystemID AND RelatedIdentity = @RelatedIdentity" : $"UPDATE bairong_Digg SET {SqlUtils.GetAddOne("Bad")} WHERE PublishmentSystemID = @PublishmentSystemID AND RelatedIdentity = @RelatedIdentity";

            var parms = new IDataParameter[]
			{
				GetParameter(ParmPublishmentSystemId, EDataType.Integer, publishmentSystemId),
				GetParameter(ParmRelatedIdentity, EDataType.Integer, relatedIdentity)
			};

            ExecuteNonQuery(sqlString, parms);
		}

        public void AddCount(int publishmentSystemId, int relatedIdentity, bool isGood)
        {
            var isExists = IsExists(publishmentSystemId, relatedIdentity);
            if (isExists)
            {
                Update(publishmentSystemId, relatedIdentity, isGood);
            }
            else
            {
                Insert(publishmentSystemId, relatedIdentity, isGood);
            }
		}

        public void Delete(int publishmentSystemId, int relatedIdentity)
		{
            var parms = new IDataParameter[]
			{
				GetParameter(ParmPublishmentSystemId, EDataType.Integer, publishmentSystemId),
				GetParameter(ParmRelatedIdentity, EDataType.Integer, relatedIdentity)
			};

            ExecuteNonQuery(SqlDeleteDigg, parms);
		}

        public bool IsExists(int publishmentSystemId, int relatedIdentity)
        {
            var isExists = false;

            var parms = new IDataParameter[]
			{
				GetParameter(ParmPublishmentSystemId, EDataType.Integer, publishmentSystemId),
				GetParameter(ParmRelatedIdentity, EDataType.Integer, relatedIdentity)
			};

            using (var rdr = ExecuteReader(SqlSelectDiggId, parms))
            {
                if (rdr.Read())
                {
                    if (!rdr.IsDBNull(0))
                    {
                        isExists = true;
                    }
                }
                rdr.Close();
            }
            return isExists;
        }

        public int[] GetCount(int publishmentSystemId, int relatedIdentity)
        {
            var good = 0;
            var bad = 0;

            var parms = new IDataParameter[]
			{
				GetParameter(ParmPublishmentSystemId, EDataType.Integer, publishmentSystemId),
				GetParameter(ParmRelatedIdentity, EDataType.Integer, relatedIdentity)
			};

            using (var rdr = ExecuteReader(SqlSelectDigg, parms))
            {
                if (rdr.Read())
                {
                    good = GetInt(rdr, 0);
                    bad = GetInt(rdr, 1);
                }
                rdr.Close();
            }
            return new[] { good, bad };
        }

        public void SetCount(int publishmentSystemId, int relatedIdentity, int goodNum, int badNum)
        {
            if (!IsExists(publishmentSystemId, relatedIdentity))
            {
                Insert(publishmentSystemId, relatedIdentity, goodNum, badNum);
            }
            else
            {
                var parms = new IDataParameter[]
			    {
                    GetParameter(ParmGood, EDataType.Integer, goodNum),
                    GetParameter(ParmBad, EDataType.Integer, badNum),
				    GetParameter(ParmPublishmentSystemId, EDataType.Integer, publishmentSystemId),
				    GetParameter(ParmRelatedIdentity, EDataType.Integer, relatedIdentity)
			    };

                ExecuteNonQuery(SqlUpdateDigg, parms);
            }
        }

        public ArrayList GetRelatedIdentityArrayListByTotal(int publishmentSystemId)
        {
            var arraylist = new ArrayList();

            string sqlString = $@"
SELECT RelatedIdentity, (Good + Bad) AS NUM
FROM bairong_Digg
WHERE (PublishmentSystemID = {publishmentSystemId} AND RelatedIdentity > 0)
GROUP BY RelatedIdentity, Good, Bad
ORDER BY NUM DESC";

            using (var rdr = ExecuteReader(sqlString))
            {
                while (rdr.Read())
                {
                    var relatedIdentity = GetInt(rdr, 0);
                    arraylist.Add(relatedIdentity);
                }
                rdr.Close();
            }

            return arraylist;
        }
	}
}
