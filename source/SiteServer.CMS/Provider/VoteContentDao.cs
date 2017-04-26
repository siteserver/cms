using BaiRong.Core;
using BaiRong.Core.Data;
using BaiRong.Core.Model.Enumerations;
using SiteServer.CMS.Model;

namespace SiteServer.CMS.Provider
{
    public class VoteContentDao : DataProviderBase
	{
        public VoteContentInfo GetContentInfo(PublishmentSystemInfo publishmentSystemInfo, int contentId)
        {
            VoteContentInfo info = null;
            if (contentId > 0)
            {
                if (!string.IsNullOrEmpty(publishmentSystemInfo.AuxiliaryTableForVote))
                {
                    string sqlWhere = $"WHERE ID = {contentId}";
                    var sqlSelect = BaiRongDataProvider.TableStructureDao.GetSelectSqlString(publishmentSystemInfo.AuxiliaryTableForVote, SqlUtils.Asterisk, sqlWhere);

                    using (var rdr = ExecuteReader(sqlSelect))
                    {
                        if (rdr.Read())
                        {
                            info = new VoteContentInfo();
                            BaiRongDataProvider.DatabaseDao.ReadResultsToExtendedAttributes(rdr, info);
                        }
                        rdr.Close();
                    }
                }
            }

            if (info != null) info.AfterExecuteReader();
            return info;
        }

        public int GetContentNum(PublishmentSystemInfo publishmentSystemInfo)
        {
            string sqlString =
                $"SELECT COUNT(*) AS TotalNum FROM {publishmentSystemInfo.AuxiliaryTableForVote} WHERE PublishmentSystemID = {publishmentSystemInfo.PublishmentSystemId}";

            return BaiRongDataProvider.DatabaseDao.GetIntResult(sqlString);
        }

        public string GetSelectCommendByNodeId(PublishmentSystemInfo publishmentSystemInfo, int nodeId)
        {
            return BaiRongDataProvider.ContentDao.GetSelectCommend(publishmentSystemInfo.AuxiliaryTableForVote, nodeId, ETriState.All);
        }
	}
}
