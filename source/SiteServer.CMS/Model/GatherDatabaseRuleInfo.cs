using System;

namespace SiteServer.CMS.Model
{
	public class GatherDatabaseRuleInfo
	{
	    public GatherDatabaseRuleInfo()
		{
			GatherRuleName = string.Empty;
			PublishmentSystemId = 0;
			ConnectionString = string.Empty;
			RelatedTableName = string.Empty;
			RelatedIdentity = string.Empty;
			RelatedOrderBy = string.Empty;
            WhereString = string.Empty;
			TableMatchId = 0;
			NodeId = 0;
			GatherNum = 0;
			IsChecked = true;
            IsAutoCreate = false;
            IsOrderByDesc = false;
			LastGatherDate = DateTime.Now;
		}

        public GatherDatabaseRuleInfo(string gatherRuleName, int publishmentSystemId, string connectionString, string relatedTableName, string relatedIdentity, string relatedOrderBy, string whereString, int tableMatchId, int nodeId, int gatherNum, bool isChecked, bool isOrderByDesc, DateTime lastGatherDate, bool isAutoCreate) 
		{
            GatherRuleName = gatherRuleName;
            PublishmentSystemId = publishmentSystemId;
            ConnectionString = connectionString;
            RelatedTableName = relatedTableName;
            RelatedIdentity = relatedIdentity;
            RelatedOrderBy = relatedOrderBy;
            WhereString = whereString;
            TableMatchId = tableMatchId;
            NodeId = nodeId;
            GatherNum = gatherNum;
            IsChecked = isChecked;
            IsAutoCreate = isAutoCreate;
            IsOrderByDesc = isOrderByDesc;
            LastGatherDate = lastGatherDate;
		}

		public string GatherRuleName { get; set; }

	    public int PublishmentSystemId { get; set; }

	    public string ConnectionString { get; set; }

	    public string RelatedTableName { get; set; }

	    public string RelatedIdentity { get; set; }

	    public string RelatedOrderBy { get; set; }

	    public string WhereString { get; set; }

	    public int TableMatchId { get; set; }

	    public int NodeId { get; set; }

	    public int GatherNum { get; set; }

	    public bool IsChecked { get; set; }

	    public bool IsAutoCreate { get; set; }

	    public bool IsOrderByDesc { get; set; }

	    public DateTime LastGatherDate { get; set; }
	}
}
