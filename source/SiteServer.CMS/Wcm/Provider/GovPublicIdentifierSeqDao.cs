using BaiRong.Core.Data;

namespace SiteServer.CMS.Wcm.Provider
{
    public class GovPublicIdentifierSeqDao : DataProviderBase
	{
        public int GetSequence(int publishmentSystemId, int nodeId, int departmentId, int addYear, int ruleSequence)
        {
            var sequence = 0;

            string sqlString =
                $"SELECT Sequence FROM wcm_GovPublicIdentifierSeq WHERE PublishmentSystemID = {publishmentSystemId} AND NodeID = {nodeId} AND DepartmentID = {departmentId} AND AddYear = {addYear}";

            using (var rdr = ExecuteReader(sqlString))
            {
                if (rdr.Read())
                {
                    sequence = GetInt(rdr, 0) + 1;
                }
                rdr.Close();
            }

            if (sequence > 0)
            {
                string sqlUpdate =
                    $"UPDATE wcm_GovPublicIdentifierSeq SET Sequence = {sequence} WHERE PublishmentSystemID = {publishmentSystemId} AND NodeID = {nodeId} AND DepartmentID = {departmentId} AND AddYear = {addYear}";
                ExecuteNonQuery(sqlUpdate);
            }
            else
            {
                sequence = ruleSequence;

                string sqlInsert =
                    $"INSERT INTO wcm_GovPublicIdentifierSeq (PublishmentSystemID, NodeID, DepartmentID, AddYear, Sequence) VALUES ({publishmentSystemId}, {nodeId}, {departmentId}, {addYear}, {sequence})";

                ExecuteNonQuery(sqlInsert);

                sequence += 1;
            }

            return sequence;
        }
	}
}