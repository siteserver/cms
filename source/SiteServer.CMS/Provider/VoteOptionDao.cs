using System.Collections;
using System.Data;
using BaiRong.Core.Data;
using BaiRong.Core.Model.Enumerations;
using SiteServer.CMS.Model;

namespace SiteServer.CMS.Provider
{
    public class VoteOptionDao : DataProviderBase
	{
        private const string SqlSelectAll = "SELECT OptionID, PublishmentSystemID, NodeID, ContentID, Title, ImageUrl, NavigationUrl, VoteNum FROM siteserver_VoteOption WHERE PublishmentSystemID = @PublishmentSystemID AND NodeID = @NodeID AND ContentID = @ContentID";

        private const string SqlSelectVoteNum = "SELECT SUM(VoteNum) AS TotalNum FROM siteserver_VoteOption WHERE PublishmentSystemID = @PublishmentSystemID AND NodeID = @NodeID AND ContentID = @ContentID";

        private const string SqlDeleteAll = "DELETE FROM siteserver_VoteOption WHERE PublishmentSystemID = @PublishmentSystemID AND NodeID = @NodeID AND ContentID = @ContentID";

        private const string ParmOptionId = "@OptionID";
        private const string ParmPublishmentsystemId = "@PublishmentSystemID";
        private const string ParmNodeId = "@NodeID";
        private const string ParmContentId = "@ContentID";
		private const string ParmTitle = "@Title";
		private const string ParmImageUrl = "@ImageUrl";
		private const string ParmNavigationUrl = "@NavigationUrl";
		private const string ParmVoteNum = "@VoteNum";

		public void Insert(ArrayList voteOptionInfoArrayList)
		{
            var sqlString = "INSERT INTO siteserver_VoteOption (PublishmentSystemID, NodeID, ContentID, Title, ImageUrl, NavigationUrl, VoteNum) VALUES (@PublishmentSystemID, @NodeID, @ContentID, @Title, @ImageUrl, @NavigationUrl, @VoteNum)";

			using (var conn = GetConnection()) 
			{
				conn.Open();
				using (var trans = conn.BeginTransaction()) 
				{
					try 
					{
                        foreach (VoteOptionInfo optionInfo in voteOptionInfoArrayList)
						{
							var insertItemParms = new IDataParameter[]
							{
								GetParameter(ParmPublishmentsystemId, EDataType.Integer, optionInfo.PublishmentSystemID),
                                GetParameter(ParmNodeId, EDataType.Integer, optionInfo.NodeID),
                                GetParameter(ParmContentId, EDataType.Integer, optionInfo.ContentID),
								GetParameter(ParmTitle, EDataType.NVarChar, 255, optionInfo.Title),
								GetParameter(ParmImageUrl, EDataType.VarChar, 200, optionInfo.ImageUrl),
								GetParameter(ParmNavigationUrl, EDataType.VarChar, 200, optionInfo.NavigationUrl),
								GetParameter(ParmVoteNum, EDataType.Integer, optionInfo.VoteNum)
							};

                            ExecuteNonQuery(trans, sqlString, insertItemParms);
						}				

						trans.Commit();
					}
					catch
					{
						trans.Rollback();
						throw;
					}
				}
			}
		}

		public void AddVoteNum(int optionId)
		{
            var sqlString = $"UPDATE siteserver_VoteOption SET {SqlUtils.GetAddOne("VoteNum")} WHERE OptionID = @OptionID";

            var parms = new IDataParameter[]
			{
				GetParameter(ParmOptionId, EDataType.Integer, optionId)
			};
            ExecuteNonQuery(sqlString, parms);
		}

        public void Delete(int publishmentSystemId, int nodeId, int contentId)
		{
			var parms = new IDataParameter[]
			{
				GetParameter(ParmPublishmentsystemId, EDataType.Integer, publishmentSystemId),
                GetParameter(ParmNodeId, EDataType.Integer, nodeId),
                GetParameter(ParmContentId, EDataType.Integer, contentId)
			};

            ExecuteNonQuery(SqlDeleteAll, parms);
		}

        public ArrayList GetVoteOptionInfoArrayList(int publishmentSystemId, int nodeId, int contentId)
		{
            var voteOptionInfoArrayList = new ArrayList();

			var parms = new IDataParameter[]
			{
				GetParameter(ParmPublishmentsystemId, EDataType.Integer, publishmentSystemId),
                GetParameter(ParmNodeId, EDataType.Integer, nodeId),
                GetParameter(ParmContentId, EDataType.Integer, contentId)
			};
			
			using (var rdr = ExecuteReader(SqlSelectAll, parms)) 
			{
				while (rdr.Read())
				{
				    var i = 0;
                    var info = new VoteOptionInfo(GetInt(rdr, i++), GetInt(rdr, i++), GetInt(rdr, i++), GetInt(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetInt(rdr, i));
                    voteOptionInfoArrayList.Add(info);
				}
				rdr.Close();
			}
            return voteOptionInfoArrayList;
		}

        public void UpdateVoteOptionInfoArrayList(int publishmentSystemId, int nodeId, int contentId, ArrayList voteOptionInfoArrayList)
        {
            var oldVoteOptionInfoArrayList = GetVoteOptionInfoArrayList(publishmentSystemId, nodeId, contentId);
            var i = 0;
            foreach (VoteOptionInfo optionInfo in voteOptionInfoArrayList)
            {
                if (i < oldVoteOptionInfoArrayList.Count)
                {
                    var oldOptionInfo = oldVoteOptionInfoArrayList[i++] as VoteOptionInfo;
                    if (oldOptionInfo != null) optionInfo.VoteNum = oldOptionInfo.VoteNum;
                }
            }
            Delete(publishmentSystemId, nodeId, contentId);
            Insert(voteOptionInfoArrayList);
        }

        public int GetTotalVoteNum(int publishmentSystemId, int nodeId, int contentId)
		{
			var totalVoteNum = 0;

			var parms = new IDataParameter[]
			{
				GetParameter(ParmPublishmentsystemId, EDataType.Integer, publishmentSystemId),
                GetParameter(ParmNodeId, EDataType.Integer, nodeId),
                GetParameter(ParmContentId, EDataType.Integer, contentId)
			};

			using (var rdr = ExecuteReader(SqlSelectVoteNum, parms))
			{
				if (rdr.Read())
				{
                    totalVoteNum = GetInt(rdr, 0);
                }
				rdr.Close();
			}
			return totalVoteNum;
		}
	}
}
