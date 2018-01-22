using System.Collections.Generic;
using System.Data;
using SiteServer.Utils.Model;
using SiteServer.CMS.Core;
using SiteServer.CMS.Data;
using SiteServer.CMS.Model;
using SiteServer.Plugin;

namespace SiteServer.CMS.Provider
{
	public class SeoMetasInNodesDao : DataProviderBase
	{
        public override string TableName => "siteserver_SeoMetasInNodes";

        public override List<TableColumnInfo> TableColumns => new List<TableColumnInfo>
        {
            new TableColumnInfo
            {
                ColumnName = "Id",
                DataType = DataType.Integer,
                IsIdentity = true,
                IsPrimaryKey = true
            },
            new TableColumnInfo
            {
                ColumnName = "NodeId",
                DataType = DataType.Integer
            },
            new TableColumnInfo
            {
                ColumnName = "IsChannel",
                DataType = DataType.VarChar,
                Length = 18
            },
            new TableColumnInfo
            {
                ColumnName = "SeoMetaId",
                DataType = DataType.Integer
            },
            new TableColumnInfo
            {
                ColumnName = "PublishmentSystemId",
                DataType = DataType.Integer
            }
        };

		private const string SqlInsertMatch = "INSERT INTO siteserver_SeoMetasInNodes (NodeID, IsChannel, SeoMetaID, PublishmentSystemID) VALUES (@NodeID, @IsChannel, @SeoMetaID, @PublishmentSystemID)";

		private const string SqlDeleteMatchByNodeId = "DELETE FROM siteserver_SeoMetasInNodes WHERE NodeID = @NodeID AND IsChannel = @IsChannel";

		private const string SqlSelectSeoMetaIdByNodeId = "SELECT SeoMetaID FROM siteserver_SeoMetasInNodes WHERE NodeID = @NodeID AND IsChannel = @IsChannel";

		private const string ParmSeoMetaId = "@SeoMetaID";
		private const string ParmPublishmentSystemId = "@PublishmentSystemID";
		private const string ParmNodeId = "@NodeID";
		private const string ParmIsChannel = "@IsChannel";

        public void InsertMatch(int publishmentSystemId, int nodeId, int seoMetaId, bool isChannel)
		{
			var lastSeoMetaId = GetSeoMetaIdByNodeId(nodeId, isChannel);
			if (lastSeoMetaId != 0)
			{
                DeleteMatch(publishmentSystemId, nodeId, isChannel);
			}

			var insertParms = new IDataParameter[]
			{
				GetParameter(ParmNodeId, DataType.Integer, nodeId),
				GetParameter(ParmIsChannel, DataType.VarChar, 18, isChannel.ToString()),
				GetParameter(ParmSeoMetaId, DataType.Integer, seoMetaId),
				GetParameter(ParmPublishmentSystemId, DataType.Integer, publishmentSystemId),
			};
							
			ExecuteNonQuery(SqlInsertMatch, insertParms);
            SeoManager.RemoveCache(publishmentSystemId);
		}

        public void DeleteMatch(int publishmentSystemId, int nodeId, bool isChannel)
		{
			var parms = new IDataParameter[]
			{
				GetParameter(ParmNodeId, DataType.Integer, nodeId),
				GetParameter(ParmIsChannel, DataType.VarChar, 18, isChannel.ToString()),
			};
							
			ExecuteNonQuery(SqlDeleteMatchByNodeId, parms);
            SeoManager.RemoveCache(publishmentSystemId);
		}


        public int GetSeoMetaIdByNodeId(int nodeId, bool isChannel)
		{
			var seoMetaId = 0;

			var parms = new IDataParameter[]
			{
				GetParameter(ParmNodeId, DataType.Integer, nodeId),
				GetParameter(ParmIsChannel, DataType.VarChar, 18, isChannel.ToString())
			};

			using (var rdr = ExecuteReader(SqlSelectSeoMetaIdByNodeId, parms))
			{
				if (rdr.Read())
				{
                    seoMetaId = GetInt(rdr, 0);
                }
				rdr.Close();
			}

			return seoMetaId;
		}

        public List<int>[] GetSeoMetaLists(int publishmentSystemId)
        {
            var sqlString = "SELECT NodeID, IsChannel FROM siteserver_SeoMetasInNodes WHERE PublishmentSystemID = " + publishmentSystemId;

            var list1 = new List<int>();
            var list2 = new List<int>();

            using (var rdr = ExecuteReader(sqlString))
            {
                while (rdr.Read())
                {
                    var nodeId = GetInt(rdr, 0);
                    var isChannel = GetBool(rdr, 1);

                    if (isChannel)
                    {
                        if (!list1.Contains(nodeId))
                        {
                            list1.Add(nodeId);
                        }
                    }
                    else
                    {
                        if (!list2.Contains(nodeId))
                        {
                            list2.Add(nodeId);
                        }
                    }
                }
                rdr.Close();
            }

            return new[] { list1, list2 };
        }

	}
}
